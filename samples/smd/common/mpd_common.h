//######################################################################################################
//
// This file is a part of the MAPeD-SMD Copyright 2017-2021 0x8BitDev ( MIT license. See LICENSE.txt )
// Desc: It contains the main data structures for use in SGDK projects
//
//######################################################################################################

#ifndef _MPD_COMMON_H_
#define _MPD_COMMON_H_

//////////////
// macroses //
//////////////

#define CHECK_MAP_FLAG( flag ) ( ( MAP_DATA_MAGIC & flag ) == flag )

//########################
//
// main data structures
//
//########################

////////////
// arrays //
////////////

typedef struct
{
	u16	count;
	u16**	data;
} mpd_ARR_PU16;

typedef struct
{
	u16	count;
	u8*	data;
} mpd_ARR_U8;

typedef struct
{
	u16	count;
	u16*	data;
} mpd_ARR_U16;

typedef struct
{
	u16	count;
	u32*	data;
} mpd_ARR_U32;

typedef struct
{
	u32			count;
	struct mpd_SCREEN**	data;
} mpd_ARR_SCREEN;

//////////////
// entities //
//////////////

typedef struct
{
	u16	id;
	u16	width;
	u16	height;
	u16	pivot_x;
	u16	pivot_y;
	u16	props_cnt;
	u16	props[];
} mpd_BASE_ENTITY;

typedef struct mpd_ENTITY_INSTANCE
{
	u16				id;
	mpd_BASE_ENTITY*		base_ent;
	struct mpd_ENTITY_INSTANCE*	target_ent;
	u16				x_pos;
	u16				y_pos;
	u16				props_cnt;
	u16				props[];
} mpd_ENTITY_INSTANCE;

#endif // _MPD_COMMON_H_