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


void	scene_init()
{
	/* disable display */
	disp_off();
	vsync();

	mpd_init( __map_ind, 1 );

	/* draw start screen */
	mpd_draw_screen();

	mpd_init_screen_arr( &scr_data, __map_ind );

	/* get player's entity in a start screen and init player data */
	mpd_get_start_screen( &scr_data, __map_ind );

#if	FLAG_ENTITIES
	if( mpd_find_entity_by_base_id( &scr_data, UID_PLAYER ) )
	{
		player_init( scr_data.inst_ent );
	}
	else
	{
		show_msg( "Can't find player entity!" );
	}
#else
	show_msg( "NO ENTITIES! RE-EXPORT THE GAME MAP DATA!" );
#endif	//FLAG_ENTITIES

	init_map_entities( __map_ind );

	/* enable display */
	disp_on();
	vsync();
}

void	scene_shake()
{
	//...
}

void	scene_update()
{
	s16	delta_x;
	s16	delta_y;

	delta_x = __player_x - mpd_scroll_x;
	delta_y = __player_y - mpd_scroll_y;

	// update scene position
	mpd_scroll_step_x = __delta_LR & 0x07;
	mpd_scroll_step_y = __delta_UD & 0x07;

	mpd_clear_update_flags();

	// move scene with the player
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
	// clear previous sprites except the player one
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

	// the warning sign flashing - red/white
	set_color( 27, ( ++__warn_sign_cnt & 0x10 ) ? 511:56 );

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

	/* see mpd.h for details */
	pokew( 0x220c, mpd_scroll_x );
	pokew( 0x2210, mpd_scroll_y );

	/* update SATB with the all pushed sprites */
	spd_SATB_to_VRAM();
}