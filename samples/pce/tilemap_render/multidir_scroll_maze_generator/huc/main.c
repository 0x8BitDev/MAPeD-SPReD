//####################################################################
//
// Copyright 2023 0x8BitDev ( MIT license. See LICENSE.txt )
// Desc: Random maze generator demo.
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
// Project file: ./data/scroll_multidir_maze_template_TEST1.mapedpce
//
// NOTE: should be compiled WITHOUT '-fno-recursive' option
//####################################################################


// Move a map data to RAM to enable dynamic map functionality

// Move a map data to RAM, this enables mpd_set_tile(...) and mpd_get_tile(...)
// use MPD_DEBUG to check array overflow(!)
#define MPD_RAM_MAP

// Move a map LUT to RAM
#define MPD_RAM_MAP_TBL

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

// Tile indexes

#define WALL_TILE_IND		1	// index of a wall tile etc...
#define FLOOR_TILE_IND		2
#define ELIXIR_TILE_IND		3
#define EXIT_TILE_IND		4
#define PATH_TRACING_TILE_IND	5

#define MAX_COLLECTABLES	50


// The main random maze generator algorithm
// https://github.com/angeluriot/Maze_solver
// p.s.: the algorithm works better with an odd number of cells vertically and horizontally

void	sub_recursive_division( u8 _x_min, u8 _y_min, u8 _x_max, u8 _y_max )
{
	static u8 i;

	u8 x;
	u8 y;

	if( ( _y_max - _y_min ) > ( _x_max - _x_min ) )
	{
		x = random_int( _x_min + 1, _x_max );
		y = random_int( _y_min + 2, _y_max - 1 );

		if( ( ( x - _x_min ) % 2 ) == 0 )
		{
			x += ( random_int( 0, 2 ) == 0 ? 1 : -1 );
		}

		if( ( ( y - _y_min ) % 2 ) == 1 )
		{
			y += ( random_int( 0, 2 ) == 0 ? 1 : -1 );
		}

		for( i = _x_min + 1; i < _x_max; i++ )
		{
			if( i != x )
			{
				mpd_set_tile( i, y, FALSE, WALL_TILE_IND );	// FALSE - tile coordinates
			}
		}

		if( ( y - _y_min ) > 2 )
		{
			sub_recursive_division( _x_min, _y_min, _x_max, y );
		}

		if( ( _y_max - y ) > 2 )
		{
			sub_recursive_division( _x_min, y, _x_max, _y_max );
		}
	}
	else
	{
		x = random_int( _x_min + 2, _x_max - 1 );
		y = random_int( _y_min + 1, _y_max );

		if( ( ( x - _x_min ) % 2 ) == 1 )
		{
			x += ( random_int( 0, 2 ) == 0 ? 1 : -1 );
		}

		if( ( ( y - _y_min ) % 2 ) == 0 )
		{
			y += ( random_int( 0, 2 ) == 0 ? 1 : -1 );
		}

		for( i = _y_min + 1; i < _y_max; i++ )
		{
			if( i != y )
			{
				mpd_set_tile( x, i, FALSE, WALL_TILE_IND );	// FALSE - tile coordinates
			}
		}

		if( ( x - _x_min ) > 2 )
		{
			sub_recursive_division( _x_min, _y_min, x, _y_max );
		}

		if( ( _x_max - x ) > 2 )
		{
			sub_recursive_division( x, _y_min, _x_max, _y_max );
		}
	}
}

u8	random_int( u8 _min, u8 _max )
{ 
	return random( _max - _min ) + _min;
}

void	generate_entities()
{
	static u8 i;

	static u8 x_range;
	static u8 y_range;

	static u8 x;
	static u8 y;

	x_range = mpd_map_tiles_width - 3;
	y_range = mpd_map_tiles_height - 3;

	// put exit
	mpd_set_tile( x_range, y_range, FALSE, EXIT_TILE_IND );

	// place the elixir bottles
	for( i = 0; i < MAX_COLLECTABLES; i++ )
	{
		while( TRUE )
		{
			x = random( x_range ) + 1;
			y = random( y_range ) + 1;

			if( mpd_get_tile( x, y, FALSE ) == FLOOR_TILE_IND )
			{
				// place the elixir tile in an empty space
				mpd_set_tile( x, y, FALSE, ELIXIR_TILE_IND );

				break;
			}
		}
	}
}

void	generate_maze()
{
	/* disable display */
	disp_off();
	vsync();

	/* reset scroll registers */
	pokew( 0x220c, 0 );
	pokew( 0x2210, 0 );

	/* initialize the template map */
	mpd_init( 0, 2 );

	/* generate random maze */
	sub_recursive_division( 0, 0, mpd_map_tiles_width - 2, mpd_map_tiles_height - 2 );

	/* generate collectables and exit entities */
	generate_entities();

	/* display the generated map */
	mpd_draw_screen();

	/* enable display */
	disp_on();
	vsync();
}

void	show_start_screen()
{
	/* disable display */
	disp_off();
	vsync();

	/* clear display */
	cls();

	put_string( mpd_ver, 25, 27 );

#if	FLAG_MODE_MULTIDIR_SCROLL
	put_string( "Maze generator demo", 3, 6 );

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

	put_string( "<SEL> - generate maze", 3, 17 );

	/* enable display */
	disp_on();
	vsync();

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

	sel_btn_pressed = FALSE;

	init_satb();

	/* initialize pseudo-random number generator */
	srand( 0xA51B );

	show_start_screen();

	generate_maze();

	for( ;; )
	{
		if( joy(0) & JOY_SEL )
		{
			if( !sel_btn_pressed )
			{
				generate_maze();

				sel_btn_pressed = TRUE;
			}
		}
		else
		{
			sel_btn_pressed = FALSE;
		}

		mpd_clear_update_flags();

		if( joy(0) & JOY_LEFT )
		{
			mpd_move_left();
		}

		if( joy(0) & JOY_RIGHT )
		{
			mpd_move_right();
		}

		if( joy(0) & JOY_UP )
		{
			mpd_move_up();
		}

		if( joy(0) & JOY_DOWN )
		{
			mpd_move_down();
		}

		mpd_update_screen();

		/* see mpd.h for details */
		pokew( 0x220c, mpd_scroll_x );
		pokew( 0x2210, mpd_scroll_y );
		vsync();
	}
}