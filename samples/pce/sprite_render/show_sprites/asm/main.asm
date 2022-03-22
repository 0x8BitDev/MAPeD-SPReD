;###################################################################
;
; Copyright 2022 0x8BitDev ( MIT license )
;
;###################################################################
;
; DESC: Simple example that shows test sprites
;

;DEF_INT_IRQ2		; int_irq2
;DEF_INT_IRQ1_VDC	; int_irq1_vdc
;DEF_INT_TIMER		; int_timer
;DEF_INT_NMI		; int_nmi


	.include "../../../common/init.asm"

	.data
	.bank 2
	.org $4000
	
	.include "sprites_test.asm"

	.code
	.bank 0
	.org $e000

	.include "../../../common/sprite_render.asm"

main:
	; map data bank

	lda #$02
	tam #02
	
	; init data

	jsr SATB_reset

	; ignore CHR data loading twice
	; because both sprites share the same CHR data

	lda #SATB_FLAG_CHECK_CHR_BANK
	sta _SATB_flags

	; load sprites palette

	load_palette	sprites_test_palette, $100 + ( SPRITES_TEST_PALETTE_SLOT << 4 ), sprites_test_palette_end - sprites_test_palette

	; set pointer to CHRs data array

	stw #sprites_test_CHR_arr,	<_CHR_data_arr

	; set sprites VADDR

	stw #SPRITES_TEST_SPR_VADDR,	<_spr_VRAM

	; 1-sprite
	; set sprite data

	stw #gigan_idle_LEFT_frame,	<_spr_data

	; set sprite coordinates

	stw #180, 		<_spr_pos_x
	stw #225, 		<_spr_pos_y

	jsr SATB_push_sprite

	; 2-sprite
	; set sprite data

	stw #tony_idle_RIGHT_frame,	<_spr_data

	; set sprite coordinates

	stw #120, 		<_spr_pos_x
	stw #225, 		<_spr_pos_y

	jsr SATB_push_sprite

	; load SATB data to VRAM

	jsr SATB_update

	; enable VRAM -> SATB DMA each VBLANK

	vdc_reg_data VDC_R0F_DCR,	#VDC_DCR_DSR

	; show the sprites

	vdc_cr_set VDC_CR_SHOW_SPR

.loop:
	vsync

	jmp .loop