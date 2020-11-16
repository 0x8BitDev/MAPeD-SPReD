;#######################################################
;
; Generated by MAPeD-SMS Copyright 2017-2020 0x8BitDev
;
;#######################################################

; export options:
;	- tiles 4x4/(columns)
;	- properties per CHR (screen attributes)
;	- mode: bidirectional scrolling
;	- layout: adjacent screens (marks)
;	- no entities
;	- first color group


.define MAP_DATA_MAGIC $7484A

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
.define MAP_FLAG_COLORS_GROUP_FIRST       $40000
.define MAP_FLAG_COLORS_GROUP_SECOND      $80000

.define	MAP_CHR_BPP	4
.define	MAP_CHRS_OFFSET	0	; first CHR index in CHR bank

.define ScrTilesWidth	8	; number of screen tiles (4x4) in width
.define ScrTilesHeight	6	; number of screen tiles (4x4) in height

.define ScrPixelsWidth	256	; screen width in pixels
.define ScrPixelsHeight	192	; screen height in pixels

; *** GLOBAL DATA ***

chr0:	.incbin "tilemap_chr0.bin"		; (3488)

tilemap_CHRs:
	.word chr0

tilemap_CHRs_size:
	.word 3488		;(chr0)

tilemap_Tiles:	.incbin "tilemap_Tiles.bin"	; (892) 4x4 tiles array of all exported data banks ( 4 bytes per tile )

tilemap_TilesOffs:
	.word 0		; (chr0)

tilemap_Attrs:	.incbin "tilemap_Attrs.bin"	; (1408) 2x2 tiles attributes array of all exported data banks ( 2 bytes per attribute ), data offset = tiles offset * 4
tilemap_TilesScr:	.incbin "tilemap_TilesScr.bin"	; (672) 4x4 tiles array for each screen ( 48 bytes per screen \ 1 byte per tile )
tilemap_Plts:	.incbin "tilemap_Plts.bin"	; (16) palettes array of all exported data banks ( data offset = chr_id * 16 )

; *** Lev0 ***

.define Lev0_StartScr	Lev0Scr24

Lev0Scr6:
	.byte 0	; chr_id
	.byte $40	; (marks) bits: 7-4 - bit mask of user defined adjacent screens ( Down(7)-Right(6)-Up(5)-Left(4) ); 3-0 - screen property

	.byte 0	; screen index

	.word 0	; left adjacent screen
	.word 0	; up adjacent screen
	.word Lev0Scr7	; right adjacent screen
	.word Lev0Scr14	; down adjacent screen

Lev0Scr7:
	.byte 0
	.byte $90

	.byte 1

	.word Lev0Scr6
	.word 0
	.word 0
	.word Lev0Scr15

Lev0Scr14:
	.byte 0
	.byte $C0

	.byte 2

	.word 0
	.word Lev0Scr6
	.word Lev0Scr15
	.word Lev0Scr22

Lev0Scr15:
	.byte 0
	.byte $30

	.byte 3

	.word Lev0Scr14
	.word Lev0Scr7
	.word 0
	.word Lev0Scr23

Lev0Scr22:
	.byte 0
	.byte $60

	.byte 4

	.word 0
	.word Lev0Scr14
	.word Lev0Scr23
	.word Lev0Scr30

Lev0Scr23:
	.byte 0
	.byte $90

	.byte 5

	.word Lev0Scr22
	.word Lev0Scr15
	.word 0
	.word Lev0Scr31

Lev0Scr24:
	.byte 0
	.byte $40

	.byte 6

	.word 0
	.word 0
	.word Lev0Scr25
	.word 0

Lev0Scr25:
	.byte 0
	.byte $50

	.byte 7

	.word Lev0Scr24
	.word 0
	.word Lev0Scr26
	.word 0

Lev0Scr26:
	.byte 0
	.byte $50

	.byte 8

	.word Lev0Scr25
	.word 0
	.word Lev0Scr27
	.word 0

Lev0Scr27:
	.byte 0
	.byte $50

	.byte 9

	.word Lev0Scr26
	.word 0
	.word Lev0Scr28
	.word 0

Lev0Scr28:
	.byte 0
	.byte $50

	.byte 10

	.word Lev0Scr27
	.word 0
	.word Lev0Scr29
	.word 0

Lev0Scr29:
	.byte 0
	.byte $50

	.byte 11

	.word Lev0Scr28
	.word 0
	.word Lev0Scr30
	.word 0

Lev0Scr30:
	.byte 0
	.byte $50

	.byte 12

	.word Lev0Scr29
	.word Lev0Scr22
	.word Lev0Scr31
	.word 0

Lev0Scr31:
	.byte 0
	.byte $30

	.byte 13

	.word Lev0Scr30
	.word Lev0Scr23
	.word 0
	.word 0
