;###################################################################
;
; Copyright 2021 0x8BitDev ( MIT license )
;
;###################################################################
;
; Tilemap renderer routines
;

MAP_FLAG_ON	.func ( MAP_DATA_MAGIC & \1 ) = \1
MAP_FLAG_OFF	.func ( MAP_DATA_MAGIC & \1 ) = 0

.if	MAP_FLAG_ON( MAP_FLAG_DIR_ROWS )
	fail 'The sample supports column ordered data only!'
.endif

.if	MAP_FLAG_ON( MAP_FLAG_MODE_STATIC_SCREENS )

.if	BAT_INDEX != 0
	fail 'The sample supports BAT 32x32 only!'
.endif

VDC_SCR_DATA
.endif

	.zp

; temporary values
;
tmp_val1:
	.ds 1
tmp_val2:
	.ds 1
tmp_val3:
	.ds 1
tmp_val4:
	.ds 1

; tilemap renderer data
;
tr_curr_scr:	
	.ds 2	; current screen

tr_CHRs_size_arr:
	.ds 2	; CHR data size array

tr_CHRs_arr:
	.ds 2	; CHRs data array

	.bss

tr_plts_arr:
	.ds 2	; palettes array

tr_VDC_scr_data:
	.ds 2	; VDC ready screens data

tr_CHR_VRAM_addr:
	.ds 2

	.code


tr_init:

	jmp _tr_draw_screen

;--- draw screen
; IN: tr_curr_scr
;
_tr_draw_screen:

.if	VDC_SCR_DATA

	ldy #$00
	lda [ tr_curr_scr ], y	; A - chr id

	pha

	; load CHRs to VRAM

	asl a
	pha
	tay

	lda [ tr_CHRs_arr ], y
	sta _bsrc
	iny
	lda [ tr_CHRs_arr ], y
	sta _bsrc + 1

	stw tr_CHR_VRAM_addr, _bdst

	pla
	tay

	lda [ tr_CHRs_size_arr ], y
	sta _blen
	iny
	lda [ tr_CHRs_size_arr ], y
	sta _blen + 1

	jsr vdc_copy_to_VRAM	

	; load palette

	pla

	asl a			; high byte = chr id << 1; low byte = 0; ( 512 * chr id )
	tay
	clx

	add_word_to_xy tr_plts_arr

	stw #VCE_WRITE_DATA, 	_bdst

	stx _bsrc
	sty _bsrc + 1

	stw #$0000,	VCE_ADDR
	stw #$200,	_blen

	jsr _TIA
	
	; load BAT

	ldy #$01
	lda [ tr_curr_scr ], y	; VDC scr data offset low byte
	tax
	iny
	lda [ tr_curr_scr ], y	; VDC scr data offset high byte
	tay

	add_word_to_xy tr_VDC_scr_data

	stx _bsrc
	sty _bsrc + 1

	stw $0000, _bdst
	stw #ScrGfxDataSize, _blen	

	jsr vdc_copy_to_VRAM	

.endif
	rts