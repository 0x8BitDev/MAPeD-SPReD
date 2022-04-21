;###################################################################
;
; Copyright 2022 0x8BitDev ( MIT license )
;
;###################################################################
;
; DESC: Simple character controller demo with a big meta-sprite 
;	character and dynamic SG data.
;	Player actions: idle, move, kick
;	Controls: LEFT, RIGHT, UP
;

;DEF_INT_IRQ2		; int_irq2
DEF_INT_IRQ1_VDC	; int_irq1_vdc
;DEF_INT_TIMER		; int_timer
;DEF_INT_NMI		; int_nmi


	.include "../../../common/init.asm"

	.data
	.bank 2
	.org $4000

	.include "player_anm.asm"

player_gfx_SG_arr:
	.word 2944, chr0
	.word 2304, chr1
	.word 2432, chr2
	.word 2944, chr3
	.word 2560, chr4
	.word 2304, chr5
	.word 2560, chr6
	.word 3200, chr7
	.word 2176, chr8
	.word 3200, chr9

chr0:	.incbin "data/player_gfx_chr0.bin"	; 2944 bytes

	.bank 3
	.org $6000

chr1:	.incbin "data/player_gfx_chr1.bin"	; 2304 bytes
chr2:	.incbin "data/player_gfx_chr2.bin"	; 2432 bytes
chr3:	.incbin "data/player_gfx_chr3.bin"	; 2944 bytes

	.bank 4
	.org $8000

chr4:	.incbin "data/player_gfx_chr4.bin"	; 2560 bytes
chr5:	.incbin "data/player_gfx_chr5.bin"	; 2304 bytes
chr6:	.incbin "data/player_gfx_chr6.bin"	; 2560 bytes

	.bank 5
	.org $a000

chr7:	.incbin "data/player_gfx_chr7.bin"	; 3200 bytes
chr9:	.incbin "data/player_gfx_chr9.bin"	; 3200 bytes

	.bank 6
	.org $c000

chr8:	.incbin "data/player_gfx_chr8.bin"	; 2176 bytes

	.code
	.bank 0
	.org $e000

	.include "../../../common/sprite_render.asm"
	.include "../../../common/anm.asm"
	.include "character_cntrl.asm"

main:
	; map data banks

	lda #$02
	tam #02

	lda #$03
	tam #03

	lda #$04
	tam #04

	lda #$05
	tam #05

	lda #$06
	tam #06

	; init the character controller

	jsr chrcntrl_init

	; enable sprites and VBLANK

	vdc_cr_set VDC_CR_SHOW_SPR | VDC_CR_VBLANK

	; enable auto-DMA VRAM->SATB every VBLANK

	vdc_reg_data VDC_R0F_DCR, #VDC_DCR_DSR

	; enable VDC interrupt

	irq_enable IRQ_FLAG_IRQ1

	cli

	SATB_set_state	SATB_FLAG_STATE_FREE

.update_loop:

	; update the character controller

	jsr chrcntrl_update_logic

	; waiting for SG data to be loaded into VRAM on VBLANK

	SATB_get_state
	cmp #SATB_FLAG_STATE_FREE
	beq .cont_update

	SATB_wait_state SATB_FLAG_STATE_FREE

	jmp .update_loop

.cont_update:

	; reset the SATB state

	jsr SATB_reset

	SATB_set_state	SATB_FLAG_STATE_BUSY

	; update the character graphics
	; load the character data to the local SATB

	jsr chrcntrl_update_graphics

	; load the local SATB data to VRAM

	jsr SATB_to_VRAM

	SATB_set_state	SATB_FLAG_STATE_READY

	jmp .update_loop


int_irq1_vdc:

	sta <_vdc_sr
	and #VDC_SR_VBLANK
	bne .vblank

	rts

.vblank:

	SATB_get_state
	cmp #SATB_FLAG_STATE_READY
	bne .vblank_exit

	; load the character's SG data to VRAM to synchronize 
	; it with the inner SATB on VBLANK

	jsr chrcntrl_load_SG_to_VRAM

	SATB_set_state	SATB_FLAG_STATE_FREE

.vblank_exit:

	rts