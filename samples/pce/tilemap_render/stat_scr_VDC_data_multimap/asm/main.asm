;###################################################################
;
; Copyright 2022 0x8BitDev ( MIT license )
;
;###################################################################
;
; DESC: Simple example that shows VDC-ready screen data
;

;DEF_INT_IRQ2		; int_irq2
;DEF_INT_IRQ1_VDC	; int_irq1_vdc
;DEF_INT_TIMER		; int_timer
;DEF_INT_NMI		; int_nmi


	.include "../../../common/asm/init.asm"

	.zp

	.data
	.bank 2
	.org $4000
	
	.include "tilemap.asm"

	.data
	.bank 3
	.org $6000

chr0:	.incbin "tilemap_chr0.bin"

data_end:

	.code
	.bank 0
	.org $e000

	.include "../../../common/asm/tilemap_render.asm"

.if	MAP_FLAG_OFF( MAP_FLAG_MODE_STATIC_SCREENS )
	fail 'Only MAP_FLAG_MODE_STATIC_SCREENS is supported by this sample !'
.endif

.if	SCR_BLOCKS2x2_WIDTH != 16 | SCR_BLOCKS2x2_HEIGHT != 14
	fail 'The input screen size must be 256x224 !'
.endif

	.code

main:
        ; map data bank
	
	lda #$02
	tam #02

.if	( bank( data_end ) - 2 ) > 0
	lda #$03
	tam #03
.if	( bank( data_end ) - 2 - 1 ) > 0
	lda #$04
	tam #04
.if	( bank( data_end ) - 2 - 2 ) > 0
	lda #$05
	tam #05
.if	( bank( data_end ) - 2 - 3 ) > 0
	lda #$06
	tam #06
.endif
.endif
.endif
.endif
	
	; init data

	stw Lev0_StartScr,		tr_curr_scr
	stw #tilemap_Plts,		tr_plts_arr
	stw #tilemap_VDCScr,		tr_VDC_scr_data
	stw #tilemap_CHRs_size,		tr_CHRs_size_arr
	stw #tilemap_CHRs,		tr_CHRs_arr
	stw #( CHRS_OFFSET << 5 ),	tr_CHR_VRAM_addr

	jsr tr_init

	cli

	; show background

	vdc_cr_set VDC_CR_SHOW_BACKGR

.loop:
	jmp .loop