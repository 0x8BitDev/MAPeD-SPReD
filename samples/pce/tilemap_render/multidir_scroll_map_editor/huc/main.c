//#################################################################
//
// Copyright 2023 0x8BitDev ( MIT license. See LICENSE.txt )
// Desc: Simple map editor demo.
//	 Multidirectional scroller with multiple maps support.
//
// Supported flags:
//
// - FLAG_TILES2X2
// - FLAG_TILES4X4
// - FLAG_DIR_COLUMNS
// - FLAG_DIR_ROWS
// - FLAG_MODE_MULTIDIR_SCROLL
// - FLAG_LAYOUT_MATRIX
// - FLAG_PROP_ID_PER_BLOCK
// - FLAG_PROP_ID_PER_CHR
//
// RECOMMENDED BAT SIZE: 64x32
//
// Project file: ./data/scroll_multidir_map_editor_TEST1.mapedpce
//
//#################################################################


// Move a map data to RAM to enable dynamic map functionality
// All these defines speed up getting a tile property and slightly speed up static screens drawing and scrolling

// Move a map data to RAM, this enables mpd_set_tile(...) and mpd_get_tile(...); use MPD_DEBUG to check array overflow(!)
#define MPD_RAM_MAP

// Move a map LUT to RAM
#define MPD_RAM_MAP_TBL

// Move a tile properties array to RAM, this enables mpd_get_tile_property(...) and mpd_set_tile_property(...); use MPD_DEBUG to check array overflow(!)
#define MPD_RAM_TILE_PROPS


// debug info (use Mednafen):
// - green border color		- screen scrolling
// - blue border color		- static screen drawing
// - yellow border color	- getting a tile property
//
//#define MPD_DEBUG

#include <huc.h>

#include "../../../common/mpd_def.h"
#include "tilemap.h"
#include "../../../common/mpd.h"


#define SCR_MOVE_BORDER		30

#define SCROLL_STEP		2	// pix/frame

#define	SCR_LEFT_BORDER		SCROLL_STEP
#define	SCR_RIGHT_BORDER	( ScrPixelsWidth - 16 - SCROLL_STEP )
#define	SCR_UP_BORDER		SCROLL_STEP
#define	SCR_DOWN_BORDER		( ScrPixelsHeight - 16 - SCROLL_STEP )

/* cursor data */

#incspr(cursor, "../../../common/digits_cursor.pcx", 0, 64, 1, 1)
#incpal(cursor_pal, "../../../common/digits_cursor.pcx", 15, 1)

#define	CURSOR_VRAM	0x5000
#define	CURSOR_PAL	16
#define	CURSOR_SPR	17

u8	cursor_x	= 0;
u8	cursor_y	= 0;

#define CURSOR_CENTER_X		( cursor_x + 8 )
#define CURSOR_CENTER_Y		( cursor_y + 8 )

#define CURSOR_TILE_X		( ( mpd_scroll_x + CURSOR_CENTER_X ) & ~0x1F )
#define CURSOR_TILE_Y		( ( mpd_scroll_y + CURSOR_CENTER_Y ) & ~0x1F )

#define CURSOR_BLOCK_X		( ( mpd_scroll_x + CURSOR_CENTER_X ) & ~0x0F )
#define CURSOR_BLOCK_Y		( ( mpd_scroll_y + CURSOR_CENTER_Y ) & ~0x0F )

/* property digits */

#incspr(digits, "../../../common/digits_cursor.pcx", 0, 0, 4, 4)

#define DIGITS_VRAM	0x5040
#define	DIGITS_PAL	16
#define	DIGITS_CNT	16

s8	curr_prop_digit	= -1;

/* other vars */

s8	map_ind		= -1;
u8	curr_tile_ind	= 1;

u8	bg_tile_ind;

u16	tile_x;
u16	tile_y;


/* Put current tile in a map and on the screen */

void	put_tile()
{
	show_tile();

	mpd_set_tile( tile_x, tile_y, TRUE, curr_tile_ind );	// TRUE - coordinates in pixels

	update_property();
}

/* Show current tile and/or change it */

void	show_tile()
{
#if	FLAG_TILES4X4
	tile_x = CURSOR_TILE_X;
	tile_y = CURSOR_TILE_Y;

	mpd_draw_tile4x4( tile_x, tile_y, curr_tile_ind );
#else
	tile_x = CURSOR_BLOCK_X;
	tile_y = CURSOR_BLOCK_Y;

	mpd_draw_block2x2( tile_x, tile_y, curr_tile_ind );
#endif
}

void	change_tile()
{
	static bool 	up_btn_pressed;
	static bool 	down_btn_pressed;

	up_btn_pressed = FALSE;
	down_btn_pressed = FALSE;

	show_tile();

	/* save background tile */
	bg_tile_ind = mpd_get_tile( tile_x, tile_y, TRUE );	// TRUE - coordinates in pixels

	for(;;)
	{
		if( joy(0) & JOY_UP )
		{
			if( !up_btn_pressed )
			{
				if( curr_tile_ind < ( mpd_tiles_cnt - 1 ) )
				{
					++curr_tile_ind;
				}
				show_tile();

				up_btn_pressed = TRUE;
			}
		}
		else
		{
			up_btn_pressed = FALSE;
		}

		if( joy(0) & JOY_DOWN )
		{
			if( !down_btn_pressed )
			{
				if( curr_tile_ind > 0 )
				{
					--curr_tile_ind;
				}
				show_tile();

				down_btn_pressed = TRUE;
			}
		}
		else
		{
			down_btn_pressed = FALSE;
		}

		if( joy( 0 ) == 0 )
		{
			break;
		}

		vsync();
	}

	/* restore background tile before exit */

#if	FLAG_TILES4X4
	mpd_draw_tile4x4( tile_x, tile_y, bg_tile_ind );
#else
	mpd_draw_block2x2( tile_x, tile_y, bg_tile_ind );
#endif
}

/* Getting a tile property that cursor is pointing to */

void	update_property()
{
	show_property_val( mpd_get_property( mpd_scroll_x + CURSOR_CENTER_X, mpd_scroll_y + CURSOR_CENTER_Y ) );
}

void	display_next_map()
{
	/* disable display */
	disp_off();
	vsync();

	map_ind	= ++map_ind >= MAPS_CNT ? 0:map_ind;

	/* init tilemap renderer data */
	mpd_init( map_ind, SCROLL_STEP );

	/* draw start screen */
	mpd_draw_screen();

	/* update tile property */
	update_property();

	/* enable display */
	disp_on();
}

void	update_cursor_pos()
{
 	spr_set( CURSOR_SPR );

	spr_x( cursor_x );
	spr_y( cursor_y );

	satb_update();

	/* update tile property */
	update_property();
}

void	init_cursor()
{
	/* init cursor sprite */

	load_sprites( CURSOR_VRAM, cursor, 1 );
	load_palette( CURSOR_PAL, cursor_pal, 1 );

	spr_set( CURSOR_SPR );
	spr_pal( CURSOR_PAL );
	spr_pattern( CURSOR_VRAM );
	spr_ctrl( SIZE_MAS|FLIP_MAS, SZ_16x16|NO_FLIP );
	spr_pri( 1 );

	cursor_x	= ( ScrPixelsWidth >> 1 ) - 8;
	cursor_y	= ( ScrPixelsHeight >> 1 ) - 8;

	update_cursor_pos();
}

void	init_digits()
{
	static u8	i;

	load_sprites( DIGITS_VRAM, digits, 2 );

	for( i = 0; i < DIGITS_CNT; i++ )
	{
		spr_set( i );
		spr_pal( DIGITS_PAL );
		spr_pattern( DIGITS_VRAM + ( i << 6 ) );
		spr_ctrl( SIZE_MAS|FLIP_MAS, SZ_16x16|NO_FLIP );
		spr_pri( 1 );
		spr_x( 0 );
		spr_y( 0 );
		spr_hide();
	}

	satb_update();
}

void	show_property_val( s8 _num )
{
	if( curr_prop_digit >= 0 )
	{
		spr_hide( curr_prop_digit );
	}

	spr_show( _num );

	curr_prop_digit = _num;

	satb_update();
}

void	cursor_move_left()
{
	static u16	tmp_scroll_x;

	cursor_x -= SCROLL_STEP;

	if( CURSOR_CENTER_X < SCR_MOVE_BORDER )
	{
		tmp_scroll_x = mpd_scroll_x;

		mpd_move_left();

		if( tmp_scroll_x != mpd_scroll_x )
		{
			cursor_x += SCROLL_STEP;
		}
	}

	if( cursor_x < SCR_LEFT_BORDER )
	{
		cursor_x = SCR_LEFT_BORDER;
	}

	update_cursor_pos();
}

void	cursor_move_right()
{
	static u16	tmp_scroll_x;

	cursor_x += SCROLL_STEP;

	if( CURSOR_CENTER_X > ( ScrPixelsWidth - SCR_MOVE_BORDER ) )
	{
		tmp_scroll_x = mpd_scroll_x;

		mpd_move_right();

		if( tmp_scroll_x != mpd_scroll_x )
		{
			cursor_x -= SCROLL_STEP;
		}
	}

	if( cursor_x > SCR_RIGHT_BORDER )
	{
		cursor_x = SCR_RIGHT_BORDER;
	}

	update_cursor_pos();
}

void	cursor_move_up()
{
	static u16	tmp_scroll_y;

	cursor_y -= SCROLL_STEP;

	if( CURSOR_CENTER_Y < SCR_MOVE_BORDER )
	{
		tmp_scroll_y = mpd_scroll_y;

		mpd_move_up();

		if( tmp_scroll_y != mpd_scroll_y )
		{
			cursor_y += SCROLL_STEP;
		}
	}

	if( cursor_y < SCR_UP_BORDER )
	{
		cursor_y = SCR_UP_BORDER;
	}

	update_cursor_pos();
}

void	cursor_move_down()
{
	static u16	tmp_scroll_y;

	cursor_y += SCROLL_STEP;

	if( CURSOR_CENTER_Y > ( ScrPixelsHeight - SCR_MOVE_BORDER ) )
	{
		tmp_scroll_y = mpd_scroll_y;

		mpd_move_down();

		if( tmp_scroll_y != mpd_scroll_y )
		{
			cursor_y -= SCROLL_STEP;
		}
	}

	if( cursor_y > SCR_DOWN_BORDER )
	{
		cursor_y = SCR_DOWN_BORDER;
	}

	update_cursor_pos();
}

void	show_start_screen()
{
	static u8	valid_maps_cnt;

	/* clear display */
	cls();

	put_string( mpd_ver, 25, 27 );

	put_string( "Maps:", 0, 0 );
	put_number( MAPS_CNT, 2, 5, 0 );

#if	FLAG_MODE_MULTIDIR_SCROLL
	put_string( "Map editor demo", 3, 6 );

	put_string( "MODE: multi-dir scroller", 3, 9 );
#else
	put_string( "Unsupported mode detected!", 2, 12 );
	put_string( "Re-export multidir maps!", 2, 13 );

	for( ;; ){}
#endif

#if	FLAG_TILES4X4
	put_string( "Tiles: 4x4", 3, 11 );
#else
	put_string( "Tiles: 2x2", 3, 11 );
#endif

#if	FLAG_PROP_ID_PER_BLOCK
	put_string( "Property Id per: Block", 3, 12 );
#else
	put_string( "Property Id per: CHR", 3, 12 );
#endif

	put_string( "Controls:", 3, 15 );

	put_string( "<A> - put tile", 3, 17 );
	put_string( "<B> - show current tile", 3, 18 );
	put_string( "<B>+<UP/DOWN> - change tile", 3, 19 );

	put_string( "<SEL> - next map", 3, 21 );

	/*  enable display */
	disp_on();

	for( ;; )
	{
		if( joy(0) & JOY_SEL )
		{
			return;
		}
	}
}

void	main()
{
	static bool 	sel_btn_pressed;

	show_start_screen();

	/* disable display */
	disp_off();

	/* show the first map */
	display_next_map();

	init_satb();

	/* init cursor data */
	init_cursor();

	/* init digits data */
	init_digits();

	/* show init property value */
	update_property();

	/*  enable display */
	disp_on();

	sel_btn_pressed = FALSE;

	for( ;; )
	{
		if( joy(0) & JOY_SEL )
		{
			if( !sel_btn_pressed )
			{
				display_next_map();

				sel_btn_pressed = TRUE;
			}
		}
		else
		{
			sel_btn_pressed = FALSE;
		}

		if( joy(0) & JOY_A )
		{
			put_tile();
		}

		if( joy(0) & JOY_B )
		{
			change_tile();
		}

		mpd_clear_update_flags();

		if( joy(0) & JOY_LEFT )
		{
			cursor_move_left();
		}

		if( joy(0) & JOY_RIGHT )
		{
			cursor_move_right();
		}

		if( joy(0) & JOY_UP )
		{
			cursor_move_up();
		}

		if( joy(0) & JOY_DOWN )
		{
			cursor_move_down();
		}

		mpd_update_screen();

		/* see mpd.h for details */
		pokew( 0x220c, mpd_scroll_x );
		pokew( 0x2210, mpd_scroll_y );
		vsync();
	}
}