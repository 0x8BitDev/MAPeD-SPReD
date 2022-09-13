//##################################################################
//
// Copyright 2021-2022 0x8BitDev ( MIT license. See LICENSE.txt )
//
// This file is a part of the game prototype demo.
//
//##################################################################

#define	JOY_UP_BTN		JOY_UP
#define	JOY_DOWN_BTN		JOY_DOWN
#define	JOY_LEFT_BTN		JOY_LEFT
#define	JOY_RIGHT_BTN		JOY_RIGHT
#define	JOY_JUMP_BTN		JOY_A
#define	JOY_ACTION_BTN		JOY_B

#define	HEAD_COLLISIONS		1

#define TILE_PROP_PLATFORM	1
#define TILE_PROP_LADDER	2
#define TILE_PROP_WALL		3
#define TILE_PROP_DAMAGE	4
#define TILE_PROP_JUMPER	5

#define	PLAYER_WIDTH		16
#define	PLAYER_HEIGHT		32

#define PLAYER_MAX_HEALTH_POINTS	6

// contact points for collisions
const u8 PLAYER_CP_GRND_LT	= (( PLAYER_WIDTH >> 1 ) - 2 );
const u8 PLAYER_CP_GRND_RT	= (( PLAYER_WIDTH >> 1 ) + 2 );
const u8 PLAYER_CP_HEAD_LT	= (( PLAYER_WIDTH >> 1 ) - 5 );
const u8 PLAYER_CP_HEAD_RT	= (( PLAYER_WIDTH >> 1 ) + 5 );
const u8 PLAYER_CP_LT_RT	= ( PLAYER_HEIGHT - 7 );
const u8 PLAYER_CP_MID_WIDTH	= ( PLAYER_WIDTH >> 1 );
const u8 PLAYER_CP_LADDER	= PLAYER_CP_MID_WIDTH;

#define	PLAYER_STATE_ON_SURFACE	__jump_acc = __jump_acc_max = JUMP_ACC_VAL; __fall_acc = 0;
#define	PLAYER_IS_FALLING	__fall_acc

#define MOVE_SPEED_LADDER	2	// pix/frame

#define MOVE_LR_ACC_VAL		16
#define MOVE_LR_ACC_CAP		2	// obj_pos +-= MOVE_ACC_VAL >> MOVE_ACC_CAP

#define JUMP_ACC_VAL		16
#define JUMPER_ACC_VAL		20
#define JUMP_ACC_CAP		1	// obj_pos +-= MOVE_ACC_VAL >> MOVE_ACC_CAP

#define FALL_ACC_VAL		16
#define FALL_ACC_CAP		1	// obj_pos +-= MOVE_ACC_VAL >> MOVE_ACC_CAP

// player dynamic data
s16	__player_x		= 0;
s16	__player_y		= 0;
u8	__move_left_acc		= 0;
u8	__move_right_acc	= 0;

u8	__jump_acc	= 0;
u8	__jump_acc_max	= 0;
u8	__fall_acc	= 0;

u8	__delta_LR	= 0;
u8	__delta_UD	= 0;

s16	__enemy_hit	= 0;

typedef struct
{
	u8	max_diamonds;
	u8	diamonds;

	u8	hp;

} PLAYER_DATA;

PLAYER_DATA	__player_data;

// movement flags
u8	__mvmnt_flags	= 0;

// variables scope
s16	map_pos_x;
s16	map_pos_y;
u8	cp1_prop;
u8	cp2_prop;

#if	HEAD_COLLISIONS
u8	cp_lt_res;
u8	cp_rt_res;
u8	cont_jump;
#endif

#define	MVMNT_FLAG_LEFT		0x01
#define	MVMNT_FLAG_RIGHT	0x02
#define	MVMNT_FLAG_UP		0x04
#define	MVMNT_FLAG_DOWN		0x08


void	player_init( u16 _x, u16 _y, u8 _reset_progress )
{
	u8	i;
	u8	x_offs;
	u8	satb_pos;

	if( _reset_progress )
	{
		__player_data.max_diamonds	= 0;
		__player_data.diamonds		= 0;
	}

	__player_data.hp	= PLAYER_MAX_HEALTH_POINTS;

	__enemy_hit	= 0;

	__player_x	= _x;
	__player_y	= _y;

	__move_left_acc		= 0;
	__move_right_acc	= 0;

	PLAYER_STATE_ON_SURFACE

	// set the SATB position to push sprites to
	spd_SATB_set_pos( SATB_POS_PLAYER );

	// push player sprite into SATB
	spd_SATB_set_sprite_LT( sprites_frames_data, PLAYER_SPRITE_ID, __player_x, __player_y );

	// init HUD
	{
		spd_SATB_set_pos( SATB_POS_HUD );
		satb_pos = SATB_POS_HUD;

		x_offs = 0;

		for( i = 0; i < ( PLAYER_MAX_HEALTH_POINTS >> 1 ); i++ )
		{
			spd_SATB_set_sprite_LT( live_heart, x_offs, 0 );	// blue heart by default
			x_offs += 16;

			spd_set_palette_LT( 17 );	// red heart

			spd_SATB_set_pos( ++satb_pos );
		}
	}
}

// check contact points

u8	__check_ground_cp()
{
	map_pos_x = __player_x;
	map_pos_y = __player_y + PLAYER_HEIGHT;

	cp1_prop = mpd_get_property( map_pos_x + PLAYER_CP_MID_WIDTH, __player_y + PLAYER_CP_LT_RT );

	if( cp1_prop == TILE_PROP_DAMAGE )
	{
		player_enemy_hit();
	}

	if( cp1_prop == TILE_PROP_JUMPER )
	{
		if( __fall_acc )	// falling?
		{
			__jump_acc_max = JUMPER_ACC_VAL;
			__jump_acc = __jump_acc_max - 1;	// jump when falling
			__fall_acc = 0xff;

			__player_y &= ~0x07;

			return 0;
		}
	}

	cp1_prop = mpd_get_property( map_pos_x + PLAYER_CP_GRND_LT, map_pos_y );
	cp2_prop = mpd_get_property( map_pos_x + PLAYER_CP_GRND_RT, map_pos_y );

	if( cp1_prop == TILE_PROP_PLATFORM || cp1_prop == TILE_PROP_WALL || cp2_prop == TILE_PROP_PLATFORM || cp2_prop == TILE_PROP_WALL )
	{
		__player_y &= ~0x07;

		// check mid ground position
		cp1_prop = mpd_get_property( map_pos_x + PLAYER_CP_MID_WIDTH, map_pos_y - 1 );

		if( cp1_prop == TILE_PROP_PLATFORM || cp1_prop == TILE_PROP_WALL )
		{
			--__player_y;
		}

		return 1;
	}

	if( cp1_prop == TILE_PROP_LADDER || cp2_prop == TILE_PROP_LADDER )
	{
		return 1;
	}

	return 0;
}

u8	__check_ladder( u8 _offs_y )
{
	map_pos_x = __player_x + PLAYER_CP_LADDER;
	map_pos_y = __player_y + PLAYER_HEIGHT;

	if( mpd_get_property( map_pos_x, map_pos_y - _offs_y ) == TILE_PROP_LADDER )
	{
		return 1;
	}

	return 0;
}

#if	HEAD_COLLISIONS
u8	__check_head_cp()
{
	cont_jump = 1;

	map_pos_x = __player_x;
	map_pos_y = __player_y;

	cp_lt_res = mpd_get_property( map_pos_x + PLAYER_CP_HEAD_LT, map_pos_y ) == TILE_PROP_WALL;
	cp_rt_res = mpd_get_property( map_pos_x + PLAYER_CP_HEAD_RT, map_pos_y ) == TILE_PROP_WALL;

	if( cp_lt_res && !cp_rt_res )
	{
		++__player_x;

		cont_jump = 0;
	}
	else
	if( !cp_lt_res && cp_rt_res )
	{
		--__player_x;

		cont_jump = 0;
	}

	if( cp_lt_res || cp_rt_res )
	{
		__player_y += 8;
		__player_y &= ~0x07;

		__delta_UD = 0;//!!!

		return cont_jump;
	}

	return 0;
}
#endif	//HEAD_COLLISIONS

void	__check_left_cp()
{
	map_pos_x = __player_x;
	map_pos_y = __player_y + PLAYER_CP_LT_RT;

	cp1_prop = mpd_get_property( map_pos_x, map_pos_y );

	if( cp1_prop == TILE_PROP_DAMAGE )
	{
		player_enemy_hit();
	}

	if( cp1_prop == TILE_PROP_WALL )
	{
		__player_x += 8;
		__player_x &= ~0x07;
	}
}

void	__check_right_cp()
{
	map_pos_x = __player_x + PLAYER_WIDTH;
	map_pos_y = __player_y + PLAYER_CP_LT_RT;

	cp1_prop = mpd_get_property( map_pos_x, map_pos_y );

	if( cp1_prop == TILE_PROP_DAMAGE )
	{
		player_enemy_hit();
	}

	if( cp1_prop == TILE_PROP_WALL )
	{
		__player_x &= ~0x07;
	}
}

void	__check_ground()
{
	// check ground if player isn't jumping
	if( __check_ground_cp() )
	{
		PLAYER_STATE_ON_SURFACE
	}
	else
	{
		++__fall_acc;	// falling
	}
}

void	player_update()
{
	__delta_LR	= 0;
	__delta_UD	= 0;
	__mvmnt_flags	= 0;

	// UP / DOWN
	if( __jpad_val & JOY_UP_BTN )
	{
		if( __check_ladder( 1 ) )
		{
			__delta_UD = MOVE_SPEED_LADDER;
			__player_y -= __delta_UD;

			__mvmnt_flags |= MVMNT_FLAG_UP;
		}
	}
	else
	if( __jpad_val & JOY_DOWN_BTN )
	{
		if( __check_ladder( 0 ) )
		{
			__delta_UD = MOVE_SPEED_LADDER;
			__player_y += __delta_UD;

			__mvmnt_flags |= MVMNT_FLAG_DOWN;
		}
	}

	// JUMPING
	if( __jpad_val & JOY_JUMP_BTN )
	{
		if( __jump_acc == __jump_acc_max )
		{
			--__jump_acc;
		}
	}

	// FALLING
	if( __fall_acc )
	{
		if( ++__fall_acc > FALL_ACC_VAL )
		{
			__fall_acc = FALL_ACC_VAL;
		}

		__delta_UD = __fall_acc >> FALL_ACC_CAP;

		// the scroll step must be in range 1-7
		if( __delta_UD > 7 )
		{
			--__delta_UD;
		}

		__player_y += __delta_UD;

		__mvmnt_flags |= MVMNT_FLAG_DOWN;

		__check_ground();
	}
	else
	if( __jump_acc < __jump_acc_max && __jump_acc )
	{
		if( --__jump_acc <= 0 )
		{
			__jump_acc = 0;

			++__fall_acc;	// start falling
		}

		__delta_UD = __jump_acc >> JUMP_ACC_CAP;
		__player_y -= __delta_UD;

		__mvmnt_flags |= MVMNT_FLAG_UP;

#if	HEAD_COLLISIONS
		if( __check_head_cp() )
		{
			__jump_acc = 0;

			++__fall_acc;	// start falling
		}
#endif	//HEAD_COLLISIONS
	}
	else
	{
		__check_ground();
	}

	// LEFT / RIGHT
	if( __jpad_val & JOY_LEFT_BTN )
	{
		if( ++__move_left_acc > MOVE_LR_ACC_VAL )
		{
			__move_left_acc = MOVE_LR_ACC_VAL;
		}

		if( --__move_right_acc < 0 )
		{
			__move_right_acc = 0;
		}
	}
	else
	if( __jpad_val & JOY_RIGHT_BTN )
	{
		if( ++__move_right_acc > MOVE_LR_ACC_VAL )
		{
			__move_right_acc = MOVE_LR_ACC_VAL;
		}

		if( --__move_left_acc < 0 )
		{
			__move_left_acc = 0;
		}
	}
	else
	{
		if( --__move_right_acc < 0 )
		{
			__move_right_acc = 0;
		}

		if( --__move_left_acc < 0 )
		{
			__move_left_acc = 0;
		}
	}

	if( __move_right_acc )
	{
		__delta_LR = __move_right_acc >> MOVE_LR_ACC_CAP;
		__player_x += __delta_LR;

		__mvmnt_flags |= MVMNT_FLAG_RIGHT;

		__check_right_cp();
	}

	if( __move_left_acc )
	{
		__delta_LR = __move_left_acc >> MOVE_LR_ACC_CAP;
		__player_x -= __delta_LR;

		__mvmnt_flags |= MVMNT_FLAG_LEFT;

		__check_left_cp();
	}
}

void	player_enemy_hit()
{
	u8 satb_pos;

	if( !__enemy_hit )
	{
		__enemy_hit = 1000;

		clock_reset();

		if( !--__player_data.hp )
		{
			// GAME OVER
			scene_game_over();
		}
		else
		{
			// update HUD lives
			satb_pos = spd_SATB_get_pos();

			spd_SATB_set_pos( SATB_POS_HUD + ( __player_data.hp >> 1 ) );

			if( __player_data.hp & 0x01 )
			{
				spd_set_palette_LT( 18 );	// blue heart
			}
			else
			{
				spd_hide_LT();
			}

			spd_SATB_set_pos( satb_pos );
		}
	}
}

void	player_update_pos( s16 _x, s16 _y )
{
	// update player's sprite position
	spd_SATB_set_pos( SATB_POS_PLAYER );

	spd_set_x_LT( _x );
	spd_set_y_LT( _y );
}

void	player_update_hit()
{
	u8 tick;

	spd_SATB_set_pos( SATB_POS_PLAYER );

	tick = clock_tt();

	if( tick & 0x04 )
	{
		spd_hide_LT();
		return;
	}

	__enemy_hit -= tick;

	if( __enemy_hit < 0 )
	{
		__enemy_hit = 0;
	}

	spd_show_LT();
}