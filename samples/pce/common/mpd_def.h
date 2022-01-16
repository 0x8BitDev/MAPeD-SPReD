//######################################################################################################
//
// This file is a part of the MAPeD-PCE Copyright 2017-2022 0x8BitDev ( MIT license. See LICENSE.txt )
// Desc: It contains some useful definitions and structures
//
//######################################################################################################

/* some useful definitions */

typedef unsigned char	u8;
typedef unsigned int	u16;
typedef char		s8;
typedef int		s16;

typedef enum
{
	TRUE = 1,
	FALSE = 0
} bool;

#define NULL	0

/* entities */

typedef struct
{
	u8	id;
	u8	width;
	u8	height;
	u8	pivot_x;
	u8	pivot_y;
	u8	props_cnt;
	u8	props[];
} mpd_BASE_ENTITY;

typedef struct
{
	u8		id;
	mpd_BASE_ENTITY*	base_ent;
	void*			target_ent;	//mpd_ENTITY_INSTANCE
	u16		x_pos;
	u16		y_pos;
	u8		props_cnt;
	u8		props[];
} mpd_ENTITY_INSTANCE;
