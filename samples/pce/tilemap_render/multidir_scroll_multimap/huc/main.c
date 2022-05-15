//#################################################################
//
// Copyright 2021-2022 0x8BitDev ( MIT license. See LICENSE.txt )
// Desc: Multidirectional scroller demo with multiple maps support
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
//#################################################################

#include <huc.h>

#include "../../../common/mpd_def.h"
#include "tilemap.h"
#include "../../../common/mpd.h"
#include "../../../common/mpd_tile_prop_demo.h"


u8	map_ind = -1;

void	show_info( bool _prop_demo_res )
{
	put_string( "Maps:", 0, 0 );
	put_number( MAPS_CNT, 2, 5, 0 );
	
	put_string( "Multi-dir scroller demo", 3, 7 );
	put_string( "<SEL> - show the next map", 3, 13 );
	put_string( "<L/U/R/D> - camera movement", 3, 14 );

	if( !_prop_demo_res )
	{
		put_string( "*No properties found!", 3, 17 );
	}
}

void	display_next_map()
{
	/*  disable display */
	disp_off();
	vsync();

	/* init tilemap renderer data */
	map_ind = ++map_ind % MAPS_CNT;
	mpd_init( map_ind, 2 );

	/* draw start screen */
	mpd_draw_screen();

	/*  enable display */
	disp_on();
}

void	check_data()
{
#if	FLAG_MODE_MULTIDIR_SCROLL == 0
	put_string( "MULTI-DIR data not found!", 1, 12 );
	put_string( "Please, re-export!", 1, 13 );
	
	disp_on(); for(;;) { vsync(); }
#endif

#if	FLAG_MARKS
	put_string( "MARKS aren't supported!", 1, 12 );
	put_string( "Please, re-export!", 1, 13 );
	
	disp_on(); for(;;) { vsync(); }
#endif

#if	FLAG_RLE
	put_string( "RLE isn't supported!", 1, 12 );
	put_string( "Please, re-export!", 1, 13 );
	
	disp_on(); for(;;) { vsync(); }
#endif
}

main()
{
	bool	sel_btn_pressed;
	bool	prop_demo_res;

	/*  disable display */
	disp_off();

	/* check exported data */
	check_data();

	/* init a tile properties demo */
	prop_demo_res = mpd_tile_prop_demo_init();

	/* the tile properties demo canceled, so the 
	/* demo continues as simple tilemap renderer */

	/* clear display */
	cls();

	/* show startup info */
	show_info( prop_demo_res );

	/*  enable display */
	disp_on();

	sel_btn_pressed	= FALSE;

	/*  demo main loop */
	for (;;)
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

		/* update BAT with tiles */
		mpd_update_screen();

		/* see mpd.h for details */
		pokew( 0x220c, mpd_scroll_x() );
		pokew( 0x2210, mpd_scroll_y() );
		vsync();
	}
}