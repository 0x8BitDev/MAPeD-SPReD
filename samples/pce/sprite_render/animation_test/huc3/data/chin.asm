;#######################################################
;
; Generated by SPReD-PCE Copyright 2017-2022 0x8BitDev
;
;#######################################################

_chin_chr0:	.incbin "data/_chin_chr0.bin"	; 1920 bytes
_chin_chr1:	.incbin "data/_chin_chr1.bin"	; 1664 bytes
_chin_chr2:	.incbin "data/_chin_chr2.bin"	; 1536 bytes
_chin_chr3:	.incbin "data/_chin_chr3.bin"	; 1536 bytes
_chin_chr4:	.incbin "data/_chin_chr4.bin"	; 1536 bytes

_chin_SG_arr:	
	.word 1920, _chin_chr0, bank(_chin_chr0)
	.word 1664, _chin_chr1, bank(_chin_chr1)
	.word 1536, _chin_chr2, bank(_chin_chr2)
	.word 1536, _chin_chr3, bank(_chin_chr3)
	.word 1536, _chin_chr4, bank(_chin_chr4)
	.word 0, 0, 0	; skipped data


_chin_palette:
_chin_palette_slot0:
	.word $1C0, $13, $14, $9E, $DE, $126, $DB, $E1, $98, $EA, $1B5, $133, $1B, $1B6, $92, $00
_chin_palette_slot1:
	.word $1C0, $1B, $24, $B6, $FE, $12E, $DB, $E0, $98, $EA, $1B5, $133, $A0, $1B6, $92, $00
_chin_palette_slot2:
	.word $1C0, $8C, $CD, $155, $19D, $15E, $DB, $E0, $98, $EA, $1B5, $133, $5B, $1B6, $92, $00
_chin_palette_end:

_chin_num_frames:
	.byte $0A
_chin_frames_data:
_chin_idle01_RIGHT_frame:
	.word _chin_idle01_RIGHT
	.byte bank(_chin_idle01_RIGHT)
_chin_walk01_RIGHT_frame:
	.word _chin_walk01_RIGHT
	.byte bank(_chin_walk01_RIGHT)
_chin_walk02_RIGHT_frame:
	.word _chin_walk02_RIGHT
	.byte bank(_chin_walk02_RIGHT)
_chin_walk03_RIGHT_frame:
	.word _chin_walk03_RIGHT
	.byte bank(_chin_walk03_RIGHT)
_chin_walk04_RIGHT_frame:
	.word _chin_walk04_RIGHT
	.byte bank(_chin_walk04_RIGHT)
_chin_idle01_LEFT_frame:
	.word _chin_idle01_LEFT
	.byte bank(_chin_idle01_LEFT)
_chin_walk01_LEFT_frame:
	.word _chin_walk01_LEFT
	.byte bank(_chin_walk01_LEFT)
_chin_walk02_LEFT_frame:
	.word _chin_walk02_LEFT
	.byte bank(_chin_walk02_LEFT)
_chin_walk03_LEFT_frame:
	.word _chin_walk03_LEFT
	.byte bank(_chin_walk03_LEFT)
_chin_walk04_LEFT_frame:
	.word _chin_walk04_LEFT
	.byte bank(_chin_walk04_LEFT)


	; #1: Y pos, #2: X pos, #3: CHR index, #4: CHR desc

_chin_idle01_RIGHT:
	.word _chin_idle01_RIGHT_end - _chin_idle01_RIGHT - 3	; data size
	.byte 0		; GFX bank index (chr0)

	.word $FFB0, $FFE9, $100, $3180
	.word $FFF0, $FFE9, $110, $180
	.word $FFB0, $09, $114, $80
	.word $FFC0, $09, $116, $80
	.word $FFD0, $09, $118, $80
	.word $FFE0, $09, $11A, $80
	.word $FFF0, $09, $11C, $80
_chin_idle01_RIGHT_end:

_chin_walk01_RIGHT:
	.word _chin_walk01_RIGHT_end - _chin_walk01_RIGHT - 3	; data size
	.byte 1		; GFX bank index (chr1)

	.word $FFB0, $FFEC, $100, $3180
	.word $FFF0, $FFEC, $110, $180
	.word $FFC0, $0C, $114, $80
	.word $FFD0, $0C, $116, $80
	.word $FFF0, $0C, $118, $80
_chin_walk01_RIGHT_end:

_chin_walk02_RIGHT:
	.word _chin_walk02_RIGHT_end - _chin_walk02_RIGHT - 3	; data size
	.byte 2		; GFX bank index (chr2)

	.word $FFB0, $FFEC, $100, $3180
	.word $FFF0, $FFEC, $110, $180
	.word $FFC0, $0C, $114, $80
	.word $FFD0, $0C, $116, $80
_chin_walk02_RIGHT_end:

_chin_walk03_RIGHT:
	.word _chin_walk03_RIGHT_end - _chin_walk03_RIGHT - 3	; data size
	.byte 3		; GFX bank index (chr3)

	.word $FFB0, $FFEC, $100, $3180
	.word $FFF0, $FFEC, $110, $180
	.word $FFC0, $0C, $114, $80
	.word $FFD0, $0C, $116, $80
_chin_walk03_RIGHT_end:

_chin_walk04_RIGHT:
	.word _chin_walk04_RIGHT_end - _chin_walk04_RIGHT - 3	; data size
	.byte 4		; GFX bank index (chr4)

	.word $FFB0, $FFEC, $100, $3180
	.word $FFF0, $FFEC, $110, $180
	.word $FFC0, $0C, $114, $80
	.word $FFD0, $0C, $116, $80
_chin_walk04_RIGHT_end:

_chin_idle01_LEFT:
	.word _chin_idle01_LEFT_end - _chin_idle01_LEFT - 3	; data size
	.byte 0		; GFX bank index (chr0)

	.word $FFB0, $FFF7, $100, $3980
	.word $FFF0, $FFF7, $110, $980
	.word $FFB0, $FFE7, $114, $880
	.word $FFC0, $FFE7, $116, $880
	.word $FFD0, $FFE7, $118, $880
	.word $FFE0, $FFE7, $11A, $880
	.word $FFF0, $FFE7, $11C, $880
_chin_idle01_LEFT_end:

_chin_walk01_LEFT:
	.word _chin_walk01_LEFT_end - _chin_walk01_LEFT - 3	; data size
	.byte 1		; GFX bank index (chr1)

	.word $FFB0, $FFF4, $100, $3980
	.word $FFF0, $FFF4, $110, $980
	.word $FFC0, $FFE4, $114, $880
	.word $FFD0, $FFE4, $116, $880
	.word $FFF0, $FFE4, $118, $880
_chin_walk01_LEFT_end:

_chin_walk02_LEFT:
	.word _chin_walk02_LEFT_end - _chin_walk02_LEFT - 3	; data size
	.byte 2		; GFX bank index (chr2)

	.word $FFB0, $FFF4, $100, $3980
	.word $FFF0, $FFF4, $110, $980
	.word $FFC0, $FFE4, $114, $880
	.word $FFD0, $FFE4, $116, $880
_chin_walk02_LEFT_end:

_chin_walk03_LEFT:
	.word _chin_walk03_LEFT_end - _chin_walk03_LEFT - 3	; data size
	.byte 3		; GFX bank index (chr3)

	.word $FFB0, $FFF4, $100, $3980
	.word $FFF0, $FFF4, $110, $980
	.word $FFC0, $FFE4, $114, $880
	.word $FFD0, $FFE4, $116, $880
_chin_walk03_LEFT_end:

_chin_walk04_LEFT:
	.word _chin_walk04_LEFT_end - _chin_walk04_LEFT - 3	; data size
	.byte 4		; GFX bank index (chr4)

	.word $FFB0, $FFF4, $100, $3980
	.word $FFF0, $FFF4, $110, $980
	.word $FFC0, $FFE4, $114, $880
	.word $FFD0, $FFE4, $116, $880
_chin_walk04_LEFT_end:

