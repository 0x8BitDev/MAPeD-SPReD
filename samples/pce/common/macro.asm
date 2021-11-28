;###################################################################
;
; Copyright 2021 0x8BitDev ( MIT license )
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

	.macro stw ; \1 - val, \2 - addr
	lda low_byte \1
	sta low_byte \2

	lda high_byte \1
	sta high_byte \2
	.endm

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