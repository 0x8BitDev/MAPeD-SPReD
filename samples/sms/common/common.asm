;########################################################################
;
; Common macroses and routines
;
; Copyright 2020 0x8BitDev ( MIT license )
;
;########################################################################


.macro MUL_POW2_BC	; \1 - num bits to shift
.repeat \1
	sla c
	rl b
.endr
.endm

.macro MUL_POW2_DE	; \1 - num bits to shift
.repeat \1
	sla e
	rl d
.endr
.endm

.macro MUL_POW2_HL	; \1 - num bits to shift
.repeat \1
	add hl, hl
.endr
.endm

.macro DIV_POW2_BC	; \1 - num bits to shift
.repeat \1
	srl b
	rr c
.endr
.endm

.macro DIV_POW2_DE	; \1 - num bits to shift
.repeat \1
	srl d
	rr e
.endr
.endm

.macro DIV_POW2_HL	; \1 - num bits to shift
.repeat \1
	srl h
	rr l
.endr
.endm

.macro DIV_POW2_A	; \1 - num bits to shift
.repeat \1
	srl a
.endr
.endm

.macro MUL_POW2_A	; \1 - num bits to shift
.repeat \1
	sla a
.endr
.endm

.macro DIV_POW2_C	; \1 - num bits to shift
.repeat \1
	srl c
.endr
.endm

.macro MUL_POW2_C	; \1 - num bits to shift
.repeat \1
	sla c
.endr
.endm

.macro DIV_POW2_B	; \1 - num bits to shift
.repeat \1
	srl b
.endr
.endm

.macro MUL_POW2_B	; \1 - num bits to shift
.repeat \1
	sla b
.endr
.endm

.macro DIV_POW2_E	; \1 - num bits to shift
.repeat \1
	srl e
.endr
.endm

.macro MUL_POW2_E	; \1 - num bits to shift
.repeat \1
	sla e
.endr
.endm

.macro DIV_POW2_D	; \1 - num bits to shift
.repeat \1
	srl d
.endr
.endm

.macro MUL_POW2_D	; \1 - num bits to shift
.repeat \1
	sla d
.endr
.endm

.macro DIV_POW2_L	; \1 - num bits to shift
.repeat \1
	srl l
.endr
.endm

.macro MUL_POW2_L	; \1 - num bits to shift
.repeat \1
	sla l
.endr
.endm

.macro DIV_POW2_H	; \1 - num bits to shift
.repeat \1
	srl h
.endr
.endm

.macro MUL_POW2_H	; \1 - num bits to shift
.repeat \1
	sla h
.endr
.endm

; http://z80-heaven.wikidot.com/math#toc14
; *** C div D ***
;
; IN:
;     C is the numerator
;     D is the denominator
; OUT:
;     A is the remainder
;     B is 0
;     C is the result of C/D
;     D,E,H,L are not changed
;

c_div_d:

	ld b,8

	xor a

	sla c
	rla
	
	cp d

	jr c,$+2

	inc c
	sub d

	djnz $-10

	ret

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

d_mul_c:

	xor a		;This is an optimised way to set A to zero. 4 cycles, 1 byte.
	ld b,8		;Number of bits in E, so number of times we will cycle through
_mul_loop:
	add a,a		;We double A, so we shift it left. Overflow goes into the c flag.
	rl c		;Rotate overflow in and get the next bit of C in the c flag

	jr nc,$+4	;If it is 0, we don't need to add anything to A

	add a,d		;Since it was 1, we do A+1*D

	jr nc,$+1	;Check if there was overflow
	inc c		;If there was overflow, we need to increment E

	djnz _mul_loop	;Decrements B, if it isn't zero yet, jump back to _mul_loop:

	ret