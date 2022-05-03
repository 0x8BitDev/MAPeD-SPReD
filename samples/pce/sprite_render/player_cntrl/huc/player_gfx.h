//#######################################################
//
// Generated by SPReD-PCE Copyright 2017-2022 0x8BitDev
//
//#######################################################

#incasm( "player_gfx.asm" )

#define PLAYER_GFX_SPR_VADDR	8192
#define PLAYER_GFX_PALETTE_SLOT	0


#ifndef	DEF_TYPE_SPD_SPRITE
#define	DEF_TYPE_SPD_SPRITE
typedef struct
{
	const unsigned short*	attrs;
	unsigned char		bank;
	unsigned short		size;
	unsigned char		SG_ind;
} spd_SPRITE;
#endif	//DEF_TYPE_SPD_SPRITE


extern unsigned short*	player_gfx_SG_arr;
const unsigned char	player_gfx_SG_cnt	= 10;	// graphics banks count
const unsigned short	player_gfx_palette_size	= 16;	// words

extern unsigned short*	player_gfx_palette;
const unsigned short	player_gfx_frames_cnt	= 24;
extern spd_SPRITE	player_gfx_frames_data[];

const unsigned char	SPR_MPWR_IDLE01_RIGHT	= 0;
const unsigned char	SPR_MPWR_WALK01_RIGHT	= 1;
const unsigned char	SPR_MPWR_WALK02_RIGHT	= 2;
const unsigned char	SPR_MPWR_WALK03_RIGHT	= 3;
const unsigned char	SPR_MPWR_WALK04_RIGHT	= 4;
const unsigned char	SPR_MPWR_WALK05_RIGHT	= 5;
const unsigned char	SPR_MPWR_WALK06_RIGHT	= 6;
const unsigned char	SPR_MPWR_KICK01_RIGHT	= 7;
const unsigned char	SPR_MPWR_KICK02_RIGHT	= 8;
const unsigned char	SPR_MPWR_KICK03_RIGHT	= 9;
const unsigned char	SPR_MPWR_KICK04_REF02_RIGHT	= 10;
const unsigned char	SPR_MPWR_KICK05_REF01_RIGHT	= 11;
const unsigned char	SPR_MPWR_IDLE01_LEFT	= 12;
const unsigned char	SPR_MPWR_WALK01_LEFT	= 13;
const unsigned char	SPR_MPWR_WALK02_LEFT	= 14;
const unsigned char	SPR_MPWR_WALK03_LEFT	= 15;
const unsigned char	SPR_MPWR_WALK04_LEFT	= 16;
const unsigned char	SPR_MPWR_WALK05_LEFT	= 17;
const unsigned char	SPR_MPWR_WALK06_LEFT	= 18;
const unsigned char	SPR_MPWR_KICK01_LEFT	= 19;
const unsigned char	SPR_MPWR_KICK02_LEFT	= 20;
const unsigned char	SPR_MPWR_KICK03_LEFT	= 21;
const unsigned char	SPR_MPWR_KICK04_REF02_LEFT	= 22;
const unsigned char	SPR_MPWR_KICK05_REF01_LEFT	= 23;

extern spd_SPRITE*	mpwr_idle01_RIGHT_frame;
extern spd_SPRITE*	mpwr_walk01_RIGHT_frame;
extern spd_SPRITE*	mpwr_walk02_RIGHT_frame;
extern spd_SPRITE*	mpwr_walk03_RIGHT_frame;
extern spd_SPRITE*	mpwr_walk04_RIGHT_frame;
extern spd_SPRITE*	mpwr_walk05_RIGHT_frame;
extern spd_SPRITE*	mpwr_walk06_RIGHT_frame;
extern spd_SPRITE*	mpwr_kick01_RIGHT_frame;
extern spd_SPRITE*	mpwr_kick02_RIGHT_frame;
extern spd_SPRITE*	mpwr_kick03_RIGHT_frame;
extern spd_SPRITE*	mpwr_kick04_ref02_RIGHT_frame;
extern spd_SPRITE*	mpwr_kick05_ref01_RIGHT_frame;
extern spd_SPRITE*	mpwr_idle01_LEFT_frame;
extern spd_SPRITE*	mpwr_walk01_LEFT_frame;
extern spd_SPRITE*	mpwr_walk02_LEFT_frame;
extern spd_SPRITE*	mpwr_walk03_LEFT_frame;
extern spd_SPRITE*	mpwr_walk04_LEFT_frame;
extern spd_SPRITE*	mpwr_walk05_LEFT_frame;
extern spd_SPRITE*	mpwr_walk06_LEFT_frame;
extern spd_SPRITE*	mpwr_kick01_LEFT_frame;
extern spd_SPRITE*	mpwr_kick02_LEFT_frame;
extern spd_SPRITE*	mpwr_kick03_LEFT_frame;
extern spd_SPRITE*	mpwr_kick04_ref02_LEFT_frame;
extern spd_SPRITE*	mpwr_kick05_ref01_LEFT_frame;

