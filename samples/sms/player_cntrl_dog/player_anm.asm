;########################################################################
;
; Copyright 2019-2020 0x8BitDev ( MIT license )
;
;########################################################################


	.incdir "data"
	.include "dog_gfx.asm"


player_tiles_0:
	.incbin "dog_gfx_chr0.bin"
player_tiles_1:
	.incbin "dog_gfx_chr1.bin"
player_tiles_2:
	.incbin "dog_gfx_chr2.bin"
player_tiles_3:
	.incbin "dog_gfx_chr3.bin"
player_tiles_4:
	.incbin "dog_gfx_chr4.bin"
player_tiles_5:
	.incbin "dog_gfx_chr5.bin"
player_tiles_6:
	.incbin "dog_gfx_chr6.bin"
player_tiles_7:	
	.incbin "dog_gfx_chr7.bin"

player_tiles_arr:
	.word 384, player_tiles_0, 
	.word 384, player_tiles_1,
	.word 384, player_tiles_2,
	.word 384, player_tiles_3,
	.word 384, player_tiles_4,
	.word 384, player_tiles_5,
	.word 384, player_tiles_6,
	.word 384, player_tiles_7


; *** ANIMATIONS DATA ***

player_run_right:
	.byte 8				; number of ticks to change a frame
	.byte $02			; number of frames
	.byte $00			; loop frame
	.word dog01_RUN01_RIGHT_frame	; frame data pointer

player_run_left:
	.byte 8
	.byte $02
	.byte $00
	.word dog01_RUN01_LEFT_frame

player_idle_right:
player_duck_right:
player_shoot_right:
	.byte 10
	.byte $02
	.byte $00
	.word dog01_IDLE01_RIGHT_frame

player_idle_left:
player_duck_left:
player_shoot_left:
	.byte 10
	.byte $02
	.byte $00
	.word dog01_IDLE01_LEFT_frame

player_jump_right:
	.byte 1
	.byte $01
	.byte $00
	.word dog01_JUMP_RIGHT_frame

player_jump_left:
	.byte 1
	.byte $01
	.byte $00
	.word dog01_JUMP_LEFT_frame
