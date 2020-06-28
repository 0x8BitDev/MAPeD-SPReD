;###################################################################
;
; Copyright 2019-2020 0x8BitDev ( MIT license )
;
;###################################################################
;
; VDP functions
;

; *** VDP initialization ***

VDP_init_clear_mem:

	; load init data to VDP

	ld hl, VDP_init_data
	ld b, VDP_init_data_end - VDP_init_data

	ld c, VDP_CMD_STATUS_REG
	otir	

	jp VDP_clear_mem

VDP_init_data:
	.db $04,$80,$00,$81,$ff,$82,$ff,$85,$ff,$86,$ff,$87,$00,$88,$00,$89,$ff,$8a
VDP_init_data_end:

; *** load tiles to VDP ***
; IN:	HL - data addr
;	BC - data size
;	DE - VRAM addr

VDP_load_tiles:

	VDP_ADD_VRAM_ADDR_AND_SEND_CMD_WRITE_RAM

	; some improvements
	; to speed up the process

.repeat 2
	srl b
	rr c
.endr

_load_tiles_loop:

.repeat 4
	ld a, (hl)
	out (VDP_CMD_DATA_REG), a
	inc hl	
.endr

	dec bc

	ld a, b
	or c

	jr nz, _load_tiles_loop

	ret

; *** clear VDP memory ***

VDP_clear_mem:
	
	VDP_WRITE_RAM_CMD $0000

	ld bc, $4000

_clear_VMEM_loop:

	ld a, $00
	out (VDP_CMD_DATA_REG), a

	dec bc

	ld a, b
	or c

	jr nz, _clear_VMEM_loop

	ret

; *** load sprites data to VDP ***
; IN:	HL - sprite buffer
;	DE - VRAM addr

VDP_load_sprites:

	; send command

	VDP_ADD_VRAM_ADDR_AND_SEND_CMD_WRITE_RAM

	; transfer data
	
	ld b, $00	; to get 256 iterations

	ld c, VDP_CMD_DATA_REG
	otir

	ret