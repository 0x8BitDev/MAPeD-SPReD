//##################################################################
//
// Copyright 2021-2022 0x8BitDev ( MIT license. See LICENSE.txt )
//
// This file is a part of the game prototype demo.
//
//##################################################################


/* exported sprite set initialization */
void	main_sprite_set_init()
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

	/* initialize main sprite set */
	main_sprite_set_init();
}
