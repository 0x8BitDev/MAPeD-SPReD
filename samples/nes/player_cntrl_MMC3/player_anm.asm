;###############################################
;
; Copyright 2018-2019 0x8BitDev ( MIT license )
;
;###############################################

; *** ANIMATIONS DATA ***

player_run_right:
	.byte 4				; number of ticks to change a frame
	.byte $09			; number of frames
	.byte $02			; loop frame
	.word frames_player_run_right	; frame data pointer

player_run_left:
	.byte 4
	.byte $09
	.byte $02
	.word frames_player_run_left

player_idle_right:
	.byte 10
	.byte $06
	.byte $00
	.word frames_player_idle_right

player_idle_left:
	.byte 10
	.byte $06
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

player_shoot_right:
	.byte 4
	.byte $04
	.byte $00
	.word frames_player_shoot_right

player_shoot_left:
	.byte 4
	.byte $04
	.byte $00
	.word frames_player_shoot_left

player_duck_right:
	.byte 5
	.byte $0a
	.byte $03
	.word frames_player_duck_right

player_duck_left:
	.byte 5
	.byte $0a
	.byte $03
	.word frames_player_duck_left

	.include "data/marco_gfx.asm"

frames_player_idle_right	= marco_idle01_RIGHT_frame
frames_player_duck_right	= marco_duck01_RIGHT_frame
frames_player_run_right		= marco_run01_RIGHT_frame
frames_player_jump_right	= marco_run08_RIGHT_frame
frames_player_shoot_right	= marco_shoot01_RIGHT_frame

frames_player_idle_left		= marco_idle01_LEFT_frame
frames_player_duck_left		= marco_duck01_LEFT_frame
frames_player_run_left		= marco_run01_LEFT_frame
frames_player_jump_left		= marco_run08_LEFT_frame
frames_player_shoot_left	= marco_shoot01_LEFT_frame