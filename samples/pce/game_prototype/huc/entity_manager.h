//##################################################################
//
// Copyright 2021-2022 0x8BitDev ( MIT license. See LICENSE.txt )
//
// This file is a part of the game prototype demo.
//
//##################################################################

#define	SCREEN_ENT_MAX			4
#define	SCREEN_ENT_COLLECTABLE_MAX	4
#define MAP_SCREENS_MAX			40	// max number of screens in a game map

#define MAP_ENTS_MAX			MAP_SCREENS_MAX * SCREEN_ENT_MAX
#define MAP_ENTS_COLLECTABLE_MAX	MAP_SCREENS_MAX * SCREEN_ENT_COLLECTABLE_MAX

#define COLLISION_CACHE_MAX			4	// number of items in collision cache
#define NUM_FRAMES_TO_UPDATE_COLLISION_CACHE	6	// number of frames to update collision cache

#define	ENT_FLAG_ACTIVE		0x8000
#define	ENT_ID_MASK		0xf000

#define	ENT_FLAG_ACTIVE_INV	0x7fff
#define	ENT_ID_MASK_INV		0x0fff

typedef struct
{
	u8	id;
	u8	targ_id;	// target entity index in __map_ents

	s16	x;		// high bit (0x8000) - active/inactive
	s16	y;		// high 4-bits (0xf000) - entity uid

	u8	width;
	u8	height;

	u8	prop0;		// property 1
	u8	prop1;		// property 2
	u8	prop2;		// ...
	u8	prop3;		// ...

} mpd_MAP_ENT;

typedef struct
{
	s16	x;	// high bit (0x8000) - active/inactive
	s16	y;	// high 4-bits (0xf000) - entity uid

} mpd_MAP_COLLECTABLE_ENT;

// map entities array
mpd_MAP_ENT		__map_ents[ MAP_ENTS_MAX ];
mpd_MAP_COLLECTABLE_ENT	__map_coll_ents[ MAP_ENTS_COLLECTABLE_MAX ];

// collision cache
u8	__map_collision_cache[ COLLISION_CACHE_MAX ];

// screen entities cache - all entities visible on the screen
u8	__screen_ent_cache[ SCREEN_ENT_MAX * 4 ];			// 4 - four screens: current, right, bottom, right bottom
s8	__screen_ent_cache_pos		= -1;

u8	__screen_ent_coll_cache[ SCREEN_ENT_COLLECTABLE_MAX * 4 ];	// 4 - four screens: current, right, bottom, right bottom
s8	__screen_ent_coll_cache_pos	= -1;

#define	ENT_CACHE_RESET		__screen_ent_coll_cache_pos = __screen_ent_cache_pos = -1;	// rebuild the cache at the next frame
#define	ENT_DEACTIVATE		ent_ptr->x &= ENT_FLAG_ACTIVE_INV;
#define	ENT_COLL_DEACTIVATE	ent_coll_ptr->x &= ENT_FLAG_ACTIVE_INV;

#define	ENT_UPDATE_CACHE_STEP	4	// update entity cache after each UPDATE_CACHE_STEP pixels
#define	ENT_NEXT_SATB_POS		spd_SATB_set_pos( SATB_pos++ );

s16	__scr_scroll_x = -1;
s16	__scr_scroll_y = -1;

u8	__ent_coll_pos;
u8	__ent_pos;
u8	__collision_cache_pos;
u8	__collision_cache_frames;

#include "entity.h"

u8	add_collectable_entity()
{
	if( __ent_coll_pos < SCREEN_ENT_COLLECTABLE_MAX )
	{
		// fill entity data
		ent_coll_ptr = &__map_coll_ents[ map_coll_ent + __ent_coll_pos ];

		++__ent_coll_pos;

		ent_coll_ptr->x = scr_data.inst_ent.inst.x_pos | ENT_FLAG_ACTIVE;	// <-- active by default
		ent_coll_ptr->y = scr_data.inst_ent.inst.y_pos | ENT_ID( scr_data.inst_ent.base.id );

		return 1;
	}

	return 0;
}

u8	add_entity()
{
	if( __ent_pos < SCREEN_ENT_MAX )
	{
		// fill entity data
		ent_ptr = &__map_ents[ map_ent + __ent_pos ];

		++__ent_pos;

		ent_ptr->x = scr_data.inst_ent.inst.x_pos | ENT_FLAG_ACTIVE;	// <-- active by default
		ent_ptr->y = scr_data.inst_ent.inst.y_pos | ENT_ID( scr_data.inst_ent.base.id );

		ent_ptr->width	= scr_data.inst_ent.base.width;
		ent_ptr->height	= scr_data.inst_ent.base.height;

		ent_ptr->id	= scr_data.inst_ent.inst.id;

		if( scr_data.inst_ent.inst.targ_ent_addr )
		{
			ent_ptr->targ_id = scr_data.targ_ent.inst.id;
		}
		else
		{
			ent_ptr->targ_id = 0xff;
		}

		// copy properties
		if( scr_data.inst_ent.inst.props_cnt > 0 )
		{
			ent_ptr->prop0 = scr_data.inst_ent.inst_props[ 0 ];

			if( scr_data.inst_ent.inst.props_cnt > 1 )
			{
				ent_ptr->prop1 = scr_data.inst_ent.inst_props[ 1 ];

				if( scr_data.inst_ent.inst.props_cnt > 2 )
				{
					ent_ptr->prop2 = scr_data.inst_ent.inst_props[ 2 ];

					if( scr_data.inst_ent.inst.props_cnt > 3 )
					{
						ent_ptr->prop3 = scr_data.inst_ent.inst_props[ 3 ];
					}
				}
			}
		}

		return 1;
	}

	return 0;
}

void	init_map_entities( u8 _map_ind )
{
	u8	scr_n;
	u8	ent2_n;

	mpd_MAP_ENT*	ent2_ptr;

	// check the entity array overflow
	if( ( mpd_map_scr_width * mpd_map_scr_height ) > MAP_SCREENS_MAX )
	{
		show_msg( "SCREEN ARRAY OVERFLOW!" );
	}

	// clear entity arrays
	{
		ent_ptr = __map_ents;

		for( ent_n = 0; ent_n < MAP_ENTS_MAX; ent_n++ )
		{
			ent_ptr->id		= 0;
			ent_ptr->targ_id	= 0;

			ent_ptr->x = 0;
			ent_ptr->y = 0;

			ent_ptr->width	= 0;
			ent_ptr->height	= 0;

			ent_ptr->prop0 = 0;
			ent_ptr->prop1 = 0;
			ent_ptr->prop2 = 0;
			ent_ptr->prop3 = 0;

			++ent_ptr;
		}

		ent_coll_ptr = __map_coll_ents;

		for( ent_n = 0; ent_n < MAP_ENTS_COLLECTABLE_MAX; ent_n++ )
		{
			ent_coll_ptr->x = 0;
			ent_coll_ptr->y = 0;

			++ent_coll_ptr;
		}
	}

	map_ent		= 0;
	map_coll_ent	= 0;

	// fill up the entity array
	for( scr_n = 0; scr_n < ( mpd_map_scr_height * mpd_map_scr_width ); scr_n++ )
	{
		mpd_get_screen_data( &scr_data, scr_n );

		__ent_pos	= 0;
		__ent_coll_pos	= 0;

		for( ent_n = 0; ent_n < scr_data.scr.ents_cnt; ent_n++ )
		{
			mpd_get_entity( &scr_data, ent_n );

			init_map_entity( scr_data.inst_ent.base.id );
		}	

		map_ent		+= SCREEN_ENT_MAX;
		map_coll_ent	+= SCREEN_ENT_COLLECTABLE_MAX;
	}

	// assign target entity indexes
	ent_ptr = __map_ents;

	for( ent_n = 0; ent_n < MAP_ENTS_MAX; ent_n++ )
	{
		if( ent_ptr->targ_id != 0xff )
		{
			ent2_ptr = __map_ents;

			for( ent2_n = 0; ent2_n < MAP_ENTS_MAX; ent2_n++ )
			{
				if( ent2_ptr->id == ent_ptr->targ_id )
				{
					ent_ptr->targ_id = ent2_n;
					break;
				}

				++ent2_ptr;
			}
		}

		++ent_ptr;
	}

	__spr_collision 	= 0;
	__collision_cache_pos	= 0;
	__collision_cache_frames= 0;

	ENT_CACHE_RESET

	__scr_scroll_x = -1;
	__scr_scroll_y = -1;
}

// OUT: new SATB pos
void	update_scene_entities()
{
	u8	scr_pos;

	u16	right_scr_offset;
	u16	bottom_scr_offset;

	// update cache after each ENT_UPDATE_CACHE_STEP
	if( ( __screen_ent_cache_pos < 0 ) || ( ( abs( mpd_scroll_x - __scr_scroll_x ) > ENT_UPDATE_CACHE_STEP ) || ( abs( mpd_scroll_y - __scr_scroll_y ) > ENT_UPDATE_CACHE_STEP ) ) )
	{
		__scr_scroll_x	= mpd_scroll_x;
		__scr_scroll_y	= mpd_scroll_y;

		// reset static entities cache
		__screen_ent_cache_pos		= 0;
		__screen_ent_coll_cache_pos	= 0;

		scr_pos = mpd_get_curr_screen_ind();

		// update the main screen where the player is
		__update_ents( scr_pos );

		// check the right screen visibility
		right_scr_offset = mpd_scroll_x % ScrPixelsWidth;

		if( right_scr_offset )
		{
			// update the right screen
			__update_ents( scr_pos + 1 );
		}

		// check the bottom screen visibility
		bottom_scr_offset = mpd_scroll_y % ScrPixelsHeight;

		if( bottom_scr_offset )
		{
			// update the bottom screen
			scr_pos	 += mpd_map_scr_width;
			__update_ents( scr_pos );
		}

		// check the right bottom screen visibility
		if( right_scr_offset && bottom_scr_offset )
		{
			// update the right bottom screen
			__update_ents( scr_pos + 1 );
		}
	}
	else
	{
		__update_cached_ents();
	}
}

void	__update_ents( u8 _scr_pos )
{
	// fill SATB by entities of the current screen

	// COLLECTABLE ENTITIES
	scr_pos = _scr_pos * SCREEN_ENT_COLLECTABLE_MAX;

	ent_coll_ptr = &__map_coll_ents[ scr_pos ];

	for( ent_n = 0; ent_n < SCREEN_ENT_COLLECTABLE_MAX; ent_n++ )
	{
		// active entity?
		if( ent_coll_ptr->x & ENT_FLAG_ACTIVE )
		{
			ent_y = ent_coll_ptr->y;

			ent_x		= ( ent_coll_ptr->x & ENT_FLAG_ACTIVE_INV ) - mpd_scroll_x;
			ent_y_unmasked	= ( ent_y & ENT_ID_MASK_INV ) - mpd_scroll_y;

			// check screen/entity intersection
			if( ( ent_x < ScrPixelsWidth ) && ( ent_y_unmasked < ScrPixelsHeight ) )
			{
				if( update_global_entity( ent_y & ENT_ID_MASK ) )
				{
					__screen_ent_coll_cache[ __screen_ent_coll_cache_pos++ ] = scr_pos + ent_n;
				}
			}
		}

		++ent_coll_ptr;
	}

	// OTHER ENTITIES
	scr_pos = _scr_pos * SCREEN_ENT_MAX;

	ent_ptr = &__map_ents[ scr_pos ];

	for( ent_n = 0; ent_n < SCREEN_ENT_MAX; ent_n++ )
	{
		// active entity?
		if( ent_ptr->x & ENT_FLAG_ACTIVE )
		{
			ent_y = ent_ptr->y;

			ent_x		= ( ent_ptr->x & ENT_FLAG_ACTIVE_INV ) - mpd_scroll_x;
			ent_y_unmasked	= ( ent_y & ENT_ID_MASK_INV ) - mpd_scroll_y;

			// check screen/entity intersection
			if( ( ent_x < ScrPixelsWidth ) && ( ent_y_unmasked < ScrPixelsHeight ) )
			{
				if( update_global_entity( ent_y & ENT_ID_MASK ) )
				{
					__screen_ent_cache[ __screen_ent_cache_pos++ ] = scr_pos + ent_n;
				}
			}
		}

		++ent_ptr;
	}
}

void	__update_cached_ents()
{
	// use cached values

	// COLLECTABLE ENTITIES
	for( ent_n = 0; ent_n < __screen_ent_coll_cache_pos; ent_n++ )
	{
		ent_coll_ptr = &__map_coll_ents[ __screen_ent_coll_cache[ ent_n ] ];

		ent_y = ent_coll_ptr->y;

		ent_x		= ( ent_coll_ptr->x & ENT_FLAG_ACTIVE_INV ) - mpd_scroll_x;
		ent_y_unmasked	= ( ent_y & ENT_ID_MASK_INV ) - mpd_scroll_y;

		update_cached_entity( ent_y & ENT_ID_MASK );
	}

	// OTHER ENTITIES
	for( ent_n = 0; ent_n < __screen_ent_cache_pos; ent_n++ )
	{
		ent_ptr = &__map_ents[ __screen_ent_cache[ ent_n ] ];

		ent_y = ent_ptr->y;

		ent_x		= ( ent_ptr->x & ENT_FLAG_ACTIVE_INV ) - mpd_scroll_x;
		ent_y_unmasked	= ( ent_y & ENT_ID_MASK_INV ) - mpd_scroll_y;

		update_cached_entity( ent_y & ENT_ID_MASK );
	}
}

void	check_player_collisions()
{
	u8	map_ent_ind;
	u8	collis_cnt;

	SATB_pos = spd_SATB_get_pos();

	spd_SATB_set_pos( SATB_POS_PLAYER );

	player_x = spd_get_x_LT();
	player_y = __player_y - mpd_scroll_y;

	// use cached entities

	// COLLISION CACHE
	if( __collision_cache_pos && ( --__collision_cache_frames != 0 ) )
	{
		collis_cnt = 0;

		for( ent_n = 0; ent_n < __collision_cache_pos; ent_n++ )
		{
			ent_ptr = &__map_ents[ __map_collision_cache[ ent_n ] ];

			ent_y = ent_ptr->y;

			ent_x		= ( ent_ptr->x & ENT_FLAG_ACTIVE_INV ) - mpd_scroll_x;
			ent_y_unmasked	= ( ent_y & ENT_ID_MASK_INV ) - mpd_scroll_y;

			if( check_cached_entity_collision( ent_y & ENT_ID_MASK ) )
			{
				++collis_cnt;
			}
		}

		if( collis_cnt )
		{
			spd_SATB_set_pos( SATB_pos );
			return;
		}
	}

	// reset collision cache
	__collision_cache_pos = 0;

	// COLLECTABLE ENTITIES
	for( ent_n = 0; ent_n < __screen_ent_coll_cache_pos; ent_n++ )
	{
		ent_coll_ptr = &__map_coll_ents[ __screen_ent_coll_cache[ ent_n ] ];

		ent_y = ent_coll_ptr->y;

		ent_x		= ( ent_coll_ptr->x & ENT_FLAG_ACTIVE_INV ) - mpd_scroll_x;
		ent_y_unmasked	= ( ent_y & ENT_ID_MASK_INV ) - mpd_scroll_y;

		if( check_cached_entity_collision( ent_y & ENT_ID_MASK ) )
		{
			spd_SATB_set_pos( SATB_pos );
			return;
		}
	}

	// OTHER ENTITIES
	for( ent_n = 0; ent_n < __screen_ent_cache_pos; ent_n++ )
	{
		map_ent_ind = __screen_ent_cache[ ent_n ];
		ent_ptr = &__map_ents[ map_ent_ind ];

		ent_y = ent_ptr->y;

		ent_x		= ( ent_ptr->x & ENT_FLAG_ACTIVE_INV ) - mpd_scroll_x;
		ent_y_unmasked	= ( ent_y & ENT_ID_MASK_INV ) - mpd_scroll_y;

		if( check_cached_entity_collision( ent_y & ENT_ID_MASK ) )
		{
			__map_collision_cache[ __collision_cache_pos++ ] = map_ent_ind;
		}
	}

	if( __collision_cache_pos )
	{
		__collision_cache_frames = NUM_FRAMES_TO_UPDATE_COLLISION_CACHE;
	}

	spd_SATB_set_pos( SATB_pos );
}