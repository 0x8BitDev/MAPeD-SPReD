;########################################################################
;
; Bidirectional scroller demo
;
; Copyright 2019-2020 0x8BitDev ( MIT license )
;
;########################################################################

; Supported options:
;
;	- tiles 4x4 / 2x2 (columns)
;	- properties per CHR: any
;	- mode: bidirectional scrolling
;	- layout: adjacent screens OR adjacent screen inds (NO MARKS!)
;	- entities: any
;	- first color group
;	- CHR bpp: 4
;

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

.sdsctag 0.1, "Bidirectional scroller demo", "MAPeD-SMS Sample: Bidirectional scroller", "0x8BitDev"

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

	call update_scroll

	ret

;************************************************************************
;
;	Some useful includes
;
;************************************************************************

	.incdir "data"
	.include "tilemap.asm"

.define CHR_BPP	MAP_CHR_BPP

.if MAP_CHR_BPP != 4
	.printt "*** ERROR: This sample supports 4 bpp tiles only! ***\n"
	.fail
.endif

.if MAP_DATA_MAGIC&MAP_FLAG_COLORS_GROUP_SECOND == MAP_FLAG_COLORS_GROUP_SECOND
	.printt "*** ERROR: The second color group for a palette isn't supported by this sample! ***\n"
	.fail
.endif

.if MAP_DATA_MAGIC&MAP_FLAG_MARKS == MAP_FLAG_MARKS
	.printt "*** ERROR: This sample doesn't support screen marks data! Please, re export with 'Marks: OFF'. ***\n"
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

	; init REG0

	VDP_WRITE_REG_CMD 0 VDPR0_FIXED|VDPR0_BLANK_LEFT_COLUMN

	; send show image command to VDP

	VDP_WRITE_REG_CMD 1 VDPR1_FIXED|VDPR1_DISPLAY_ON|VDPR1_VBLANK

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

.ifdef	TR_DATA_TILES4X4
	ld hl, tilemap_Tiles
	ld (TR_TILES4x4), hl
.endif
	ld hl, tilemap_Attrs
	ld (TR_BLOCK_ATTRS), hl

	ld hl, tilemap_TilesOffs
	ld (TR_TILES_OFFSETS), hl

	ld hl, Lev0_StartScr
	ld (TR_START_SCR), hl

	ld hl, ScrTilesWidth
	ld (TR_MAP_TILES_WIDTH), hl

	ld hl, ScrTilesHeight
	ld (TR_MAP_TILES_HEIGHT), hl

	ld hl, tilemap_CHRs
	ld (TR_CHR_ARR), hl

	ld hl, tilemap_CHRs_size
	ld (TR_CHR_SIZE_ARR), hl

	ld hl, tilemap_TilesScr
	ld (TR_SCR_TILES_ARR), hl

	ld hl, tilemap_Plts
	ld (TR_PALETTES_ARR), hl

.ifdef	TR_ADJACENT_SCR_INDS
	ld hl, Lev0_ScrArr
	ld (TR_SCR_LABELS_ARR), hl
.endif

	jp tr_init

;************************************************************************
;
;	RAM
;
;************************************************************************

.bank 1 slot 1
	