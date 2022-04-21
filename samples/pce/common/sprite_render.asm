;###################################################################
;
; Copyright 2022 0x8BitDev ( MIT license )
;
;###################################################################
;
; Sprite render using a local SATB
;

; macroses
;
	.macro set_sprite_data
	stw #\1,	<_spr_data
	stw #\2,	<_spr_pos_x
	stw #\3,	<_spr_pos_y
	.endm

	.macro SATB_set_state
	SATB_get_flags
	ora #\1
	sta _SATB_flags
	.endm

	.macro SATB_get_state
	lda _SATB_flags
	and #SATB_FLAG_STATE_MASK
	.endm

	.macro SATB_get_flags
	lda _SATB_flags
	and #~SATB_FLAG_STATE_MASK
	.endm

	.macro SATB_set_flags
	lda #(\1)
	and #~SATB_FLAG_STATE_MASK
	ora _SATB_flags
	sta _SATB_flags
	.endm

	.macro SATB_wait_state
.wait_loop\@:
	SATB_get_state
	cmp #\1
	bne .wait_loop\@
	.endm

	.data
	.zp
	
; sprite render data
;
_SG_data_arr	.ds 2
_spr_data	.ds 2

_spr_pos_x	.ds 2
_spr_pos_y	.ds 2
_spr_VRAM	.ds 2

	.bss

SATB_SIZE			= 64

SATB_FLAG_CHECK_SG_BANK		= %00000001	; SG data will be copied to VRAM once if set
SATB_FLAG_PEND_SG_DATA		= %00000010	; copy SG data _bsrc/_bdst/_blen for delayed use

SATB_FLAG_STATE_MASK		= %11100000
SATB_FLAG_STATE_FREE		= %10000000	; end of screen drawing
SATB_FLAG_STATE_BUSY		= %01000000	; SATB -> VRAM
SATB_FLAG_STATE_READY		= %00100000	; SATB data moved to VRAM

_SATB_flags	.ds 1
_last_SG_bank	.ds 1
_SATB_pos	.ds 1
_SATB		.ds 512

_SG_DATA_SRC	.ds 2
_SG_DATA_DST	.ds 2
_SG_DATA_LEN	.ds 2

	.code

; *** initialization of the local SATB ***

SATB_reset:

	lda #$ff
	sta _last_SG_bank

	stz _SATB_pos

	stz _SATB
	tii _SATB, _SATB+1, 511

	rts

; *** load the local SATB to VRAM ***

SATB_to_VRAM:

	load_data_to_VRAM #_SATB, #VDC_VRAM_DEFAULT_SAT_ADDR, #$200

	rts

; *** push sprite data to the local SATB ***
; IN:	_SG_data_arr
;	_spr_VRAM
;	_spr_data
;

SATB_push_sprite:

	lda _SATB_pos
	cmp #SATB_SIZE
	bcc .push_data

	; SATB overflow

	rts

.push_data:

	; _bsrci = attributes address

	cly
	lda [<_spr_data], y
	sta _bsrci
	iny
	lda [<_spr_data], y
	sta _bsrci + 1
	iny

	; _bdsti = _SATB + ( _SATB_pos * 8 )

	stw #_SATB, _bdsti

	; calc SATB offset in bytes

	lda _SATB_pos
	sta <__bl
	stz <__bh
	mul8_word <__bl			; __cl - SATB offset in bytes

	; calc SATB address to copy sprite data

	add_word_to_word <__bl, _bdsti

	; _bleni = attributes data length

	lda [<_spr_data], y
	sta _bleni
	iny
	lda [<_spr_data], y
	sta _bleni + 1
	iny

	; _bleni /= 8

	div8_word _bleni		; OUT: A - number of attrs in sprite

	; calc the number of attributes that fit into SATB

	sta <__al			; __al - number of attrs

	lda #SATB_SIZE
	sec
	sbc _SATB_pos
	tax
	sec
	sbc <__al
	bpl .full_sprite

	; clipped attributes

	stx <__al			; __al - number of clipped attrs

.full_sprite:

	lda <__al
	sta <__cl
	stz <__ch
	mul8_word <__cl

	stw <__cl, _bleni

	; sprite attributes

	jsr _TII

	; calc SATB offset address

	add_word_to_word #_SATB, <__bl

	phy

.attr_loop:

	cly

	; attr.Y += _spr_pos_y

	lda [<__bl], y
	tax
	iny
	lda [<__bl], y

	sax
	clc
	adc <_spr_pos_y
	dey
	sta [<__bl], y
	txa
	adc <_spr_pos_y + 1
	iny
	sta [<__bl], y

	iny	

	; attr.X += _spr_pos_x

	lda [<__bl], y
	tax
	iny
	lda [<__bl], y

	sax
	clc
	adc <_spr_pos_x
	dey
	sta [<__bl], y
	txa
	adc <_spr_pos_x + 1
	iny
	sta [<__bl], y

	; calc next attr offset

	lda #$08
	add_a_to_word <__bl

	inc _SATB_pos

	dec <__al			; __al - number of attrs
	bne .attr_loop

	ply

	; load SG data to VRAM

	lda [<_spr_data], y		; A = chr data index

	; check SATB_FLAG_CHECK_SG_BANK flag

	tax
	SATB_get_flags
	and #SATB_FLAG_CHECK_SG_BANK
	beq .load_SG_data

	txa
	cmp _last_SG_bank
	bne .load_SG_data

	; SG data already loaded

	rts

.load_SG_data:

	txa
	sta _last_SG_bank

	asl a
	asl a				; x4

	; __ax = SG data array addr (size, SG data addr) - (*_SG_data_arr) + A

	clc
	adc <_SG_data_arr
	sta <__al
	lda #$00
	adc <_SG_data_arr + 1
	sta <__ah

	; _blen = *__ax (SG data size)

	cly
	lda [<__al], y
	sta _blen
	iny
	lda [<__al], y
	sta _blen + 1
	iny

	; _bsrc = *__ax (SG addr)

	lda [<__al], y
	sta _bsrc
	iny
	lda [<__al], y
	sta _bsrc + 1

	; _bdst = sprite data VRAM

	lda <_spr_VRAM
	sta _bdst
	lda <_spr_VRAM + 1
	sta _bdst + 1

	SATB_get_flags
	and #SATB_FLAG_PEND_SG_DATA
	bne .copy_src_dst_len

	; set VRAM write mode and perform TIA

	jmp vdc_copy_to_VRAM

.copy_src_dst_len:

	stw _SG_DATA_SRC, <__ax
	stw _SG_DATA_DST, <__bx
	stw _SG_DATA_LEN, <__cx

	stw_zpii _bsrc, <__ax
	stw_zpii _bdst, <__bx
	stw_zpii _blen, <__cx

	rts