;###################################################################
;
; Copyright 2022 0x8BitDev ( MIT license )
;
;###################################################################
;
; Animation routines
;

; animation data structure:
;
;	RAM data:
;	.ds 1	; current tick
;	.ds 1	; current frame
;	ROM data:
;	.ds 1	; number of ticks to change a frame
;	.ds 1	; number of frames
;	.ds 1	; loop frame
;	.ds 2	; frame data pointer
;

; data offsets

ANM_OFFS_RAM_DATA	= 0

ANM_OFFS_CURR_TICK	= 0
ANM_OFFS_CURR_FRAME	= 1

ANM_OFFS_ROM_DATA	= 2

ANM_OFFS_FRAME_TICKS	= 2
ANM_OFFS_FRAMES_CNT	= 3
ANM_OFFS_LOOP_FRAME	= 4
ANM_OFFS_FRAME_PTR	= 5

ANM_ROM_DATA_SIZE	= 5
ANM_RAM_DATA_SIZE	= 7

	.code

; *** init animation ***
; IN:	__ax - RAM anm buffer
;	__bx - ROM anm data structure
;

anm_init:

	cly
	cla
	sta [<__ax], y
	iny
	sta [<__ax], y

	stw <__bx, _bsrci
	stw <__ax, _bdsti

	lda #ANM_OFFS_ROM_DATA
	add_a_to_word _bdsti

	stw #ANM_ROM_DATA_SIZE, _bleni

	jmp _TII

; *** update frame ***
; IN:	__ax - RAM anm buffer
;

anm_update_frame:

	; inc tick

	cly
	lda [<__ax], y
	inc a
	sta [<__ax], y

	; compare tick to change a frame

	ldy #ANM_OFFS_FRAME_TICKS
	cmp [<__ax], y

	beq .update_frame

	rts

.update_frame:

	; reset ticks counter

	cla
	cly
	sta [<__ax], y

	; inc frame

	ldy #ANM_OFFS_CURR_FRAME
	lda [<__ax], y
	inc a
	sta [<__ax], y

	; check frame

	ldy #ANM_OFFS_FRAMES_CNT
	cmp [<__ax], y

	beq .loop_frame

	rts

.loop_frame:

	; reset frames counter

	ldy #ANM_OFFS_LOOP_FRAME
	lda [<__ax], y

	ldy #ANM_OFFS_CURR_FRAME
	sta [<__ax], y

	rts

; *** copy anm frame to SATB ***
; IN:	__ax - RAM anm buffer
;	_spr_pos_x - sprite pos x
;	_spr_pos_y - sprite pos y
;

anm_copy_frame_to_SATB:

	ldy #ANM_OFFS_CURR_FRAME
	lda [<__ax], y

	; calc offset x5 ( 5 - frame data size )

	sta <__bl
	asl a
	asl a
	clc
	adc <__bl
	tax

	ldy #ANM_OFFS_FRAME_PTR
	lda [<__ax], y
	sta <_spr_data
	iny
	lda [<__ax], y
	sta <_spr_data + 1

	txa
	add_a_to_word <_spr_data

	jmp SATB_push_sprite