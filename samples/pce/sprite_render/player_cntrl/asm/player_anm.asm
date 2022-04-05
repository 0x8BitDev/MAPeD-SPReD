;###############################################
;
; Copyright 2022 0x8BitDev ( MIT license )
;
;###############################################

; *** ANIMATIONS DATA ***

anm_player_walk_right:
	.byte 14			; number of ticks to change a frame
	.byte $06			; number of frames
	.byte $00			; loop frame
	.word frames_player_walk_right	; frames data pointer

anm_player_walk_left:
	.byte 14
	.byte $06
	.byte $00
	.word frames_player_walk_left

anm_player_idle_right:
	.byte 10
	.byte $01
	.byte $00
	.word frames_player_idle_right

anm_player_idle_left:
	.byte 10
	.byte $01
	.byte $00
	.word frames_player_idle_left

anm_player_kick_right:
	.byte 18
	.byte $05
	.byte $00
	.word frames_player_kick_right

anm_player_kick_left:
	.byte 18
	.byte $05
	.byte $00
	.word frames_player_kick_left

	.include "data/player_gfx.asm"

frames_player_idle_right	= mpwr_idle01_RIGHT_frame
frames_player_walk_right	= mpwr_walk01_RIGHT_frame
frames_player_kick_right	= mpwr_kick01_RIGHT_frame

frames_player_idle_left		= mpwr_idle01_LEFT_frame
frames_player_walk_left		= mpwr_walk01_LEFT_frame
frames_player_kick_left		= mpwr_kick01_LEFT_frame