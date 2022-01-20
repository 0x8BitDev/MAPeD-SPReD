//################################################################
//
// Copyright 2021-2022 0x8BitDev ( MIT license. See LICENSE.txt )
// Desc: Multidirectional scroller demo
//
// Supported flags:
//
// - FLAG_TILES2X2
// - FLAG_TILES4X4
// - FLAG_DIR_COLUMNS
// - FLAG_DIR_ROWS
// - FLAG_MODE_MULTIDIR_SCROLL
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


main()
{
	/*  disable display */
	disp_off();

	/* init a tile properties demo */
	mpd_tile_prop_demo_init();

	/* the tile properties demo canceled, so the 
	/* demo continues as simple tilemap renderer */

	/* init tilemap renderer data */
	mpd_init( 0, ms_2px );

	/* draw start screen */
	mpd_draw_screen();

	/*  enable display */
	disp_on();

	/*  demo main loop */
	for (;;)
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