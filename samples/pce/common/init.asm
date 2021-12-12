;###################################################################
;
; Copyright 2021 0x8BitDev ( MIT license )
;
;###################################################################
;
; The PCE hardware init entry point
;

	.include "..\..\..\common\def.asm"
	.include "..\..\..\common\macro.asm"

	.zp

_vdc_reg:	.ds 1 ; last register
_vdc_sr:	.ds 1 ;	last status register data
_vdc_cr:	.ds 2 ; last control register data
_irq_flags:	.ds 1 ; last interrupt control flags

_data:		.ds 2

_frame_cnt:	.ds 1

_jpad_arr:
_jpad5:		.ds 1
_jpad4:		.ds 1
_jpad3:		.ds 1
_jpad2:		.ds 1
_jpad1:		.ds 1

	.bss

; fast data transfer

_TIA:	.ds 1	; $e3 tia
_bsrc:	.ds 2
_bdst:	.ds 2
_blen:	.ds 2
_tiarts	.ds 1	; $60 rts

	.code
	.bank 0
	.org $fd00

	.include "..\..\..\common\vdc.asm"
	.include "..\..\..\common\jpad.asm"

reset:
	; On reset:
	; Interrupt is disabled
	; Decimal flag is reset
	; The timer is stopped
	; The interrupt disable register is all reset.
	; Low speed mode is set.

	sei				; just in case disable all interrupts
	csh				; switch CPU to 7.16 MHz mode

	lda #$ff			; map hardware bank ( I/O ports )
	tam #00
	tax				; A(#$ff) -> X

	lda #$f8			; map RAM 8K
	tam #01

	txs				; setup stack at $21ff ( X(#$ff -> SP )

	; clear RAM

      	lda #$00
      	sta $2000
      	tii $2000,$2001,$1fff

	; disable interrupts

	irq_disable ( IRQ_FLAG_IRQ2 | IRQ_FLAG_IRQ1 | IRQ_FLAG_TIMER )

	; init VDC

	vdc_reg_data VDC_R05_CR,	$0000
	vdc_reg_data VDC_R06_RCR,	$0000
	vdc_reg_data VDC_R07_BXR,	$0000
	vdc_reg_data VDC_R08_BYR,	$0000
	vdc_reg_data VDC_R09_MWR,	#VDC_MWR_BAT32x32
	vdc_reg_data VDC_R0A_HSR,	#VDC_HSR( $02, $02 )
	vdc_reg_data VDC_R0B_HDR,	#VDC_HDR( $04, $1f )
	vdc_reg_data VDC_R0C_VPR,	#VDC_VPR( $17, $02 )
	vdc_reg_data VDC_R0D_VDR,	$00df
	vdc_reg_data VDC_R0E_VCR,	$000c
	vdc_reg_data VDC_R0F_DCR,	$0000
	vdc_reg_data VDC_R13_DVSSR,	$7f00	

	; init TIA

	lda #$e3
	sta _TIA
	lda #$60
	sta _tiarts

	jmp main

irq2:
IFDEF	DEF_INT_IRQ2
	pha
	phx
	phy

	jsr int_irq2

	ply
	plx
	pla
ENDIF
	rti

irq1_vdc:
	pha
	phx
	phy

	lda VDC_REG			; to avoid infinite IRQ1 call loop
	sta <_vdc_sr			; save status reg data

	bbr5 <_vdc_sr, .cont		; 5 bit - VBLANK

	dec <_frame_cnt

.cont:
IFDEF	DEF_INT_IRQ1_VDC
	jsr int_irq1_vdc
ENDIF

	lda <_vdc_reg
	sta VDC_REG

	ply
	plx
	pla
	rti

timer:
IFDEF	DEF_INT_TIMER
	pha
	phx
	phy

	sta IRQ_STATUS			; writing any value clears the interrupt state of the TIMER.
	stz TIMER_CNTRL			; turn off timer

	jsr int_timer

	ply
	plx
	pla
ENDIF
	rti

nmi:
IFDEF	DEF_INT_NMI
	pha
	phx
	phy

	jsr int_NMI

	ply
	plx
	pla
ENDIF
	rti

	.org $fff6
	.dw irq2	; software BRK
	.dw irq1_vdc
	.dw timer
	.dw nmi
	.dw reset