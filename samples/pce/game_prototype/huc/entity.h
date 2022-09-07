//##################################################################
//
// Copyright 2021-2022 0x8BitDev ( MIT license. See LICENSE.txt )
//
// This file is a part of the game prototype demo.
//
//##################################################################

// entity uids

#define UID_PLAYER			0
#define	UID_ENT_COLLECTABLE_STAR	1	// ignores collision cache
#define	UID_ENT_ENEMY_WALKING		2	// collision cache
#define	UID_ENT_ENEMY_FLYING		3	// collision cache
#define	UID_ENT_SWITCH			4	// collision cache
#define	UID_ENT_BUTTON			5	// collision cache
#define	UID_ENT_PLATFORM		6	// ignores collision cache - enables collision detection every frame
#define	UID_ENT_LOGS			7	// ignores collision cache - enables collision detection every frame
#define	UID_ENT_DOOR			8	// ignores collision cache - enables collision detection every frame
#define	UID_ENT_HEAVY_LOAD		9	// ignores collision cache - enables collision detection every frame
#define	UID_ENT_PORTAL			10	// collision cache

// sprite name aliases

#define	ENT_SPR_COLLECTABLE_STAR	collectable_star
#define	ENT_SPR_ENEMY_WALKING_LEFT	enemy_walking_16x32_1_LEFT
#define	ENT_SPR_ENEMY_WALKING_RIGHT	enemy_walking_16x32_0_RIGHT
#define	ENT_SPR_ENEMY_FLYING_LEFT	enemy_flying_16x16_LEFT
#define	ENT_SPR_ENEMY_FLYING_RIGHT	enemy_flying_16x16_RIGHT
#define	ENT_SPR_SWITCH_OFF		switch_off
#define	ENT_SPR_SWITCH_ON		switch_on
#define	ENT_SPR_BUTTON_OFF		button_off
#define	ENT_SPR_BUTTON_ON		button_on
#define	ENT_SPR_PLATFORM		platform_32x8
#define	ENT_SPR_LOGS			logs_32x16
#define	ENT_SPR_DOOR			door_16x32_0
#define	ENT_SPR_HEAVY_LOAD		heavy_load_32x64
#define	ENT_SPR_PORTAL			portal_16x32_1

// static entities data
u8	base_ent_coll_star_width	= 0;
u8	base_ent_coll_star_height	= 0;

#define	ENT_ENEMY_WALKING_WIDTH			16
#define	ENT_ENEMY_FLYING_WIDTH			16
#define	ENT_PLATFORM_WIDTH			32
#define	ENT_PLATFORM_HEIGHT			8
#define	ENT_LOGS_PLATFORM_HEIGHT		8
#define	ENT_DOOR_HEIGHT				32
#define	ENT_HEAVY_LOAD_HEIGHT			64

#define	ENT_DOOR_LOOP_PAUSE_IN_FRAMES		0
#define	ENT_HEAVY_LOAD_LOOP_PAUSE_IN_FRAMES	60

#define	ENT_ID( id )	id << 12

// global variables scope

u8	ent_n;
u8	SATB_pos;
u8	scr_pos;

u8	map_coll_ent;
u8	map_ent;

u8	tmp_val8;

s16	ent_x;
s16	ent_y;
s16	ent_y_unmasked;

s16	player_x;
s16	player_y;

mpd_MAP_ENT*			ent_ptr;
mpd_MAP_COLLECTABLE_ENT*	ent_coll_ptr;

mpd_SCR_DATA			scr_data;

// end of the global vars scope

#define	IS_PLAYER_INTERSECT_BOX( box_x, box_y, box_width, box_height ) ( player_x < ( box_x + box_width ) ) && ( ( player_x + PLAYER_WIDTH ) > box_x ) && ( player_y < ( box_y + box_height ) ) && ( ( player_y + PLAYER_HEIGHT ) > box_y )

//**************************************************************************
//
// 	There are a few steps to work with entities:
//	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//	1. Map entities initialization.
//	2. Entities update and SATB/screen cache filling.
//	3. Cached entities update.
//	4. Player/entity collision detection.
//
//	[!] An entity UID is the index of the entity processing function.
//
//**************************************************************************

//************************************************************
// 1. Map entities initialization.
//************************************************************

void	__fastcall	init_map_entity( u8 _ind<acc> );	// entity ID is an entity index

#asm
	.proc _init_map_entity.1

	txa
	asl a
	tax

	jmp [_init_ent_arr, x]

	.endp

_init_ent_arr:

	.dw _init_ent_null
	.dw _init_ent_coll_star
	.dw _init_ent_enemy_walking
	.dw _init_ent_enemy_flying
	.dw _init_ent_switch
	.dw _init_ent_button
	.dw _init_ent_platform
	.dw _init_ent_logs
	.dw _init_ent_door
	.dw _init_ent_heavy_load
	.dw _init_ent_portal

_init_ent_null:
	call _init_null
	rts

_init_ent_coll_star:
	call _init_coll_star
	rts

_init_ent_enemy_walking:
_init_ent_enemy_flying:
_init_ent_switch:
_init_ent_button:
_init_ent_platform:
_init_ent_logs:
_init_ent_door:
_init_ent_heavy_load:
_init_ent_portal:
	call _init_entity
	rts

#endasm

void	init_null()
{
	//...
}

void	init_coll_star()
{
	// get entity size
	base_ent_coll_star_width	= scr_data.inst_ent.base.width;
	base_ent_coll_star_height	= scr_data.inst_ent.base.height;

	if( add_collectable_entity() )
	{
		++__player_data.max_stars;
	}
}

void	init_entity()
{
	add_entity();
}

//************************************************************
// 2. Entities update and SATB/screen cache filling.
//************************************************************

u8	__fastcall	update_global_entity( u8 _ind<acc> );	// entity ID is an entity index

#asm
	.proc _update_global_entity.1

	lsr a
	lsr a
	lsr a
	tax

	jmp [_upd_ent_arr, x]

	.endp

_upd_ent_arr:

	.dw _upd_ent_null
	.dw _upd_ent_coll_star
	.dw _upd_ent_enemy_walking
	.dw _upd_ent_enemy_flying
	.dw _upd_ent_switch
	.dw _upd_ent_button
	.dw _upd_ent_platform
	.dw _upd_ent_logs
	.dw _upd_ent_door
	.dw _upd_ent_heavy_load
	.dw _upd_ent_portal

_upd_ent_null:
	call _update_null
	rts

_upd_ent_coll_star:
	call _update_coll_star
	rts

_upd_ent_enemy_walking:
	call _update_enemy_walking
	rts

_upd_ent_enemy_flying:
	call _update_enemy_flying
	rts

_upd_ent_switch:
	call _update_switch
	rts

_upd_ent_button:
	call _update_button
	rts

_upd_ent_platform:
	call _update_platform
	rts

_upd_ent_logs:
	call _update_logs
	rts

_upd_ent_door:
	call _update_door
	rts

_upd_ent_heavy_load:
	call _update_heavy_load
	rts

_upd_ent_portal:
	call _update_portal
	rts

#endasm

u8	update_null()
{
	return 0;	// ignore entity
}

u8	update_coll_star()
{
	if( ( ( ent_x + base_ent_coll_star_width ) > 0 ) && ( ( ent_y_unmasked + base_ent_coll_star_height ) > 0 ) )
	{
		update_cached_coll_star();

		return 1;
	}

	return 0;
}

u8	update_enemy_walking()
{
	if( ( ( ent_x + ent_ptr->width ) > 0 ) && ( ( ent_y_unmasked + ent_ptr->height ) > 0 ) )
	{
		update_cached_enemy_walking();

		return 1;
	}

	return 0;
}

u8	update_enemy_flying()
{
	if( ( ( ent_x + ent_ptr->width ) > 0 ) && ( ( ent_y_unmasked + ent_ptr->height ) > 0 ) )
	{
		update_cached_enemy_flying();

		return 1;
	}

	return 0;
}

u8	update_switch()
{
	if( ( ( ent_x + ent_ptr->width ) > 0 ) && ( ( ent_y_unmasked + ent_ptr->height ) > 0 ) )
	{
		update_cached_switch();

		return 1;
	}

	return 0;
}

u8	update_button()
{
	if( ( ( ent_x + ent_ptr->width ) > 0 ) && ( ( ent_y_unmasked + ent_ptr->height ) > 0 ) )
	{
		update_cached_button();

		return 1;
	}

	return 0;
}

u8	update_platform()
{
	if( ( ( ent_x + ent_ptr->width ) > 0 ) && ( ( ent_y_unmasked + ent_ptr->height ) > 0 ) )
	{
		update_cached_platform();

		return 1;
	}

	return 0;
}

u8	update_logs()
{
	if( ( ( ent_x + ent_ptr->width ) > 0 ) && ( ( ent_y_unmasked + ent_ptr->height ) > 0 ) )
	{
		update_cached_logs();

		return 1;
	}

	return 0;
}

u8	update_door()
{
	if( ( ( ent_x + ent_ptr->width ) > 0 ) && ( ( ent_y_unmasked + ent_ptr->height ) > 0 ) )
	{
		update_cached_door();

		return 1;
	}

	return 0;
}

u8	update_heavy_load()
{
	if( ( ( ent_x + ent_ptr->width ) > 0 ) && ( ( ent_y_unmasked + ent_ptr->height ) > 0 ) )
	{
		update_cached_heavy_load();

		return 1;
	}

	return 0;
}

u8	update_portal()
{
	if( ( ( ent_x + ent_ptr->width ) > 0 ) && ( ( ent_y_unmasked + ent_ptr->height ) > 0 ) )
	{
		update_cached_portal();

		return 1;
	}

	return 0;
}

//************************************************************
// 3. Cached entities update.
//************************************************************

void	__fastcall	update_cached_entity( u8 _ind<acc> );	// entity ID is an entity index

#asm
	.proc _update_cached_entity.1

	lsr a
	lsr a
	lsr a
	tax

	jmp [_upd_cached_ent_arr, x]

	.endp

_upd_cached_ent_arr:

	.dw _upd_cached_ent_null
	.dw _upd_cached_ent_coll_star
	.dw _upd_cached_ent_enemy_walking
	.dw _upd_cached_ent_enemy_flying
	.dw _upd_cached_ent_switch
	.dw _upd_cached_ent_button
	.dw _upd_cached_ent_platform
	.dw _upd_cached_ent_logs
	.dw _upd_cached_ent_door
	.dw _upd_cached_ent_heavy_load
	.dw _upd_cached_ent_portal

_upd_cached_ent_null:
	call _update_cached_null
	rts

_upd_cached_ent_coll_star:
	call _update_cached_coll_star
	rts

_upd_cached_ent_enemy_walking:
	call _update_cached_enemy_walking
	rts

_upd_cached_ent_enemy_flying:
	call _update_cached_enemy_flying
	rts

_upd_cached_ent_switch:
	call _update_cached_switch
	rts

_upd_cached_ent_button:
	call _update_cached_button
	rts

_upd_cached_ent_platform:
	call _update_cached_platform
	rts

_upd_cached_ent_logs:
	call _update_cached_logs
	rts

_upd_cached_ent_door:
	call _update_cached_door
	rts

_upd_cached_ent_heavy_load:
	call _update_cached_heavy_load
	rts

_upd_cached_ent_portal:
	call _update_cached_portal
	rts

#endasm

void	update_cached_null()
{
	//...
}

void	update_cached_coll_star()
{
	ENT_ADD_TO_SATB
	spd_SATB_set_sprite_LT( ENT_SPR_COLLECTABLE_STAR, ent_x, ent_y_unmasked );
}

#asm
__eny_y_offs:
	.db $1, $1, $1, $1, $0, $0, $0, $0
#endasm

//
// prop0: 7-bit: 1 - RIGHT, 0-LEFT; 6-0: distance
// prop1: 7-0: current distance
// prop2: 7-0: pause in frames
// prop3: 7-0: current pause value
//
void	update_cached_enemy_walking()
{
	ENT_ADD_TO_SATB

	if( ent_ptr->prop0 & 0x80 )	// RIGHT
	{
		if( ent_ptr->prop3 )
		{
			--ent_ptr->prop3;

			spd_SATB_set_sprite_LT( ENT_SPR_ENEMY_WALKING_LEFT, ent_x, ent_y_unmasked );
			return;
		}

		++ent_ptr->prop1;

		ent_x += ent_ptr->prop1;
#asm
	lda _ent_x
	and #$07
	tax

	lda __eny_y_offs, x

	clc
	adc _ent_y_unmasked
	sta _ent_y_unmasked
	bcc .skip_hb_1
	inc _ent_y_unmasked + 1
.skip_hb_1:
#endasm
		spd_SATB_set_sprite_LT( ENT_SPR_ENEMY_WALKING_RIGHT, ent_x, ent_y_unmasked );

		if( ent_ptr->prop1 >= ( ent_ptr->prop0 & 0x7f ) )
		{
			ent_ptr->prop0 ^= 0x80;
			ent_ptr->prop3 = ent_ptr->prop2;
		}
	}
	else	// LEFT
	{
		if( ent_ptr->prop3 )
		{
			--ent_ptr->prop3;

			spd_SATB_set_sprite_LT( ENT_SPR_ENEMY_WALKING_RIGHT, ent_x + ent_ptr->prop1, ent_y_unmasked );
			return;
		}

		--ent_ptr->prop1;

		ent_x += ent_ptr->prop1;
#asm
	lda _ent_x
	and #$07
	tax

	lda __eny_y_offs, x

	clc
	adc _ent_y_unmasked
	sta _ent_y_unmasked
	bcc .skip_hb_2
	inc _ent_y_unmasked + 1
.skip_hb_2:
#endasm
		spd_SATB_set_sprite_LT( ENT_SPR_ENEMY_WALKING_LEFT, ent_x, ent_y_unmasked );

		if( !ent_ptr->prop1 )
		{
			ent_ptr->prop0 ^= 0x80;
			ent_ptr->prop3 = ent_ptr->prop2;
		}
	}
}

//
// prop0: 7-bit: 1 - RIGHT, 0-LEFT; 6-0: distance
// prop1: 7-0: current distance
//
void	update_cached_enemy_flying()
{
	ENT_ADD_TO_SATB

	if( ent_ptr->prop0 & 0x80 )	// RIGHT
	{
		++ent_ptr->prop1;

		ent_x += ent_ptr->prop1;
#asm
	lda _ent_x
	and #$07
	tax

	lda __eny_y_offs, x

	clc
	adc _ent_y_unmasked
	sta _ent_y_unmasked
	bcc .skip_hb_1
	inc _ent_y_unmasked + 1
.skip_hb_1:
#endasm
		spd_SATB_set_sprite_LT( ENT_SPR_ENEMY_FLYING_RIGHT, ent_x, ent_y_unmasked );

		if( ent_ptr->prop1 >= ( ent_ptr->prop0 & 0x7f ) )
		{
			ent_ptr->prop0 ^= 0x80;
		}
	}
	else	// LEFT
	{
		--ent_ptr->prop1;

		ent_x += ent_ptr->prop1;
#asm
	lda _ent_x
	and #$07
	tax

	lda __eny_y_offs, x

	clc
	adc _ent_y_unmasked
	sta _ent_y_unmasked
	bcc .skip_hb_2
	inc _ent_y_unmasked + 1
.skip_hb_2:
#endasm
		spd_SATB_set_sprite_LT( ENT_SPR_ENEMY_FLYING_LEFT, ent_x, ent_y_unmasked );

		if( !ent_ptr->prop1 )
		{
			ent_ptr->prop0 ^= 0x80;
		}
	}
}

//
// prop2: 7-bit: 1 - ON, 0 - OFF
//
void	update_cached_switch()
{
	ENT_ADD_TO_SATB

	if( ent_ptr->prop2 & 0x80 )
	{
		spd_SATB_set_sprite_LT( ENT_SPR_SWITCH_ON, ent_x, ent_y_unmasked );
	}
	else
	{
		spd_SATB_set_sprite_LT( ENT_SPR_SWITCH_OFF, ent_x, ent_y_unmasked );
	}
}

//
// prop2: 7-bit: 1 - ON, 0 - OFF
//
void	update_cached_button()
{
	ENT_ADD_TO_SATB

	if( ent_ptr->prop2 & 0x80 )
	{
		spd_SATB_set_sprite_LT( ENT_SPR_BUTTON_ON, ent_x, ent_y_unmasked );
	}
	else
	{
		spd_SATB_set_sprite_LT( ENT_SPR_BUTTON_OFF, ent_x, ent_y_unmasked );
	}
}

//
// prop0: 7-bit: 1 - RIGHT/DOWN, 0-LEFT/UP; 6-0: distance
// prop1: 7-0: current distance
// prop2: 7-bit: 1 - ON, 0 - OFF; 6-bit: 1 - UP/DOWN, 0 - LEFT/RIGHT; 5-bit: looped?; 4-bit: sprite low priority
//
void	update_cached_platform()
{
	ENT_ADD_TO_SATB

	if( ent_ptr->prop2 & 0x40 )	// UP/DOWN
	{
		if( ent_ptr->prop2 & 0x80 )	// ON
		{
			if( ent_ptr->prop0 & 0x80 )	// DOWN
			{
				if( ++ent_ptr->prop1 >= ( ent_ptr->prop0 & 0x7f ) )
				{
					if( !( ent_ptr->prop2 & 0x20 ) )	// looped?
					{
						ent_ptr->prop2 &= 0x7f;		// NO!

						if( ent_ptr->targ_id != 0xff )
						{
							__map_ents[ ent_ptr->targ_id ].prop2 ^= 0x80;	// switch on/off target entity
						}
					}

					ent_ptr->prop0 ^= 0x80;
				}
			}
			else	// UP
			{
				if( !--ent_ptr->prop1 )
				{
					if( !( ent_ptr->prop2 & 0x20 ) )	// looped?
					{
						ent_ptr->prop2 &= 0x7f;		// NO!

						if( ent_ptr->targ_id != 0xff )
						{
							__map_ents[ ent_ptr->targ_id ].prop2 ^= 0x80;	// switch on/off target entity
						}
					}

					ent_ptr->prop0 ^= 0x80;
				}
			}
		}
		spd_SATB_set_sprite_LT( ENT_SPR_PLATFORM, ent_x, ent_y_unmasked + ent_ptr->prop1 );
	}
	else
	{
		if( ent_ptr->prop2 & 0x80 )	// ON
		{
			if( ent_ptr->prop0 & 0x80 )	// RIGHT
			{
				if( ++ent_ptr->prop1 >= ( ent_ptr->prop0 & 0x7f ) )
				{
					if( !( ent_ptr->prop2 & 0x20 ) )	// looped?
					{
						ent_ptr->prop2 &= 0x7f;		// NO!

						if( ent_ptr->targ_id != 0xff )
						{
							__map_ents[ ent_ptr->targ_id ].prop2 ^= 0x80;	// switch on/off target entity
						}
					}

					ent_ptr->prop0 ^= 0x80;
				}
			}
			else	// LEFT
			{
				if( !--ent_ptr->prop1 )
				{
					if( !( ent_ptr->prop2 & 0x20 ) )	// looped?
					{
						ent_ptr->prop2 &= 0x7f;		// NO!

						if( ent_ptr->targ_id != 0xff )
						{
							__map_ents[ ent_ptr->targ_id ].prop2 ^= 0x80;	// switch on/off target entity
						}
					}

					ent_ptr->prop0 ^= 0x80;
				}
			}
		}
		spd_SATB_set_sprite_LT( ENT_SPR_PLATFORM, ent_x + ent_ptr->prop1, ent_y_unmasked );
	}

	if( ent_ptr->prop2 & 0x10 )
	{
		spd_set_pri_LT( SPD_SPR_PRI_LOW );
	}

	FORCE_COLLISION_DETECTION
}

//
// prop0: 7-0: falling pause in frames
//*prop1: 7-bit: 1 - activation, 6-bit: 1 - start falling; 5-bit: 1 - inactive
//
#asm
__prand_offs:
	.db $0, $1, $2, $3, $2, $1, $0, $1
#endasm

void	update_cached_logs()
{
	if( !( ent_ptr->prop1 & 0x20 ) )	// active?
	{
		if( ent_ptr->prop1 & 0x80 )	// activated?
		{
			if( ent_ptr->prop1 & 0x40 )
			{
				ent_ptr->prop2 += 5;

				if( ent_ptr->prop2 > ScrPixelsHeight )
				{
					ent_ptr->prop1 |= 0x20;	// deactivate
				}
			}
			else
			if( !--ent_ptr->prop0 )
			{
				ent_ptr->prop1 |= 0x40;	// time is over, start falling
			}

			tmp_val8 = ent_ptr->prop0;
#asm	; apply shake

	lda _tmp_val8
	and #$07
	tax

	lda __prand_offs, x

	clc
	adc _ent_y_unmasked
	sta _ent_y_unmasked
	bcc .skip_hb
	inc _ent_y_unmasked + 1
.skip_hb:
#endasm
		}

		ENT_ADD_TO_SATB
		spd_SATB_set_sprite_LT( ENT_SPR_LOGS, ent_x, ent_y_unmasked + ent_ptr->prop2 );

		FORCE_COLLISION_DETECTION
	}
}

void	update_cached_door()
{
	update_obstacle_logic( ENT_DOOR_LOOP_PAUSE_IN_FRAMES );

	ENT_ADD_TO_SATB
	spd_SATB_set_sprite_LT( ENT_SPR_DOOR, ent_x, ent_y_unmasked + ent_ptr->prop1 );
	spd_set_pri_LT( SPD_SPR_PRI_LOW );

	FORCE_COLLISION_DETECTION
}

void	update_cached_heavy_load()
{
	update_obstacle_logic( ENT_HEAVY_LOAD_LOOP_PAUSE_IN_FRAMES );

	ENT_ADD_TO_SATB
	spd_SATB_set_sprite_LT( ENT_SPR_HEAVY_LOAD, ent_x, ent_y_unmasked + ent_ptr->prop1 );
	spd_set_pri_LT( SPD_SPR_PRI_LOW );

	FORCE_COLLISION_DETECTION
}

//
// prop0: 7-bit: 1 - DOWN, 0 - UP; 6-0: distance
// prop1: 7-0: current distance
// prop2: 7-bit: 1 - ON, 0 - OFF; 6-bit: loop, 5-bit: damage ON/OFF, 4-bit: shake when hits the ground
//*prop3: 7-0: pause in frames
//
void	update_obstacle_logic( u8 _pause_in_frames )
{
	if( ent_ptr->prop2 & 0x80 )	// ON/OFF
	{
		if( ent_ptr->prop0 & 0x80 )	// DOWN
		{
			if( ent_ptr->prop3 )	// pause?
			{
				--ent_ptr->prop3;
				return;
			}

			if( ( ent_ptr->prop1 += 4 ) >= ( ent_ptr->prop0 & 0x7f ) )
			{
				ent_ptr->prop0 ^= 0x80;

				if( !( ent_ptr->prop2 & 0x40 ) )	// looped?
				{
					ent_ptr->prop2 &= 0x7f;		// NO!

					if( ent_ptr->targ_id != 0xff )
					{
						__map_ents[ ent_ptr->targ_id ].prop2 ^= 0x80;	// switch on/off target entity
					}
				}
				else
				{
					ent_ptr->prop3 = _pause_in_frames;
				}
			}
		}
		else	// UP
		{
			if( ent_ptr->prop3 )	// pause?
			{
				--ent_ptr->prop3;

				return;
			}

			if( !--ent_ptr->prop1 )
			{
				ent_ptr->prop0 ^= 0x80;

				if( !( ent_ptr->prop2 & 0x40 ) )	// looped?
				{
					ent_ptr->prop2 &= 0x7f;		// NO!

					if( ent_ptr->targ_id != 0xff )
					{
						__map_ents[ ent_ptr->targ_id ].prop2 ^= 0x80;	// switch on/off target entity
					}
				}
				else
				{
					ent_ptr->prop3 = _pause_in_frames;

					if( ent_ptr->prop2 & 0x10 )	// shake?
					{
						scene_shake();
					}
				}
			}
		}
	}
}

//
// prop0: 0-bit: 1 - portal IN, 0 - portal OUT
//
void	update_cached_portal()
{
	ENT_ADD_TO_SATB

	if( ent_ptr->prop0 & 0x01 )
	{
		spd_SATB_set_sprite_LT( ENT_SPR_PORTAL, ent_x, ent_y_unmasked );
	}
}

//************************************************************
// 4. Player/entity collision detection.
//************************************************************

u8	__fastcall	check_cached_entity_collision( u8 _ind<acc> );	// entity ID is an entity index

#asm
	.proc _check_cached_entity_collision.1

	lsr a
	lsr a
	lsr a
	tax

	jmp [_check_collision_func_arr, x]

	.endp

_check_collision_func_arr:

	.dw _check_null
	.dw _check_coll_star
	.dw _check_enemy_walking
	.dw _check_enemy_flying
	.dw _check_switch
	.dw _check_button
	.dw _check_platform
	.dw _check_logs
	.dw _check_door
	.dw _check_heavy_load
	.dw _check_portal

_check_null:
	call _check_collision_null
	rts

_check_coll_star:
	call _check_collision_coll_star
	rts

_check_enemy_walking:
	call _check_collision_enemy_walking
	rts

_check_enemy_flying:
	call _check_collision_enemy_flying
	rts

_check_switch:
	call _check_collision_switch
	rts

_check_button:
	call _check_collision_button
	rts

_check_platform:
	call _check_collision_platform
	rts

_check_logs:
	call _check_collision_logs
	rts

_check_door:
	call _check_collision_door
	rts

_check_heavy_load:
	call _check_collision_heavy_load
	rts

_check_portal:
	call _check_collision_portal
	rts

#endasm

u8	check_collision_null()
{
	return 0;
}

u8	check_collision_coll_star()
{
	if( IS_PLAYER_INTERSECT_BOX( ent_x, ent_y_unmasked, base_ent_coll_star_width, base_ent_coll_star_height ) )
	{
		ENT_COLL_DEACTIVATE

		++__player_data.stars;	// increment collected stars counter
		ENT_CACHE_RESET
		
//		return 1;	// add to collision cache <- IGNORE COLLISION CACHE FOR COLLECTABLE ENTITIES!
	}

	return 0;	// continue collision detection
}

u8	check_collision_enemy_walking()
{
	if( IS_PLAYER_INTERSECT_BOX( ent_x + ent_ptr->prop1, ent_y_unmasked, ENT_ENEMY_WALKING_WIDTH, ent_ptr->height ) )
	{
		player_enemy_hit();

		return 1;	// add to collision cache
	}

	return 0;	// continue collision detection
}

u8	check_collision_enemy_flying()
{
	if( IS_PLAYER_INTERSECT_BOX( ent_x + ent_ptr->prop1, ent_y_unmasked, ENT_ENEMY_FLYING_WIDTH, ent_ptr->height ) )
	{
		player_enemy_hit();

		return 1;	// add to collision cache
	}

	return 0;	// continue collision detection
}

//
// prop2: 7-bit - ON/OFF; 6-bit - bit lock for inner use
//
u8	check_collision_switch()
{
	u8	pos;

	if( IS_PLAYER_INTERSECT_BOX( ent_x, ent_y_unmasked, ent_ptr->width, ent_ptr->height ) )
	{
		if( __jpad_val & JOY_ACTION_BTN )
		{
			if( !( ent_ptr->prop2 & 0x40 ) )
			{
				ent_ptr->prop2 ^= 0x80;
				ent_ptr->prop2 |= 0x40;

				if( ent_ptr->targ_id != 0xff )
				{
					__map_ents[ ent_ptr->targ_id ].prop2 ^= 0x80;	// switch on/off target entity
				}
			}
		}
		else
		{
			ent_ptr->prop2 &= ~0x40;
		}

		return 1;	// add to collision cache
	}

	return 0;	// continue collision detection
}

//
// prop2: 7-bit - ON/OFF
//
u8	check_collision_button()
{
	u8	pos;

	if( IS_PLAYER_INTERSECT_BOX( ent_x, ent_y_unmasked, ent_ptr->width, ent_ptr->height ) )
	{
		ent_ptr->prop2 |= 0x80;

		if( ent_ptr->targ_id != 0xff )
		{
			__map_ents[ ent_ptr->targ_id ].prop2 ^= 0x80;	// switch on target entity
		}

		return 1;	// add to collision cache
	}
	else
	{
		ent_ptr->prop2 &= ~0x80;

		if( ent_ptr->targ_id != 0xff )
		{
			__map_ents[ ent_ptr->targ_id ].prop2 &= ~0x80;	// switch off target entity
		}
	}

	return 0;	// continue collision detection
}

u8	check_collision_platform()
{
	s16	player_bottom;
	s16	ent_y_pos;

	if( PLAYER_IS_FALLING )
	{
		if( ent_ptr->prop2 & 0x40 )	// UP/DOWN
		{
			ent_y_pos = ent_y_unmasked + ent_ptr->prop1;

			if( IS_PLAYER_INTERSECT_BOX( ent_x, ent_y_pos, ENT_PLATFORM_WIDTH, ENT_PLATFORM_HEIGHT ) )
			{
				player_bottom = player_y + PLAYER_HEIGHT;

				if( player_bottom > ent_y_pos && player_bottom <= ( ent_y_pos + ENT_PLATFORM_HEIGHT ) )
				{
					// update player Y-position
					player_y = ent_y_pos - PLAYER_HEIGHT;
					spd_set_y_LT( player_y );
					__player_y = player_y + mpd_scroll_y + 1;

					if( ent_ptr->prop2 & 0x80 )	// ON
					{
						mpd_scroll_step_y = ( mpd_scroll_step_y + 1 ) & 0x07;

						// move player with platform
						if( ent_ptr->prop0 & 0x80 )	// DOWN
						{
							++__player_y;
							mpd_move_down();

						}
						else	// UP
						{
							--__player_y;
							mpd_move_up();
						}
					}

					PLAYER_STATE_ON_SURFACE
				}
			}

//			return 1;	// add to collision cache <- IGNORE COLLISION CACHE FOR PLATFORM ENTITIES!
		}
		else
		{
			if( IS_PLAYER_INTERSECT_BOX( ent_x + ent_ptr->prop1, ent_y_unmasked, ENT_PLATFORM_WIDTH, ENT_PLATFORM_HEIGHT ) )
			{
				player_bottom = player_y + PLAYER_HEIGHT;

				if( player_bottom > ent_y_unmasked && player_bottom <= ( ent_y_unmasked + ENT_PLATFORM_HEIGHT ) )
				{
					// update player Y-position
					player_y = ent_y_unmasked - PLAYER_HEIGHT;
					spd_set_y_LT( player_y );
					__player_y = player_y + mpd_scroll_y + 1;

					if( ent_ptr->prop2 & 0x80 )	// ON
					{
						mpd_scroll_step_x = ( mpd_scroll_step_x + 1 ) & 0x07;

						// move player with platform
						if( ent_ptr->prop0 & 0x80 )	// RIGHT
						{
							++__player_x;
							mpd_move_right();
						}
						else	// LEFT
						{
							--__player_x;
							mpd_move_left();
						}
					}

					PLAYER_STATE_ON_SURFACE
				}
			}

//			return 1;	// add to collision cache <- IGNORE COLLISION CACHE FOR PLATFORM ENTITIES!
		}
	}

	return 0;	// continue collision detection
}

//
// prop0: 7-0: falling pause in frames
//*prop1: 7-bit: 1 - activation, 6-bit: 1 - start falling; 5-bit: 1 - inactive
//
u8	check_collision_logs()
{
	s16	player_bottom;

	if( !( ent_ptr->prop1 & 0x60 ) )	// active and isn't falling
	{
		if( PLAYER_IS_FALLING )
		{
			if( IS_PLAYER_INTERSECT_BOX( ent_x, ent_y_unmasked, ent_ptr->width, ent_ptr->height ) )
			{
				player_bottom = player_y + PLAYER_HEIGHT;

				if( player_bottom > ent_y_unmasked && player_bottom < ( ent_y_unmasked + ENT_LOGS_PLATFORM_HEIGHT ) )
				{
					// activate platform
					ent_ptr->prop1 |= 0x80;

					// update player Y-position
					player_y = ent_y_unmasked - PLAYER_HEIGHT;
					spd_set_y_LT( player_y );
					__player_y = player_y + mpd_scroll_y + 1;

					PLAYER_STATE_ON_SURFACE
				}

//				return 1;	// add to collision cache <- IGNORE COLLISION CACHE FOR PLATFORM ENTITIES!
			}
		}
	}

	return 0;	// continue collision detection
}

//
// prop0: 7-bit: 1 - DOWN, 0 - UP; 6-0: distance
// prop1: 7-0: current distance
// prop2: 7-bit: 1 - ON, 0 - OFF; 6-bit: loop, 5-bit: damage ON/OFF, 4-bit: shake when hits the ground
//*prop3: 7-0: pause in frames
//
u8	check_collision_door()
{
	if( IS_PLAYER_INTERSECT_BOX( ent_x, ent_y_unmasked + ent_ptr->prop1, ent_ptr->width, ENT_DOOR_HEIGHT ) )
	{
		if( player_x + PLAYER_CP_MID_WIDTH < ent_x + ( ent_ptr->width >> 1 ) )
		{
			player_x = ent_x - PLAYER_WIDTH;
		}
		else
		{
			player_x = ent_x + ent_ptr->width;
		}

		spd_set_x_LT( player_x );
		__player_x = player_x + mpd_scroll_x;

		if( ent_ptr->prop2 & 0x20 )	// hit?
		{
			player_enemy_hit();
		}

//		return 1;	// add to collision cache <- IGNORE COLLISION CACHE
	}

	return 0;	// continue collision detection
}

//
// prop0: 7-bit: 1 - DOWN, 0 - UP; 6-0: distance
// prop1: 7-0: current distance
// prop2: 7-bit: 1 - ON, 0 - OFF; 6-bit: loop, 5-bit: damage ON/OFF, 4-bit: shake when hits the ground
//*prop3: 7-0: pause in frames
//
u8	check_collision_heavy_load()
{
	if( IS_PLAYER_INTERSECT_BOX( ent_x, ent_y_unmasked + ent_ptr->prop1, ent_ptr->width, ENT_HEAVY_LOAD_HEIGHT ) )
	{
		if( player_x + PLAYER_CP_MID_WIDTH < ent_x )
		{
			player_x = ent_x - PLAYER_WIDTH;

			spd_set_x_LT( player_x );
			__player_x = player_x + mpd_scroll_x;
		}
		else
		if( player_x + PLAYER_CP_MID_WIDTH > ent_x + ent_ptr->width )
		{
			player_x = ent_x + ent_ptr->width;

			spd_set_x_LT( player_x );
			__player_x = player_x + mpd_scroll_x;
		}

		if( ent_ptr->prop2 & 0x20 )	// hit?
		{
			player_enemy_hit();
		}

//		return 1;	// add to collision cache <- IGNORE COLLISION CACHE
	}

	return 0;	// continue collision detection
}

//
// prop0: 0-bit: 1 - portal IN, 0 - portal OUT
//
u8	check_collision_portal()
{
	s16		portal_out_x;
	s16		portal_out_y;
	s16		scr_pos_x;
	s16		scr_pos_y;

	mpd_MAP_ENT*	ent_portal_out_ptr;


	if( IS_PLAYER_INTERSECT_BOX( ent_x, ent_y_unmasked, ent_ptr->width, ent_ptr->height ) )
	{
		if( ( __jpad_val & JOY_ACTION_BTN ) && ( ent_ptr->prop0 & 0x01 ) )	// portal IN ?
		{
			if( ent_ptr->targ_id != 0xff )
			{
				ent_portal_out_ptr = &__map_ents[ ent_ptr->targ_id ];

				// get target portal position
				portal_out_x	= ( ent_portal_out_ptr->x & ENT_FLAG_ACTIVE_INV );
				portal_out_y	= ( ent_portal_out_ptr->y & ENT_ID_MASK_INV );

				// calc new screen position
				scr_pos_x	= portal_out_x - ( ScrPixelsWidth >> 1 ) + ( ent_portal_out_ptr->width >> 1 );
				scr_pos_y	= portal_out_y - ( ScrPixelsHeight >> 1 ) + ( ent_portal_out_ptr->height >> 1 );

				scr_pos_x	= scr_pos_x < 0 ? 0:scr_pos_x;
				scr_pos_y	= scr_pos_y < 0 ? 0:scr_pos_y;

				scr_pos_x	= ( scr_pos_x > mpd_map_active_width ) ? mpd_map_active_width:scr_pos_x;
				scr_pos_y	= ( scr_pos_y > mpd_map_active_height ) ? mpd_map_active_height:scr_pos_y;

				disp_off();
				vsync();

				// redraw scene at target portal position
				mpd_draw_screen_by_pos( scr_pos_x, scr_pos_y );

				// update player position
				spd_set_x_LT( portal_out_x - scr_pos_x );
				spd_set_y_LT( portal_out_y - scr_pos_y );

				__player_x = portal_out_x;
				__player_y = portal_out_y;

				PLAYER_STATE_ON_SURFACE

				disp_on();
				vsync();

				// rebuild entity cache at new scene position
				ENT_CACHE_RESET

				return 0;	// continue collision detection
			}
		}

		return 1;	// add to collision cache
	}

	return 0;	// continue collision detection
}