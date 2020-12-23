;###############################################
;
; Copyright 2018-2019 0x8BitDev ( MIT license )
;
;###############################################


	.incdir "data"
	.include "marco_gfx.asm"


chr0:	.incbin "marco_gfx_chr0.bin"	; 512 bytes
chr1:	.incbin "marco_gfx_chr1.bin"	; 512 bytes
chr2:	.incbin "marco_gfx_chr2.bin"	; 512 bytes
chr3:	.incbin "marco_gfx_chr3.bin"	; 576 bytes
chr4:	.incbin "marco_gfx_chr4.bin"	; 544 bytes
chr5:	.incbin "marco_gfx_chr5.bin"	; 384 bytes
chr6:	.incbin "marco_gfx_chr6.bin"	; 384 bytes
chr7:	.incbin "marco_gfx_chr7.bin"	; 384 bytes
chr8:	.incbin "marco_gfx_chr8.bin"	; 384 bytes
chr9:	.incbin "marco_gfx_chr9.bin"	; 544 bytes
chr10:	.incbin "marco_gfx_chr10.bin"	; 544 bytes
chr11:	.incbin "marco_gfx_chr11.bin"	; 576 bytes
chr12:	.incbin "marco_gfx_chr12.bin"	; 512 bytes
chr13:	.incbin "marco_gfx_chr13.bin"	; 576 bytes
chr14:	.incbin "marco_gfx_chr14.bin"	; 512 bytes
chr15:	.incbin "marco_gfx_chr15.bin"	; 672 bytes
chr16:	.incbin "marco_gfx_chr16.bin"	; 736 bytes
chr17:	.incbin "marco_gfx_chr17.bin"	; 576 bytes
chr18:	.incbin "marco_gfx_chr18.bin"	; 576 bytes
chr19:	.incbin "marco_gfx_chr19.bin"	; 512 bytes
chr20:	.incbin "marco_gfx_chr20.bin"	; 544 bytes
chr21:	.incbin "marco_gfx_chr21.bin"	; 544 bytes
chr22:	.incbin "marco_gfx_chr22.bin"	; 512 bytes
chr23:	.incbin "marco_gfx_chr23.bin"	; 512 bytes
chr24:	.incbin "marco_gfx_chr24.bin"	; 512 bytes
chr25:	.incbin "marco_gfx_chr25.bin"	; 512 bytes
chr26:	.incbin "marco_gfx_chr26.bin"	; 512 bytes
chr27:	.incbin "marco_gfx_chr27.bin"	; 512 bytes
chr28:	.incbin "marco_gfx_chr28.bin"	; 576 bytes
chr29:	.incbin "marco_gfx_chr29.bin"	; 544 bytes
chr30:	.incbin "marco_gfx_chr30.bin"	; 384 bytes
chr31:	.incbin "marco_gfx_chr31.bin"	; 384 bytes
chr32:	.incbin "marco_gfx_chr32.bin"	; 384 bytes
chr33:	.incbin "marco_gfx_chr33.bin"	; 384 bytes
chr34:	.incbin "marco_gfx_chr34.bin"	; 384 bytes
chr35:	.incbin "marco_gfx_chr35.bin"	; 384 bytes
chr36:	.incbin "marco_gfx_chr36.bin"	; 384 bytes
chr37:	.incbin "marco_gfx_chr37.bin"	; 384 bytes
chr38:	.incbin "marco_gfx_chr38.bin"	; 512 bytes
chr39:	.incbin "marco_gfx_chr39.bin"	; 544 bytes
chr40:	.incbin "marco_gfx_chr40.bin"	; 544 bytes
chr41:	.incbin "marco_gfx_chr41.bin"	; 576 bytes
chr42:	.incbin "marco_gfx_chr42.bin"	; 544 bytes
chr43:	.incbin "marco_gfx_chr43.bin"	; 512 bytes
chr44:	.incbin "marco_gfx_chr44.bin"	; 544 bytes
chr45:	.incbin "marco_gfx_chr45.bin"	; 576 bytes
chr46:	.incbin "marco_gfx_chr46.bin"	; 512 bytes
chr47:	.incbin "marco_gfx_chr47.bin"	; 672 bytes
chr48:	.incbin "marco_gfx_chr48.bin"	; 736 bytes
chr49:	.incbin "marco_gfx_chr49.bin"	; 576 bytes
chr50:	.incbin "marco_gfx_chr50.bin"	; 576 bytes

player_tiles_arr:	
	.word 512, chr0
	.word 512, chr1
	.word 512, chr2
	.word 576, chr3
	.word 544, chr4
	.word 384, chr5
	.word 384, chr6
	.word 384, chr7
	.word 384, chr8
	.word 544, chr9
	.word 544, chr10
	.word 576, chr11
	.word 512, chr12
	.word 576, chr13
	.word 512, chr14
	.word 672, chr15
	.word 736, chr16
	.word 576, chr17
	.word 576, chr18
	.word 512, chr19
	.word 544, chr20
	.word 544, chr21
	.word 512, chr22
	.word 512, chr23
	.word 512, chr24
	.word 512, chr25
	.word 512, chr26
	.word 512, chr27
	.word 576, chr28
	.word 544, chr29
	.word 384, chr30
	.word 384, chr31
	.word 384, chr32
	.word 384, chr33
	.word 384, chr34
	.word 384, chr35
	.word 384, chr36
	.word 384, chr37
	.word 512, chr38
	.word 544, chr39
	.word 544, chr40
	.word 576, chr41
	.word 544, chr42
	.word 512, chr43
	.word 544, chr44
	.word 576, chr45
	.word 512, chr46
	.word 672, chr47
	.word 736, chr48
	.word 576, chr49
	.word 576, chr50


; *** ANIMATIONS DATA ***

player_run_right:
	.byte 4				; number of ticks to change a frame
	.byte $09			; number of frames
	.byte $02			; loop frame
	.word marco_run01_RIGHT_frame	; frame data pointer

player_run_left:
	.byte 4
	.byte $09
	.byte $02
	.word marco_run01_LEFT_frame

player_idle_right:
	.byte 10
	.byte $06
	.byte $00
	.word marco_idle01_RIGHT_frame

player_idle_left:
	.byte 10
	.byte $06
	.byte $00
	.word marco_idle01_LEFT_frame

player_jump_right:
	.byte 1
	.byte $01
	.byte $00
	.word marco_run08_RIGHT_frame

player_jump_left:
	.byte 1
	.byte $01
	.byte $00
	.word marco_run08_LEFT_frame

player_shoot_right:
	.byte 4
	.byte $04
	.byte $00
	.word marco_shoot01_RIGHT_frame

player_shoot_left:
	.byte 4
	.byte $04
	.byte $00
	.word marco_shoot01_LEFT_frame

player_duck_right:
	.byte 5
	.byte $0a
	.byte $03
	.word marco_duck01_RIGHT_frame

player_duck_left:
	.byte 5
	.byte $0a
	.byte $03
	.word marco_duck01_LEFT_frame