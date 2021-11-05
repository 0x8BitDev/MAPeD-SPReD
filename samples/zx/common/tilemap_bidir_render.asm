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
		assert ( MAP_DATA_MAGIC&MAP_FLAG_DIR_ROWS ) == 0, The sample doesn't support rows ordered data!

TR_DATA_TILES4X4	equ MAP_DATA_MAGIC&MAP_FLAG_TILES4X4
TR_DATA_MARKS		equ MAP_DATA_MAGIC&MAP_FLAG_MARKS
TR_COLORED_MAP		equ MAP_DATA_MAGIC&MAP_FLAG_COLOR_TILES
TR_ADJ_SCREENS		equ MAP_DATA_MAGIC&MAP_FLAG_LAYOUT_ADJACENT_SCREENS

SCR_CHR_WIDTH		equ SCR_BLOCKS2x2_WIDTH << 1
SCR_CHR_HEIGHT		equ SCR_BLOCKS2x2_HEIGHT << 1

; this data must be filled in for each map in the main code
;
map_screens	dw 0			; tilemap_TilesScr screen tiles aray for each screen
map_tiles_offs	dw 0			; Tiles(4x4)-tilemap_TilesOffs / Blocks(2x2)-tilemap_BlocksPropsOffs
map_gfx_arr	dw 0			; tilemap_Gfx exported graphics array

map_curr_scr	dw 0

map_scr_size	db 0

		IF !TR_ADJ_SCREENS
map_scr_arr	dw 0			; Lev0_ScrArr
		ENDIF //!TR_ADJ_SCREENS

		IF TR_DATA_TILES4X4
map_tiles4x4	dw 0			; tilemap_Tiles	tiles 4x4 data
		ELSE
map_props_arr	dw 0			; tilemap_BlocksPropsOffs
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
		ld a, l		;4 save hl
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
		add #20		;7
		ld l, a		;4
		ld h, b		;4 = 23

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

		ld a, 31
		add_hl_a

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
		ei
		halt
		di

		call _draw_black_screen

		ei
		halt
		di
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

		ld hl, #4000

		ld hx, SCR_BLOCKS2x2_WIDTH
.loop1
		push hl
		call _draw_tiles_col
		pop hl

		inc l
		inc l

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

		ld hx, SCR_BLOCKS2x2_WIDTH
.loop2
		push hl
		call _draw_clrs_col
		pop hl

		inc l
		inc l

		dec hx
		jp nz, .loop2

		ei
		halt
		di

		call _draw_clrs_buff

		ei
		halt
		di

		ENDIF //TR_COLORED_MAP

		ret

		IF TR_COLORED_MAP
		
_draw_clrs_buff
		ld de, #5800
		ld hl, scr_clrs_arr

		ld lx, SCR_BLOCKS2x2_HEIGHT << 1
.loop1
		push de

		ld bc, SCR_BLOCKS2x2_WIDTH << 1

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
		
		ld hl, #5800 + ( SCR_BLOCKS2x2_WIDTH << 1 )

		ld lx, SCR_BLOCKS2x2_HEIGHT << 1
.loop1
		ld de, 0
		ld sp, hl

		dup SCR_CHR_WIDTH
		push de
		edup

		ld de, #20
		add hl, de

		dec lx
		jp nz, .loop1

		ld sp, (_tmp_sp)

		ret

		ENDIF //TR_COLORED_MAP

; draw colors column
; IN: 	bc' - clrs data
;	de' - screen tiles
;	hl - screen address
;
_draw_clrs_col
		ld (_tmp_sp), sp

		ld lx, SCR_BLOCKS2x2_HEIGHT
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

		ld a, 31
		add_hl_a

		dec lx
		jp nz, .loop

		ld sp, (_tmp_sp)

		ret

; draw tiles column
; IN: 	bc' - tiles gfx
;	de' - screen tiles
;	hl - screen address
;
_draw_tiles_col
		ld (_tmp_sp), sp

		ld lx, SCR_BLOCKS2x2_HEIGHT
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

		; check the next third
		ld a, l
		add #20
		ld l, a
		jp c, .next
		ld a, h
		sub 8
		ld h, a
.next
		dec lx
		jp nz, .loop

		ld sp, (_tmp_sp)

		ret

_tmp_sp		dw 0

		ENDMODULE