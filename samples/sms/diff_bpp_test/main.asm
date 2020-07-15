;########################################################################
;
; Copyright 2019-2020 0x8BitDev ( MIT license )
;
;########################################################################

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

.sdsctag 0.1, "BPP test", "SPReD-SMS Sample: bpp test", "0x8BitDev"

;************************************************************************
;
;	Some useful includes
;
;************************************************************************

	.incdir "../common"
	.include "anm_struct.asm"
	.include "vdp_macro_def.asm"

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
;	PAUSE handler
;
;************************************************************************

	.org $0066	; NMI handler
	retn

;************************************************************************
;
;	Some useful includes
;
;************************************************************************

	.incdir "data"
	.include "image.asm"

	.incdir "../common"
	.include "vdp.asm"
	.include "spr.asm"

;************************************************************************
;
;	The program entry point
;
;************************************************************************

main:
	call VDP_init_clear_mem

	; load palette into the second colors group
	; sprite colors are always taken from the second group of 16 colors

	VDP_WRITE_CLR_CMD $0010
	VDP_WRITE_DATA_ARRAY image_palette 16

	; load CHRs data	

	ld hl, CHR_data
	ld bc, CHR_data_size
	ld de, $0000		; VRAM addr (the first CHR bank)

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
	ld e, 50		; DE - X/Y
	
	call spr_push

	ld hl, SPR_BUFF
	ld de, $3f00 		; VRAM addr (sprites attribute table)

	call VDP_load_sprites

	; send show image command to VDP

.if SPR_MODE_8X16 == 1      
	VDP_WRITE_REG_CMD 1 VDPR1_FIXED|VDPR1_DISPLAY_ON|VDPR1_SPRITES_8x16
.else
	VDP_WRITE_REG_CMD 1 VDPR1_FIXED|VDPR1_DISPLAY_ON
.endif

	; sprite tiles use the first 8Kb of VRAM

	VDP_WRITE_REG_CMD 6 VDPR6_SPR_TILES_FIRST_8K

loop:   
	jp loop

CHR_data:
	.incbin "image_chr0.bin" FSIZE CHR_data_size

;************************************************************************
;
;	RAM
;
;************************************************************************

.bank 1 slot 1
 
	.incdir "../common"
	.include "ram_vars.asm"
	