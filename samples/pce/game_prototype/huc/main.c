//##################################################################
//
// Copyright 2021-2022 0x8BitDev ( MIT license. See LICENSE.txt )
//
// Desc: Simple game prototype demo.
//
//##################################################################

#define	PLAYER_SPRITE_ID	SPR_PLAYER_16X32_0
#define SATB_POS_PLAYER		0
#define SATB_POS_ENTITY		1

// debug info:
// - green+red border color	- screen scrolling
// - blue border color		- static screen drawing
// - yellow border color	- getting a tile property
//
#asm
MPD_DEBUG
SPD_DEBUG
#endasm

#define	DBG_MODE		1
#if	DBG_MODE
#define	DBG_SHOW_INFO		0
#endif
#define DBG_ENTITIES_OFF	0

#if	DBG_MODE
#define	DBG_UPDATE_ENTITIES	mpd_dbg_set_border( 511 );
#define	DBG_PLAYER_COLLISION	mpd_dbg_set_border( 500 );
#define	DBG_BLACK_BORDER	mpd_dbg_set_border( 0 );
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

void	show_msg( char* _msg )
{
	disp_off();
	cls();

	put_string( _msg, 0, 0 );

	pokew( 0x220c, 0 );
	pokew( 0x2210, 0 );

	disp_on();
	vsync();

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
	/* initialization of a local SATB with sprite data and sending it to VRAM */
	sprites_init();

	scene_init();

	hdwr_collisions_init();

	load_default_font( 0, 0x1a00 );

	/* demo main loop */
	for( ;; )
	{
		__jpad_val = joy( 0 );

		player_update();

		scene_update();

		wait_vsync();
	}
}