;############################################################
;
; Generated by MAPeD-PCE (x64) Copyright 2017-2023 0x8BitDev
;
;############################################################


; *** GLOBAL DATA ***

_chr0:	.incbin "_tilemap__chr0.bin"		; (7776)


_mpd_CHRs:
	.word _chr0
	.byte bank(_chr0)

_mpd_CHRs_size:
	.word 7776	; (_chr0)

_mpd_Props:	.incbin "_tilemap_Props.bin"	; (816) block properties array of all exported data banks ( 4 bytes per block )

_mpd_PropsOffs:
	.word 0	; (chr0)
	.word 816	; data end

_mpd_BlocksOffs:
	.word 0	; (chr0)
	.word 1632	; data end

_mpd_Attrs:	.incbin "_tilemap_Attrs.bin"	; (1632) 2x2 tiles attributes array of all exported data banks ( 2 bytes per attribute )
_mpd_TilesScr:	.incbin "_tilemap_TilesScr.bin"	; (3584) 2x2 tiles array for each screen ( 224 bytes per screen \ 1 byte per tile )
_mpd_Plts:	.incbin "_tilemap_Plts.bin"	; (512) palettes array of all exported data banks ( data offset = chr_id * 512 )

; *** _Lev0 ***

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
	.byte $04	; down adjacent screen index

_Lev0Scr1:
	.byte 0
	.byte $50

	.byte 1

	.byte $00
	.byte $FF
	.byte $02
	.byte $05

_Lev0Scr2:
	.byte 0
	.byte $50

	.byte 2

	.byte $01
	.byte $FF
	.byte $03
	.byte $06

_Lev0Scr3:
	.byte 0
	.byte $10

	.byte 3

	.byte $02
	.byte $FF
	.byte $FF
	.byte $07

_Lev0Scr8:
	.byte 0
	.byte $60

	.byte 4

	.byte $FF
	.byte $00
	.byte $05
	.byte $FF

_Lev0Scr9:
	.byte 0
	.byte $50

	.byte 5

	.byte $04
	.byte $01
	.byte $06
	.byte $FF

_Lev0Scr10:
	.byte 0
	.byte $50

	.byte 6

	.byte $05
	.byte $02
	.byte $07
	.byte $FF

_Lev0Scr11:
	.byte 0
	.byte $50

	.byte 7

	.byte $06
	.byte $03
	.byte $08
	.byte $FF

_Lev0Scr12:
	.byte 0
	.byte $50

	.byte 8

	.byte $07
	.byte $FF
	.byte $09
	.byte $FF

_Lev0Scr13:
	.byte 0
	.byte $50

	.byte 9

	.byte $08
	.byte $FF
	.byte $0A
	.byte $FF

_Lev0Scr14:
	.byte 0
	.byte $50

	.byte 10

	.byte $09
	.byte $FF
	.byte $0B
	.byte $FF

_Lev0Scr15:
	.byte 0
	.byte $90

	.byte 11

	.byte $0A
	.byte $FF
	.byte $FF
	.byte $0C

_Lev0Scr23:
	.byte 0
	.byte $A0

	.byte 12

	.byte $FF
	.byte $0B
	.byte $FF
	.byte $0F

_Lev0Scr29:
	.byte 0
	.byte $40

	.byte 13

	.byte $FF
	.byte $FF
	.byte $0E
	.byte $FF

_Lev0Scr30:
	.byte 0
	.byte $50

	.byte 14

	.byte $0D
	.byte $FF
	.byte $0F
	.byte $FF

_Lev0Scr31:
	.byte 0
	.byte $30

	.byte 15

	.byte $0E
	.byte $0C
	.byte $FF
	.byte $FF

; screens array
_Lev0_ScrArr:
	.word _Lev0Scr0
	.word _Lev0Scr1
	.word _Lev0Scr2
	.word _Lev0Scr3
	.word _Lev0Scr8
	.word _Lev0Scr9
	.word _Lev0Scr10
	.word _Lev0Scr11
	.word _Lev0Scr12
	.word _Lev0Scr13
	.word _Lev0Scr14
	.word _Lev0Scr15
	.word _Lev0Scr23
	.word _Lev0Scr29
	.word _Lev0Scr30
	.word _Lev0Scr31

_mpd_LayoutsArr:
	.word _Lev0_StartScr
	.byte bank(_Lev0_StartScr)

_mpd_LayoutsScrArr:
	.word _Lev0_ScrArr
	.byte bank(_Lev0_ScrArr)

