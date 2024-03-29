//#######################################################
//
// Generated by SPReD-PCE Copyright 2017-2022 0x8BitDev
//
//#######################################################

#incasm( "sprites_test.asm" )

#define SPRITES_TEST_SPR_VADDR	0x2000

#ifndef	DEF_TYPE_SPD_SPRITE
#define	DEF_TYPE_SPD_SPRITE
typedef struct
{
	const unsigned short	size;
	const unsigned char	SG_ind;
	const unsigned short	attrs[];
} spd_SPRITE;
#endif	//DEF_TYPE_SPD_SPRITE


#define	SPRITES_TEST_SG_CNT	1	// graphics banks count
extern unsigned short*	sprites_test_SG_arr;

#define	SPRITES_TEST_PALETTE_SIZE	6	// active palettes
#define SPRITES_TEST_PALETTE_SLOT	16

extern unsigned short*	sprites_test_palette_slot0;
extern unsigned short*	sprites_test_palette_slot1;
extern unsigned short*	sprites_test_palette_slot2;
extern unsigned short*	sprites_test_palette_slot3;
extern unsigned short*	sprites_test_palette_slot4;
extern unsigned short*	sprites_test_palette_slot5;
extern unsigned short*	sprites_test_palette;

#define	SPRITES_TEST_FRAMES_CNT	10
extern unsigned char*	sprites_test_frames_data;

#define	SPR_JCH_RIGHT_32X64	0
#define	SPR_DR_MSL_UP_16X64_0	1
#define	SPR_DR_MSL_UP_16X64_1_REF	2
#define	SPR_JCH_MIN_16X32_0	3
#define	SPR_JCH_MIN_16X32_1_REF	4
#define	SPR_RPL_FLY_RIGHT_32X32	5
#define	SPR_BRSTICK_RIGHT_32X16	6
#define	SPR_DR_MSL_UP	7
#define	SPR_GIGAN_IDLE_LEFT	8
#define	SPR_TONY_IDLE_RIGHT	9

extern spd_SPRITE*	jch_RIGHT_32x64;
extern spd_SPRITE*	dr_msl_UP_16x64_0;
extern spd_SPRITE*	dr_msl_UP_16x64_1_ref;
extern spd_SPRITE*	jch_min_16x32_0;
extern spd_SPRITE*	jch_min_16x32_1_ref;
extern spd_SPRITE*	rpl_fly_RIGHT_32x32;
extern spd_SPRITE*	brstick_RIGHT_32x16;
extern spd_SPRITE*	dr_msl_UP;
extern spd_SPRITE*	gigan_idle_LEFT;
extern spd_SPRITE*	tony_idle_RIGHT;

