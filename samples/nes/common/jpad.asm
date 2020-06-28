;###############################################
;
; Copyright 2018-2019 0x8BitDev ( MIT license )
;
;###############################################
;
; Joypad functions
;

; the button constants
JPAD_BTN_A	= %10000000
JPAD_BTN_B	= %01000000
JPAD_BTN_SELECT	= %00100000
JPAD_BTN_START	= %00010000
JPAD_BTN_UP	= %00001000
JPAD_BTN_DOWN	= %00000100
JPAD_BTN_LEFT	= %00000010
JPAD_BTN_RIGHT	= %00000001

.segment "ZP"
_jpad1_state:	.res	1
_jpad2_state:	.res	1

.segment "CODE"

; MACROSES
	.macro jpad1_check_btn btn
	lda #btn
	bit _jpad1_state
	.endmacro

	.macro jpad2_check_btn btn
	lda #btn
	bit _jpad2_state
	.endmacro

	.macro jpad1_get_state
	lda _jpad1_state
	.endmacro

	.macro jpad2_get_state
	lda _jpad2_state
	.endmacro

; joypad state init
	.macro jpad1_init
	; reset the state variable of the first joypad
	lda #$00
	sta _jpad1_state
	.endmacro

	.macro jpad2_init
	; reset the state variable of the first joypad
	lda #$00
	sta _jpad2_state
	.endmacro

; joypad state reading
	.macro read_jpad _port, _jpad_state
	.local jpad_read_loop
	; send the strobe signal
	ldx #$01
	stx $4016
	dex
	stx $4016

	; read joypad buttons state
	ldx #$08

@jpad_read_loop:
	lda _port

	lsr a
	rol _jpad_state

	dex
	bne @jpad_read_loop
	rts	
	.endmacro

; read state of the FIRST joypad
jpad1_read_state:
	read_jpad $4016, _jpad1_state

; read state of the SECOND joypad
jpad2_read_state:
	read_jpad $4017, _jpad2_state
