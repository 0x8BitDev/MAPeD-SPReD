;#######################################################
;
; Generated by MAPeD-SMS Copyright 2017-2022 0x8BitDev
;
;#######################################################

; export options:
;	- tiles 4x4/(columns)
;	- properties per CHR (screen attributes)
;	- mode: static screens
;	- layout: adjacent screens (no marks)
;	- no entities


.define MAP_DATA_MAGIC $3088A

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

.define SCR_BLOCKS2x2_WIDTH	16	; number of screen blocks (2x2) in width
.define SCR_BLOCKS2x2_HEIGHT	12	; number of screen blocks (2x2) in height

.define ScrTilesWidth	8	; number of screen tiles (4x4) in width
.define ScrTilesHeight	6	; number of screen tiles (4x4) in height

.define ScrPixelsWidth	256	; screen width in pixels
.define ScrPixelsHeight	192	; screen height in pixels

; *** GLOBAL DATA ***

chr0:	.incbin "tilemap_chr0.bin"		; (3072)
chr1:	.incbin "tilemap_chr1.bin"		; (7072)
chr2:	.incbin "tilemap_chr2.bin"		; (7424)

tilemap_CHRs:
	.word chr0
	.word chr1
	.word chr2

tilemap_CHRs_size:
	.word 3072	; (chr0)
	.word 7072	; (chr1)
	.word 7424	; (chr2)

tilemap_VDPScr:	.incbin "tilemap_VDPScr.bin"	; (4608) VDP-ready data array for each screen (1536 bytes per screen)

.define ScrGfxDataSize	 1536

tilemap_Plts:	.incbin "tilemap_Plts.bin"	; (96) palettes array of all exported data banks ( data offset = chr_id * 32 )

; *** Lev0 ***

.define Lev0_StartScr	Lev0Scr1

Lev0Scr0:
	.byte 1	; chr_id

	.word 1536	; tilemap_VDPScr offset

	.byte 1	; screen index

	.word 0	; left adjacent screen
	.word 0	; up adjacent screen
	.word Lev0Scr1	; right adjacent screen
	.word 0	; down adjacent screen

Lev0Scr1:
	.byte 0

	.word 0

	.byte 0

	.word Lev0Scr0
	.word 0
	.word Lev0Scr2
	.word 0

Lev0Scr2:
	.byte 2

	.word 3072

	.byte 2

	.word Lev0Scr1
	.word 0
	.word 0
	.word 0


MapsArr:
	.word Lev0_StartScr

