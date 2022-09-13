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


//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
// The main game states switching functions //
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//

void	scene_main_menu()
{
	disp_off();
	cls();

	// set black border color
	set_color( 256, 0 );

	// set black screen color
	set_color( 0, 0 );

	put_string( "GAME PROTOTYPE DEMO", 4, 7 );
	put_string( DEMO_VER, 24, 7 );
	put_string( "CONTROLS:", 11, 13 );
	put_string( "<ARROWS> - MOVE", 8, 15 );
	put_string( "<A> - JUMP", 8, 16 );
	put_string( "<B> - ACTION", 8, 17 );
	put_string( "PUSH START", 11, 23 );
	put_string( "(c) 2022 0x8BitDev", 7, 26 );
	put_string( mpd_ver, 27, 26 );
	put_string( spd_ver, 27, 27 );

	disp_on();
	wait_vsync();

	for(;;)
	{
		if( joy( 0 ) & JOY_STRT )
		{
			// reset game level
			__map_ind = 0;

			start_game_level( TRUE );
		}

		//...

		wait_vsync();
	}
}

void	scene_game_over()
{
	u8	time;

	disp_off();
	wait_vsync();
	cls();

	put_string( "GAME OVER", 11, 13 );
	put_string( "CONTINUE?", 11, 20 );

	// clear map scrolling values
	pokew( 0x220c, 0 );
	pokew( 0x2210, 0 );

	// clear sprites
	spd_SATB_clear_from( 0 );
	spd_SATB_to_VRAM();

	disp_on();
	wait_vsync();

	// waiting for releasing jpad buttons
	while( joy( 0 ) ){}

	clock_reset();

	for(;;)
	{
		time = clock_ss();

		put_number( 9 - time, 1, 15, 22 );

		if( time > 9 )
		{
			scene_main_menu();
		}

		if( joy( 0 ) )
		{
			start_game_level( FALSE );
		}

		//...

		wait_vsync();
	}
}

void	start_game_level( u8 _new_level )
{
	// waiting for releasing jpad buttons
	while( joy( 0 ) ){}

	if( _new_level )
	{
		disp_off();
		wait_vsync();
		cls();

		put_string( "LEVEL", 12, 13 );
		put_number( __map_ind + 1, 1, 18, 13 );

		// reset the last checkpoint for the new game level
		checkpoint_x = checkpoint_y = 0xffff;

		disp_on();
		wait_vsync();

		clock_reset();

		// waiting 3 seconds
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

	// enable display
	disp_on();
	wait_vsync();

	// set black border color
	set_color( 256, 0 );
}

//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
// A game scene utility functions //
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//

void	scene_shake()
{
	//...
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

//~~~~~~~~~~~~~~~~~~~~//
// The main game loop //
//~~~~~~~~~~~~~~~~~~~~//

void	game_update_loop()
{
	s16	delta_x;
	s16	delta_y;

	for( ;; )
	{
		__jpad_val = joy( 0 );

		player_update();

		delta_x = __player_x - mpd_scroll_x;
		delta_y = __player_y - mpd_scroll_y;

		// update scene position
		mpd_scroll_step_x = __delta_LR & 0x07;
		mpd_scroll_step_y = __delta_UD & 0x07;

		mpd_clear_update_flags();

		// move scene with player
		if( ( __mvmnt_flags & MVMNT_FLAG_UP ) && delta_y < SCR_TOP_BORDER )
		{
			mpd_move_up();
		}
		else
		if( ( __mvmnt_flags & MVMNT_FLAG_DOWN ) && delta_y > SCR_BOTTOM_BORDER )
		{
			mpd_move_down();
		}

		if( ( __mvmnt_flags & MVMNT_FLAG_LEFT ) && delta_x < SCR_LEFT_BORDER )
		{
			mpd_move_left();
		}
		else
		if( ( __mvmnt_flags & MVMNT_FLAG_RIGHT ) && delta_x > SCR_RIGHT_BORDER )
		{
			mpd_move_right();
		}

		// use updated scroll values to synchronize player's position with the scene coordinates
		player_update_pos( __player_x - mpd_scroll_x, __player_y - mpd_scroll_y );

		SATB_pos = SATB_POS_ENTITY;	// 0 - reserved for player, the next positions for HUD
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

		// flashing the warning sign - red/white
		set_color( 26, ( ++__warn_sign_cnt & 0x10 ) ? 511:56 );

#endif	//!DBG_ENTITIES_ON

DBG_BLACK_BORDER

		if( __enemy_hit )
		{
			player_update_hit();
		}

#if	DBG_SHOW_INFO
put_string( "ccach:", 3, 0 );
put_number( __collision_cache_pos, 2, 9, 0 );
put_string( "ents :", 3, 1 );
put_number( __screen_ent_cache_pos, 2, 9, 1 );
put_string( "cents:", 3, 2 );
put_number( __screen_ent_coll_cache_pos, 2, 9, 2 );
put_string( "dmnds:", 3, 3 );
put_number( __player_data.diamonds, 2, 9, 3 );
put_string( "/", 11, 3 );
put_number( __player_data.max_diamonds, 3, 12, 3 );
#endif

		// update BAT with tiles
		mpd_update_screen();

		// see mpd.h for details
		pokew( 0x220c, mpd_scroll_x );
		pokew( 0x2210, mpd_scroll_y );

		// update SATB with the all pushed sprites
		spd_SATB_to_VRAM();

		wait_vsync();
	}
}