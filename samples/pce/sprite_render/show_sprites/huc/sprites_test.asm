;#######################################################
;
; Generated by SPReD-PCE Copyright 2017-2022 0x8BitDev
;
;#######################################################

_sprites_test_chr0:	.incbin "_sprites_test_chr0.bin"	; 8192 bytes

_sprites_test_SG_arr:	
	.word 8192, _sprites_test_chr0, bank(_sprites_test_chr0)


_sprites_test_palette:
	.word $147, $02, $C6, $06, $173, $133, $F3, $1B6, $00, $00, $00, $00, $00, $00, $00, $00
	.word $147, $136, $CB, $15C, $11B, $1B0, $170, $E9, $173, $F0, $58, $68, $61, $F3, $1B6, $00
	.word $147, $1B0, $170, $E9, $F0, $58, $68, $00, $00, $00, $00, $00, $00, $00, $00, $00
	.word $147, $8B, $D4, $82, $41, $16A, $91, $DA, $10, $1B6, $124, $DB, $92, $49, $00, $72
	.word $147, $12B, $E8, $A0, $E9, $A8, $58, $28, $20, $10, $124, $00, $4A, $93, $124, $1BC
	.word $147, $1F8, $178, $F8, $78, $1FF, $68, $00, $00, $00, $00, $00, $00, $00, $00, $00
_sprites_test_palette_end:

_sprites_test_num_frames:
	.byte $0A
_sprites_test_frames_data:
_jch_RIGHT_32x64_frame:
	.word _jch_RIGHT_32x64
	.byte bank(_jch_RIGHT_32x64)
	.word jch_RIGHT_32x64_end - _jch_RIGHT_32x64	; data size
	.byte 0		; GFX bank index (chr0)
_dr_msl_UP_16x64_0_frame:
	.word _dr_msl_UP_16x64_0
	.byte bank(_dr_msl_UP_16x64_0)
	.word dr_msl_UP_16x64_0_end - _dr_msl_UP_16x64_0
	.byte 0
_dr_msl_UP_16x64_1_ref_frame:
	.word _dr_msl_UP_16x64_1_ref
	.byte bank(_dr_msl_UP_16x64_1_ref)
	.word dr_msl_UP_16x64_1_ref_end - _dr_msl_UP_16x64_1_ref
	.byte 0
_jch_min_16x32_0_frame:
	.word _jch_min_16x32_0
	.byte bank(_jch_min_16x32_0)
	.word jch_min_16x32_0_end - _jch_min_16x32_0
	.byte 0
_jch_min_16x32_1_ref_frame:
	.word _jch_min_16x32_1_ref
	.byte bank(_jch_min_16x32_1_ref)
	.word jch_min_16x32_1_ref_end - _jch_min_16x32_1_ref
	.byte 0
_rpl_fly_RIGHT_32x32_frame:
	.word _rpl_fly_RIGHT_32x32
	.byte bank(_rpl_fly_RIGHT_32x32)
	.word rpl_fly_RIGHT_32x32_end - _rpl_fly_RIGHT_32x32
	.byte 0
_brstick_RIGHT_32x16_frame:
	.word _brstick_RIGHT_32x16
	.byte bank(_brstick_RIGHT_32x16)
	.word brstick_RIGHT_32x16_end - _brstick_RIGHT_32x16
	.byte 0
_dr_msl_UP_frame:
	.word _dr_msl_UP
	.byte bank(_dr_msl_UP)
	.word dr_msl_UP_end - _dr_msl_UP
	.byte 0
_gigan_idle_LEFT_frame:
	.word _gigan_idle_LEFT
	.byte bank(_gigan_idle_LEFT)
	.word gigan_idle_LEFT_end - _gigan_idle_LEFT
	.byte 0
_tony_idle_RIGHT_frame:
	.word _tony_idle_RIGHT
	.byte bank(_tony_idle_RIGHT)
	.word tony_idle_RIGHT_end - _tony_idle_RIGHT
	.byte 0


	; #1: Y pos, #2: X pos, #3: CHR index, #4: CHR desc

_jch_RIGHT_32x64:
	.word $FFC0, $FFF0, $100, $3180
jch_RIGHT_32x64_end:

_dr_msl_UP_16x64_0:
	.word $00, $FFF8, $110, $3085
dr_msl_UP_16x64_0_end:

_dr_msl_UP_16x64_1_ref:
	.word $00, $FFF8, $112, $3085
dr_msl_UP_16x64_1_ref_end:

_jch_min_16x32_0:
	.word $FFE0, $FFF8, $120, $1080
jch_min_16x32_0_end:

_jch_min_16x32_1_ref:
	.word $FFE0, $FFF8, $122, $1080
jch_min_16x32_1_ref_end:

_rpl_fly_RIGHT_32x32:
	.word $FFF0, $FFF0, $128, $1181
rpl_fly_RIGHT_32x32_end:

_brstick_RIGHT_32x16:
	.word $FFF8, $FFF0, $130, $182
brstick_RIGHT_32x16_end:

_dr_msl_UP:
	.word $00, $FFF8, $134, $85
	.word $10, $FFF8, $136, $85
dr_msl_UP_end:

_gigan_idle_LEFT:
	.word $FFA0, $FFF6, $138, $83
	.word $FFB0, $FFF2, $13A, $83
	.word $FFB0, $02, $13C, $83
	.word $FFB0, $12, $13E, $83
	.word $FFC0, $FFE4, $140, $83
	.word $FFC0, $FFF4, $142, $83
	.word $FFC0, $04, $144, $83
	.word $FFC0, $14, $146, $83
	.word $FFD0, $FFE4, $148, $83
	.word $FFD0, $FFF4, $14A, $83
	.word $FFD0, $04, $14C, $83
	.word $FFD0, $14, $14E, $83
	.word $FFD0, $26, $150, $83
	.word $FFD0, $36, $152, $83
	.word $FFE0, $FFF6, $154, $83
	.word $FFE0, $06, $156, $83
	.word $FFE0, $16, $158, $83
	.word $FFE0, $26, $15A, $83
	.word $FFE0, $36, $15C, $83
	.word $FFF0, $FFF0, $15E, $83
	.word $FFF0, $02, $160, $83
	.word $FFF0, $12, $162, $83
gigan_idle_LEFT_end:

_tony_idle_RIGHT:
	.word $FFA0, $FFF4, $164, $884
	.word $FFB0, $05, $166, $884
	.word $FFB0, $FFF5, $168, $884
	.word $FFB0, $FFE5, $16A, $884
	.word $FFC0, $05, $16C, $884
	.word $FFC0, $FFF5, $16E, $884
	.word $FFC0, $FFE5, $170, $884
	.word $FFD0, $FFFD, $172, $884
	.word $FFD0, $FFED, $174, $884
	.word $FFE0, $FFFF, $176, $884
	.word $FFE0, $FFEC, $178, $884
	.word $FFF0, $02, $17A, $884
	.word $FFF0, $FFF2, $17C, $884
	.word $FFF0, $FFE2, $17E, $884
tony_idle_RIGHT_end:
