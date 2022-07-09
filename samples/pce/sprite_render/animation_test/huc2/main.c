//###############################################################################
//
// Copyright 2021-2022 0x8BitDev ( MIT license. See LICENSE.txt )
//
// DESC: Animation demo that shows 3 independent, dynamic meta-sprite sets 
//	 with optional double-buffering (see 'DEF_SET<N>_SG_DBL_BUFF' flags)
//
//###############################################################################

// debug info:
// - pink border color - ROM-VRAM data copying
// - white border color - spd_SATB_push_sprite
#asm
SPD_DEBUG
#endasm

#include <huc.h>
#include "../../../common/spd.h"
#include "set1.h"
#include "set2.h"
#include "set3.h"


/* single-/double-buffering mode flags */
#define	DEF_SET1_SG_DBL_BUFF 1
#define	DEF_SET2_SG_DBL_BUFF 1
#define	DEF_SET3_SG_DBL_BUFF 1


#if DEF_SET1_SG_DBL_BUFF
unsigned short  SET1_LAST_DBL_BUFF_IND	= SPD_DBL_BUFF_INIT_VAL;
#else
/* variables for delayed use of SG data */
unsigned short	SET1_SG_DATA_SRC_ADDR 	= 0;
unsigned char	SET1_SG_DATA_SRC_BANK 	= 0;
unsigned short	SET1_SG_DATA_DST_ADDR 	= 0;
unsigned short	SET1_SG_DATA_LEN	= 0;
#endif

#if DEF_SET2_SG_DBL_BUFF
unsigned short  SET2_LAST_DBL_BUFF_IND	= SPD_DBL_BUFF_INIT_VAL;
#else
unsigned short	SET2_SG_DATA_SRC_ADDR 	= 0;
unsigned char 	SET2_SG_DATA_SRC_BANK 	= 0;
unsigned short	SET2_SG_DATA_DST_ADDR 	= 0;
unsigned short	SET2_SG_DATA_LEN	= 0;
#endif

#if DEF_SET3_SG_DBL_BUFF
unsigned short  SET3_LAST_DBL_BUFF_IND	= SPD_DBL_BUFF_INIT_VAL;
#else
unsigned short	SET3_SG_DATA_SRC_ADDR 	= 0;
unsigned char	SET3_SG_DATA_SRC_BANK 	= 0;
unsigned short	SET3_SG_DATA_DST_ADDR 	= 0;
unsigned short	SET3_SG_DATA_LEN	= 0;
#endif

unsigned char SET1_LAST_SG_BANK	= 0xff;
unsigned char SET2_LAST_SG_BANK	= 0xff;
unsigned char SET3_LAST_SG_BANK	= 0xff;

unsigned char SET1_SPR_PUSH_RES;
unsigned char SET2_SPR_PUSH_RES;
unsigned char SET3_SPR_PUSH_RES;

/* animation sequence description */
typedef struct
{
	unsigned char	curr_tick;
	unsigned char	curr_frame;

	unsigned char	num_ticks;	// number of frame updates to change an animation frame
	unsigned char	num_frames;	// number of animated frames
	unsigned char	loop_frame;
	unsigned char	start_frame;	// start frame index
} anim_desc;

/* our test animations */
static anim_desc	test_anim1	= { 0, 0, 8, 12, 0, 0 };
static anim_desc	test_anim2	= { 2, 0, 8, 6,  0, 0 };
static anim_desc	test_anim3	= { 4, 6, 8, 12, 0, 0 };

static short		y_pos		= 176;
static unsigned char	dir_trigger	= 0;

/* show sprite by input index */
void	sprite1_show( char _ind, short _x, short _y )
{
	// set up exported sprite set with SG data array and VRAM address to load SG data to.
	// NOTE: you can combine any number of exported sprite sets in your program.
	//	 call the 'spd_sprite_params' to switch between them.
#if	DEF_SET1_SG_DBL_BUFF
	// NOTE: using the `SPD_FLAG_DBL_BUFF` flag means double-buffering for sprite graphics.
	//	 it costs x2 of dynamic SG data in VRAM, but glitches free. you have to compare the results
	//	 of using 'SPD_FLAG_DBL_BUFF' and 'SPD_FLAG_PEND_SG_DATA' and decide which is better in your case.
	//	 THIS FLAG CAN BE USED WITH UNPACKED SPRITES ONLY! WHERE EACH SPRITE HAS A SEPARATE SG DATA!
	// NOTE: passing '_last_bank_ind' allows to avoid loading SG data to VRAM twice when you are switching back from another data set.
	//	 the last value can be obtained using 'spd_SG_bank_get_ind()'. the initial value is '0xff'.
	spd_sprite_params( set1_SG_arr, SET1_SPR_VADDR, SPD_FLAG_DBL_BUFF, SET1_LAST_SG_BANK );

	// set the second VRAM address for double-buffering (SPD_FLAG_DBL_BUFF)
	// and the last double-buffer index value (initial value is 'SPD_DBL_BUFF_INIT_VAL').
	spd_dbl_buff_VRAM_addr( SET1_SPR_VADDR + 0x800, SET1_LAST_DBL_BUFF_IND );
#else
	//	 THIS FLAG CAN BE USED WITH UNPACKED SPRITES ONLY! WHERE EACH SPRITE HAS A SEPARATE SG DATA!
	spd_sprite_params( set1_SG_arr, SET1_SPR_VADDR, SPD_FLAG_PEND_SG_DATA, SET1_LAST_SG_BANK );

	// set pointers to SG data for delayed use (SPD_FLAG_PEND_SG_DATA).
	spd_SG_data_params( &SET1_SG_DATA_SRC_ADDR, &SET1_SG_DATA_SRC_BANK, &SET1_SG_DATA_DST_ADDR, &SET1_SG_DATA_LEN );
#endif

	// NOTE: Use sprite pushing by index ONLY, if double buffering is enabled (!)
	SET1_SPR_PUSH_RES = spd_SATB_push_sprite( set1_frames_data, _ind, _x, _y );

#if	DEF_SET1_SG_DBL_BUFF
	// save the last double-buffer index
	SET1_LAST_DBL_BUFF_IND = spd_get_dbl_buff_ind();
#endif

	SET1_LAST_SG_BANK = spd_SG_bank_get_ind();
}

void	sprite2_show( char _ind, short _x, short _y )
{
#if	DEF_SET2_SG_DBL_BUFF
	spd_sprite_params( set2_SG_arr, SET2_SPR_VADDR, SPD_FLAG_DBL_BUFF, SET2_LAST_SG_BANK );
	spd_dbl_buff_VRAM_addr( SET2_SPR_VADDR + 0x800, SET2_LAST_DBL_BUFF_IND );
#else
	spd_sprite_params( set2_SG_arr, SET2_SPR_VADDR, SPD_FLAG_PEND_SG_DATA, SET2_LAST_SG_BANK );
	spd_SG_data_params( &SET2_SG_DATA_SRC_ADDR, &SET2_SG_DATA_SRC_BANK, &SET2_SG_DATA_DST_ADDR, &SET2_SG_DATA_LEN );
#endif
	SET2_SPR_PUSH_RES = spd_SATB_push_sprite( set2_frames_data, _ind, _x, _y );

#if	DEF_SET2_SG_DBL_BUFF
	SET2_LAST_DBL_BUFF_IND = spd_get_dbl_buff_ind();
#endif

	SET2_LAST_SG_BANK = spd_SG_bank_get_ind();
}

void	sprite3_show( char _ind, short _x, short _y )
{
#if	DEF_SET3_SG_DBL_BUFF
	spd_sprite_params( set3_SG_arr, SET3_SPR_VADDR, SPD_FLAG_DBL_BUFF, SET3_LAST_SG_BANK );
	spd_dbl_buff_VRAM_addr( SET3_SPR_VADDR + 0x800, SET3_LAST_DBL_BUFF_IND );
#else
	spd_sprite_params( set3_SG_arr, SET3_SPR_VADDR, SPD_FLAG_PEND_SG_DATA, SET3_LAST_SG_BANK );
	spd_SG_data_params( &SET3_SG_DATA_SRC_ADDR, &SET3_SG_DATA_SRC_BANK, &SET3_SG_DATA_DST_ADDR, &SET3_SG_DATA_LEN );
#endif
	SET3_SPR_PUSH_RES = spd_SATB_push_sprite( set3_frames_data, _ind, _x, _y );

#if	DEF_SET3_SG_DBL_BUFF
	SET3_LAST_DBL_BUFF_IND = spd_get_dbl_buff_ind();
#endif

	SET3_LAST_SG_BANK = spd_SG_bank_get_ind();
}

/* animation update */
void	update_frame( anim_desc* _anm_desc )
{
	if( ++_anm_desc->curr_tick == _anm_desc->num_ticks )
	{
		_anm_desc->curr_tick = 0;

		if( ++_anm_desc->curr_frame == _anm_desc->num_frames )
		{
			_anm_desc->curr_frame = _anm_desc->loop_frame;
		}
	}
}

/* update vertical position */
void	update_y_pos()
{
	if( dir_trigger )
	{
		if( ++y_pos > 330 )
		{
			dir_trigger ^= 1;
		}
	}
	else
	{
		if( --y_pos < 0 )
		{
			dir_trigger ^= 1;
		}
	}
}

main()
{
	unsigned short data_size;

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

	/* load palettes in the usual way */
	load_palette( SET1_PALETTE_SLOT, set1_palette, SET1_PALETTE_SIZE );
	load_palette( SET2_PALETTE_SLOT, set2_palette, SET2_PALETTE_SIZE );
	load_palette( SET3_PALETTE_SLOT, set3_palette, SET3_PALETTE_SIZE );

	/* enable display */
	disp_on();

	/* font for printing useful info */
	load_default_font();

	/* demo main loop */
	for( ;; )
	{
		/* update all aniamtions */
		update_frame( &test_anim1 );
		update_frame( &test_anim2 );
		update_frame( &test_anim3 );

		/* reset local SATB */
		reset_satb();

		/* set SATB position to push meta-sprites to */
		spd_SATB_set_pos( 0 );

		/* push meta-sprites to SATB */
		sprite1_show( test_anim1.start_frame + test_anim1.curr_frame, 40,  y_pos );
		sprite2_show( test_anim2.start_frame + test_anim2.curr_frame, 120, y_pos );
		sprite3_show( test_anim3.start_frame + test_anim3.curr_frame, 220, y_pos );

		/* change sprites position */
		update_y_pos();

		/* after pushing all sprites, `spd_SATB_get_pos()` returns the number of sprites in SATB. */
		satb_update( spd_SATB_get_pos() );

		vsync();

		data_size = 0;

#if	DEF_SET1_SG_DBL_BUFF
		put_string( "S1:DBL-BUFF", 0, 25 );
#else
		// the HuC doesn't allow to handle VBLANK directly, so we use 'vsync' instead for synchronization of sprite graphics and the inner SATB.
		// this may cause some graphical glitches at the upper part of the screen

		// delayed copying of SG data to VRAM to synchronize it with the inner SATB
		if( SET1_SPR_PUSH_RES & SPD_SG_NEW_DATA )
		{
			spd_copy_SG_data_to_VRAM( SET1_SG_DATA_SRC_ADDR, SET1_SG_DATA_SRC_BANK, SET1_SG_DATA_DST_ADDR, SET1_SG_DATA_LEN );
		}
		put_string( "S1:SNGL-BUFF", 0, 25 );

		data_size += SET1_SG_DATA_LEN;
#endif

#if	DEF_SET2_SG_DBL_BUFF
		put_string( "S2:DBL-BUFF", 0, 26 );
#else
		if( SET2_SPR_PUSH_RES & SPD_SG_NEW_DATA )
		{
			spd_copy_SG_data_to_VRAM( SET2_SG_DATA_SRC_ADDR, SET2_SG_DATA_SRC_BANK, SET2_SG_DATA_DST_ADDR, SET2_SG_DATA_LEN );
		}
		put_string( "S2:SNGL-BUFF", 0, 26 );

		data_size += SET2_SG_DATA_LEN;
#endif

#if	DEF_SET3_SG_DBL_BUFF
		put_string( "S3:DBL-BUFF", 0, 27 );
#else
		if( SET3_SPR_PUSH_RES & SPD_SG_NEW_DATA )
		{
			spd_copy_SG_data_to_VRAM( SET3_SG_DATA_SRC_ADDR, SET3_SG_DATA_SRC_BANK, SET3_SG_DATA_DST_ADDR, SET3_SG_DATA_LEN );
		}
		put_string( "S3:SNGL-BUFF", 0, 27 );

		data_size += SET3_SG_DATA_LEN;
#endif

		if( !( DEF_SET1_SG_DBL_BUFF * DEF_SET2_SG_DBL_BUFF * DEF_SET3_SG_DBL_BUFF ) )
		{
			// print ROM->VRAM data size for single-buffering mode
			put_string( "SNGL-BUFF:", 16, 27 );
			put_number( ( data_size << 1 ), 5, 26, 27 );
		}
	}
}