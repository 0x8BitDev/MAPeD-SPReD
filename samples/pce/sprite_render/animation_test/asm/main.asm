;###################################################################
;
; Copyright 2022 0x8BitDev ( MIT license )
;
;###################################################################
;
; DESC: Simple animation demo.
;

;DEF_INT_IRQ2		; int_irq2
;DEF_INT_IRQ1_VDC	; int_irq1_vdc
;DEF_INT_TIMER		; int_timer
;DEF_INT_NMI		; int_nmi


	.include "../../../common/init.asm"

	.bss

; animation description buffer: dynamic and static data

player_anm:	.ds 7;ANM_RAM_DATA_SIZE

	.data
	.bank 2
	.org $4000

	.include "data/anm_test.asm"

; ROM animation data structure

animation_desc:
	.byte 9				; number of ticks to change a frame
	.byte $06			; number of frames
	.byte $00			; loop frame
	.word anm_test_frames_data	; frames data pointer

	.bank 3
	.org $6000

anm_test_SG_arr:	
	.word 9600, chr0

chr0:	.incbin "data/anm_test_chr0.bin"	; 9600 bytes

	.code
	.bank 0
	.org $e000

	.include "../../../common/sprite_render.asm"
	.include "../../../common/anm.asm"

main:
	; map data banks

	lda #$02
	tam #02

	lda #$03
	tam #03

	lda #$04
	tam #04

	; init data

	; ignore SG data loading to VRAM more than once on anm_copy_frame_to_SATB

	SATB_set_flags SATB_FLAG_CHECK_SG_BANK

	; load palette

	load_palette	anm_test_palette, $100 + ( ANM_TEST_PALETTE_SLOT << 4 ), ( anm_test_palette_end - anm_test_palette )

	; set pointer to SG data array

	stw #anm_test_SG_arr, <_SG_data_arr

	; set sprites VADDR

	stw #ANM_TEST_SPR_VADDR, <_spr_VRAM

	; init animation

	stw #player_anm,	<__ax
	stw #animation_desc,	<__bx

	jsr anm_init

	; enable sprites and VBLANK

	vdc_cr_set VDC_CR_SHOW_SPR | VDC_CR_VBLANK

	; enable auto-DMA VRAM->SATB every VBLANK

	vdc_reg_data VDC_R0F_DCR, #VDC_DCR_DSR

	; enable VDC interrupt

	irq_enable IRQ_FLAG_IRQ1

	cli

.update_loop:

	; update animation frame

	stw #player_anm,	<__ax
	jsr anm_update_frame

	; reset the SATB state

	jsr SATB_reset

	; copy animation frame to the local SATB

	stw #player_anm,	<__ax
	stw #150, 		<_spr_pos_x
	stw #240,		<_spr_pos_y

	jsr anm_copy_frame_to_SATB

	; load the local SATB data to VRAM

	jsr SATB_to_VRAM

	vsync

	jmp .update_loop
