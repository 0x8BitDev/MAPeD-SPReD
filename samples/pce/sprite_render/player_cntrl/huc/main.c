//#####################################################################
//
// Copyright 2021-2022 0x8BitDev ( MIT license. See LICENSE.txt )
//
// DESC: Simple character controller with a big meta-sprite character,
//	 dynamic sprite data and optional double-buffering (change
//	 'DEF_SG_DBL_BUFF' and rebuild the sample);
//	 Controls: LEFT, RIGHT, UP
//
//#####################################################################

// debug info:
// - pink border color - ROM-VRAM data copying
// - white border color - spd_SATB_push_sprite
#asm
SPD_DEBUG
#endasm

#include <huc.h>
#include "../../../common/spd.h"
#include "player_gfx.h"

// enable double-buffering for SG data
#define DEF_SG_DBL_BUFF	0


#if DEF_SG_DBL_BUFF
unsigned short  LAST_DBL_BUFF_IND	= SPD_DBL_BUFF_INIT_VAL;
#else
/* variables for delayed use of SG data */
unsigned short SG_DATA_SRC_ADDR 	= 0;
unsigned short SG_DATA_SRC_BANK 	= 0;
unsigned short SG_DATA_DST_ADDR 	= 0;
unsigned short SG_DATA_LEN		= 0;
#endif

unsigned char SPR_PUSH_RES;

/* animation data */
typedef struct
{
	unsigned char	curr_tick;
	unsigned char	curr_frame;

	unsigned char	anm_desc_offs;	// offset in player_anims array
} rt_anim_desc;

/* all possible player states */
#define	PLAYER_STATE_IDLE	1
#define	PLAYER_STATE_WALK	2
#define	PLAYER_STATE_KICK	3
#define	PLAYER_MAX_STATES	3	// idle, walk, kick

#define	PLAYER_DIR_RIGHT	0x80
#define	PLAYER_DIR_LEFT		0x00
#define	PLAYER_DIR_MASK		0x80

/* data offset in player_anims array */
#define ANM_OFFS_NUM_TICKS	0	// number of updates to change a frame
#define ANM_OFFS_NUM_FRAMES	1	// number of frames
#define ANM_OFFS_LOOP_FRAME	2
#define ANM_OFFS_START_FRAME	3	// start frame index
#define ANM_DATA_SIZE		4	// animation description data size

/* 
 for adding a new animation state, just add an animation description to 'player_anims' and a constant
 'PLAYER_STATE_<name>' and that animation will be automatically applied on 'player_apply_anim_by_state'
 */
const unsigned char player_anims[] = 
{
	10,1, 0, SPR_MPWR_IDLE01_LEFT,	// idle left
	7, 6, 0, SPR_MPWR_WALK01_LEFT,	// move left
	9, 5, 0, SPR_MPWR_KICK01_LEFT,	// kick left

	10,1, 0, SPR_MPWR_IDLE01_RIGHT,	// idle right
	7, 6, 0, SPR_MPWR_WALK01_RIGHT,	// move right
	9, 5, 0, SPR_MPWR_KICK01_RIGHT	// kick right
};

/* player animation state */
static rt_anim_desc	player_anim	= { 0, 0, 0 };

/* player parameters */
static short player_pos_x	= 118;
static short player_pos_y	= 176;
static short player_state	= 0;

const unsigned char move_step	= 2;	// pix/frame

#define	PLAYER_SET_STATE( _val )	player_state = ( ( player_state & PLAYER_DIR_MASK ) | _val )
#define	PLAYER_SET_DIR( _val )		player_state = ( ( player_state & ~PLAYER_DIR_MASK ) | _val )

/* update character controller logic */
void	update_player_controller()
{
	if( joy(0) & JOY_UP )
	{
		if( ( player_state & ~PLAYER_DIR_MASK ) != PLAYER_STATE_KICK )
		{
			PLAYER_SET_STATE( PLAYER_STATE_KICK );
			player_apply_anim_by_state();
		}
	}
	else
	if( joy(0) & JOY_RIGHT )
	{
		player_pos_x += move_step;

		if( player_pos_x >= 224 )
		{
			player_pos_x = 224;
		}

		if( player_state != ( PLAYER_DIR_RIGHT | PLAYER_STATE_WALK ) )
		{
			PLAYER_SET_STATE( PLAYER_STATE_WALK );
			PLAYER_SET_DIR( PLAYER_DIR_RIGHT );
			player_apply_anim_by_state();
		}
	}
	else
	if( joy(0) & JOY_LEFT )
	{
		player_pos_x -= move_step;

		if( player_pos_x <= 32 )
		{
			player_pos_x = 32;
		}

		if( player_state != ( PLAYER_DIR_LEFT | PLAYER_STATE_WALK ) )
		{
			PLAYER_SET_STATE( PLAYER_STATE_WALK );
			PLAYER_SET_DIR( PLAYER_DIR_LEFT );
			player_apply_anim_by_state();
		}
	}
	else
	{
		PLAYER_SET_STATE( PLAYER_STATE_IDLE );
		player_apply_anim_by_state();
	}

	// update current animation sequence
	update_frame( &player_anim );
}

/* animation init by state flags */
void	player_apply_anim_by_state()
{
	unsigned char anm_ind;

	anm_ind = ( player_state & PLAYER_DIR_MASK ) ? ( player_state - PLAYER_DIR_RIGHT + ( PLAYER_MAX_STATES - 1 ) ):( player_state - 1 );

	player_anim.anm_desc_offs = anm_ind << 2; // anm_ind * ANM_DATA_SIZE (4)

	player_anim.curr_tick = player_anim.curr_frame = 0;
}

/* update animation sequence */
void	update_frame( rt_anim_desc* _anm_desc )
{
	if( ++_anm_desc->curr_tick == player_anims[ _anm_desc->anm_desc_offs + ANM_OFFS_NUM_TICKS ] )
	{
		_anm_desc->curr_tick = 0;

		if( ++_anm_desc->curr_frame == player_anims[ _anm_desc->anm_desc_offs + ANM_OFFS_NUM_FRAMES ] )
		{
			_anm_desc->curr_frame = player_anims[ _anm_desc->anm_desc_offs + ANM_OFFS_LOOP_FRAME ];
		}
	}
}

/* exported sprite set initialization */
void	sprite_set_init()
{
	// load palette in the usual way
	load_palette( PLAYER_GFX_PALETTE_SLOT, player_gfx_palette, PLAYER_GFX_PALETTE_SIZE );

	// set up exported sprite set with SG data array and VRAM address to load SG data to.
#if	DEF_SG_DBL_BUFF
	// NOTE: using the `SPD_FLAG_DBL_BUFF` flag means double-buffering for sprite graphics.
	//	 it costs x2 of dynamic SG data in VRAM, but glitches free. you have to compare the results
	//	 of using 'SPD_FLAG_DBL_BUFF' and 'SPD_FLAG_PEND_SG_DATA' and decide which is better in your case.
	//	 THIS FLAG CAN BE USED WITH UNPACKED SPRITES ONLY! WHERE EACH SPRITE HAS A SEPARATE SG DATA!
	// NOTE: passing '_last_bank_ind' allows to avoid loading SG data to VRAM twice when you are switching back from another data set.
	//	 the last value can be obtained using 'spd_SG_bank_get_ind()'. the initial value is '0xff'.
	spd_sprite_params( player_gfx_SG_arr, PLAYER_GFX_SPR_VADDR, SPD_FLAG_DBL_BUFF, 0xff );

	// set the second VRAM address for double-buffering (SPD_FLAG_DBL_BUFF)
	// and the last double-buffer index value (initial value is 'SPD_DBL_BUFF_INIT_VAL').
	spd_dbl_buff_VRAM_addr( 0x2800, LAST_DBL_BUFF_IND );
#else
	// NOTE: using the `SPD_FLAG_PEND_SG_DATA` flag means that SG data will not be loaded
	//	 to VRAM automatically. You should do that manually on VBLANK.
	//	 THIS FLAG CAN BE USED WITH UNPACKED SPRITES ONLY! WHERE EACH SPRITE HAS A SEPARATE SG DATA!
	spd_sprite_params( player_gfx_SG_arr, PLAYER_GFX_SPR_VADDR, SPD_FLAG_PEND_SG_DATA, 0xff );

	// set pointers to SG data for delayed use (SPD_FLAG_PEND_SG_DATA).
	spd_SG_data_params( &SG_DATA_SRC_ADDR, &SG_DATA_SRC_BANK, &SG_DATA_DST_ADDR, &SG_DATA_LEN );
#endif
}

/* show sprite by an index */
unsigned char	sprite_show( char _ind, short _x, short _y )
{
	unsigned char res;

	// NOTE: Use sprite pushing by index ONLY, if double buffering is enabled (!)
	res = spd_SATB_push_sprite( player_gfx_frames_data, _ind, _x, _y );

#if	DEF_SG_DBL_BUFF
	// save the last double-buffer index
	LAST_DBL_BUFF_IND = spd_get_dbl_buff_ind();
#endif

	return res;
}

main()
{

#asm
.ifdef	SPD_DEBUG
#endasm
	// make the screen a little smaller so that the border color is visible
	set_xres( 252, XRES_SOFT );
#asm
.endif
#endasm

	/* disable display */
	disp_off();

	/* clear display */
	cls();

	/* SPD-render initialization */
	spd_init();

	/* exported sprite set initialization */
	sprite_set_init();

	/* enable display */
	disp_on();

	/* 'VRAM-SATB Transfer Auto-Repeat on VBLANK' (DCR ROF-$10) enabled by default in HuC at startup, so we skip this step. */

	/* default font for displaying information */
	load_default_font();

	/* demo main loop */
	for( ;; )
	{
		// update character controller logic
		update_player_controller();

		// clear local SATB
		reset_satb();

		// reset sprite position in local SATB
		spd_SATB_set_pos( 0 );

		// push current sprite to local SATB
		SPR_PUSH_RES = sprite_show( player_anims[ player_anim.anm_desc_offs + ANM_OFFS_START_FRAME ] + player_anim.curr_frame, player_pos_x, player_pos_y );

		// load all sprites to VRAM SAT
		satb_update( spd_SATB_get_pos() );

		vsync();

#if	DEF_SG_DBL_BUFF
		put_string( "DBL-BUFF", 0, 0 );
#else
		// the HuC doesn't allow to handle VBLANK directly, so we use 'vsync' instead for sprite data and SATB synchronization
		// this may cause some graphical glitches at the upper part of the screen

		// delayed copying of SG data to VRAM to synchronize it with the inner SATB
		if( SPR_PUSH_RES & SPD_SG_NEW_DATA )
		{
			spd_copy_SG_data_to_VRAM( SG_DATA_SRC_ADDR, SG_DATA_SRC_BANK, SG_DATA_DST_ADDR, SG_DATA_LEN );
		}

		put_number( ( SG_DATA_LEN << 1 ), 5, 10, 0 );
		put_string( "ROM->VRAM:", 0, 0 );
#endif
	}
}