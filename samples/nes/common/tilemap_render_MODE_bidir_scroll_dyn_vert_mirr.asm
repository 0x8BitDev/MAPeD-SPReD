;############################################################################################################
;
; Copyright 2018-2020 0x8BitDev ( MIT license )
;
;############################################################################################################
; DESC: Bidirectional tilemap scroller with dynamic mirroring ( fullscreen PAL without attribute\tile
;	glitches ) and dynamic CHR\palette switching. So it's possible to use more than 1 CHR bank in a game 
;	map with transition screen(s). It can be used with 4x4/2x2 tiles. The column data order is required.
;	Uses attributes per block (2x2 tile).
;
; LIMITATIONS:
;
;	- The mirroring can be changed dynamically at each even screen from the start one.
;
; P.S.: An example of a real NES game that used dynamic mirroring is "Little Samson" © 1992 Taito / Takeru
;
; P.P.S.: Also these sources contain a bidirectional scroller implementation with vertical mirroring
;	 ( fullscreen NTSC without attribute\tile glitches ). See supported options below for details.
;
;############################################################################################################

; Public procs:
;
;	init_draw 	- X\Y start screen ptr
; 	move_left
; 	move_right
; 	move_up
; 	move_down
; 	update_jpad
; 	update_scroll_reg
;	update_nametable
;

; SUPPORTED OPTIONS:
;
; .define TR_DYNAMIC_MIRRORING		1 / 0 - TR_MIRRORING_VERTICAL
; .define TR_MIRR_VERT_HALF_ATTR	1 - half attributes (16x16) (min attr glitches), 0 - full attributes (32x32) (max attr glitches)
;
;	MAP_FLAG_TILES2X2
;	MAP_FLAG_TILES4X4
;	MAP_FLAG_DIR_COLUMNS
;	MAP_FLAG_MODE_BIDIR_SCROLL
;	MAP_FLAG_ATTRS_PER_BLOCK
;

.define TR_MIRRORING_VERTICAL	!TR_DYNAMIC_MIRRORING

; routines aliases
CHR_bankswitching = mmc1_chr_bank1_write	; switch CHR bank \ A - bank index

	.IF TR_DYNAMIC_MIRRORING
.define	TR_MIRR_VERT_HALF_ATTR	0

mirroring_vertical 	= mmc1_mirroring_vertical
mirroring_horizontal	= mmc1_mirroring_horizontal
	.ELSE
.define	TR_MIRR_VERT_HALF_ATTR	1
	.ENDIF	;TR_DYNAMIC_MIRRORING


.include "../../common/tilemap_render_UTILS.asm"


SUPPORTED_MAP_FLAGS	= MAP_FLAG_DIR_COLUMNS | MAP_FLAG_MODE_BIDIR_SCROLL | MAP_FLAG_ATTRS_PER_BLOCK

	.IF ( MAP_DATA_MAGIC & SUPPORTED_MAP_FLAGS ) <> SUPPORTED_MAP_FLAGS
	.fatal "UNSUPPORTED MAP FLAGS DETECTED ! TRY TO RE-EXPORT THE DATA WITH THE CORRECT OPTIONS !"
	.ENDIF

	.IF ( MAP_DATA_MAGIC & MAP_FLAG_RLE ) = MAP_FLAG_RLE
	.fatal "RLE COMPRESSION DOESN'T SUPPORTED !"
	.ENDIF

	.IF ( MAP_DATA_MAGIC & MAP_FLAG_MARKS ) = MAP_FLAG_MARKS
	.fatal "THE SAMPLE DOESN'T SUPPORT THE SCREEN MARKS !"
	.ENDIF

	.IF ( MAP_DATA_MAGIC & MAP_FLAG_LAYOUT_MATRIX ) = MAP_FLAG_LAYOUT_MATRIX
	.fatal "THE SAMPLE DOESN'T SUPPORT THE MATRIX LAYOUT !"
	.ENDIF

	.scope TR_sts

TR_SCROLL_STEP	= 2 ; 1 / 2 / 4


.segment "ZP"

; INIT DATA: BEG --------
tr_blocks_props_offs:
		.res 2	; tilemap_BlocksPropsOffs
tr_blocks:	.res 2	; tilemap_Blocks
tr_palettes:	.res 2	; tilemap_Plts

tr_tiles_scr:	.res 2	; tilemap_TileScr - 64 bytes per screen

	.IF ::TR_DATA_TILES4X4
tr_tiles_offs:	.res 2	; tilemap_TilesOffs - offset in the tilemap_Tiles array
tr_tiles:	.res 2	; tilemap_Tiles
tr_attrs:	.res 2	; tilemap_Attrs
	.ELSE
tr_attrs_scr:	.res 2	; tilemap_AttrsScr
	.ENDIF	;TR_DATA_TILES4X4
; INIT DATA: END --------

	.scope inner_vars

_tr_pos_x:	.res 2
_tr_pos_y:	.res 2

_nametable:	.res 1

TR_UPD_FLAG_SCROLL			= %00000001
; for correct bidirectional scrolling
TR_UPD_FLAG_IGNORE_LEFT_OVERFLOW	= %00000010
TR_UPD_FLAG_IGNORE_RIGHT_OVERFLOW	= %00000100
TR_UPD_FLAG_IGNORE_UP_OVERFLOW		= %00001000
TR_UPD_FLAG_IGNORE_DOWN_OVERFLOW	= %00010000

TR_UPD_FLAG_DIR_LEFT_RIGHT		= %00100000
TR_UPD_FLAG_DIR_UP_DOWN			= %01000000
TR_UPD_FLAG_DIR_SELECT			= %10000000

TR_UPD_FLAG_DIR_MASK	= TR_UPD_FLAG_DIR_LEFT_RIGHT | TR_UPD_FLAG_DIR_UP_DOWN

_tr_upd_flags:	.res 1

; data ptrs with offset by CHR id
;-----------------------------------
_tr_blocks:	.res 2
	.IF ::TR_DATA_TILES4X4
_tr_tiles:	.res 2
_tr_attrs:	.res 2
	.ENDIF	;TR_DATA_TILES4X4

_tr_last_chr_id:
		.res 1

_drw_rowcol_inds_offset:
		.res 1	; offsets in the _drw_rowcol_inds_tbl array

_loop_cntr:	.res 1


	.IF TR_MIRRORING_VERTICAL

_tr_ext_upd_flags:	.res 1

TR_UPD_FLAG_FORCED_ATTRS	= %00100000	; forced drawing of attributes
TR_UPD_FLAG_FORCED_TOP		= %01000000	; forced drawing of top line
TR_UPD_FLAG_FORCED_BOTTOM	= %10000000	; forced drawing of bottom line

	.IF TR_MIRR_VERT_HALF_ATTR
TR_EXTRA_FLAGS_DOWN_HALF_ATTR	= %10000000
	.ENDIF ;TR_MIRR_VERT_HALF_ATTR

	.ENDIF	;TR_MIRRORING_VERTICAL

	.endscope

.segment "CODE"

	.IF ::TR_DATA_TILES4X4

_drw_rowcol_inds_tbl:	.byte 0, 1, 0, 1, 0, 1, 2, 3, 2, 3, 0, 1, 2, 3, 2, 3
			.byte 0, 2, 0, 2, 0, 2, 1, 3, 1, 3, 0, 2, 1, 3, 1, 3

	.ELSE	; TR_DATA_TILES4X4

_drw_rowcol_inds_tbl:	.byte 0, 1, 2, 3
			.byte 0, 2, 1, 3

_tiles_col_offset:	.byte 0*SCR_HTILES_CNT, 1*SCR_HTILES_CNT, 2*SCR_HTILES_CNT, 3*SCR_HTILES_CNT
			.byte 4*SCR_HTILES_CNT, 5*SCR_HTILES_CNT, 6*SCR_HTILES_CNT, 7*SCR_HTILES_CNT
			.byte 8*SCR_HTILES_CNT, 9*SCR_HTILES_CNT, 10*SCR_HTILES_CNT, 11*SCR_HTILES_CNT
			.byte 12*SCR_HTILES_CNT, 13*SCR_HTILES_CNT, 14*SCR_HTILES_CNT, 15*SCR_HTILES_CNT

	.ENDIF ;TR_DATA_TILES4X4

; MACROSES

	.IF TR_MIRRORING_VERTICAL

	.macro tr_set_draw_flag_forced_top
	lda inner_vars::_tr_ext_upd_flags
	and #<~(inner_vars::TR_UPD_FLAG_FORCED_BOTTOM | inner_vars::TR_UPD_FLAG_FORCED_ATTRS)
	ora #inner_vars::TR_UPD_FLAG_FORCED_TOP
	sta inner_vars::_tr_ext_upd_flags
	.endmacro

	.macro tr_set_draw_flag_forced_bottom
	lda inner_vars::_tr_ext_upd_flags
	and #<~(inner_vars::TR_UPD_FLAG_FORCED_TOP | inner_vars::TR_UPD_FLAG_FORCED_ATTRS)
	ora #inner_vars::TR_UPD_FLAG_FORCED_BOTTOM
	sta inner_vars::_tr_ext_upd_flags
	.endmacro

	.ENDIF	;TR_MIRRORING_VERTICAL

	; OUT: A - screen index
	.macro get_screen_index _ovflw_flag, _check_scr_dir_proc, 
	.local @cont0
	.local @cont1
	lda inner_vars::_tr_upd_flags
	and #_ovflw_flag
	beq @cont0

	get_screen_index_offset

	lda (<TR_utils::tr_curr_scr), y	; A - screen index
	jmp @cont1
@cont0:
	; get screen id
	_check_scr_dir_proc
	jmp_bcc @exit

	jsr _get_screen_index		; A - screen index
@cont1:			
	.endmacro

	; jmp_beq and jmp_bcc are macroses to fix out of range error

	.macro jmp_beq _label
	.local @cont
	bne @cont
	jmp _label
@cont:
	.endmacro			

	.macro jmp_bcc _label
	.local @cont
	bcs @cont
	jmp _label
@cont:
	.endmacro			

	.macro check_direction _dir_flag
	.local @cont
	lda inner_vars::_tr_upd_flags
	and #_dir_flag
	bne @cont

	lda inner_vars::_tr_upd_flags
	and #inner_vars::TR_UPD_FLAG_DIR_SELECT
	jmp_beq @exit
@cont:
	.endmacro

	.macro check_direction_select _dir_flag, _pos, FIX_MOVE_UP, FIX_MOVE_DOWN
	lda inner_vars::_tr_upd_flags
	and #inner_vars::TR_UPD_FLAG_DIR_SELECT
	jmp_beq @exit

	; select state
	upd_flag_reset inner_vars::TR_UPD_FLAG_DIR_SELECT
	upd_flag_set _dir_flag

	.IF FIX_MOVE_UP
	lda #$f0
	sta _pos

	lda #$10
	sta _pos + 1

	upd_flag_set inner_vars::TR_UPD_FLAG_IGNORE_DOWN_OVERFLOW

	.ENDIF	;FIX_MOVE_UP

	.IF FIX_MOVE_DOWN
	lda #$00
	sta _pos
	sta _pos + 1

	upd_flag_set inner_vars::TR_UPD_FLAG_IGNORE_UP_OVERFLOW

	.ENDIF	;FIX_MOVE_DOWN

	.endmacro

	; IN: A - position
	.macro check_direction_horiz_overflow_pos
	.local @cont1
	bne @cont1

	upd_flag_reset inner_vars::TR_UPD_FLAG_DIR_MASK
	upd_flag_set inner_vars::TR_UPD_FLAG_DIR_SELECT
@cont1:
	.endmacro

	; IN: A - position
	.macro check_direction_vert_overflow_pos
	.local @cont1
	bcc @cont1

	lda #$00
	sta inner_vars::_tr_pos_y
	sta inner_vars::_tr_pos_y + 1

	upd_flag_reset inner_vars::TR_UPD_FLAG_DIR_MASK
	upd_flag_set inner_vars::TR_UPD_FLAG_DIR_SELECT
@cont1:
	.endmacro

	; a = ( a / 32 ) * 8
	.macro div32_mul8
	lsr a
	lsr a
	and #%11111000
	.endmacro	

	.macro get_screen_index_offset
	.IF MAP_CHECK(::MAP_FLAG_MARKS)
	ldy #02
	.ELSE
	ldy #01
	.ENDIF	; MAP_FLAG_MARKS
	.endmacro
	
	.macro save_curr_scr_XY
	stx TR_utils::tr_curr_scr
	sty TR_utils::tr_curr_scr + 1
	.endmacro

	; OUT: A - chr id
	.macro get_curr_scr_chr_id
	ldy #$00
	lda (<TR_utils::tr_curr_scr), y
	.endmacro			

	.macro upd_flag_reset _flag
	lda inner_vars::_tr_upd_flags
	and #<~_flag
	sta inner_vars::_tr_upd_flags
	.endmacro

	.macro upd_flag_set _flag
	lda inner_vars::_tr_upd_flags
	ora #_flag
	sta inner_vars::_tr_upd_flags
	.endmacro

	; IN: A - data index
	.macro calc_64bytes_data_offset	_arr_word, _out_word
	sta _out_word
	lda #$00
	sta _out_word + 1

	mul64_word _out_word

	ldx _arr_word
	ldy _arr_word + 1

	add_xy_to_word _out_word			; _out_word - screen tiles\attrs data ptr
	.endmacro

	; IN: A - data index
	; A*240 = A*256 - A*16
	.macro calc_240bytes_data_offset _arr_word, _out_word
	; XY = A*256
	tay
	ldx #$00

	; _out_word = A*16
	sta _out_word
	stx _out_word + 1
	mul16_word _out_word

	sub_xy_from_word _out_word

	ldx _arr_word
	ldy _arr_word + 1

	add_xy_to_word _out_word			; _out_word - screen tiles\attrs data ptr
	.endmacro

	; IN: A - CHR id
	.macro calc_data_offset_by_CHR_id _offs_arr, _data_arr, _out_word, DIV_OFFS_BY_4
	lda inner_vars::_tr_last_chr_id
	asl a
	tay
	lda (<_offs_arr), y
	sta _out_word
	iny 
	lda (<_offs_arr), y
	sta _out_word + 1

	.IF DIV_OFFS_BY_4
	div4_word _out_word
	.ENDIF

	add_word_to_word _data_arr, _out_word
	.endmacro


	; IN: X\Y - start screen ptr
init_draw:	
	save_curr_scr_XY

	; get CHR id
	get_curr_scr_chr_id

	sta inner_vars::_tr_last_chr_id

	; apply CHR bank
	jsr CHR_bankswitching

	; apply palette
	lda inner_vars::_tr_last_chr_id
	load_palette_16 tr_palettes, data_addr, data_size, $00

	; calc tiles data offset by CHR bank index
	.IF ::TR_DATA_TILES4X4
	calc_data_offset_by_CHR_id tr_tiles_offs, tr_tiles, inner_vars::_tr_tiles, 0
	calc_data_offset_by_CHR_id tr_tiles_offs, tr_attrs, inner_vars::_tr_attrs, 1
	.ENDIF	;TR_DATA_TILES4X4

	; calc blocks data offset by CHR bank index
	calc_data_offset_by_CHR_id tr_blocks_props_offs, tr_blocks, inner_vars::_tr_blocks, 0

	; data init
	lda #$00
	sta inner_vars::_tr_pos_x
	sta inner_vars::_tr_pos_x + 1
	sta inner_vars::_tr_pos_y
	sta inner_vars::_tr_pos_y + 1

	lda #inner_vars::TR_UPD_FLAG_DIR_SELECT
	sta inner_vars::_tr_upd_flags

	.IF TR_MIRRORING_VERTICAL
	lda #$00
	sta inner_vars::_tr_ext_upd_flags
	.ENDIF

	lda #$02
	sta inner_vars::_nametable

	jsr TR_utils::buff_reset

	set_ppu_1_inc

; draw tiles

	; get screen index
	get_screen_index_offset

	lda (<TR_utils::tr_curr_scr), y	; A - screen data index

	.IF !::TR_DATA_TILES4X4
	pha
	.ENDIF	;TR_DATA_TILES4X4

	; get screen tiles data ptr by index

	.IF ::TR_DATA_TILES4X4
	calc_64bytes_data_offset tr_tiles_scr, _tmp_val
	push_word _tmp_val
	.ELSE
	calc_240bytes_data_offset tr_tiles_scr, _tmp_val
	.ENDIF	; TR_DATA_TILES4X4

	lda inner_vars::_nametable
	jsr ppu_calc_nametable_addr		; data_addr - PPU nametable address

	push_word data_addr

	ldx #SCR_WTILES_CNT
	jsr TR_utils::draw_screen_nametable

; draw attrs

	pop_word data_addr

	; add $03c0 to PPU nametable address to get attributes address
	ldx #$c0
	ldy #$03
	add_xy_to_word data_addr

	.IF ::TR_DATA_TILES4X4
	pop_word _tmp_val
	.ELSE
	pla
	calc_64bytes_data_offset tr_attrs_scr, _tmp_val
	.ENDIF ;TR_DATA_TILES4X4

	ldx #SCR_WATTRS_CNT

	jmp TR_utils::draw_screen_attrs

	; IN: A - chr id
_apply_CHR_bank_and_palette:

	cmp inner_vars::_tr_last_chr_id
	jmp_beq @exit

	sta inner_vars::_tr_last_chr_id

	; calc tiles data offset by CHR bank index
	.IF ::TR_DATA_TILES4X4
	calc_data_offset_by_CHR_id tr_tiles_offs, tr_tiles, inner_vars::_tr_tiles, 0
	calc_data_offset_by_CHR_id tr_tiles_offs, tr_attrs, inner_vars::_tr_attrs, 1
	.ENDIF	;TR_DATA_TILES4X4

	; calc blocks data offset by CHR bank index
	calc_data_offset_by_CHR_id tr_blocks_props_offs, tr_blocks, inner_vars::_tr_blocks, 0

	lda inner_vars::_tr_last_chr_id
	jsr CHR_bankswitching

	; apply palette

	; make data header
	lda #$00
	sta data_addr
	lda #$3f
	sta data_addr + 1

	lda #TR_utils::TR_BUFF_FLAGS_INC1
	sta TR_utils::tr_data_flags
	lda #$10

	jsr TR_utils::buff_push_hdr

	lda inner_vars::_tr_last_chr_id

	; copy palette data
	sta data_addr
	lda #$00
	sta data_addr + 1
	mul16_word data_addr
	ldx tr_palettes
	ldy tr_palettes + 1
	add_xy_to_word data_addr

	ldx #$10
	ldy #$00
@loop:
	push_y
	lda (<data_addr), y
	jsr TR_utils::buff_push_data
	pop_y
	iny
	dex
	bne @loop

@exit:
	rts

move_left:
	check_direction inner_vars::TR_UPD_FLAG_DIR_LEFT_RIGHT

	lda inner_vars::_tr_pos_x + 1
	beq @min_check

	clc
	adc #TR_SCROLL_STEP
	bcc @cont1

	; reset active IGNORE_LEFT_OVERFLOW flag and jump to @cont0
	lda inner_vars::_tr_upd_flags
	and #inner_vars::TR_UPD_FLAG_IGNORE_LEFT_OVERFLOW
	beq @cont0

	upd_flag_reset inner_vars::TR_UPD_FLAG_IGNORE_LEFT_OVERFLOW
	jmp @cont1

@cont0:
	; switch screen
	check_left_screen				; skip screen which already at screen

	jmp_bcc @exit					; no screen

	save_curr_scr_XY

	upd_flag_reset inner_vars::TR_UPD_FLAG_IGNORE_RIGHT_OVERFLOW

	jmp @cont1

@min_check:

	check_left_screen				; get the next screen

	bcc @exit					; no screen

	check_direction_select inner_vars::TR_UPD_FLAG_DIR_LEFT_RIGHT, inner_vars::_tr_pos_x + 1, 0, 0

	; get CHR id
	stx data_addr
	sty data_addr + 1
	ldy #$00
	lda (<data_addr), y

	jsr _apply_CHR_bank_and_palette

	lda inner_vars::_nametable
	eor #01
	sta inner_vars::_nametable

	.IF TR_DYNAMIC_MIRRORING
	jsr mirroring_vertical
	.ENDIF	;TR_DYNAMIC_MIRRORING

@cont1:
	ldx #$00

	lda inner_vars::_tr_pos_x
	and #%00000111

	bne @cont2

	inx						; need to draw a column

@cont2:
	lda inner_vars::_tr_pos_x
	sec
	sbc #TR_SCROLL_STEP
	sta inner_vars::_tr_pos_x

	bcs @cont3
	upd_flag_set inner_vars::TR_UPD_FLAG_IGNORE_RIGHT_OVERFLOW

@cont3:

	lda inner_vars::_tr_pos_x + 1
	clc
	adc #TR_SCROLL_STEP
	sta inner_vars::_tr_pos_x + 1

	check_direction_horiz_overflow_pos

	txa
	beq @exit

	jmp _tr_drw_left_col
@exit:
	rts

move_right:
	check_direction inner_vars::TR_UPD_FLAG_DIR_LEFT_RIGHT

	lda inner_vars::_tr_pos_x
	beq @min_check

	clc
	adc #TR_SCROLL_STEP
	jmp_bcc @cont1

	; reset active IGNORE_RIGHT_OVERFLOW flag and jump to @cont0
	lda inner_vars::_tr_upd_flags
	and #inner_vars::TR_UPD_FLAG_IGNORE_RIGHT_OVERFLOW
	beq @cont0

	upd_flag_reset inner_vars::TR_UPD_FLAG_IGNORE_RIGHT_OVERFLOW

	lda inner_vars::_nametable
	eor #01
	sta inner_vars::_nametable

	jmp @cont1

@cont0:
	; switch screen
	check_right_screen				; skip screen which already at screen

	jmp_bcc @exit					; no screen

	save_curr_scr_XY

	upd_flag_reset inner_vars::TR_UPD_FLAG_IGNORE_LEFT_OVERFLOW

	lda inner_vars::_nametable
	eor #01
	sta inner_vars::_nametable

	jmp @cont1

@min_check:
	check_right_screen				; get the next screen

	bcc @exit

	check_direction_select inner_vars::TR_UPD_FLAG_DIR_LEFT_RIGHT, inner_vars::_tr_pos_x, 0, 0

	; get CHR id
	stx data_addr
	sty data_addr + 1
	ldy #$00
	lda (<data_addr), y

	jsr _apply_CHR_bank_and_palette

	.IF TR_DYNAMIC_MIRRORING
	jsr mirroring_vertical
	.ENDIF	;TR_DYNAMIC_MIRRORING

@cont1:
	ldx #$00

	lda inner_vars::_tr_pos_x
	and #%00000111

	bne @cont2

	inx						; need to draw a column

@cont2:
	lda inner_vars::_tr_pos_x
	clc
	adc #TR_SCROLL_STEP
	sta inner_vars::_tr_pos_x

	check_direction_horiz_overflow_pos

	lda inner_vars::_tr_pos_x + 1
	sec
	sbc #TR_SCROLL_STEP
	sta inner_vars::_tr_pos_x + 1

	bcs @cont3
	upd_flag_set inner_vars::TR_UPD_FLAG_IGNORE_LEFT_OVERFLOW

@cont3:
	txa
	beq @exit

	jmp _tr_drw_right_col
@exit:
	rts

move_up:
	.IF TR_MIRRORING_VERTICAL
	.IF !TR_MIRR_VERT_HALF_ATTR

	lda inner_vars::_tr_ext_upd_flags
	and #inner_vars::TR_UPD_FLAG_FORCED_TOP
	beq @cont0

	lda inner_vars::_tr_pos_y
	beq @cont0

	lda #inner_vars::TR_UPD_FLAG_FORCED_ATTRS
	sta inner_vars::_tr_ext_upd_flags

	jsr _tr_drw_up_row

@cont0:
	tr_set_draw_flag_forced_bottom

	.ENDIF ;!TR_MIRR_VERT_HALF_ATTR
	.ENDIF	;TR_MIRRORING_VERTICAL

	check_direction inner_vars::TR_UPD_FLAG_DIR_UP_DOWN

	lda inner_vars::_tr_pos_y + 1

	beq @min_check

	clc
	adc #TR_SCROLL_STEP

	jmp_bcc @cont2

	; reset active IGNORE_UP_OVERFLOW flag and jump to @cont
	lda inner_vars::_tr_upd_flags
	and #inner_vars::TR_UPD_FLAG_IGNORE_UP_OVERFLOW
	beq @cont1

	upd_flag_reset inner_vars::TR_UPD_FLAG_IGNORE_UP_OVERFLOW

	jmp @cont2

@cont1:
	; switch screen
	check_up_screen					; skip screen which already at screen

	jmp_bcc @exit					; no screen

	save_curr_scr_XY

	upd_flag_reset inner_vars::TR_UPD_FLAG_IGNORE_DOWN_OVERFLOW

	jmp @cont2

@min_check:

	check_up_screen					; get the next screen

	jmp_bcc @exit					; no screen

	check_direction_select inner_vars::TR_UPD_FLAG_DIR_UP_DOWN, inner_vars::_tr_pos_y, 1, 0

	; get CHR id
	stx data_addr
	sty data_addr + 1
	ldy #$00
	lda (<data_addr), y

	jsr _apply_CHR_bank_and_palette

	lda inner_vars::_nametable
	eor #02
	sta inner_vars::_nametable

	.IF TR_DYNAMIC_MIRRORING
	jsr mirroring_horizontal
	.ENDIF	;TR_DYNAMIC_MIRRORING

@cont2:
	ldx #$00

	lda inner_vars::_tr_pos_y
	and #%00000111

	bne @cont3

	inx						; need to draw a row

@cont3:
	lda inner_vars::_tr_pos_y
	sec
	sbc #TR_SCROLL_STEP
	sta inner_vars::_tr_pos_y

	lda inner_vars::_tr_pos_y + 1
	clc
	adc #TR_SCROLL_STEP
	sta inner_vars::_tr_pos_y + 1

	check_direction_vert_overflow_pos

	txa
	beq @exit

	jmp _tr_drw_up_row

@exit:
	rts

	.IF TR_MIRRORING_VERTICAL
	.IF ::TR_DATA_TILES4X4

_tr_forced_drw_down_row_attrs:

	get_screen_index_offset
	lda (<TR_utils::tr_curr_scr), y	; A - screen index

	pha

	calc_64bytes_data_offset tr_tiles_scr, _tmp_val

	lda inner_vars::_tr_pos_y
	lsr a
	lsr a
	lsr a
	lsr a
	lsr a

	tax	
	ldy #$00
	add_xy_to_word _tmp_val

	pla

	tay
	jmp _tr_drw_down_row_attrs

	.ENDIF ;TR_MIRRORING_VERTICAL
	.ENDIF ;TR_DATA_TILES4X4

move_down:
	.IF TR_MIRRORING_VERTICAL
	.IF !TR_MIRR_VERT_HALF_ATTR

	lda inner_vars::_tr_ext_upd_flags
	and #inner_vars::TR_UPD_FLAG_FORCED_BOTTOM
	beq @cont0

	lda #inner_vars::TR_UPD_FLAG_FORCED_ATTRS
	sta inner_vars::_tr_ext_upd_flags

	jsr _tr_drw_down_row

@cont0:
	tr_set_draw_flag_forced_top

	.ENDIF ;!TR_MIRR_VERT_HALF_ATTR
	.ENDIF ;TR_MIRRORING_VERTICAL

	check_direction inner_vars::TR_UPD_FLAG_DIR_UP_DOWN

	lda inner_vars::_tr_pos_y
	beq @min_check

	clc
	adc #TR_SCROLL_STEP

	sec
	sbc #$f0
	jmp_bcc @cont2

	.IF TR_MIRR_VERT_HALF_ATTR
	lda #$00
	sta inner_vars::_tr_pos_y
	jsr _tr_drw_down_row				; draw the last row of the current screen

	lda #( $f0 - TR_SCROLL_STEP )
	sta inner_vars::_tr_pos_y
	.ENDIF ;TR_MIRR_VERT_HALF_ATTR

	; reset active IGNORE_DOWN_OVERFLOW flag and jump to @cont1
	lda inner_vars::_tr_upd_flags
	and #inner_vars::TR_UPD_FLAG_IGNORE_DOWN_OVERFLOW
	beq @cont1

	upd_flag_reset inner_vars::TR_UPD_FLAG_IGNORE_DOWN_OVERFLOW

	lda inner_vars::_nametable
	eor #02
	sta inner_vars::_nametable

	; shift attrs drawing on 2 CHRs (bottom block fix)
	.IF TR_MIRRORING_VERTICAL
	.IF !TR_MIRR_VERT_HALF_ATTR
	.IF ::TR_DATA_TILES4X4
	jsr _tr_forced_drw_down_row_attrs
	.ENDIF ;TR_DATA_TILES4X4
	.ENDIF ;!TR_MIRR_VERT_HALF_ATTR
	.ENDIF ;TR_MIRRORING_VERTICAL

	jmp @cont2

@cont1:
	; switch screen
	check_down_screen				; skip screen which already at screen

	jmp_bcc @exit					; no screen

	save_curr_scr_XY

	upd_flag_reset inner_vars::TR_UPD_FLAG_IGNORE_UP_OVERFLOW

	lda inner_vars::_nametable
	eor #02
	sta inner_vars::_nametable

	; shift attrs drawing on 2 CHRs (bottom block fix)
	.IF TR_MIRRORING_VERTICAL
	.IF !TR_MIRR_VERT_HALF_ATTR
	.IF ::TR_DATA_TILES4X4
	jsr _tr_forced_drw_down_row_attrs
	.ENDIF ;TR_DATA_TILES4X4
	.ENDIF ;!TR_MIRR_VERT_HALF_ATTR
	.ENDIF ;TR_MIRRORING_VERTICAL

	jmp @cont2

@min_check:

	check_down_screen				; get the next screen

	jmp_bcc @exit					; no screen

	check_direction_select inner_vars::TR_UPD_FLAG_DIR_UP_DOWN, inner_vars::_tr_pos_y, 0, 1

	; get CHR id
	stx data_addr
	sty data_addr + 1
	ldy #$00
	lda (<data_addr), y

	jsr _apply_CHR_bank_and_palette

	.IF TR_DYNAMIC_MIRRORING
	jsr mirroring_horizontal
	.ENDIF	;TR_DYNAMIC_MIRRORING

@cont2:

	ldx #$00

	lda inner_vars::_tr_pos_y

	.IF TR_MIRR_VERT_HALF_ATTR
	and #%00001000
	beq @cont3
	.ELSE
	and #%00000111
	bne @cont3
	.ENDIF ;TR_MIRR_VERT_HALF_ATTR

	inx						; need to draw a row

@cont3:
	lda inner_vars::_tr_pos_y + 1
	sec
	sbc #TR_SCROLL_STEP
	sta inner_vars::_tr_pos_y + 1

	lda inner_vars::_tr_pos_y
	clc
	adc #TR_SCROLL_STEP
	sta inner_vars::_tr_pos_y

	sec
	sbc #$f0
	check_direction_vert_overflow_pos

	.IF TR_MIRR_VERT_HALF_ATTR
	bcs @exit
	.ENDIF ;TR_MIRR_VERT_HALF_ATTR

	txa
	beq @exit

	jmp _tr_drw_down_row

@exit:
	rts

update_jpad:

	; disable scroll
	upd_flag_reset inner_vars::TR_UPD_FLAG_SCROLL

	jsr jpad1_read_state

	jpad1_check_btn JPAD_BTN_LEFT
	beq @upd_check_right_btn

	jsr move_left
;	jmp @exit

@upd_check_right_btn:

	jpad1_check_btn JPAD_BTN_RIGHT
	beq @upd_check_up_btn

	jsr move_right
;	jmp @exit

@upd_check_up_btn:

	jpad1_check_btn JPAD_BTN_UP
	beq @upd_check_down_btn

	jsr move_up
;	jmp @exit

@upd_check_down_btn:

	jpad1_check_btn JPAD_BTN_DOWN
	beq @exit

	jsr move_down

@exit:
	jsr TR_utils::buff_end

	; enable scroll
	upd_flag_set inner_vars::TR_UPD_FLAG_SCROLL

	.IF TR_MIRR_VERT_HALF_ATTR
	lda inner_vars::_tr_ext_upd_flags
	and #<~inner_vars::TR_EXTRA_FLAGS_DOWN_HALF_ATTR
	sta inner_vars::_tr_ext_upd_flags
	.ENDIF ;TR_MIRR_VERT_HALF_ATTR

	rts

	; IN: X\Y - screen data address
	; OUT: A - screen index
_get_screen_index:

	stx data_addr
	sty data_addr + 1

	get_screen_index_offset
	lda (<data_addr), y

	rts

_tr_drw_up_row:

	get_screen_index inner_vars::TR_UPD_FLAG_IGNORE_UP_OVERFLOW, check_up_screen
	pha	; save the screen index

	; get tile data address

	.IF ::TR_DATA_TILES4X4
	calc_64bytes_data_offset tr_tiles_scr, _tmp_val
	.ELSE
	calc_240bytes_data_offset tr_tiles_scr, _tmp_val
	.ENDIF	;TR_DATA_TILES4X4

	; calc tile row address
	lda inner_vars::_tr_pos_y
	lsr a
	lsr a
	lsr a
	lsr a
	.IF ::TR_DATA_TILES4X4
	lsr a
	.ENDIF ;TR_DATA_TILES4X4

	tax
	
	ldy #$00
	add_xy_to_word _tmp_val

	; calculate PPU _nametable address
	lda inner_vars::_nametable
	jsr ppu_calc_nametable_addr

	lda inner_vars::_tr_pos_y

	lsr a
	lsr a
	lsr a

	tax

	ldy #$00
	mul32_xy
	add_xy_to_word data_addr

	; select data from table to search for blocks/CHRs
	lda inner_vars::_tr_pos_y

	.IF ::TR_DATA_TILES4X4
	and #%00011111
	.ELSE
	and #%00001111
	.ENDIF ;TR_DATA_TILES4X4
	lsr a
	lsr a
	lsr a

	pha					; save offset in a tile by rows

	.IF ::TR_DATA_TILES4X4
	asl a
	.ENDIF ;TR_DATA_TILES4X4
	asl a
	sta inner_vars::_drw_rowcol_inds_offset

	; data header
	lda #TR_utils::TR_BUFF_FLAGS_INC1
	sta TR_utils::tr_data_flags
	lda #SCR_WCHR_CNT
	sta inner_vars::_loop_cntr		; tiles counter

	jsr TR_utils::buff_push_hdr

	.IF ::TR_DATA_TILES4X4
	load_data_word _tmp_val, _tmp_val3	; necessary for attributes drawing
	.ENDIF	;TR_DATA_TILES4X4

	jsr _drw_tiles_row_dyn

	pop_x					; get position in a tile
	pop_y					; screen index

	.IF TR_MIRRORING_VERTICAL
	lda inner_vars::_tr_ext_upd_flags
	and #inner_vars::TR_UPD_FLAG_FORCED_ATTRS
	bne _tr_drw_up_row_attrs
	.ENDIF	;TR_MIRRORING_VERTICAL

	txa

	.IF TR_MIRRORING_VERTICAL
	.IF TR_MIRR_VERT_HALF_ATTR
	and #$01
	beq _tr_drw_up_row_attrs
	.ELSE
	; shift attrs drawing on 2 CHRs
	.IF ::TR_DATA_TILES4X4
	cmp #$01
	.ELSE
;	cmp #$00
	.ENDIF ;TR_DATA_TILES4X4
	.ENDIF ;TR_MIRR_VERT_HALF_ATTR
	.ELSE
	.IF ::TR_DATA_TILES4X4
	cmp #$03
	.ELSE
	cmp #$01
	.ENDIF ;TR_DATA_TILES4X4
	.ENDIF ;TR_MIRRORING_VERTICAL

	.IF !TR_MIRR_VERT_HALF_ATTR
	beq _tr_drw_up_row_attrs
	.ENDIF ;!TR_MIRR_VERT_HALF_ATTR

	lda inner_vars::_tr_pos_y
	lsr a
	lsr a
	lsr a					; nametable row number

	cmp #$1d				; the last 29 row
	bne @exit

	.IF !TR_MIRR_VERT_HALF_ATTR
	txa
	cmp #$01
	beq _tr_drw_up_row_attrs
	.ENDIF ;!TR_MIRR_VERT_HALF_ATTR
@exit:
	rts

_tr_drw_up_row_attrs:

	.IF TR_MIRR_VERT_HALF_ATTR
	lda inner_vars::_tr_ext_upd_flags
	and #<~inner_vars::TR_EXTRA_FLAGS_DOWN_HALF_ATTR
	sta inner_vars::_tr_ext_upd_flags

	push_y

	lda inner_vars::_tr_upd_flags
	and #inner_vars::TR_UPD_FLAG_IGNORE_DOWN_OVERFLOW
	beq @cont0

	get_screen_index_offset
	lda (<TR_utils::tr_curr_scr), y	; A - screen index

	jmp @cont1

@cont0:
	check_down_screen	
	stx _tmp_val
	sty _tmp_val + 1

	get_screen_index_offset
	lda (<_tmp_val), y	; A - screen index
	
@cont1:
	; calc attributes data address
	; IN: A - screen index
	.IF ::TR_DATA_TILES4X4
	calc_64bytes_data_offset tr_tiles_scr, _tmp_val2
	.ELSE
	calc_64bytes_data_offset tr_attrs_scr, _tmp_val2
	.ENDIF ;TR_DATA_TILES4X4

	lda inner_vars::_tr_pos_y
	lsr a
	lsr a
	lsr a
	lsr a
	lsr a
	add_a_to_word _tmp_val2

	pop_y
	.ENDIF ;TR_MIRR_VERT_HALF_ATTR

	; calc attributes data address
	.IF ::TR_DATA_TILES4X4
	load_data_word _tmp_val3, _tmp_val
	.ELSE
	tya					; screen index
	calc_64bytes_data_offset tr_attrs_scr, _tmp_val

	lda inner_vars::_tr_pos_y

	lsr a
	lsr a
	lsr a
	lsr a
	lsr a

	add_a_to_word _tmp_val
	.ENDIF	;TR_DATA_TILES4X4

	; PPU address
	; ( inner_vars::_tr_pos_y / 32 ) * 8
	lda inner_vars::_tr_pos_y
	div32_mul8

	clc
	adc #$c0
	sta data_addr

	lda inner_vars::_nametable
	asl a
	asl a
	clc
	adc #$23
	sta data_addr + 1

	; data header
	lda #TR_utils::TR_BUFF_FLAGS_INC1
	sta TR_utils::tr_data_flags
	lda #SCR_WATTRS_CNT
	sta inner_vars::_loop_cntr			; tiles counter

	jsr TR_utils::buff_push_hdr

	jmp _drw_attrs_row_dyn

_tr_drw_down_row:

	get_screen_index inner_vars::TR_UPD_FLAG_IGNORE_DOWN_OVERFLOW, check_down_screen
	pha						; save the screen index

	; get tile data address

	.IF ::TR_DATA_TILES4X4
	calc_64bytes_data_offset tr_tiles_scr, _tmp_val
	.ELSE
	calc_240bytes_data_offset tr_tiles_scr, _tmp_val
	.ENDIF	;TR_DATA_TILES4X4

	.IF TR_MIRR_VERT_HALF_ATTR
	lda inner_vars::_tr_pos_y
	pha

	sec 
	sbc #$08

	bcs @cont

	lda #$e8					; the last bottom row pos

@cont:
	sta inner_vars::_tr_pos_y

	.ENDIF ;TR_MIRR_VERT_HALF_ATTR

	; calc tile row address
	lda inner_vars::_tr_pos_y
	lsr a
	lsr a
	lsr a
	lsr a
	.IF ::TR_DATA_TILES4X4
	lsr a
	.ENDIF ;TR_DATA_TILES4X4

	tax
	
	ldy #$00
	add_xy_to_word _tmp_val

	; calculate PPU _nametable address
	lda inner_vars::_nametable
	eor #$02  			
	jsr ppu_calc_nametable_addr

	lda inner_vars::_tr_pos_y
	lsr a
	lsr a
	lsr a

	tax

	ldy #$00
	mul32_xy
	add_xy_to_word data_addr

	; select data from the table to search for blocks/CHRs
	lda inner_vars::_tr_pos_y
	.IF ::TR_DATA_TILES4X4
	and #%00011111
	.ELSE
	and #%00001111
	.ENDIF ;TR_DATA_TILES4X4
	lsr a
	lsr a
	lsr a

	pha						; save offset in a tile by rows

	.IF ::TR_DATA_TILES4X4
	asl a
	.ENDIF ;TR_DATA_TILES4X4
	asl a
	sta inner_vars::_drw_rowcol_inds_offset

	; data header
	lda #TR_utils::TR_BUFF_FLAGS_INC1
	sta TR_utils::tr_data_flags
	lda #SCR_WCHR_CNT
	sta inner_vars::_loop_cntr			; tiles counter

	jsr TR_utils::buff_push_hdr

	.IF ::TR_DATA_TILES4X4
	load_data_word _tmp_val, _tmp_val3		; necessary for attributes drawing
	.ENDIF	;TR_DATA_TILES4X4

	jsr _drw_tiles_row_dyn

	pop_x						; get position in a tile

	.IF TR_MIRR_VERT_HALF_ATTR
	pla
	sta inner_vars::_tr_pos_y
	.ENDIF ;TR_MIRR_VERT_HALF_ATTR

	pop_y						; screen index

	.IF TR_MIRRORING_VERTICAL
	lda inner_vars::_tr_ext_upd_flags
	and #inner_vars::TR_UPD_FLAG_FORCED_ATTRS
	bne _tr_drw_down_row_attrs
	.ENDIF	;TR_MIRRORING_VERTICAL

	txa

	.IF TR_MIRRORING_VERTICAL
	.IF TR_MIRR_VERT_HALF_ATTR
	and #$01
	beq _tr_drw_down_row_attrs
	.ELSE
	; shift attrs drawing on 2 CHRs
	.IF ::TR_DATA_TILES4X4
	cmp #$02
	.ELSE
	cmp #$01
	.ENDIF ;TR_DATA_TILES4X4
	.ENDIF ;TR_MIRR_HORIZ_HALF_ATTR
	.ENDIF ;TR_MIRRORING_VERTICAL

	.IF !TR_MIRR_VERT_HALF_ATTR
	beq _tr_drw_down_row_attrs
	.ENDIF ;TR_MIRR_VERT_HALF_ATTR

@exit:
	rts

_tr_drw_down_row_attrs:

	.IF TR_MIRR_VERT_HALF_ATTR
	lda inner_vars::_tr_ext_upd_flags
	ora #inner_vars::TR_EXTRA_FLAGS_DOWN_HALF_ATTR
	sta inner_vars::_tr_ext_upd_flags

	push_y

	lda inner_vars::_tr_upd_flags
	and #inner_vars::TR_UPD_FLAG_IGNORE_UP_OVERFLOW
	beq @cont0

	get_screen_index_offset
	lda (<TR_utils::tr_curr_scr), y	; A - screen index

	jmp @cont1

@cont0:
	check_up_screen	
	stx _tmp_val
	sty _tmp_val + 1

	get_screen_index_offset
	lda (<_tmp_val), y	; A - screen index
	
@cont1:
	; calc attributes data address
	; IN: A - screen index
	.IF ::TR_DATA_TILES4X4
	calc_64bytes_data_offset tr_tiles_scr, _tmp_val2
	.ELSE
	calc_64bytes_data_offset tr_attrs_scr, _tmp_val2
	.ENDIF ;TR_DATA_TILES4X4

	lda inner_vars::_tr_pos_y
	lsr a
	lsr a
	lsr a
	lsr a
	lsr a
	add_a_to_word _tmp_val2

	pop_y
	.ENDIF ;TR_MIRR_VERT_HALF_ATTR

	; calc attributes data address
	.IF ::TR_DATA_TILES4X4
	load_data_word _tmp_val3, _tmp_val
	.ELSE
	tya						; screen index
	calc_64bytes_data_offset tr_attrs_scr, _tmp_val

	lda inner_vars::_tr_pos_y
	lsr a
	lsr a
	lsr a
	lsr a
	lsr a

	add_a_to_word _tmp_val
	.ENDIF	; !TR_DATA_TILES4X4

	; PPU address
	; ( inner_vars::_tr_pos_y / 32 ) * 8
	lda inner_vars::_tr_pos_y
	div32_mul8

	clc
	adc #$c0
	sta data_addr

	lda inner_vars::_nametable
	eor #$02			
	asl a
	asl a
	clc
	adc #$23
	sta data_addr + 1

	; data header
	lda #TR_utils::TR_BUFF_FLAGS_INC1
	sta TR_utils::tr_data_flags
	lda #SCR_WATTRS_CNT
	sta inner_vars::_loop_cntr			; tiles counter

	jsr TR_utils::buff_push_hdr

	jmp _drw_attrs_row_dyn

_tr_drw_left_col:

	get_screen_index inner_vars::TR_UPD_FLAG_IGNORE_LEFT_OVERFLOW, check_left_screen
	pha

	; get tile data address

	.IF ::TR_DATA_TILES4X4
	calc_64bytes_data_offset tr_tiles_scr, _tmp_val
	.ELSE
	calc_240bytes_data_offset tr_tiles_scr, _tmp_val
	.ENDIF	;TR_DATA_TILES4X4

	; calc tile column address
	.IF ::TR_DATA_TILES4X4
	lda inner_vars::_tr_pos_x

	div32_mul8

	tax
	.ELSE
	lda inner_vars::_tr_pos_x
	
	lsr a
	lsr a
	lsr a
	lsr a

	tax
	lda _tiles_col_offset, x			; mul by SCR_HTILES_CNT
	tax
	.ENDIF ;TR_DATA_TILES4X4

	ldy #$00
	add_xy_to_word _tmp_val
	
	; select data from the table to search for blocks/CHRs
	lda inner_vars::_tr_pos_x
	.IF ::TR_DATA_TILES4X4
	and #%00011111
	.ELSE
	and #%00001111
	.ENDIF ;TR_DATA_TILES4X4
	lsr a
	lsr a
	lsr a

	pha						; save offset in tile by columns

	.IF ::TR_DATA_TILES4X4
	asl a
	asl a
	clc
	adc #$10
	.ELSE
	asl a
	clc
	adc #$04
	.ENDIF ;TR_DATA_TILES4X4
	sta inner_vars::_drw_rowcol_inds_offset

	; calc PPU address
	lda inner_vars::_tr_pos_x
	lsr a
	lsr a
	lsr a
	sta data_addr

	lda inner_vars::_nametable
	asl a
	asl a
	clc
	adc #$20
	sta data_addr + 1

	; data header
	lda #TR_utils::TR_BUFF_FLAGS_INC32
	sta TR_utils::tr_data_flags
	lda #SCR_HCHR_CNT
	sta inner_vars::_loop_cntr			; tiles counter

	jsr TR_utils::buff_push_hdr

	jsr _drw_tiles_col_dyn

	pla						; get position in a tile
	tax

	pla						; screen index
	tay

	txa

	.IF ::TR_DATA_TILES4X4
	cmp #$03
	.ELSE
	cmp #$01
	.ENDIF ;TR_DATA_TILES4X4

	beq _tr_drw_left_col_attrs	; draw attributes if non zero

@exit:
	rts

_tr_drw_left_col_attrs:

	; calc attributes data address
	.IF !::TR_DATA_TILES4X4
	tya						; screen index
	calc_64bytes_data_offset tr_attrs_scr, _tmp_val

	lda inner_vars::_tr_pos_x

	div32_mul8

	add_a_to_word _tmp_val
	.ENDIF	;!TR_DATA_TILES4X4

	; calc PPU address
	lda inner_vars::_tr_pos_x
	lsr a
	lsr a
	lsr a
	lsr a
	lsr a
	clc
	adc #$c0
	sta data_addr

	lda inner_vars::_nametable
	asl a
	asl a
	clc
	adc #$23
	sta data_addr + 1

	; data header
	lda #TR_utils::TR_BUFF_FLAGS_INC8
	sta TR_utils::tr_data_flags
	lda #SCR_HATTRS_CNT
	sta inner_vars::_loop_cntr			; set tiles counter

	jsr TR_utils::buff_push_hdr

	jmp _drw_attrs_col_dyn

_tr_drw_right_col:

	get_screen_index inner_vars::TR_UPD_FLAG_IGNORE_RIGHT_OVERFLOW, check_right_screen
	pha

	; get CHR data address

	.IF ::TR_DATA_TILES4X4
	calc_64bytes_data_offset tr_tiles_scr, _tmp_val
	.ELSE
	calc_240bytes_data_offset tr_tiles_scr, _tmp_val
	.ENDIF	;TR_DATA_TILES4X4

	.IF ::TR_DATA_TILES4X4
	lda inner_vars::_tr_pos_x

	div32_mul8

	tax
	.ELSE
	lda inner_vars::_tr_pos_x

	lsr a
	lsr a
	lsr a
	lsr a

	tax
	lda _tiles_col_offset, x			; mul by SCR_HTILES_CNT
	tax
	.ENDIF ;TR_DATA_TILES4X4

	ldy #$00
	add_xy_to_word _tmp_val
	
	; select data from the table to search for blocks/CHRs
	lda inner_vars::_tr_pos_x
	.IF ::TR_DATA_TILES4X4
	and #%00011111
	.ELSE
	and #%00001111
	.ENDIF ;TR_DATA_TILES4X4
	lsr a
	lsr a
	lsr a

	pha						; save offset in tile by columns

	.IF ::TR_DATA_TILES4X4
	asl a
	asl a
	clc
	adc #$10
	.ELSE
	asl a
	clc
	adc #$04
	.ENDIF ;TR_DATA_TILES4X4
	sta inner_vars::_drw_rowcol_inds_offset

	; calc PPU address
	lda inner_vars::_tr_pos_x
	lsr a
	lsr a
	lsr a
	sta data_addr

	lda inner_vars::_nametable
	eor #$01
	asl a
	asl a
	clc
	adc #$20
	sta data_addr + 1

	; data header
	lda #TR_utils::TR_BUFF_FLAGS_INC32
	sta TR_utils::tr_data_flags
	lda #SCR_HCHR_CNT
	sta inner_vars::_loop_cntr			; tiles counter

	jsr TR_utils::buff_push_hdr

	jsr _drw_tiles_col_dyn

	pla						; get position in a tile
	tax

	pla						; screen index
	tay

	txa
	beq _tr_drw_right_col_attrs	; draw attributes if non zero

@exit:
	rts

_tr_drw_right_col_attrs:

	; calc attributes data address
	.IF !::TR_DATA_TILES4X4
	tya						; screen index
	calc_64bytes_data_offset tr_attrs_scr, _tmp_val

	lda inner_vars::_tr_pos_x

	div32_mul8

	add_a_to_word _tmp_val
	.ENDIF	;!TR_DATA_TILES4X4

	; calc PPU addr
	lda inner_vars::_tr_pos_x
	lsr a
	lsr a
	lsr a
	lsr a
	lsr a
	clc
	adc #$c0
	sta data_addr

	lda inner_vars::_nametable
	eor #$01
	asl a
	asl a
	clc
	adc #$23
	sta data_addr + 1

	; data header
	lda #TR_utils::TR_BUFF_FLAGS_INC8
	sta TR_utils::tr_data_flags
	lda #SCR_HATTRS_CNT
	sta inner_vars::_loop_cntr			; set tiles counter

	jsr TR_utils::buff_push_hdr

	jmp _drw_attrs_col_dyn

	.IF TR_MIRR_VERT_HALF_ATTR

; IN: 	A - current attribute

half_attrs_fix_up_down:

	pha

	lda inner_vars::_tr_pos_y
	lsr a
	lsr a
	lsr a

	tay

	; Y - tile rows
	; STACK - attribute

	lda inner_vars::_tr_ext_upd_flags
	and #inner_vars::TR_EXTRA_FLAGS_DOWN_HALF_ATTR
	beq @up_attrs

	tya
	and #$02
	bne @_34_tiles_col_down

	; 1-2 tiles col

	ldy #$00
	lda (<_tmp_val2), y

	.IF ::TR_DATA_TILES4X4
	tay
	lda (<tr_attrs), y		; read attr
	.ENDIF ;TR_DATA_TILES4X4

	and #%11110000
	sta _tmp_val4

	pla

	and #%00001111
	ora _tmp_val4

	rts

@_34_tiles_col_down:

	pla

	rts

@up_attrs:

	tya
	and #$02
	beq @_34_tiles_col_up

	; 1-2 tiles col

	ldy #$00
	lda (<_tmp_val2), y

	.IF ::TR_DATA_TILES4X4
	tay
	lda (<tr_attrs), y
	.ENDIF ;TR_DATA_TILES4X4

	and #%00001111
	sta _tmp_val4

	pla

	and #%11110000
	ora _tmp_val4

	rts

@_34_tiles_col_up:

	pla

	rts

	.ENDIF ;TR_MIRR_VERT_HALF_ATTR

_drw_attrs_row_dyn:

	; draw row of attributes

@drw_attrs_row_dyn_loop:

	; read tile
	ldy #$00
	lda (<_tmp_val), y

	.IF ::TR_DATA_TILES4X4
	tay
	lda (<inner_vars::_tr_attrs), y			; read attribute
	.ENDIF ;TR_DATA_TILES4X4

	.IF TR_MIRR_VERT_HALF_ATTR
	jsr half_attrs_fix_up_down
	.ENDIF ;TR_MIRR_VERT_HALF_ATTR

	jsr TR_utils::buff_push_data

	.IF TR_MIRR_VERT_HALF_ATTR
	.IF ::TR_DATA_TILES4X4
	lda #::SCR_HTILES_CNT
	.ELSE
	lda #::SCR_HATTRS_CNT
	.ENDIF ;TR_DATA_TILES4X4
	add_a_to_word _tmp_val2
	.ENDIF ;TR_MIRR_VERT_HALF_ATTR

	.IF ::TR_DATA_TILES4X4
	lda #::SCR_HTILES_CNT
	.ELSE
	lda #::SCR_HATTRS_CNT
	.ENDIF ;TR_DATA_TILES4X4
	add_a_to_word _tmp_val

	dec inner_vars::_loop_cntr
	bne @drw_attrs_row_dyn_loop

	rts

_drw_attrs_col_dyn:

	; draw a column of attributes
	ldy #$00

@drw_attrs_col_dyn_loop:

	; read tile
	push_y				;<----
	lda (<_tmp_val), y

	.IF ::TR_DATA_TILES4X4
	tay
	lda (<inner_vars::_tr_attrs), y			; read attribute
	.ENDIF ;TR_DATA_TILES4X4
	
	jsr TR_utils::buff_push_data

	pop_y
	iny

	dec inner_vars::_loop_cntr
	bne @drw_attrs_col_dyn_loop

	rts

_drw_tiles_row_dyn:

	; draw a row of tiles

@drw_tiles_row_dyn_loop:

	; read tile
	ldy #$00
	lda (<_tmp_val), y

	; A *= 4
	mul4_a_xy

	.IF ::TR_DATA_TILES4X4
	add_word_to_xy inner_vars::_tr_tiles	; now XY contain tile's blocks address
	.ELSE
	add_word_to_xy inner_vars::_tr_blocks	; now XY contain an address of a 2x2 tile
	.ENDIF ;TR_DATA_TILES4X4

	stx TR_utils::inner_vars::_tile_addr
	sty TR_utils::inner_vars::_tile_addr + 1

	.IF ::TR_DATA_TILES4X4
	ldy inner_vars::_drw_rowcol_inds_offset		;3
	lda _drw_rowcol_inds_tbl, y			;4
	tay						;2

	lda (<TR_utils::inner_vars::_tile_addr), y	; 1 block

	jsr _drw_blocks_dyn_xy

	bcs @exit

	ldy inner_vars::_drw_rowcol_inds_offset		;3
	iny						;2
	lda _drw_rowcol_inds_tbl, y			;4
	tay						;2

	lda (<TR_utils::inner_vars::_tile_addr), y	; 2 block

	jsr _drw_blocks_dyn_xy

	bcs @exit
	.ELSE
	ldy inner_vars::_drw_rowcol_inds_offset		;3
	lda _drw_rowcol_inds_tbl, y			;4
	tay						;2

	lda (<TR_utils::inner_vars::_tile_addr), y	; 1 block

	jsr TR_utils::buff_push_data

	dec inner_vars::_loop_cntr
	beq @exit

	ldy inner_vars::_drw_rowcol_inds_offset		;3
	iny
	lda _drw_rowcol_inds_tbl, y			;4
	tay						;2

	lda (<TR_utils::inner_vars::_tile_addr), y	; 2 block
	jsr TR_utils::buff_push_data

	dec inner_vars::_loop_cntr
	beq @exit
	.ENDIF ;TR_DATA_TILES4X4

	lda #::SCR_HTILES_CNT
	add_a_to_word _tmp_val

	jmp @drw_tiles_row_dyn_loop

@exit:

	rts

_drw_tiles_col_dyn:

	; draw a column of tiles
	ldy #$00

@drw_tiles_col_dyn_loop:

	; read tile
	push_y				;<----
	lda (<_tmp_val), y

	; A *= 4
	mul4_a_xy

	.IF ::TR_DATA_TILES4X4
	add_word_to_xy inner_vars::_tr_tiles		; now XY contain tile's blocks address
	.ELSE
	add_word_to_xy inner_vars::_tr_blocks		; now XY contain an address of a 2x2 tile
	.ENDIF ;TR_DATA_TILES4X4

	stx TR_utils::inner_vars::_tile_addr
	sty TR_utils::inner_vars::_tile_addr + 1

	.IF ::TR_DATA_TILES4X4
	ldy inner_vars::_drw_rowcol_inds_offset		;3
	lda _drw_rowcol_inds_tbl, y			;4
	tay						;2

	lda (<TR_utils::inner_vars::_tile_addr), y	; 1 block

	jsr _drw_blocks_dyn_xy

	bcs @exit

	ldy inner_vars::_drw_rowcol_inds_offset		;3
	iny
	lda _drw_rowcol_inds_tbl, y			;4
	tay						;2

	lda (<TR_utils::inner_vars::_tile_addr), y	; 2 block

	jsr _drw_blocks_dyn_xy

	bcs @exit
	.ELSE
	ldy inner_vars::_drw_rowcol_inds_offset		;3
	lda _drw_rowcol_inds_tbl, y			;4
	tay						;2

	lda (<TR_utils::inner_vars::_tile_addr), y	; 1 block

	jsr TR_utils::buff_push_data

	dec inner_vars::_loop_cntr
	beq @exit

	ldy inner_vars::_drw_rowcol_inds_offset		;3
	iny
	lda _drw_rowcol_inds_tbl, y			;4
	tay						;2

	lda (<TR_utils::inner_vars::_tile_addr), y	; 2 block

	jsr TR_utils::buff_push_data

	dec inner_vars::_loop_cntr
	beq @exit
	.ENDIF ;TR_DATA_TILES4X4

	pop_y
	iny					;<----

	jmp @drw_tiles_col_dyn_loop

@exit:

	pop_y

	rts

	.IF ::TR_DATA_TILES4X4
_drw_blocks_dyn_xy:

	mul4_a_xy

	add_word_to_xy inner_vars::_tr_blocks		; now XY contain an address of blocks
	stx TR_utils::inner_vars::_block_addr
	sty TR_utils::inner_vars::_block_addr + 1

	ldy inner_vars::_drw_rowcol_inds_offset		;3
	iny
	iny
	lda _drw_rowcol_inds_tbl, y			;4
	tay						;2

	lda (<TR_utils::inner_vars::_block_addr), y
	jsr TR_utils::buff_push_data

	dec inner_vars::_loop_cntr
	beq @exit

	ldy inner_vars::_drw_rowcol_inds_offset		;3
	iny
	iny
	iny
	lda _drw_rowcol_inds_tbl, y			;4
	tay						;2

	lda (<TR_utils::inner_vars::_block_addr), y
	jsr TR_utils::buff_push_data

	dec inner_vars::_loop_cntr
	beq @exit

	clc
	rts						; reset carry flag and continue drawing

@exit:
	sec						; set carry flag as a sign of the end
	rts
	.ENDIF ;TR_DATA_TILES4X4

tr_get_prop:
	; get the block property by current coordinates
	; IN: inner_vars::_tr_pos_x, inner_vars::_tr_pos_y
	; OUT: A - property index

	;...

	rts

update_scroll_reg:

	lda inner_vars::_tr_upd_flags
	and #inner_vars::TR_UPD_FLAG_SCROLL
	beq _skip_upd_scroll

	; clean up PPU address registers
	lda #$00
	sta $2006
	sta $2006			

	lda inner_vars::_tr_pos_x
	sta $2005

	lda inner_vars::_tr_pos_y
	sta $2005

_skip_upd_scroll:

	rts

update_nametable:

	jsr ppu_get_2000
	ora inner_vars::_nametable
	sta $2000

	rts

	.endscope

; some aliases to draw fullscreen nametable\attrs data 
; using TR_utils::draw_screen_nametable and TR_utils::draw_screen_attrs

TR_HTILES:	.byte ::SCR_HTILES_CNT
TR_BLOCKS	= TR_sts::inner_vars::_tr_blocks

	.IF ::TR_DATA_TILES4X4
TR_TILES	= TR_sts::inner_vars::_tr_tiles
TR_ATTRS	= TR_sts::inner_vars::_tr_attrs
	.ELSE
TR_HATTRS_CNT:	.byte ::SCR_HATTRS_CNT
	.ENDIF	;TR_DATA_TILES4X4