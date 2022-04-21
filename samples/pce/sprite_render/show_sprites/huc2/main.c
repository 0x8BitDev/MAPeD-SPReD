//#################################################################
//
// Copyright 2021-2022 0x8BitDev ( MIT license. See LICENSE.txt )
//
// Desc: Simple show sprites demo using exported PCX images.
//
//#################################################################

#include <huc.h>

// sprites data
#incspr(jackie_chan, "data/jch_RIGHT_32x64.pcx")
#incspr(ripple, "data/rpl_fly_RIGHT_32x32.pcx")
#incspr(broomstick, "data/brstick_RIGHT_32x16.pcx")
#incspr(dryad_missile, "data/dr_msl_UP_16x64_0.pcx")
#incspr(jackie_chan_mini, "data/jch_min_16x32_0.pcx")

// all sprites use the same palette
#incpal(palette, "data/jch_RIGHT_32x64.pcx", 0, 6)

#define	JACKIE_CHAN_PALETTE_SLOT	16
#define	JACKIE_CHAN_VRAM		0x5000

#define	DR_MISSILE_PALETTE_SLOT		21
#define	DR_MISSILE1_VRAM		0x5200
#define	DR_MISSILE2_VRAM		0x5240

#define	RIPPLE_PALETTE_SLOT		17
#define	RIPPLE_VRAM			0x5400

#define	JC_MINI_PALETTE_SLOT		16
#define	JC_MINI1_VRAM			0x5500
#define	JC_MINI2_VRAM			0x5540

#define	BROOMSTICK_PALETTE_SLOT		18
#define	BROOMSTICK_VRAM			0x5600


void init_sprites()
{
	init_satb();

	// set background transparent color from the sprites palette
	load_palette( 0, palette, 1 );

	// init Jackie Chan sprite
	load_vram( JACKIE_CHAN_VRAM, jackie_chan, 8 << 6 );	// 8 SPR 16x16
	load_palette( 16, palette, 6 );				// each exported PCX image contains all palettes

	spr_set( 0 );
	spr_pal( JACKIE_CHAN_PALETTE_SLOT );
	spr_pattern( JACKIE_CHAN_VRAM );
	spr_ctrl( SIZE_MAS|FLIP_MAS, SZ_32x64|NO_FLIP );
	spr_pri( 1 );
	spr_x( 58 );
	spr_y( 70 );

	// init Dryad missile sprites
	load_vram( DR_MISSILE1_VRAM, dryad_missile, 8 << 6 );	// 8 SPR 16x16

	spr_set( 1 );
	spr_pal( DR_MISSILE_PALETTE_SLOT );
	spr_pattern( DR_MISSILE1_VRAM );
	spr_ctrl( SIZE_MAS|FLIP_MAS, SZ_16x64|NO_FLIP );
	spr_pri( 1 );
	spr_x( 134 );
	spr_y( 80 );

	spr_set( 2 );
	spr_pal( DR_MISSILE_PALETTE_SLOT );
	spr_pattern( DR_MISSILE2_VRAM );
	spr_ctrl( SIZE_MAS|FLIP_MAS, SZ_16x64|NO_FLIP );
	spr_pri( 1 );
	spr_x( 166 );
	spr_y( 80 );

	// init Jackie Chan mini sprites
	load_vram( JC_MINI1_VRAM, jackie_chan_mini, 4 << 6 );	// 4 SPR 16x16

	spr_set( 3 );
	spr_pal( JC_MINI_PALETTE_SLOT );
	spr_pattern( JC_MINI1_VRAM );
	spr_ctrl( SIZE_MAS|FLIP_MAS, SZ_16x32|NO_FLIP );
	spr_pri( 1 );
	spr_x( 150 );
	spr_y( 80 );

	spr_set( 4 );
	spr_pal( JC_MINI_PALETTE_SLOT );
	spr_pattern( JC_MINI2_VRAM );
	spr_ctrl( SIZE_MAS|FLIP_MAS, SZ_16x32|NO_FLIP );
	spr_pri( 1 );
	spr_x( 182 );
	spr_y( 80 );

	// init Ripple sprite
	load_vram( RIPPLE_VRAM, ripple, 4 << 6 );		// 4 SPR 16x16

	spr_set( 5 );
	spr_pal( RIPPLE_PALETTE_SLOT );
	spr_pattern( RIPPLE_VRAM );
	spr_ctrl( SIZE_MAS|FLIP_MAS, SZ_32x32|NO_FLIP );
	spr_pri( 1 );
	spr_x( 96 );
	spr_y( 80 );

	// init broomstick sprite
	load_vram( BROOMSTICK_VRAM, broomstick, 2 << 6 );	// 2 SPR 16x16

	spr_set( 6 );
	spr_pal( BROOMSTICK_PALETTE_SLOT );
	spr_pattern( BROOMSTICK_VRAM );
	spr_ctrl( SIZE_MAS|FLIP_MAS, SZ_32x16|NO_FLIP );
	spr_pri( 1 );
	spr_x( 96 );
	spr_y( 112 );

	satb_update();
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