;###################################################################
;
; Copyright 2018-2021 0x8BitDev ( MIT license )
;
;###################################################################
;
; DESC: Some useful macroses / routines.
;

		; add 'A *= 2' to an input address
		; OUT: hl
	macro	ADD_ADDR_AX2 _addr
;		ld h, high _addr	<-- optimized version when a tile index is premultiplied by 2
;		ld l, a			<-- (a) is premultiplied by 2 (max 128 tiles)

		ld h, high _addr
		add a
		jp nc, .skip
		inc h 
.skip		
		ld l, a
	endm

	macro	ADD_HL_VAL _val
		ld a, _val
		add l
		ld l, a
		jp nc, .skip
		inc h
.skip
	endm

	macro	SUB_HL_VAL _val
		ld a, l
		sub _val
		ld l, a
		jp nc, .skip
		dec h
.skip
	endm

	macro	MUL_POW2_BC _cnt
		dup _cnt
		sla c
		rl b
		edup
	endm

	macro	LOAD_WDATA _data, _addr
		ld hl, _data
		ld (_addr), hl
	endm

	macro	LOAD_BDATA _data, _addr
		ld a, _data
		ld (_addr), a
	endm

		; IN: hl - screen address
	macro	CHECK_NEXT_THIRD_LINE_HL
		ld a, l
		add #20
		ld l, a
		jp c, .next
		ld a, h
		sub 8
		ld h, a
.next
	endm

	macro	CHECK_NEXT_THIRD_CHR_LINE_HL
		jp nc, .next		; check next CHR line
		ld a, h
		add 8
		and -8
		ld h, a
.next
	endm

	macro	VSYNC
		ei
		halt
		di
	endm	


; http://z80-heaven.wikidot.com/math#toc6
; *** D mul C ***
;
;Returns a 16-bit result
;
; IN:
;     D and C are factors
; OUT:
;     A is the product (lower 8 bits)
;     B is 0
;     C is the overflow (upper 8 bits)
;     DE, HL are not changed
;

d_mul_c

		xor a		;This is an optimised way to set A to zero. 4 cycles, 1 byte.
		ld b,8		;Number of bits in E, so number of times we will cycle through
_mul_loop
		add a,a		;We double A, so we shift it left. Overflow goes into the c flag.
		rl c		;Rotate overflow in and get the next bit of C in the c flag

		jr nc, _skip	;If it is 0, we don't need to add anything to A

		add a,d		;Since it was 1, we do A+1*D

		jr nc, _skip	;Check if there was overflow
		inc c		;If there was overflow, we need to increment E
_skip
		djnz _mul_loop	;Decrements B, if it isn't zero yet, jump back to _mul_loop:

		ret