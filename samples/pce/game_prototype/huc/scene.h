//##################################################################
//
// Copyright 2021-2022 0x8BitDev ( MIT license. See LICENSE.txt )
//
// This file is a part of the game prototype demo.
//
//##################################################################

// player active area

const u8 SCR_LEFT_BORDER	= ( ScrPixelsWidth >> 1 ) - 20;
const u8 SCR_RIGHT_BORDER	= ( ScrPixelsWidth >> 1 ) + 20;
const u8 SCR_TOP_BORDER		= ( ScrPixelsHeight >> 1 ) - 50;
const u8 SCR_BOTTOM_BORDER	= ( ScrPixelsHeight >> 1 );

u8	__map_ind	= 0;
u8	__warn_sign_cnt	= 0;

// scene shaking data

const s8	__scene_shaking_vals_arr[ 9 ]	= { -1, 1, -1, 2, -2, 3, -3, 4, -4 };
u8		__scene_shaking_arr_pos		= 0;

// level stats

u8	__ls_attempts;
u8	__ls_time_hh;
u8	__ls_time_mm;
u16	__ls_time_ss;
u16	__ls_time_tt;


//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
// The main game states switching functions //
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//

void	scene_main_menu()
{
	__draw_main_menu();

	for(;;)
	{
		if( joy( 0 ) & JOY_START_BTN )
		{
			// reset game level
			__map_ind = 0;

			start_game_level( TRUE );

			__draw_main_menu();

			wait_jpad_btns_release();
		}

		//...

		wait_vsync();
	}
}

void __draw_main_menu()
{
	disp_off();
	cls();

	// set black border color
	set_color( 256, 0 );

	// set black screen color
	set_color( 0, 0 );

	put_string( "GAME PROTOTYPE DEMO", 4, 7 );
	put_string( DEMO_VER, 24, 7 );
	put_string( "CONTROLS:", 11, 12 );
	put_string( "<ARROWS> - MOVE", 8, 14 );
	put_string( "<A> - JUMP", 8, 15 );
	put_string( "<B> - ACTION", 8, 16 );
	put_string( "<START> - PAUSE", 8, 18 );
	put_string( "PUSH START", 11, 23 );
	put_string( "(c) 2022 0x8BitDev", 7, 26 );
	put_string( mpd_ver, 27, 26 );
	put_string( spd_ver, 27, 27 );

	disp_on();
	wait_vsync();
}

void	scene_game_over()
{
	u8	time;

	// save current level time and ignore the 10-sec timer
	scene_save_curr_time();

	// set black screen color
	set_color( 0, 0 );

	disp_off();
	wait_vsync();

	scene_clear_screen();

	put_string( "GAME OVER", 11, 13 );
	put_string( "CONTINUE?", 11, 20 );

	disp_on();
	wait_vsync();

	wait_jpad_btns_release();

	clock_reset();

	for(;;)
	{
		time = clock_ss();

		put_number( 9 - time, 1, 15, 22 );

		if( time > 9 )
		{
			// return to the main menu by stack
			break;
		}

		if( joy( 0 ) & JOY_START_BTN )
		{
			start_game_level( FALSE );

			break;
		}

		//...

		wait_vsync();
	}
}

void	scene_level_passed()
{
	u8	next_level;

	// save current level time and ignore the waiting pause
	scene_save_curr_time();

	disp_off();
	wait_vsync();

	// set black screen color
	set_color( 0, 0 );

	scene_clear_screen();

	put_string( "LEVEL    PASSED!", 8, 13 );
	put_number( __map_ind + 101, 2, 14, 13 );

	put_string( "<A> - RESTART", 9, 26 );

	if( __map_ind + 1 < MAPS_CNT )
	{
		next_level = TRUE;
	}
	else
	{
		put_string( "GAME COMPLETED!", 8, 15 );

		next_level = FALSE;
	}

	__show_level_stats();

	disp_on();
	wait_vsync();

	wait_jpad_btns_release();

	for(;;)
	{
		__jpad_val = joy( 0 );

		if( __jpad_val )
		{
			if( __jpad_val & JOY_RESTART_BTN )
			{
				start_game_level( TRUE );

				break;
			}
			else
			if( next_level )
			{
				++__map_ind;

				start_game_level( TRUE );

				break;
			}
			else
			{
				// return to the main menu by stack
				break;
			}
		}

		//...

		wait_vsync();
	}

}

void	__show_level_stats()
{
	put_string( "ATTEMPTS:", 8, 20 );
	put_number( __ls_attempts + 100, 2, 18, 20 );

	put_string( "TIME:", 8, 21 );

	__ls_time_ss += __ls_time_tt / 60;
	__ls_time_mm += __ls_time_ss / 60;

	__ls_time_ss %= 60;
	__ls_time_mm %= 60;

	if( __ls_time_hh ) // hours?! are you kidding me? ))
	{
		put_number( __ls_time_hh + 100, 2, 18, 21 );
		put_string( ":", 20, 21 );
		put_number( __ls_time_mm + 100, 2, 21, 21 );
		put_string( ":", 23, 21 );
		put_number( __ls_time_ss + 100, 2, 24, 21 );
	}
	else
	{
		put_number( __ls_time_mm + 100, 2, 18, 21 );
		put_string( ":", 20, 21 );
		put_number( __ls_time_ss + 100, 2, 21, 21 );
	}

	put_string( "GEMS:", 8, 22 );
	put_number( __player_HUD_data.gems + 100, 2, 18, 22 );
	put_string( "/", 20, 22 );
	put_number( __player_HUD_data.max_gems + 100, 2, 21, 22 );
}

void	start_game_level( u8 _new_level )
{
	wait_jpad_btns_release();

	if( _new_level )
	{
		disp_off();
		wait_vsync();
		cls();

		put_string( "LEVEL", 12, 13 );
		put_number( __map_ind + 101, 2, 18, 13 );

		// reset the last checkpoint for the new game level
		checkpoint_x = checkpoint_y = 0xffff;

		// reset level stats
		__ls_attempts	= 1;
		__ls_time_hh	= 0;
		__ls_time_mm	= 0;
		__ls_time_ss	= 0;
		__ls_time_tt	= 0;

		disp_on();
		wait_vsync();

		clock_reset();

		// waiting for 3 seconds
		for(;;)
		{
			if( clock_ss() == 3 || joy( 0 ) )
			{
				break;
			}

			//...

			wait_vsync();
		}
	}
	else
	{
		// restart the last level
		++__ls_attempts;
	}

	// init a game level timer
	clock_reset();

	if( !_new_level && ( checkpoint_x != 0xffff ) )
	{
		// restore player at checkpoint position
		scene_restore_player( checkpoint_x, checkpoint_y, TRUE );
	}
	else
	{
		scene_init();
	}

	game_update_loop();

	// level state processing
	switch( __level_state )
	{
		case LEVEL_STATE_PASSED:
		{
			scene_level_passed();
		}
		break;

		case LEVEL_STATE_FAILED:
		{
			scene_game_over();
		}
		break;
	}
}

void	scene_pause()
{
	spd_SATB_set_pos( SATB_POS_PAUSE );
	spd_SATB_set_sprite_LT( pause_32x16, 112, 0 );

	// update SATB with the all pushed sprites
	spd_SATB_to_VRAM();

	// save current level time and ignore the pausing time
	scene_save_curr_time();

	// show the 'PAUSE' sprite immediately after pressing the pause button
	wait_vsync();

	wait_jpad_btns_release();

	for(;;)
	{
		if( joy( 0 ) & JOY_PAUSE_BTN )
		{
			wait_jpad_btns_release();

			spd_hide_LT();

			break;
		}

		//...

		wait_vsync();
	}

	// restart level timer after pausing
	clock_reset();
}

void	scene_clear_screen()
{
	cls();

	// clear map scrolling values
	pokew( 0x220c, 0 );
	pokew( 0x2210, 0 );

	// clear sprites
	spd_SATB_clear_from( 0 );
	spd_SATB_to_VRAM();
}

void	wait_jpad_btns_release()
{
	// waiting for releasing jpad buttons
	while( joy( 0 ) ){}
}

//~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
// A game level init function //
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~//

void	scene_init()
{
	// disable display
	disp_off();
	wait_vsync();

	// initialization of a local SATB with sprite data and sending it to VRAM
	sprites_init();

	mpd_init( __map_ind, 1 );

	// draw start screen
	mpd_draw_screen();

	// update scroll values (see mpd.h for details)
	pokew( 0x220c, mpd_scroll_x );
	pokew( 0x2210, mpd_scroll_y );

	mpd_init_screen_arr( &scr_data, __map_ind );

	// get player's entity in a start screen and init player data
	mpd_get_start_screen( &scr_data, __map_ind );

#if	FLAG_ENTITIES
	if( mpd_find_entity_by_base_id( &scr_data, UID_PLAYER ) )
	{
		player_init( scr_data.inst_ent.inst.x_pos, scr_data.inst_ent.inst.y_pos, TRUE );
	}
	else
	{
		show_msg( "Can't find player entity!" );
	}
#else
	show_msg( "NO ENTITIES! RE-EXPORT THE GAME MAP DATA!" );
#endif	//FLAG_ENTITIES

	init_map_entities( __map_ind );

	player_HUD_update_gems_cnt();

	// enable display
	disp_on();
	wait_vsync();

	// set black border color
	set_color( 256, 0 );
}

//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
// A game scene utility functions //
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//

void	scene_start_shaking()
{
	// apply shaking if it is inactive
	if( !__scene_shaking_arr_pos )
	{
		__scene_shaking_arr_pos = 8;
	}
}

void	__scene_shaking()
{
	mpd_clear_update_flags();

	mpd_al = __scene_shaking_vals_arr[ __scene_shaking_arr_pos ];

	if( mpd_al & 0x80 )
	{
		// negative val
		mpd_scroll_step_y = ~mpd_al + 1;	// max 7 pix!

		mpd_move_up();
	}
	else
	{
		mpd_scroll_step_y = mpd_al;		// max 7 pix!

		mpd_move_down();
	}

	// update BAT with tiles if needed, to avoid a map breaking
	mpd_update_screen();

	--__scene_shaking_arr_pos;
}

void	scene_restore_player( s16 _x, s16 _y, u8 _reinit_player )
{
	s16	scr_pos_x;
	s16	scr_pos_y;

	// calc new screen position
	scr_pos_x	= _x - ( ScrPixelsWidth >> 1 );
	scr_pos_y	= _y - ( ScrPixelsHeight >> 1 );

	scr_pos_x	= scr_pos_x < 0 ? 0:scr_pos_x;
	scr_pos_y	= scr_pos_y < 0 ? 0:scr_pos_y;

	scr_pos_x	= ( scr_pos_x > mpd_map_active_width ) ? mpd_map_active_width:scr_pos_x;
	scr_pos_y	= ( scr_pos_y > mpd_map_active_height ) ? mpd_map_active_height:scr_pos_y;

	disp_off();
	wait_vsync();

	// redraw scene at target position
	mpd_draw_screen_by_pos( scr_pos_x, scr_pos_y );

	if( _reinit_player )
	{
		player_init( _x, _y, FALSE );
	}
	else
	{
		// update player position
		spd_set_x_LT( _x - scr_pos_x );
		spd_set_y_LT( _y - scr_pos_y );

		__player_x = _x;
		__player_y = _y;

		PLAYER_STATE_ON_SURFACE
	}

	disp_on();
	wait_vsync();
}

void	scene_save_curr_time()
{
	__ls_time_hh	+= clock_hh();
	__ls_time_mm	+= clock_mm();
	__ls_time_ss	+= clock_ss();
	__ls_time_tt	+= clock_tt();
}

//~~~~~~~~~~~~~~~~~~~~//
// The main game loop //
//~~~~~~~~~~~~~~~~~~~~//

void	game_update_loop()
{
	static s16	delta_x;
	static s16	delta_y;

	__level_state = 0;

	for( ;; )
	{
		__jpad_val = joy( 0 );

		// check scene shaking
		if( __scene_shaking_arr_pos )
		{
			__scene_shaking();
		}

		if( __player_HUD_data.hp )
		{
			player_update();
		}

		delta_x = __player_x - mpd_scroll_x;
		delta_y = __player_y - mpd_scroll_y;

		// update scene position
		mpd_scroll_step_x = __delta_LR & 0x07;
		mpd_scroll_step_y = __delta_UD & 0x07;

		mpd_clear_update_flags();

		// move scene with player
		if( ( __player_flags & PLAYER_FLAGS_MOVE_UP ) && delta_y < SCR_TOP_BORDER )
		{
			mpd_move_up();
		}
		else
		if( ( __player_flags & PLAYER_FLAGS_MOVE_DOWN ) && delta_y > SCR_BOTTOM_BORDER )
		{
			mpd_move_down();
		}

		if( ( __player_flags & PLAYER_FLAGS_MOVE_LEFT ) && delta_x < SCR_LEFT_BORDER )
		{
			mpd_move_left();
		}
		else
		if( ( __player_flags & PLAYER_FLAGS_MOVE_RIGHT ) && delta_x > SCR_RIGHT_BORDER )
		{
			mpd_move_right();
		}

		// use updated scroll values to synchronize player's position with the scene coordinates
		player_update_pos( __player_x - mpd_scroll_x, __player_y - mpd_scroll_y );

		SATB_pos = SATB_POS_ENTITY;	// 0 - reserved for player, the next positions for HUD and PAUSE sprite
		// set initial SATB pos
		spd_SATB_set_pos( SATB_pos );
		// clear previous entity sprites
		spd_SATB_clear_from( SATB_pos );

#if	!DBG_ENTITIES_OFF

		// check player collision
		if( __spr_collision )
		{
DBG_PLAYER_COLLISION

			check_player_collisions();

			__spr_collision = 0;
		}

DBG_UPDATE_ENTITIES

		update_scene_entities();

#endif	//!DBG_ENTITIES_ON

DBG_BLACK_BORDER

		if( __enemy_hit )
		{
			player_update_damage_state();
		}

#if	DBG_SHOW_INFO
put_string( "ccach:", 3, 0 );
put_number( __collision_cache_pos, 2, 9, 0 );
put_string( "ents :", 3, 1 );
put_number( __screen_ent_cache_pos, 2, 9, 1 );
put_string( "cents:", 3, 2 );
put_number( __screen_ent_coll_cache_pos, 2, 9, 2 );
put_string( "dmnds:", 3, 3 );
put_number( __player_HUD_data.gems, 2, 9, 3 );
put_string( "/", 11, 3 );
put_number( __player_HUD_data.max_gems, 3, 12, 3 );
#endif

		// update BAT with tiles
		mpd_update_screen();

		// see mpd.h for details
		pokew( 0x220c, mpd_scroll_x );
		pokew( 0x2210, mpd_scroll_y );

		// update SATB with the all pushed sprites
		spd_SATB_to_VRAM();

		wait_vsync();

		// flashing the warning sign - red/white
		// palette color(s) update should be placed on VBLANK to avoid glitches
		set_color( 26, ( ++__warn_sign_cnt & 0x10 ) ? 511:56 );

		if( __jpad_val & JOY_PAUSE_BTN )
		{
			scene_pause();
		}

		if( __level_state && !__enemy_hit )
		{
			// the level state has changed: passed, failed
			break;
		}
	}
}