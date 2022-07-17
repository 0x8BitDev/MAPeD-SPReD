//##################################################################
//
// Copyright 2021-2022 0x8BitDev ( MIT license. See LICENSE.txt )
//
// Desc: Simple show sprites demo using exported simple sprites and
//	 meta-sprites data.
//
//##################################################################

#include <huc.h>
#include "../../../common/spd.h"
#include "sprites_test.h"

unsigned char	SATB_pos = 0;

/* exported sprite set initialization */
void	sprite_set_init()
{
	// load palette in the usual way.
	load_palette( SPRITES_TEST_PALETTE_SLOT, sprites_test_palette, SPRITES_TEST_PALETTE_SIZE );

	// Set up exported sprite set with SG data array and VRAM address to load SG data to.
	// NOTE: You can combine any number of exported sprite sets in your program.
	//	 Call the 'spd_sprite_params' to switch between them.
	// NOTE: Passing ZERO as the third parameter, means that SG data will be automatically
	//	 loaded to VRAM on 'spd_SATB_push_sprite'.
	// NOTE: Passing 'SPD_FLAG_IGNORE_SG' as the third parameter will ignore loading SG to VRAM.
	//	 This is useful for PACKED(!) sprites when you are switching to a sprite set and SG data already loaded to VRAM.
	//	 Such way you avoid loading SG to VRAM twice.
	// NOTE: Passing '_last_bank_ind' allows to avoid loading SG data to VRAM twice when you are switching back from another data set.
	//	 The last value can be obtained using 'spd_SG_bank_get_ind()'. the initial value is 'SPD_SG_BANK_INIT_VAL'.

	spd_sprite_params( sprites_test_SG_arr, SPRITES_TEST_SPR_VADDR, 0, SPD_SG_BANK_INIT_VAL );

	// NOTE: There are two ways to load SG data to VRAM:
	//	 1. Indirect loading, when you push the first sprite by calling 'spd_SATB_push_sprite'.
	//	 The third argument for the 'spd_sprite_params' must be ZERO.
	//
	//	 2. Direct loading, when you call 'spd_copy_SG_data_to_VRAM' with a sprite name/index.
	//	 The third argument for the 'spd_sprite_params' must be 'SPD_FLAG_IGNORE_SG'.
	//
	//	 spd_copy_SG_data_to_VRAM( <exported_name>_frames_data, _spr_ind )
	//	 spd_copy_SG_data_to_VRAM( <sprite_name> )
}

/* show sprite from exported sprite set */
void	sprite_show( char _ind, short _x, short _y )
{
	/* set the SATB position to push sprites to */
	spd_SATB_set_pos( SATB_pos++ );

	// NOTE: There are two ways to push sprite to SATB:
	//	 1. spd_SATB_push_sprite - for meta-sprites
	//
	//	 2. spd_SATB_set_sprite_LT - for simple sprites, it takes a little less processing time
	//	 compared to 'spd_SATB_push_sprite' and allows you to use sprite offset values, that will be
	//	 zeroed out when using HuC sprite functions.
	//
	// NOTE: Double-buffering and 'spd_change_palette' aren't supported with 'spd_SATB_set_sprite_LT'.
	//	 Use 'spd_set_palette_LT' instead of 'spd_change_palette' for simple sprites.

	spd_SATB_set_sprite_LT( sprites_test_frames_data, _ind, _x, _y );
}

void init_sprites()
{
	/* SPD-render initialization */
	spd_init();	

	/* SATB initialization */
	spd_SATB_clear_from( 0 );

	/* initialize exported sprite set */
	sprite_set_init();

	/* there are two ways to show a sprite: */
	/* 1. by sprite index. it's suitable for animation sequences. */

	sprite_show( SPR_JCH_RIGHT_32X64, 138, 96 );

	sprite_show( SPR_JCH_MIN_16X32_0, 173, 61 );

	sprite_show( SPR_JCH_MIN_16X32_1_REF, 205, 61 );

	sprite_show( SPR_DR_MSL_UP_16X64_0, 189, 36 );

	sprite_show( SPR_DR_MSL_UP_16X64_1_REF, 221, 36 );

	/* 2. by sprite data pointer or by sprite name (it is faster than by sprite index). */

	spd_SATB_set_pos( SATB_pos++ );
	spd_SATB_set_sprite_LT( brstick_RIGHT_32x16, 58, 66 );

	spd_SATB_set_pos( SATB_pos++ );
	spd_SATB_set_sprite_LT( rpl_fly_RIGHT_32x32, 98, 66 );

	/* the following sprites are meta-sprites, so we use 'spd_SATB_push_sprite' for them */

	spd_SATB_set_pos( SATB_pos );

	// NOTE: unlike to the 'spd_SATB_set_sprite_LT' the 'spd_SATB_push_sprite' increments SATB position automatically.
	//	 so it is enough to set a start SATB position and then push sprites.

	spd_SATB_push_sprite( dr_msl_UP, 173, 66 );

	spd_SATB_push_sprite( gigan_idle_LEFT, 148, 196 );

	spd_SATB_push_sprite( tony_idle_RIGHT, 88, 196 );

	/* move whole SATB to VRAM */
	satb_update( 64 );

	// NOTE: There are also some functions available for simple sprites:
	//
	//	 void		spd_set_palette_LT( unsigned char _plt_ind );
	//	 unsigned char	spd_get_palette_LT();
	//	 void		spd_set_pri_LT( SPD_SPR_PRI_HIGH/SPD_SPR_PRI_LOW );
	//	 void		spd_set_x_LT( unsigned short _x );
	//	 unsigned short	spd_get_x_LT();
	//	 void		spd_set_y_LT( unsigned short _y );
	//	 unsigned short	spd_get_y_LT();
	//	 void		spd_show_LT();
	//	 void		spd_hide_LT();
	//
	// NOTE: 'spd_set_x_LT' and 'spd_set_y_LT' don't use sprite offset values!
	//	 They work like HuC analogues.
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