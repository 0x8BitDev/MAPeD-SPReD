;###################################################################
;
; Copyright 2022 0x8BitDev ( MIT license )
;
;###################################################################
;
; Macroses
;


; XY += word
; X - LOW; Y - HIGH
	.macro add_word_to_xy
	clc
	txa
	adc \1
	tax
	tya
	adc \1 + 1
	tay
	.endm
; \2 = \1 + \2
	.macro add_word_to_word
	clc
	lda low_byte \1
	adc low_byte \2
	sta low_byte \2
	lda high_byte \1
	adc high_byte \2
	sta high_byte \2
	.endm
; word += A
	.macro add_a_to_word
	clc
	adc low_byte \1
	sta low_byte \1

	lda #$00
	adc high_byte \1
	sta high_byte \1
	.endm
; word -= A
	.macro sub_a_from_word
	sta <__al
	sec
	lda low_byte \1
	sbc <__al
	sta low_byte \1

	lda high_byte \1
	sbc #$00
	sta high_byte \1
	.endm
; \1 /= 8
	.macro div8_word
	lda low_byte \1
	lsr a	
	ror high_byte \1

	lsr a	
	ror high_byte \1

	lsr a	
	ror high_byte \1
	sta low_byte \1
	.endm
; \1 *= 8
	.macro mul8_word
	lda low_byte \1
	asl a	
	rol high_byte \1

	asl a	
	rol high_byte \1

	asl a	
	rol high_byte \1
	sta low_byte \1
	.endm	
; val -> *addr
	.macro stw ; \1 - val, \2 - addr
	lda low_byte \1
	sta low_byte \2

	lda high_byte \1
	sta high_byte \2
	.endm
; val -> &(*addr) zpii - zero-page indexed indirect addressing
	.macro stw_zpii ; \1 - val, \2 - addr
	cly
	lda low_byte \1
	sta [\2], y
	iny
	lda high_byte \1
	sta [\2], y
	.endm
; addr = null
	.macro stwz ; \1 - addr
	cla
	sta low_byte \1
	sta high_byte \1
	.endm

	.macro irq_disable
	lda #\1
	sta <_irq_flags
	sta IRQ_DISABLE
	.endm

	.macro irq_enable
	lda #~(\1)	
	and <_irq_flags
	sta <_irq_flags
	sta IRQ_DISABLE
	.endm