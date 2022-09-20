//##################################################################
//
// Copyright 2021-2022 0x8BitDev ( MIT license. See LICENSE.txt )
//
// This file is a part of the game prototype demo.
//
//##################################################################


#define DEMO_VER	"v0.1"

#define	PLAYER_SPRITE_ID	SPR_PLAYER_16X32_0

// controls aliases

#define	JOY_UP_BTN		JOY_UP
#define	JOY_DOWN_BTN		JOY_DOWN
#define	JOY_LEFT_BTN		JOY_LEFT
#define	JOY_RIGHT_BTN		JOY_RIGHT
#define	JOY_JUMP_BTN		JOY_A
#define	JOY_ACTION_BTN		JOY_B
#define	JOY_PAUSE_BTN		JOY_STRT
#define	JOY_START_BTN		JOY_STRT

// main SATB positions

#define SATB_POS_PLAYER			0
#define SATB_POS_HUD_LIVES		1
#define SATB_POS_HUD_GEM		4
#define SATB_POS_HUD_GEMS_CNT		5
#define SATB_POS_HUD_GEMS_CNT_SHD	6
#define SATB_POS_PAUSE			7
#define SATB_POS_ENTITY			8

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