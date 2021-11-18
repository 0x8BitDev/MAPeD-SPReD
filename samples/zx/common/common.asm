;###################################################################
;
; Copyright 2018-2021 0x8BitDev ( MIT license )
;
;###################################################################
;
; DESC: Some useful macroses / routines.
;

		; add 'A *= 2' to an input address (256-bytes aligned)
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

	macro	ADD_HL_A
		add l
		ld l, a
		jp nc, .skip
		inc h
.skip
	endm

	macro	ADD_HL_VAL _val
		IF _val < 5
		dup _val
		inc hl
		edup
		ELSE
		ld a, _val
		ADD_HL_A
		ENDIF
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

	macro	DIV_POW2_BC _cnt
		dup _cnt
		srl b
		rr c
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
		IF DEF_VERT_SYNC
		ei
		halt
		di
		ENDIF
	endm	

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
._div_loop
		sla c
		rla
		
		cp d

		jr c, ._skip;$+2

		inc c
		sub d
._skip
		djnz ._div_loop;$-10

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

d_mul_c

		xor a		;This is an optimised way to set A to zero. 4 cycles, 1 byte.
		ld b,8		;Number of bits in E, so number of times we will cycle through
._mul_loop
		add a,a		;We double A, so we shift it left. Overflow goes into the c flag.
		rl c		;Rotate overflow in and get the next bit of C in the c flag

		jr nc, ._skip	;If it is 0, we don't need to add anything to A

		add a,d		;Since it was 1, we do A+1*D

		jr nc, ._skip	;Check if there was overflow
		inc c		;If there was overflow, we need to increment E
._skip
		djnz ._mul_loop	;Decrements B, if it isn't zero yet, jump back to _mul_loop:

		ret

		IF	DEF_128K_DBL_BUFFER
_scr_trigg	db 0	; screen switching trigger
_last_port_data	db 0	; last #7ffd port data


		; show the main screen and switch to the 7th bank to draw image

_switch_Uscr

		ld bc, #7ffd
		ld a, (23388)
		ld a, %00000111
		ld (_last_port_data), a
		ld (23388), a
		out (c), a

		ret

		; show the extended screen to draw on the main one
		
_switch_Escr

		ld bc, #7ffd
		ld a, (23388)
		ld a, %00001000
		ld (_last_port_data), a
		ld (23388), a
		out (c), a

		ret

_restore_Xscr

		ld bc, #7ffd
		ld a, (23388)
		ld a, (_last_port_data)
		ld (23388), a
		out (c), a

		ret

_hide_7th_bank

		ld bc, #7ffd
		ld a, (23388)
		ld a, (_last_port_data)
		and %11111000
		ld (23388), a
		out (c), a

		ret

		ENDIF	//DEF_128K_DBL_BUFFER

im2_init
		IF DEF_128K_DBL_BUFFER

		; interrupt init

		ld a,24      		; JR code
		ld (65535),a
		ld a,195     		; JP code
		ld (65524),a
		ld hl,im_prc    	; handler address
		ld (65525),hl
		ld hl,#FE00
		ld de,#FE01
		ld bc,#0100
		ld (hl),#FF
		ld a,h
		ldir
		ld i,a
		im 2

		call _switch_Uscr

		ld a,24      		; JR code
		ld (65535),a
		ld a,195     		; JP code
		ld (65524),a
		ld hl,im_prc    	; handler address
		ld (65525),hl
		ld hl,#FE00
		ld de,#FE01
		ld bc,#0100
		ld (hl),#FF
		ld a,h
		ldir

		jp _hide_7th_bank

		ELSE
		
		ld a,24			; JR code
		ld (65535),a
		ld a,195		; JP code
		ld (65524),a
		ld hl,im_prc		; handler address
		ld (65525),hl
		ld hl,#FE00
		ld de,#FE01
		ld bc,#0100
		ld (hl),#FF
		ld a,h
		ldir
		ld i,a
		im 2

		ret

		ENDIF	//DEF_128K_DBL_BUFFER

im_prc		ei
		reti

		IF DEF_128K_DBL_BUFFER

clear_ext_scr_attrs

		call _switch_Uscr

		; clear attributes of extended screen

		ld hl, #d800
		ld a, 7<<3
		ld (hl), a
		ld de, #d801
		ld bc, #300
		ldir

		jp _hide_7th_bank

		ENDIF //DEF_128K_DBL_BUFFER