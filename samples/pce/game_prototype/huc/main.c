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
;MPD_DEBUG
#endasm

#define	DBG_MODE	1

#if	DBG_MODE
#define	DBG_UPDATE_ENTITIES	mpd_dbg_set_border( 511 );
#define	DBG_PLAYER_COLLISION	mpd_dbg_set_border( 500 );
#define	DBG_BLACK_BORDER	mpd_dbg_set_border( 0 );
#else
#define	DBG_ENTITY_UPDATE	//
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

	load_default_font();

	put_string( _msg, 0, 0 );

	pokew( 0x220c, 0 );
	pokew( 0x2210, 0 );

	disp_on();
	vsync();

	for(;;) {}
}

#include "hdwr_collision.h"
#include "player.h"
#include "entity_manager.h"
#include "scene.h"


/* exported sprite set initialization */
void	sprite_set_init()
{
	// load palette in the usual way.
	load_palette( SPRITES_PALETTE_SLOT, sprites_palette, SPRITES_PALETTE_SIZE );

	// set up exported sprite set with SG data array and VRAM address to load SG data to.
	// NOTE: you can combine any number of exported sprite sets in your program.
	//	 call the 'spd_sprite_params' to switch between them.
	// NOTE: passing ZERO as the third parameter, means that SG data of all sprites must 
	//	 be packed in a single file!
	// NOTE: passing 'SPD_FLAG_IGNORE_SG' as the third parameter will ignore loading SG to VRAM.
	//	 it's useful for PACKED(!) sprites when you are switching to a sprite set and SG data already loaded to VRAM.
	//	 such way you avoid loading SG to VRAM twice.
	spd_sprite_params( sprites_SG_arr, SPRITES_SPR_VADDR, SPD_FLAG_IGNORE_SG, SPD_SG_BANK_INIT_VAL );

	// NOTE: There are two ways to load SG data to VRAM:
	//	 1. Indirect loading, when you push the first sprite by calling 'spd_SATB_push_sprite'.
	//	 The third argument for the 'spd_sprite_params' must be ZERO.
	//
	//	 2. Direct loading, when you call 'spd_copy_SG_data_to_VRAM' with a SG data index. It's always ZERO for PACKED sprites.
	//	 The third argument for the 'spd_sprite_params' must be 'SPD_FLAG_IGNORE_SG'.
	//
	spd_copy_SG_data_to_VRAM( sprites_frames_data, 0 );	// '_SG_ind' is an index in the '<exported_name>_SG_arr' array
}

void	sprites_init()
{
	/* SPD library initialization */
	spd_init();	

	/* init local SATB */
	spd_SATB_clear_from( 0 );

	/* initialize exported sprite set */
	sprite_set_init();
}

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

	/* demo main loop */
	for( ;; )
	{
		__jpad_val = joy( 0 );

		player_update();

		scene_update();

		wait_vsync();
	}
}