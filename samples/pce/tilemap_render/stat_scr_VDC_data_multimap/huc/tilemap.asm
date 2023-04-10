;############################################################
;
; Generated by MAPeD-PCE (x64) Copyright 2017-2023 0x8BitDev
;
;############################################################


; *** GLOBAL DATA ***

_chr0:	.incbin "_tilemap__chr0.bin"		; (28512)
_chr1:	.incbin "_tilemap__chr1.bin"		; (28704)
_chr2:	.incbin "_tilemap__chr2.bin"		; (28704)
_chr3:	.incbin "_tilemap__chr3.bin"		; (28704)


_mpd_CHRs:
	.word _chr0
	.byte bank(_chr0)
	.word _chr1
	.byte bank(_chr1)
	.word _chr2
	.byte bank(_chr2)
	.word _chr3
	.byte bank(_chr3)

_mpd_CHRs_size:
	.word 28512	; (_chr0)
	.word 28704	; (_chr1)
	.word 28704	; (_chr2)
	.word 28704	; (_chr3)

_mpd_Props:	.incbin "_tilemap_Props.bin"	; (3600) block properties array of all exported data banks ( 4 bytes per block )

_mpd_PropsOffs:
	.word 0	; (chr0)
	.word 900	; (chr1)
	.word 1800	; (chr2)
	.word 2700	; (chr3)
	.word 3600	; data end

_mpd_Tiles:	.incbin "_tilemap_Tiles.bin"	; (896) 4x4 tiles array of all exported data banks ( 4 bytes per tile )

_mpd_TilesOffs:
	.word 0	; (chr0)
	.word 224	; (chr1)
	.word 448	; (chr2)
	.word 672	; (chr3)
	.word 896	; data end

_mpd_BlocksOffs:
	.word 0	; (chr0)
	.word 1800	; (chr1)
	.word 3600	; (chr2)
	.word 5400	; (chr3)
	.word 7200	; data end

_mpd_VDCScr:	.incbin "_tilemap_VDCScr.bin"	; (7168) VDC-ready data array for each screen (1792 bytes per screen)
_mpd_TilesScr:	.incbin "_tilemap_TilesScr.bin"	; (224) 4x4 tiles array for each screen ( 56 bytes per screen \ 1 byte per tile )
_mpd_Plts:	.incbin "_tilemap_Plts.bin"	; (2048) palettes array of all exported data banks ( data offset = chr_id * 512 )

; *** _Lev0 ***

_Lev0_StartScr:	.word _Lev0Scr0

_Lev0Scr0:
	.byte 0	; chr_id

	.word 0	; _tilemap_VDCScr offset

	.byte 0	; screen index

; adjacent screen indices ( the valid values are $00 - $FE, $FF - means no screen )
; use the _Lev0_ScrArr array to get a screen description by adjacent screen index
	.byte $FF	; left adjacent screen index
	.byte $FF	; up adjacent screen index
	.byte $FF	; right adjacent screen index
	.byte $FF	; down adjacent screen index

; screens array
_Lev0_ScrArr:
	.word _Lev0Scr0


; *** _Lev1 ***

_Lev1_StartScr:	.word _Lev1Scr0

_Lev1Scr0:
	.byte 1	; chr_id

	.word 1792	; _tilemap_VDCScr offset

	.byte 1	; screen index

; adjacent screen indices ( the valid values are $00 - $FE, $FF - means no screen )
; use the _Lev1_ScrArr array to get a screen description by adjacent screen index
	.byte $FF	; left adjacent screen index
	.byte $FF	; up adjacent screen index
	.byte $FF	; right adjacent screen index
	.byte $FF	; down adjacent screen index

; screens array
_Lev1_ScrArr:
	.word _Lev1Scr0


; *** _Lev2 ***

_Lev2_StartScr:	.word _Lev2Scr0

_Lev2Scr0:
	.byte 2	; chr_id

	.word 3584	; _tilemap_VDCScr offset

	.byte 2	; screen index

; adjacent screen indices ( the valid values are $00 - $FE, $FF - means no screen )
; use the _Lev2_ScrArr array to get a screen description by adjacent screen index
	.byte $FF	; left adjacent screen index
	.byte $FF	; up adjacent screen index
	.byte $FF	; right adjacent screen index
	.byte $FF	; down adjacent screen index

; screens array
_Lev2_ScrArr:
	.word _Lev2Scr0


; *** _Lev3 ***

_Lev3_StartScr:	.word _Lev3Scr0

_Lev3Scr0:
	.byte 3	; chr_id

	.word 5376	; _tilemap_VDCScr offset

	.byte 3	; screen index

; adjacent screen indices ( the valid values are $00 - $FE, $FF - means no screen )
; use the _Lev3_ScrArr array to get a screen description by adjacent screen index
	.byte $FF	; left adjacent screen index
	.byte $FF	; up adjacent screen index
	.byte $FF	; right adjacent screen index
	.byte $FF	; down adjacent screen index

; screens array
_Lev3_ScrArr:
	.word _Lev3Scr0

_mpd_LayoutsArr:
	.word _Lev0_StartScr
	.byte bank(_Lev0_StartScr)
	.word _Lev1_StartScr
	.byte bank(_Lev1_StartScr)
	.word _Lev2_StartScr
	.byte bank(_Lev2_StartScr)
	.word _Lev3_StartScr
	.byte bank(_Lev3_StartScr)

_mpd_LayoutsScrArr:
	.word _Lev0_ScrArr
	.byte bank(_Lev0_ScrArr)
	.word _Lev1_ScrArr
	.byte bank(_Lev1_ScrArr)
	.word _Lev2_ScrArr
	.byte bank(_Lev2_ScrArr)
	.word _Lev3_ScrArr
	.byte bank(_Lev3_ScrArr)

