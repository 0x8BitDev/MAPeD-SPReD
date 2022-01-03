//######################################################################################################
//
// This file is a part of the MAPeD-PCE Copyright 2017-2022 0x8BitDev ( MIT license. See LICENSE.txt )
// Desc: It contains some useful defines and structures
//
//######################################################################################################

/* some useful defines */

#define u8	unsigned char
#define u16	unsigned int
#define s8	char
#define s16	int

#define TRUE	1
#define FALSE	0
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
