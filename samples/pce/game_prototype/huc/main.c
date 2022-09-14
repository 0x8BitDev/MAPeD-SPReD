//##################################################################
//
// Copyright 2021-2022 0x8BitDev ( MIT license. See LICENSE.txt )
//
// Desc: Simple game prototype demo.
//
// Features:
// ~~~~~~~~~
// - 40 screens (10x4) scrollable multidirectional map
// - inertial player/scene movement
// - sprite/tile objects
// - entities:  collectable, dynamic platforms, buttons, switches, 
//		dynamic obstacles, simple dynamic enemies, checkpoint, level exit
// - hardware collisions
// - 2-level entities cache, +collisions cache
// - infinite credits to complete the game! )))
//
//##################################################################

#define DEMO_VER	"v0.1"

#define	PLAYER_SPRITE_ID	SPR_PLAYER_16X32_0
#define SATB_POS_PLAYER		0
#define SATB_POS_HUD		1
#define SATB_POS_PAUSE		4
#define SATB_POS_ENTITY		5

// debug info:
// - green+red border color	- screen scrolling
// - blue border color		- static screen drawing
// - yellow border color	- getting a tile property
//
#asm
;MPD_DEBUG
;SPD_DEBUG
#endasm

#define	DBG_MODE		0
#define	DBG_SHOW_INFO		0
#define DBG_ENTITIES_OFF	0

#if	DBG_MODE
#define	DBG_UPDATE_ENTITIES	DBG_MPD_BORDER_COLOR( 511 )
#define	DBG_PLAYER_COLLISION	DBG_MPD_BORDER_COLOR( 500 )
#define	DBG_BLACK_BORDER	DBG_MPD_BORDER_COLOR( 0 )
#else
#define	DBG_UPDATE_ENTITIES	//
#define	DBG_PLAYER_COLLISION	//
#define	DBG_BLACK_BORDER	//
#endif

#include <huc.h>

#include "../../common/spd.h"
#include "../../common/mpd_def.h"
#include "sprites.h"
#include "maps.h"
#include "../../common/mpd.h"

u16	__jpad_val	= 0;
u8	__level_state	= 0;

#define	LEVEL_STATE_PASSED	0x01
#define	LEVEL_STATE_FAILED	0x02


void	show_msg( char* _msg )
{
	disp_off();
	wait_vsync();
	cls();

	put_string( _msg, 0, 0 );

	// reset scroll values
	pokew( 0x220c, 0 );
	pokew( 0x2210, 0 );

	disp_on();
	wait_vsync();

	for(;;) {}
}

#include "sprite_set.h"
#include "hdwr_collisions.h"
#include "player.h"
#include "entity_manager.h"
#include "scene.h"


main()
{
#asm
.ifdef	MPD_DEBUG
#endasm
	// make the screen a little smaller so that the border color is visible
//	set_xres( 252, XRES_SOFT );
#asm
.endif
#endasm
	hdwr_collisions_init();

	load_default_font( 0, 0x1a00 );

	scene_main_menu();
}