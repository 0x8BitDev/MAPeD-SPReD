;###################################################################
;
; Copyright 2022 0x8BitDev ( MIT license )
;
;###################################################################
;
; DESC: Simple character controller: idle, move, kick
;	Controls: LEFT, RIGHT, UP
;

; macroses

	.macro player_set_dir
	player_get_state
	ora #\1
	sta player_state
	.endm

	.macro player_set_state
	lda player_state
	and #PLAYER_DIR_MASK
	ora #\1
	sta player_state
	.endm

	.macro player_get_state
	lda player_state
	and #~PLAYER_DIR_MASK
	.endm

	.bss

; *** character controller variables ***

; player's animation description

player_anm:	.ds ANM_RAM_DATA_SIZE

; player's CHR data

player_CHR_src:	.ds 2
player_CHR_dst:	.ds 2
player_CHR_len:	.ds 2

player_pos_x:	.ds 2
player_pos_y:	.ds 2

player_state:	.ds 1

; player state constants

PLAYER_STATE_IDLE	= 1
PLAYER_STATE_WALK	= 2
PLAYER_STATE_KICK	= 3

PLAYER_MAX_STATES	= 3		; idle, walk, kick

PLAYER_DIR_MASK		= %10000000
PLAYER_DIR_RIGHT	= %10000000
PLAYER_DIR_LEFT		= %00000000

; character's initial parameters

PLAYER_INIT_POS_X	= 150
PLAYER_INIT_POS_Y	= 240
PLAYER_INIT_DIR		= PLAYER_DIR_RIGHT
PLAYER_MOVE_STEP	= 1

	.data

player_anm_tbl:
	.word anm_player_idle_left
	.word anm_player_walk_left
	.word anm_player_kick_left

	.word anm_player_idle_right
	.word anm_player_walk_right
	.word anm_player_kick_right

	.code

chrcntrl_init:

	; ignore CHR data loading to VRAM more than once on anm_copy_frame_to_SATB
	; and enable delayed loading of CHR data to VRAM

	SATB_set_flags SATB_FLAG_CHECK_CHR_BANK | SATB_FLAG_PEND_CHR_DATA

	; set CHR data parameters for delayed use (SATB_FLAG_PEND_CHR_DATA)

	stw #player_CHR_src,	_CHR_DATA_SRC
	stw #player_CHR_dst,	_CHR_DATA_DST
	stw #player_CHR_len,	_CHR_DATA_LEN

	; load sprites palette

	load_palette	player_gfx_palette, $100 + ( PLAYER_GFX_PALETTE_SLOT << 4 ), ( player_gfx_palette_end - player_gfx_palette )

	; set pointer to CHRs data array

	stw #player_gfx_CHR_arr,	<_CHR_data_arr

	; set sprites VADDR

	stw #PLAYER_GFX_SPR_VADDR,	<_spr_VRAM

	; init player state

	stw #PLAYER_INIT_POS_X, player_pos_x
	stw #PLAYER_INIT_POS_Y, player_pos_y

	stz player_state

	player_set_dir PLAYER_INIT_DIR
	player_set_state PLAYER_STATE_IDLE

	jmp __chrctrl_apply_state

; *** update character controller logic ***
;

chrcntrl_update_logic:

	jsr jpad_read

	; check KICK button

	jpad1_check_btn	JPAD_BTN_UP
	beq .check_move

	player_get_state
	cmp #PLAYER_STATE_KICK
	bne .set_kick_state

	jmp .update_anm					; skip init of the same state

.set_kick_state:

	; set 'kick' state

	player_set_state PLAYER_STATE_KICK

	jsr __chrctrl_apply_state

	jmp .update_anm

.check_move:

	; check RIGHT button

	jpad1_check_btn	JPAD_BTN_RIGHT
	beq .check_walk_left

	; move right

	lda #PLAYER_MOVE_STEP
	add_a_to_word player_pos_x

	; check the right border of the screen

	lda player_pos_x
	bne .cont_check_walk_right

	stw #$ff, player_pos_x

.cont_check_walk_right:

	lda player_state
	cmp #( PLAYER_DIR_RIGHT | PLAYER_STATE_WALK )
	beq .check_walk_left				; skip init of the same state

	; set 'walking right' state

	player_set_dir PLAYER_DIR_RIGHT
	player_set_state PLAYER_STATE_WALK

	jsr __chrctrl_apply_state

.check_walk_left:

	; check LEFT button

	jpad1_check_btn	JPAD_BTN_LEFT

	beq .cont

	; move left

	lda #PLAYER_MOVE_STEP
	sub_a_from_word player_pos_x

	; check the left border of the screen

	lda player_pos_x
	cmp #64
	bne .cont_check_walk_left

	stw #(64 + PLAYER_MOVE_STEP), player_pos_x

.cont_check_walk_left:

	lda player_state
	cmp #( PLAYER_DIR_LEFT | PLAYER_STATE_WALK )
	beq .cont					; skip init of the same state

	; set 'walking left' state

	player_set_dir PLAYER_DIR_LEFT
	player_set_state PLAYER_STATE_WALK

	jsr __chrctrl_apply_state

.cont:

	jpad1_check_btn	( JPAD_BTN_LEFT | JPAD_BTN_RIGHT )
	bne .update_anm

	; set 'idle' state if no buttons were pressed

	player_set_state PLAYER_STATE_IDLE

	jsr __chrctrl_apply_state

.update_anm:

	; update animation frame index

	stw #player_anm,	<__ax
	jmp anm_update_frame


; *** init animation by state flags ***
;

__chrctrl_apply_state:

	lda player_state
	tax
	and #PLAYER_DIR_MASK
	bne .right_dir

	; left dir
	; anm_ind = player_state - 1

	txa
	dec a

	jmp .cont

.right_dir:

	; anm_ind = player_state - PLAYER_DIR_RIGHT + ( PLAYER_MAX_STATES - 1 )

	txa
	sec
	sbc #PLAYER_DIR_MASK
	clc
	adc #( PLAYER_MAX_STATES - 1 )

.cont:

	asl a
	tay

	stw #player_anm_tbl, 	<__cx

	lda [<__cx], y
	sta <__bl
	iny
	lda [<__cx], y
	sta <__bh

	stw #player_anm,	<__ax

	jmp anm_init


chrcntrl_update_graphics:

	; copy animation frame to the local SATB

	stw #player_anm,	<__ax
	stw player_pos_x,	<_spr_pos_x
	stw player_pos_y,	<_spr_pos_y

	jmp anm_copy_frame_to_SATB


chrcntrl_load_CHR_to_VRAM:

	; copy CHR data to VRAM

	stw player_CHR_src, _bsrc
	stw player_CHR_dst, _bdst
	stw player_CHR_len, _blen

	jmp vdc_copy_to_VRAM