//######################################################################################################
//
// This file is a part of the MAPeD-SMD Copyright 2017-2021 0x8BitDev ( MIT license. See LICENSE.txt )
// Desc: It contains the screen data structure description depending on the export options
//
//######################################################################################################

#ifndef _MPD_SCREEN_H_
#define _MPD_SCREEN_H_

//////////////////
// some defines //
//////////////////

#define SCR_IND_LEFT	0
#define SCR_IND_UP	1
#define SCR_IND_RIGHT	2
#define SCR_IND_DOWN	3
#define SCR_IND_MAX	4

/////////////////
// screen data //
/////////////////

typedef struct mpd_SCREEN
{
#if CHECK_MAP_FLAG( MAP_FLAG_MODE_BIDIR_SCROLL ) || CHECK_MAP_FLAG( MAP_FLAG_MODE_STATIC_SCREENS )
	// CHR data index
	u16		chr_ind;
#endif

#if CHECK_MAP_FLAG( MAP_FLAG_MARKS )
	//(marks) bits: 7-4 - bit mask of user defined adjacent screens ( Down(7)-Right(6)-Up(5)-Left(4) ); 3-0 - screen property
	u16		marks;
#endif

#if CHECK_MAP_FLAG( MAP_FLAG_MODE_STATIC_SCREENS )
	u16		VDP_data_offset;
#endif

#if CHECK_MAP_FLAG( MAP_FLAG_MODE_BIDIR_SCROLL ) || CHECK_MAP_FLAG( MAP_FLAG_MODE_STATIC_SCREENS )
	// screen index
	u16		scr_ind;
#endif

#if CHECK_MAP_FLAG( MAP_FLAG_LAYOUT_ADJACENT_SCREENS )
	union
	{
		struct
		{
			struct mpd_SCREEN*	left;
			struct mpd_SCREEN*	up;
			struct mpd_SCREEN*	right;
			struct mpd_SCREEN*	down;
		};

		struct mpd_SCREEN*	arr[4];
	} adj_scr;
#endif

#if CHECK_MAP_FLAG( MAP_FLAG_LAYOUT_ADJACENT_SCR_INDS )
	union
	{
		struct
		{
			u8	left;
			u8	up;
			u8	right;
			u8	down;
		};

		u8	arr[4];
	} adj_scr_inds;
#endif

#if CHECK_MAP_FLAG( MAP_FLAG_ENTITIES )
	// entities count
	u16			ent_count;
	// entities array
	struct mpd_ENTITY_INSTANCE*	ents;
#endif
} mpd_SCREEN;

#endif // _MPD_SCREEN_H_