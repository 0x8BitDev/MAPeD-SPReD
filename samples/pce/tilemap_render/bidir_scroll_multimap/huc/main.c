//################################################################
//
// Copyright 2021-2022 0x8BitDev ( MIT license. See LICENSE.txt )
// Desc: Bidirectional scroller demo with multiple maps
//
// Supported flags:
//
// - FLAG_TILES2X2
// - FLAG_TILES4X4
// - FLAG_DIR_COLUMNS
// - FLAG_DIR_ROWS
// - FLAG_MODE_BIDIR_SCROLL
// - FLAG_LAYOUT_ADJ_SCR
// - FLAG_LAYOUT_ADJ_SCR_INDS
// - FLAG_MARKS
//
// RECOMMENDED BAT SIZE: 64x32
//
//################################################################

#include <huc.h>

#include "../../../common/mpd_def.h"
#include "tilemap.h"
#include "../../../common/mpd.h"
#include "../../../common/mpd_tile_prop_demo.h"

u8	map_ind = -1;

void	show_info()
{
	/* clear display */
	cls();

	put_string( "Bidirectional scroller demo", 3, 7 );
	put_string( "<SEL> - show the next map", 3, 13 );
	put_string( "<L/U/R/D> - camera movement", 3, 14 );
}

void	display_next_map()
{
	/*  disable display */
	disp_off();

	/* init tilemap renderer data */
	map_ind = ++map_ind % MAPS_CNT;
	mpd_init( map_ind, ms_2px );

	/* draw start screen */
	mpd_draw_screen();

	/*  enable display */
	disp_on();
}

main()
{
	bool sel_btn_pressed;

	/*  disable display */
	disp_off();

	/* init a tile properties demo */
	mpd_tile_prop_demo_init();

	/* the tile properties demo canceled, so the 
	/* demo continues as simple tilemap renderer */

	/* show startup info */
	show_info();

	/*  enable display */
	disp_on();

	sel_btn_pressed = FALSE;

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
	
		if( map_ind >= 0 )
		{
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

			mpd_update_screen( TRUE );
		}
	}
}