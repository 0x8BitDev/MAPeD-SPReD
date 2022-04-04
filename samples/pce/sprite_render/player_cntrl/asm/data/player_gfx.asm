;#######################################################
;
; Generated by SPReD-PCE Copyright 2017-2022 0x8BitDev
;
;#######################################################

PLAYER_GFX_SPR_VADDR	= 8192
PLAYER_GFX_PALETTE_SLOT	= 0


;chr0:	.incbin "player_gfx_chr0.bin"	; 2944 bytes
;chr1:	.incbin "player_gfx_chr1.bin"	; 2304 bytes
;chr2:	.incbin "player_gfx_chr2.bin"	; 2432 bytes
;chr3:	.incbin "player_gfx_chr3.bin"	; 2944 bytes
;chr4:	.incbin "player_gfx_chr4.bin"	; 2560 bytes
;chr5:	.incbin "player_gfx_chr5.bin"	; 2304 bytes
;chr6:	.incbin "player_gfx_chr6.bin"	; 2560 bytes
;chr7:	.incbin "player_gfx_chr7.bin"	; 3200 bytes
;chr8:	.incbin "player_gfx_chr8.bin"	; 2176 bytes
;chr9:	.incbin "player_gfx_chr9.bin"	; 3200 bytes

;player_gfx_CHR_arr:	
;	.word 2944, chr0
;	.word 2304, chr1
;	.word 2432, chr2
;	.word 2944, chr3
;	.word 2560, chr4
;	.word 2304, chr5
;	.word 2560, chr6
;	.word 3200, chr7
;	.word 2176, chr8
;	.word 3200, chr9


player_gfx_palette:
	.word $147, $C2, $81, $14B, $19D, $1B0, $170, $E9, $173, $F0, $1B5, $B0, $58, $133, $F2, $1B6
player_gfx_palette_end:

player_gfx_num_frames:
	.byte $18
player_gfx_frames_data:
muscle_power_idle01_RIGHT_frame:
	.word muscle_power_idle01_RIGHT
	.word muscle_power_idle01_RIGHT_end - muscle_power_idle01_RIGHT	; data size
	.byte 6		; CHR bank index (chr6)
muscle_power_walk01_RIGHT_frame:
	.word muscle_power_walk01_RIGHT
	.word muscle_power_walk01_RIGHT_end - muscle_power_walk01_RIGHT
	.byte 0
muscle_power_walk02_RIGHT_frame:
	.word muscle_power_walk02_RIGHT
	.word muscle_power_walk02_RIGHT_end - muscle_power_walk02_RIGHT
	.byte 1
muscle_power_walk03_RIGHT_frame:
	.word muscle_power_walk03_RIGHT
	.word muscle_power_walk03_RIGHT_end - muscle_power_walk03_RIGHT
	.byte 2
muscle_power_walk04_RIGHT_frame:
	.word muscle_power_walk04_RIGHT
	.word muscle_power_walk04_RIGHT_end - muscle_power_walk04_RIGHT
	.byte 3
muscle_power_walk05_RIGHT_frame:
	.word muscle_power_walk05_RIGHT
	.word muscle_power_walk05_RIGHT_end - muscle_power_walk05_RIGHT
	.byte 4
muscle_power_walk06_RIGHT_frame:
	.word muscle_power_walk06_RIGHT
	.word muscle_power_walk06_RIGHT_end - muscle_power_walk06_RIGHT
	.byte 5
muscle_power_kick01_RIGHT_frame:
	.word muscle_power_kick01_RIGHT
	.word muscle_power_kick01_RIGHT_end - muscle_power_kick01_RIGHT
	.byte 7
muscle_power_kick02_RIGHT_frame:
	.word muscle_power_kick02_RIGHT
	.word muscle_power_kick02_RIGHT_end - muscle_power_kick02_RIGHT
	.byte 8
muscle_power_kick03_RIGHT_frame:
	.word muscle_power_kick03_RIGHT
	.word muscle_power_kick03_RIGHT_end - muscle_power_kick03_RIGHT
	.byte 9
muscle_power_kick04_ref02_RIGHT_frame:
	.word muscle_power_kick04_ref02_RIGHT
	.word muscle_power_kick04_ref02_RIGHT_end - muscle_power_kick04_ref02_RIGHT
	.byte 8
muscle_power_kick05_ref01_RIGHT_frame:
	.word muscle_power_kick05_ref01_RIGHT
	.word muscle_power_kick05_ref01_RIGHT_end - muscle_power_kick05_ref01_RIGHT
	.byte 7
muscle_power_idle01_LEFT_frame:
	.word muscle_power_idle01_LEFT
	.word muscle_power_idle01_LEFT_end - muscle_power_idle01_LEFT
	.byte 6
muscle_power_walk01_LEFT_frame:
	.word muscle_power_walk01_LEFT
	.word muscle_power_walk01_LEFT_end - muscle_power_walk01_LEFT
	.byte 0
muscle_power_walk02_LEFT_frame:
	.word muscle_power_walk02_LEFT
	.word muscle_power_walk02_LEFT_end - muscle_power_walk02_LEFT
	.byte 1
muscle_power_walk03_LEFT_frame:
	.word muscle_power_walk03_LEFT
	.word muscle_power_walk03_LEFT_end - muscle_power_walk03_LEFT
	.byte 2
muscle_power_walk04_LEFT_frame:
	.word muscle_power_walk04_LEFT
	.word muscle_power_walk04_LEFT_end - muscle_power_walk04_LEFT
	.byte 3
muscle_power_walk05_LEFT_frame:
	.word muscle_power_walk05_LEFT
	.word muscle_power_walk05_LEFT_end - muscle_power_walk05_LEFT
	.byte 4
muscle_power_walk06_LEFT_frame:
	.word muscle_power_walk06_LEFT
	.word muscle_power_walk06_LEFT_end - muscle_power_walk06_LEFT
	.byte 5
muscle_power_kick01_LEFT_frame:
	.word muscle_power_kick01_LEFT
	.word muscle_power_kick01_LEFT_end - muscle_power_kick01_LEFT
	.byte 7
muscle_power_kick02_LEFT_frame:
	.word muscle_power_kick02_LEFT
	.word muscle_power_kick02_LEFT_end - muscle_power_kick02_LEFT
	.byte 8
muscle_power_kick03_LEFT_frame:
	.word muscle_power_kick03_LEFT
	.word muscle_power_kick03_LEFT_end - muscle_power_kick03_LEFT
	.byte 9
muscle_power_kick04_ref02_LEFT_frame:
	.word muscle_power_kick04_ref02_LEFT
	.word muscle_power_kick04_ref02_LEFT_end - muscle_power_kick04_ref02_LEFT
	.byte 8
muscle_power_kick05_ref01_LEFT_frame:
	.word muscle_power_kick05_ref01_LEFT
	.word muscle_power_kick05_ref01_LEFT_end - muscle_power_kick05_ref01_LEFT
	.byte 7


	; #1: Y pos, #2: X pos, #3: CHR index, #4: CHR desc

muscle_power_idle01_RIGHT:
	.word $FF9E, $0B, $100, $880
	.word $FF9E, $FFFB, $102, $880
	.word $FF9E, $FFEB, $104, $880
	.word $FFAE, $0F, $106, $880
	.word $FFAE, $FFFF, $108, $880
	.word $FFAE, $FFEF, $10A, $880
	.word $FFBE, $0C, $10C, $880
	.word $FFBE, $FFFC, $10E, $880
	.word $FFBE, $FFEC, $110, $880
	.word $FFCE, $0B, $112, $880
	.word $FFCE, $FFFB, $114, $880
	.word $FFCE, $FFEB, $116, $880
	.word $FFDE, $0B, $118, $880
	.word $FFDE, $FFFB, $11A, $880
	.word $FFDE, $FFEB, $11C, $880
	.word $FFEE, $0B, $11E, $880
	.word $FFEE, $FFFB, $120, $880
	.word $FFEE, $FFEB, $122, $880
	.word $FFEE, $FFDB, $124, $880
	.word $FFFE, $FFE4, $126, $880
muscle_power_idle01_RIGHT_end:

muscle_power_walk01_RIGHT:
	.word $FF97, $03, $100, $880
	.word $FF97, $FFF3, $102, $880
	.word $FF97, $FFE3, $104, $880
	.word $FFA7, $18, $106, $880
	.word $FFA7, $08, $108, $880
	.word $FFA7, $FFF8, $10A, $880
	.word $FFA7, $FFE8, $10C, $880
	.word $FFA7, $FFD8, $10E, $880
	.word $FFB7, $17, $110, $880
	.word $FFB7, $07, $112, $880
	.word $FFB7, $FFF7, $114, $880
	.word $FFB7, $FFE7, $116, $880
	.word $FFC7, $FFFD, $118, $880
	.word $FFC7, $FFED, $11A, $880
	.word $FFD7, $01, $11C, $880
	.word $FFD7, $FFF1, $11E, $880
	.word $FFD7, $FFE1, $120, $880
	.word $FFE7, $FFFF, $122, $880
	.word $FFE7, $FFE9, $124, $880
	.word $FFE7, $FFD9, $126, $880
	.word $FFF7, $05, $128, $880
	.word $FFF7, $FFF5, $12A, $880
	.word $FFF7, $FFDC, $12C, $880
muscle_power_walk01_RIGHT_end:

muscle_power_walk02_RIGHT:
	.word $FF96, $02, $100, $880
	.word $FF96, $FFF2, $102, $880
	.word $FFA6, $12, $104, $880
	.word $FFA6, $02, $106, $880
	.word $FFA6, $FFF2, $108, $880
	.word $FFB6, $FFFF, $10A, $880
	.word $FFB6, $FFEF, $10C, $880
	.word $FFC6, $FFF9, $10E, $880
	.word $FFC6, $FFE9, $110, $880
	.word $FFD6, $FFFC, $112, $880
	.word $FFD6, $FFEC, $114, $880
	.word $FFD6, $FFDC, $116, $880
	.word $FFE6, $FFFA, $118, $880
	.word $FFE6, $FFEA, $11A, $880
	.word $FFE6, $FFDA, $11C, $880
	.word $FFF6, $FFFE, $11E, $880
	.word $FFF6, $FFEE, $120, $880
	.word $FFF6, $FFDE, $122, $880
muscle_power_walk02_RIGHT_end:

muscle_power_walk03_RIGHT:
	.word $FF93, $02, $100, $880
	.word $FF93, $FFF2, $102, $880
	.word $FFA3, $10, $104, $880
	.word $FFA3, $00, $106, $880
	.word $FFA3, $FFF0, $108, $880
	.word $FFB3, $02, $10A, $880
	.word $FFB3, $FFF2, $10C, $880
	.word $FFB3, $FFE2, $10E, $880
	.word $FFC3, $0A, $110, $880
	.word $FFC3, $FFFA, $112, $880
	.word $FFC3, $FFEA, $114, $880
	.word $FFD3, $0A, $116, $880
	.word $FFD3, $FFFA, $118, $880
	.word $FFD3, $FFEA, $11A, $880
	.word $FFE3, $06, $11C, $880
	.word $FFE3, $FFF6, $11E, $880
	.word $FFE3, $FFE6, $120, $880
	.word $FFF3, $FFE7, $122, $880
	.word $FFF3, $FFD7, $124, $880
muscle_power_walk03_RIGHT_end:

muscle_power_walk04_RIGHT:
	.word $FF97, $02, $100, $880
	.word $FF97, $FFF2, $102, $880
	.word $FFA7, $12, $104, $880
	.word $FFA7, $02, $106, $880
	.word $FFA7, $FFF2, $108, $880
	.word $FFA7, $FFE2, $10A, $880
	.word $FFB7, $0C, $10C, $880
	.word $FFB7, $FFFC, $10E, $880
	.word $FFB7, $FFEC, $110, $880
	.word $FFC7, $06, $112, $880
	.word $FFC7, $FFF6, $114, $880
	.word $FFC7, $FFE6, $116, $880
	.word $FFD7, $09, $118, $880
	.word $FFD7, $FFF9, $11A, $880
	.word $FFD7, $FFE9, $11C, $880
	.word $FFD7, $FFD9, $11E, $880
	.word $FFE7, $0D, $120, $880
	.word $FFE7, $FFFD, $122, $880
	.word $FFE7, $FFE9, $124, $880
	.word $FFE7, $FFD9, $126, $880
	.word $FFF7, $0D, $128, $880
	.word $FFF7, $FFFD, $12A, $880
	.word $FFF7, $FFD7, $12C, $880
muscle_power_walk04_RIGHT_end:

muscle_power_walk05_RIGHT:
	.word $FF9A, $02, $100, $880
	.word $FF9A, $FFF2, $102, $880
	.word $FFAA, $0E, $104, $880
	.word $FFAA, $FFFE, $106, $880
	.word $FFAA, $FFEE, $108, $880
	.word $FFBA, $02, $10A, $880
	.word $FFBA, $FFF2, $10C, $880
	.word $FFBA, $FFE2, $10E, $880
	.word $FFCA, $02, $110, $880
	.word $FFCA, $FFF2, $112, $880
	.word $FFCA, $FFE2, $114, $880
	.word $FFDA, $03, $116, $880
	.word $FFDA, $FFF3, $118, $880
	.word $FFDA, $FFE3, $11A, $880
	.word $FFEA, $01, $11C, $880
	.word $FFEA, $FFF1, $11E, $880
	.word $FFEA, $FFE1, $120, $880
	.word $FFFA, $02, $122, $880
	.word $FFFA, $FFF2, $124, $880
	.word $FFFA, $FFDA, $126, $880
muscle_power_walk05_RIGHT_end:

muscle_power_walk06_RIGHT:
	.word $FF98, $02, $100, $880
	.word $FF98, $FFF2, $102, $880
	.word $FFA8, $12, $104, $880
	.word $FFA8, $02, $106, $880
	.word $FFA8, $FFF2, $108, $880
	.word $FFA8, $FFE2, $10A, $880
	.word $FFB8, $00, $10C, $880
	.word $FFB8, $FFF0, $10E, $880
	.word $FFB8, $FFE0, $110, $880
	.word $FFC8, $02, $112, $880
	.word $FFC8, $FFF2, $114, $880
	.word $FFC8, $FFE2, $116, $880
	.word $FFD8, $02, $118, $880
	.word $FFD8, $FFF2, $11A, $880
	.word $FFE8, $FFFE, $11C, $880
	.word $FFE8, $FFEE, $11E, $880
	.word $FFF8, $FFF0, $120, $880
	.word $FFF8, $FFE0, $122, $880
muscle_power_walk06_RIGHT_end:

muscle_power_kick01_RIGHT:
	.word $FF98, $15, $100, $880
	.word $FF98, $05, $102, $880
	.word $FF98, $FFF5, $104, $880
	.word $FF98, $FFE5, $106, $880
	.word $FFA8, $18, $108, $880
	.word $FFA8, $08, $10A, $880
	.word $FFA8, $FFF8, $10C, $880
	.word $FFA8, $FFE8, $10E, $880
	.word $FFB8, $17, $110, $880
	.word $FFB8, $06, $112, $880
	.word $FFB8, $FFF6, $114, $880
	.word $FFC8, $0C, $116, $880
	.word $FFC8, $FFFC, $118, $880
	.word $FFC8, $FFEC, $11A, $880
	.word $FFD8, $0F, $11C, $880
	.word $FFD8, $FFFF, $11E, $880
	.word $FFD8, $FFEF, $120, $880
	.word $FFD8, $FFDF, $122, $880
	.word $FFE8, $0B, $124, $880
	.word $FFE8, $FFFB, $126, $880
	.word $FFE8, $FFEB, $128, $880
	.word $FFE8, $FFDB, $12A, $880
	.word $FFF8, $0B, $12C, $880
	.word $FFF8, $FFFB, $12E, $880
	.word $FFF8, $FFE1, $130, $880
muscle_power_kick01_RIGHT_end:

muscle_power_kick02_RIGHT:
	.word $FF93, $FFFD, $100, $880
	.word $FF93, $FFED, $102, $880
	.word $FFA3, $0E, $104, $880
	.word $FFA3, $FFFE, $106, $880
	.word $FFA3, $FFEE, $108, $880
	.word $FFB3, $07, $10A, $880
	.word $FFB3, $FFF7, $10C, $880
	.word $FFC3, $0D, $10E, $880
	.word $FFC3, $FFFD, $110, $880
	.word $FFD3, $10, $112, $880
	.word $FFD3, $00, $114, $880
	.word $FFD3, $FFF0, $116, $880
	.word $FFE3, $10, $118, $880
	.word $FFE3, $00, $11A, $880
	.word $FFE3, $FFF0, $11C, $880
	.word $FFF3, $0B, $11E, $880
	.word $FFF3, $FFFB, $120, $880
muscle_power_kick02_RIGHT_end:

muscle_power_kick03_RIGHT:
	.word $FF9A, $33, $100, $880
	.word $FF9A, $18, $102, $880
	.word $FF9A, $08, $104, $880
	.word $FF9A, $FFF8, $106, $880
	.word $FFAA, $34, $108, $880
	.word $FFAA, $24, $10A, $880
	.word $FFAA, $14, $10C, $880
	.word $FFAA, $04, $10E, $880
	.word $FFAA, $FFF4, $110, $880
	.word $FFAA, $FFE4, $112, $880
	.word $FFBA, $24, $114, $880
	.word $FFBA, $14, $116, $880
	.word $FFBA, $04, $118, $880
	.word $FFBA, $FFF4, $11A, $880
	.word $FFBA, $FFE4, $11C, $880
	.word $FFCA, $13, $11E, $880
	.word $FFCA, $03, $120, $880
	.word $FFCA, $FFF3, $122, $880
	.word $FFCA, $FFE3, $124, $880
	.word $FFDA, $13, $126, $880
	.word $FFDA, $03, $128, $880
	.word $FFEA, $0E, $12A, $880
	.word $FFEA, $FFFE, $12C, $880
	.word $FFFA, $0B, $12E, $880
	.word $FFFA, $FFFB, $130, $880
muscle_power_kick03_RIGHT_end:

muscle_power_kick04_ref02_RIGHT:
	.word $FF93, $FFFD, $100, $880
	.word $FF93, $FFED, $102, $880
	.word $FFA3, $0E, $104, $880
	.word $FFA3, $FFFE, $106, $880
	.word $FFA3, $FFEE, $108, $880
	.word $FFB3, $07, $10A, $880
	.word $FFB3, $FFF7, $10C, $880
	.word $FFC3, $0D, $10E, $880
	.word $FFC3, $FFFD, $110, $880
	.word $FFD3, $10, $112, $880
	.word $FFD3, $00, $114, $880
	.word $FFD3, $FFF0, $116, $880
	.word $FFE3, $10, $118, $880
	.word $FFE3, $00, $11A, $880
	.word $FFE3, $FFF0, $11C, $880
	.word $FFF3, $0B, $11E, $880
	.word $FFF3, $FFFB, $120, $880
muscle_power_kick04_ref02_RIGHT_end:

muscle_power_kick05_ref01_RIGHT:
	.word $FF98, $15, $100, $880
	.word $FF98, $05, $102, $880
	.word $FF98, $FFF5, $104, $880
	.word $FF98, $FFE5, $106, $880
	.word $FFA8, $18, $108, $880
	.word $FFA8, $08, $10A, $880
	.word $FFA8, $FFF8, $10C, $880
	.word $FFA8, $FFE8, $10E, $880
	.word $FFB8, $17, $110, $880
	.word $FFB8, $06, $112, $880
	.word $FFB8, $FFF6, $114, $880
	.word $FFC8, $0C, $116, $880
	.word $FFC8, $FFFC, $118, $880
	.word $FFC8, $FFEC, $11A, $880
	.word $FFD8, $0F, $11C, $880
	.word $FFD8, $FFFF, $11E, $880
	.word $FFD8, $FFEF, $120, $880
	.word $FFD8, $FFDF, $122, $880
	.word $FFE8, $0B, $124, $880
	.word $FFE8, $FFFB, $126, $880
	.word $FFE8, $FFEB, $128, $880
	.word $FFE8, $FFDB, $12A, $880
	.word $FFF8, $0B, $12C, $880
	.word $FFF8, $FFFB, $12E, $880
	.word $FFF8, $FFE1, $130, $880
muscle_power_kick05_ref01_RIGHT_end:

muscle_power_idle01_LEFT:
	.word $FF9E, $FFE5, $100, $80
	.word $FF9E, $FFF5, $102, $80
	.word $FF9E, $05, $104, $80
	.word $FFAE, $FFE1, $106, $80
	.word $FFAE, $FFF1, $108, $80
	.word $FFAE, $01, $10A, $80
	.word $FFBE, $FFE4, $10C, $80
	.word $FFBE, $FFF4, $10E, $80
	.word $FFBE, $04, $110, $80
	.word $FFCE, $FFE5, $112, $80
	.word $FFCE, $FFF5, $114, $80
	.word $FFCE, $05, $116, $80
	.word $FFDE, $FFE5, $118, $80
	.word $FFDE, $FFF5, $11A, $80
	.word $FFDE, $05, $11C, $80
	.word $FFEE, $FFE5, $11E, $80
	.word $FFEE, $FFF5, $120, $80
	.word $FFEE, $05, $122, $80
	.word $FFEE, $15, $124, $80
	.word $FFFE, $0C, $126, $80
muscle_power_idle01_LEFT_end:

muscle_power_walk01_LEFT:
	.word $FF97, $FFED, $100, $80
	.word $FF97, $FFFD, $102, $80
	.word $FF97, $0D, $104, $80
	.word $FFA7, $FFD8, $106, $80
	.word $FFA7, $FFE8, $108, $80
	.word $FFA7, $FFF8, $10A, $80
	.word $FFA7, $08, $10C, $80
	.word $FFA7, $18, $10E, $80
	.word $FFB7, $FFD9, $110, $80
	.word $FFB7, $FFE9, $112, $80
	.word $FFB7, $FFF9, $114, $80
	.word $FFB7, $09, $116, $80
	.word $FFC7, $FFF3, $118, $80
	.word $FFC7, $03, $11A, $80
	.word $FFD7, $FFEF, $11C, $80
	.word $FFD7, $FFFF, $11E, $80
	.word $FFD7, $0F, $120, $80
	.word $FFE7, $FFF1, $122, $80
	.word $FFE7, $07, $124, $80
	.word $FFE7, $17, $126, $80
	.word $FFF7, $FFEB, $128, $80
	.word $FFF7, $FFFB, $12A, $80
	.word $FFF7, $14, $12C, $80
muscle_power_walk01_LEFT_end:

muscle_power_walk02_LEFT:
	.word $FF96, $FFEE, $100, $80
	.word $FF96, $FFFE, $102, $80
	.word $FFA6, $FFDE, $104, $80
	.word $FFA6, $FFEE, $106, $80
	.word $FFA6, $FFFE, $108, $80
	.word $FFB6, $FFF1, $10A, $80
	.word $FFB6, $01, $10C, $80
	.word $FFC6, $FFF7, $10E, $80
	.word $FFC6, $07, $110, $80
	.word $FFD6, $FFF4, $112, $80
	.word $FFD6, $04, $114, $80
	.word $FFD6, $14, $116, $80
	.word $FFE6, $FFF6, $118, $80
	.word $FFE6, $06, $11A, $80
	.word $FFE6, $16, $11C, $80
	.word $FFF6, $FFF2, $11E, $80
	.word $FFF6, $02, $120, $80
	.word $FFF6, $12, $122, $80
muscle_power_walk02_LEFT_end:

muscle_power_walk03_LEFT:
	.word $FF93, $FFEE, $100, $80
	.word $FF93, $FFFE, $102, $80
	.word $FFA3, $FFE0, $104, $80
	.word $FFA3, $FFF0, $106, $80
	.word $FFA3, $00, $108, $80
	.word $FFB3, $FFEE, $10A, $80
	.word $FFB3, $FFFE, $10C, $80
	.word $FFB3, $0E, $10E, $80
	.word $FFC3, $FFE6, $110, $80
	.word $FFC3, $FFF6, $112, $80
	.word $FFC3, $06, $114, $80
	.word $FFD3, $FFE6, $116, $80
	.word $FFD3, $FFF6, $118, $80
	.word $FFD3, $06, $11A, $80
	.word $FFE3, $FFEA, $11C, $80
	.word $FFE3, $FFFA, $11E, $80
	.word $FFE3, $0A, $120, $80
	.word $FFF3, $09, $122, $80
	.word $FFF3, $19, $124, $80
muscle_power_walk03_LEFT_end:

muscle_power_walk04_LEFT:
	.word $FF97, $FFEE, $100, $80
	.word $FF97, $FFFE, $102, $80
	.word $FFA7, $FFDE, $104, $80
	.word $FFA7, $FFEE, $106, $80
	.word $FFA7, $FFFE, $108, $80
	.word $FFA7, $0E, $10A, $80
	.word $FFB7, $FFE4, $10C, $80
	.word $FFB7, $FFF4, $10E, $80
	.word $FFB7, $04, $110, $80
	.word $FFC7, $FFEA, $112, $80
	.word $FFC7, $FFFA, $114, $80
	.word $FFC7, $0A, $116, $80
	.word $FFD7, $FFE7, $118, $80
	.word $FFD7, $FFF7, $11A, $80
	.word $FFD7, $07, $11C, $80
	.word $FFD7, $17, $11E, $80
	.word $FFE7, $FFE3, $120, $80
	.word $FFE7, $FFF3, $122, $80
	.word $FFE7, $07, $124, $80
	.word $FFE7, $17, $126, $80
	.word $FFF7, $FFE3, $128, $80
	.word $FFF7, $FFF3, $12A, $80
	.word $FFF7, $19, $12C, $80
muscle_power_walk04_LEFT_end:

muscle_power_walk05_LEFT:
	.word $FF9A, $FFEE, $100, $80
	.word $FF9A, $FFFE, $102, $80
	.word $FFAA, $FFE2, $104, $80
	.word $FFAA, $FFF2, $106, $80
	.word $FFAA, $02, $108, $80
	.word $FFBA, $FFEE, $10A, $80
	.word $FFBA, $FFFE, $10C, $80
	.word $FFBA, $0E, $10E, $80
	.word $FFCA, $FFEE, $110, $80
	.word $FFCA, $FFFE, $112, $80
	.word $FFCA, $0E, $114, $80
	.word $FFDA, $FFED, $116, $80
	.word $FFDA, $FFFD, $118, $80
	.word $FFDA, $0D, $11A, $80
	.word $FFEA, $FFEF, $11C, $80
	.word $FFEA, $FFFF, $11E, $80
	.word $FFEA, $0F, $120, $80
	.word $FFFA, $FFEE, $122, $80
	.word $FFFA, $FFFE, $124, $80
	.word $FFFA, $16, $126, $80
muscle_power_walk05_LEFT_end:

muscle_power_walk06_LEFT:
	.word $FF98, $FFEE, $100, $80
	.word $FF98, $FFFE, $102, $80
	.word $FFA8, $FFDE, $104, $80
	.word $FFA8, $FFEE, $106, $80
	.word $FFA8, $FFFE, $108, $80
	.word $FFA8, $0E, $10A, $80
	.word $FFB8, $FFF0, $10C, $80
	.word $FFB8, $00, $10E, $80
	.word $FFB8, $10, $110, $80
	.word $FFC8, $FFEE, $112, $80
	.word $FFC8, $FFFE, $114, $80
	.word $FFC8, $0E, $116, $80
	.word $FFD8, $FFEE, $118, $80
	.word $FFD8, $FFFE, $11A, $80
	.word $FFE8, $FFF2, $11C, $80
	.word $FFE8, $02, $11E, $80
	.word $FFF8, $00, $120, $80
	.word $FFF8, $10, $122, $80
muscle_power_walk06_LEFT_end:

muscle_power_kick01_LEFT:
	.word $FF98, $FFDB, $100, $80
	.word $FF98, $FFEB, $102, $80
	.word $FF98, $FFFB, $104, $80
	.word $FF98, $0B, $106, $80
	.word $FFA8, $FFD8, $108, $80
	.word $FFA8, $FFE8, $10A, $80
	.word $FFA8, $FFF8, $10C, $80
	.word $FFA8, $08, $10E, $80
	.word $FFB8, $FFD9, $110, $80
	.word $FFB8, $FFEA, $112, $80
	.word $FFB8, $FFFA, $114, $80
	.word $FFC8, $FFE4, $116, $80
	.word $FFC8, $FFF4, $118, $80
	.word $FFC8, $04, $11A, $80
	.word $FFD8, $FFE1, $11C, $80
	.word $FFD8, $FFF1, $11E, $80
	.word $FFD8, $01, $120, $80
	.word $FFD8, $11, $122, $80
	.word $FFE8, $FFE5, $124, $80
	.word $FFE8, $FFF5, $126, $80
	.word $FFE8, $05, $128, $80
	.word $FFE8, $15, $12A, $80
	.word $FFF8, $FFE5, $12C, $80
	.word $FFF8, $FFF5, $12E, $80
	.word $FFF8, $0F, $130, $80
muscle_power_kick01_LEFT_end:

muscle_power_kick02_LEFT:
	.word $FF93, $FFF3, $100, $80
	.word $FF93, $03, $102, $80
	.word $FFA3, $FFE2, $104, $80
	.word $FFA3, $FFF2, $106, $80
	.word $FFA3, $02, $108, $80
	.word $FFB3, $FFE9, $10A, $80
	.word $FFB3, $FFF9, $10C, $80
	.word $FFC3, $FFE3, $10E, $80
	.word $FFC3, $FFF3, $110, $80
	.word $FFD3, $FFE0, $112, $80
	.word $FFD3, $FFF0, $114, $80
	.word $FFD3, $00, $116, $80
	.word $FFE3, $FFE0, $118, $80
	.word $FFE3, $FFF0, $11A, $80
	.word $FFE3, $00, $11C, $80
	.word $FFF3, $FFE5, $11E, $80
	.word $FFF3, $FFF5, $120, $80
muscle_power_kick02_LEFT_end:

muscle_power_kick03_LEFT:
	.word $FF9A, $FFBD, $100, $80
	.word $FF9A, $FFD8, $102, $80
	.word $FF9A, $FFE8, $104, $80
	.word $FF9A, $FFF8, $106, $80
	.word $FFAA, $FFBC, $108, $80
	.word $FFAA, $FFCC, $10A, $80
	.word $FFAA, $FFDC, $10C, $80
	.word $FFAA, $FFEC, $10E, $80
	.word $FFAA, $FFFC, $110, $80
	.word $FFAA, $0C, $112, $80
	.word $FFBA, $FFCC, $114, $80
	.word $FFBA, $FFDC, $116, $80
	.word $FFBA, $FFEC, $118, $80
	.word $FFBA, $FFFC, $11A, $80
	.word $FFBA, $0C, $11C, $80
	.word $FFCA, $FFDD, $11E, $80
	.word $FFCA, $FFED, $120, $80
	.word $FFCA, $FFFD, $122, $80
	.word $FFCA, $0D, $124, $80
	.word $FFDA, $FFDD, $126, $80
	.word $FFDA, $FFED, $128, $80
	.word $FFEA, $FFE2, $12A, $80
	.word $FFEA, $FFF2, $12C, $80
	.word $FFFA, $FFE5, $12E, $80
	.word $FFFA, $FFF5, $130, $80
muscle_power_kick03_LEFT_end:

muscle_power_kick04_ref02_LEFT:
	.word $FF93, $FFF3, $100, $80
	.word $FF93, $03, $102, $80
	.word $FFA3, $FFE2, $104, $80
	.word $FFA3, $FFF2, $106, $80
	.word $FFA3, $02, $108, $80
	.word $FFB3, $FFE9, $10A, $80
	.word $FFB3, $FFF9, $10C, $80
	.word $FFC3, $FFE3, $10E, $80
	.word $FFC3, $FFF3, $110, $80
	.word $FFD3, $FFE0, $112, $80
	.word $FFD3, $FFF0, $114, $80
	.word $FFD3, $00, $116, $80
	.word $FFE3, $FFE0, $118, $80
	.word $FFE3, $FFF0, $11A, $80
	.word $FFE3, $00, $11C, $80
	.word $FFF3, $FFE5, $11E, $80
	.word $FFF3, $FFF5, $120, $80
muscle_power_kick04_ref02_LEFT_end:

muscle_power_kick05_ref01_LEFT:
	.word $FF98, $FFDB, $100, $80
	.word $FF98, $FFEB, $102, $80
	.word $FF98, $FFFB, $104, $80
	.word $FF98, $0B, $106, $80
	.word $FFA8, $FFD8, $108, $80
	.word $FFA8, $FFE8, $10A, $80
	.word $FFA8, $FFF8, $10C, $80
	.word $FFA8, $08, $10E, $80
	.word $FFB8, $FFD9, $110, $80
	.word $FFB8, $FFEA, $112, $80
	.word $FFB8, $FFFA, $114, $80
	.word $FFC8, $FFE4, $116, $80
	.word $FFC8, $FFF4, $118, $80
	.word $FFC8, $04, $11A, $80
	.word $FFD8, $FFE1, $11C, $80
	.word $FFD8, $FFF1, $11E, $80
	.word $FFD8, $01, $120, $80
	.word $FFD8, $11, $122, $80
	.word $FFE8, $FFE5, $124, $80
	.word $FFE8, $FFF5, $126, $80
	.word $FFE8, $05, $128, $80
	.word $FFE8, $15, $12A, $80
	.word $FFF8, $FFE5, $12C, $80
	.word $FFF8, $FFF5, $12E, $80
	.word $FFF8, $0F, $130, $80
muscle_power_kick05_ref01_LEFT_end:

