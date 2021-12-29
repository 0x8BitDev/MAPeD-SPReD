//################################################################
//
// Copyright 2021 0x8BitDev ( MIT license. See LICENSE.txt )
// Desc: Static screens switching sample
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
//################################################################

#include <huc.h>
#include "tilemap.h"

#undef	FLAG_MODE_BIDIR_STAT_SCR
#define	FLAG_MODE_BIDIR_STAT_SCR	1
#undef	FLAG_MODE_BIDIR_SCROLL
#define	FLAG_MODE_BIDIR_SCROLL		0

#include "../../../common/mpd.h"


main()
{
	u8 adj_scr_res;
	u8 btn_pressed;

	/*  disable display */
	disp_off();

	/* init tilemap renderer data */
	mpd_init( Lev0_StartScr );

	/*  clear display */
	cls();

	/* draw start screen */
	mpd_draw_screen();

	/*  enable display */
	disp_on();

	btn_pressed = FALSE;

	/*  demo main loop */
	for (;;)
	{
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

		vsync();
	}
}