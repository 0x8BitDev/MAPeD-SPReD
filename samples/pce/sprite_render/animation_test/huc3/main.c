//#####################################################################
//
// Copyright 2021-2022 0x8BitDev ( MIT license. See LICENSE.txt )
//
// DESC: Meta-sprite instancing demo. Each meta-sprite has its own
//	 unique behaviour, palette and share the same graphics data.
//
//	 The simple case, when your meta-sprite has one palette only.
//
// Source file: dd_chin_tai_mei_UNPACKED.spredpce
//
// Data export options:
// ~~~~~~~~~~~~~~~~~~~~
// Exported name: chin
// VADDR: $2000
// Comment CHR data: X
// Add filename to sprite names: V
// Data directory: data
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

#include "chin.h"

/* double-buffering or delayed SG data load to VRAM */
#define	DEF_SG_DBL_BUFF	0

/* available sprite types */
#define	SPR_TYPE_CHIN_VIOLET	0
#define	SPR_TYPE_CHIN_PINK	1
#define	SPR_TYPE_CHIN_BLUE	2

/* object actions */
#define	ACTION_IDLE_LEFT	0x00
#define	ACTION_MOVE_LEFT	0x01
#define	ACTION_IDLE_RIGHT	0x02
#define	ACTION_MOVE_RIGHT	0x03
#define	ACTIONS_MASK		0x03

/* screen borders for sprites movement */
#define	SCREEN_LEFT_BORDER	20
#define	SCREEN_RIGHT_BORDER	230

#define MOVE_STEP		2	// pix per frame


/* sprite instance description */
typedef struct
{
	// SPD library data

	unsigned short	alt_VADDR;	// alternative VADDR for sprite instancing
	unsigned char	last_bank;	// 0xff

#if	DEF_SG_DBL_BUFF
	unsigned short	dbf_VADDR;	// VADDR for double-buffering
	unsigned short	dbf_ind;	// SPD_DBL_BUFF_INIT_VAL
#else
	unsigned short	SG_src_addr;	// data for delayed
	unsigned char	SG_src_bank;	// load of SG data
	unsigned short	SG_dst_addr;	// to VRAM
	unsigned short	SG_data_len;	// ...
#endif
	unsigned char	push_spr_res;	// spd_SATB_push_sprite result
	
	// sprite type and position

	unsigned char	spr_type;	// SPR_TYPE_CHIN_VIOLET / SPR_TYPE_CHIN_PINK / SPR_TYPE_CHIN_BLUE

	short		x;
	short		y;

	// animation data

	unsigned char	curr_tick;
	unsigned char	curr_frame;

	unsigned char	num_ticks;	// number of game ticks or frame updates to change a frame
	unsigned char	num_frames;	// number of animated frames
	unsigned char	loop_frame;
	unsigned char	start_frame;	// start frame index

	// simple behaviour data

	unsigned char	action;
	unsigned short	act_time;

	char		active;		// 1/0

} sprite_desc;

/* sprites cache */
#define	DEF_SPR_CACHE_SIZE	4

sprite_desc	sprite_cache[ DEF_SPR_CACHE_SIZE ] = 
{
	{
		// SPD library data
		0x2000, 0xff,
#if	DEF_SG_DBL_BUFF
		0x3000, SPD_DBL_BUFF_INIT_VAL,
#else
		0, 0, 0, 0,
#endif
		0,

		// sprite type and position
		SPR_TYPE_CHIN_VIOLET,
		68, 170,

		// animation data
		0, 0, 0, 0, 0, 0,

		// simple behaviour data
		ACTION_IDLE_RIGHT, 25,

		// always active
		1
	},
	{
		0x2400, 0xff,
#if	DEF_SG_DBL_BUFF
		0x3400, SPD_DBL_BUFF_INIT_VAL,
#else
		0, 0, 0, 0,
#endif
		0,

		SPR_TYPE_CHIN_PINK,
		118, 160,

		0, 0, 0, 0, 0, 0,

		ACTION_MOVE_LEFT, 31,

		1
	},
	{
		0x2800, 0xff,
#if	DEF_SG_DBL_BUFF
		0x3800, SPD_DBL_BUFF_INIT_VAL,
#else
		0, 0, 0, 0,
#endif
		0,

		SPR_TYPE_CHIN_VIOLET,
		168, 150,

		0, 0, 0, 0, 0, 0,

		ACTION_MOVE_RIGHT, 64,

		1
	},
	{
		0x2c00, 0xff,
#if	DEF_SG_DBL_BUFF
		0x3c00, SPD_DBL_BUFF_INIT_VAL,
#else
		0, 0, 0, 0,
#endif
		0,

		SPR_TYPE_CHIN_BLUE,
		218, 140,

		0, 0, 0, 0, 0, 0,

		ACTION_IDLE_LEFT, 12,

		1
	}
};

/* exported sprite set initialization and pushing sprite to SATB */
void	push_sprite_chin( sprite_desc* _desc )
{
	// set up exported sprite set with SG data array and VRAM address to load SG data to.
	// NOTE: you can combine any number of exported sprite sets in your program.
	//	 call the 'spd_sprite_params' to switch between them.
	// NOTE: passing ZERO as the third parameter, means that SG data will be automatically
	//	 loaded to VRAM on 'spd_SATB_push_sprite'.
	// NOTE: passing 'SPD_FLAG_IGNORE_SG' as the third parameter will ignore loading SG to VRAM.
	//	 it's useful for PACKED(!) sprites when you are switching to a sprite set and SG data already loaded to VRAM.
	//	 such way you avoid loading SG to VRAM twice.
	// NOTE: using the `SPD_FLAG_DBL_BUFF` flag means double-buffering for sprite graphics.
	//	 it costs x2 of dynamic SG data in VRAM, but glitches free. you have to compare the results
	//	 of using 'SPD_FLAG_DBL_BUFF' and 'SPD_FLAG_PEND_SG_DATA' and decide which is better in your case.
	//	 THIS FLAG CAN BE USED WITH UNPACKED SPRITES ONLY! WHERE EACH SPRITE HAS A SEPARATE SG DATA!
	// NOTE: passing '_last_bank_ind' allows to avoid loading SG data to VRAM twice when you are switching back from another data set.
	//	 the last value can be obtained using 'spd_SG_bank_get_ind()'. the initial value is '0xff'.

#if	DEF_SG_DBL_BUFF
	spd_sprite_params( chin_SG_arr, CHIN_SPR_VADDR, SPD_FLAG_DBL_BUFF, _desc->last_bank );

	// set the second VRAM address for double-buffering (SPD_FLAG_DBL_BUFF)
	// and the last double-buffer index value (initial value is 'SPD_DBL_BUFF_INIT_VAL').
	spd_dbl_buff_VRAM_addr( _desc->dbf_VADDR, _desc->dbf_ind );
#else
	spd_sprite_params( chin_SG_arr, CHIN_SPR_VADDR, SPD_FLAG_PEND_SG_DATA, _desc->last_bank );

	// set pointers to SG data for delayed use (SPD_FLAG_PEND_SG_DATA).
	spd_SG_data_params( &_desc->SG_src_addr, &_desc->SG_src_bank, &_desc->SG_dst_addr, &_desc->SG_data_len );
#endif

	// set alternative VADDR for instancing
	spd_alt_VRAM_addr( _desc->alt_VADDR );

	// NOTE: There are two ways to load SG data to VRAM:
	//	 1. Indirect loading, when you push the first sprite by calling 'spd_SATB_push_sprite'.
	//	 The third argument for the 'spd_sprite_params' must be ZERO.
	//
	//	 2. Direct loading, when you call 'spd_copy_SG_data_to_VRAM' with a sprite data frame/index.
	//	 The third argument for the 'spd_sprite_params' must be 'SPD_FLAG_IGNORE_SG'.
	//
	//	 spd_copy_SG_data_to_VRAM( <exported_name>_frames_data, _spr_ind )
	//	 spd_copy_SG_data_to_VRAM( <animation_name>_frame )

	// push sprite to SATB
	_desc->push_spr_res	= spd_SATB_push_sprite( chin_frames_data, _desc->curr_frame, _desc->x, _desc->y );

	if( _desc->push_spr_res )
	{
		// if sprite was pushed, check do we need to change a palette?
		if( _desc->spr_type == SPR_TYPE_CHIN_PINK )
		{
			spd_change_palette( 1 );		// palette index in SPReD-PCE project
		}
		else
		if( _desc->spr_type == SPR_TYPE_CHIN_BLUE )
		{
			spd_change_palette( 2 );		// palette index in SPReD-PCE project
		}
	}

	_desc->last_bank	= spd_SG_bank_get_ind();

#if	DEF_SG_DBL_BUFF
	_desc->dbf_ind		= spd_get_dbl_buff_ind();
#endif
}

/* initialization of a sprite action */
void	init_action( sprite_desc* _desc, unsigned char _action )
{
	_desc->action	= _action;

	switch( _action )
	{
		case ACTION_IDLE_LEFT:
		{
			_desc->curr_tick	= 0;
			_desc->curr_frame	= SPR_CHIN_IDLE01_LEFT;

			_desc->num_ticks	= 10;
			_desc->num_frames	= 1;
			_desc->loop_frame	= SPR_CHIN_IDLE01_LEFT;
			_desc->start_frame	= SPR_CHIN_IDLE01_LEFT;

			_desc->act_time		= 25 + rand() % 63;
		}
		break;

		case ACTION_IDLE_RIGHT:
		{
			_desc->curr_tick	= 0;
			_desc->curr_frame	= SPR_CHIN_IDLE01_RIGHT;

			_desc->num_ticks	= 10;
			_desc->num_frames	= 1;
			_desc->loop_frame	= SPR_CHIN_IDLE01_RIGHT;
			_desc->start_frame	= SPR_CHIN_IDLE01_RIGHT;

			_desc->act_time		= 25 + rand() % 63;
		}
		break;

		case ACTION_MOVE_LEFT:
		{
			_desc->curr_tick	= 0;
			_desc->curr_frame	= SPR_CHIN_WALK01_LEFT;

			_desc->num_ticks	= 10;
			_desc->num_frames	= 4;
			_desc->loop_frame	= SPR_CHIN_WALK01_LEFT;
			_desc->start_frame	= SPR_CHIN_WALK01_LEFT;

			_desc->act_time		= 50 + rand() % 63;
		}
		break;

		case ACTION_MOVE_RIGHT:
		{
			_desc->curr_tick	= 0;
			_desc->curr_frame	= SPR_CHIN_WALK01_RIGHT;

			_desc->num_ticks	= 10;
			_desc->num_frames	= 4;
			_desc->loop_frame	= SPR_CHIN_WALK01_RIGHT;
			_desc->start_frame	= SPR_CHIN_WALK01_RIGHT;

			_desc->act_time		= 50 + rand() % 63;
		}
		break;
	}
}

/* initialization of all sprite actions at startup */
void	init_actions()
{
	sprite_desc*	desc;
	char		i;

	for( i = 0; i < DEF_SPR_CACHE_SIZE; i++ )
	{
		desc = &sprite_cache[ i ];

		if( desc->active )
		{
			init_action( desc, desc->action );
		}
	}
}

/* update current sprite action */
void	update_action( sprite_desc* _desc )
{
	switch( _desc->action )
	{
		case ACTION_MOVE_LEFT:
		{
			_desc->x -= MOVE_STEP;

			if( _desc->x < SCREEN_LEFT_BORDER )
			{
				init_action( _desc, ACTION_MOVE_RIGHT );
			}
		}
		break;

		case ACTION_MOVE_RIGHT:
		{
			_desc->x += MOVE_STEP;

			if( _desc->x > SCREEN_RIGHT_BORDER )
			{
				init_action( _desc, ACTION_MOVE_LEFT );
			}
		}
		break;
	}
}

/* animation update */
void	update_animation( sprite_desc* _desc )
{
	if( ++_desc->curr_tick == _desc->num_ticks )
	{
		_desc->curr_tick = 0;

		if( ++_desc->curr_frame == ( _desc->start_frame + _desc->num_frames ) )
		{
			_desc->curr_frame = _desc->loop_frame;
		}
	}
}

/* update all sprites in cache */
void	sprite_cache_update()
{
	sprite_desc*	desc;
	char		i;

	for( i = 0; i < DEF_SPR_CACHE_SIZE; i++ )
	{
		desc = &sprite_cache[ i ];

		if( desc->active )
		{
			if( !--desc->act_time )
			{
				// apply random action
				init_action( desc, rand() % ACTIONS_MASK );
			}
			else
			{
				update_action( desc );
			}

			update_animation( desc );
			push_sprite_chin( desc );
		}
	}
}

#if	!DEF_SG_DBL_BUFF
/* delayed load of SG data to VRAM */
unsigned short	sprite_cache_post_update()
{
	sprite_desc*	desc;
	unsigned short	SG_data_len;
	char		i;

	SG_data_len = 0;

	for( i = 0; i < DEF_SPR_CACHE_SIZE; i++ )
	{
		desc = &sprite_cache[ i ];

		if( desc->active )
		{
			if( desc->push_spr_res & SPD_SG_NEW_DATA )
			{
				// we have a new data to load to VRAM
				spd_copy_SG_data_to_VRAM( desc->SG_src_addr, desc->SG_src_bank, desc->SG_dst_addr, desc->SG_data_len );

				SG_data_len += desc->SG_data_len;
			}
		}
	}

	return SG_data_len << 1;
}
#endif

main()
{
	unsigned short	SG_data_len;
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

	/* load font */
	load_default_font( 0 );

	/* clear display */
	cls();

	/* initialize pseudo random number generator */
	srand( 32768 );

	/* SPD-render initialization */
	spd_init();	

	/* load sprites palettes */
	load_palette( CHIN_PALETTE_SLOT, chin_palette, CHIN_PALETTE_SIZE );

	/* initialize sprite actions */
	init_actions();

	/* enable display */
	disp_on();

	/* demo main loop */
	for( ;; )
	{
		/* reset local SATB */
		reset_satb();

		/* set SATB position to push sprites to */
		spd_SATB_set_pos( 0 );

		/* update sprite cache and push sprites to SATB */
		sprite_cache_update();

		/* after pushing all sprites, `spd_SATB_get_pos()` returns the number of sprites in SATB */
		satb_update( spd_SATB_get_pos() );

		vsync();

#if	DEF_SG_DBL_BUFF
		put_string( "DBL-BUFF: ON", 0, 27 );
#else
		put_string( "DBL-BUFF: OFF", 0, 26 );
		put_string( "ROM->VRAM:", 0, 27 );

		SG_data_len = sprite_cache_post_update();

		put_number( SG_data_len, 5, 10, 27 );
#endif
	}
}