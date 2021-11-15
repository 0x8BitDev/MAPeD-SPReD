;###################################################################
;
; Copyright 2018-2021 0x8BitDev ( MIT license )
;
;###################################################################
;
; DESC: Bidirectional scroller example.
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
; Additional options
;
; DEF_SCR_SCROLL	1/0
; DEF_VERT_SYNC		1/0
;


		MODULE	tilemap_render

		include "common.asm"

		assert ( MAP_DATA_MAGIC&MAP_FLAG_LAYOUT_MATRIX ) == 0, The sample doesn't support matrix layout data!
		assert ( MAP_DATA_MAGIC&MAP_FLAG_MODE_MULTIDIR_SCROLL ) == 0, The sample doesn't support data for multidirectional scroller!
		assert ( MAP_DATA_MAGIC&MAP_FLAG_RLE ) == 0, The sample doesn't support compressed data!

		assert SCR_BLOCKS2x2_WIDTH <= 16, The exported screen size is more than 256x192!
		assert SCR_BLOCKS2x2_HEIGHT <= 12, The exported screen size is more than 256x192!

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

SCR_SIZE	equ ( SCR_BLOCKS2x2_WIDTH * SCR_BLOCKS2x2_HEIGHT ) >> 2
		ELSE
SCR_SIZE	equ SCR_BLOCKS2x2_WIDTH * SCR_BLOCKS2x2_HEIGHT
		ENDIF //TR_DATA_TILES4X4

		IF TR_DATA_TILES4X4
		assert ( SCR_BLOCKS2x2_WIDTH&0x01 ) == 0, The sample doesn't support half tile data!
		assert ( SCR_BLOCKS2x2_HEIGHT&0x01 ) == 0, The sample doesn't support half tile data!
		ENDIF //TR_DATA_TILES4X4

; this data must be filled in for each map in the main code
;
map_screens	dw 0			; tilemap_TilesScr screen tiles aray for each screen
map_gfx_arr	dw 0			; tilemap_Gfx exported graphics array

map_props_arr	dw 0			; tilemap_BlocksPropsOffs

map_curr_scr	dw 0

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

		IF DEF_SCR_SCROLL
scr_scroll_dir		db 0	; 0-left; 1-up; 2-right; 3-down

scr_scr_addr		dw 0
scr_tile_data_offset	dw 0
scr_skip_tiles		db 0

			IF TR_COLORED_MAP
scr_attr_addr		dw 0
			ENDIF //TR_COLORED_MAP

scr_tiles_width		db 0
scr_tiles_height	db 0

map_new_scr		dw 0

		IF DEF_128K_DBL_BUFFER
dbl_buff_scr_addr	dw 0
dbl_buff_attr_addr	dw 0
		ENDIF //DEF_128K_DBL_BUFFER
		ENDIF //DEF_SCR_SCROLL

			align 256
tiles_addr_tbl		block 512,0	; tile graphics address table
tiles_clr_addr_tbl	block 512,0	; tile colors(attrbutes) address table

_adj_scr_left	= %00010000
_adj_scr_up	= %00100000
_adj_scr_right	= %01000000
_adj_scr_down	= %10000000

	macro	SCR_PUT_BLOCK2X2
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

	macro	SCR_PUT_CLR_BLOCK2X2
		pop de

		ld (hl), e
		inc hl
		ld (hl), d

		ADD_HL_VAL SCR_CHR_WIDTH - 1

		pop de

		ld (hl), e
		inc hl
		ld (hl), d
	endm

		; OUT: Carry flag - 1-new screen; 0-no screen
		;
	macro	CHECK_ADJACENT_SCREEN _adj_scr_flag, _offset
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

		add a
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

		IF DEF_SCR_SCROLL
		ld (map_new_scr), de

		ld a, _offset >> 1
		ld (scr_scroll_dir), a
		ELSE
		ld (map_curr_scr), de
		ENDIF //DEF_SCR_SCROLL

		scf			; set the carry as sign of a new screen
	endm

	macro	LD_IXH _val, _alt_val
		IF DEF_SCR_SCROLL
		ld a, (_alt_val)
		ld ixh, a
		ELSE
		ld ixh, _val
		ENDIF //DEF_SCR_SCROLL
	endm

	macro	LD_IXL _val, _alt_val
		IF DEF_SCR_SCROLL
		ld a, (_alt_val)
		ld ixl, a
		ELSE
		ld ixl, _val
		ENDIF //DEF_SCR_SCROLL
	endm

	macro	LD_HL _val, _alt_val
		IF DEF_SCR_SCROLL
		ld hl, (_alt_val)
		ELSE
		ld hl, _val
		ENDIF //DEF_SCR_SCROLL
	endm

	macro	GET_SCR_ADDR_HL
		IF DEF_128K_DBL_BUFFER
		ld hl, (dbl_buff_scr_addr)
		ELSE
		ld hl, #4000 + SCR_CENTER_OFFS_X
		ENDIF //DEF_128K_DBL_BUFFER
	endm

	macro	GET_ATTR_ADDR_DE
		IF DEF_128K_DBL_BUFFER
		ld de, (dbl_buff_attr_addr)
		ELSE
		ld de, #5800 + SCR_CENTER_OFFS_X
		ENDIF //DEF_128K_DBL_BUFFER
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

		IF DEF_128K_DBL_BUFFER

		xor a
		ld (_scr_trigg), a

		ENDIF //DEF_128K_DBL_BUFFER

		ret

check_up_screen
		CHECK_ADJACENT_SCREEN _adj_scr_up, 2
		ret

check_down_screen
		CHECK_ADJACENT_SCREEN _adj_scr_down, 6
		ret

check_left_screen
		CHECK_ADJACENT_SCREEN _adj_scr_left, 0
		ret

check_right_screen
		CHECK_ADJACENT_SCREEN _adj_scr_right, 4
		ret

		IF DEF_SCR_SCROLL
_scr_scroll_left

		IF TR_DIR_ROWS
		ld a, SCR_UNI_WIDTH
		ld (scr_tiles_width), a

		ld a, SCR_UNI_HEIGHT - 1
		ELSE
		ld a, SCR_UNI_HEIGHT
		ld (scr_tiles_height), a

		ld a, SCR_UNI_WIDTH - 1
		ENDIF //TR_DIR_ROWS
.loop
		push af

		IF DEF_128K_DBL_BUFFER
		call _check_active_screen
		ENDIF //DEF_128K_DBL_BUFF

		pop af
		push af		

	; draw new screen

		IF TR_DIR_ROWS

		ld (scr_skip_tiles), a

		ld l, a
		ld h, 0
		ld (scr_tile_data_offset), hl

		ld c, a
		ld a, SCR_UNI_HEIGHT

		sub c

		ld (scr_tiles_height), a

		ELSE //TR_DIR_ROWS

		ld c, a
		ld a, SCR_UNI_WIDTH
		sub c

		ld (scr_tiles_width), a

		ld d, SCR_UNI_HEIGHT

		call d_mul_c
		ld b, c
		ld c, a

		ld (scr_tile_data_offset), bc

		ENDIF //TR_DIR_ROWS

		GET_SCR_ADDR_HL
		ld (scr_scr_addr), hl

		IF TR_COLORED_MAP
		ld de, scr_clrs_arr
		ld (scr_attr_addr), de
		ENDIF //TR_COLORED_MAP

		ld de, (map_new_scr)

		call _draw_screen_part

	; draw current screen

		pop af
		push af

		IF TR_DIR_ROWS

		ld (scr_tiles_height), a

		ld b, a
		ld a, SCR_UNI_HEIGHT
		sub b

		ld (scr_skip_tiles), a

		ld hl, 0
		ld (scr_tile_data_offset), hl

		ELSE //TR_DIR_ROWS

		ld (scr_tiles_width), a

		ld b, a
		ld a, SCR_UNI_WIDTH
		sub b

		ENDIF //TR_DIR_ROWS

		add a

		IF TR_DATA_TILES4X4
		add a
		ENDIF //TR_DATA_TILES4X4

		ld c, a

		GET_SCR_ADDR_HL
		ADD_HL_A
		ld (scr_scr_addr), hl

		IF TR_COLORED_MAP
		ld hl, scr_clrs_arr
		ld a, c
		ADD_HL_A
		ld (scr_attr_addr), hl
		ENDIF //TR_COLORED_MAP

		ld hl, 0
		ld (scr_tile_data_offset), hl

		ld de, (map_curr_scr)

		call _draw_screen_part

		pop af

		dec a
		jp nz, .loop

		ld hl, (map_new_scr)
		ld (map_curr_scr), hl

		jp _draw_curr_screen

_scr_scroll_up

		IF TR_DIR_ROWS
		ld a, SCR_UNI_HEIGHT
		ld (scr_tiles_height), a

		ld a, SCR_UNI_WIDTH - 1
		ELSE
		ld a, SCR_UNI_WIDTH
		ld (scr_tiles_width), a

		ld a, SCR_UNI_HEIGHT - 1
		ENDIF //TR_DIR_ROWS
.loop
		push af

		IF DEF_128K_DBL_BUFFER
		call _check_active_screen
		ENDIF //DEF_128K_DBL_BUFF

		pop af
		push af		

	; draw new screen

		IF TR_DIR_ROWS

		ld c, a
		ld a, SCR_UNI_WIDTH
		sub c

		ld (scr_tiles_width), a

		ld d, SCR_UNI_HEIGHT

		call d_mul_c
		ld b, c
		ld c, a

		ld (scr_tile_data_offset), bc

		ELSE //TR_DIR_ROWS

		ld (scr_skip_tiles), a

		ld l, a
		ld h, 0
		ld (scr_tile_data_offset), hl

		ld c, a
		ld a, SCR_UNI_HEIGHT

		sub c

		ld (scr_tiles_height), a

		ENDIF //TR_DIR_ROWS

		GET_SCR_ADDR_HL
		ld (scr_scr_addr), hl

		IF TR_COLORED_MAP
		ld de, scr_clrs_arr
		ld (scr_attr_addr), de
		ENDIF //TR_COLORED_MAP

		ld de, (map_new_scr)

		call _draw_screen_part

	; draw current screen

		pop af
		push af

		IF TR_DIR_ROWS

		ld (scr_tiles_width), a

		ld b, a
		ld a, SCR_UNI_WIDTH
		sub b

		ELSE //TR_DIR_ROWS

		ld (scr_tiles_height), a

		ld b, a
		ld a, SCR_UNI_HEIGHT
		sub b

		ld (scr_skip_tiles), a

		ENDIF //TR_DIR_ROWS

		ld hl, 0
		ld (scr_tile_data_offset), hl

		add a

		IF TR_DATA_TILES4X4
		add a
		ENDIF //TR_DATA_TILES4X4

		ld c, a
		ld b, 0
		MUL_POW2_BC 5

		sla b
		sla b
		sla b

		GET_SCR_ADDR_HL
		add hl, bc

		ld (scr_scr_addr), hl

		ld hl, 0
		ld (scr_tile_data_offset), hl

		IF TR_COLORED_MAP
		ld hl, scr_clrs_arr
		ld c, a
		ld d, SCR_BLOCKS2x2_WIDTH << 1
		call d_mul_c
		ld b, c
		ld c, a
		add hl, bc
		ld (scr_attr_addr), hl
		ENDIF //TR_COLORED_MAP

		ld de, (map_curr_scr)

		call _draw_screen_part

		pop af

		dec a
		jp nz, .loop

		ld hl, (map_new_scr)
		ld (map_curr_scr), hl

		jp _draw_curr_screen

_scr_scroll_right

		IF TR_DIR_ROWS
		ld a, SCR_UNI_WIDTH
		ld (scr_tiles_width), a

		ld a, SCR_UNI_HEIGHT - 1
		ELSE
		ld a, SCR_UNI_HEIGHT
		ld (scr_tiles_height), a

		ld a, SCR_UNI_WIDTH - 1
		ENDIF //TR_DIR_ROWS
.loop
		push af

		IF DEF_128K_DBL_BUFFER
		call _check_active_screen
		ENDIF //DEF_128K_DBL_BUFF

		pop af
		push af		

	; draw current screen

		IF TR_DIR_ROWS

		ld (scr_tiles_height), a

		ld c, a
		ld a, SCR_UNI_HEIGHT

		sub c

		ld (scr_skip_tiles), a

		ld l, a
		ld h, 0
		ld (scr_tile_data_offset), hl

		ELSE //TR_DIR_ROWS

		ld (scr_tiles_width), a

		ld c, a
		ld a, SCR_UNI_WIDTH
		sub c

		ld c, a
		ld d, SCR_UNI_HEIGHT

		call d_mul_c
		ld b, c
		ld c, a

		ld (scr_tile_data_offset), bc

		ld b, a
		ld a, SCR_UNI_WIDTH
		sub b

		ENDIF //TR_DIR_ROWS

		add a
		
		IF TR_DATA_TILES4X4
		add a
		ENDIF //TR_DATA_TILES4X4

		GET_SCR_ADDR_HL
		ld (scr_scr_addr), hl

		IF TR_COLORED_MAP
		ld de, scr_clrs_arr
		ld (scr_attr_addr), de
		ENDIF //TR_COLORED_MAP

		ld de, (map_curr_scr)

		call _draw_screen_part

	; draw new screen

		pop af
		push af

		IF TR_DIR_ROWS

		ld (scr_skip_tiles), a

		ld b, a
		ld a, SCR_UNI_HEIGHT
		sub b

		ld (scr_tiles_height), a

		ld hl, 0
		ld (scr_tile_data_offset), hl

		ld a, b

		ELSE //TR_DIR_ROWS

		ld c, a
		ld a, SCR_UNI_WIDTH
		sub c

		ld (scr_tiles_width), a

		ld a, c

		ENDIF //TR_DIR_ROWS

		add a

		IF TR_DATA_TILES4X4
		add a
		ENDIF //TR_DATA_TILES4X4

		ld c, a

		GET_SCR_ADDR_HL
		ADD_HL_A
		ld (scr_scr_addr), hl

		IF TR_COLORED_MAP
		ld hl, scr_clrs_arr
		ld a, c
		ADD_HL_A
		ld (scr_attr_addr), hl
		ENDIF //TR_COLORED_MAP

		ld hl, 0
		ld (scr_tile_data_offset), hl

		ld de, (map_new_scr)

		call _draw_screen_part

		pop af

		dec a
		jp nz, .loop

		ld hl, (map_new_scr)
		ld (map_curr_scr), hl

		jp _draw_curr_screen

_scr_scroll_down

		IF TR_DIR_ROWS
		ld a, SCR_UNI_HEIGHT
		ld (scr_tiles_height), a

		ld a, SCR_UNI_WIDTH - 1
		ELSE
		ld a, SCR_UNI_WIDTH
		ld (scr_tiles_width), a

		ld a, SCR_UNI_HEIGHT - 1
		ENDIF //TR_DIR_ROWS
.loop
		push af

		IF DEF_128K_DBL_BUFFER
		call _check_active_screen
		ENDIF //DEF_128K_DBL_BUFF

		pop af
		push af		

	; draw current screen

		IF TR_DIR_ROWS

		ld (scr_tiles_width), a

		ld c, a
		ld a, SCR_UNI_WIDTH
		sub c

		ld c, a
		ld d, SCR_UNI_HEIGHT

		call d_mul_c
		ld b, c
		ld c, a

		ld (scr_tile_data_offset), bc

		ld b, a
		ld a, SCR_UNI_WIDTH
		sub b

		ELSE //TR_DIR_ROWS

		ld (scr_tiles_height), a

		ld c, a
		ld a, SCR_UNI_HEIGHT

		sub c

		ld (scr_skip_tiles), a

		ld l, a
		ld h, 0
		ld (scr_tile_data_offset), hl

		ENDIF //TR_DIR_ROWS

		GET_SCR_ADDR_HL
		ld (scr_scr_addr), hl

		IF TR_COLORED_MAP
		ld de, scr_clrs_arr
		ld (scr_attr_addr), de
		ENDIF //TR_COLORED_MAP

		ld de, (map_curr_scr)

		call _draw_screen_part

	; draw new screen

		pop af
		push af

		IF TR_DIR_ROWS

		ld c, a
		ld a, SCR_UNI_WIDTH
		sub c

		ld (scr_tiles_width), a

		ld a, c

		ELSE //TR_DIR_ROWS

		ld (scr_skip_tiles), a

		ld b, a
		ld a, SCR_UNI_HEIGHT
		sub b

		ld (scr_tiles_height), a

		ld hl, 0
		ld (scr_tile_data_offset), hl

		ld a, b

		ENDIF //TR_DIR_ROWS

		add a

		IF TR_DATA_TILES4X4
		add a
		ENDIF //TR_DATA_TILES4X4

		ld c, a
		ld b, 0
		MUL_POW2_BC 5

		sla b
		sla b
		sla b

		GET_SCR_ADDR_HL
		add hl, bc

		ld (scr_scr_addr), hl

		ld hl, 0
		ld (scr_tile_data_offset), hl

		IF TR_COLORED_MAP
		ld hl, scr_clrs_arr
		ld c, a
		ld d, SCR_BLOCKS2x2_WIDTH << 1
		call d_mul_c
		ld b, c
		ld c, a
		add hl, bc
		ld (scr_attr_addr), hl
		ENDIF //TR_COLORED_MAP

		ld de, (map_new_scr)

		call _draw_screen_part

		pop af

		dec a
		jp nz, .loop

		ld hl, (map_new_scr)
		ld (map_curr_scr), hl

;(!)		jp _draw_curr_screen

_draw_curr_screen

		IF DEF_128K_DBL_BUFFER
		call _check_active_screen
		ENDIF //DEF_128K_DBL_BUFF

		GET_SCR_ADDR_HL
		ld (scr_scr_addr), hl

		IF TR_COLORED_MAP
		ld hl, scr_clrs_arr
		ld (scr_attr_addr), hl
		ENDIF //TR_COLORED_MAP

		ld a, SCR_UNI_WIDTH
		ld (scr_tiles_width), a
		ld a, SCR_UNI_HEIGHT
		ld (scr_tiles_height), a

		xor a
		ld (scr_skip_tiles), a

		ld hl, 0
		ld (scr_tile_data_offset), hl

		ld de, (map_curr_scr)

		call _draw_screen_part

		IF DEF_128K_DBL_BUFFER
		call _show_drawn_screen
		ENDIF //DEF_128K_DBL_BUFF		

		ret

		ENDIF //DEF_SCR_SCROLL

; draw screen
; IN: map_curr_scr
;
draw_screen
		IF DEF_SCR_SCROLL

		ld hl, (map_new_scr)
		ld a, h
		or l
		jp nz, .cont

		jp _draw_curr_screen
.cont
		ld a, (scr_scroll_dir)
		cp 0
		jp z, _scr_scroll_left
		cp 1
		jp z, _scr_scroll_up
		cp 2
		jp z, _scr_scroll_right
		jp _scr_scroll_down

; draw a part of a screen
; IN:	de - screen data addr
;	scr_scr_addr
;	scr_attr_addr
;	scr_tiles_width
;	scr_tiles_height
;
_draw_screen_part
		ELSE
		IF TR_COLORED_MAP
		VSYNC
		call _draw_black_screen
		VSYNC
		ENDIF //TR_COLORED_MAP
		ld de, (map_curr_scr)
		ENDIF //DEF_SCR_SCROLL

		ld a, (de)		; chr id

		IF TR_COLORED_MAP
		push af			; save chr id
		ENDIF //TR_COLORED_MAP

		exx
		add a
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
		ld a, SCR_SIZE
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
		IF DEF_SCR_SCROLL
		ex de, hl
		ld de, (scr_tile_data_offset)
		add hl, de
		ex de, hl
		ENDIF //DEF_SCR_SCROLL

		IF TR_COLORED_MAP
		push de			; save screen tiles
		ENDIF //TR_COLORED_MAP

		exx

		; draw screen tiles

		LD_HL #4000 + SCR_CENTER_OFFS_X, scr_scr_addr

		LD_IXH SCR_UNI_WIDTH, scr_tiles_width
.loop1
		push hl
		LD_IXL SCR_UNI_HEIGHT, scr_tiles_height
		call _draw_tiles_line
		pop hl

		IF DEF_SCR_SCROLL
		exx
		ld a, (scr_skip_tiles)
		ld l, a
		ld a, e
		add l
		ld e, a
		jp nc, .skip1
		inc d
.skip1
		exx
		ENDIF //DEF_SCR_SCROLL

		IF TR_DATA_TILES4X4
		IF TR_DIR_ROWS
		ADD_HL_VAL 128
		CHECK_NEXT_THIRD_CHR_LINE_HL
		ELSE
		ld a, l
		add 4
		ld l, a
		ENDIF //TR_DIR_ROWS
		ELSE //TR_DATA_TILES4X4
		IF TR_DIR_ROWS
		ADD_HL_VAL 64
		CHECK_NEXT_THIRD_CHR_LINE_HL
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

		add a
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

		LD_HL scr_clrs_arr, scr_attr_addr

		LD_IXH SCR_UNI_WIDTH, scr_tiles_width
.loop2
		push hl
		LD_IXL SCR_UNI_HEIGHT, scr_tiles_height
		call _draw_clrs_line

		IF DEF_SCR_SCROLL
		exx
		ld a, (scr_skip_tiles)
		ld l, a
		ld a, e
		add l
		ld e, a
		jp nc, .skip2
		inc d
.skip2
		exx
		ENDIF //DEF_SCR_SCROLL

		pop hl

		IF TR_DATA_TILES4X4
		IF TR_DIR_ROWS
		ADD_HL_VAL SCR_CHR_WIDTH << 2
		ELSE
		ADD_HL_VAL 4
		ENDIF //TR_DIR_ROWS
		ELSE
		IF TR_DIR_ROWS
		ADD_HL_VAL SCR_CHR_WIDTH << 1
		ELSE
		ADD_HL_VAL 2
		ENDIF //TR_DIR_ROWS
		ENDIF //TR_DATA_TILES4X4

		dec hx
		jp nz, .loop2

		IF DEF_SCR_SCROLL
		call _draw_clrs_buff
		ELSE
		VSYNC
		call _draw_clrs_buff
		VSYNC
		ENDIF //DEF_SCR_SCROLL

		ENDIF //TR_COLORED_MAP

		ret

		IF TR_COLORED_MAP
		
_draw_clrs_buff
		GET_ATTR_ADDR_DE
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
		ADD_ADDR_AX2 tiles_clr_addr_tbl
		ld sp, hl
		pop hl
		add hl, bc			; hl - tile gfx
		ld sp, hl
		exx

		SCR_PUT_CLR_BLOCK2X2
		SUB_HL_VAL SCR_CHR_WIDTH - 1

	; daw 2nd block
		db $3e			;ld a, N
._2nd_block_val	db 0			;7
		exx
		ADD_ADDR_AX2 tiles_clr_addr_tbl
		ld sp, hl
		pop hl
		add hl, bc			; hl - tile gfx
		ld sp, hl
		exx

		SCR_PUT_CLR_BLOCK2X2
		ADD_HL_VAL SCR_CHR_WIDTH - 3

	; draw 3st block
		ld a, ly
		exx
		ADD_ADDR_AX2 tiles_clr_addr_tbl
		ld sp, hl
		pop hl
		add hl, bc			; hl - tile gfx
		ld sp, hl
		exx

		SCR_PUT_CLR_BLOCK2X2
		SUB_HL_VAL SCR_CHR_WIDTH - 1

	; draw 4st block
		ld a, hy
		exx
		ADD_ADDR_AX2 tiles_clr_addr_tbl
		ld sp, hl
		pop hl
		add hl, bc			; hl - tile gfx
		ld sp, hl
		exx

		SCR_PUT_CLR_BLOCK2X2

		IF TR_DIR_ROWS
		SUB_HL_VAL ( SCR_CHR_WIDTH * 3 ) - 1
		ELSE
		ADD_HL_VAL SCR_CHR_WIDTH - 3
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
		ADD_ADDR_AX2 tiles_addr_tbl
		ld sp, hl
		ex de, hl
		exx
		pop hl
		add hl, bc			; hl - tile gfx
		ld sp, hl
		exx

		SCR_PUT_BLOCK2X2

		inc c
		inc c
		ld l, c
		ld h, b

	; draw 2st block
		exx
		db $3e			;ld a, N
._2nd_block_val	db 0			;7
		ADD_ADDR_AX2 tiles_addr_tbl
		ld sp, hl
		pop hl
		add hl, bc			; hl - tile gfx
		ld sp, hl
		exx

		SCR_PUT_BLOCK2X2

		ld l, c
		ld h, b
		ADD_HL_VAL 62

	; draw 3st block
		exx
		ld a, ly
		ADD_ADDR_AX2 tiles_addr_tbl
		ld sp, hl
		pop hl
		add hl, bc			; hl - tile gfx
		ld sp, hl
		exx

		SCR_PUT_BLOCK2X2

		inc c
		inc c
		ld l, c
		ld h, b

	; draw 4st block
		exx
		ld a, hy
		ADD_ADDR_AX2 tiles_addr_tbl
		ld sp, hl
		pop hl
		add hl, bc			; hl - tile gfx
		ld sp, hl
		exx

		SCR_PUT_BLOCK2X2

		ld l, c
		ld h, b

		IF TR_DIR_ROWS
		SUB_HL_VAL 62
		ELSE
		ADD_HL_VAL 62
		CHECK_NEXT_THIRD_CHR_LINE_HL
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
		
		ADD_ADDR_AX2 tiles_clr_addr_tbl
		ld sp, hl
		pop hl
		add hl, bc			; hl - tile gfx

		ld sp, hl
		exx

		SCR_PUT_CLR_BLOCK2X2

		IF TR_DIR_ROWS
		SUB_HL_VAL SCR_CHR_WIDTH - 1
		ELSE
		ADD_HL_VAL SCR_CHR_WIDTH - 1
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
		
		ADD_ADDR_AX2 tiles_addr_tbl
		ld sp, hl
		pop hl
		add hl, bc			; hl - tile gfx

		ld sp, hl
		exx

		SCR_PUT_BLOCK2X2

		IF TR_DIR_ROWS
		inc c
		inc c
		ld l, c
		ld h, b
		ELSE
		CHECK_NEXT_THIRD_LINE_HL
		ENDIF //TR_DIR_ROWS

		dec lx
		jp nz, .loop

		ld sp, (_tmp_sp)

		ret

		ENDIF //TR_DATA_TILES4X4

_tmp_sp		dw 0

		IF DEF_128K_DBL_BUFFER

_check_active_screen

		VSYNC

		ld a, (_scr_trigg)

		xor #01
		cp #01

		ld (_scr_trigg), a

		jp nz, .draw_scr0

		; show the main screen and draw on the extended one

		call _switch_Uscr

		ld hl, #c000 + SCR_CENTER_OFFS_X
		ld (dbl_buff_scr_addr), hl

		ld hl, #d800 + SCR_CENTER_OFFS_X
		ld (dbl_buff_attr_addr), hl

		ret

.draw_scr0
		; show the extended screen and draw on the main one

		call _switch_Escr

		ld hl, #4000 + SCR_CENTER_OFFS_X
		ld (dbl_buff_scr_addr), hl

		ld hl, #5800 + SCR_CENTER_OFFS_X
		ld (dbl_buff_attr_addr), hl

		ret

_show_drawn_screen

		ld a, (_scr_trigg)
		cp #01
		ld (_scr_trigg), a

		jp nz, .show_scr0

		; show the extended screen and draw on the main one

		jp _switch_Escr

.show_scr0
		; show the main screen and draw on the extended one

		jp _switch_Uscr



		ENDIF //DEF_128K_DBL_BUFFER

		ENDMODULE