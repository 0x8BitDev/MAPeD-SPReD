//##################################################################
//
// Copyright 2021-2022 0x8BitDev ( MIT license. See LICENSE.txt )
//
// Desc: Simple game prototype demo.
//
// Features:
// ~~~~~~~~~
// - fully playable game prototype, run'n'jump platformer
// - 40 screens (10x4) scrollable multidirectional map with 200+ entities, +bonus level
// - inertial player/map movement (map scrolling uses player inertial movement)
// - sprite/tile objects
// - entities:  collectable, dynamic platform, button, switch, 
//		dynamic obstacles, simple dynamic enemies, checkpoint, level exit
// - hardware collisions
// - 2-level entities cache, +collisions cache
// - infinite credits to complete the game! )))
//
//##################################################################

/*
History:

v0.2
- fixed the player/platform collision bug that led to incorrect drawing of map tiles
- added ability to restart a passed level
- added a passed level stats: attempts, time, gems
- 'hdwr_collisions_init()' replaced with 'hdwr_collisions_enable(TRUE/FALSE)'
- added scene shaking when a heavy load hits the ground

v0.1
- fixed auto-jumps
- replaced solid brick tiles with decorative ones in the grass platforms
- fixed a bug in jumps
- fixed mid-frame palette update
- initial release
*/

// MPD debug info (use Mednaffen PCE Dev version):
// - green border color		- screen scrolling
// - blue border color		- static screen drawing
// - yellow border color	- getting a tile property
//
// SPD debug info (use Mednaffen PCE Dev version):
// - pink border color		- ROM-VRAM data copying
// - white border color		- spd_SATB_push_sprite
// - cyan border color		- attributes transformation

#asm
;MPD_DEBUG
;SPD_DEBUG
#endasm

#define	DBG_MODE		0
#define	DBG_SHOW_INFO		0
#define	DBG_SHOW_NUM_CACHED_SCR	0	// number of cached screens
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

#include "common.h"
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
	// uncomment for Mednaffen non-PCE Dev version
//	set_xres( 252, XRES_SOFT );
#asm
.endif
#endasm
	hdwr_collisions_enable( TRUE );

	load_default_font( 0, 0x1a00 );

	scene_main_menu();
}