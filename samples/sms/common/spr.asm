;########################################################################
;
; Copyright 2019-2020 0x8BitDev ( MIT license )
;
;########################################################################
;
; Sprite related functions
;

.define SPR_BUFF_X_CHR_OFFSET	$c080
.define SPR_BUFF_SIZE		$ff

; *** push a sprite to the characters "pool" ***
; IN:	HL - data addr
;	B - data size
;	DE - X/Y

spr_push:

	exx

	ld hl, SPR_BUFF			; Y data addr
	ld de, SPR_BUFF_X_CHR_OFFSET	; X, CHR_ind offset

	ld bc, SPR_NUM_ATTRS
	ld a, (bc)

	exx

	cp 64
	ret z				; buffer overflows!

_push_sprite_loop:

	ld a, (hl)	; A - Y
	add a, e

	; save a / Y
	exx
	ld (hl), a
	inc hl
	exx

	inc hl
	ld a, (hl)	; A - X
	add a, d

	; save a / X
	exx
	ld (de), a
	inc de
	exx

	inc hl
	ld a, (hl)	; A - CHR index

	; save a / CHR_ind
	exx
	ld (de), a
	inc de

	ld a, (bc)
	inc a
	ld (bc), a

	exx

	inc hl	

	cp 64
	jr z, _exit

	dec b
	dec b

	djnz _push_sprite_loop

_exit:
	exx
	ld a, $d0	; the end!
	ld (hl), a
	exx

	ret

; *** characters "pool" initialization ***

spr_buff_init:

	ld hl, SPR_BUFF
	ld a, $d0	; the end!
	ld (hl), a

	ld hl, SPR_NUM_ATTRS
	xor a
	ld (hl), a

	ret