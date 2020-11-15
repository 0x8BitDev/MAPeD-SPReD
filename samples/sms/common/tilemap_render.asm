;########################################################################
;
; Some helpful tilemap renderers code
;
; Copyright 2020 0x8BitDev ( MIT license )
;
;########################################################################


; Public routines:
;
;	tr_init
;	tr_jpad_update
;	tr_move_right
;	tr_move_left
;	tr_move_up
;	tr_move_down
;	update_scroll
;

; disable bidirectional scrolling and use static screens switching
;.define	TR_BIDIR_STAT_SCR


.if MAP_DATA_MAGIC&MAP_FLAG_DIR_ROWS == MAP_FLAG_DIR_ROWS
	.printt "*** ERROR: The sample doesn't support row ordered data! ***\n"
	.fail
.endif

.if	( MAP_DATA_MAGIC & MAP_FLAG_MODE_MULTIDIR_SCROLL ) == MAP_FLAG_MODE_MULTIDIR_SCROLL
.define	TR_MULTIDIR_SCROLL
.define	TR_SCROLL
.else
.if	( MAP_DATA_MAGIC & MAP_FLAG_MODE_BIDIR_SCROLL ) == MAP_FLAG_MODE_BIDIR_SCROLL
.define	TR_BIDIR_SCROLL
.define	TR_BIDIR		; for both cases: bidir scroll and bidir stat scr
.define	TR_SCROLL
.endif
.endif

.if	( MAP_DATA_MAGIC & MAP_FLAG_TILES4X4 ) == MAP_FLAG_TILES4X4
.define	TR_DATA_TILES4X4
.endif

.if	( MAP_DATA_MAGIC & MAP_FLAG_RLE ) == MAP_FLAG_RLE
.define	TR_DATA_RLE
.endif

.if	( MAP_DATA_MAGIC & MAP_FLAG_DIR_COLUMNS ) == MAP_FLAG_DIR_COLUMNS
.define	TR_DATA_COLUMN_ORDER
.endif

.if	( MAP_DATA_MAGIC & MAP_FLAG_MARKS ) == MAP_FLAG_MARKS
.define	TR_DATA_MARKS
.endif

.if	( MAP_DATA_MAGIC & MAP_FLAG_LAYOUT_ADJACENT_SCREENS ) == MAP_FLAG_LAYOUT_ADJACENT_SCREENS
.define	TR_ADJACENT_SCR
.endif

.if	( MAP_DATA_MAGIC & MAP_FLAG_LAYOUT_ADJACENT_SCR_INDS ) == MAP_FLAG_LAYOUT_ADJACENT_SCR_INDS
.define	TR_ADJACENT_SCR_INDS
.endif

.ifdef	TR_BIDIR_STAT_SCR
.undef	TR_BIDIR_SCROLL
.undef	TR_SCROLL
.endif

; *** constants ***

.define TR_MOVE_STEP		2		; 1/2/4 ???

.define TR_UPD_FLAG_SCROLL	%00000001
.define TR_UPD_FLAG_DRAW_UP	%00000010
.define TR_UPD_FLAG_DRAW_DOWN	%00000100
.define TR_UPD_FLAG_DRAW_LEFT	%00001000
.define TR_UPD_FLAG_DRAW_RIGHT	%00010000

.define TR_UPD_FLAG_DRAW_MASK	TR_UPD_FLAG_DRAW_UP | TR_UPD_FLAG_DRAW_DOWN | TR_UPD_FLAG_DRAW_LEFT | TR_UPD_FLAG_DRAW_RIGHT

.ifdef	TR_MULTIDIR_SCROLL
.define	TR_COLUMN_DATA_HEADER	TR_BUFF_STEP_64 | 25
.define	TR_ROW_DATA_HEADER	32
.else
.define	TR_COLUMN_DATA_HEADER	TR_BUFF_STEP_64 | 24
.define	TR_ROW_DATA_HEADER	31

; adjacent screen masks
.define TR_MASK_ADJ_SCR_LEFT	%00010000
.define TR_MASK_ADJ_SCR_UP	%00100000
.define TR_MASK_ADJ_SCR_RIGHT	%01000000
.define TR_MASK_ADJ_SCR_DOWN	%10000000

.endif

.ifdef	TR_SCROLL

; *** tiles column/row drawing routines tables ***

_tr_tiles_col_routine_tbl:
	.dw _tr_tile_data_col0, _tr_tile_data_col1
.ifdef	TR_DATA_TILES4X4
	.dw _tr_tile_data_col2, _tr_tile_data_col3
.endif

_tr_tiles_row_routine_tbl:
	.dw _tr_tile_data_row0, _tr_tile_data_row1
.ifdef	TR_DATA_TILES4X4
	.dw _tr_tile_data_row2, _tr_tile_data_row3
.endif

.endif	;TR_SCROLL

; *** useful macroses ***

.ifdef	TR_BIDIR_SCROLL

; *** get screen tiles addr ***
; IN:	HL - screen data addr
; OUT:	HL - tiles addr

.macro	GET_SCREEN_TILES_ADDR

	inc hl

.ifdef	TR_DATA_MARKS
	inc hl
.endif
	call _tr_get_tiles_addr
.endm

.endif	;TR_BIDIR_SCROLL


.ifdef	TR_BIDIR

.macro	SCREEN_ON
	VDP_WRITE_REG_CMD 1 VDPR1_FIXED|VDPR1_VBLANK|VDPR1_DISPLAY_ON
.endm

.macro	SCREEN_OFF
	VDP_WRITE_REG_CMD 1 VDPR1_FIXED|VDPR1_VBLANK
.endm

; *** get adjacent screen data addr ***
; OUT: 	HL - new screen data addr

.macro	GET_ADJACENT_SCREEN	; \1 - adjacent screen mask direction value, \2 - adjacent screen label offset,  \3 - adjacent screen index offset

	inc hl

.ifdef	TR_DATA_MARKS

	ld a, (hl)
	and \1
	ret z

	inc hl
.endif

.ifdef	TR_ADJACENT_SCR

	ld de, \2
	add hl, de

	ld e, (hl)
	inc hl
	ld d, (hl)

	ex de, hl
.endif

.ifdef	TR_ADJACENT_SCR_INDS

	ld de, \3
	add hl, de

	ld a, (hl)
	cp $ff
	ret z

	MUL_POW2_A 1
	ld e, a
	ld d, 0
	ld hl, (TR_SCR_LABELS_ARR)
	add hl, de

	ld e, (hl)
	inc hl
	ld d, (hl)

	ex de, hl
.endif
.endm

.endif	;TR_BIDIR

.macro TR_PUT_12ATTR_TO_VRAM	; a - attribute index
	exx
	ld hl, (TR_BLOCK_ATTRS)

	ld c, a
	ld b, 0

	MUL_POW2_BC 3

	add hl, bc

.ifdef	TR_BIDIR_SCROLL
	ld bc, (tr_tiles_offset)
	MUL_POW2_BC 2
	add hl, bc
.endif

	VDP_SCR_ATTR_TO_DATA_REG_HL
	VDP_SCR_ATTR_TO_DATA_REG_HL

	exx
.endm

.macro TR_PUT_34ATTR_TO_VRAM	; a - attribute index
	exx

	VDP_SCR_ATTR_TO_DATA_REG_HL
	VDP_SCR_ATTR_TO_DATA_REG_HL

	exx
.endm

; IN: DE - VRAM addr
;     HL - block data index addr

.macro TR_PUT_BLOCK_TO_VRAM	; \1 - next block offset in bytes
	ld c, (hl)

	push hl
	push de				; blocks data
	call _tr_put_block
	pop de

	; move to the next block

.if \1 != 0
	ld hl, \1
	add hl, de

	ex de, hl
.endif
	pop hl
.endm

.macro TR_SET_DRAW_FLAG_UP
	ld a, (tr_flags)
	or TR_UPD_FLAG_DRAW_UP
	ld (tr_flags), a
.endm

.macro TR_SET_DRAW_FLAG_DOWN
	ld a, (tr_flags)
	or TR_UPD_FLAG_DRAW_DOWN
	ld (tr_flags), a
.endm

.macro TR_SET_DRAW_FLAG_LEFT
	ld a, (tr_flags)
	or TR_UPD_FLAG_DRAW_LEFT
	ld (tr_flags), a
.endm

.macro TR_SET_DRAW_FLAG_RIGHT
	ld a, (tr_flags)
	or TR_UPD_FLAG_DRAW_RIGHT
	ld (tr_flags), a
.endm

.macro	TR_FILL_TILE_COLROW	; \1 - first block offset, \2 - second block offset, \3 - first CHR offset, \4 - second CHR offset
	exx

	ld e, (hl)
	ld d, 0

	push hl

.ifdef	TR_DATA_TILES4X4

	MUL_POW2_DE 2

	ld hl, (TR_TILES4x4)
	add hl, de

.ifdef	TR_BIDIR_SCROLL
	ld de, (tr_tiles_offset)
	add hl, de
.endif	

.repeat \1
	inc hl
.endr

	ld e, (hl)			; e - 0 tile's block index
	ld d, 0

	push hl

	call _tr_fill_block_CHR\@

	pop hl

.repeat \2
	inc hl
.endr

	ld e, (hl)			; d - 2 tile's block index
	ld d, 0
.endif
	call _tr_fill_block_CHR\@

	pop hl

	ret

; *** fill block's CHRs ***
; IN: DE - block index

_tr_fill_block_CHR\@:

	MUL_POW2_DE 3

	ld hl, (TR_BLOCK_ATTRS)
	add hl, de

.ifdef	TR_BIDIR_SCROLL
	ld de, (tr_tiles_offset)
	MUL_POW2_DE 2
	add hl, de
.endif

.repeat \3
	inc hl
.endr

	ld e, (hl)
	inc hl
	ld d, (hl)
	inc hl

	ld a, b
	or a
	jp z, _tr_fill_block_CHR_cont1\@

	dec b				; dec skipped CHRs count

	jp _tr_fill_block_CHR_cont2\@

_tr_fill_block_CHR_cont1\@:

	ld a, c
	or a
	ret z
	dec a				; dec CHRs count
	ld c, a

	push hl

	call buff_push_data		; push 0 CHR

	pop hl

_tr_fill_block_CHR_cont2\@:

	ld a, b
	or a
	jp z, _tr_fill_block_CHR_cont3\@

	dec b				; dec skipped CHRs count

	ret

_tr_fill_block_CHR_cont3\@:

	ld a, c
	or a
	ret z
	dec a				; dec CHRs count
	ld c, a

.repeat \4
	inc hl
.endr
	ld e, (hl)
	inc hl
	ld d, (hl)

	jp buff_push_data		; push 2 CHR
.endm	

; *** routines ***

; *** tilemap render init ***
;
; IN: 'tilemap_render_vars'
;
; OUT: start screen image
;

tr_init:

	xor a
	ld (tr_scroll_x), a
	ld (tr_scroll_y), a
	ld (tr_flags), a

.ifdef	TR_MULTIDIR_SCROLL
	ld hl, Lev0_WPixelsCnt - ScrPixelsWidth
	ld (tr_map_pix_cropped_width), hl

	ld hl, Lev0_HPixelsCnt - ScrPixelsHeight
	ld (tr_map_pix_cropped_height), hl
.endif	;TR_MULTIDIR_SCROLL

	call buff_reset

.ifdef	TR_MULTIDIR_SCROLL
	ld a, (TR_START_SCR)
.else

.ifdef	TR_BIDIR_STAT_SCR
	ld hl, $ffff
	ld (tr_jpad_state), hl
.endif	;TR_BIDIR_STAT_SCR

	ld a, $ff
	ld (tr_CHR_id), a

.ifdef	TR_BIDIR_SCROLL
	ld hl, 0
	ld (tr_horiz_dir_pos), hl
	ld (tr_vert_dir_pos), hl
.endif	;TR_BIDIR_SCROLL

	ld hl, (TR_START_SCR)
	ld (tr_curr_scr), hl

	call _tr_upload_palette_tiles
.endif	;TR_MULTIDIR_SCROLL

	call _tr_get_tiles_addr		; hl - tiles addr

	ld de, TR_VRAM_SCR_ATTR_ADDR

	jp _tr_draw_screen

; *** fill the screen area by tiles data stored in HL ***
;
; IN: 	HL - tiles addr
;	DE - VRAM addr
;
; OUT: filled screen area
;

_tr_draw_screen:

	ld b, ScrTilesWidth

_drw_tiles_row:

	ld c, ScrTilesHeight

_drw_tiles_col:

	push bc	

	ld c, (hl)			; tile index
	ld b, 0

.ifdef	TR_DATA_TILES4X4
	MUL_POW2_BC 2
.else
	MUL_POW2_BC 3
.endif

	push hl

	push de

.ifdef	TR_DATA_TILES4X4
	ld hl, (TR_TILES4x4)
	add hl, bc

.ifdef	TR_BIDIR_SCROLL
	ld bc, (tr_tiles_offset)
	add hl, bc
.endif	

	TR_PUT_BLOCK_TO_VRAM 4
	inc hl
	TR_PUT_BLOCK_TO_VRAM 124
	inc hl
	TR_PUT_BLOCK_TO_VRAM 4
	inc hl
	TR_PUT_BLOCK_TO_VRAM 0
.else
	TR_PUT_BLOCK_TO_VRAM 0
.endif

	pop de

	; move down to the next tile in attributes area

.ifdef	TR_DATA_TILES4X4
	ld hl, $100
.else
	ld hl, $80
.endif
	add hl, de

	ex de, hl

	pop hl
	inc hl

	pop bc

	dec c
	jp nz, _drw_tiles_col

	push de

	; move to the next tiles column

	ld de, (TR_MAP_TILES_HEIGHT)
	add hl, de
	ld de, -ScrTilesHeight
	add hl, de

	pop de

	push hl

	; move to the next attributes column

	ex de, hl
.ifdef	TR_DATA_TILES4X4	
	ld de, -($600 - $08)
.else
	ld de, -($600 - $04)
.endif
	add hl, de

	ex de, hl

	pop hl

	dec b
	jp nz, _drw_tiles_row

	ret

; *** put a block on screen ***
;
; IN:	C - block index
;	DE - VRAM addr
;

_tr_put_block:

	VDP_WRITE_RAM_CMD_DE

	ld a, d
	and ~VDP_CMD_WRITE_RAM_HW		; reset VDP_CMD_WRITE_RAM flag
	ld d, a

	ld a, c
	TR_PUT_12ATTR_TO_VRAM

	; move to the next attribute line

	ld hl, $40
	add hl, de

	ex de, hl

	VDP_WRITE_RAM_CMD_DE
	
	TR_PUT_34ATTR_TO_VRAM

	ret

.ifdef	TR_MULTIDIR_SCROLL

; *** get tiles addr by input screen number ***
;
; IN: A - screen number
;
; OUT: HL - tiles addr
;

_tr_get_tiles_addr:

	; x_offset = TR_MAP_LUT[ ScrTilesWidth * ( TR_START_SCR % TR_MAP_SCR_WIDTH ) ]

	ld c, a

	ld a, (TR_MAP_SCR_WIDTH)
	ld d, a

	call c_div_d

	push bc				; c = c div d

	ld c, a				; a - remainder
	ld d, ScrTilesWidth

	call d_mul_c

	ld b, c
	ld c, a				; bc - product

	; calc X camera pos

	ld l, c
	ld h, b

.ifdef	TR_DATA_TILES4X4
	MUL_POW2_HL 5
.else	
	MUL_POW2_HL 4
.endif	

	ld (tr_pos_x), hl

	MUL_POW2_BC 1

	ld hl, (TR_MAP_LUT)

	add hl, bc

	ld e, (hl)
	inc hl
	ld d, (hl)			; de = x_offset

	pop bc				; c = TR_START_SCR / TR_MAP_SCR_WIDTH
	push de

	; y_offset = ScrTilesHeight * ( TR_START_SCR / TR_MAP_SCR_WIDTH )

	ld a, ScrTilesHeight
	ld d, a

	call d_mul_c

	ld d, c
	ld e, a				; de = y_offset

	; calc Y camera pos

	ld l, e
	ld h, d

.ifdef	TR_DATA_TILES4X4
	MUL_POW2_HL 5
.else
	MUL_POW2_HL 4
.endif

	ld (tr_pos_y), hl

	ex de, hl			; hl = y_offset

	pop de				; x_offset

	; tiles_addr = *TR_TILES_MAP + x_offset + y_offset

	add hl, de			; hl = x_offset + y_offset

	ld de, (TR_TILES_MAP)
	add hl, de			; hl - tiles_addr

	ret
.else

; *** upload palette and tiles to VDP ***
; IN: HL - screen data addr

_tr_upload_palette_tiles:

	SCREEN_OFF

	ld a, (tr_CHR_id)
	ld c, a

	ld a, (hl)			; a - CHR data index
	cp c

	jp z, _tr_upload_palette_tiles_exit

	ld (tr_CHR_id), a

	ld c, a
	ld b, 0

	push hl
	push bc

	; load palette into the first colors group

	VDP_WRITE_CLR_CMD $0000

	ld a, c

	MUL_POW2_A 4			; a x= 16

	ld e, a
	ld d, 0

	ld hl, (TR_PALETTES_ARR)
	add hl, de

	ld b, 16

	ld c, VDP_CMD_DATA_REG
	otir

	; load tiles

	pop bc
	push bc

	MUL_POW2_C 1

	ld hl, (TR_CHR_ARR)
	add hl, bc

	ld e, (hl)
	inc hl
	ld d, (hl)			; de - CHRs addr

	push de

	ld hl, (TR_CHR_SIZE_ARR)
	add hl, bc

	ld c, (hl)
	inc hl
	ld b, (hl)			; de - CHR data size

	pop hl

	ld de, $0000 + ($20 * MAP_CHRS_OFFSET)	; VRAM addr (the first CHR bank)

	call VDP_load_tiles

	pop bc

	; calc tiles offset

	MUL_POW2_BC 1

	ld hl, (TR_TILES_OFFSETS)
	add hl, bc

	ld e, (hl)
	inc hl
	ld d, (hl)

	ld (tr_tiles_offset), de

	pop hl

_tr_upload_palette_tiles_exit:

	inc hl

.ifdef	TR_DATA_MARKS
	inc hl
.endif

	SCREEN_ON

	ret

;
; IN: HL - screen index addr
;
; OUT: HL - tiles addr
;

_tr_get_tiles_addr:

	ld c, (hl)			; c - screen index
	ld b, 0

	ld l, c
	ld h, b

.ifdef	TR_DATA_TILES4X4

	; de = bc x 48

	MUL_POW2_BC 5			; x32
	MUL_POW2_HL 4			; x16

	add hl, bc
	ex de, hl

.else
	; de = bc x 192

	MUL_POW2_BC 7			; x128
	MUL_POW2_HL 6			; x64

	add hl, bc
	ex de, hl

.endif
	ld hl, (TR_SCR_TILES_ARR)
	add hl, de

	ret

.endif	;TR_MULTIDIR_SCROLL

.ifdef	TR_BIDIR_STAT_SCR

tr_jpad_update:	

	JPAD_LOAD_STATE

	; if JPAD_STATE == tr_jpad_state then exit

	ld hl, (tr_jpad_state)
	ld de, (JPAD_STATE)

	ld a, l
	cp e
	jp nz, tr_jpad_update_cont

	ld a, h
	cp d
	ret z

tr_jpad_update_cont:

	JPAD1_CHECK_RIGHT
	call nz, tr_move_right

	JPAD1_CHECK_LEFT
	call nz, tr_move_left

	JPAD1_CHECK_DOWN
	call nz, tr_move_down

	JPAD1_CHECK_UP
	call nz, tr_move_up

	; save jpad state

	ld hl, (JPAD_STATE)
	ld (tr_jpad_state), hl

	ret

tr_move_right:

	ld hl, (tr_curr_scr)

	GET_ADJACENT_SCREEN TR_MASK_ADJ_SCR_RIGHT 5 3

	jp _tr_update_screen
	
tr_move_left:

	ld hl, (tr_curr_scr)

	GET_ADJACENT_SCREEN TR_MASK_ADJ_SCR_LEFT 1 1

	jp _tr_update_screen

tr_move_down:

	ld hl, (tr_curr_scr)

	GET_ADJACENT_SCREEN TR_MASK_ADJ_SCR_DOWN 7 4

	jp _tr_update_screen
	
tr_move_up:

	ld hl, (tr_curr_scr)

	GET_ADJACENT_SCREEN TR_MASK_ADJ_SCR_UP 3 2

; IN:	HL - screen data addr

_tr_update_screen:

	ld a, h
	or l
	ret z

	ld (tr_curr_scr), hl

	call _tr_upload_palette_tiles
	call _tr_get_tiles_addr		; hl - tiles addr

	ld de, TR_VRAM_SCR_ATTR_ADDR

	jp _tr_draw_screen

.endif	;TR_BIDIR_STAT_SCR	

.ifdef	TR_SCROLL

tr_jpad_update:	

	; disable scrolling on VBLANK and reset drawing flags

	ld a, (tr_flags)
	and ~( TR_UPD_FLAG_DRAW_MASK | TR_UPD_FLAG_SCROLL )
	ld (tr_flags), a

	JPAD_LOAD_STATE

	JPAD1_CHECK_RIGHT
	call nz, tr_move_right

	JPAD1_CHECK_LEFT
	call nz, tr_move_left

	JPAD1_CHECK_DOWN
	call nz, tr_move_down

	JPAD1_CHECK_UP
	call nz, tr_move_up

	ld a, (tr_flags)
	and TR_UPD_FLAG_DRAW_MASK
	jp z, _jpad_upd_exit

	and TR_UPD_FLAG_DRAW_RIGHT
	call nz, _tr_draw_right_tiles_column

	ld a, (tr_flags)
	and TR_UPD_FLAG_DRAW_LEFT
	call nz, _tr_draw_left_tiles_column

	ld a, (tr_flags)
	and TR_UPD_FLAG_DRAW_DOWN
	call nz, _tr_draw_down_tiles_row

	ld a, (tr_flags)
	and TR_UPD_FLAG_DRAW_UP
	call nz, _tr_draw_up_tiles_row

_jpad_upd_exit:

	; enable scrolling

	ld a, (tr_flags)
	or TR_UPD_FLAG_SCROLL
	ld (tr_flags), a

	ret

.ifdef	TR_MULTIDIR_SCROLL

tr_move_right:

	ld hl, (tr_pos_x)

	ld de, TR_MOVE_STEP
	add hl, de

	ld a, e

	ld e, l
	ld d, h					; 'push' hl

	ld bc, (tr_map_pix_cropped_width)
	and a
	sbc hl, bc

	jr c, _move_right_cont1

	ld a, TR_MOVE_STEP
	sub a, l

	ld de, (tr_map_pix_cropped_width)

_move_right_cont1:

	ex de, hl

	ld (tr_pos_x), hl

	ld e, a

	jp _tr_upd_scroll_right

tr_move_left:

	ld hl, (tr_pos_x)

	ld de, TR_MOVE_STEP
	and a
	sbc hl, de

	jr nc, _move_left_cont1

	add hl, de				; get fixed step value

	ld e, l

	ld hl, 0

_move_left_cont1:

	ld (tr_pos_x), hl

	jp _tr_upd_scroll_left

tr_move_down:

	ld hl, (tr_pos_y)

	ld de, TR_MOVE_STEP
	add hl, de

	ld a, e

	ld e, l
	ld d, h					; 'push' hl

	ld bc, (tr_map_pix_cropped_height)
	and a
	sbc hl, bc

	jr c, _move_down_cont1

	ld a, TR_MOVE_STEP
	sub a, l

	ld de, (tr_map_pix_cropped_height)

_move_down_cont1:

	ex de, hl

	ld (tr_pos_y), hl

	and a
	ret z					; zero step - exit

	ld e, a

	jp _tr_upd_scroll_down

tr_move_up:

	ld hl, (tr_pos_y)

	ld de, TR_MOVE_STEP
	and a
	sbc hl, de

	jr nc, _move_up_cont1

	add hl, de				; get fixed step value

	ld e, l

	ld hl, 0

_move_up_cont1:

	ld (tr_pos_y), hl

	jp _tr_upd_scroll_up

.else
.ifdef	TR_BIDIR_SCROLL

tr_move_right:

	ld hl, (tr_vert_dir_pos)
	ld a, l
	or h
	ret nz

	ld hl, (tr_horiz_dir_pos)

	ld a, l
	or h
	jp nz, _move_right_cont0

	ld hl, (tr_curr_scr)

	GET_ADJACENT_SCREEN TR_MASK_ADJ_SCR_RIGHT 5 3

	ld a, h
	or l
	ret z

	ld hl, (tr_horiz_dir_pos)

_move_right_cont0:

	; if HL == 256

	ld a, l
	or a
	jp nz, _move_right_cont1

	ld a, h
	cp $01
	jp nz, _move_right_cont1

	; HL == 256

	ld hl, 0
	ld (tr_horiz_dir_pos), hl

	ld hl, (tr_curr_scr)

	GET_ADJACENT_SCREEN TR_MASK_ADJ_SCR_RIGHT 5 3

	ld a, h
	or l
	ret z

	ld (tr_curr_scr), hl

	jp tr_move_right

_move_right_cont1:

	ld hl, (tr_horiz_dir_pos)
	ld de, TR_MOVE_STEP
	add hl, de
	ld (tr_horiz_dir_pos), hl

	jp _tr_upd_scroll_right

tr_move_left:

	ld hl, (tr_vert_dir_pos)
	ld a, l
	or h
	ret nz

	ld hl, (tr_horiz_dir_pos)

	ld a, l
	or h
	jp nz, _move_left_cont0

	ld hl, (tr_curr_scr)

	GET_ADJACENT_SCREEN TR_MASK_ADJ_SCR_LEFT 1 1

	ld a, h
	or l
	ret z

	ld hl, (tr_horiz_dir_pos)

_move_left_cont0:

	; if HL == -256

	ld a, l
	or a
	jp nz, _move_left_cont1

	ld a, h
	cp $ff
	jp nz, _move_left_cont1

	; HL == -256

	ld hl, 0
	ld (tr_horiz_dir_pos), hl

	ld hl, (tr_curr_scr)

	GET_ADJACENT_SCREEN TR_MASK_ADJ_SCR_LEFT 1 1

	ld a, h
	or l
	ret z

	ld (tr_curr_scr), hl

	jp tr_move_left

_move_left_cont1:

	ld hl, (tr_horiz_dir_pos)
	ld de, TR_MOVE_STEP
	and a
	sbc hl, de
	ld (tr_horiz_dir_pos), hl

	jp _tr_upd_scroll_left

tr_move_down:

	ld hl, (tr_horiz_dir_pos)
	ld a, l
	or h
	ret nz

	ld hl, (tr_vert_dir_pos)

	ld a, l
	or h
	jp nz, _move_down_cont0

	ld hl, (tr_curr_scr)

	GET_ADJACENT_SCREEN TR_MASK_ADJ_SCR_DOWN 7 4

	ld a, h
	or l
	ret z

	ld hl, (tr_vert_dir_pos)

_move_down_cont0:

	; if HL == 192

	ld a, l
	cp $c0
	jp nz, _move_down_cont1

	ld a, h
	or a
	jp nz, _move_down_cont1

	; HL == 192

	ld hl, 0
	ld (tr_vert_dir_pos), hl

	ld hl, (tr_curr_scr)

	GET_ADJACENT_SCREEN TR_MASK_ADJ_SCR_DOWN 7 4

	ld a, h
	or l
	ret z

	ld (tr_curr_scr), hl

	jp tr_move_down

_move_down_cont1:

	ld hl, (tr_vert_dir_pos)
	ld de, TR_MOVE_STEP
	add hl, de
	ld (tr_vert_dir_pos), hl

	jp _tr_upd_scroll_down

tr_move_up:

	ld hl, (tr_horiz_dir_pos)
	ld a, l
	or h
	ret nz

	ld hl, (tr_vert_dir_pos)

	ld a, l
	or h
	jp nz, _move_up_cont0

	ld hl, (tr_curr_scr)

	GET_ADJACENT_SCREEN TR_MASK_ADJ_SCR_UP 3 2

	ld a, h
	or l
	ret z

	ld hl, (tr_vert_dir_pos)

_move_up_cont0:

	; if HL == -192

	ld a, l
	cp $40
	jp nz, _move_up_cont1

	ld a, h
	cp $ff
	jp nz, _move_up_cont1

	; HL == -192

	ld hl, 0
	ld (tr_vert_dir_pos), hl

	ld hl, (tr_curr_scr)

	GET_ADJACENT_SCREEN TR_MASK_ADJ_SCR_UP 3 2

	ld a, h
	or l
	ret z

	ld (tr_curr_scr), hl

	jp tr_move_up

_move_up_cont1:

	ld hl, (tr_vert_dir_pos)
	ld de, TR_MOVE_STEP
	and a
	sbc hl, de
	ld (tr_vert_dir_pos), hl

	jp _tr_upd_scroll_up

.endif	;TR_BIDIR_SCROLL
.endif	;TR_MULTIDIR_SCROLL

; *** update scroll values routines ***
; IN:	E - move step

_tr_upd_scroll_right:

	ld a, (tr_scroll_x)
	ld c, a
	ld b, 0

	and %00000111
	jp nz, _move_right_cont2

	inc b					; need draw tiles column

_move_right_cont2:

	ld a, c
	sub a, e
	ld (tr_scroll_x), a

	ret z

	ld a, b
	and a

	ret z

	TR_SET_DRAW_FLAG_RIGHT
	ret

; IN:	E - move step

_tr_upd_scroll_left:

	ld a, (tr_scroll_x)
	ld c, a
	ld b, 0

	and %00000111
	jp nz, _move_left_cont2

	inc b					; need draw tiles column

_move_left_cont2:

	ld a, c
	add a, e
	ld (tr_scroll_x), a

	ret z

	ld a, b
	and a

	ret z

	TR_SET_DRAW_FLAG_LEFT
	ret

; IN:	E - move step

_tr_upd_scroll_down:

	ld a, (tr_scroll_y)
	ld c, a
	ld b, 0

	and %00000111
	jp nz, _move_down_cont2

	inc b					; need draw tiles row

_move_down_cont2:

	ld a, c
	add a, e

	jr nc, _move_down_cont3

	add $20

_move_down_cont3:

	ld (tr_scroll_y), a

	ret z

	ld a, b
	and a

	ret z

	TR_SET_DRAW_FLAG_DOWN
	ret

; IN:	E - move step

_tr_upd_scroll_up:

	ld a, (tr_scroll_y)
	ld c, a
	ld b, 0

	and %00000111
	jp nz, _move_up_cont2

	inc b					; need draw tiles column

_move_up_cont2:

	ld a, c
	sub a, e

	jr nc, _move_up_cont3

	add $e0

_move_up_cont3:

	ld (tr_scroll_y), a

	ret z

	ld a, b
	and a

	ret z

	TR_SET_DRAW_FLAG_UP
	ret
	
_tr_draw_right_tiles_column:

	; calc tiles data addr

.ifdef	TR_BIDIR_SCROLL

	ld a, (tr_horiz_dir_pos)
	ld c, a
	ld b, 0

	push bc

.ifdef	TR_DATA_TILES4X4
	DIV_POW2_C 4			; de /= 16
.else
	DIV_POW2_C 3			; de /= 8
.endif
	res 0, c

	; c *= ScrTilesHeight [6-4x4;12-2x2]

	ld a, c

.ifdef	TR_DATA_TILES4X4
	; x3
	MUL_POW2_C 1
.else
	; x6
	MUL_POW2_C 2
	MUL_POW2_A 1
.endif
	add a, c

	ld e, a
	ld d, 0

	push de

	ld hl, (tr_horiz_dir_pos)

	; if HL < 0

	ld a, h
	bit 7, h

	ld hl, (tr_curr_scr)

	jp nz, _tr_draw_right_tiles_column_cont1

	; HL >= 0

	GET_ADJACENT_SCREEN TR_MASK_ADJ_SCR_RIGHT 5 3

_tr_draw_right_tiles_column_cont1:

	GET_SCREEN_TILES_ADDR		; hl - curr screen tiles addr

	pop de

	pop bc

	add hl, de			; hl - tiles data addr

.else	;TR_BIDIR_SCROLL
	ld de, (tr_pos_x)
	ld c, e

.ifdef	TR_DATA_TILES4X4
	DIV_POW2_DE 4			; de /= 16
.else
	DIV_POW2_DE 3			; de /= 8
.endif
	res 0, e

	ld hl, (TR_MAP_LUT)
	add hl, de

	; get the right screen tiles ( +8 4x4 tiles )

	ld de, ScrTilesWidth * 2
	add hl, de

	ld e, (hl)
	inc hl
	ld d, (hl)

	ld hl, (TR_TILES_MAP)
	add hl, de			; hl - tiles_addr

	ld de, (tr_pos_y)
	ld b, e

.ifdef	TR_DATA_TILES4X4
	DIV_POW2_DE 5
.else
	DIV_POW2_DE 4
.endif
	add hl, de			; hl - tiles data addr
.endif	;TR_BIDIR_SCROLL

	push hl

	; calc VRAM addr by scroll values

	call _tr_get_VRAM_addr

	; hl -= 2

	ld a, l
	and %11000000
	ld e, a
	dec l
	dec l
	ld a, l
	and %00111111
	or e
	ld l, a

	jp _tr_fill_column_data

_tr_draw_left_tiles_column:

	; calc tiles data addr

.ifdef	TR_BIDIR_SCROLL

	ld a, (tr_horiz_dir_pos)
	add a, 8			; left blank column offset
	ld c, a
	ld b, 0

	push bc

.ifdef	TR_DATA_TILES4X4
	DIV_POW2_C 4			; de /= 16
.else
	DIV_POW2_C 3			; de /= 8
.endif
	res 0, c

	; c *= ScrTilesHeight [6-4x4;12-2x2]

	ld a, c

.ifdef	TR_DATA_TILES4X4
	; x3
	MUL_POW2_C 1
.else
	; x6
	MUL_POW2_C 2
	MUL_POW2_A 1
.endif
	add a, c

	ld e, a
	ld d, 0

	push de

	ld hl, (tr_horiz_dir_pos)

	ld de, 8
	add hl, de			; left blank column offset

	; if HL > 0

	ld a, h
	bit 7, h

	ld hl, (tr_curr_scr)

	jp z, _tr_draw_left_tiles_column_cont1

	; HL < 0

	GET_ADJACENT_SCREEN TR_MASK_ADJ_SCR_LEFT 1 1

_tr_draw_left_tiles_column_cont1:

	GET_SCREEN_TILES_ADDR		; hl - curr screen tiles addr

	pop de

	pop bc

	add hl, de			; hl - tiles data addr

.else	;TR_BIDIR_SCROLL
	ld de, (tr_pos_x)

	; shift camera pos

	ld hl, 8
	add hl, de
	ex de, hl

	ld c, e

.ifdef	TR_DATA_TILES4X4
	DIV_POW2_DE 4			; de /= 16, ( ( de >> 5 ) << 1 )
.else
	DIV_POW2_DE 3			; de /= 8
.endif
	res 0, e

	ld hl, (TR_MAP_LUT)
	add hl, de

	ld e, (hl)
	inc hl
	ld d, (hl)

	ld hl, (TR_TILES_MAP)
	add hl, de			; hl - tiles_addr

	ld de, (tr_pos_y)
	ld b, e

.ifdef	TR_DATA_TILES4X4
	DIV_POW2_DE 5
.else
	DIV_POW2_DE 4
.endif
	add hl, de			; hl - tiles data addr
.endif	;TR_BIDIR_SCROLL

	push hl

	; calc VRAM addr by scroll values

	call _tr_get_VRAM_addr

_tr_fill_column_data:

	; push header to the data buffer

	ld a, TR_COLUMN_DATA_HEADER
	ex de, hl

	call buff_push_hdr

	pop hl

	and ~TR_BUFF_STEP_64		; reset TR_BUFF_STEP_64

	call _tr_push_tiles_column
	jp buff_end

_tr_draw_down_tiles_row:

	; calc tiles data addr

.ifdef	TR_BIDIR_SCROLL

	ld de, (tr_vert_dir_pos)
	ld a, d
	bit 7, d
	ld a, e
	jp  z, _tr_draw_down_raw_cont0	; +

	neg
	ld c, a
	ld a, $c0
	sub c

_tr_draw_down_raw_cont0:

	ld c, 8				; left blank column offset
	ld b, a

	push bc

.ifdef	TR_DATA_TILES4X4
	DIV_POW2_B 5			; de /= 32
.else
	DIV_POW2_B 4			; de /= 16
.endif

	ld l, b
	ld h, 0

	push hl

	; if DE < 0

	ld a, d
	bit 7, d

	ld hl, (tr_curr_scr)

	jp nz, _tr_draw_down_tiles_column_cont1

	; DE >= 0

	GET_ADJACENT_SCREEN TR_MASK_ADJ_SCR_DOWN 7 4

_tr_draw_down_tiles_column_cont1:

	GET_SCREEN_TILES_ADDR		; hl - curr screen tiles addr

	pop de

	pop bc

	add hl, de			; hl - tiles data addr

.else	;TR_BIDIR_SCROLL
	ld de, (tr_pos_x)

	; shift camera pos

	ld hl, 8
	add hl, de
	ex de, hl

	ld c, e

.ifdef	TR_DATA_TILES4X4
	DIV_POW2_DE 4			; de /= 16
.else
	DIV_POW2_DE 3			; de /= 8
.endif

	res 0, e

	ld hl, (TR_MAP_LUT)
	add hl, de

	ld e, (hl)
	inc hl
	ld d, (hl)

	ld hl, (TR_TILES_MAP)
	add hl, de			; hl - tiles_addr

	; get the bottom screen tiles ( +6 4x4 tiles )

	ld de, ScrTilesHeight
	add hl, de

	ld de, (tr_pos_y)
	ld b, e

.ifdef	TR_DATA_TILES4X4
	DIV_POW2_DE 5
.else
	DIV_POW2_DE 4
.endif
	add hl, de			; hl - tiles data addr
.endif	;TR_BIDIR_SCROLL

	push hl

	; calc VRAM addr by scroll values

	call _tr_get_VRAM_addr

	ld de, $600
	add hl, de
	LOOP_VRAM_ATTR_ADDR_HL 0

	jp _tr_fill_row_data

_tr_draw_up_tiles_row:

	; calc tiles data addr

.ifdef	TR_BIDIR_SCROLL

	ld de, (tr_vert_dir_pos)
	ld a, d
	bit 7, d
	ld a, e
	jp  z, _tr_draw_up_raw_cont0	; +

	neg
	ld c, a
	ld a, $c0
	sub c

_tr_draw_up_raw_cont0:

	ld c, 8				; left blank column offset
	ld b, a

	push bc

.ifdef	TR_DATA_TILES4X4
	DIV_POW2_B 5			; de /= 32
.else
	DIV_POW2_B 4			; de /= 16
.endif

	ld l, b
	ld h, 0

	push hl

	; if DE > 0

	ld a, d
	bit 7, d

	ld hl, (tr_curr_scr)

	jp z, _tr_draw_up_tiles_column_cont1

	; DE < 0

	GET_ADJACENT_SCREEN TR_MASK_ADJ_SCR_UP 3 2

_tr_draw_up_tiles_column_cont1:

	GET_SCREEN_TILES_ADDR		; hl - curr screen tiles addr

	pop de

	pop bc

	add hl, de			; hl - tiles data addr

.else	;TR_BIDIR_SCROLL
	ld de, (tr_pos_x)

	; shift camera pos

	ld hl, 8
	add hl, de
	ex de, hl

	ld c, e

.ifdef	TR_DATA_TILES4X4
	DIV_POW2_DE 4			; de /= 16
.else
	DIV_POW2_DE 3			; de /= 8
.endif

	res 0, e

	ld hl, (TR_MAP_LUT)
	add hl, de

	ld e, (hl)
	inc hl
	ld d, (hl)

	ld hl, (TR_TILES_MAP)
	add hl, de			; hl - tiles_addr

	ld de, (tr_pos_y)
	ld b, e

.ifdef	TR_DATA_TILES4X4
	DIV_POW2_DE 5
.else
	DIV_POW2_DE 4
.endif
	add hl, de			; hl - tiles data addr
.endif	;TR_BIDIR_SCROLL

	push hl

	; calc VRAM addr by scroll values

	call _tr_get_VRAM_addr

_tr_fill_row_data:

.if	TR_FAST_ROW_FILLING == 0

	; push header to the data buffer

	ld a, TR_ROW_DATA_HEADER
	ex de, hl

	call buff_push_hdr

	pop hl

	call _tr_push_tiles_row
	jp buff_end
.else

	ld a, l
	and %00111111
	sub $40
	neg

	DIV_POW2_A 1

	ex de, hl

	call buff_push_hdr

	pop hl
	push de
	push af

	call _tr_push_tiles_row

	pop bc
	pop de

	push hl

	ld a, e
	and %11000000
	ld e, a

	ld a, $20
	sub b

	call buff_push_hdr

	ld hl, (tr_pos_y)
	ld b, l
	ld c, 0

	pop hl

	call _tr_push_tiles_row
	jp buff_end

.endif	

; *** calc VRAM addr by scroll values ***
; IN:	tr_scroll_x
;	tr_scroll_y
; OUT:	HL - VRAM addr

_tr_get_VRAM_addr:

	ld a, (tr_scroll_x)

	neg
	add a, 8

	DIV_POW2_A 2	; a /= 4

	ld e, a
	ld d, 0

	ld a, (tr_scroll_y)

	and %11111000

	ld l, a
	ld h, 0

	MUL_POW2_HL 3

	add hl, de

	ld de, TR_VRAM_SCR_ATTR_ADDR
	add hl, de

	res 0, l			; we need even address

	LOOP_VRAM_ATTR_ADDR_HL 1

	ret

; *** push tiles column data into the data buffer ***
; IN:	HL - tiles data addr
;	BC - low bytes of camera pos X/Y
;	A - tiles count
; OUT:	filled data buffer

_tr_push_tiles_column:

	push af

	; calc first CHR index

	DIV_POW2_B 3	; b /= 8

	ld a, b
.ifdef	TR_DATA_TILES4X4	
	and $03
.else
	and $01
.endif	
	ld b, a				; b - how many CHRs will be skipped

	; calc tile's column index

	DIV_POW2_C 3	; c /= 8

	ld a, c
.ifdef	TR_DATA_TILES4X4	
	and $03
.else
	and $01
.endif	

	MUL_POW2_A 1	; x2

	; calc appropriate routine address

	exx

	ld e, a
	ld d, 0

	exx

	pop af
	ld c, a				; c - CHRs count

	exx

	ld hl, _tr_tiles_col_routine_tbl
	add hl, de

	ld e, (hl)
	inc hl
	ld d, (hl)

	ex de, hl			; hl' - routine address

_tr_push_column_loop:

	ld de, _tr_push_column_cont

	push de
	jp hl

_tr_push_column_cont:

	inc hl

	ld a, c
	or a
	ret z

	exx

	jp _tr_push_column_loop

.ifdef	TR_DATA_TILES4X4
_tr_tile_data_col0:

	TR_FILL_TILE_COLROW 0 2 0 2

_tr_tile_data_col1:

	TR_FILL_TILE_COLROW 0 2 2 2

_tr_tile_data_col2:

	TR_FILL_TILE_COLROW 1 2 0 2

_tr_tile_data_col3:

	TR_FILL_TILE_COLROW 1 2 2 2
.else
_tr_tile_data_col0:

	TR_FILL_TILE_COLROW 0 0 0 2

_tr_tile_data_col1:

	TR_FILL_TILE_COLROW 0 0 2 2
.endif

; *** push tiles row data into the data buffer ***
; IN:	HL - tiles data addr
;	BC - low bytes of camera pos X/Y
;	A - tiles count
; OUT:	filled data buffer

_tr_push_tiles_row:

	push af

	; calc first CHR index

	DIV_POW2_C 3	; c /= 8

	ld a, c
.ifdef	TR_DATA_TILES4X4	
	and $03
.else
	and $01
.endif	
	ld c, a				; c - how many CHRs will be skipped

	; calc tile's row index

	DIV_POW2_B 3	; b /= 8

	ld a, b
.ifdef	TR_DATA_TILES4X4	
	and $03
.else
	and $01
.endif	

	MUL_POW2_A 1	; x2

	ld b, c

	; calc appropriate routine address

	exx

	ld e, a
	ld d, 0

	exx

	pop af
	ld c, a				; c - CHRs count

	exx

	ld hl, _tr_tiles_row_routine_tbl
	add hl, de

	ld e, (hl)
	inc hl
	ld d, (hl)

	ex de, hl			; hl' - routine address

_tr_push_row_loop:

	ld de, _tr_push_row_cont

	push de
	jp hl

_tr_push_row_cont:

	ld de, (TR_MAP_TILES_HEIGHT)
	add hl, de

	ld a, c
	or a
	ret z

	exx

	jp _tr_push_row_loop

.ifdef	TR_DATA_TILES4X4
_tr_tile_data_row0:

	TR_FILL_TILE_COLROW 0 1 0 0

_tr_tile_data_row1:

	TR_FILL_TILE_COLROW 0 1 4 0

_tr_tile_data_row2:

	TR_FILL_TILE_COLROW 2 1 0 0

_tr_tile_data_row3:

	TR_FILL_TILE_COLROW 2 1 4 0
.else
_tr_tile_data_row0:

	TR_FILL_TILE_COLROW 0 0 0 0

_tr_tile_data_row1:

	TR_FILL_TILE_COLROW 0 0 4 0
.endif
.endif	;TR_SCROLL

update_scroll:

	ld a, (tr_flags)
	and TR_UPD_FLAG_SCROLL
	ret z

	ld a, (tr_scroll_x)
	ld b, a

	ld a, (tr_scroll_y)
	ld c, a

	jp VDP_update_scroll