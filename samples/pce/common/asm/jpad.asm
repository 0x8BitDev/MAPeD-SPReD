;###################################################################
;
; Copyright 2021 0x8BitDev ( MIT license )
;
;###################################################################
;
; Gamepad routines
;

	.macro jpad1_check_btn
	lda #\1
	bit _jpad1
	.endm

	.macro _delay
	pha
	pla
	nop
	nop
	.endm

jpad_read:

	lda #$01
	sta JPAD_PORT

	lda #$03
	sta JPAD_PORT

	_delay

	ldy #$05

.loop:
	lda #$01
	sta JPAD_PORT
	
	_delay

	lda JPAD_PORT	; read directions
	asl a
	asl a
	asl a
	asl a
	tax

	stz JPAD_PORT

	_delay

	lda JPAD_PORT	; read buttons
	and #$0f
	sta _jpad_arr-1, y
	txa
	ora _jpad_arr-1, y
	eor #$ff
	sta _jpad_arr-1, y
	
	dey
	bne .loop

	rts