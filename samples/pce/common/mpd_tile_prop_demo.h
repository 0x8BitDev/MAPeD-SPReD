//######################################################################################################
//
// This file is a part of the MAPeD-PCE Copyright 2017-2022 0x8BitDev ( MIT license. See LICENSE.txt )
// Desc: This is a demo of getting tile properties for different tilemap renderers.
//
// Supported flags:
//
// - FLAG_TILES2X2
// - FLAG_TILES4X4
// - FLAG_DIR_COLUMNS
// - FLAG_DIR_ROWS
// - FLAG_MODE_MULTIDIR_SCROLL
// - FLAG_MODE_BIDIR_SCROLL
// - FLAG_MODE_BIDIR_STAT_SCR
// - FLAG_MODE_STAT_SCR
// - FLAG_PROP_ID_PER_BLOCK
// - FLAG_PROP_ID_PER_CHR
//
//######################################################################################################


#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
#define SCR_MOVE_BORDER	30
#else
#define SCR_MOVE_BORDER	10
#endif

#define	SCR_LEFT_BORDER		__scroll_step
#define	SCR_RIGHT_BORDER	( ScrPixelsWidth - 16 - __scroll_step )
#define	SCR_UP_BORDER		__scroll_step
#define	SCR_DOWN_BORDER		( ScrPixelsHeight - 16 - __scroll_step )

/* cursor data */

#incspr(cursor, "../../../common/digits_cursor.pcx", 0, 64, 1, 1)
#incpal(cursor_pal, "../../../common/digits_cursor.pcx", 15, 1)

#define	CURSOR_VRAM	0x5000
#define	CURSOR_PAL	16
#define	CURSOR_SPR	17

u8	__cursor_x	= 0;
u8	__cursor_y	= 0;

/* property digits */

#incspr(digits, "../../../common/digits_cursor.pcx", 0, 0, 4, 4)

#define DIGITS_VRAM	0x5040
#define	DIGITS_PAL	16
#define	DIGITS_CNT	16

s8	__curr_prop_digit	= -1;

/* other vars */

s8	__map_ind		= -1;

const u8 __scroll_step		= 2;

//--------------------------------------------------------------------//
// The main function that gets a property value by cursor coordinates //
//--------------------------------------------------------------------//

void	__update_property()
{
#if	FLAG_MODE_MULTIDIR_SCROLL
	__show_property_val( mpd_get_property( mpd_scroll_x() + __cursor_center_x(), mpd_scroll_y() + __cursor_center_y() ) );
#else
	__show_property_val( mpd_get_property( __cursor_center_x(), __cursor_center_y() ) );
#endif
}

bool	mpd_tile_prop_demo_init()
{
	u8	valid_maps_cnt;

	if( !( valid_maps_cnt = __check_properties() ) )
	{
		/* there are no properties, so return to the main program */
		return FALSE;
	}

	/* clear display */
	cls();

	put_string( mpd_ver, 25, 27 );

	put_string( "Maps:  /", 0, 0 );
	put_number( MAPS_CNT, 2, 5, 0 );
	put_number( valid_maps_cnt, 2, 8, 0 );

#if	0	// it's strange, but such way FLAG_MODE_MULTIDIR_SCROLL works...
#elif	FLAG_MODE_MULTIDIR_SCROLL
	put_string( "MODE: multi-dir scroller", 3, 7 );
#elif	FLAG_MODE_BIDIR_SCROLL
	put_string( "MODE: bi-dir scroller", 3, 7 );
#elif	FLAG_MODE_BIDIR_STAT_SCR
	put_string( "MODE: bi-dir static screens", 3, 7 );
#elif	FLAG_MODE_STAT_SCR
	put_string( "MODE: bi-dir VDC-ready data", 3, 7 );
#else
	put_string( "Unknown layout mode detected!", 2, 12 );
	put_string( "Try to re-export your data!", 2, 13 );

	for( ;; ){}
#endif
	put_string( "<A> - tile properties demo", 3, 12 );
	put_string( "<B> - tilemap render", 3, 13 );
	put_string( "<SEL> - show the next map*", 3, 15 );

#if	FLAG_PROP_ID_PER_BLOCK
	put_string( "Property Id per: Block", 3, 18 );
#else
	put_string( "Property Id per: CHR", 3, 18 );
#endif
	put_string( "*Maps without properties", 3, 20 );
	put_string( " will be skipped!", 3, 21 );

	/*  enable display */
	disp_on();

	for( ;; )
	{
		if( joy(0) & JOY_A )
		{
			__tile_prop_demo_start();
		}

		if( joy(0) & JOY_B )
		{
			disp_off();

			return TRUE;
		}
	}
}

bool	__check_map_properties( u8 _map_ind )
{
	u8	chr_id_mul2;
	u16	i;
	u16	beg_offset;
	u16	end_offset;

#if	FLAG_MODE_MULTIDIR_SCROLL
	chr_id_mul2	= mpd_farpeekb( mpd_MapsCHRBanks, _map_ind ) << 1;
#else
	chr_id_mul2	= ( *mpd_MapsArr[ _map_ind ] )->chr_id << 1;
#endif

	beg_offset = mpd_farpeekw( mpd_PropsOffs, chr_id_mul2 );
	end_offset = mpd_farpeekw( mpd_PropsOffs, chr_id_mul2 + 2 );

	for( i = beg_offset; i < end_offset; i++ )
	{
		if( mpd_farpeekb( mpd_Props, i ) )
		{
			return TRUE;
		}
	}

	return FALSE;
}

u8	__check_properties()
{
	u8	i;
	u8	valid_maps_cnt;

	valid_maps_cnt = 0;

	for( i = 0; i < MAPS_CNT; i++ )
	{
		if( __check_map_properties( i ) )
		{
			++valid_maps_cnt;
		}
	}

	return valid_maps_cnt;
}

void	__display_next_map()
{
	/* disable display */
	disp_off();

	/* get a map with properties */
	do
	{
		__map_ind = ++__map_ind % MAPS_CNT;
	}
	while( !__check_map_properties( __map_ind ) );

	/* init tilemap renderer data */
#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
	mpd_init( __map_ind, __scroll_step );
#else
	mpd_init( __map_ind );
#endif	//FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL

	/* draw start screen */
	mpd_draw_screen();

	/* update tile property */
	__update_property();

	/* enable display */
	disp_on();
}

u8	__cursor_center_x()
{
	return __cursor_x + 8;
}

u8	__cursor_center_y()
{
	return __cursor_y + 8;
}

void	__update_cursor_pos()
{
 	spr_set( CURSOR_SPR );

	spr_x( __cursor_x );
	spr_y( __cursor_y );

	satb_update();

	/* update tile property */
	__update_property();
}

void	__init_cursor()
{
	/*  init cursor sprite */

	load_sprites( CURSOR_VRAM, cursor, 1 );
	load_palette( CURSOR_PAL, cursor_pal, 1 );

	spr_set( CURSOR_SPR );
	spr_pal( CURSOR_PAL );
	spr_pattern( CURSOR_VRAM );
	spr_ctrl( SIZE_MAS|FLIP_MAS, SZ_16x16|NO_FLIP );
	spr_pri( 1 );

	__cursor_x	= ( ScrPixelsWidth >> 1 ) - 8;
	__cursor_y	= ( ScrPixelsHeight >> 1 ) - 8;

	__update_cursor_pos();
}

void	__init_digits()
{
	u8	i;

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

void	__show_property_val( s8 _num )
{
	if( __curr_prop_digit >= 0 )
	{
		spr_hide( __curr_prop_digit );
	}

	spr_show( _num );

	__curr_prop_digit = _num;

	satb_update();
}

#if	FLAG_MODE_BIDIR_STAT_SCR + FLAG_MODE_STAT_SCR
void	__change_screen()
{
	disp_off();
	vsync();

	mpd_draw_screen();

	disp_on();
}
#endif	//FLAG_MODE_BIDIR_STAT_SCR + FLAG_MODE_STAT_SCR

void	__cursor_move_left()
{
	u16	tmp_scroll_x;

	__cursor_x -= __scroll_step;

	if( __cursor_center_x() < SCR_MOVE_BORDER )
#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
	{
		tmp_scroll_x = mpd_scroll_x();

		mpd_move_left();

		if( tmp_scroll_x != mpd_scroll_x() )
		{
			__cursor_x += __scroll_step;
		}
	}
#else
	{
		if( mpd_check_adj_screen( ADJ_SCR_LEFT ) )
		{
			__change_screen();

			__cursor_x = SCR_RIGHT_BORDER;
		}
	}
#endif	//FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL

	if( __cursor_x < SCR_LEFT_BORDER )
	{
		__cursor_x = SCR_LEFT_BORDER;
	}

	__update_cursor_pos();
}

void	__cursor_move_right()
{
	u16	tmp_scroll_x;

	__cursor_x += __scroll_step;

	if( __cursor_center_x() > ( ScrPixelsWidth - SCR_MOVE_BORDER ) )
#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
	{
		tmp_scroll_x = mpd_scroll_x();

		mpd_move_right();

		if( tmp_scroll_x != mpd_scroll_x() )
		{
			__cursor_x -= __scroll_step;
		}
	}
#else
	{
		if( mpd_check_adj_screen( ADJ_SCR_RIGHT ) )
		{
			__change_screen();

			__cursor_x = SCR_LEFT_BORDER;
		}
	}
#endif	//FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL

	if( __cursor_x > SCR_RIGHT_BORDER )
	{
		__cursor_x = SCR_RIGHT_BORDER;
	}

	__update_cursor_pos();
}

void	__cursor_move_up()
{
	u16	tmp_scroll_y;

	__cursor_y -= __scroll_step;

	if( __cursor_center_y() < SCR_MOVE_BORDER )
#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
	{
		tmp_scroll_y = mpd_scroll_y();

		mpd_move_up();

		if( tmp_scroll_y != mpd_scroll_y() )
		{
			__cursor_y += __scroll_step;
		}
	}
#else
	{
		if( mpd_check_adj_screen( ADJ_SCR_UP ) )
		{
			__change_screen();

			__cursor_y = SCR_DOWN_BORDER;
		}
	}
#endif	//FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL

	if( __cursor_y < SCR_UP_BORDER )
	{
		__cursor_y = SCR_UP_BORDER;
	}

	__update_cursor_pos();
}

void	__cursor_move_down()
{
	u16	tmp_scroll_y;

	__cursor_y += __scroll_step;

	if( __cursor_center_y() > ( ScrPixelsHeight - SCR_MOVE_BORDER ) )
#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
	{
		tmp_scroll_y = mpd_scroll_y();

		mpd_move_down();

		if( tmp_scroll_y != mpd_scroll_y() )
		{
			__cursor_y -= __scroll_step;
		}
	}
#else
	{
		if( mpd_check_adj_screen( ADJ_SCR_DOWN ) )
		{
			__change_screen();

			__cursor_y = SCR_UP_BORDER;
		}
	}
#endif	//FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL

	if( __cursor_y > SCR_DOWN_BORDER )
	{
		__cursor_y = SCR_DOWN_BORDER;
	}

	__update_cursor_pos();
}

void	__tile_prop_demo_start()
{
	bool 	sel_btn_pressed;

	/* disable display */
	disp_off();

	/* show the first map */
	__display_next_map();

	init_satb();

	/* init cursor data */
	__init_cursor();

	/* init digits data */
	__init_digits();

	/* show init property value */
	__update_property();

	/*  enable display */
	disp_on();

	sel_btn_pressed = FALSE;

	for( ;; )
	{
		if( joy(0) & JOY_SEL )
		{
			if( !sel_btn_pressed )
			{
				__display_next_map();

				sel_btn_pressed = TRUE;
			}
		}
		else
		{
			sel_btn_pressed = FALSE;
		}

#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
		mpd_clear_update_flags();
#endif
		if( joy(0) & JOY_LEFT )
		{
			__cursor_move_left();
		}

		if( joy(0) & JOY_RIGHT )
		{
			__cursor_move_right();
		}

		if( joy(0) & JOY_UP )
		{
			__cursor_move_up();
		}

		if( joy(0) & JOY_DOWN )
		{
			__cursor_move_down();
		}

#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
		mpd_update_screen( TRUE );
#else
		vsync();
#endif
	}
}