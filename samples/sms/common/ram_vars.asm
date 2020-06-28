;###################################################################
;
; Copyright 2019-2020 0x8BitDev ( MIT license )
;
;###################################################################
;
; RAM variables
;

.RAMSECTION "ram_vars" bank 1 slot 1 align 256

; the sprite buffer must be aligned (256?)

SPR_BUFF:
	dsb 256

SPR_NUM_ATTRS:
	byte

VDP_STATE_FLAGS:
	byte

JPAD_STATE:
	dw

player_anm	INSTANCEOF runtime_anm_data

player_jump_max_height:
	db

player_flags:		
	db

player_state:
	db

.ENDS