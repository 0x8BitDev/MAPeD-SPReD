//#######################################################
//
// Generated by MAPeD-PCE Copyright 2017-2022 0x8BitDev
//
//#######################################################


#incasm( "tilemap.asm" )

// export options:
//	- tiles 2x2/(columns)
//	- properties per CHR
//	- mode: bidirectional scrolling
//	- layout: adjacent screen indices (marks)
//	- no entities


/* exported data flags: */

#define FLAG_TILES2X2                 1
#define FLAG_TILES4X4                 0
#define FLAG_RLE                      0
#define FLAG_DIR_COLUMNS              1
#define FLAG_DIR_ROWS                 0
#define FLAG_MODE_MULTIDIR_SCROLL     0
#define FLAG_MODE_BIDIR_SCROLL        1
#define FLAG_MODE_BIDIR_STAT_SCR      0
#define FLAG_MODE_STAT_SCR            0
#define FLAG_ENTITIES                 0
#define FLAG_ENTITY_SCR_COORDS        0
#define FLAG_ENTITY_MAP_COORS         0
#define FLAG_LAYOUT_ADJ_SCR           0
#define FLAG_LAYOUT_ADJ_SCR_INDS      1
#define FLAG_LAYOUT_MATRIX            0
#define FLAG_MARKS                    1
#define FLAG_PROP_ID_PER_BLOCK        0
#define FLAG_PROP_ID_PER_CHR          1

#define MAPS_CNT	1

#define BAT_INDEX	1
#define CHRS_OFFSET	64	// first CHR index from the beginning of VRAM

#define SCR_BLOCKS2x2_WIDTH	16	// number of screen blocks (2x2) in width
#define SCR_BLOCKS2x2_HEIGHT	14	// number of screen blocks (2x2) in height

#define ScrTilesWidth	16	// number of screen tiles (2x2) in width
#define ScrTilesHeight	14	// number of screen tiles (2x2) in height

#define ScrPixelsWidth	256	// screen width in pixels
#define ScrPixelsHeight	224	// screen height in pixels

/* screen data */

typedef struct
{
#if	FLAG_MODE_BIDIR_SCROLL + FLAG_MODE_STAT_SCR
	u8	chr_id;
#endif
#if	FLAG_MARKS
	//bits: 7-4 - bit mask of user defined adjacent screens ( Down(7)-Right(6)-Up(5)-Left(4) ); 3-0 - screen property
	u8	marks;
#endif
#if	FLAG_MODE_STAT_SCR
	u16	VDC_data_offset;
#endif
#if	FLAG_MODE_BIDIR_SCROLL + FLAG_MODE_STAT_SCR
	u8	scr_ind;
#endif
#if	FLAG_LAYOUT_ADJ_SCR
	// adjacent screen pointers
	void*	adj_scr[4];
#endif
#if	FLAG_LAYOUT_ADJ_SCR_INDS
	u8	adj_scr[4];
#endif
#if	FLAG_ENTITIES
	u8	ents_cnt;
	mpd_ENTITY_INSTANCE*	ents[];
#endif
} mpd_SCREEN;


extern u16*	chr0;

#asm
_mpd_CHRs:
	.word bank(_chr0)
	.word _chr0
#endasm

extern u16	mpd_CHRs[];
extern u16*	mpd_CHRs_size;
extern u8*	mpd_Props;
extern u16*	mpd_PropsOffs;
extern u16*	mpd_BlocksOffs;
extern u16*	mpd_Attrs;
extern u8*	mpd_TilesScr;
extern u16*	mpd_Plts;

// *** _Lev0 ***

extern mpd_SCREEN*	Lev0_ScrArr[];
extern mpd_SCREEN*	Lev0_StartScr;

#asm
_Lev0_StartScr:	.word _Lev0Scr29

_Lev0Scr0:
	.byte 0	; chr_id
	.byte $C0	; (marks) bits: 7-4 - bit mask of user defined adjacent screens ( Down(7)-Right(6)-Up(5)-Left(4) ); 3-0 - screen property

	.byte 0	; screen index

; adjacent screen indices ( the valid values are $00 - $FE, $FF - means no screen )
; use the _Lev0_ScrArr array to get a screen description by adjacent screen index
	.byte $FF	; left adjacent screen index
	.byte $FF	; up adjacent screen index
	.byte $01	; right adjacent screen index
	.byte $08	; down adjacent screen index

_Lev0Scr1:
	.byte 0
	.byte $50

	.byte 1

	.byte $00
	.byte $FF
	.byte $02
	.byte $09

_Lev0Scr2:
	.byte 0
	.byte $50

	.byte 2

	.byte $01
	.byte $FF
	.byte $03
	.byte $0A

_Lev0Scr3:
	.byte 0
	.byte $10

	.byte 3

	.byte $02
	.byte $FF
	.byte $FF
	.byte $0B

_Lev0Scr8:
	.byte 0
	.byte $60

	.byte 4

	.byte $FF
	.byte $00
	.byte $09
	.byte $FF

_Lev0Scr9:
	.byte 0
	.byte $50

	.byte 5

	.byte $08
	.byte $01
	.byte $0A
	.byte $FF

_Lev0Scr10:
	.byte 0
	.byte $50

	.byte 6

	.byte $09
	.byte $02
	.byte $0B
	.byte $FF

_Lev0Scr11:
	.byte 0
	.byte $50

	.byte 7

	.byte $0A
	.byte $03
	.byte $0C
	.byte $FF

_Lev0Scr12:
	.byte 0
	.byte $50

	.byte 8

	.byte $0B
	.byte $FF
	.byte $0D
	.byte $FF

_Lev0Scr13:
	.byte 0
	.byte $50

	.byte 9

	.byte $0C
	.byte $FF
	.byte $0E
	.byte $FF

_Lev0Scr14:
	.byte 0
	.byte $50

	.byte 10

	.byte $0D
	.byte $FF
	.byte $0F
	.byte $FF

_Lev0Scr15:
	.byte 0
	.byte $90

	.byte 11

	.byte $0E
	.byte $FF
	.byte $FF
	.byte $17

_Lev0Scr23:
	.byte 0
	.byte $A0

	.byte 12

	.byte $FF
	.byte $0F
	.byte $FF
	.byte $1F

_Lev0Scr29:
	.byte 0
	.byte $40

	.byte 13

	.byte $FF
	.byte $FF
	.byte $1E
	.byte $FF

_Lev0Scr30:
	.byte 0
	.byte $50

	.byte 14

	.byte $1D
	.byte $FF
	.byte $1F
	.byte $FF

_Lev0Scr31:
	.byte 0
	.byte $30

	.byte 15

	.byte $1E
	.byte $17
	.byte $FF
	.byte $FF

; screens array
_Lev0_ScrArr:
	.word _Lev0Scr0
	.word _Lev0Scr1
	.word _Lev0Scr2
	.word _Lev0Scr3
	.word $00
	.word $00
	.word $00
	.word $00
	.word _Lev0Scr8
	.word _Lev0Scr9
	.word _Lev0Scr10
	.word _Lev0Scr11
	.word _Lev0Scr12
	.word _Lev0Scr13
	.word _Lev0Scr14
	.word _Lev0Scr15
	.word $00
	.word $00
	.word $00
	.word $00
	.word $00
	.word $00
	.word $00
	.word _Lev0Scr23
	.word $00
	.word $00
	.word $00
	.word $00
	.word $00
	.word _Lev0Scr29
	.word _Lev0Scr30
	.word _Lev0Scr31

#endasm

extern mpd_SCREEN** mpd_MapsArr[];

#asm
_mpd_MapsArr:
	.word _Lev0_StartScr

#endasm

extern mpd_SCREEN** mpd_MapsScrArr[];

#asm
_mpd_MapsScrArr:
	.word _Lev0_ScrArr

#endasm