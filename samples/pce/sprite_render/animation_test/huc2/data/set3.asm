;#######################################################
;
; Generated by SPReD-PCE Copyright 2017-2022 0x8BitDev
;
;#######################################################

_set3_chr0:	.incbin "data/_set3_chr0.bin"	; 2944 bytes
_set3_chr1:	.incbin "data/_set3_chr1.bin"	; 2304 bytes
_set3_chr2:	.incbin "data/_set3_chr2.bin"	; 2432 bytes
_set3_chr3:	.incbin "data/_set3_chr3.bin"	; 2944 bytes
_set3_chr4:	.incbin "data/_set3_chr4.bin"	; 2560 bytes
_set3_chr5:	.incbin "data/_set3_chr5.bin"	; 2304 bytes
_set3_chr6:	.incbin "data/_set3_chr6.bin"	; 2560 bytes
_set3_chr7:	.incbin "data/_set3_chr7.bin"	; 3200 bytes
_set3_chr8:	.incbin "data/_set3_chr8.bin"	; 2176 bytes
_set3_chr9:	.incbin "data/_set3_chr9.bin"	; 3200 bytes

_set3_SG_arr:	
	.word 2944, _set3_chr0, bank(_set3_chr0)
	.word 2304, _set3_chr1, bank(_set3_chr1)
	.word 2432, _set3_chr2, bank(_set3_chr2)
	.word 2944, _set3_chr3, bank(_set3_chr3)
	.word 2560, _set3_chr4, bank(_set3_chr4)
	.word 2304, _set3_chr5, bank(_set3_chr5)
	.word 2560, _set3_chr6, bank(_set3_chr6)
	.word 3200, _set3_chr7, bank(_set3_chr7)
	.word 2176, _set3_chr8, bank(_set3_chr8)
	.word 3200, _set3_chr9, bank(_set3_chr9)


_set3_palette:
_set3_palette_slot0:
	.word $147, $87, $44, $107, $14D, $1B0, $170, $E9, $173, $F0, $1B5, $B0, $58, $133, $F2, $1B6
_set3_palette_end:

_set3_num_frames:
	.byte $0C
_set3_frames_data:
_mpwr_idle01_LEFT_frame:
	.word _mpwr_idle01_LEFT
	.byte bank(_mpwr_idle01_LEFT)
	.word mpwr_idle01_LEFT_end - _mpwr_idle01_LEFT	; data size
	.byte 6		; GFX bank index (chr6)
_mpwr_walk01_LEFT_frame:
	.word _mpwr_walk01_LEFT
	.byte bank(_mpwr_walk01_LEFT)
	.word mpwr_walk01_LEFT_end - _mpwr_walk01_LEFT
	.byte 0
_mpwr_walk02_LEFT_frame:
	.word _mpwr_walk02_LEFT
	.byte bank(_mpwr_walk02_LEFT)
	.word mpwr_walk02_LEFT_end - _mpwr_walk02_LEFT
	.byte 1
_mpwr_walk03_LEFT_frame:
	.word _mpwr_walk03_LEFT
	.byte bank(_mpwr_walk03_LEFT)
	.word mpwr_walk03_LEFT_end - _mpwr_walk03_LEFT
	.byte 2
_mpwr_walk04_LEFT_frame:
	.word _mpwr_walk04_LEFT
	.byte bank(_mpwr_walk04_LEFT)
	.word mpwr_walk04_LEFT_end - _mpwr_walk04_LEFT
	.byte 3
_mpwr_walk05_LEFT_frame:
	.word _mpwr_walk05_LEFT
	.byte bank(_mpwr_walk05_LEFT)
	.word mpwr_walk05_LEFT_end - _mpwr_walk05_LEFT
	.byte 4
_mpwr_walk06_LEFT_frame:
	.word _mpwr_walk06_LEFT
	.byte bank(_mpwr_walk06_LEFT)
	.word mpwr_walk06_LEFT_end - _mpwr_walk06_LEFT
	.byte 5
_mpwr_kick01_LEFT_frame:
	.word _mpwr_kick01_LEFT
	.byte bank(_mpwr_kick01_LEFT)
	.word mpwr_kick01_LEFT_end - _mpwr_kick01_LEFT
	.byte 7
_mpwr_kick02_LEFT_frame:
	.word _mpwr_kick02_LEFT
	.byte bank(_mpwr_kick02_LEFT)
	.word mpwr_kick02_LEFT_end - _mpwr_kick02_LEFT
	.byte 8
_mpwr_kick03_LEFT_frame:
	.word _mpwr_kick03_LEFT
	.byte bank(_mpwr_kick03_LEFT)
	.word mpwr_kick03_LEFT_end - _mpwr_kick03_LEFT
	.byte 9
_mpwr_kick04_ref02_LEFT_frame:
	.word _mpwr_kick04_ref02_LEFT
	.byte bank(_mpwr_kick04_ref02_LEFT)
	.word mpwr_kick04_ref02_LEFT_end - _mpwr_kick04_ref02_LEFT
	.byte 8
_mpwr_kick05_ref01_LEFT_frame:
	.word _mpwr_kick05_ref01_LEFT
	.byte bank(_mpwr_kick05_ref01_LEFT)
	.word mpwr_kick05_ref01_LEFT_end - _mpwr_kick05_ref01_LEFT
	.byte 7


	; #1: Y pos, #2: X pos, #3: CHR index, #4: CHR desc

_mpwr_idle01_LEFT:
	.word $FF9E, $FFE5, $200, $82
	.word $FF9E, $FFF5, $202, $82
	.word $FF9E, $05, $204, $82
	.word $FFAE, $FFE1, $206, $82
	.word $FFAE, $FFF1, $208, $82
	.word $FFAE, $01, $20A, $82
	.word $FFBE, $FFE4, $20C, $82
	.word $FFBE, $FFF4, $20E, $82
	.word $FFBE, $04, $210, $82
	.word $FFCE, $FFE5, $212, $82
	.word $FFCE, $FFF5, $214, $82
	.word $FFCE, $05, $216, $82
	.word $FFDE, $FFE5, $218, $82
	.word $FFDE, $FFF5, $21A, $82
	.word $FFDE, $05, $21C, $82
	.word $FFEE, $FFE5, $21E, $82
	.word $FFEE, $FFF5, $220, $82
	.word $FFEE, $05, $222, $82
	.word $FFEE, $15, $224, $82
	.word $FFFE, $0C, $226, $82
mpwr_idle01_LEFT_end:

_mpwr_walk01_LEFT:
	.word $FF97, $FFED, $200, $82
	.word $FF97, $FFFD, $202, $82
	.word $FF97, $0D, $204, $82
	.word $FFA7, $FFD8, $206, $82
	.word $FFA7, $FFE8, $208, $82
	.word $FFA7, $FFF8, $20A, $82
	.word $FFA7, $08, $20C, $82
	.word $FFA7, $18, $20E, $82
	.word $FFB7, $FFD9, $210, $82
	.word $FFB7, $FFE9, $212, $82
	.word $FFB7, $FFF9, $214, $82
	.word $FFB7, $09, $216, $82
	.word $FFC7, $FFF3, $218, $82
	.word $FFC7, $03, $21A, $82
	.word $FFD7, $FFEF, $21C, $82
	.word $FFD7, $FFFF, $21E, $82
	.word $FFD7, $0F, $220, $82
	.word $FFE7, $FFF1, $222, $82
	.word $FFE7, $07, $224, $82
	.word $FFE7, $17, $226, $82
	.word $FFF7, $FFEB, $228, $82
	.word $FFF7, $FFFB, $22A, $82
	.word $FFF7, $14, $22C, $82
mpwr_walk01_LEFT_end:

_mpwr_walk02_LEFT:
	.word $FF96, $FFEE, $200, $82
	.word $FF96, $FFFE, $202, $82
	.word $FFA6, $FFDE, $204, $82
	.word $FFA6, $FFEE, $206, $82
	.word $FFA6, $FFFE, $208, $82
	.word $FFB6, $FFF1, $20A, $82
	.word $FFB6, $01, $20C, $82
	.word $FFC6, $FFF7, $20E, $82
	.word $FFC6, $07, $210, $82
	.word $FFD6, $FFF4, $212, $82
	.word $FFD6, $04, $214, $82
	.word $FFD6, $14, $216, $82
	.word $FFE6, $FFF6, $218, $82
	.word $FFE6, $06, $21A, $82
	.word $FFE6, $16, $21C, $82
	.word $FFF6, $FFF2, $21E, $82
	.word $FFF6, $02, $220, $82
	.word $FFF6, $12, $222, $82
mpwr_walk02_LEFT_end:

_mpwr_walk03_LEFT:
	.word $FF93, $FFEE, $200, $82
	.word $FF93, $FFFE, $202, $82
	.word $FFA3, $FFE0, $204, $82
	.word $FFA3, $FFF0, $206, $82
	.word $FFA3, $00, $208, $82
	.word $FFB3, $FFEE, $20A, $82
	.word $FFB3, $FFFE, $20C, $82
	.word $FFB3, $0E, $20E, $82
	.word $FFC3, $FFE6, $210, $82
	.word $FFC3, $FFF6, $212, $82
	.word $FFC3, $06, $214, $82
	.word $FFD3, $FFE6, $216, $82
	.word $FFD3, $FFF6, $218, $82
	.word $FFD3, $06, $21A, $82
	.word $FFE3, $FFEA, $21C, $82
	.word $FFE3, $FFFA, $21E, $82
	.word $FFE3, $0A, $220, $82
	.word $FFF3, $09, $222, $82
	.word $FFF3, $19, $224, $82
mpwr_walk03_LEFT_end:

_mpwr_walk04_LEFT:
	.word $FF97, $FFEE, $200, $82
	.word $FF97, $FFFE, $202, $82
	.word $FFA7, $FFDE, $204, $82
	.word $FFA7, $FFEE, $206, $82
	.word $FFA7, $FFFE, $208, $82
	.word $FFA7, $0E, $20A, $82
	.word $FFB7, $FFE4, $20C, $82
	.word $FFB7, $FFF4, $20E, $82
	.word $FFB7, $04, $210, $82
	.word $FFC7, $FFEA, $212, $82
	.word $FFC7, $FFFA, $214, $82
	.word $FFC7, $0A, $216, $82
	.word $FFD7, $FFE7, $218, $82
	.word $FFD7, $FFF7, $21A, $82
	.word $FFD7, $07, $21C, $82
	.word $FFD7, $17, $21E, $82
	.word $FFE7, $FFE3, $220, $82
	.word $FFE7, $FFF3, $222, $82
	.word $FFE7, $07, $224, $82
	.word $FFE7, $17, $226, $82
	.word $FFF7, $FFE3, $228, $82
	.word $FFF7, $FFF3, $22A, $82
	.word $FFF7, $19, $22C, $82
mpwr_walk04_LEFT_end:

_mpwr_walk05_LEFT:
	.word $FF9A, $FFEE, $200, $82
	.word $FF9A, $FFFE, $202, $82
	.word $FFAA, $FFE2, $204, $82
	.word $FFAA, $FFF2, $206, $82
	.word $FFAA, $02, $208, $82
	.word $FFBA, $FFEE, $20A, $82
	.word $FFBA, $FFFE, $20C, $82
	.word $FFBA, $0E, $20E, $82
	.word $FFCA, $FFEE, $210, $82
	.word $FFCA, $FFFE, $212, $82
	.word $FFCA, $0E, $214, $82
	.word $FFDA, $FFED, $216, $82
	.word $FFDA, $FFFD, $218, $82
	.word $FFDA, $0D, $21A, $82
	.word $FFEA, $FFEF, $21C, $82
	.word $FFEA, $FFFF, $21E, $82
	.word $FFEA, $0F, $220, $82
	.word $FFFA, $FFEE, $222, $82
	.word $FFFA, $FFFE, $224, $82
	.word $FFFA, $16, $226, $82
mpwr_walk05_LEFT_end:

_mpwr_walk06_LEFT:
	.word $FF98, $FFEE, $200, $82
	.word $FF98, $FFFE, $202, $82
	.word $FFA8, $FFDE, $204, $82
	.word $FFA8, $FFEE, $206, $82
	.word $FFA8, $FFFE, $208, $82
	.word $FFA8, $0E, $20A, $82
	.word $FFB8, $FFF0, $20C, $82
	.word $FFB8, $00, $20E, $82
	.word $FFB8, $10, $210, $82
	.word $FFC8, $FFEE, $212, $82
	.word $FFC8, $FFFE, $214, $82
	.word $FFC8, $0E, $216, $82
	.word $FFD8, $FFEE, $218, $82
	.word $FFD8, $FFFE, $21A, $82
	.word $FFE8, $FFF2, $21C, $82
	.word $FFE8, $02, $21E, $82
	.word $FFF8, $00, $220, $82
	.word $FFF8, $10, $222, $82
mpwr_walk06_LEFT_end:

_mpwr_kick01_LEFT:
	.word $FF98, $FFDB, $200, $82
	.word $FF98, $FFEB, $202, $82
	.word $FF98, $FFFB, $204, $82
	.word $FF98, $0B, $206, $82
	.word $FFA8, $FFD8, $208, $82
	.word $FFA8, $FFE8, $20A, $82
	.word $FFA8, $FFF8, $20C, $82
	.word $FFA8, $08, $20E, $82
	.word $FFB8, $FFD9, $210, $82
	.word $FFB8, $FFEA, $212, $82
	.word $FFB8, $FFFA, $214, $82
	.word $FFC8, $FFE4, $216, $82
	.word $FFC8, $FFF4, $218, $82
	.word $FFC8, $04, $21A, $82
	.word $FFD8, $FFE1, $21C, $82
	.word $FFD8, $FFF1, $21E, $82
	.word $FFD8, $01, $220, $82
	.word $FFD8, $11, $222, $82
	.word $FFE8, $FFE5, $224, $82
	.word $FFE8, $FFF5, $226, $82
	.word $FFE8, $05, $228, $82
	.word $FFE8, $15, $22A, $82
	.word $FFF8, $FFE5, $22C, $82
	.word $FFF8, $FFF5, $22E, $82
	.word $FFF8, $0F, $230, $82
mpwr_kick01_LEFT_end:

_mpwr_kick02_LEFT:
	.word $FF93, $FFF3, $200, $82
	.word $FF93, $03, $202, $82
	.word $FFA3, $FFE2, $204, $82
	.word $FFA3, $FFF2, $206, $82
	.word $FFA3, $02, $208, $82
	.word $FFB3, $FFE9, $20A, $82
	.word $FFB3, $FFF9, $20C, $82
	.word $FFC3, $FFE3, $20E, $82
	.word $FFC3, $FFF3, $210, $82
	.word $FFD3, $FFE0, $212, $82
	.word $FFD3, $FFF0, $214, $82
	.word $FFD3, $00, $216, $82
	.word $FFE3, $FFE0, $218, $82
	.word $FFE3, $FFF0, $21A, $82
	.word $FFE3, $00, $21C, $82
	.word $FFF3, $FFE5, $21E, $82
	.word $FFF3, $FFF5, $220, $82
mpwr_kick02_LEFT_end:

_mpwr_kick03_LEFT:
	.word $FF9A, $FFBD, $200, $82
	.word $FF9A, $FFD8, $202, $82
	.word $FF9A, $FFE8, $204, $82
	.word $FF9A, $FFF8, $206, $82
	.word $FFAA, $FFBC, $208, $82
	.word $FFAA, $FFCC, $20A, $82
	.word $FFAA, $FFDC, $20C, $82
	.word $FFAA, $FFEC, $20E, $82
	.word $FFAA, $FFFC, $210, $82
	.word $FFAA, $0C, $212, $82
	.word $FFBA, $FFCC, $214, $82
	.word $FFBA, $FFDC, $216, $82
	.word $FFBA, $FFEC, $218, $82
	.word $FFBA, $FFFC, $21A, $82
	.word $FFBA, $0C, $21C, $82
	.word $FFCA, $FFDD, $21E, $82
	.word $FFCA, $FFED, $220, $82
	.word $FFCA, $FFFD, $222, $82
	.word $FFCA, $0D, $224, $82
	.word $FFDA, $FFDD, $226, $82
	.word $FFDA, $FFED, $228, $82
	.word $FFEA, $FFE2, $22A, $82
	.word $FFEA, $FFF2, $22C, $82
	.word $FFFA, $FFE5, $22E, $82
	.word $FFFA, $FFF5, $230, $82
mpwr_kick03_LEFT_end:

_mpwr_kick04_ref02_LEFT:
	.word $FF93, $FFF3, $200, $82
	.word $FF93, $03, $202, $82
	.word $FFA3, $FFE2, $204, $82
	.word $FFA3, $FFF2, $206, $82
	.word $FFA3, $02, $208, $82
	.word $FFB3, $FFE9, $20A, $82
	.word $FFB3, $FFF9, $20C, $82
	.word $FFC3, $FFE3, $20E, $82
	.word $FFC3, $FFF3, $210, $82
	.word $FFD3, $FFE0, $212, $82
	.word $FFD3, $FFF0, $214, $82
	.word $FFD3, $00, $216, $82
	.word $FFE3, $FFE0, $218, $82
	.word $FFE3, $FFF0, $21A, $82
	.word $FFE3, $00, $21C, $82
	.word $FFF3, $FFE5, $21E, $82
	.word $FFF3, $FFF5, $220, $82
mpwr_kick04_ref02_LEFT_end:

_mpwr_kick05_ref01_LEFT:
	.word $FF98, $FFDB, $200, $82
	.word $FF98, $FFEB, $202, $82
	.word $FF98, $FFFB, $204, $82
	.word $FF98, $0B, $206, $82
	.word $FFA8, $FFD8, $208, $82
	.word $FFA8, $FFE8, $20A, $82
	.word $FFA8, $FFF8, $20C, $82
	.word $FFA8, $08, $20E, $82
	.word $FFB8, $FFD9, $210, $82
	.word $FFB8, $FFEA, $212, $82
	.word $FFB8, $FFFA, $214, $82
	.word $FFC8, $FFE4, $216, $82
	.word $FFC8, $FFF4, $218, $82
	.word $FFC8, $04, $21A, $82
	.word $FFD8, $FFE1, $21C, $82
	.word $FFD8, $FFF1, $21E, $82
	.word $FFD8, $01, $220, $82
	.word $FFD8, $11, $222, $82
	.word $FFE8, $FFE5, $224, $82
	.word $FFE8, $FFF5, $226, $82
	.word $FFE8, $05, $228, $82
	.word $FFE8, $15, $22A, $82
	.word $FFF8, $FFE5, $22C, $82
	.word $FFF8, $FFF5, $22E, $82
	.word $FFF8, $0F, $230, $82
mpwr_kick05_ref01_LEFT_end:

