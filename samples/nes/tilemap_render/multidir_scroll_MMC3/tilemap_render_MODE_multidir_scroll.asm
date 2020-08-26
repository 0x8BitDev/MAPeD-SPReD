;#########################################################################################################################
;
; Copyright 2018-2019 0x8BitDev ( MIT license )
;
;#########################################################################################################################
; DESC: Multidirectional tilemap scroller. It can be used with 4x4/2x2 tiles and compressed map data.
;	Uses attributes per block (2x2 tile). The column data order is required.
;
;#########################################################################################################################
; LIMITATIONS:
;
;	- TR_MAP_FLAGS_TILES4X4 : MAX 16 screens in width ( rewrite _get_tile_addr_tbl_val to use 16bit values )
;	  ( the memory limit is 16x22 of uncompressed screens );
;
;	- TR_MAP_FLAGS_TILES2Х2 : MAX 8 screens in width ( rewrite _get_tile_addr_tbl_val to use 16bit values )
;	  ( the memory limit is 8x9 of uncompressed screens );
;
;	- The map data limit when using RLE is about 8Kb of decompressed data ( decompressed to the $6000 optional RAM );
;
;#########################################################################################################################

; Public procs:
;
;	init_draw 	- 'A' start screen index
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
; .define TR_MIRRORING_VERTICAL	 --  1 vertical, 0 - 4-screen mirroring
;
;	MAP_FLAG_TILES2X2
;	MAP_FLAG_TILES4X4
;	MAP_FLAG_RLE
;	MAP_FLAG_DIR_COLUMNS
;	MAP_FLAG_MODE_MULTIDIR_SCROLL
;	MAP_FLAG_ATTRS_PER_BLOCK
;

.include "../../common/tilemap_render_UTILS.asm"


SUPPORTED_MAP_FLAGS	= MAP_FLAG_DIR_COLUMNS | MAP_FLAG_MODE_MULTIDIR_SCROLL | MAP_FLAG_ATTRS_PER_BLOCK

	.IF ( MAP_DATA_MAGIC & SUPPORTED_MAP_FLAGS ) <> SUPPORTED_MAP_FLAGS
	.fatal "UNSUPPORTED MAP FLAGS DETECTED ! TRY TO RE-EXPORT THE DATA WITH THE CORRECT OPTIONS !"
	.ENDIF

	.scope TR_ms

TR_SCROLL_STEP	= 2 ; 1 / 2 / 4

.segment "ZP"

; INIT DATA: BEG --------
tr_blocks:	.res 2
tr_props:	.res 2
tr_map:		.res 2
tr_map_tbl:	.res 2
tr_palette:	.res 2
tr_wtiles:	.res 1	; number of tiles in map in width
tr_htiles:	.res 1	; number of tiles in map in height
tr_wscr_cnt:	.res 1
tr_hscr_cnt:	.res 1

	.IF ::TR_DATA_TILES4X4

tr_tiles:	.res 2
tr_attrs:	.res 2

	.ELSE	; TR_DATA_TILES4X4

tr_attrs_map:	.res 2
tr_hattrs_cnt:	.res 1	; number of attributes in map in height

	.ENDIF; TR_DATA_TILES4X4	
; INIT DATA: END --------

	.scope inner_vars

_tr_pos_x:	.res 2
_tr_pos_y:	.res 2

_tr_max_x:	.res 2	; max value of X coordinate
_tr_max_y:	.res 2	; max value of Y coordinate

_nametable:	.res 1	; name table for background drawing

_loop_cntr:	.res 1
_ignore_cnt:	.res 1	; number of items which will be skipped in nametable during drawing
_extra_cnt:	.res 1	; additional counter of tiles\attrs which will be drawing on another screen

_drw_rowcol_inds_offset:	
		.res 1	; offset in the array _drw_rowcol_inds_tbl to search for data in tiles

_tr_upd_flags:	.res 1

TR_UPD_FLAG_SCROLL	= %00000001
TR_UPD_FLAG_DRAW_UP	= %00000010
TR_UPD_FLAG_DRAW_DOWN	= %00000100
TR_UPD_FLAG_DRAW_LEFT	= %00001000
TR_UPD_FLAG_DRAW_RIGHT	= %00010000

	.IF TR_MIRRORING_VERTICAL
	
TR_UPD_FLAG_FORCE_ATTRS		= %00100000	; force draw of attributes
TR_UPD_FLAG_FORCE_TOP		= %01000000	; force draw of top line
TR_UPD_FLAG_FORCE_BOTTOM	= %10000000	; force draw of bottom line

	.ENDIF	; TR_MIRRORING_VERTICAL

TR_UPD_FLAG_DRAW_MASK	= TR_UPD_FLAG_DRAW_UP | TR_UPD_FLAG_DRAW_DOWN | TR_UPD_FLAG_DRAW_LEFT | TR_UPD_FLAG_DRAW_RIGHT

	.endscope

.segment "CODE"

	.IF ::TR_DATA_TILES4X4

_drw_rowcol_inds_tbl:	.byte 0, 1, 0, 1, 0, 1, 2, 3, 2, 3, 0, 1, 2, 3, 2, 3
			.byte 0, 2, 0, 2, 0, 2, 1, 3, 1, 3, 0, 2, 1, 3, 1, 3

	.ELSE	; TR_DATA_TILES4X4

_drw_rowcol_inds_tbl:	.byte 0, 1, 2, 3
			.byte 0, 2, 1, 3

	.ENDIF; TR_DATA_TILES4X4

	.IF TR_MIRRORING_VERTICAL

	.macro tr_set_draw_flag_up_force_bottom_attr
	lda inner_vars::_tr_upd_flags
	and #<~inner_vars::TR_UPD_FLAG_FORCE_TOP
	ora #(inner_vars::TR_UPD_FLAG_DRAW_UP | inner_vars::TR_UPD_FLAG_FORCE_ATTRS | inner_vars::TR_UPD_FLAG_FORCE_BOTTOM)
	sta inner_vars::_tr_upd_flags
	.endmacro

	.macro tr_set_draw_flag_down_force_top_attr
	lda inner_vars::_tr_upd_flags
	and #<~inner_vars::TR_UPD_FLAG_FORCE_BOTTOM
	ora #(inner_vars::TR_UPD_FLAG_DRAW_DOWN | inner_vars::TR_UPD_FLAG_FORCE_ATTRS | inner_vars::TR_UPD_FLAG_FORCE_TOP)
	sta inner_vars::_tr_upd_flags
	.endmacro

	.macro tr_set_draw_flag_force_top
	lda inner_vars::_tr_upd_flags
	and #<~inner_vars::TR_UPD_FLAG_FORCE_BOTTOM
	ora #inner_vars::TR_UPD_FLAG_FORCE_TOP
	sta inner_vars::_tr_upd_flags
	.endmacro

	.macro tr_set_draw_flag_force_bottom
	lda inner_vars::_tr_upd_flags
	and #<~inner_vars::TR_UPD_FLAG_FORCE_TOP
	ora #inner_vars::TR_UPD_FLAG_FORCE_BOTTOM
	sta inner_vars::_tr_upd_flags
	.endmacro

	.ENDIF	; TR_MIRRORING_VERTICAL

	.macro tr_set_draw_flag_up
	lda inner_vars::_tr_upd_flags
	ora #inner_vars::TR_UPD_FLAG_DRAW_UP
	sta inner_vars::_tr_upd_flags
	.endmacro

	.macro tr_set_draw_flag_down
	lda inner_vars::_tr_upd_flags
	ora #inner_vars::TR_UPD_FLAG_DRAW_DOWN
	sta inner_vars::_tr_upd_flags
	.endmacro

	.macro tr_set_draw_flag_left
	lda inner_vars::_tr_upd_flags
	ora #inner_vars::TR_UPD_FLAG_DRAW_LEFT
	sta inner_vars::_tr_upd_flags
	.endmacro

	.macro tr_set_draw_flag_right
	lda inner_vars::_tr_upd_flags
	ora #inner_vars::TR_UPD_FLAG_DRAW_RIGHT
	sta inner_vars::_tr_upd_flags
	.endmacro

init_draw:	
	; IN: A - start screen index

	pha

	lda #$00
	sta inner_vars::_tr_upd_flags

	; decode compressed tilemap to $6000 ( additional 8K in MMC3 )

	.IF ::TR_DATA_RLE
	lda #$00
	sta RLE_DECOMP_ADDR
	lda #$60
	sta RLE_DECOMP_ADDR + 1

	ldx tr_map
	ldy tr_map + 1

	jsr unrle_to_mem

	lda #$00
	sta tr_map
	lda #$60
	sta tr_map + 1

	.IF !::TR_DATA_TILES4X4

	; attributes map decoding

	ldx tr_attrs_map
	ldy tr_attrs_map + 1

	lda RLE_DECOMP_ADDR
	sta tr_attrs_map
	lda RLE_DECOMP_ADDR + 1
	sta tr_attrs_map + 1

	jsr unrle_to_mem

	.ENDIF	; !TR_DATA_TILES4X4
	.ENDIF	; TR_DATA_RLE

	pla

	; calc screen coordinates by value in A

	sta _tmp_val
	ldx tr_wscr_cnt
	stx _tmp_val + 1
	div_byte_byte _tmp_val, _tmp_val + 1

	pha						; A = X

	.IF TR_MIRRORING_VERTICAL
	lda #$00
	.ELSE
	and #%00000001
	sta _tmp_val2
	lda _tmp_val
	asl a
	ora _tmp_val2
	and #%00000011
	.ENDIF
	sta inner_vars::_nametable

	.IF ::TR_DATA_TILES4X4
	lda #SCR_HTILES_CNT << 2
	.ELSE
	lda  #( ( SCR_HTILES_CNT + 1 ) << 1 )
	.ENDIF; TR_DATA_TILES4X4
	sta _tmp_val + 1				; _tmp_val = Y

	jsr mul8x8

	mul8_word _tmp_val2
	load_data_word _tmp_val2, inner_vars::_tr_pos_y

	pla						; X

	sta _tmp_val
	lda #SCR_WCHR_CNT
	sta _tmp_val + 1

	jsr mul8x8

	mul8_word _tmp_val2
	load_data_word _tmp_val2, inner_vars::_tr_pos_x

	; inner_vars::_tr_max_x = ( tr_wscr_cnt - 1 ) * SCR_WCHR_CNT

	ldx tr_wscr_cnt
	dex
	stx _tmp_val + 1
	lda #SCR_WCHR_CNT
	sta _tmp_val
	mul_byte_byte _tmp_val + 1, _tmp_val, inner_vars::_tr_max_x
	mul8_word inner_vars::_tr_max_x

	lda inner_vars::_tr_max_x
	and #%11111110
	sta inner_vars::_tr_max_x

	; inner_vars::_tr_max_y = ( tr_hscr_cnt - 1 ) * SCR_HCHR_CNT

	ldx tr_hscr_cnt
	dex
	stx _tmp_val + 1
	.IF ::TR_DATA_TILES4X4
	lda #SCR_HTILES_CNT << 2
	.ELSE
	lda #( ( SCR_HTILES_CNT + 1 ) << 1 )
	.ENDIF; TR_DATA_TILES4X4
	sta _tmp_val
	mul_byte_byte _tmp_val + 1, _tmp_val, inner_vars::_tr_max_y
	mul8_word inner_vars::_tr_max_y

	lda inner_vars::_tr_max_y
	and #%11111110
	sta inner_vars::_tr_max_y

; draw start screen

	jsr TR_utils::buff_reset

	set_ppu_1_inc

	
	lda inner_vars::_nametable
	jsr ppu_calc_nametable_addr		; data_addr - PPU address for tiles drawing

	push_word data_addr

	; get tile address by tr_pos coordinates
	jsr _get_tile_addr

	push_word _tmp_val

	ldx #SCR_WTILES_CNT
	jsr TR_utils::draw_screen_nametable

	pop_word _tmp_val

	pop_word data_addr

	; attributes address = nametable address + $03c0
	ldx #$c0
	ldy #$03
	add_xy_to_word data_addr

	.IF !::TR_DATA_TILES4X4
	jsr _get_attr_addr
	.ENDIF; TR_DATA_TILES4X4

	ldx #SCR_WATTRS_CNT
	
	jsr TR_utils::draw_screen_attrs

	; check Y overflow

	sub_word_from_word_sign inner_vars::_tr_max_y, inner_vars::_tr_pos_y
	beq @exit

	load_data_word inner_vars::_tr_max_y, inner_vars::_tr_pos_y

	.IF !TR_MIRRORING_VERTICAL
	lda inner_vars::_nametable
	eor #$02
	sta inner_vars::_nametable
	.ENDIF	; !TR_MIRRORING_VERTICAL

@exit:

	rts

	.IF !::TR_DATA_TILES4X4

_get_attr_addr:

	; get attributes address by tr_pos coordinates
	; IN: 	inner_vars::_tr_pos_x, inner_vars::_tr_pos_y
	; OUT: _tmp_val - attributes address

	load_data_word inner_vars::_tr_pos_x, _tmp_val

_get_attr_addr_interm:

	; IN: 	_tmp_val - pos x

	div32_word_xy _tmp_val

	stx _tmp_val
	lda tr_hattrs_cnt
	sta _tmp_val + 1
	jsr mul8x8			; _tmp_val2 = ( inner_vars::_tr_pos_x / 32 ) * tr_hattrs_cnt

	; _tmp_val = _tmp_val2
	load_data_word _tmp_val2, _tmp_val

	; divide inner_vars::_tr_pos_y by 16, to find out
	; which tile number corresponds to the Y coordinate

	div16_word_xy inner_vars::_tr_pos_y
	txa

	; if Y coordinate isn't lie on the 15th tile
	; then we calculate as usual

	cmp #$0f
	bne @cont1

	; if Y coordinate lies on the 15th tile
	; add 1 to resulting attribute index
	; to skip the problem area

	; divide again by 2, to get a division by 32

	tya
	lsr a
	tay
	txa
	ror a
	tax

	inx

	jmp @cont2

@cont1:

	; divide again by 2, to get a division by 32

	tya
	lsr a
	tay
	txa
	ror a
	tax

@cont2:

	add_xy_to_word _tmp_val				; _tmp_val += inner_vars::_tr_pos_y / 32

	add_word_to_word tr_attrs_map, _tmp_val		; _tmp_val += tr_attrs_map

	rts

	.ENDIF; !TR_DATA_TILES4X4

_get_tile_addr:

	; get tiles address by tr_pos coordinates
	; IN: 	inner_vars::_tr_pos_x, inner_vars::_tr_pos_y
	; OUT: _tmp_val - tiles address

	.IF ::TR_DATA_TILES4X4
	div32_word_xy inner_vars::_tr_pos_x
	.ELSE
	div16_word_xy inner_vars::_tr_pos_x
	.ENDIF; TR_DATA_TILES4X4
	txa
	asl a

_get_tile_addr_tbl_val:

	; IN: 	A - index x2 in the tr_map_tbl table
	;	inner_vars::_tr_pos_y - Y coordinate
	; OUT: _tmp_val - tiles address

	sta _tmp_val
	lda #$00
	sta _tmp_val + 1

	; _tmp_val = tr_map + [tr_map_tbl + A]

	lda tr_map_tbl
	tax
	lda tr_map_tbl + 1
	tay

	add_xy_to_word _tmp_val

	ldy #$00
	lda (<_tmp_val), y
	tax
	iny
	lda (<_tmp_val), y
	tay

	clc
	txa
	adc tr_map
	sta _tmp_val
	tya
	adc tr_map + 1
	sta _tmp_val + 1

	; add the Y coordinate
	.IF ::TR_DATA_TILES4X4
	div32_word_xy inner_vars::_tr_pos_y
	.ELSE
	div16_word_xy inner_vars::_tr_pos_y

	; to compensate for the gap between map tiles
	; and coordinates along the Y axis, 
	; subtract from the current tile number the value 
	; of the high byte inner_vars::_ tr_pos_y, which
	; increases when switching to a new screen in the nametable
	txa
	sec
	sbc inner_vars::_tr_pos_y + 1
	tax

	.ENDIF; TR_DATA_TILES4X4

	add_xy_to_word _tmp_val

	rts

move_left:

	lda #<-TR_SCROLL_STEP
	ldx inner_vars::_tr_pos_x + 1			; to check screen switching
	ldy inner_vars::_tr_pos_x			; to check drawing of new tiles
	add_neg_a_to_word inner_vars::_tr_pos_x

	lda inner_vars::_tr_pos_x + 1
	and #%10000000
	beq @cont1

	lda #$00
	sta inner_vars::_tr_pos_x
	sta inner_vars::_tr_pos_x + 1
	
	rts

@cont1:
	txa
	eor inner_vars::_tr_pos_x + 1
	beq @cont2

	lda inner_vars::_nametable
	eor #$01
	sta inner_vars::_nametable
@cont2:
	lda inner_vars::_tr_pos_x
	and #%00000111
	sta _tmp_val

	tya
	and #%00000111
	sec
	sbc _tmp_val
	bpl @cont3

	tr_set_draw_flag_left

	.IF TR_MIRRORING_VERTICAL
	lda inner_vars::_tr_pos_y
	and #$07
	bne @cont3
	tr_set_draw_flag_force_top
	.ENDIF	; TR_MIRRORING_VERTICAL	

@cont3:

	rts


move_right:  			

	lda #TR_SCROLL_STEP
	ldx inner_vars::_tr_pos_x + 1			; to check screen switching
	ldy inner_vars::_tr_pos_x			; to check drawing of new tiles
	add_a_to_word inner_vars::_tr_pos_x

	sub_word_from_word_sign inner_vars::_tr_max_x, inner_vars::_tr_pos_x
	beq @cont1

	load_data_word inner_vars::_tr_max_x, inner_vars::_tr_pos_x
	
	rts

@cont1:
	txa
	eor inner_vars::_tr_pos_x + 1
	beq @cont2

	lda inner_vars::_nametable
	eor #$01
	sta inner_vars::_nametable

@cont2:
	tya
	and #%00000111
	beq @upd_drw_right_col
	
	rts

@upd_drw_right_col:

	tr_set_draw_flag_right

	.IF TR_MIRRORING_VERTICAL
	lda inner_vars::_tr_pos_y
	and #$07
	bne @exit  			
	tr_set_draw_flag_force_top
@exit:		
	.ENDIF	; TR_MIRRORING_VERTICAL	

	rts

move_up:

	.IF TR_MIRRORING_VERTICAL

	lda inner_vars::_tr_upd_flags
	and #inner_vars::TR_UPD_FLAG_FORCE_TOP
	beq @cont1

	tr_set_draw_flag_up_force_bottom_attr
	rts

@cont1:			
	tr_set_draw_flag_force_bottom

	.ENDIF	; TR_MIRRORING_VERTICAL

	lda #<-TR_SCROLL_STEP
	ldy inner_vars::_tr_pos_y			; to check drawing of new tiles
	add_neg_a_to_word inner_vars::_tr_pos_y

	; check if < 0
	lda inner_vars::_tr_pos_y + 1
	and #%10000000
	beq @cont2

	lda #$00
	sta inner_vars::_tr_pos_y
	sta inner_vars::_tr_pos_y + 1
	
	rts

@cont2:

	; screen switching test
	lda inner_vars::_tr_pos_y
	and #%11110000
	cmp #%11110000

	.IF !TR_MIRRORING_VERTICAL
	bne @cont4
	.ELSE
	bne @upd_drw_up_col
	.ENDIF	; !TR_MIRRORING_VERTICAL

	lda #$ee
	and #<~( TR_SCROLL_STEP - 1 )		; _tr_pos_y is a multiple of TR_SCROLL_STEP
	sta inner_vars::_tr_pos_y

	.IF !TR_MIRRORING_VERTICAL
	lda inner_vars::_nametable
	eor #$02
	sta inner_vars::_nametable

@cont4:
	tya
	and #%00000111
	beq @upd_drw_up_col
	rts

	.ENDIF	; !TR_MIRRORING_VERTICAL

@upd_drw_up_col:

	tr_set_draw_flag_up
	rts

move_down:

	.IF TR_MIRRORING_VERTICAL

	lda inner_vars::_tr_upd_flags
	and #inner_vars::TR_UPD_FLAG_FORCE_BOTTOM
	beq @cont1

	tr_set_draw_flag_down_force_top_attr
	rts

@cont1:			
	tr_set_draw_flag_force_top

	.ENDIF	; TR_MIRRORING_VERTICAL

	lda #TR_SCROLL_STEP
	ldy inner_vars::_tr_pos_y			; to check drawing of new tiles
	add_a_to_word inner_vars::_tr_pos_y

	sub_word_from_word_sign inner_vars::_tr_max_y, inner_vars::_tr_pos_y
	beq @cont2

	load_data_word inner_vars::_tr_max_y, inner_vars::_tr_pos_y
	rts

@cont2:

	lda inner_vars::_tr_pos_y
	and #%11110000
	cmp #%11110000

	.IF !TR_MIRRORING_VERTICAL
	bne @cont3
	.ELSE
	bne @upd_drw_down_row
	.ENDIF

	lda #$00
	sta inner_vars::_tr_pos_y

	inc inner_vars::_tr_pos_y + 1

	.IF !TR_MIRRORING_VERTICAL
	lda inner_vars::_nametable
	eor #$02
	sta inner_vars::_nametable

@cont3:
	tya
	and #%00000111
	beq @upd_drw_down_row

	rts
	.ENDIF	; !TR_MIRRORING_VERTICAL

@upd_drw_down_row:

	tr_set_draw_flag_down
	rts

update_jpad:

	; disable scrolling update on PPU
	; and reset the draw flags
	lda inner_vars::_tr_upd_flags

	.IF TR_MIRRORING_VERTICAL
	and #<~(inner_vars::TR_UPD_FLAG_DRAW_MASK | inner_vars::TR_UPD_FLAG_SCROLL | inner_vars::TR_UPD_FLAG_FORCE_ATTRS)
	.ELSE
	and #<~(inner_vars::TR_UPD_FLAG_DRAW_MASK | inner_vars::TR_UPD_FLAG_SCROLL)
	.ENDIF	; TR_MIRRORING_VERTICAL

	sta inner_vars::_tr_upd_flags

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
	beq @upd_cont

	jsr move_down

@upd_cont:

	lda inner_vars::_tr_upd_flags
	and #inner_vars::TR_UPD_FLAG_DRAW_MASK
	beq @upd_exit

	and #inner_vars::TR_UPD_FLAG_DRAW_LEFT
	beq @upd_chck_right

	jsr _tr_drw_left_col

@upd_chck_right:

	lda inner_vars::_tr_upd_flags
	and #inner_vars::TR_UPD_FLAG_DRAW_RIGHT
	beq @upd_chck_up

	jsr _tr_drw_right_col

@upd_chck_up:

	lda inner_vars::_tr_upd_flags
	and #inner_vars::TR_UPD_FLAG_DRAW_UP
	beq @upd_chck_down

	jsr _tr_drw_up_row

@upd_chck_down:

	lda inner_vars::_tr_upd_flags
	and #inner_vars::TR_UPD_FLAG_DRAW_DOWN
	beq @upd_exit

	jsr _tr_drw_down_row

@upd_exit:
	; enable scrolling update on PPU
	lda inner_vars::_tr_upd_flags
	ora #inner_vars::TR_UPD_FLAG_SCROLL
	sta inner_vars::_tr_upd_flags

	rts

_tr_drw_up_row:

	; draw tiles

	; calculate the _nametable address in PPU
	lda inner_vars::_nametable
	jsr ppu_calc_nametable_addr

	lda inner_vars::_tr_pos_y
	lsr a
	lsr a
	lsr a

	pha					; save the line number in nametable

	tax

	ldy #$00
	mul32_xy
	add_xy_to_word data_addr

	; add X
	lda inner_vars::_tr_pos_x
	lsr a
	lsr a
	lsr a
	tax

	.IF ::TR_DATA_TILES4X4
	and #%00000011
	.ELSE
	and #%00000001
	.ENDIF; TR_DATA_TILES4X4

	sta inner_vars::_ignore_cnt		; save row offset in a tile or number of CHRs that should be skipped
	
	ldy #$00
	add_xy_to_word data_addr

	; calculate how many bytes of this nametable can be placed in the buffer
	lda data_addr
	and #%00011111
	sta _tmp_val			
	lda #SCR_WCHR_CNT
	sec
	sbc _tmp_val
	tax

	; make header to write the data packet to the buffer
	lda #TR_utils::TR_BUFF_FLAGS_INC1
	sta TR_utils::tr_data_flags
	txa
	sta inner_vars::_loop_cntr		; tiles counter

	jsr TR_utils::buff_push_hdr

	; calculate missing number of tiles to be drawn on another screen
	sec
	lda #SCR_WCHR_CNT + 1
	sbc inner_vars::_loop_cntr
	sta inner_vars::_extra_cnt

	; tiles address calculation
	.IF ::TR_DATA_TILES4X4
	div32_word_xy inner_vars::_tr_pos_x
	.ELSE
	div16_word_xy inner_vars::_tr_pos_x
	.ENDIF; TR_DATA_TILES4X4
	txa
	asl a

	jsr _get_tile_addr_tbl_val

	; select data from the table to search for blocks/CHR
	lda inner_vars::_tr_pos_y
	.IF ::TR_DATA_TILES4X4
	and #%00011111
	.ELSE
	and #%00001111
	.ENDIF; TR_DATA_TILES4X4
	lsr a
	lsr a
	lsr a

	pha					; save offset in a row

	.IF ::TR_DATA_TILES4X4
	asl a
	.ENDIF; TR_DATA_TILES4X4
	asl a
	sta inner_vars::_drw_rowcol_inds_offset

	jsr _drw_tiles_row_dyn
	jsr TR_utils::buff_end

	pla					; load position in a tile
	tay

	pop_x					; load a line number in the nametable

	.IF TR_MIRRORING_VERTICAL
	lda inner_vars::_tr_upd_flags
	and #inner_vars::TR_UPD_FLAG_FORCE_ATTRS
	bne _tr_drw_up_row_attrs
	.ENDIF	; TR_MIRRORING_VERTICAL

	tya

	.IF TR_MIRRORING_VERTICAL
	.IF ::TR_DATA_TILES4X4
	cmp #$00
	beq _tr_drw_up_row_attrs
	cmp #$02
	.ELSE
	cmp #$00
	.ENDIF	; ::TR_DATA_TILES4X4
	.ELSE
	.IF ::TR_DATA_TILES4X4
	cmp #$03
	.ELSE
	cmp #$01
	.ENDIF	; TR_DATA_TILES4X4
	.ENDIF	; TR_MIRRORING_VERTICAL
	beq _tr_drw_up_row_attrs		; if the position is zero, then it's time to draw the attributes column

	txa
	cmp #$1d				; now we trace attribute drawing in the last 29th line
	bne @exit

	tya
	cmp #$01				; when moving up, when switching screen, the first line in a tile is not #03, but #01
	beq _tr_drw_up_row_attrs

@exit:

	rts

_tr_drw_up_row_attrs:

	; draw attrs
	
	; calculate PPU address
	; ( inner_vars::_tr_pos_y / 32 ) * 8
	lda inner_vars::_tr_pos_y
	lsr a
	lsr a
	and #%11111000

	clc
	adc #$c0
	sta data_addr

	lda inner_vars::_nametable
	asl a
	asl a
	clc
	adc #$23
	sta data_addr + 1

	; add X
	lda inner_vars::_tr_pos_x
	lsr a
	lsr a
	lsr a
	lsr a
	lsr a
	tax
	
	ldy #$00
	add_xy_to_word data_addr

	; calculate how many bytes of attributes can be buffered
	lda data_addr
	and #%00000111
	sta _tmp_val			
	lda #SCR_WATTRS_CNT
	sec
	sbc _tmp_val
	tax

	; make header to write the data packet to the buffer
	lda #TR_utils::TR_BUFF_FLAGS_INC1
	sta TR_utils::tr_data_flags
	txa
	sta inner_vars::_loop_cntr			; tiles counter

	jsr TR_utils::buff_push_hdr

	; calculate missing number of tiles to be drawn on another screen
	sec
	lda #SCR_WATTRS_CNT + 1
	sbc inner_vars::_loop_cntr
	sta inner_vars::_extra_cnt

	; tiles address calculation
	.IF ::TR_DATA_TILES4X4
	div32_word_xy inner_vars::_tr_pos_x
	txa
	asl a

	jsr _get_tile_addr_tbl_val
	.ELSE
	jsr _get_attr_addr
	.ENDIF; TR_DATA_TILES4X4

	jsr _drw_attrs_row_dyn
	jmp TR_utils::buff_end

_tr_drw_down_row:

	; draw tiles

	; calculate PPU address
	lda inner_vars::_nametable
	.IF !TR_MIRRORING_VERTICAL
	eor #$02
	.ENDIF ; TR_MIRRORING_VERTICAL
	jsr ppu_calc_nametable_addr

	lda inner_vars::_tr_pos_y
	lsr a
	lsr a
	lsr a
	tax

	ldy #$00
	mul32_xy
	add_xy_to_word data_addr

	; add X
	lda inner_vars::_tr_pos_x
	lsr a
	lsr a
	lsr a
	tax

	.IF ::TR_DATA_TILES4X4
	and #%00000011
	.ELSE
	and #%00000001
	.ENDIF; TR_DATA_TILES4X4

	sta inner_vars::_ignore_cnt			; save row offset in a tile or number of CHRs that should be skipped
	
	ldy #$00
	add_xy_to_word data_addr

	; calculate how many bytes of this nametable can be buffered
	lda data_addr
	and #%00011111
	sta _tmp_val			
	lda #SCR_WCHR_CNT
	sec
	sbc _tmp_val
	tax

	; make header to write the data packet to the buffer
	lda #TR_utils::TR_BUFF_FLAGS_INC1
	sta TR_utils::tr_data_flags
	txa
	sta inner_vars::_loop_cntr			; tles counter

	jsr TR_utils::buff_push_hdr

	; calculate missing number of tiles to be drawn on another screen
	sec
	lda #SCR_WCHR_CNT + 1
	sbc inner_vars::_loop_cntr
	sta inner_vars::_extra_cnt

	; the tiles address calculation
	.IF ::TR_DATA_TILES4X4
	div32_word_xy inner_vars::_tr_pos_x
	.ELSE
	div16_word_xy inner_vars::_tr_pos_x
	.ENDIF; TR_DATA_TILES4X4
	txa
	asl a

	jsr _get_tile_addr_tbl_val

	lda #SCR_HTILES_CNT
	add_a_to_word _tmp_val

	; select data from the table to search for blocks / CHR
	lda inner_vars::_tr_pos_y
	.IF ::TR_DATA_TILES4X4
	and #%00011111
	.ELSE
	and #%00001111
	.ENDIF; TR_DATA_TILES4X4
	lsr a
	lsr a
	lsr a

	pha						; save offset in a row

	.IF ::TR_DATA_TILES4X4
	asl a
	.ENDIF; TR_DATA_TILES4X4
	asl a
	sta inner_vars::_drw_rowcol_inds_offset

	jsr _drw_tiles_row_dyn
	jsr TR_utils::buff_end

	pla						; load position in a tile
	tay

	.IF TR_MIRRORING_VERTICAL
	lda inner_vars::_tr_upd_flags
	and #inner_vars::TR_UPD_FLAG_FORCE_ATTRS
	bne _tr_drw_down_row_attrs
	.ENDIF	; TR_MIRRORING_VERTICAL

	tya

	.IF TR_MIRRORING_VERTICAL
	cmp #$01
	.ENDIF

	beq _tr_drw_down_row_attrs	; if the position is zero, then it's time to draw attribute column

	rts

_tr_drw_down_row_attrs:

	; draw attributes
	
	; calculate PPU address
	; ( inner_vars::_tr_pos_y / 32 ) * 8
	lda inner_vars::_tr_pos_y
	lsr a
	lsr a
	and #%11111000

	clc
	adc #$c0
	sta data_addr

	lda inner_vars::_nametable
	.IF !TR_MIRRORING_VERTICAL
	eor #$02
	.ENDIF	; TR_MIRRORING_VERTICAL
	asl a
	asl a
	clc
	adc #$23
	sta data_addr + 1

	; add X
	lda inner_vars::_tr_pos_x
	lsr a
	lsr a
	lsr a
	lsr a
	lsr a
	tax
	
	ldy #$00
	add_xy_to_word data_addr

	; calculate how many bytes of attributes can be buffered
	lda data_addr
	and #%00000111
	sta _tmp_val			
	lda #SCR_WATTRS_CNT
	sec
	sbc _tmp_val
	tax

	; make header to write the data packet to the buffer
	lda #TR_utils::TR_BUFF_FLAGS_INC1
	sta TR_utils::tr_data_flags
	txa
	sta inner_vars::_loop_cntr			; tiles counter

	jsr TR_utils::buff_push_hdr

	; calculate missing number of tiles to be drawn on another screen
	sec
	lda #SCR_WATTRS_CNT + 1
	sbc inner_vars::_loop_cntr
	sta inner_vars::_extra_cnt

	; tiles address calculation
	.IF ::TR_DATA_TILES4X4
	div32_word_xy inner_vars::_tr_pos_x
	txa
	asl a

	jsr _get_tile_addr_tbl_val

	lda #::SCR_HATTRS_CNT
	add_a_to_word _tmp_val
	.ELSE
	jsr _get_attr_addr

	lda #::SCR_HATTRS_CNT
	add_a_to_word _tmp_val
	.ENDIF; TR_DATA_TILES4X4

	jsr _drw_attrs_row_dyn
	jmp TR_utils::buff_end

_tr_drw_left_col:

	; draw tiles

	; calculate PPU address
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

	; add Y
	lda inner_vars::_tr_pos_y
	lsr a
	lsr a
	lsr a
	tax

	.IF ::TR_DATA_TILES4X4
	and #%00000011
	.ELSE
	and #%00000001
	.ENDIF; TR_DATA_TILES4X4

	sta inner_vars::_ignore_cnt			; save row offset in a tile or number of CHRs that should be skipped

	ldy #$00
	mul32_xy
	add_xy_to_word data_addr

	; calculate how many bytes of this nametable can be buffered
	lda inner_vars::_nametable
	asl a
	asl a
	clc
	adc #$23
	tay 
	; ( XY - data_addr ) >> 5 = number of bytes to write to the nametable
	clc
	lda data_addr
	sta _tmp_val
	lda #$c0
	sbc _tmp_val
	tax
	tya
	sbc data_addr + 1
	tay
	div32_xy
	inx

	; make header to write the data packet to the buffer
	lda #TR_utils::TR_BUFF_FLAGS_INC32
	sta TR_utils::tr_data_flags
	txa
	sta inner_vars::_loop_cntr			; tiles counter

	jsr TR_utils::buff_push_hdr

	; calculate missing number of tiles to be drawn on another screen
	sec
	lda #SCR_HCHR_CNT + 1
	sbc inner_vars::_loop_cntr
	sta inner_vars::_extra_cnt

	; tiles address calculation
	.IF ::TR_DATA_TILES4X4
	div32_word_xy inner_vars::_tr_pos_x
	.ELSE
	div16_word_xy inner_vars::_tr_pos_x
	.ENDIF; TR_DATA_TILES4X4
	txa
	asl a

	jsr _get_tile_addr_tbl_val

	; select data from the table to search for blocks / CHR
	lda inner_vars::_tr_pos_x
	.IF ::TR_DATA_TILES4X4
	and #%00011111
	.ELSE
	and #%00001111
	.ENDIF; TR_DATA_TILES4X4
	lsr a
	lsr a
	lsr a

	pha						; save column index in a tile

	.IF ::TR_DATA_TILES4X4
	asl a
	asl a
	clc
	adc #$10
	.ELSE
	asl a
	clc
	adc #$04
	.ENDIF; TR_DATA_TILES4X4
	sta inner_vars::_drw_rowcol_inds_offset

	jsr _drw_tiles_col_dyn
	jsr TR_utils::buff_end

	pla						; get position in a tile
	.IF ::TR_DATA_TILES4X4
	cmp #$03
	.ELSE
	cmp #$01
	.ENDIF; TR_DATA_TILES4X4
	beq _tr_drw_left_col_attrs	; if the position is rightmost, then it's time to draw an attributes column

	rts

_tr_drw_left_col_attrs:

	; draw attributes
	
	; calculate the PPU address
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

	; add Y
	lda inner_vars::_tr_pos_y
	lsr a
	lsr a
	lsr a
	lsr a
	lsr a
	tax
	
	ldy #$00
	mul8_xy
	add_xy_to_word data_addr

	; calculate how many bytes of attributes can be buffered
	clc
	ldx #$00
	lda #$01
	adc data_addr + 1
	tay 
	; ( XY - data_addr ) >> 3 = number of bytes to write to the nametable
	clc
	txa
	sbc data_addr
	tax
	tya
	sbc data_addr + 1
	tay
	div8_xy
	inx

	; make header to write the data packet to the buffer
	lda #TR_utils::TR_BUFF_FLAGS_INC8
	sta TR_utils::tr_data_flags
	txa
	sta inner_vars::_loop_cntr			; tiles counter

	jsr TR_utils::buff_push_hdr

	; calculate missing number of tiles to be drawn on another screen
	sec
	lda #::SCR_HATTRS_CNT + 1
	sbc inner_vars::_loop_cntr
	sta inner_vars::_extra_cnt

	; tile address calculation
	.IF ::TR_DATA_TILES4X4
	div32_word_xy inner_vars::_tr_pos_x
	txa
	asl a

	jsr _get_tile_addr_tbl_val
	.ELSE
	jsr _get_attr_addr
	.ENDIF; TR_DATA_TILES4X4

	jsr _drw_attrs_col_dyn
	jmp TR_utils::buff_end

_tr_drw_right_col:

	; draw tiles

	; calculate PPU address
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

	; add Y
	lda inner_vars::_tr_pos_y
	lsr a
	lsr a
	lsr a
	tax

	.IF ::TR_DATA_TILES4X4			
	and #%00000011
	.ELSE
	and #%00000001
	.ENDIF; TR_DATA_TILES4X4

	sta inner_vars::_ignore_cnt			; save row offset in a tile or number of CHRs that should be skipped

	ldy #$00
	mul32_xy
	add_xy_to_word data_addr

	; calculate how many bytes of this nametable can be buffered
	lda inner_vars::_nametable
	eor #$01
	asl a
	asl a
	clc
	adc #$23
	tay 
	; ( XY - data_addr ) >> 5 = number of bytes to write to the nametable
	clc
	lda data_addr
	sta _tmp_val
	lda #$c0
	sbc _tmp_val
	tax
	tya
	sbc data_addr + 1
	tay
	div32_xy
	inx

	; make header to write the data packet to the buffer
	lda #TR_utils::TR_BUFF_FLAGS_INC32
	sta TR_utils::tr_data_flags
	txa
	sta inner_vars::_loop_cntr			; tiles counter

	jsr TR_utils::buff_push_hdr

	; calculate missing number of tiles to be drawn on another screen
	sec
	lda #SCR_HCHR_CNT + 1
	sbc inner_vars::_loop_cntr
	sta inner_vars::_extra_cnt

	; tile address calculation
	.IF ::TR_DATA_TILES4X4
	div32_word_xy inner_vars::_tr_pos_x
	.ELSE
	div16_word_xy inner_vars::_tr_pos_x
	.ENDIF; TR_DATA_TILES4X4
	stx _tmp_val
	lda #SCR_WTILES_CNT
	clc
	adc _tmp_val
	asl a

	jsr _get_tile_addr_tbl_val

	; select data from the table to search for blocks / CHR
	lda inner_vars::_tr_pos_x
	.IF ::TR_DATA_TILES4X4
	and #%00011111
	.ELSE
	and #%00001111
	.ENDIF; TR_DATA_TILES4X4
	lsr a
	lsr a
	lsr a

	pha						; save offset in a column

	.IF ::TR_DATA_TILES4X4
	asl a
	asl a
	clc
	adc #$10
	.ELSE
	asl a
	clc
	adc #$04
	.ENDIF; TR_DATA_TILES4X4
	sta inner_vars::_drw_rowcol_inds_offset

	jsr _drw_tiles_col_dyn
	jsr TR_utils::buff_end

	pla						; load position in a tile
	beq _tr_drw_right_col_attrs			; if the position is zero, then it's time to draw attribute column

	rts

_tr_drw_right_col_attrs:

	; draw attributes

	; calculate PPU address
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

	; add Y
	lda inner_vars::_tr_pos_y
	lsr a
	lsr a
	lsr a
	lsr a
	lsr a
	tax
	
	ldy #$00
	mul8_xy
	add_xy_to_word data_addr

	; calculate how many bytes of attributes can be buffered
	clc
	ldx #$00
	lda #$01
	adc data_addr + 1
	tay 
	; ( XY - data_addr ) >> 3 = number of bytes to write to the nametable
	clc
	txa
	sbc data_addr
	tax
	tya
	sbc data_addr + 1
	tay
	div8_xy
	inx

	; make header to write the data packet to the buffer
	lda #TR_utils::TR_BUFF_FLAGS_INC8
	sta TR_utils::tr_data_flags
	txa
	sta inner_vars::_loop_cntr			; tiles counter

	jsr TR_utils::buff_push_hdr

	; calculate missing number of tiles to be drawn on another screen
	sec
	lda #::SCR_HATTRS_CNT + 1
	sbc inner_vars::_loop_cntr
	sta inner_vars::_extra_cnt

	; tile address calculation
	.IF ::TR_DATA_TILES4X4
	div32_word_xy inner_vars::_tr_pos_x
	stx _tmp_val
	lda #SCR_WATTRS_CNT
	clc
	adc _tmp_val
	asl a

	jsr _get_tile_addr_tbl_val
	.ELSE
	lda #.lobyte( SCR_WCHR_CNT << 3 )
	sta _tmp_val
	lda #.hibyte( SCR_WCHR_CNT << 3 )
	sta _tmp_val + 1

	add_word_to_word inner_vars::_tr_pos_x, _tmp_val

	jsr _get_attr_addr_interm
	.ENDIF; TR_DATA_TILES4X4

	jsr _drw_attrs_col_dyn
	jmp TR_utils::buff_end

_drw_attrs_row_dyn:

	; draw tiles row

@drw_attrs_row_dyn_loop:

	; read tile
	ldy #$00
	lda (<_tmp_val), y

	.IF ::TR_DATA_TILES4X4
	tay
	lda (<tr_attrs), y		; read attr
	.ENDIF; TR_DATA_TILES4X4

	jsr TR_utils::buff_push_data

	.IF ::TR_DATA_TILES4X4
	lda tr_htiles
	.ELSE
	lda tr_hattrs_cnt
	.ENDIF; TR_DATA_TILES4X4
	add_a_to_word _tmp_val

	dec inner_vars::_loop_cntr
	bne @drw_attrs_row_dyn_loop

	lda inner_vars::_extra_cnt
	beq @drw_attrs_row_dyn_exit

	sta inner_vars::_loop_cntr

	lda #$00
	sta inner_vars::_extra_cnt

	lda data_addr + 1
	eor #$04			; get horizontal adjacent screen index
	sta data_addr + 1

	lda data_addr
	and #%11111000
	sta data_addr

	lda inner_vars::_loop_cntr
	jsr TR_utils::buff_push_hdr

	jmp @drw_attrs_row_dyn_loop

@drw_attrs_row_dyn_exit:

	rts

_drw_attrs_col_dyn:

	; draw attributes column
	ldy #$00

@drw_attrs_col_dyn_loop:

	; read tile
	push_y				;<----
	lda (<_tmp_val), y

	.IF ::TR_DATA_TILES4X4
	tay
	lda (<tr_attrs), y		; read attr
	.ENDIF; TR_DATA_TILES4X4
	
	jsr TR_utils::buff_push_data

	pop_y
	iny

	dec inner_vars::_loop_cntr
	bne @drw_attrs_col_dyn_loop

	lda inner_vars::_extra_cnt
	beq @exit

	sta inner_vars::_loop_cntr

	lda #$00
	sta inner_vars::_extra_cnt

	lda data_addr + 1
	and #%00101111
	.IF !TR_MIRRORING_VERTICAL
	eor #$08			; get vertical adjacent screen index
	.ENDIF
	sta data_addr + 1

	lda data_addr
	and #%11000111
	sta data_addr

	push_y

	lda inner_vars::_loop_cntr
	jsr TR_utils::buff_push_hdr

	pop_y

	jmp @drw_attrs_col_dyn_loop

@exit:

	rts

_drw_tiles_row_dyn:

	; draw tiles row

@drw_tiles_row_dyn_loop:

	; read tile
	ldy #$00
	lda (<_tmp_val), y

	; A *= 4
	mul4_a_xy

	.IF ::TR_DATA_TILES4X4
	add_word_to_xy tr_tiles		; XY contain address of tile's blocks
	.ELSE
	add_word_to_xy tr_blocks	; XY contain address of 2x2 tile
	.ENDIF; TR_DATA_TILES4X4

	stx TR_utils::inner_vars::_tile_addr
	sty TR_utils::inner_vars::_tile_addr + 1

	.IF ::TR_DATA_TILES4X4
	ldy inner_vars::_drw_rowcol_inds_offset		;3
	lda _drw_rowcol_inds_tbl, y			;4
	tay						;2

	lda (<TR_utils::inner_vars::_tile_addr), y	; 1 block

	jsr _drw_blocks_dyn_xy

	bcs @check_exit

	ldy inner_vars::_drw_rowcol_inds_offset		;3
	iny						;2
	lda _drw_rowcol_inds_tbl, y			;4
	tay						;2

	lda (<TR_utils::inner_vars::_tile_addr), y	; 2 block

	jsr _drw_blocks_dyn_xy

	bcs @check_exit
	.ELSE
	lda inner_vars::_ignore_cnt
	bne @cont1

	ldy inner_vars::_drw_rowcol_inds_offset		;3
	lda _drw_rowcol_inds_tbl, y			;4
	tay						;2

	lda (<TR_utils::inner_vars::_tile_addr), y	; 1 block

	jsr TR_utils::buff_push_data

	dec inner_vars::_loop_cntr
	beq @check_exit

	jmp @cont2

@cont1:

	dec inner_vars::_ignore_cnt

@cont2:

	ldy inner_vars::_drw_rowcol_inds_offset		;3
	iny
	lda _drw_rowcol_inds_tbl, y			;4
	tay						;2

	lda (<TR_utils::inner_vars::_tile_addr), y	; 2 block

	jsr TR_utils::buff_push_data

	dec inner_vars::_loop_cntr
	beq @check_exit
	.ENDIF; TR_DATA_TILES4X4

	lda tr_htiles
	add_a_to_word _tmp_val

	jmp @drw_tiles_row_dyn_loop

@check_exit:

	lda inner_vars::_extra_cnt
	beq @exit

	sta inner_vars::_loop_cntr

	lda #$00
	sta inner_vars::_extra_cnt

	lda data_addr + 1
	eor #$04					; get horizontal adjacent screen index
	sta data_addr + 1

	lda data_addr
	and #%11100000
	sta data_addr

	lda inner_vars::_loop_cntr

	jsr TR_utils::buff_push_hdr

	lda tr_htiles
	add_a_to_word _tmp_val

	jmp @drw_tiles_row_dyn_loop

@exit:

	rts

_drw_tiles_col_dyn:

	; draw tiles column
	ldy #$00

@drw_tiles_col_dyn_loop:

	; read tile
	push_y				;<----
	lda (<_tmp_val), y

	; A *= 4
	mul4_a_xy

	.IF ::TR_DATA_TILES4X4
	add_word_to_xy tr_tiles		; XY contain address of tile's blocks
	.ELSE
	add_word_to_xy tr_blocks	; XY contain address of 2x2 tile
	.ENDIF; TR_DATA_TILES4X4

	stx TR_utils::inner_vars::_tile_addr
	sty TR_utils::inner_vars::_tile_addr + 1

	.IF ::TR_DATA_TILES4X4
	ldy inner_vars::_drw_rowcol_inds_offset		;3
	lda _drw_rowcol_inds_tbl, y			;4
	tay						;2

	lda (<TR_utils::inner_vars::_tile_addr), y	; 1 block

	jsr _drw_blocks_dyn_xy

	bcs @check_exit

	ldy inner_vars::_drw_rowcol_inds_offset		;3
	iny
	lda _drw_rowcol_inds_tbl, y			;4
	tay						;2

	lda (<TR_utils::inner_vars::_tile_addr), y	; 2 block

	jsr _drw_blocks_dyn_xy

	bcs @check_exit
	.ELSE
	lda inner_vars::_ignore_cnt
	bne @cont1

	ldy inner_vars::_drw_rowcol_inds_offset		;3
	lda _drw_rowcol_inds_tbl, y			;4
	tay						;2

	lda (<TR_utils::inner_vars::_tile_addr), y	; 1 block

	jsr TR_utils::buff_push_data

	dec inner_vars::_loop_cntr
	beq @check_exit

	jmp @cont2

@cont1:

	dec inner_vars::_ignore_cnt

@cont2:

	ldy inner_vars::_drw_rowcol_inds_offset		;3
	iny
	lda _drw_rowcol_inds_tbl, y			;4
	tay						;2

	lda (<TR_utils::inner_vars::_tile_addr), y	; 2 block

	jsr TR_utils::buff_push_data

	dec inner_vars::_loop_cntr
	beq @check_exit
	.ENDIF; TR_DATA_TILES4X4

	pop_y
	iny					;<----

	jmp @drw_tiles_col_dyn_loop

@check_exit:

	lda inner_vars::_extra_cnt
	beq @exit

	sta inner_vars::_loop_cntr

	lda #$00
	sta inner_vars::_extra_cnt

	lda data_addr + 1
	and #%00101100
	.IF !TR_MIRRORING_VERTICAL
	eor #$08					; get vertical adjacent screen index
	.ENDIF
	sta data_addr + 1

	lda data_addr
	and #%00011111
	sta data_addr

	lda inner_vars::_loop_cntr

	jsr TR_utils::buff_push_hdr

	pop_y
	iny

	jmp @drw_tiles_col_dyn_loop

@exit:

	pop_y

	rts

	.IF ::TR_DATA_TILES4X4
_drw_blocks_dyn_xy:

	mul4_a_xy

	add_word_to_xy tr_blocks			; XY contain blocks address
	stx TR_utils::inner_vars::_block_addr
	sty TR_utils::inner_vars::_block_addr + 1

	lda inner_vars::_ignore_cnt
	bne @cont1

	ldy inner_vars::_drw_rowcol_inds_offset		;3
	iny
	iny
	lda _drw_rowcol_inds_tbl, y			;4
	tay						;2

	lda (<TR_utils::inner_vars::_block_addr), y
	jsr TR_utils::buff_push_data

	dec inner_vars::_loop_cntr
	beq @exit

	jmp @cont2

@cont1:

	dec inner_vars::_ignore_cnt

@cont2:

	lda inner_vars::_ignore_cnt
	bne @cont3

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

@cont3:

	dec inner_vars::_ignore_cnt

	clc
	rts						; reset carry flag and continue drawing

@exit:
	sec						; set carry flag as a sign of the end

	rts
	.ENDIF; TR_DATA_TILES4X4

tr_get_prop:
	; get block property by input coordinates
	; IN: inner_vars::_tr_pos_x, inner_vars::_tr_pos_y
	; OUT: A - prop index

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

TR_HTILES	= TR_ms::tr_htiles
TR_BLOCKS	= TR_ms::tr_blocks

	.IF ::TR_DATA_TILES4X4
TR_TILES	= TR_ms::tr_tiles
TR_ATTRS	= TR_ms::tr_attrs
	.ELSE
TR_HATTRS_CNT	= TR_ms::tr_hattrs_cnt	
	.ENDIF	; TR_DATA_TILES4X4