;###############################################
;
; Copyright 2018-2020 0x8BitDev ( MIT license )
;
;###############################################
;
; Simple character controller
;

.define	_player_move_speed	2
.define	_player_jump_speed	5
.define	_player_jump_height	50

; CHARACTER BIT FLAGS:
; 0-dir(1-right/0-left)
; 1-jump up
; 2-jump down

; constants

.define	_PLAYER_FLAG_DIR_MASK		%00000001
.define	_PLAYER_FLAG_JUMP_UP		%00000010
.define	_PLAYER_FLAG_JUMP_DOWN		%00000100
.define	_PLAYER_FLAG_JUMP_MASK		_PLAYER_FLAG_JUMP_UP|_PLAYER_FLAG_JUMP_DOWN
.define	_PLAYER_FLAG_JUMP_MASK_INV	$ff ~ _PLAYER_FLAG_JUMP_MASK
.define	_PLAYER_FLAG_DIR_MASK_INV	$ff ~ _PLAYER_FLAG_DIR_MASK

.define	_STATE_IDLE		0
.define	_STATE_RUN_LEFT		1
.define	_STATE_RUN_RIGHT	2
.define	_STATE_SHOOT_LEFT	4
.define	_STATE_SHOOT_RIGHT	8
.define	_STATE_DUCK_LEFT	16
.define	_STATE_DUCK_RIGHT	32

.define	_RUN_MASK		JPAD1_LEFT|JPAD1_RIGHT
.define	_SHOOT_MASK		_STATE_SHOOT_LEFT|_STATE_SHOOT_RIGHT
.define	_DUCK_MASK		_STATE_DUCK_LEFT|_STATE_DUCK_RIGHT

.define	PLAYER_FLAG_DIR_RIGHT	1
.define	PLAYER_FLAG_DIR_LEFT	0

; MACROSES

.macro _init_anm

	ld hl, player_anm
	ld de, \1

	call _player_anm_init
.endm

.macro _init_jump_anm

	ld hl, player_anm
	ld de, \1

	ld a, 1		; skip X/Y

	call anm_init
.endm

;*** player data init ***
; IN:
;	B - Y coord
;	C - X coord
;	A - player direction PLAYER_DIR_RIGHT/PLAYER_DIR_LEFT

player_init:

	ld (player_flags), a

	xor a
	ld (player_jump_max_height), a

	ld a,$ff
	ld (player_state), a

	ld hl, player_anm
	ld (hl), b
	inc hl
	ld (hl), c

	jp _set_state_idle

; *** character controller update ***

player_update:

; DUCK

	ld a, (player_state)
	and _DUCK_MASK
	jr z, _cont1			; if( state & _DUCK_MASK != 0 && cntrl_state == CNTRL_DUCK )

	JPAD1_CHECK_DOWN
	jr z, _cont1                    ; if( cntrl_state & CNTRL_DUCK == CNTRL_DUCK ) {\1

	jp _update_player_anm

_cont1:
; DUCK

; SHOOT

	ld a, (player_state)
	and _SHOOT_MASK
	jr z, _cont2			; if( state & _SHOOT_MASK != 0 && cntrl_state == CNTRL_SHOOT )

	JPAD1_CHECK_SW2
	jr z, _cont2                    ; if( cntrl_state & CNTRL_SHOOT == CNTRL_SHOOT ) {\1

	jp _update_player_anm
	
_cont2:
; SHOOT

        ld a, (JPAD_STATE)
	and _RUN_MASK
	jr nz, _player_check_cntrl_left	; if( cntrl_state & RUN_MASK == 0 ) {/1

	ld a, (player_state)
	cp _STATE_IDLE
	jr z, _cont3                   	; if( state != STATE_IDLE ) {/2

	call _set_state_idle
	
_cont3:
	jp  _check_jump

_player_check_cntrl_left:		;/1} else

	JPAD1_CHECK_LEFT		; if( cntrl_state & CNTRL_RUN_LEFT == CNTRL_RUN_LEFT )
	jr z, _player_check_cntrl_right

	ld a, (player_state)
	and _STATE_RUN_LEFT
	jr nz, _set_state_run_left	; if( state != STATE_RUN_LEFT )

	_init_anm player_run_left       ; init_anm( run_left );

_set_state_run_left:

	ld a, _STATE_RUN_LEFT
	ld (player_state), a		; state = STATE_RUN_LEFT

	ld a, (player_flags)
	and _PLAYER_FLAG_DIR_MASK_INV
	ld (player_flags), a            ; player_flags &= ~DIR_RIGHT;

	ld a,(player_anm + 1)		; X
	sub _player_move_speed		; player_pos_x -= move_speed;
	ld (player_anm + 1), a
	
	jp _check_jump

_player_check_cntrl_right:	

	JPAD1_CHECK_RIGHT		; if( cntrl_state & CNTRL_RUN_RIGHT == CNTRL_RUN_RIGHT )
	jr z, _check_jump

	ld a, (player_state)
	and _STATE_RUN_RIGHT
	jr nz, _set_state_run_right	; if( state != STATE_RUN_RIGHT )

	_init_anm player_run_right	; init_anm( run_right );

_set_state_run_right:

	ld a, _STATE_RUN_RIGHT
	ld (player_state), a		; state = STATE_RUN_RIGHT

	ld a, (player_flags)
	or PLAYER_FLAG_DIR_RIGHT
	ld (player_flags), a            ; player_flags |= DIR_RIGHT;

	ld a,(player_anm + 1)		; X
	add _player_move_speed		; player_pos_x += move_speed;
	ld (player_anm + 1), a

_check_jump:
	
	; JUMP
	ld a, (player_flags)
	and _PLAYER_FLAG_JUMP_MASK
	jr z, _check_duck

	jp _update_jump			; if( jump_flag == JUMP_NONE ) {\1
	

; DUCK

_check_duck:

	JPAD1_CHECK_DOWN		; if( cntrl_state & CNTRL_DOWN == CNTRL_DOWN ) {\1
	jr z, _cont_check_shoot

	ld a, (player_state)
	and _DUCK_MASK
	jr nz, _cont_check_shoot        ; if( state & _DUCK_MASK != 0 ) {\2

	ld a, (player_flags)
	and PLAYER_FLAG_DIR_RIGHT
	jr z, _duck_left		; if( player_flags & DIR_RIGHT )

	_init_anm player_duck_right 	; init_anm( duck_right );
	ld a, _STATE_DUCK_RIGHT
	ld (player_state), a		; state = STATE_DUCK_RIGHT

	jp _update_player_anm
	
_duck_left:

	_init_anm player_duck_left	; init_anm( duck_left );
	ld a, _STATE_DUCK_LEFT
	ld (player_state), a		; state = STATE_DUCK_LEFT

	jp _update_player_anm

	;\2}
	;\1}

_cont_check_shoot:

; DUCK
	
; SHOOT	

	JPAD1_CHECK_SW2
	jr z, _cont_check_jump		; if( cntrl_state & CNTRL_SHOOT == CNTRL_SHOOT ) {\1

	ld a, (player_state)
	and _SHOOT_MASK
	jr nz, _cont_check_jump		; if( state & _SHOOT_MASK != 0 ) {\2

	ld a, (player_flags)
	and PLAYER_FLAG_DIR_RIGHT
	jr z, _shoot_left               ; if( player_flags & DIR_RIGHT )

	_init_anm player_shoot_right 	; init_anm( shoot_right );
	ld a, _STATE_SHOOT_RIGHT
	ld (player_state), a            ; state = STATE_SHOOT_RIGHT

	jp _update_player_anm

_shoot_left:
	
	_init_anm player_shoot_left 	; init_anm( shoot_left );
	ld a, _STATE_SHOOT_LEFT
	ld (player_state), a            ; state = STATE_SHOOT_LEFT
	
	jp _update_player_anm

	;\2}
	;\1}

_cont_check_jump:

; SHOOT

	JPAD1_CHECK_SW1
	jr z, _update_jump		; if( cntrl_state & CNTRL_JUMP == true ) {\2

	ld a, (player_flags)
	and _PLAYER_FLAG_JUMP_MASK_INV  ; player_flags &= ~JUMP_MASK;
	or _PLAYER_FLAG_JUMP_UP
	ld (player_flags), a            ; player_flags |= JUMP_UP;

	ld a,(player_anm)		; A <- anm_Y
	sub _player_jump_height
	ld (player_jump_max_height), a	; player_jump_max_height = player_pos_y - jump_height;	

	;/1}
	;/2}

_update_jump:

	ld a, (player_flags)
	ld c, a
	and _PLAYER_FLAG_JUMP_MASK
	jr z, _update_player_anm	; if( jump_flag == JUMP_NONE ) {\1

	ld a, PLAYER_FLAG_DIR_RIGHT
	and c                           ; if( player_flags & DIR_RIGHT )
	jr z, _set_jump_left

	_init_jump_anm player_jump_right ; init_anm( jump_right );

	jp _jump_continue

_set_jump_left:

	_init_jump_anm player_jump_left	; else init_anm( jump_left );

_jump_continue:

	ld a, (player_flags)
	and _PLAYER_FLAG_JUMP_UP
	jr z, _jump_down                ; if( jump_flag == JUMP_UP )

	ld a, (player_jump_max_height)
	ld c, a

	ld a, (player_anm)		; A <- anm_Y
	sub _player_jump_speed
	ld (player_anm), a		; player_pos_y -= jump_delta;

	sub c
	jr nc, _update_player_anm       ; if( player_pos_y >= player_jump_max_height )

	ld a, (player_jump_max_height)
	ld (player_anm), a		; player_pos_y = player_jump_max_height;

	ld a, (player_flags)
	and _PLAYER_FLAG_JUMP_MASK_INV	; player_flags &= ~JUMP_MASK;
	or _PLAYER_FLAG_JUMP_DOWN       ; player_flags |= JUMP_DOWN;
	ld (player_flags), a

	jp _update_player_anm	

_jump_down:

	ld a, (player_anm)
	add _player_jump_speed 
	ld (player_anm), a		; player_pos_y += jump_delta;
	ld c, a

	ld a, (player_jump_max_height)
	add _player_jump_height		; var ground_pos = jump_max_height + jump_height;
	
	ld b, a

	sub c
	jr nc, _update_player_anm

	ld a, b
	ld (player_anm), a		; player_pos_y = ground_pos;

	ld a, (player_flags)
	and _PLAYER_FLAG_JUMP_MASK_INV	; player_flags &= ~JUMP_MASK;
	ld (player_flags), a

	call _set_state_idle_forced	; set IDLE state to have a continuous jump with a landing phase

_update_player_anm:

	ld hl, player_anm
	call anm_update

	ret

;*** set new animation if player isn't jumping ***

_player_anm_init:

	ld a, (player_flags)	
	and _PLAYER_FLAG_JUMP_MASK
	jp nz, _player_ignore_anm
	

	ld a, 1				; skip X/Y

	jp anm_init

_player_ignore_anm:
	ret

;*** set the STATE_IDLE ***

_set_state_idle:

	ld a, (player_state)
	cp _STATE_IDLE
	jr z, _set_state_idle_exit

_set_state_idle_forced:

	ld a, _STATE_IDLE
	ld (player_state), a

	ld a, (player_flags)
	and PLAYER_FLAG_DIR_RIGHT
	jr z, __init_idle_left

	_init_anm player_idle_right	;init_anm( idle_right );

_set_state_idle_exit:
	ret

__init_idle_left:

	_init_anm player_idle_left	;init_anm( idle_left );

	ret