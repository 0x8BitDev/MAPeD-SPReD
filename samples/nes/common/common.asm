;###############################################
;
; Copyright 2018-2020 0x8BitDev ( MIT license )
;
;###############################################
;
; Some useful macroses and functions
;

.segment "ZP"

; inner variables for temporary calculations
_tmp_val:	.res 2
_tmp_val2:	.res 2
_tmp_val3:	.res 2
_tmp_val4:	.res 2

; outer variables
data_addr:	.res 2
data_size:	.res 2

.segment "CODE"

; MACROSES

; push X
	.macro push_x
	txa
	pha
	.endmacro

; pop X
	.macro pop_x
	pla
	tax
	.endmacro

; push Y
	.macro push_y
	tya
	pha
	.endmacro

; pop Y
	.macro pop_y
	pla
	tay
	.endmacro

; save registers values to the stack
	.macro push_FAXY
	php
	pha
	push_x
	push_y
	.endmacro

; load registers values from the stack
	.macro pop_FAXY
	pop_y
	pop_x
	pla
	plp
	.endmacro

; save input word to the stack
	.macro push_word word
	lda word
	pha
	lda word + 1
	pha
	.endmacro

; load word from the stack
	.macro pop_word word
	pla
	sta word + 1
	pla
	sta word
	.endmacro

; load data pointer
	.macro load_data_ptr ptr, word
	lda #<ptr
	sta word
	lda #>ptr
	sta word + 1
	.endmacro

; save data: word1 -> word2
	.macro load_data_word word1, word2
	lda word1
	sta word2
	lda word1 + 1
	sta word2 + 1
	.endmacro

; addr += 32
	.macro addr_add_32 addr
	lda #$20
	add_a_to_word addr
	.endmacro

; addr += 8
	.macro addr_add_8 addr
	lda addr
	clc
	adc #$08
	sta addr
	.endmacro

; word2 = word1 + word2
	.macro add_word_to_word word1, word2
	clc
	lda word1
	adc word2
	sta word2
	lda word1 + 1
	adc word2 + 1
	sta word2 + 1
	.endmacro

; word += N [0..255]
	.macro add_N_to_word word, N
	clc
	lda word
	adc N
	sta word
	lda word + 1
	adc #$00
	sta word + 1
	.endmacro

; word -= N [0..255]
	.macro sub_N_from_word word, N
	sec
	lda word
	sbc N
	sta word
	lda word + 1
	sbc #$00
	sta word + 1
	.endmacro

; word -= 1
	.macro dec_word word
	sub_N_from_word word, #$01
	.endmacro

; word += 1
	.macro inc_word word
	add_N_to_word word #$01
	.endmacro

; word2 = word1 - word2
	.macro sub_word_from_word word1, word2
	lda word1
	sec
	sbc word2
	sta word2
	lda word1 + 1
	sbc word2 + 1
	sta word2 + 1
	.endmacro

; word = XY - word
	.macro sub_xy_from_word word
	txa
	sec
	sbc word
	sta word
	tya
	sbc word + 1
	sta word + 1
	.endmacro

; sign( word1 - word2 )
; RES: Z = 0 (+); Z = 1 (-)
	.macro sub_word_from_word_sign word1, word2
	lda word1
	sec
	sbc word2
	lda word1 + 1
	sbc word2 + 1
	and #%10000000
	.endmacro

; word += A
	.macro add_a_to_word word
	clc
	adc word
	sta word

	lda #$00
	adc word + 1
	sta word + 1
	.endmacro

; word += -A
	.macro add_neg_a_to_word word
	clc
	adc word
	sta word

	lda #$ff
	adc word + 1
	sta word + 1
	.endmacro

; word += XY
; X - LOW; Y - HIGH
	.macro add_xy_to_word word
	clc
	txa
	adc word
	sta word
	tya
	adc word + 1
	sta word + 1
	.endmacro

; XY += word
; X - LOW; Y - HIGH
	.macro add_word_to_xy word
	clc
	txa
	adc word
	tax
	tya
	adc word + 1
	tay
	.endmacro

; XY = A * 2
	.macro mul2_a_xy
	ldy #$00		;2
	asl a			;2
	tax			;2
	tya			;2
	rol a			;2
	tay			;2
	.endmacro

; XY = A * 4
	.macro mul4_a_xy
	ldy #$00		;2
	asl a			;2
	tax			;2
	tya			;2
	rol a			;2
	tay			;2

	txa			;2
	asl a			;2
	tax			;2
	tya			;2
	rol a			;2
	tay 			;2 = 24
	.endmacro

; word /= 4
	.macro div4_word word
	lda word + 1
	lsr a
	ror word

	lsr a
	ror word
	
	sta word + 1
	.endmacro	

; XY *= 8
	.macro mul8_xy
	sty _tmp_val2
	txa

	asl a
	rol _tmp_val2

	asl a
	rol _tmp_val2

	asl a
	rol _tmp_val2

	tax
	ldy _tmp_val2
	.endmacro

	.macro mul8_word word
	lda word
	asl a	
	rol word + 1

	asl a	
	rol word + 1

	asl a	
	rol word + 1
	sta word
	.endmacro

; XY /= 8
	.macro div8_xy
	tya
	stx _tmp_val2

	lsr a
	ror _tmp_val2

	lsr a
	ror _tmp_val2

	lsr a
	ror _tmp_val2

	tay
	ldx _tmp_val2
	.endmacro

; XY = word / 16
	.macro div16_word_xy word
	lda word
	sta _tmp_val2
	lda word + 1

	lsr a
	ror _tmp_val2

	lsr a
	ror _tmp_val2

	lsr a
	ror _tmp_val2

	lsr a
	ror _tmp_val2

	tay
	ldx _tmp_val2
	.endmacro

; XY = word / 32
	.macro div32_word_xy word
	lda word
	sta _tmp_val2
	lda word + 1

	lsr a
	ror _tmp_val2

	lsr a
	ror _tmp_val2

	lsr a
	ror _tmp_val2

	lsr a
	ror _tmp_val2

	lsr a
	ror _tmp_val2

	tay
	ldx _tmp_val2
	.endmacro

; XY /= 32
	.macro div32_xy
	tya
	stx _tmp_val2

	lsr a
	ror _tmp_val2

	lsr a
	ror _tmp_val2

	lsr a
	ror _tmp_val2

	lsr a
	ror _tmp_val2

	lsr a
	ror _tmp_val2

	tay
	ldx _tmp_val2
	.endmacro

; XY *= 32
	.macro mul32_xy
	sty _tmp_val2
	txa

	asl a
	rol _tmp_val2

	asl a
	rol _tmp_val2

	asl a
	rol _tmp_val2

	asl a
	rol _tmp_val2

	asl a
	rol _tmp_val2

	tax
	ldy _tmp_val2
	.endmacro

; word /= 32
	.macro div32_word word
	lda word + 1
	lsr a
	ror word

	lsr a
	ror word

	lsr a
	ror word

	lsr a
	ror word

	lsr a
	ror word
	sta word + 1
	.endmacro	

; word *= 32
	.macro mul32_word word
	lda word
	asl a
	rol word + 1

	asl a
	rol word + 1

	asl a
	rol word + 1

	asl a
	rol word + 1

	asl a
	rol word + 1
	sta word
	.endmacro	

; word *= 64
	.macro mul64_word word
	lda word
	asl a
	rol word + 1

	asl a
	rol word + 1

	asl a
	rol word + 1

	asl a
	rol word + 1

	asl a
	rol word + 1

	asl a
	rol word + 1
	sta word
	.endmacro	

; word *= 16
	.macro mul16_word word
	lda word
	asl a
	rol word + 1

	asl a
	rol word + 1

	asl a
	rol word + 1

	asl a
	rol word + 1
	sta word
	.endmacro	

; XY *= 16
	.macro mul16_xy
	sty _tmp_val2
	txa

	asl a
	rol _tmp_val2

	asl a
	rol _tmp_val2

	asl a
	rol _tmp_val2

	asl a
	rol _tmp_val2

	tax
	ldy _tmp_val2
	.endmacro

; b1 * b2 = b3
; http://codebase64.org/doku.php?id=base:short_8bit_multiplication_16bit_product
	.macro mul_byte_byte b1, b2, b3
	.local @L1
	.local @L2
	lda #0
	ldx #8
	clc
@L1: 
	bcc @L2
	clc
	adc b2
@L2:
	ror a
   	ror b1
	dex
	bpl @L1
	sta b3 + 1
	lda b1
	sta b3	
	.endmacro

; b1 = b1 / b2 ; A - remainder
; http://codebase64.org/doku.php?id=base:8bit_divide_8bit_product
	.macro div_byte_byte b1, b2
	.local @L1
	.local @L2
	lda #$00
 	ldx #$07
 	clc
@L1:
	rol b1
  	rol a
  	cmp b2
  	bcc @L2	;:+
   	sbc b2
@L2: 	
	dex
 	bpl @L1	;:--
 	rol b1
 	.endmacro	

; b1 = clamp( b1, b2 )
	.macro clamp_byte b1, b2
	lda b1
	sec
	sbc b2
	bpl @m1
	lda b1
	sta b2
@m1:			
	.endmacro

mul8x8:
	; _tmp_val2 = _tmp_val * ( _tmp_val + 1 )
	; _tmp_val - will be corrupted!

	lda #0
	ldx #8
	clc

@tr4x4_mul_bb_L1:
	bcc @tr4x4_mul_bb_L2
	clc
	adc _tmp_val + 1

@tr4x4_mul_bb_L2:
	ror a
	ror _tmp_val
	dex
	bpl @tr4x4_mul_bb_L1

	sta _tmp_val2 + 1
	lda _tmp_val
	sta _tmp_val2	
		
	rts

; *** copy data of any length into memory ***
; IN:
;	X = LOW byte of dest mem addr
;	Y = HIGH byte of dest mem addr
;	data_addr - src mem addr
;	data_size

mem_copy_data:
  	stx _tmp_val
  	sty _tmp_val + 1

	ldy #$00
	ldx data_size + 1
	beq @copy_final

@copy_pages:
	lda (<data_addr), y
	sta (<_tmp_val), y
	iny
	bne @copy_pages
	inc data_addr + 1
	inc _tmp_val + 1
	dex
	bne @copy_pages

@copy_final:
	ldx data_size
	beq @copy_done

	ldy #$00
@copy_loop:
	lda (<data_addr), y
	sta (<_tmp_val), y
	iny
	dex
	bne @copy_loop

@copy_done:
	rts

; RED SCREEN ) for debugging purposes...

red_screen:
	ldx #$00
	ldy #$3f
	lda $2002
	sty $2006
	stx $2006
	lda #$05
	sta $2007
	lda #$16
	sta $2007
	lda #$03
	sta $2007
	lda #$28
	sta $2007

	lda #%00100000
	sta $2001

@loop:
	jsr @loop