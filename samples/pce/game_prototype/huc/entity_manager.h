//##################################################################
//
// Copyright 2021-2022 0x8BitDev ( MIT license. See LICENSE.txt )
//
// This file is a part of the game prototype demo.
//
//##################################################################

#define	SCREEN_ENT_MAX			4	// power of 2 to simplify data accessing by shifting screen number by 2 bits
#define	SCREEN_ENT_COLLECTABLE_MAX	8	// power of 2 to simplify data accessing by shifting screen number by 3 bits
#define MAP_SCREENS_MAX			10
#define MAP_ENTS_MAX			SCREEN_ENT_MAX * MAP_SCREENS_MAX
#define MAP_ENTS_COLLECTABLE_MAX	SCREEN_ENT_COLLECTABLE_MAX * MAP_SCREENS_MAX

#define	ENT_FLAG_ACTIVE		0x8000
#define	ENT_ID_MASK		0xf000

#define	ENT_FLAG_ACTIVE_INV	0x7fff
#define	ENT_ID_MASK_INV		0x0fff

typedef struct
{
	s16	x;	// high bit (0x8000) - active/inactive
	s16	y;	// high 4-bits (0xf000) - entity uid

	u8	width;
	u8	height;

	u8	prop0;	// property 1
	u8	prop1;	// property 2
	u8	prop2;	// ...
	u8	prop3;	// ...

} mpd_MAP_ENT;

typedef struct
{
	s16	x;	// high bit (0x8000) - active/inactive
	s16	y;	// high 4-bits (0xf000) - entity uid

} mpd_MAP_COLLECTABLE_ENT;

// map entities array
mpd_MAP_ENT		__map_ents[ MAP_ENTS_MAX ];
mpd_MAP_COLLECTABLE_ENT	__map_coll_ents[ MAP_ENTS_COLLECTABLE_MAX ];

// screen entities cache - all entities visible on the screen
u8	__screen_ent_cache[ SCREEN_ENT_MAX * 4 ];			// 4 - four screens: current, right, bottom, right bottom
s8	__screen_ent_cache_pos		= -1;

u8	__screen_ent_coll_cache[ SCREEN_ENT_COLLECTABLE_MAX * 4 ];	// 4 - four screens: current, right, bottom, right bottom
s8	__screen_ent_coll_cache_pos	= -1;

#define	ENT_CACHE_RESET		__screen_ent_coll_cache_pos = __screen_ent_cache_pos = -1;	// rebuild the cache at the next frame
#define	ENT_DEACTIVATE		ent_ptr->x &= ENT_FLAG_ACTIVE_INV;
#define	ENT_COLL_DEACTIVATE	ent_coll_ptr->x &= ENT_FLAG_ACTIVE_INV;

#define	ENT_UPDATE_CACHE_STEP	4	// update entity cache after each UPDATE_CACHE_STEP pixels
#define	ENT_ADD_TO_SATB		spd_SATB_set_pos( SATB_pos++ );

s16	__scr_scroll_x = -1;
s16	__scr_scroll_y = -1;

u8	__scr_width;
u8	__scr_height;

u8	__ent_coll_pos;
u8	__ent_pos;

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
	u8	scr_w_n;
	u8	scr_h_n;

	u16	map_size;

	map_size	= mpd_get_map_size( _map_ind );

	__scr_width	= map_size & 0x00ff;
	__scr_height	= ( map_size & 0xff00 ) >> 8;

	// check the entity array overflow
	if( ( __scr_width * __scr_height ) > MAP_SCREENS_MAX )
	{
		show_msg( "SCREEN ARRAY OVERFLOW!" );
	}

	// clear entity arrays
	{
		ent_ptr = __map_ents;

		for( ent_n = 0; ent_n < MAP_ENTS_MAX; ent_n++ )
		{
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
		}
	}

	map_ent		= 0;
	map_coll_ent	= 0;

	// fill up the entity array
	for( scr_h_n = 0; scr_h_n < __scr_height; scr_h_n++ )
	{
		for( scr_w_n = 0; scr_w_n < __scr_width; scr_w_n++ )
		{
			mpd_get_screen_data( &scr_data, scr_h_n * __scr_width + scr_w_n );

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
	}

	__spr_collision = 0;

	__scr_scroll_x = -1;
	__scr_scroll_y = -1;
}

// OUT: new SATB pos
u8	update_scene_entities( u8 _SATB_pos )
{
	u8	scr_pos;

	u16	scr_x;
	u16	scr_y;
	u16	right_scr_offset;
	u16	bottom_scr_offset;

	scr_scroll_x = mpd_scroll_x();
	scr_scroll_y = mpd_scroll_y();

	SATB_pos = _SATB_pos;

	// update cache after each ENT_UPDATE_CACHE_STEP
	if( ( __screen_ent_cache_pos < 0 ) || ( ( abs( scr_scroll_x - __scr_scroll_x ) > ENT_UPDATE_CACHE_STEP ) || ( abs( scr_scroll_y - __scr_scroll_y ) > ENT_UPDATE_CACHE_STEP ) ) )
	{
		__scr_scroll_x	= scr_scroll_x;
		__scr_scroll_y	= scr_scroll_y;

		// reset static entities cache
		__screen_ent_cache_pos		= 0;
		__screen_ent_coll_cache_pos	= 0;

		scr_x	= scr_scroll_x / ScrPixelsWidth;
		scr_y	= scr_scroll_y / ScrPixelsHeight;

		scr_pos = ( scr_y * __scr_width ) + scr_x;

		// update the main screen where the player is
		__update_ents( scr_pos );

		// check the right screen visibility
		right_scr_offset = scr_scroll_x % ScrPixelsWidth;

		if( right_scr_offset )
		{
			// update the right screen
			__update_ents( scr_pos + 1 );
		}

		// check the bottom screen visibility
		bottom_scr_offset = scr_scroll_y % ScrPixelsHeight;

		if( bottom_scr_offset )
		{
			// update the right screen
			scr_pos	 += __scr_width;
			__update_ents( scr_pos );
		}

		// check the right bottom screen visibility
		if( right_scr_offset && bottom_scr_offset )
		{
			// update the right screen
			__update_ents( scr_pos + 1 );
		}
	}
	else
	{
		__update_cached_ents();
	}

	return SATB_pos;
}

void	__update_ents( u8 _scr_pos )
{
	scr_scroll_x = mpd_scroll_x();
	scr_scroll_y = mpd_scroll_y();

	// fill SATB by entities of the current screen

	// COLLECTABLE ENTITIES
	scr_pos	= _scr_pos;
	scr_pos <<= 3;	// x8 - SCREEN_ENT_COLLECTABLE_MAX[8]

	ent_coll_ptr = &__map_coll_ents[ scr_pos ];

	for( ent_n = 0; ent_n < SCREEN_ENT_COLLECTABLE_MAX; ent_n++ )
	{
		// active entity?
		if( ent_coll_ptr->x & ENT_FLAG_ACTIVE )
		{
			ent_y = ent_coll_ptr->y;

			ent_x		= ( ent_coll_ptr->x & ENT_FLAG_ACTIVE_INV ) - scr_scroll_x;
			ent_y_unmasked	= ( ent_y & ENT_ID_MASK_INV ) - scr_scroll_y;

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
	scr_pos	= _scr_pos;
	scr_pos <<= 2;	// x4 - SCREEN_ENT_MAX[4]

	ent_ptr = &__map_ents[ scr_pos ];

	for( ent_n = 0; ent_n < SCREEN_ENT_MAX; ent_n++ )
	{
		// active entity?
		if( ent_ptr->x & ENT_FLAG_ACTIVE )
		{
			ent_y = ent_ptr->y;

			ent_x		= ( ent_ptr->x & ENT_FLAG_ACTIVE_INV ) - scr_scroll_x;
			ent_y_unmasked	= ( ent_y & ENT_ID_MASK_INV ) - scr_scroll_y;

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
	scr_scroll_x = mpd_scroll_x();
	scr_scroll_y = mpd_scroll_y();

	// use cached values

	// COLLECTABLE ENTITIES
	for( ent_n = 0; ent_n < __screen_ent_coll_cache_pos; ent_n++ )
	{
		ent_coll_ptr = &__map_coll_ents[ __screen_ent_coll_cache[ ent_n ] ];

		ent_y = ent_coll_ptr->y;

		ent_x		= ( ent_coll_ptr->x & ENT_FLAG_ACTIVE_INV ) - scr_scroll_x;
		ent_y_unmasked	= ( ent_y & ENT_ID_MASK_INV ) - scr_scroll_y;

		update_cached_entity( ent_y & ENT_ID_MASK );
	}

	// OTHER ENTITIES
	for( ent_n = 0; ent_n < __screen_ent_cache_pos; ent_n++ )
	{
		ent_ptr = &__map_ents[ __screen_ent_cache[ ent_n ] ];

		ent_y = ent_ptr->y;

		ent_x		= ( ent_ptr->x & ENT_FLAG_ACTIVE_INV ) - scr_scroll_x;
		ent_y_unmasked	= ( ent_y & ENT_ID_MASK_INV ) - scr_scroll_y;

		update_cached_entity( ent_y & ENT_ID_MASK );
	}
}

void	check_player_collision()
{
	scr_scroll_x = mpd_scroll_x();
	scr_scroll_y = mpd_scroll_y();

	SATB_pos = spd_SATB_get_pos();

	spd_SATB_set_pos( SATB_POS_PLAYER );

	player_x = spd_get_x_LT();
	player_y = spd_get_y_LT();

	// use cached entities

	// COLLECTABLE ENTITIES
	for( ent_n = 0; ent_n < __screen_ent_coll_cache_pos; ent_n++ )
	{
		ent_coll_ptr = &__map_coll_ents[ __screen_ent_coll_cache[ ent_n ] ];

		ent_y = ent_coll_ptr->y;

		ent_x		= ( ent_coll_ptr->x & ENT_FLAG_ACTIVE_INV ) - scr_scroll_x;
		ent_y_unmasked	= ( ent_y & ENT_ID_MASK_INV ) - scr_scroll_y;

		if( check_cached_entity_collision( ent_y & ENT_ID_MASK ) )
		{
			spd_SATB_set_pos( SATB_pos );
			return;
		}
	}

	// OTHER ENTITIES
	for( ent_n = 0; ent_n < __screen_ent_cache_pos; ent_n++ )
	{
		ent_ptr = &__map_ents[ __screen_ent_cache[ ent_n ] ];

		ent_y = ent_ptr->y;

		ent_x		= ( ent_ptr->x & ENT_FLAG_ACTIVE_INV ) - scr_scroll_x;
		ent_y_unmasked	= ( ent_y & ENT_ID_MASK_INV ) - scr_scroll_y;

		if( check_cached_entity_collision( ent_y & ENT_ID_MASK ) )
		{
			break;
		}
	}

	spd_SATB_set_pos( SATB_pos );
}