;###################################################################
;
; Copyright 2022 0x8BitDev ( MIT license )
;
;###################################################################
;
; VDC related routines and macroses
;

	.macro vdc_reg
	lda #\1
	sta <_vdc_reg
	sta VDC_REG
	.endm

	.macro vdc_reg_data	; \1 - reg N, \2 - 16 bit value
	vdc_reg \1

.if ( \?2 = 6 ) ; ARG_LABEL
	lda \2
	sta VDC_DATA_L

	lda \2 + 1
	sta VDC_DATA_H
.else
	lda #low( \2 )
	sta VDC_DATA_L

	lda #high( \2 )
	sta VDC_DATA_H
.endif
	.endm

	.macro load_data_to_VRAM	; \1 - src addr, \2 - dst addr, \3 - data len
	stw #\1, _bsrc
	stw #\2, _bdst
	stw #\3, _blen

	jsr vdc_copy_to_VRAM
	.endm

	.macro load_palette		; \1 - src addr, \2 - dst addr, \3 - data len
	stw #VCE_WRITE_DATA, 	_bdst
	stw #\1,		_bsrc
	stw #\2, 		VCE_ADDR
	stw #\3, 		_blen

	jsr _TIA
	.endm

	.macro vdc_cr_set	; \1 - data 16 bit
	lda low_byte #\1
	ora low_byte _vdc_cr
	sta low_byte _vdc_cr

	lda high_byte #\1
	ora high_byte _vdc_cr
	sta high_byte _vdc_cr
	
	vdc_reg_data VDC_R05_CR, _vdc_cr
	.endm

	.macro vdc_cr_res	; \1 - data 16 bit
	lda low_byte #~\1
	and low_byte _vdc_cr
	sta low_byte _vdc_cr

	lda high_byte #~\1
	and high_byte _vdc_cr
	sta high_byte _vdc_cr
	
	vdc_reg_data VDC_R05_CR, _vdc_cr
	.endm
	
	.macro vdc_sr_wait_set
.wait_loop\@:
	lda VDC_REG
	and #\1
	beq .wait_loop\@
	.endm

	.macro vdc_sr_wait_res
.wait_loop\@:
	lda VDC_REG
	and #\1
	bne .wait_loop\@
	.endm

	.macro vsync
.if ( \# > 0 )
	lda #\1
.else
	lda #$01
.endif
	sta <_frame_cnt

.loop\@:
	lda <_frame_cnt
	bne .loop\@
	.endm


; --- set border color ---
;
; IN: 	_ax - 16 bit color index
;

vce_border_color:

	; set data addr

	stw #$0100, VCE_ADDR

	; set color	

	stw <__ax, VCE_WRITE_DATA

	rts

; --- copy data to VRAM ---
;
; IN: 	_bdst - VRAM address
;	_bsrc - local memory address
;	_blen - number of bytes to copy
;

vdc_copy_to_VRAM:

	vdc_reg_data 	VDC_R00_MAWR,	_bdst
	vdc_reg		VDC_R02_VWR
	
	stw #VDC_DATA, 	_bdst

	jmp _TIA