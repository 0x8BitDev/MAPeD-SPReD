;############################################################
;
; Generated by MAPeD-PCE (x64) Copyright 2017-2023 0x8BitDev
;
;############################################################

; *** LAYOUTS DATA ***

; *** Lev0 ***

_Lev0_TilesCnt	= 7	; map tiles count
_Lev0_StartScr	= 0	; start screen
_Lev0_WScrCnt	= 3	; number of screens in width
_Lev0_HScrCnt	= 3	; number of screens in height

_Lev0_Layout:	
	.word _Lev0Scr0
	.word _Lev0Scr1
	.word _Lev0Scr2
	.word _Lev0Scr3
	.word _Lev0Scr4
	.word _Lev0Scr5
	.word _Lev0Scr6
	.word _Lev0Scr7
	.word _Lev0Scr8

_Lev0Scr0:

_Lev0Scr1:

_Lev0Scr2:

_Lev0Scr3:

_Lev0Scr4:

_Lev0Scr5:

_Lev0Scr6:

_Lev0Scr7:

_Lev0Scr8:


; *** GLOBAL DATA ***

_chr0:	.incbin "_tilemap__chr0.bin"		; (704)


_mpd_CHRs_size:
	.word 704	; (_chr0)

_mpd_CHRs:
	.word _chr0
	.byte bank(_chr0)

_mpd_Attrs:	.incbin "_tilemap_Attrs.bin"	; (56) attributes array per block ( 2 bytes per attribute; 8 bytes per block ) of all exported data banks

_mpd_BlocksOffs:
	.word 0	; (chr0)
	.word 56	; data end

_mpd_Props:	.incbin "_tilemap_Props.bin"	; (7) blocks properties array ( 1 byte per block ) of all exported data banks

_mpd_PropsOffs:
	.word 0	; (chr0)
	.word 7	; data end

_mpd_Map0:	.incbin "_tilemap_Map0.bin"	; (2016) tilemap blocks (2x2)

_mpd_MapsArr:
	.word _mpd_Map0
	.byte bank(_mpd_Map0)

_mpd_MapsTbl:	.incbin "_tilemap_MapsTbl.bin"	; (96) lookup table for fast calculation of tile addresses; columns by X coordinate ( 16 bit offset per column of tiles ) of all exported data banks

_mpd_MapsTblOffs:
	.word 0
	.word 96	; data end

_mpd_Plts:	.incbin "_tilemap_Plts.bin"	; (512) palettes array of all exported data banks ( data offset = chr_id * 512 )

_mpd_MapsCHRBanks:
	.byte 0

_mpd_StartScrArr:
	.byte _Lev0_StartScr

_mpd_LayoutsDimArr:
	.byte _Lev0_WScrCnt, _Lev0_HScrCnt

_mpd_LayoutsArr:
	.word _Lev0_Layout
	.byte bank(_Lev0_Layout)

