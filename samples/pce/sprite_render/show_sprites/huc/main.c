//##################################################################
//
// Copyright 2021-2022 0x8BitDev ( MIT license. See LICENSE.txt )
//
// Desc: Simple show sprites demo using exported normal sprites and
//	 meta-sprites data.
//
//##################################################################

#include <huc.h>
#include "../../../common/spd.h"
#include "sprites_test.h"


/* exported sprite set initialization */
void	sprite_set_init()
{
	// load palette in the usual way.
	load_palette( 16 + SPRITES_TEST_PALETTE_SLOT, sprites_test_palette, sprites_test_palette_size >> 4 );

	// set up exported sprite set with SG data array and VRAM address to load SG data to.
	// NOTE: you can combine any number of exported sprite sets in your program.
	//	 call the 'spd_sprite_params' to switch between them.
	// NOTE: passing ZERO as the third parameter, means that SG data of all sprites must 
	//	 be packed in a single file!
	spd_sprite_params( sprites_test_SG_arr, SPRITES_TEST_SPR_VADDR, 0 );
}

/* show sprite from exported sprite set */
char	sprite_show( char _ind, short _x, short _y )
{
	return spd_SATB_push_sprite( sprites_test_frames_data, _ind, _x, _y );
}

void init_sprites()
{
	/* SPD-render initialization */
	spd_init();	

	init_satb();

	/* initialize exported sprite set */
	sprite_set_init();

	/* set the SATB position to push sprites to */
	spd_SATB_set_pos( 0 );

	/* there are two ways to show a sprite: */
	/* 1. by sprite index. it's suitable for animation sequences. */

	sprite_show( SPR_JCH_RIGHT_32X64, 138, 96 );

	sprite_show( SPR_JCH_MIN_16X32_0, 173, 61 );

	sprite_show( SPR_JCH_MIN_16X32_1_REF, 205, 61 );

	sprite_show( SPR_DR_MSL_UP_16X64_0, 189, 36 );

	sprite_show( SPR_DR_MSL_UP_16X64_1_REF, 221, 36 );

	/* 2. by sprite data pointer. */

	spd_SATB_push_sprite( dr_msl_UP_frame, 173, 66 );

	spd_SATB_push_sprite( brstick_RIGHT_32x16_frame, 58, 66 );

	spd_SATB_push_sprite( rpl_fly_RIGHT_32x32_frame, 98, 66 );

	spd_SATB_push_sprite( gigan_idle_LEFT_frame, 148, 196 );

	spd_SATB_push_sprite( tony_idle_RIGHT_frame, 88, 196 );

	/* update SATB with the all pushed sprites */
	satb_update( spd_SATB_get_pos() );
}

main()
{
	/* disable display */
	disp_off();

	/* clear display */
	cls();

	/* initialization of a local SATB with sprite data and sending it to VRAM */
	init_sprites();

	/* enable display */
	disp_on();

	/* demo main loop */
	for( ;; )
	{
		vsync();
	}
}