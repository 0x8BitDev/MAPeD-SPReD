;###############################################
;
; Copyright 2018-2019 0x8BitDev ( MIT license )
;
;###############################################


	.incdir "data"
	.include "marco_gfx.asm"


chr0:	.incbin "marco_gfx_chr0.bin"	; 544 bytes
chr1:	.incbin "marco_gfx_chr1.bin"	; 544 bytes
chr2:	.incbin "marco_gfx_chr2.bin"	; 544 bytes
chr3:	.incbin "marco_gfx_chr3.bin"	; 608 bytes
chr4:	.incbin "marco_gfx_chr4.bin"	; 544 bytes
chr5:	.incbin "marco_gfx_chr5.bin"	; 416 bytes
chr6:	.incbin "marco_gfx_chr6.bin"	; 416 bytes
chr7:	.incbin "marco_gfx_chr7.bin"	; 416 bytes
chr8:	.incbin "marco_gfx_chr8.bin"	; 416 bytes
chr9:	.incbin "marco_gfx_chr9.bin"	; 512 bytes
chr10:	.incbin "marco_gfx_chr10.bin"	; 608 bytes
chr11:	.incbin "marco_gfx_chr11.bin"	; 544 bytes
chr12:	.incbin "marco_gfx_chr12.bin"	; 608 bytes
chr13:	.incbin "marco_gfx_chr13.bin"	; 544 bytes
chr14:	.incbin "marco_gfx_chr14.bin"	; 512 bytes
chr15:	.incbin "marco_gfx_chr15.bin"	; 544 bytes
chr16:	.incbin "marco_gfx_chr16.bin"	; 704 bytes
chr17:	.incbin "marco_gfx_chr17.bin"	; 544 bytes
chr18:	.incbin "marco_gfx_chr18.bin"	; 704 bytes
chr19:	.incbin "marco_gfx_chr19.bin"	; 768 bytes
chr20:	.incbin "marco_gfx_chr20.bin"	; 640 bytes
chr21:	.incbin "marco_gfx_chr21.bin"	; 608 bytes
chr22:	.incbin "marco_gfx_chr22.bin"	; 544 bytes
chr23:	.incbin "marco_gfx_chr23.bin"	; 544 bytes
chr24:	.incbin "marco_gfx_chr24.bin"	; 544 bytes
chr25:	.incbin "marco_gfx_chr25.bin"	; 544 bytes
chr26:	.incbin "marco_gfx_chr26.bin"	; 544 bytes
chr27:	.incbin "marco_gfx_chr27.bin"	; 544 bytes
chr28:	.incbin "marco_gfx_chr28.bin"	; 608 bytes
chr29:	.incbin "marco_gfx_chr29.bin"	; 544 bytes
chr30:	.incbin "marco_gfx_chr30.bin"	; 416 bytes
chr31:	.incbin "marco_gfx_chr31.bin"	; 416 bytes
chr32:	.incbin "marco_gfx_chr32.bin"	; 416 bytes
chr33:	.incbin "marco_gfx_chr33.bin"	; 416 bytes
chr34:	.incbin "marco_gfx_chr34.bin"	; 416 bytes
chr35:	.incbin "marco_gfx_chr35.bin"	; 416 bytes
chr36:	.incbin "marco_gfx_chr36.bin"	; 416 bytes
chr37:	.incbin "marco_gfx_chr37.bin"	; 416 bytes
chr38:	.incbin "marco_gfx_chr38.bin"	; 512 bytes
chr39:	.incbin "marco_gfx_chr39.bin"	; 608 bytes
chr40:	.incbin "marco_gfx_chr40.bin"	; 544 bytes
chr41:	.incbin "marco_gfx_chr41.bin"	; 608 bytes
chr42:	.incbin "marco_gfx_chr42.bin"	; 544 bytes
chr43:	.incbin "marco_gfx_chr43.bin"	; 512 bytes
chr44:	.incbin "marco_gfx_chr44.bin"	; 544 bytes
chr45:	.incbin "marco_gfx_chr45.bin"	; 704 bytes
chr46:	.incbin "marco_gfx_chr46.bin"	; 544 bytes
chr47:	.incbin "marco_gfx_chr47.bin"	; 704 bytes
chr48:	.incbin "marco_gfx_chr48.bin"	; 768 bytes
chr49:	.incbin "marco_gfx_chr49.bin"	; 640 bytes
chr50:	.incbin "marco_gfx_chr50.bin"	; 608 bytes

player_tiles_arr:	
	.word 544, chr0
	.word 544, chr1
	.word 544, chr2
	.word 608, chr3
	.word 544, chr4
	.word 416, chr5
	.word 416, chr6
	.word 416, chr7
	.word 416, chr8
	.word 512, chr9
	.word 608, chr10
	.word 544, chr11
	.word 608, chr12
	.word 544, chr13
	.word 512, chr14
	.word 544, chr15
	.word 704, chr16
	.word 544, chr17
	.word 704, chr18
	.word 768, chr19
	.word 640, chr20
	.word 608, chr21
	.word 544, chr22
	.word 544, chr23
	.word 544, chr24
	.word 544, chr25
	.word 544, chr26
	.word 544, chr27
	.word 608, chr28
	.word 544, chr29
	.word 416, chr30
	.word 416, chr31
	.word 416, chr32
	.word 416, chr33
	.word 416, chr34
	.word 416, chr35
	.word 416, chr36
	.word 416, chr37
	.word 512, chr38
	.word 608, chr39
	.word 544, chr40
	.word 608, chr41
	.word 544, chr42
	.word 512, chr43
	.word 544, chr44
	.word 704, chr45
	.word 544, chr46
	.word 704, chr47
	.word 768, chr48
	.word 640, chr49
	.word 608, chr50


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