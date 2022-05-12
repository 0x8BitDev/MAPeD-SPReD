//#################################################################
//
// Copyright 2021-2022 0x8BitDev ( MIT license. See LICENSE.txt )
// Desc: Static screens switching demo for multidirectional map
//	 data with double buffering and with multiple maps support
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
// RECOMMENDED BAT SIZE: 64x32 (double buffering)
//
// NOTE: mpd_get_property( scr_pos_x + x, scr_pos_y + y )
//
//#################################################################

#include <huc.h>

#include "../../../common/mpd_def.h"
#include "tilemap.h"
#include "../../../common/mpd.h"


/* variables */

u8	map_ind		= 0xff;

u8	map_scr_width	= 0;
u8	map_scr_height	= 0;
u8	map_max_scr	= 0;	// W x H

s8	map_scr_ind	= 0;

u8	dbl_buff_trig	= 0;


void	show_info()
{
	put_string( "Maps:", 0, 0 );
	put_number( MAPS_CNT, 2, 5, 0 );
	
	put_string( "Multi-dir static screens", 3, 7 );
	put_string( "double-buffering: ON", 3, 8 );
	put_string( "<SEL> - show the next map", 3, 13 );
	put_string( "<L/U/R/D> - switch screen", 3, 14 );

	put_string( mpd_ver, 25, 27 );
}

void	display_next_map()
{
	/* disable display */
	disp_off();
	vsync();

	/* init tilemap renderer data */
	map_ind = ++map_ind % MAPS_CNT;
	mpd_init( map_ind, 2 );

	/* start screen */
	map_scr_ind	= mpd_StartScrArr[ map_ind ];

	/* draw start screen */
	mpd_draw_screen_by_ind_offs( map_scr_ind, 0, TRUE );	// 0 - BAT offset; TRUE - disable scrolling

	/* map size */
	map_scr_width	= mpd_MapsDimArr[ map_ind << 1 ];
	map_scr_height	= mpd_MapsDimArr[ ( map_ind << 1 ) + 1 ];

	map_max_scr	= map_scr_width * map_scr_height;

	/* reset double-buffers flag */
	dbl_buff_trig	= 0;

	/* jump to an active BAT buffer */
	scroll( 0, ( dbl_buff_trig ? ScrPixelsWidth:0 ), 0, 0, ScrPixelsHeight, 0xC0 );

	/* enable display */
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

void	show_screen()
{
	u8	BAT_offset;

	/* switch buffers flag */
	dbl_buff_trig ^= 1;

	/* an offset to BAT's active buffer */
	BAT_offset = dbl_buff_trig * ( SCR_BLOCKS2x2_WIDTH << 1 );

	/* draw screen by index to invisible BAT buffer */
	mpd_draw_screen_by_ind_offs( map_scr_ind, BAT_offset, TRUE );	// TRUE - disable scrolling

	/* print an active buffer index */
	put_hex( dbl_buff_trig, 1, BAT_offset, 0 );

	/* show buffered screen */
	scroll( 0, ( dbl_buff_trig ? ScrPixelsWidth:0 ), 0, 0, ScrPixelsHeight, 0xC0 );
}

main()
{
	bool	sel_btn_pressed;
	bool	btn_pressed;

	load_default_font( 0, 0x2c00 );

	/* disable display */
	disp_off();

	/* clear display */
	cls();

	/* check exported data */
	check_data();

	/* show startup info */
	show_info();

	/* enable display */
	disp_on();

	/* create scrollable window to jump between buffers */
	scroll( 0, 0, 0, 0, ScrPixelsHeight, 0xC0 );

	sel_btn_pressed	= FALSE;
	btn_pressed	= FALSE;

	/* demo main loop */
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

		if( map_ind != 0xff )
		{
			if( joy(0) & JOY_LEFT )
			{
				if( !btn_pressed )
				{
					if( --map_scr_ind < 0 )
					{
						map_scr_ind = 0;
					}
					show_screen();

					btn_pressed = TRUE;
				}
			}

			if( joy(0) & JOY_RIGHT )
			{
				if( !btn_pressed )
				{
					if( ++map_scr_ind >= map_max_scr )
					{
						map_scr_ind = map_max_scr - 1;
					}
					show_screen();

					btn_pressed = TRUE;
				}
			}

			if( joy(0) & JOY_UP )
			{
				if( !btn_pressed )
				{
					if( ( map_scr_ind - map_scr_width ) >= 0 )
					{
						map_scr_ind -= map_scr_width;
					}
					show_screen();
					
					btn_pressed = TRUE;
				}
			}

			if( joy(0) & JOY_DOWN )
			{
				if( !btn_pressed )
				{
					if( ( map_scr_ind + map_scr_width ) < map_max_scr )
					{
						map_scr_ind += map_scr_width;
					}
					show_screen();
					
					btn_pressed = TRUE;
				}
			}

			if( !( joy(0) & (JOY_LEFT|JOY_RIGHT|JOY_DOWN|JOY_UP) ) )
			{
				btn_pressed = FALSE;
			}
		}

		vsync();
	}
}