;########################################################################
;
; Multidirectional scroller demo
;
; Copyright 2019-2022 0x8BitDev ( MIT license )
;
;########################################################################

; Supported options:
;
;	- tiles 4x4/2x2 (RLE)/(columns)
;	- properties per CHR (screen attributes)
;	- mode: multidirectional scrolling
;	- layout: matrix (no marks)
;	- no entities

;************************************************************************
;
;	Memory & ROM banks map
;
;************************************************************************

.memorymap
	slot 0 start $0000 size $8000	; 32K of ROM
	slot 1 start $c000 size $2000	; 8K of RAM
	defaultslot 0
.endme

.rombankmap
	bankstotal 2
	banksize $8000
	banks 1
	banksize $2000
	banks 1
.endro

;************************************************************************
;
;	ROM description
;
;************************************************************************

.sdsctag 0.1, "Multidirectional scroller demo", "MAPeD-SMS Sample: Multidirectional scroller", "0x8BitDev"

;************************************************************************
;
;	Some useful includes
;
;************************************************************************

	.incdir "../../common"
	.include "anm_struct.asm"
	.include "vdp_macro_def.asm"

	.incdir "../../common"
	.include "ram_vars.asm"

	.include "tilemap_render_vars.asm"

;************************************************************************
;
;	The main entry point
;
;************************************************************************

.bank 0 slot 0
	.org $0000

	di
	im 1
	ld sp, $dff8

	jp main

;************************************************************************
;
; 	VBLANK handler
;
;	This interrupt can be activated by two events in the VDP chip:
;
;	1. Vertical Blanking Interval.
;	2. Horizontal Line Counter.
;
;************************************************************************

	.org $0038	; mask interrupt handler

	push bc
	push de
	push hl
	push af

	ex af, af'
	push af

	exx
	push bc
	push de
	push hl

	; clear the interrupt request line from the VDP chip

	in a, (VDP_CMD_STATUS_REG)
	bit 7, a			; 7 bit means - VBLANK

	call nz, vblank_handler

	pop hl
	pop de
	pop bc
	exx

	pop af
	ex af, af'

	pop af
	pop hl
	pop de
	pop bc

	ei
	reti

;************************************************************************
;
;	PAUSE handler
;
;************************************************************************

	.org $0066	; NMI handler
	retn

;************************************************************************
;
;	VBLANK handler
;
;************************************************************************

vblank_handler:

	call need_draw
	and $ff
	jr z, exit

	call buff_apply
	call buff_reset

exit:

	call tr_update_scroll

	ret

;************************************************************************
;
;	Some useful includes
;
;************************************************************************

	.incdir "data"
	.include "tilemap.asm"

	.incdir "data/sprite"
	.include "image.asm"

.define CHR_BPP	SPR_CHR_BPP

.if MAP_CHR_BPP != 4
	.printt "*** ERROR: This sample supports 4 bpp tiles only! ***\n"
	.fail
.endif

	.incdir "../../common"
	.include "vdp.asm"
	.include "vdp_data_buffer.asm"
	.include "spr.asm"
	.include "jpad.asm"
	.include "rle.asm"

	.include "common.asm"

	.include "tilemap_render.asm"

;************************************************************************
;
;	The program entry point
;
;************************************************************************

main:
	call VDP_init_clear_mem

	call init_game_level
	call init_sprite

	; init REG0

	VDP_WRITE_REG_CMD 0 VDPR0_FIXED|VDPR0_BLANK_LEFT_COLUMN

	; send show image command to VDP

.if SPR_MODE_8X16 == 1      
	VDP_WRITE_REG_CMD 1 VDPR1_FIXED|VDPR1_DISPLAY_ON|VDPR1_SPRITES_8x16|VDPR1_VBLANK
.else
	VDP_WRITE_REG_CMD 1 VDPR1_FIXED|VDPR1_DISPLAY_ON|VDPR1_VBLANK
.endif

	; sprite tiles use the first 8Kb of VRAM

	VDP_WRITE_REG_CMD 6 VDPR6_SPR_TILES_FIRST_8K

	ei
loop:   
	call tr_jpad_update

	halt		; one frame delay

	; just in case make sure that the drawing operation is complete

drw_wait_loop:

	call need_draw
	and $ff
	jr nz, drw_wait_loop

	jp loop

init_game_level:

	; load palette into the first colors group

	VDP_WRITE_CLR_CMD $0000
	VDP_WRITE_DATA_ARRAY Bank0_Palette 32

	; load CHR data

	ld hl, Bank0_CHR
	ld bc, Bank0_CHR_data_size
	ld de, $0000 + ( $20 * MAP_CHRS_OFFSET )	; VRAM addr (the first CHR bank)

	call VDP_load_tiles

.ifdef	TR_DATA_TILES4X4
	ld hl, Bank0_Tiles
	ld (TR_TILES4x4), hl
.endif
	ld hl, Bank0_Attrs
	ld (TR_BLOCK_ATTRS), hl

.ifdef	TR_DATA_RLE
	ld de, Lev0_Map
	ld hl, decoded_map

	call unrle

	ld hl, decoded_map
.else
	ld hl, Lev0_Map
.endif
	ld (TR_TILES_MAP), hl

	ld hl, Lev0_MapTbl
	ld (TR_MAP_LUT), hl

	ld a, Lev0_StartScr
	ld (TR_START_SCR), a

	ld hl, Lev0_WTilesCnt
	ld (TR_MAP_TILES_WIDTH), hl

	ld hl, Lev0_HTilesCnt
	ld (TR_MAP_TILES_HEIGHT), hl

	ld a, Lev0_WScrCnt
	ld (TR_MAP_SCR_WIDTH), a

	ld a, Lev0_HScrCnt
	ld (TR_MAP_SCR_HEIGHT), a

	jp tr_init

init_sprite:

	; load palette into the second colors group
	; sprite colors are always taken from the second group of 16 colors

	VDP_WRITE_CLR_CMD $0010
	VDP_WRITE_DATA_ARRAY image_palette 16

	; load CHRs data	

	ld hl, SPR_CHR_data
	ld bc, SPR_CHR_data_size
	ld de, $0000 + ( $20 * SPR_CHRS_OFFSET )	; VRAM addr (the first CHR bank)

	call VDP_load_tiles

	; load sprite data

	ld hl, image_frames_data
	ld e, (hl)
	inc hl
	ld d, (hl)		; DE - sprite data addr

	inc hl
	ld b, (hl)		; B - sprite data size

	ex de, hl		; HL - sprite data addr

	ld d, 100
	ld e, 64		; DE - X/Y
	
	call spr_push

	ld hl, SPR_BUFF
	ld de, $3f00 		; VRAM addr (sprites attribute table)

	jp VDP_load_sprites

SPR_CHR_data:
	.incdir "data/sprite"
	.incbin "image_chr0.bin" FSIZE SPR_CHR_data_size

;************************************************************************
;
;	RAM
;
;************************************************************************

.bank 1 slot 1
	