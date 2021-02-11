;#######################################################
;
; Generated by MAPeD-SMS Copyright 2017-2021 0x8BitDev
;
;#######################################################

; export options:
;	- tiles 4x4/(columns)
;	- properties per CHR (screen attributes)
;	- mode: bidirectional scrolling
;	- layout: adjacent screen indices (no marks)
;	- no entities


.define MAP_DATA_MAGIC $3104A

; data flags:
.define MAP_FLAG_TILES2X2                 $01
.define MAP_FLAG_TILES4X4                 $02
.define MAP_FLAG_RLE                      $04
.define MAP_FLAG_DIR_COLUMNS              $08
.define MAP_FLAG_DIR_ROWS                 $10
.define MAP_FLAG_MODE_MULTIDIR_SCROLL     $20
.define MAP_FLAG_MODE_BIDIR_SCROLL        $40
.define MAP_FLAG_MODE_STATIC_SCREENS      $80
.define MAP_FLAG_ENTITIES                 $100
.define MAP_FLAG_ENTITY_SCREEN_COORDS     $200
.define MAP_FLAG_ENTITY_MAP_COORS         $400
.define MAP_FLAG_LAYOUT_ADJACENT_SCREENS  $800
.define MAP_FLAG_LAYOUT_ADJACENT_SCR_INDS $1000
.define MAP_FLAG_LAYOUT_MATRIX            $2000
.define MAP_FLAG_MARKS                    $4000
.define MAP_FLAG_PROP_ID_PER_BLOCK        $8000
.define MAP_FLAG_PROP_ID_PER_CHR          $10000
.define MAP_FLAG_PROP_IN_SCR_ATTRS        $20000

.define	MAP_CHR_BPP	4
.define	MAP_CHRS_OFFSET	0	; first CHR index in CHR bank

.define ScrTilesWidth	8	; number of screen tiles (4x4) in width
.define ScrTilesHeight	6	; number of screen tiles (4x4) in height

.define ScrPixelsWidth	256	; screen width in pixels
.define ScrPixelsHeight	192	; screen height in pixels

; *** GLOBAL DATA ***

chr0:	.incbin "tilemap_chr0.bin"		; (3552)

tilemap_CHRs:
	.word chr0

tilemap_CHRs_size:
	.word 3552		;(chr0)

tilemap_Tiles:	.incbin "tilemap_Tiles.bin"	; (1020) 4x4 tiles array of all exported data banks ( 4 bytes per tile )

tilemap_TilesOffs:
	.word 0		; (chr0)

tilemap_Attrs:	.incbin "tilemap_Attrs.bin"	; (696) 2x2 tiles attributes array of all exported data banks ( 2 bytes per attribute ), data offset = tiles offset * 4
tilemap_TilesScr:	.incbin "tilemap_TilesScr.bin"	; (576) 4x4 tiles array for each screen ( 48 bytes per screen \ 1 byte per tile )
tilemap_Plts:	.incbin "tilemap_Plts.bin"	; (32) palettes array of all exported data banks ( data offset = chr_id * 32 )

; *** Lev0 ***

.define Lev0_StartScr	Lev0Scr32

Lev0Scr7:
	.byte 0	; chr_id

	.byte 0	; screen index

; adjacent screen indices ( the valid values are $00 - $FE, $FF - means no screen )
; use the Lev0_ScrArr array to get a screen description by adjacent screen index
	.byte $FF	; left adjacent screen index
	.byte $FF	; up adjacent screen index
	.byte $FF	; right adjacent screen index
	.byte $0F	; down adjacent screen index

Lev0Scr15:
	.byte 0

	.byte 1

	.byte $FF
	.byte $07
	.byte $FF
	.byte $17

Lev0Scr19:
	.byte 0

	.byte 2

	.byte $FF
	.byte $FF
	.byte $14
	.byte $1B

Lev0Scr20:
	.byte 0

	.byte 3

	.byte $13
	.byte $FF
	.byte $15
	.byte $FF

Lev0Scr21:
	.byte 0

	.byte 4

	.byte $14
	.byte $FF
	.byte $16
	.byte $FF

Lev0Scr22:
	.byte 0

	.byte 5

	.byte $15
	.byte $FF
	.byte $17
	.byte $FF

Lev0Scr23:
	.byte 0

	.byte 6

	.byte $16
	.byte $0F
	.byte $FF
	.byte $FF

Lev0Scr27:
	.byte 0

	.byte 7

	.byte $FF
	.byte $13
	.byte $FF
	.byte $23

Lev0Scr32:
	.byte 0

	.byte 8

	.byte $FF
	.byte $FF
	.byte $21
	.byte $FF

Lev0Scr33:
	.byte 0

	.byte 9

	.byte $20
	.byte $FF
	.byte $22
	.byte $FF

Lev0Scr34:
	.byte 0

	.byte 10

	.byte $21
	.byte $FF
	.byte $23
	.byte $FF

Lev0Scr35:
	.byte 0

	.byte 11

	.byte $22
	.byte $1B
	.byte $FF
	.byte $FF

; screens array
Lev0_ScrArr:
	.word $00
	.word $00
	.word $00
	.word $00
	.word $00
	.word $00
	.word $00
	.word Lev0Scr7
	.word $00
	.word $00
	.word $00
	.word $00
	.word $00
	.word $00
	.word $00
	.word Lev0Scr15
	.word $00
	.word $00
	.word $00
	.word Lev0Scr19
	.word Lev0Scr20
	.word Lev0Scr21
	.word Lev0Scr22
	.word Lev0Scr23
	.word $00
	.word $00
	.word $00
	.word Lev0Scr27
	.word $00
	.word $00
	.word $00
	.word $00
	.word Lev0Scr32
	.word Lev0Scr33
	.word Lev0Scr34
	.word Lev0Scr35
	.word $00
	.word $00
	.word $00
	.word $00
