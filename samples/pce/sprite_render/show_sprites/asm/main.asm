;###################################################################
;
; Copyright 2022 0x8BitDev ( MIT license )
;
;###################################################################
;
; DESC: Simple demo that shows test sprites.
;

;DEF_INT_IRQ2		; int_irq2
;DEF_INT_IRQ1_VDC	; int_irq1_vdc
;DEF_INT_TIMER		; int_timer
;DEF_INT_NMI		; int_nmi


	.include "../../../common/init.asm"

	.data
	.bank 2
	.org $4000
	
	.include "data/sprites_test.asm"

sprites_test_SG_arr:
	.word 8192, sprites_test_chr0

	.bank 3
	.org $6000

sprites_test_chr0:	.incbin "data/sprites_test_chr0.bin"	; 8192 bytes

	.code
	.bank 0
	.org $e000

	.include "../../../common/sprite_render.asm"

main:
	; map data bank

	lda #$02
	tam #02

	lda #$03
	tam #03

	; init data

	jsr SATB_reset

	; ignore SG data loading twice
	; because all sprites share the same SG data

	lda #SATB_FLAG_CHECK_SG_BANK
	sta _SATB_flags

	; load sprites palette

	load_palette	sprites_test_palette, $100 + ( SPRITES_TEST_PALETTE_SLOT << 4 ), sprites_test_palette_end - sprites_test_palette

	; set pointer to SG data array

	stw #sprites_test_SG_arr,	<_SG_data_arr

	; set sprites VADDR

	stw #SPRITES_TEST_SPR_VADDR,	<_spr_VRAM

	; push sprites data to SATB

	set_sprite_data brstick_RIGHT_32x16_frame, 90, 130
	jsr SATB_push_sprite

	set_sprite_data rpl_fly_RIGHT_32x32_frame, 130, 130
	jsr SATB_push_sprite

	set_sprite_data jch_RIGHT_32x64_frame, 170, 160
	jsr SATB_push_sprite

	set_sprite_data dr_msl_UP_frame, 205, 130
	jsr SATB_push_sprite

	set_sprite_data jch_min_16x32_0_frame, 205, 125
	jsr SATB_push_sprite

	set_sprite_data jch_min_16x32_1_ref_frame, 237, 125
	jsr SATB_push_sprite

	set_sprite_data dr_msl_UP_16x64_0_frame, 221, 100
	jsr SATB_push_sprite

	set_sprite_data dr_msl_UP_16x64_1_ref_frame, 253, 100
	jsr SATB_push_sprite

	set_sprite_data gigan_idle_LEFT_frame, 180, 260
	jsr SATB_push_sprite

	set_sprite_data tony_idle_RIGHT_frame, 120, 260
	jsr SATB_push_sprite

	; load SATB data to VRAM

	jsr SATB_to_VRAM

	; load VRAM-SATB to the inner SATB

	vdc_reg_data VDC_R13_DVSSR,	#VDC_VRAM_DEFAULT_SAT_ADDR

	; show the sprites

	vdc_cr_set VDC_CR_SHOW_SPR

.loop:
	vsync

	jmp .loop