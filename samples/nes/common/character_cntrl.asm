;###############################################
;
; Copyright 2018-2019 0x8BitDev ( MIT license )
;
;###############################################
;
; Simple character controller
;

.segment "ZP"

; character controller variables
_player_pos_x:		.res 1	
_player_pos_y:		.res 1	

_player_jump_max_height:
			.res 1

; CHARACTER BIT FLAGS:
; 0-dir(1-right/0-left)
; 1-jump up
; 2-jump down

_player_flags:		.res 1

_player_anm:		.res .sizeof( runtime_anm_data )

_state:			.res 1


.segment "CODE"

_player_move_speed:	.byte 2
_player_jump_speed:	.byte 5
_player_jump_height:	.byte 50

; constants
_PLAYER_FLAG_DIR_MASK		= %00000001
_PLAYER_FLAG_JUMP_UP		= %00000010
_PLAYER_FLAG_JUMP_DOWN		= %00000100
_PLAYER_FLAG_JUMP_MASK		= _PLAYER_FLAG_JUMP_UP|_PLAYER_FLAG_JUMP_DOWN
_PLAYER_FLAG_JUMP_MASK_INV	= ~_PLAYER_FLAG_JUMP_MASK
_PLAYER_FLAG_DIR_MASK_INV	= ~_PLAYER_FLAG_DIR_MASK

_STATE_IDLE		= 0
_STATE_RUN_LEFT		= 1
_STATE_RUN_RIGHT	= 2
_STATE_SHOOT_LEFT	= 4
_STATE_SHOOT_RIGHT	= 8
_STATE_DUCK_LEFT	= 16
_STATE_DUCK_RIGHT	= 32

_RUN_MASK		= JPAD_BTN_LEFT|JPAD_BTN_RIGHT
_SHOOT_MASK		= _STATE_SHOOT_LEFT|_STATE_SHOOT_RIGHT
_DUCK_MASK		= _STATE_DUCK_LEFT|_STATE_DUCK_RIGHT

PLAYER_FLAG_DIR_RIGHT	= 1
PLAYER_FLAG_DIR_LEFT	= 0

; MACROSES
	.macro _init_anm anm
	load_data_ptr _player_anm, data_addr

	ldx #<anm
	ldy #>anm
	jsr _player_anm_init
	.endmacro

	.macro _init_jump_anm anm
	load_data_ptr _player_anm, data_addr

	ldx #<anm
	ldy #>anm

	jsr anm_init
	.endmacro

;*** player data init ***
; IN:
;	X - X coord
;	Y - Y coord
;	A - player direction PLAYER_DIR_RIGHT/PLAYER_DIR_LEFT

player_init:
	sta _player_flags

	; player coordinates init
	stx _player_pos_x
	sty _player_pos_y

	lda #$00
	sta _player_jump_max_height

	lda #$ff
	sta _state

	jmp _set_state_idle		; set the STATE_IDLE with animation
					; depending on a player direction

; *** character controller update ***
; OUT: 	
;	inner_vars::tmp_anm_addr - current animation frame address

player_update:

; DUCK
	lda _state
	and #<_DUCK_MASK
	beq _cont1			; if( state & _DUCK_MASK != 0 && cntrl_state == CNTRL_DUCK )

	jpad1_check_btn JPAD_BTN_DOWN	; if( cntrl_state & CNTRL_DUCK == CNTRL_DUCK ) {\1
	beq _cont1

	jmp _update_player_anm

_cont1:
; DUCK

; SHOOT
	lda _state
	and #<_SHOOT_MASK
	beq _cont2			; if( state & _SHOOT_MASK != 0 && cntrl_state == CNTRL_SHOOT )

	jpad1_check_btn JPAD_BTN_B	; if( cntrl_state & CNTRL_SHOOT == CNTRL_SHOOT ) {\1
	beq _cont2

	jmp _update_player_anm

_cont2:
; SHOOT

	jpad1_check_btn _RUN_MASK
	bne _player_check_cntrl_left 	; if( cntrl_state & RUN_MASK == 0 ) {/1

	lda #<_STATE_IDLE
	bit _state
	bne _cont3			; if( state != STATE_IDLE ) {/2

	jsr _set_state_idle
		
_cont3:
	jmp _check_jump

_player_check_cntrl_left:		;/1} else

	jpad1_check_btn JPAD_BTN_LEFT	; if( cntrl_state & CNTRL_RUN_LEFT == CNTRL_RUN_LEFT )
	beq _player_check_cntrl_right

	lda #<_STATE_RUN_LEFT
	bit _state
	bne _set_state_run_left		; if( state != STATE_RUN_LEFT )

	_init_anm player_run_left	; init_anm( run_left );

_set_state_run_left:

	lda #<_STATE_RUN_LEFT
	sta _state			; state = STATE_RUN_LEFT

	lda #<_PLAYER_FLAG_DIR_MASK_INV
	and _player_flags
	sta _player_flags		; player_flags &= ~DIR_RIGHT;

	sec
	lda _player_pos_x
	sbc _player_move_speed
	sta _player_pos_x		; player_pos_x -= move_speed;

	jmp _check_jump

_player_check_cntrl_right:	

	jpad1_check_btn JPAD_BTN_RIGHT	; if( cntrl_state & CNTRL_RUN_RIGHT == CNTRL_RUN_RIGHT )
	beq _check_jump

	lda #<_STATE_RUN_RIGHT
	bit _state
	bne _set_state_run_right	; if( state != STATE_RUN_RIGHT )

	_init_anm player_run_right	; init_anm( run_right );

_set_state_run_right:

	lda #<_STATE_RUN_RIGHT
	sta _state			; state = STATE_RUN_RIGHT

	lda #<PLAYER_FLAG_DIR_RIGHT
	ora _player_flags
	sta _player_flags		; player_flags |= DIR_RIGHT;

	clc
	lda _player_pos_x
	adc _player_move_speed
	sta _player_pos_x		; player_pos_x += move_speed;

_check_jump:
	
	; JUMP
	lda #<_PLAYER_FLAG_JUMP_MASK
	bit _player_flags
	beq _check_duck
	
	jmp _update_jump		; if( jump_flag == JUMP_NONE ) {\1

; DUCK

_check_duck:

	jpad1_check_btn JPAD_BTN_DOWN	; if( cntrl_state & CNTRL_DOWN == CNTRL_DOWN ) {\1
	beq _cont_check_shoot

	lda _state
	and #<_DUCK_MASK
	bne _cont_check_shoot		; if( state & _DUCK_MASK != 0 ) {\2

	lda #<PLAYER_FLAG_DIR_RIGHT
	bit _player_flags
	beq _duck_left			; if( player_flags & DIR_RIGHT )

	_init_anm player_duck_right 	; init_anm( duck_right );
	lda #<_STATE_DUCK_RIGHT
	sta _state			; state = STATE_DUCK_RIGHT

	jmp _update_player_anm

_duck_left:

	_init_anm player_duck_left 	; init_anm( duck_left );
	lda #<_STATE_DUCK_LEFT
	sta _state			; state = STATE_DUCK_LEFT

	jmp _update_player_anm

	;\2}
	;\1}

_cont_check_shoot:

; DUCK
	
; SHOOT	

	jpad1_check_btn JPAD_BTN_B	; if( cntrl_state & CNTRL_SHOOT == CNTRL_SHOOT ) {\1
	beq _cont_check_jump

	lda _state
	and #<_SHOOT_MASK
	bne _cont_check_jump		; if( state & _SHOOT_MASK != 0 ) {\2

	lda #<PLAYER_FLAG_DIR_RIGHT
	bit _player_flags
	beq _shoot_left			; if( player_flags & DIR_RIGHT )

	_init_anm player_shoot_right 	; init_anm( shoot_right );
	lda #<_STATE_SHOOT_RIGHT
	sta _state			; state = STATE_SHOOT_RIGHT

	jmp _update_player_anm

_shoot_left:

	_init_anm player_shoot_left 	; init_anm( shoot_left );
	lda #<_STATE_SHOOT_LEFT
	sta _state			; state = STATE_SHOOT_LEFT

	jmp _update_player_anm

	;\2}
	;\1}

_cont_check_jump:

; SHOOT

	jpad1_check_btn JPAD_BTN_A
	beq _update_jump		; if( cntrl_state & CNTRL_JUMP == true ) {\2

	lda #<_PLAYER_FLAG_JUMP_MASK_INV
	and _player_flags
	sta _player_flags		; player_flags &= ~JUMP_MASK_INV;
	lda #<_PLAYER_FLAG_JUMP_UP		
	ora _player_flags
	sta _player_flags		; player_flags |= JUMP_UP;

	sec
	lda _player_pos_y
	sbc _player_jump_height
	sta _player_jump_max_height	; player_jump_max_height = player_pos_y - jump_height;

	;/1}
	;/2}

_update_jump:

	lda #<_PLAYER_FLAG_JUMP_MASK
	bit _player_flags
	beq _update_player_anm		; if( jump_flag == JUMP_NONE ) {\1

	lda #<PLAYER_FLAG_DIR_RIGHT
	bit _player_flags		; if( player_flags & DIR_RIGHT )
	beq _set_jump_left				

	_init_jump_anm player_jump_right ; init_anm( jump_right );

	jmp _jump_continue

_set_jump_left:

	_init_jump_anm player_jump_left	; else init_anm( jump_left );

_jump_continue:

	lda #<_PLAYER_FLAG_JUMP_UP
	bit _player_flags
	beq _jump_down			; if( jump_flag == JUMP_UP )

	sec
	lda _player_pos_y
	sbc _player_jump_speed
	sta _player_pos_y		; player_pos_y -= jump_delta;

	cmp _player_jump_max_height	; if( player_pos_y >= player_jump_max_height )
	bpl _update_player_anm

	lda _player_jump_max_height
	sta _player_pos_y		; player_pos_y = player_jump_max_height;

	lda #<_PLAYER_FLAG_JUMP_MASK_INV
	and _player_flags		; player_flags &= ~JUMP_MASK_INV;
	sta _player_flags
	lda #<_PLAYER_FLAG_JUMP_DOWN
	ora _player_flags
	sta _player_flags		; player_flags |= JUMP_DOWN;

	jmp _update_player_anm

_jump_down:

	clc
	lda _player_pos_y
	adc _player_jump_speed
	sta _player_pos_y		; player_pos_y += jump_delta;

	clc
	lda _player_jump_height
	adc _player_jump_max_height	; var ground_pos = jump_max_height + jump_height;

	cmp _player_pos_y
	bpl _update_player_anm

	sta _player_pos_y		; player_pos_y = ground_pos;

	lda #<_PLAYER_FLAG_JUMP_MASK_INV
	and _player_flags				
	sta _player_flags		; player_flags &= ~JUMP_MASK_INV;

	jsr _set_state_idle_forced	; set IDLE state to have a continuous jump with a landing phase

_update_player_anm:

	ldx #<_player_anm
	ldy #>_player_anm

	lda _player_pos_x
	sta anm_pos_x
	lda _player_pos_y
	sta anm_pos_y

	jsr anm_update
	; inner_vars::tmp_anm_addr - current animation frame address

	rts

;*** set new animation if player isn't jumping ***

_player_anm_init:
	
	lda #<_PLAYER_FLAG_JUMP_MASK
	bit _player_flags
	bne _player_ignore_anm

	jmp anm_init

_player_ignore_anm:
	rts

;*** set the STATE_IDLE ***

_set_state_idle:

	lda _state
	cmp #<_STATE_IDLE
	beq _set_state_idle_exit

_set_state_idle_forced:
	lda #<_STATE_IDLE
	sta _state

	lda #<PLAYER_FLAG_DIR_RIGHT
	bit _player_flags
	beq __init_idle_left		;if( player_flags & DIR_RIGHT )

	_init_anm player_idle_right	;init_anm( idle_right );

_set_state_idle_exit:
	rts

__init_idle_left:

	_init_anm player_idle_left	;init_anm( idle_left );

	rts	