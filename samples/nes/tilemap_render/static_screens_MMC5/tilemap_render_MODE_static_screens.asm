;############################################################################################################
;
; Copyright 2018-2019 0x8BitDev ( MIT license )
;
;############################################################################################################
; DESC: Static screens switching render which uses a level topology data.
;
;############################################################################################################

; Public procs:
;
;	init_screen	- X-low/Y-high of a screen data ptr
;	update_jpad
;	move_left
;	move_right
;	move_up
;	move_down
;	update_PPU	- disables rendering(!)
;	update_ExRAM
;

; SUPPORTED OPTIONS:
;
;	MAP_FLAG_RLE
;	MAP_FLAG_MODE_STATIC_SCREENS
;	MAP_FLAG_LAYOUT_ADJACENT_SCREENS
;	MAP_FLAG_LAYOUT_ADJACENT_SCR_INDS
;	MAP_FLAG_MARKS
;	MAP_FLAG_ATTRS_PER_BLOCK
;	MAP_FLAG_ATTRS_PER_CHR
;

CHR_bankswitching = mmc5_CHR_bankswitching	; switch CHR bank / A - bank index

.include "../../common/tilemap_render_UTILS.asm"


SUPPORTED_MAP_FLAGS = MAP_FLAG_MODE_STATIC_SCREENS

	.IF MAP_CHECK(MAP_FLAG_LAYOUT_MATRIX)
	.fatal "LAYOUT MATRIX ISN't SUPPORTED BY THIS SAMPLE !"
	.ENDIF

	.IF MAP_CHECK(SUPPORTED_MAP_FLAGS) <> SUPPORTED_MAP_FLAGS
	.fatal "UNSUPPORTED MODE DETECTED !"
	.ENDIF

	.scope TR_ss

.segment "ZP"

; INIT DATA: BEG --------
tr_ppu_scr:	.res 2	; tilemap_PPUScr
tr_palettes:	.res 2	; tilemap_Plts

		.IF MAP_CHECK(::MAP_FLAG_ATTRS_PER_CHR)
tr_attrs_scr:	.res 2	; tilemap_AttrsScr ( MMC5 Ex RAM )
		.ENDIF	; MAP_FLAG_ATTRS_PER_CHR
; INIT DATA: END --------

	.scope inner_vars
_tr_update_flags:	.res 1	; 0 - need update PPU, 1 - need update MMC5 Ex RAM
	.endscope

TR_FLAGS_UPDATE_PPU	=	1
TR_FLAGS_UPDATE_EX_RAM	=	2


.segment "CODE"

; IN:	X - screen data low byte
;	Y - screen data high byte
init_screen:

	stx TR_utils::tr_curr_scr
	sty TR_utils::tr_curr_scr + 1

	lda #TR_FLAGS_UPDATE_PPU	; update PPU

	.IF MAP_CHECK(::MAP_FLAG_ATTRS_PER_CHR)
	ora #TR_FLAGS_UPDATE_EX_RAM
	.ENDIF	; MAP_FLAG_ATTRS_PER_CHR

	sta inner_vars::_tr_update_flags

	rts

update_jpad:

	jsr jpad1_read_state

	jpad1_check_btn JPAD_BTN_LEFT
	beq @upd_check_right_btn

	jsr move_left

@upd_check_right_btn:

	jpad1_check_btn JPAD_BTN_RIGHT
	beq @upd_check_up_btn

	jsr move_right

@upd_check_up_btn:

	jpad1_check_btn JPAD_BTN_UP
	beq @upd_check_down_btn

	jsr move_up

@upd_check_down_btn:

	jpad1_check_btn JPAD_BTN_DOWN
	beq @upd_exit

	jsr move_down

@upd_exit:

	rts

move_left:

	check_left_screen
	bcs init_screen

	rts

move_up:
	check_up_screen
	bcs init_screen

	rts

move_right:

	check_right_screen
	bcs init_screen

	rts

move_down:

	check_down_screen
	bcs init_screen

	rts

update_PPU:

	lda inner_vars::_tr_update_flags
	and #TR_FLAGS_UPDATE_PPU
	bne @cont

	rts

@cont:

	set_ppu_1_inc

	lda inner_vars::_tr_update_flags
	eor #TR_FLAGS_UPDATE_PPU
	sta inner_vars::_tr_update_flags

	ldy #$00
	lda (<TR_utils::tr_curr_scr), y		; get chr_id

	.IF MAP_CHECK(::MAP_FLAG_ATTRS_PER_BLOCK)
	; switch CHR bank (MMC5)
	jsr CHR_bankswitching
	.ENDIF	; MAP_FLAG_ATTRS_PER_CHR

	; load palette
	load_palette_16 tr_palettes, data_addr, data_size, $00

	jsr render_off

	; skip marks
	.IF MAP_CHECK(::MAP_FLAG_MARKS)
	ldy #$02
	.ELSE
	ldy #$01
	.ENDIF ; MAP_FLAG_MARKS

	; decompress gfx to PPU
	lda tr_ppu_scr
	sta _tmp_val
	lda tr_ppu_scr + 1
	sta _tmp_val + 1

	; get PPU screen offset
	lda (<TR_utils::tr_curr_scr), y
	tax
	iny 
	lda (<TR_utils::tr_curr_scr), y
	tay
	add_word_to_xy _tmp_val

	.IF MAP_CHECK(::MAP_FLAG_RLE)
	lda #$00
	sta data_addr
	lda #$20
	sta data_addr + 1
	set_ppu_addr

	jsr unrle_to_PPU
	.ELSE
	; copy to PPU
	stx data_addr
	sty data_addr + 1
	ldx #$00
	ldy #$20

	.IF MAP_CHECK(::MAP_FLAG_ATTRS_PER_BLOCK)
	lda #(ScrGfxDataSize / 256)

	jsr ppu_load_data_opt
	.ELSE
	load_data_ptr ScrGfxDataSize, data_size

	jsr ppu_load_data
	.ENDIF	; MAP_FLAG_ATTRS_PER_BLOCK
	.ENDIF	; MAP_FLAG_RLE

	rts

	.IF MAP_CHECK(::MAP_FLAG_ATTRS_PER_CHR)
update_ExRAM:
	lda inner_vars::_tr_update_flags
	and #TR_FLAGS_UPDATE_EX_RAM
	bne @cont

	rts

@cont:

	lda inner_vars::_tr_update_flags
	eor #TR_FLAGS_UPDATE_EX_RAM
	sta inner_vars::_tr_update_flags

	; fill ExRAM via PPU $2000
	; Extended RAM mode: 0 - Use as extra nametable
	lda #$00
	sta $5104
	; Nametable mapping: #AA - Single-screen ExRAM
	lda #$aa
	sta $5105

	; skip marks
	.IF MAP_CHECK(::MAP_FLAG_MARKS)
	.IF MAP_CHECK(::MAP_FLAG_RLE)
	ldy #$04
	.ELSE
	ldy #$02
	.ENDIF	; MAP_FLAG_RLE
	.ELSE
	.IF MAP_CHECK(::MAP_FLAG_RLE)
	ldy #$03
	.ELSE
	ldy #$01
	.ENDIF	; MAP_FLAG_RLE
	.ENDIF	; MAP_FLAG_MARKS

	; decompress gfx to PPU
	lda tr_attrs_scr
	sta _tmp_val
	lda tr_attrs_scr + 1
	sta _tmp_val + 1

	; get ATTRs screen offset
	lda (<TR_utils::tr_curr_scr), y
	tax
	iny 
	lda (<TR_utils::tr_curr_scr), y
	tay
	add_word_to_xy _tmp_val

	.IF MAP_CHECK(::MAP_FLAG_RLE)
	lda #$00
	sta data_addr
	lda #$20
	sta data_addr + 1
	set_ppu_addr

	jsr unrle_to_PPU

	.ELSE
	; copy to Ex RAM
	stx data_addr
	sty data_addr + 1
	load_data_ptr ScrGfxDataSize, data_size
	ldx #$00
	ldy #$20

	jsr ppu_load_data

	.ENDIF	; MAP_FLAG_RLE

	; Extended RAM mode: 1 - Use as extended attribute data (can also be used as extended nametable)
	lda #$01
	sta $5104
	; Nametable mapping: 0 - Single-screen CIRAM 0
	lda #$00
	sta $5105

	rts
	.ENDIF	; MAP_FLAG_ATTRS_PER_CHR

	.endscope