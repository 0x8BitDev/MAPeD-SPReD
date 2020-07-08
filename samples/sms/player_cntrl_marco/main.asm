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

.sdsctag 0.1, "Character controller (Marco)", "SPReD-SMS Sample: Simple character controller", "0x8BitDev"
                                                                 
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
;	Some useful includes
;
;************************************************************************

	.incdir "../common"
	.include "anm_struct.asm"
	.include "ram_vars.asm"
	.include "vdp_macro_def.asm"

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

	VDP_CHECK_FLAG VDP_FLAG_READY
	jr z, exit
 
	ld de, (VDP_TILES_VRAM_ADDR)
	ld hl, (VDP_TILES_ROM_ADDR)
	ld bc, (VDP_TILES_SIZE)

	call VDP_load_tiles

	ld hl, SPR_BUFF
	ld de, $3f00 		; VRAM addr

	call VDP_load_sprites

	VDP_SET_FLAG VDP_FLAG_FREE

exit:
	ret

;************************************************************************
;
;	Another useful includes
;
;************************************************************************

	.incdir "../common"
	.include "jpad.asm"
	.include "vdp.asm"
	.include "anm.asm"
	.include "spr.asm"

	.incdir "./"
	.include "player_anm.asm"

	.incdir "../common"
	.include "character_cntrl.asm"

;************************************************************************
;
;	The program entry point
;
;************************************************************************

main:

	call VDP_init_clear_mem

	xor a
	ld a, (VDP_STATE_FLAGS)
	
	; load palette into the second colors group
	; sprite colors are always taken from the second group of 16 colors

	VDP_WRITE_CLR_CMD $0010
	VDP_WRITE_DATA_ARRAY marco_gfx_palette 16

	; sprite tiles use the first 8Kb of VRAM

	VDP_WRITE_REG_CMD 6 VDPR6_SPR_TILES_FIRST_8K

	; background color

	call set_background_color

	; send show image command to VDP

.if SPR_MODE_8X16 == 1
	VDP_WRITE_REG_CMD 1 VDPR1_FIXED|VDPR1_DISPLAY_ON|VDPR1_SPRITES_8x16|VDPR1_VBLANK
.else
	VDP_WRITE_REG_CMD 1 VDPR1_FIXED|VDPR1_DISPLAY_ON|VDPR1_VBLANK
.endif

	; player init

	ld a, PLAYER_FLAG_DIR_RIGHT

	ld b, 130	; Y
	ld c, 130	; X

	call player_init

	VDP_SET_FLAG VDP_FLAG_FREE

        ei

loop:   
	JPAD_LOAD_STATE

	call player_update

	VDP_CHECK_FLAG VDP_FLAG_FREE
	jr z, _skip_data_preparation		; if equal to zero -> VBLANK works

	; push current animation into the characters "pool"

	call spr_buff_init

	; get anm coordinates

	ld hl, player_anm

	ld e, (hl)
	inc hl
	ld d, (hl)

	push de

	ld hl, player_anm
	ld bc, player_tiles_arr
	call anm_get_frame_gfx_data

	; HL - sprite data address
	; B - sprite data size
	; DE - tiles data address

	push hl
	push bc

	ex de, hl

	; data size

	ld c, (hl)
	inc hl
	ld b, (hl)
	inc hl

	ld (VDP_TILES_SIZE), bc

	; data addr

	ld e, (hl)
	inc hl
	ld d, (hl)

	ld (VDP_TILES_ROM_ADDR), de

	ld hl, $0000		; VRAM addr

	ld (VDP_TILES_VRAM_ADDR), hl

	pop bc
	pop hl

	pop de			; de - X/Y

	call spr_push

	VDP_SET_FLAG VDP_FLAG_READY

_skip_data_preparation:

	VDP_WAITING_FREE_STATE

	jp loop

set_background_color:

	; all memory zeroed at startup so it's not necessary to upload 
	; a tiles data, we can use zeros in the VDP memory as tiles data

	ld hl, VDP_SCR_ATTR
	ld de, $3800		; VRAM address
	ld bc, 32*24		; data size

	VDP_ADD_VRAM_ADDR_AND_SEND_CMD_WRITE_RAM

_load_scr_attrs_loop:

	ld a, (hl)
	out (VDP_CMD_DATA_REG), a
	inc hl

	ld a, (hl)
	out (VDP_CMD_DATA_REG), a
	dec hl

	dec bc

	ld a, b
	or c

	jr nz, _load_scr_attrs_loop
	
	; set the border color

	VDP_WRITE_REG_CMD 7 $f0

	ret

VDP_SCR_ATTR:
	.dw ( $01 << 11 ) | $0100	; use the first tile of the second CHR bank
					; and take colors from the second palette

;************************************************************************
;
;	RAM
;
;************************************************************************

.bank 1 slot 1

	.org $0000

.RAMSECTION "app_vars" APPENDTO "ram_vars"

VDP_TILES_VRAM_ADDR:
	dw
VDP_TILES_ROM_ADDR:
	dw
VDP_TILES_SIZE:
	dw

.ENDS