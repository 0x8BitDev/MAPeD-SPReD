//#################################################################
//
// Copyright 2021-2022 0x8BitDev ( MIT license. See LICENSE.txt )
//
// DESC: Simple animation demo.
//
//#################################################################

#include <huc.h>
#include "../../../common/spd.h"
#include "anm_test.h"


/* animation sequence description */
typedef struct
{
	unsigned char	curr_tick;
	unsigned char	curr_frame;

	unsigned char	num_ticks;	// number of game ticks or frame updates to change a frame
	unsigned char	num_frames;	// number of animated frames
	unsigned char	loop_frame;
	unsigned char	start_frame;	// start frame index
} anim_desc;

/* our test animation */
anim_desc	test_anim	= { 0, 0, 9, 6, 0, 0 };

/* exported sprite set initialization */
void	sprite_set_init()
{
	// load palette in the usual way.
	load_palette( 16 + ANM_TEST_PALETTE_SLOT, anm_test_palette, anm_test_palette_size >> 4 );

	// set up exported sprite set with SG data array and VRAM address to load SG data to.
	// NOTE: you can combine any number of exported sprite sets in your program.
	//	 call the 'spd_sprite_params' to switch between them.
	// NOTE: passing ZERO as the third parameter, means that SG data of all sprites must 
	//	 be packed in a single file!
	// NOTE: passing 'SPD_FLAG_IGNORE_SG' as the third parameter will ignore loading SG to VRAM.
	//	 it's useful for PACKED(!) sprites when you are switching to a sprite set and SG data already loaded to VRAM.
	//	 such way you avoid loading SG to VRAM twice.
	spd_sprite_params( anm_test_SG_arr, ANM_TEST_SPR_VADDR, 0 );

	// NOTE: There are two ways to load SG data to VRAM:
	//	 1. Indirect loading, when you push the first sprite by calling 'spd_SATB_push_sprite'.
	//	 The third argument for the 'spd_sprite_params' must be ZERO.
	//
	//	 2. Direct loading, when you call 'spd_copy_SG_data_to_VRAM' with a SG data index. It's always ZERO for PACKED sprites.
	//	 The third argument for the 'spd_sprite_params' must be 'SPD_FLAG_IGNORE_SG'.
	//
	//	 spd_copy_SG_data_to_VRAM( _SG_ind ) - '_SG_ind' is an index in the '<exported_name>_SG_arr' array
}

/* show sprite by input index */
char	sprite_show( char _ind, short _x, short _y )
{
	return spd_SATB_push_sprite( anm_test_frames_data, _ind, _x, _y );
}

/* animation update */
void	update_frame( anim_desc* _anm_desc )
{
	if( ++_anm_desc->curr_tick == _anm_desc->num_ticks )
	{
		_anm_desc->curr_tick = 0;

		if( ++_anm_desc->curr_frame == _anm_desc->num_frames )
		{
			_anm_desc->curr_frame = _anm_desc->loop_frame;
		}
	}
}

main()
{
	/* disable display */
	disp_off();

	/* clear display */
	cls();

	/* SPD-render initialization */
	spd_init();	

	/* exported sprite set initialization */
	sprite_set_init();

	/* enable display */
	disp_on();

	/* demo main loop */
	for( ;; )
	{
		/* animation update */
		update_frame( &test_anim );

		/* reset local SATB */
		reset_satb();

		/* set SATB position to push sprites to */
		spd_SATB_set_pos( 0 );

		/* push sprite to SATB */
		sprite_show( test_anim.start_frame + test_anim.curr_frame, 118, 176 );

		/* after pushing all sprites, `spd_SATB_get_pos()` returns the number of sprites in SATB. */
		satb_update( spd_SATB_get_pos() );

		vsync();
	}
}