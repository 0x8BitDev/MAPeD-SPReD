;###############################################
;
; Copyright 2018-2022 0x8BitDev ( MIT license )
;
;###############################################
;
; Animation functions
; LIMITATION: max 32 frames in animation
; 

.struct runtime_anm_data
	curr_tick	.byte	; 0
	num_ticks	.byte	; 1 num ticks to change a frame
	curr_frame	.byte	; 2
	num_frames	.byte	; 3 num frames to loop an animation
	loop_frame	.byte	; 4
	frames_ptr	.word	; 5
.endstruct

.struct static_anm_data
	num_ticks	.byte
	num_frames	.byte
	loop_frame	.byte
	frames_ptr	.word
.endstruct

.struct frame_data
	gfx_ptr		.word
.endstruct

.struct attr_data
	data_size	.byte
	chr_id		.byte
.endstruct

.segment "ZP"

anm_pos_x:		.res 1
anm_pos_y:		.res 1

ppu_sprite_CHR_bank:	.res 1

	.scope inner_vars
tick_frame:		.res 1
tmp_anm_addr: 		.res 2
	.endscope


.segment "CODE"

; *** animation data init ***
; IN:
;	X - LOW byte of anm data
;	Y - HIGH byte of anm data
;	data_addr - address at which animation will be initialized

anm_init:
	stx inner_vars::tmp_anm_addr
	sty inner_vars::tmp_anm_addr + 1

	lda #$00

	ldy #runtime_anm_data::curr_tick
	sta (<data_addr), y

	ldy #runtime_anm_data::curr_frame
	sta (<data_addr), y

	ldy #static_anm_data::num_ticks
	lda (<inner_vars::tmp_anm_addr), y

	ldy #runtime_anm_data::num_ticks
	sta (<data_addr), y

	ldy #static_anm_data::num_frames
	lda (<inner_vars::tmp_anm_addr), y
	ldy #runtime_anm_data::num_frames
	sta (<data_addr), y

	ldy #static_anm_data::loop_frame
	lda (<inner_vars::tmp_anm_addr), y
	ldy #runtime_anm_data::loop_frame
	sta (<data_addr), y

	ldy #static_anm_data::frames_ptr
	lda (<inner_vars::tmp_anm_addr), y
	ldy #runtime_anm_data::frames_ptr
	sta (<data_addr), y

	ldy #static_anm_data::frames_ptr + 1
	lda (<inner_vars::tmp_anm_addr), y
	ldy #runtime_anm_data::frames_ptr + 1
	sta (<data_addr), y

	rts

; *** update animation frame and load address of a current frame into the inner_vars::tmp_anm_addr ***
; IN:
;	X - LOW byte of anm addr
;	Y - HIGH byte of anm addr
; OUT:
;	inner_vars::tmp_anm_addr - current animation frame address

anm_update:
	jsr _anm_get_updated_frame_ind

	; A - current anm frame
	; A *= 2 ( .sizeof( frame_data ) )
	clc
	rol a

	; add offset to frames start address
	ldy #runtime_anm_data::frames_ptr
	adc (<inner_vars::tmp_anm_addr), y
	pha
	iny
	lda #$00
	adc (<inner_vars::tmp_anm_addr), y

	; save the result to the inner_vars::tmp_anm_addr
	sta inner_vars::tmp_anm_addr + 1
	pla
	sta inner_vars::tmp_anm_addr

	rts

; *** get index of a current updated frame ***
; IN:
;	X - LOW byte of anm addr
;	Y - HIGH byte of anm addr
; OUT:
;	A - current frame index

_anm_get_updated_frame_ind:
	; save animation address
	stx inner_vars::tmp_anm_addr
	sty inner_vars::tmp_anm_addr + 1

	ldy #runtime_anm_data::curr_tick
	lda (<inner_vars::tmp_anm_addr), y	; A - current tick
	tax
	inx					; inc tick counter
	stx inner_vars::tick_frame		; save to the temp var

	ldy #runtime_anm_data::num_ticks	
	lda (<inner_vars::tmp_anm_addr), y	; A - number of ticks to change the frame

	cmp inner_vars::tick_frame		; tick - A
	beq _switch_frame			; if _tick == delay_ticks -> go to the next frame

	ldy #runtime_anm_data::curr_tick
	lda inner_vars::tick_frame
	sta (<inner_vars::tmp_anm_addr), y	; save new tick value

	ldy #runtime_anm_data::curr_frame
	lda (<inner_vars::tmp_anm_addr), y	; get current frame value

	rts

_switch_frame:
	ldy #runtime_anm_data::curr_tick
	lda #$00
	sta (<inner_vars::tmp_anm_addr), y	; save the new tick value

	;iny
	ldy #runtime_anm_data::curr_frame
	lda (<inner_vars::tmp_anm_addr), y	; get current frame value
	tax
	inx
	stx inner_vars::tick_frame		; save new frame value

	ldy #runtime_anm_data::num_frames
	lda (<inner_vars::tmp_anm_addr), y	; get the MAX number of animation frames
	cmp inner_vars::tick_frame
	bne _save_next_frame

	ldy #runtime_anm_data::loop_frame	; begin animation from loop frame
	lda (<inner_vars::tmp_anm_addr), y

	ldy #runtime_anm_data::curr_frame
	sta (<inner_vars::tmp_anm_addr), y

	rts

_save_next_frame:
	ldy #runtime_anm_data::curr_frame
	lda inner_vars::tick_frame
	sta (<inner_vars::tmp_anm_addr), y

	rts	