//###############################################################################
//
// Copyright 2021-2022 0x8BitDev ( MIT license. See LICENSE.txt )
// Desc: Static screens switching demo (raw BAT data) with multiple maps support
//
// Supported flags:
//
// - FLAG_MODE_STAT_SCR
// - FLAG_LAYOUT_ADJ_SCR
// - FLAG_LAYOUT_ADJ_SCR_INDS
// - FLAG_MARKS
//
// - FLAG_TILES2X2
// - FLAG_TILES4X4
// - FLAG_DIR_COLUMNS
// - FLAG_DIR_ROWS
// - FLAG_PROP_ID_PER_BLOCK
// - FLAG_PROP_ID_PER_CHR
//
// RECOMMENDED BAT SIZE: 32x32
//
//###############################################################################

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

	put_string( "Static screens demo", 3, 7 );
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

	/* init tilemap renderer data */
	map_ind = ++map_ind % MAPS_CNT;
	mpd_init( map_ind );

	/* draw start screen */
	mpd_draw_screen();

	/*  enable display */
	disp_on();
}

main()
{
	bool	adj_scr_res;
	bool	btn_pressed;
	bool	sel_btn_pressed;
	bool	prop_demo_res;

	/*  disable display */
	disp_off();

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
	btn_pressed	= FALSE;

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
	
		adj_scr_res = 0;

		if( joy(0) & JOY_LEFT )
		{
			if( !btn_pressed )
			{
				adj_scr_res = mpd_check_adj_screen( ADJ_SCR_LEFT );
			}
		}
		else
		if( joy(0) & JOY_RIGHT )
		{
			if( !btn_pressed )
			{
				adj_scr_res = mpd_check_adj_screen( ADJ_SCR_RIGHT );
			}
		}
		else
		if( joy(0) & JOY_UP )
		{
			if( !btn_pressed )
			{
				adj_scr_res = mpd_check_adj_screen( ADJ_SCR_UP );
			}
		}
		else
		if( joy(0) & JOY_DOWN )
		{
			if( !btn_pressed )
			{
				adj_scr_res = mpd_check_adj_screen( ADJ_SCR_DOWN );
			}
		}
		else
		{
			btn_pressed = FALSE;
		}

		if( adj_scr_res )
		{
			disp_off();
			vsync();

			mpd_draw_screen();

			disp_on();

			btn_pressed = TRUE;
		}
		else
		{
			vsync();
		}
	}
}