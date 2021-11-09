;###################################################################
;
; Copyright 2018-2021 0x8BitDev ( MIT license )
;
;###################################################################
;
; DESC: Bidirectional static screens switching test.
;
; Public procs:
;
; tilemap_render.init			- render init
; tilemap_render.draw_screen		- draw tiles to screen
; tilemap_render.check_up_screen	- check adjacent screen
; tilemap_render.check_down_screen	- check adjacent screen
; tilemap_render.check_left_screen	- check adjacent screen
; tilemap_render.check_right_screen	- check adjacent screen
;
; Supported options:
;
; MAP_FLAG_TILES2X2
; MAP_FLAG_TILES4X4
; MAP_FLAG_DIR_COLUMNS
; MAP_FLAG_DIR_ROWS
; MAP_FLAG_MODE_BIDIR_SCROLL
; MAP_FLAG_LAYOUT_ADJACENT_SCREENS
; MAP_FLAG_LAYOUT_ADJACENT_SCR_INDS
; MAP_FLAG_COLOR_TILES
;


		MODULE	tilemap_render

		include "common.asm"

		assert ( MAP_DATA_MAGIC&MAP_FLAG_LAYOUT_MATRIX ) == 0, The sample doesn't support matrix layout data!
		assert ( MAP_DATA_MAGIC&MAP_FLAG_MODE_MULTIDIR_SCROLL ) == 0, The sample doesn't support data for multidirectional scroller!
		assert ( MAP_DATA_MAGIC&MAP_FLAG_RLE ) == 0, The sample doesn't support compressed data!

TR_DATA_TILES4X4	equ MAP_DATA_MAGIC&MAP_FLAG_TILES4X4
TR_DATA_MARKS		equ MAP_DATA_MAGIC&MAP_FLAG_MARKS
TR_COLORED_MAP		equ MAP_DATA_MAGIC&MAP_FLAG_COLOR_TILES
TR_ADJ_SCREENS		equ MAP_DATA_MAGIC&MAP_FLAG_LAYOUT_ADJACENT_SCREENS
TR_DIR_ROWS		equ MAP_DATA_MAGIC&MAP_FLAG_DIR_ROWS

SCR_CHR_WIDTH		equ SCR_BLOCKS2x2_WIDTH << 1
SCR_CHR_HEIGHT		equ SCR_BLOCKS2x2_HEIGHT << 1

SCR_CENTER_OFFS_X	equ ( 32 - SCR_CHR_WIDTH ) >> 1

		IF TR_DIR_ROWS
SCR_UNI_WIDTH	equ SCR_BLOCKS2x2_HEIGHT
SCR_UNI_HEIGHT	equ SCR_BLOCKS2x2_WIDTH
		ELSE
SCR_UNI_WIDTH	equ SCR_BLOCKS2x2_WIDTH
SCR_UNI_HEIGHT	equ SCR_BLOCKS2x2_HEIGHT
		ENDIF //TR_DIR_ROWS

		IF TR_DATA_TILES4X4
SCR_UNI_WIDTH	equ SCR_UNI_WIDTH >> 1
SCR_UNI_HEIGHT	equ SCR_UNI_HEIGHT >> 1
		ENDIF //TR_DATA_TILES4X4

; this data must be filled in for each map in the main code
;
map_screens	dw 0			; tilemap_TilesScr screen tiles aray for each screen
map_gfx_arr	dw 0			; tilemap_Gfx exported graphics array

map_props_arr	dw 0			; tilemap_BlocksPropsOffs

map_curr_scr	dw 0

map_scr_size	db 0

		IF !TR_ADJ_SCREENS
map_scr_arr	dw 0			; Lev0_ScrArr
		ENDIF //!TR_ADJ_SCREENS

		IF TR_DATA_TILES4X4
map_tiles4x4	dw 0			; tilemap_Tiles	tiles 4x4 data
map_tiles_offs	dw 0			; Tiles(4x4)-tilemap_TilesOffs
map_tiles4x4_data
		dw 0
		ENDIF //TR_DATA_TILES4X4

		IF TR_COLORED_MAP
map_clrs	dw 0			; tilemap_Clrs blocks 2x2 colors array
scr_clrs_arr	block ( SCR_BLOCKS2x2_WIDTH * SCR_BLOCKS2x2_HEIGHT ) << 2, 0
		ENDIF //TR_COLORED_MAP

			align 256
tiles_addr_tbl		block 512,0	; tile graphics address table
tiles_clr_addr_tbl	block 512,0	; tile colors(attrbutes) address table

_adj_scr_left	= %00010000
_adj_scr_up	= %00100000
_adj_scr_right	= %01000000
_adj_scr_down	= %10000000

	macro	scr_put_block2x2
		ld c, l		;4 save hl
		ld b, h		;4

		dup 4			;<---
		pop de

		ld (hl), e
		inc l
		ld (hl), d
		inc h

		pop de

		ld (hl), d
		dec l
		ld (hl), e
		inc h
		edup			;<---

		; next CHR line
		ld a, c		;4
		add #20		;7
		ld l, a		;4
		ld h, b		;4 = 27

		dup 4			;<---
		pop de

		ld (hl), e
		inc l
		ld (hl), d
		inc h

		pop de

		ld (hl), d
		dec l
		ld (hl), e
		inc h
		edup			;<---
	endm

	macro	scr_put_clr_block2x2
		pop de

		ld (hl), e
		inc l
		ld (hl), d

		add_hl_val SCR_CHR_WIDTH - 1

		pop de

		ld (hl), e
		inc l
		ld (hl), d
	endm

		; OUT: C - 1-new screen; 0-no screen
		;
	macro	check_adjacent_screen _adj_scr_flag, _offset
		xor a			; reset carry flag
		ld hl, (map_curr_scr)
		inc hl

		IF TR_DATA_MARKS
		ld a, (hl)		; get adjacent screens mark
		and _adj_scr_flag
		ret z
		inc hl			; skip mark
		ENDIF //TR_DATA_MARKS

		inc hl			; skip screen id

		IF TR_ADJ_SCREENS

		IF _offset > 0
		ld bc, _offset
		add hl, bc
		ENDIF

		ELSE

		IF _offset > 0
		ld bc, _offset >> 1
		add hl, bc
		ENDIF

		ld a, (hl)
		cp #ff
		ret z

		add a, a
		ld c, a
		ld b, 0
		ld hl, (map_scr_arr)		
		add hl, bc

		ENDIF //TR_ADJ_SCREENS

		ld e, (hl)
		inc hl
		ld d, (hl)

		IF !TR_DATA_MARKS
		ld a, e
		or d
		ret z
		ENDIF //TR_DATA_MARKS

		ld (map_curr_scr), de

		scf			; set the carry as sign of a new screen
	endm

init
		; fill the tile graphics lookup table
		
		ld b, 0

		ld ix, tiles_addr_tbl

		xor a

.loop1		ld l, a
		ld h, 0

		dup 5
		add hl,hl
		edup

		ld (ix + 0), l
		ld (ix + 1), h

		inc a
		
		inc ix
		inc ix

		djnz .loop1

		IF TR_COLORED_MAP

		; fill the tile colors lookup table
		
		ld b, 0

		ld ix, tiles_clr_addr_tbl

		xor a

.loop2		ld l, a
		ld h, 0

		add hl,hl
		add hl,hl

		ld (ix + 0), l
		ld (ix + 1), h

		inc a
		
		inc ix
		inc ix

		djnz .loop2

		ENDIF //TR_COLORED_MAP

		ret

check_up_screen
		check_adjacent_screen _adj_scr_up, 2
		ret

check_down_screen
		check_adjacent_screen _adj_scr_down, 6
		ret

check_left_screen
		check_adjacent_screen _adj_scr_left, 0
		ret

check_right_screen
		check_adjacent_screen _adj_scr_right, 4
		ret

; draw screen
; IN: map_curr_scr
;
draw_screen
		IF TR_COLORED_MAP
		vsync
		call _draw_black_screen
		vsync
		ENDIF //TR_COLORED_MAP

		ld de, (map_curr_scr)
		ld a, (de)		; chr id

		IF TR_COLORED_MAP
		push af			; save chr id
		ENDIF //TR_COLORED_MAP

		exx
		add a, a
		ld c, a
		ld b, 0

		ld hl, (map_gfx_arr)
		add hl, bc
		ld e, (hl)
		inc hl
		ld d, (hl)		
		ex de, hl		; hl' - tiles gfx

		IF TR_DATA_TILES4X4
		push hl
		ld hl, (map_tiles_offs)
		add hl, bc
		ld e, (hl)
		inc hl
		ld d, (hl)
		ld hl, (map_tiles4x4)
		add hl, de
		ld (map_tiles4x4_data), hl
		pop hl
		ENDIF //TR_DATA_TILES4X4

		exx

		inc de

		IF TR_DATA_MARKS
		inc de
		ENDIF //TR_DATA_MARKS

		ld a, (de)		; screen index

		exx
		ld c, a
		ld a, (map_scr_size)
		ld d, a
		call d_mul_c
		ld d, c
		ld e, a			; de' - screen tiles offset

		ld bc, (map_screens)
		ex de, hl
		add hl, bc
		ex de, hl		
		ld bc, hl		; bc' - tiles gfx
					; de' - screen tiles
		IF TR_COLORED_MAP
		push de			; save screen tiles
		ENDIF //TR_COLORED_MAP

		exx

		; draw screen tiles

		ld hl, #4000 + SCR_CENTER_OFFS_X

		ld hx, SCR_UNI_WIDTH
.loop1
		push hl
		ld lx, SCR_UNI_HEIGHT
		call _draw_tiles_line
		pop hl

		IF TR_DATA_TILES4X4
		IF TR_DIR_ROWS
		add_hl_val 128
		check_next_third_CHR_line_hl
		ELSE
		ld a, l
		add 4
		ld l, a
		ENDIF //TR_DIR_ROWS
		ELSE //TR_DATA_TILES4X4
		IF TR_DIR_ROWS
		add_hl_val 64
		check_next_third_CHR_line_hl
		ELSE
		inc l
		inc l
		ENDIF //TR_DIR_ROWS
		ENDIF //TR_DATA_TILES4X4

		dec hx
		jp nz, .loop1

		IF TR_COLORED_MAP

		exx
		pop ix			; ix - screen tiles
		pop af			; a - chr id

		add a, a
		ld c, a
		ld b, 0

		ld hl, (map_props_arr)
		add hl, bc
		ld e, (hl)
		inc hl
		ld d, (hl)		
		ld hl, (map_clrs)
		add hl, de		; hl' - colors gfx
		ld bc, hl

		ld e, lx
		ld d, hx		; de' - screen tiles
		exx

		; draw colors buffer

		ld hl, scr_clrs_arr

		ld hx, SCR_UNI_WIDTH
.loop2
		push hl
		ld lx, SCR_UNI_HEIGHT
		call _draw_clrs_line
		pop hl

		IF TR_DATA_TILES4X4
		IF TR_DIR_ROWS
		add_hl_val SCR_CHR_WIDTH << 2
		ELSE
		ld a, l
		add 4
		ld l, a
		ENDIF //TR_DIR_ROWS
		ELSE
		IF TR_DIR_ROWS
		add_hl_val SCR_CHR_WIDTH << 1
		ELSE
		inc l
		inc l
		ENDIF //TR_DIR_ROWS
		ENDIF //TR_DATA_TILES4X4

		dec hx
		jp nz, .loop2

		vsync
		call _draw_clrs_buff
		vsync

		ENDIF //TR_COLORED_MAP

		ret

		IF TR_COLORED_MAP
		
_draw_clrs_buff
		ld de, #5800 + SCR_CENTER_OFFS_X
		ld hl, scr_clrs_arr

		ld lx, SCR_CHR_HEIGHT
.loop1
		push de

		ld bc, SCR_CHR_WIDTH

		dup SCR_CHR_WIDTH
		ldi
		edup

		pop de

		push hl
		ld hl, #20
		add hl, de
		ex de, hl
		pop hl

		dec lx
		jp nz, .loop1

		ret

_draw_black_screen
		ld (_tmp_sp), sp
		
		ld hl, #5800 + SCR_CHR_WIDTH + SCR_CENTER_OFFS_X

		ld lx, SCR_CHR_HEIGHT
.loop1
		ld de, 0
		ld sp, hl

		dup SCR_BLOCKS2x2_WIDTH
		push de
		edup

		ld de, #20
		add hl, de

		dec lx
		jp nz, .loop1

		ld sp, (_tmp_sp)

		ret

		ENDIF //TR_COLORED_MAP

		IF TR_DATA_TILES4X4
; draw colors column
; IN: 	bc' - clrs data
;	de' - screen tiles
;	hl - screen address
;	lx - num tiles
;
_draw_clrs_line
		ld (_tmp_sp), sp
.loop
		exx
		ld a, (de)			; get tile index
		inc de
		exx

		ld c, a
		ld b, 0
		MUL_POW2_BC 2

		ex de, hl

		ld hl, (map_tiles4x4_data)
		add hl, bc

		ld sp, hl		;6
		pop bc			;10
		pop iy			;14
		ld a, b			;4
		ld (._2nd_block_val), a	;13

		ex de, hl

	; draw 1st block
		ld a, c
		exx
		add_addr_ax2 tiles_clr_addr_tbl
		ld sp, hl
		pop hl
		add hl, bc			; hl - tile gfx
		ld sp, hl
		exx

		scr_put_clr_block2x2
		sub_hl_val SCR_CHR_WIDTH - 1

	; daw 2nd block
		db $3e			;ld a, N
._2nd_block_val	db 0			;7
		exx
		add_addr_ax2 tiles_clr_addr_tbl
		ld sp, hl
		pop hl
		add hl, bc			; hl - tile gfx
		ld sp, hl
		exx

		scr_put_clr_block2x2
		add_hl_val SCR_CHR_WIDTH - 3

	; draw 3st block
		ld a, ly
		exx
		add_addr_ax2 tiles_clr_addr_tbl
		ld sp, hl
		pop hl
		add hl, bc			; hl - tile gfx
		ld sp, hl
		exx

		scr_put_clr_block2x2
		sub_hl_val SCR_CHR_WIDTH - 1

	; draw 4st block
		ld a, hy
		exx
		add_addr_ax2 tiles_clr_addr_tbl
		ld sp, hl
		pop hl
		add hl, bc			; hl - tile gfx
		ld sp, hl
		exx

		scr_put_clr_block2x2

		IF TR_DIR_ROWS
		sub_hl_val ( SCR_CHR_WIDTH * 3 ) - 1
		ELSE
		add_hl_val SCR_CHR_WIDTH - 3
		ENDIF //TR_DIR_ROWS

		dec lx
		jp nz, .loop

		ld sp, (_tmp_sp)

		ret

; draw tiles column
; IN: 	bc' - tiles gfx
;	de' - screen tiles
;	hl - screen address
;	lx - num tiles
;
_draw_tiles_line
		ld (_tmp_sp), sp
.loop
		exx
		ld a, (de)			; get tile index
		inc de
		exx

		ld c, a
		ld b, 0
		MUL_POW2_BC 2

		ex de, hl

		ld hl, (map_tiles4x4_data)
		add hl, bc

		ld sp, hl		;6
		pop bc			;10
		pop iy			;14
		ld a, b			;4
		ld (._2nd_block_val), a	;13

	; draw 1st block
		ld a, c
		add_addr_ax2 tiles_addr_tbl
		ld sp, hl
		ex de, hl
		exx
		pop hl
		add hl, bc			; hl - tile gfx
		ld sp, hl
		exx

		scr_put_block2x2

		inc c
		inc c
		ld l, c
		ld h, b

	; draw 2st block
		exx
		db $3e			;ld a, N
._2nd_block_val	db 0			;7
		add_addr_ax2 tiles_addr_tbl
		ld sp, hl
		pop hl
		add hl, bc			; hl - tile gfx
		ld sp, hl
		exx

		scr_put_block2x2

		ld l, c
		ld h, b
		add_hl_val 62

	; draw 3st block
		exx
		ld a, ly
		add_addr_ax2 tiles_addr_tbl
		ld sp, hl
		pop hl
		add hl, bc			; hl - tile gfx
		ld sp, hl
		exx

		scr_put_block2x2

		inc c
		inc c
		ld l, c
		ld h, b

	; draw 4st block
		exx
		ld a, hy
		add_addr_ax2 tiles_addr_tbl
		ld sp, hl
		pop hl
		add hl, bc			; hl - tile gfx
		ld sp, hl
		exx

		scr_put_block2x2

		ld l, c
		ld h, b

		IF TR_DIR_ROWS
		sub_hl_val 62
		ELSE
		add_hl_val 62
		check_next_third_CHR_line_hl
		ENDIF //TR_DIR_ROWS

		dec lx
		jp nz, .loop

		ld sp, (_tmp_sp)

		ret

		ELSE //TR_DATA_TILES4X4
; draw colors column
; IN: 	bc' - clrs data
;	de' - screen tiles
;	hl - screen address
;	lx - num tiles
;
_draw_clrs_line
		ld (_tmp_sp), sp
.loop
		exx
		ld a, (de)			; get tile index
		inc de
		
		add_addr_ax2 tiles_clr_addr_tbl
		ld sp, hl
		pop hl
		add hl, bc			; hl - tile gfx

		ld sp, hl
		exx

		scr_put_clr_block2x2

		IF TR_DIR_ROWS
		sub_hl_val SCR_CHR_WIDTH - 1
		ELSE
		add_hl_val SCR_CHR_WIDTH - 1
		ENDIF //TR_DIR_ROWS

		dec lx
		jp nz, .loop

		ld sp, (_tmp_sp)

		ret

; draw tiles column
; IN: 	bc' - tiles gfx
;	de' - screen tiles
;	hl - screen address
;	lx - num tiles
;
_draw_tiles_line
		ld (_tmp_sp), sp
.loop
		exx
		ld a, (de)			; get tile index
		inc de
		
		add_addr_ax2 tiles_addr_tbl
		ld sp, hl
		pop hl
		add hl, bc			; hl - tile gfx

		ld sp, hl
		exx

		scr_put_block2x2

		IF TR_DIR_ROWS
		inc c
		inc c
		ld l, c
		ld h, b
		ELSE
		check_next_third_line_hl
		ENDIF //TR_DIR_ROWS

		dec lx
		jp nz, .loop

		ld sp, (_tmp_sp)

		ret

		ENDIF //TR_DATA_TILES4X4

_tmp_sp		dw 0

		ENDMODULE