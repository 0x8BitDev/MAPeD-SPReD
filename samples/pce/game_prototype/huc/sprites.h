//#######################################################
//
// Generated by SPReD-PCE Copyright 2017-2022 0x8BitDev
//
//#######################################################

#incasm( "sprites.asm" )

#define SPRITES_SPR_VADDR	0x2000

#ifndef	DEF_TYPE_SPD_SPRITE
#define	DEF_TYPE_SPD_SPRITE
typedef struct
{
	const unsigned short	size;
	const unsigned char	SG_ind;
	const unsigned short	attrs[];
} spd_SPRITE;
#endif	//DEF_TYPE_SPD_SPRITE


#define	SPRITES_SG_CNT	1	// graphics banks count
extern unsigned short*	sprites_SG_arr;

#define	SPRITES_PALETTE_SIZE	3	// active palettes
#define SPRITES_PALETTE_SLOT	16

extern unsigned short*	sprites_palette_slot0;
extern unsigned short*	sprites_palette_slot1;
extern unsigned short*	sprites_palette_slot2;
extern unsigned short*	sprites_palette;

#define	SPRITES_FRAMES_CNT	21
extern unsigned char*	sprites_frames_data;

#define	SPR_HEAVY_LOAD_32X64	0
#define	SPR_PLAYER_16X32_0	1
#define	SPR_CHECKPOINT_16X32_1	2
#define	SPR_ENEMY_WALKING_16X32_0_RIGHT	3
#define	SPR_ENEMY_WALKING_16X32_1_LEFT	4
#define	SPR_DOOR_16X32_0	5
#define	SPR_PORTAL_16X32_1	6
#define	SPR_LOGS_32X16	7
#define	SPR_PLATFORM_32X10	8
#define	SPR_ENEMY_FLYING_16X16_RIGHT	9
#define	SPR_ENEMY_FLYING_16X16_LEFT	10
#define	SPR_PLAYER_16X16	11
#define	SPR_COLLECTABLE_GEM	12
#define	SPR_SWITCH_OFF	13
#define	SPR_SWITCH_ON	14
#define	SPR_BUTTON_OFF	15
#define	SPR_BUTTON_ON	16
#define	SPR_HUD_LIVES_HEART	17
#define	SPR_HUD_GEM	18
#define	SPR_HUD_GEMS_CNT	19
#define	SPR_PAUSE_32X16	20

extern spd_SPRITE*	heavy_load_32x64;
extern spd_SPRITE*	player_16x32_0;
extern spd_SPRITE*	checkpoint_16x32_1;
extern spd_SPRITE*	enemy_walking_16x32_0_RIGHT;
extern spd_SPRITE*	enemy_walking_16x32_1_LEFT;
extern spd_SPRITE*	door_16x32_0;
extern spd_SPRITE*	portal_16x32_1;
extern spd_SPRITE*	logs_32x16;
extern spd_SPRITE*	platform_32x10;
extern spd_SPRITE*	enemy_flying_16x16_RIGHT;
extern spd_SPRITE*	enemy_flying_16x16_LEFT;
extern spd_SPRITE*	player_16x16;
extern spd_SPRITE*	collectable_gem;
extern spd_SPRITE*	switch_off;
extern spd_SPRITE*	switch_on;
extern spd_SPRITE*	button_off;
extern spd_SPRITE*	button_on;
extern spd_SPRITE*	HUD_lives_heart;
extern spd_SPRITE*	HUD_gem;
extern spd_SPRITE*	HUD_gems_cnt;
extern spd_SPRITE*	pause_32x16;

