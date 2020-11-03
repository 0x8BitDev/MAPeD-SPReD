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


.if MAP_DATA_MAGIC&MAP_FLAG_DIR_ROWS == MAP_FLAG_DIR_ROWS
	.printt "*** ERROR: The sample doesn't support row ordered data! ***\n"
	.fail
.endif

.if	( MAP_DATA_MAGIC & MAP_FLAG_MODE_MULTIDIR_SCROLL ) == MAP_FLAG_MODE_MULTIDIR_SCROLL
.define	TR_MULTIDIR_SCROLL
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
.else
.define	TR_COLUMN_DATA_HEADER	TR_BUFF_STEP_64 | 24
.endif

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

; *** useful macroses ***

.macro TR_PUT_12ATTR_TO_VRAM	; a - attribute index
	exx
	ld hl, ( TR_BLOCK_ATTRS )

	ld c, a
	ld b, 0

	MUL_POW2_BC 3

	add hl, bc

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
	ld a, ( tr_flags )
	or TR_UPD_FLAG_DRAW_UP
	ld ( tr_flags ), a
.endm

.macro TR_SET_DRAW_FLAG_DOWN
	ld a, ( tr_flags )
	or TR_UPD_FLAG_DRAW_DOWN
	ld ( tr_flags ), a
.endm

.macro TR_SET_DRAW_FLAG_LEFT
	ld a, ( tr_flags )
	or TR_UPD_FLAG_DRAW_LEFT
	ld ( tr_flags ), a
.endm

.macro TR_SET_DRAW_FLAG_RIGHT
	ld a, ( tr_flags )
	or TR_UPD_FLAG_DRAW_RIGHT
	ld ( tr_flags ), a
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
	ld ( tr_scroll_x ), a
	ld ( tr_scroll_y ), a
	ld ( tr_flags ), a

	ld hl, Lev0_WPixelsCnt - ScrPixelsWidth
	ld ( tr_map_pix_cropped_width ), hl

	ld hl, Lev0_HPixelsCnt - ScrPixelsHeight
	ld ( tr_map_pix_cropped_height ), hl

	call buff_reset

	ld a, ( TR_START_SCR )

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
	ld hl, ( TR_TILES4x4 )
	add hl, bc

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

	ld de, ( TR_MAP_TILES_HEIGHT )
	add hl, de
	ld de, -ScrTilesHeight
	add hl, de

	pop de

	push hl

	; move to the next attributes column

	ex de, hl
.ifdef	TR_DATA_TILES4X4	
	ld de, -( $600 - $08 )
.else
	ld de, -( $600 - $04 )
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

; *** get tiles addr by input screen number ***
;
; IN: A - screen number
;
; OUT: HL - tiles addr
;

_tr_get_tiles_addr:

	; x_offset = TR_MAP_LUT[ ScrTilesWidth * ( TR_START_SCR % TR_MAP_SCR_WIDTH ) ]

	ld c, a

	ld a, ( TR_MAP_SCR_WIDTH )
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

	ld ( tr_pos_x ), hl

	MUL_POW2_BC 1

	ld hl, ( TR_MAP_LUT )

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

	ld ( tr_pos_y ), hl

	ex de, hl			; hl = y_offset

	pop de				; x_offset

	; tiles_addr = *TR_TILES_MAP + x_offset + y_offset

	add hl, de			; hl = x_offset + y_offset

	ld de, ( TR_TILES_MAP )
	add hl, de			; hl - tiles_addr

	ret

tr_jpad_update:	

	; disable scrolling on VBLANK and reset drawing flags

	ld a, ( tr_flags )
	and ~( TR_UPD_FLAG_DRAW_MASK | TR_UPD_FLAG_SCROLL )
	ld ( tr_flags ), a

	JPAD_LOAD_STATE

	JPAD1_CHECK_RIGHT
	call nz, tr_move_right

	JPAD1_CHECK_LEFT
	call nz, tr_move_left

	JPAD1_CHECK_DOWN
	call nz, tr_move_down

	JPAD1_CHECK_UP
	call nz, tr_move_up

	ld a, ( tr_flags )
	and TR_UPD_FLAG_DRAW_MASK
	jp z, _jpad_upd_exit

	and TR_UPD_FLAG_DRAW_RIGHT
	call nz, _tr_draw_right_tiles_column

	ld a, ( tr_flags )
	and TR_UPD_FLAG_DRAW_LEFT
	call nz, _tr_draw_left_tiles_column

	ld a, ( tr_flags )
	and TR_UPD_FLAG_DRAW_DOWN
	call nz, _tr_draw_down_tiles_row

	ld a, ( tr_flags )
	and TR_UPD_FLAG_DRAW_UP
	call nz, _tr_draw_up_tiles_row

_jpad_upd_exit:

	; enable scrolling

	ld a, ( tr_flags )
	or TR_UPD_FLAG_SCROLL
	ld ( tr_flags ), a

	ret

tr_move_right:

	ld hl, ( tr_pos_x )

	ld de, TR_MOVE_STEP
	add hl, de

	and a

	ld a, e

	ld e, l
	ld d, h					; 'push' hl

	ld bc, ( tr_map_pix_cropped_width )
	sbc hl, bc

	jr c, _move_right_cont1

	ld a, TR_MOVE_STEP
	sub a, l

	ld de, ( tr_map_pix_cropped_width )

_move_right_cont1:

	ld b, 0

	ex de, hl

	ld ( tr_pos_x ), hl

	ld e, a

	ld a, ( tr_scroll_x )
	ld c, a

	and a					; reset carry flag

	and %00000111
	jp nz, _move_right_cont2

	inc b					; need draw tiles column

_move_right_cont2:

	ld a, c
	sub a, e
	ld ( tr_scroll_x ), a

	ret z

	ld a, b
	and a

	ret z

	TR_SET_DRAW_FLAG_RIGHT
	ret

tr_move_left:

	ld hl, ( tr_pos_x )

	ld de, TR_MOVE_STEP
	and a
	sbc hl, de

	jr nc, _move_left_cont1

	add hl, de				; get fixed step value

	ld e, l

	ld hl, 0

_move_left_cont1:

	ld b, 0

	ld ( tr_pos_x ), hl

	ld a, ( tr_scroll_x )
	ld c, a

	and a					; reset carry flag

	and %00000111
	jp nz, _move_left_cont2

	inc b					; need draw tiles column

_move_left_cont2:

	ld a, c
	add a, e
	ld ( tr_scroll_x ), a

	ret z

	ld a, b
	and a

	ret z

	TR_SET_DRAW_FLAG_LEFT
	ret

tr_move_down:

	ld hl, ( tr_pos_y )

	ld de, TR_MOVE_STEP
	add hl, de

	and a

	ld a, e

	ld e, l
	ld d, h					; 'push' hl

	ld bc, ( tr_map_pix_cropped_height )
	sbc hl, bc

	jr c, _move_down_cont1

	ld a, TR_MOVE_STEP
	sub a, l

	ld de, ( tr_map_pix_cropped_height )

_move_down_cont1:

	ld b, 0

	ex de, hl

	ld ( tr_pos_y ), hl

	and a
	ret z					; zero step - exit

	ld e, a

	ld a, ( tr_scroll_y )
	ld c, a

	and a					; reset carry flag

	and %00000111
	jp nz, _move_down_cont2

	inc b					; need draw tiles row

_move_down_cont2:

	ld a, c
	add a, e

	jr nc, _move_down_cont3

	add $20

_move_down_cont3:

	ld ( tr_scroll_y ), a

	ld a, b
	and a

	ret z

	TR_SET_DRAW_FLAG_DOWN
	ret

tr_move_up:

	ld hl, ( tr_pos_y )

	ld de, TR_MOVE_STEP
	and a
	sbc hl, de

	jr nc, _move_up_cont1

	add hl, de				; get fixed step value

	ld e, l

	ld hl, 0

_move_up_cont1:

	ld b, 0

	ld ( tr_pos_y ), hl

	ld a, ( tr_scroll_y )
	ld c, a

	and a					; reset carry flag

	and %00000111
	jp nz, _move_up_cont2

	inc b					; need draw tiles column

_move_up_cont2:

	ld a, c
	sub a, e

	jr nc, _move_up_cont3

	add $e0

_move_up_cont3:

	ld ( tr_scroll_y ), a

	ret z

	ld a, b
	and a

	ret z

	TR_SET_DRAW_FLAG_UP
	ret

_tr_draw_right_tiles_column:

	; calc tiles data addr

	ld de, ( tr_pos_x )
	ld c, e

.ifdef	TR_DATA_TILES4X4
	DIV_POW2_DE 4			; de /= 16
.else
	DIV_POW2_DE 3			; de /= 8
.endif
	res 0, e

	ld hl, ( TR_MAP_LUT )
	add hl, de

	; get the right screen tiles ( +8 4x4 tiles )

	ld de, ScrTilesWidth * 2
	add hl, de

	ld e, (hl)
	inc hl
	ld d, (hl)

	ld hl, ( TR_TILES_MAP )
	add hl, de			; hl - tiles_addr

	ld de, ( tr_pos_y )
	ld b, e

.ifdef	TR_DATA_TILES4X4
	DIV_POW2_DE 5
.else
	DIV_POW2_DE 4
.endif
	add hl, de			; hl - tiles data addr

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

	ld de, ( tr_pos_x )

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

	ld hl, ( TR_MAP_LUT )
	add hl, de

	ld e, (hl)
	inc hl
	ld d, (hl)

	ld hl, ( TR_TILES_MAP )
	add hl, de			; hl - tiles_addr

	ld de, ( tr_pos_y )
	ld b, e

.ifdef	TR_DATA_TILES4X4
	DIV_POW2_DE 5
.else
	DIV_POW2_DE 4
.endif
	add hl, de			; hl - tiles data addr

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

	ld de, ( tr_pos_x )

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

	ld hl, ( TR_MAP_LUT )
	add hl, de

	ld e, (hl)
	inc hl
	ld d, (hl)

	ld hl, ( TR_TILES_MAP )
	add hl, de			; hl - tiles_addr

	; get the bottom screen tiles ( +6 4x4 tiles )

	ld de, ScrTilesHeight
	add hl, de

	ld de, ( tr_pos_y )
	ld b, e

.ifdef	TR_DATA_TILES4X4
	DIV_POW2_DE 5
.else
	DIV_POW2_DE 4
.endif

	add hl, de			; hl - tiles data addr

	push hl

	; calc VRAM addr by scroll values

	call _tr_get_VRAM_addr

	ld de, $600
	add hl, de
	LOOP_VRAM_ATTR_ADDR_HL 0

	jp _tr_fill_row_data

_tr_draw_up_tiles_row:

	; calc tiles data addr

	ld de, ( tr_pos_x )

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

	ld hl, ( TR_MAP_LUT )
	add hl, de

	ld e, (hl)
	inc hl
	ld d, (hl)

	ld hl, ( TR_TILES_MAP )
	add hl, de			; hl - tiles_addr

	ld de, ( tr_pos_y )
	ld b, e

.ifdef	TR_DATA_TILES4X4
	DIV_POW2_DE 5
.else
	DIV_POW2_DE 4
.endif

	add hl, de			; hl - tiles data addr

	push hl

	; calc VRAM addr by scroll values

	call _tr_get_VRAM_addr

_tr_fill_row_data:

.if	TR_FAST_ROW_FILLING == 0

	; push header to the data buffer

	ld a, 32
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

	ld hl, ( tr_pos_y )
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

	ld a, ( tr_scroll_x )

	neg
	add a, 8

	DIV_POW2_A 2	; a /= 4

	ld e, a
	ld d, 0

	ld a, ( tr_scroll_y )

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

	ld de, ( TR_MAP_TILES_HEIGHT )
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

update_scroll:

	ld a, ( tr_flags )
	and TR_UPD_FLAG_SCROLL
	ret z

	ld a, ( tr_scroll_x )
	ld b, a

	ld a, ( tr_scroll_y )
	ld c, a

	jp VDP_update_scroll