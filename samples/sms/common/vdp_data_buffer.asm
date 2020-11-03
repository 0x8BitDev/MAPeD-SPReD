;########################################################################
;
; VDP Data Buffer
;
; Copyright 2020 0x8BitDev ( MIT license )
;
;########################################################################

; Dynamic VDP data buffer to transfer graphics data during VBLANK

; Public routines:
;
;	buff_reset
;	buff_push_hdr
;	buff_push_data
;	buff_end
;	buff_apply
;
;	need_draw
;

.define TR_FAST_ROW_FILLING	1

.define TR_VRAM_SCR_ATTR_ADDR 	$3800

.define TR_BUFF_STEP_64		%10000000		; 1 by default
.define TR_BUFF_END		0			; end of the buffer


;*** loop VRAM attribute address ***

.macro LOOP_VRAM_ATTR_ADDR_HL	; \1 - 1-immediate ret, 0-continue
	ld a, h
	cp >TR_VRAM_SCR_ATTR_ADDR + $07	; $3f

.if \1 == 1
	ret m
.else
	jp m, _cont\@
.endif
	; clamp address to $3800-$3f00 range

	ld de, $700

	and a
	sbc hl, de

.if \1 == 0
_cont\@:
.endif

.endm

; *** reset the buffer state ***

buff_reset:

	xor a
	ld ( tr_buff ), a

	ld hl, tr_buff
	ld ( tr_buff_addr ), hl

	ret

; *** push data header before data filling ***
; IN:	A - flags | length
;	DE - VRAM addr

buff_push_hdr:

	ld hl, ( tr_buff_addr )

	ld (hl), a
	inc hl

	ld (hl), e
	inc hl
	ld (hl), d
	inc hl

	ld ( tr_buff_addr ), hl

	ret

; *** push data into the buffer ***
; IN:	DE - data

buff_push_data:

	ld hl, ( tr_buff_addr )

	ld (hl), e
	inc hl
	ld (hl), d
	inc hl

	ld ( tr_buff_addr ), hl

	ret

; *** data end marker ***

buff_end:

	ld hl, ( tr_buff_addr )

	ld a, TR_BUFF_END
	ld (hl), a

	ret

; *** transfer the buffer data to the VDP ***

buff_apply:

	ld hl, tr_buff

_buff_load_header:

	ld a, (hl)
	ld c, a
	cp TR_BUFF_END
	jr nz, _buff_cont

	ret

_buff_cont:

	and TR_BUFF_STEP_64
	jr z, _buff_step_1

	; 32 bytes step

	call _buff_set_VDP_addr

	push de
	exx
	pop de
	exx

_buff_data_loop:

	VDP_SCR_ATTR_TO_DATA_REG_HL

	; VRAMaddr += 64

	exx
	ld hl, $40
	add hl, de

	LOOP_VRAM_ATTR_ADDR_HL 0

	ex de, hl

	VDP_WRITE_RAM_CMD_DE
	exx

	djnz _buff_data_loop

	jp _buff_load_header

_buff_step_1:

	call _buff_set_VDP_addr

.if	TR_FAST_ROW_FILLING == 1

	sla b
	ld c, VDP_CMD_DATA_REG
	otir	
.else

	push de
	exx
	pop de

	ld a, e
	and %11000000
	ld c, a

	exx

_buff_data_loop2:

	VDP_SCR_ATTR_TO_DATA_REG_HL

	; VRAMaddr += 2

	exx

	ld a, e
	add a, 2
	and %00111111
	or c
	ld e, a

	VDP_WRITE_RAM_CMD_DE

	exx

	djnz _buff_data_loop2
.endif
	jp _buff_load_header

_buff_set_VDP_addr:

	; OUT: B - data length

	ld a, c
	and ~TR_BUFF_STEP_64	; a - data length
	ld b, a

	; get VRAM addr

	inc hl
	ld e, (hl)
	inc hl
	ld d, (hl)
	inc hl

	VDP_WRITE_RAM_CMD_DE

	ret

; *** is there any data in the buffer for transfer to the VDP? ***
; OUT: A != 0 - need draw

need_draw:

	ld hl, tr_buff
	ld a, (hl)

	ret