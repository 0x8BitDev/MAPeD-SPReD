;########################################################################
;
; Static screens switching demo (VDP-ready screens data)
;
; Copyright 2019-2020 0x8BitDev ( MIT license )
;
;########################################################################

; Supported options:
;
;	- mode: static screens
;	- properties per CHR: any
;	- layout: adjacent screens OR adjacent screen inds (NO MARKS!)
;	- entities: off
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

	;...

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

.if MAP_DATA_MAGIC&MAP_FLAG_MARKS == MAP_FLAG_MARKS
	.printt "*** ERROR: This sample doesn't support screen marks data! Please, re export with 'Marks: OFF'. ***\n"
	.fail
.endif

.if MAP_DATA_MAGIC&MAP_FLAG_LAYOUT_MATRIX == MAP_FLAG_LAYOUT_MATRIX
	.printt "*** ERROR: This sample doesn't support a matrix layout! Please, re export with 'Adjacent screens' or 'Adjacent screen inds'. ***\n"
	.fail
.endif

.if MAP_DATA_MAGIC&MAP_FLAG_RLE == MAP_FLAG_RLE
	.printt "*** ERROR: This sample doesn't support RLE compressed data! Please, re export with 'RLE: OFF'. ***\n"
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
;	ROM description
;
;************************************************************************

.sdsctag 0.1, "Static screens switching demo (VDP-ready screens data)", "MAPeD-SMS Sample: Static screens switching demo", "0x8BitDev"

;************************************************************************
;
;	The program entry point
;
;************************************************************************

main:
	call VDP_init_clear_mem

	call init_game_level

	; init REG0

.ifdef	TR_BIDIR_STAT_SCR
	VDP_WRITE_REG_CMD 0 VDPR0_FIXED
.else
	VDP_WRITE_REG_CMD 0 VDPR0_FIXED|VDPR0_BLANK_LEFT_COLUMN
.endif
	; send show image command to VDP

	VDP_WRITE_REG_CMD 1 VDPR1_FIXED|VDPR1_DISPLAY_ON|VDPR1_VBLANK

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

	ld hl, Lev0_StartScr
	ld (TR_START_SCR), hl

	ld hl, tilemap_CHRs
	ld (TR_CHR_ARR), hl

	ld hl, tilemap_CHRs_size
	ld (TR_CHR_SIZE_ARR), hl

	ld hl, tilemap_VDPScr
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
	