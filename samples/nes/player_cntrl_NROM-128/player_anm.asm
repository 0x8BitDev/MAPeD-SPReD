;###############################################
;
; Copyright 2018-2019 0x8BitDev ( MIT license )
;
;###############################################

; *** ANIMATIONS DATA ***

player_run_right:
	.byte 8				; number of ticks to change the frame
	.byte $02			; number of frames
	.byte $00			; loop frame
	.word frames_player_run_right	; frame data pointer

player_run_left:
	.byte 8
	.byte $02
	.byte $00
	.word frames_player_run_left

player_idle_right:
player_duck_right:
player_shoot_right:
	.byte 10
	.byte $02
	.byte $00
	.word frames_player_idle_right

player_idle_left:
player_duck_left:
player_shoot_left:
	.byte 10
	.byte $02
	.byte $00
	.word frames_player_idle_left

player_jump_right:
	.byte 1
	.byte $01
	.byte $00
	.word frames_player_jump_right

player_jump_left:
	.byte 1
	.byte $01
	.byte $00
	.word frames_player_jump_left

	.include "data/dog_gfx.asm"

frames_player_idle_right	= dog01_IDLE01_RIGHT_frame
frames_player_duck_right	= dog01_IDLE01_RIGHT_frame
frames_player_shoot_right	= dog01_IDLE01_RIGHT_frame
frames_player_run_right		= dog01_RUN01_RIGHT_frame
frames_player_jump_right	= dog01_JUMP_RIGHT_frame

frames_player_idle_left		= dog01_IDLE01_LEFT_frame
frames_player_duck_left		= dog01_IDLE01_LEFT_frame
frames_player_shoot_left	= dog01_IDLE01_LEFT_frame
frames_player_run_left		= dog01_RUN01_LEFT_frame
frames_player_jump_left		= dog01_JUMP_LEFT_frame
