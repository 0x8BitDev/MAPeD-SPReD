;###################################################################
;
; Copyright 2018-2021 0x8BitDev ( MIT license )
;
;###################################################################
;
; DESC: Multidirectional scroller example [direct on-screen drawing].
;
; Public procs:
;
; tilemap_render.init		- render init
; tilemap_render.draw_tiles  	- draw tiles on the screen
; tilemap_render.show_screen 	- ...
;
; Supported options:
;
; MAP_FLAG_TILES2X2
; MAP_FLAG_TILES4X4
; MAP_FLAG_DIR_COLUMNS
; MAP_FLAG_MODE_MULTIDIR_SCROLL
; MAP_FLAG_LAYOUT_MATRIX
; MAP_FLAG_COLOR_TILES
;
; Additional options (see settings.asm):
;
; DEF_128K_DBL_BUFFER	1/0			( 128K dbl buffering or usual 48K rendering )
; DEF_FULLSCREEN	1/0			( fullscreen or upper 2/3 of the screen )
; DEF_COLOR		1/0			( BW or CLR )
; DEF_MOVE_STEP		MS_TILE/MS_8b/MS_4b	( 4/8-bits step or 16/32 depends on exported tile size )
; DEF_VERT_SYNC		1/0			( rendering begins with HALT or not... )
;


		MODULE	tilemap_render

		include "common.asm"

		assert ( MAP_DATA_MAGIC&MAP_FLAG_RLE ) == 0, The sample doesn't support compressed data!
		assert ( MAP_DATA_MAGIC&MAP_FLAG_DIR_ROWS ) == 0, The sample doesn't support rows ordered data!

		assert SCR_BLOCKS2x2_WIDTH == 16, "This sample requires full screen data (blocks 2x2: 16x15)!"
		assert SCR_BLOCKS2x2_HEIGHT == 12, "This sample requires full screen data (blocks 2x2: 16x15)!"

TR_DATA_TILES4X4 equ MAP_DATA_MAGIC&MAP_FLAG_TILES4X4

		IF TR_DATA_TILES4X4
		assert ( DEF_MOVE_STEP == MS_TILE ), The tiles 4x4 mode is only supported when the MS_TILE step mode is active! Please, change the sample settings!
		assert ( SCR_BLOCKS2x2_WIDTH & #01 ) == 0, The tiles 4x4 mode is only supported even number of blocks on a map screen!
		assert ( SCR_BLOCKS2x2_HEIGHT & #01 ) == 0, The tiles 4x4 mode is only supported even number of blocks on a map screen!
		ENDIF

SCR_CHR_WIDTH	equ SCR_BLOCKS2x2_WIDTH << 1
SCR_CHR_HEIGHT	equ SCR_BLOCKS2x2_HEIGHT << 1

MAX_LEV_TILES_W	equ Lev0_wtls		; max width of a level in tiles (!)

; level drawing data

; this data must be filled in for each map in the main code
;
map_data	dw 0			; Lev0_Map	game level tile map address
map_tiles_gfx	dw 0			; Lev0_Gfx 	tile graphics data
map_tiles_clr	dw 0			; Lev0_Clrs 	tile colors data
map_data_cnt	dw 0 			; Lev0_t_tiles 	number of tiles in map
map_tiles_cnt	db 0			; Lev0_u_tiles 	number of unique tiles in map

		IF TR_DATA_TILES4X4
map_tiles4x4	dw 0			; Lev0_Tiles	tiles 4x4 data
		ENDIF //TR_DATA_TILES4X4

map_tiles_w	dw 0			; Lev0_wtls number of tiles in map in width
map_tiles_h	dw 0			; Lev0_htls number of tiles in map in height
map_chrs_w	dw 0 			; Lev0_wchr number of CHRs in map in width
map_chrs_h	dw 0 			; Lev0_hchr number of CHRs in map in height

map_start_scr	db 0			; Lev0_StartScr
map_scr_width	db 0			; Lev0_wscr number of map screens in width
map_scr_height	db 0			; Lev0_hscr number of map screens in height
map_scr_blocks_w
		db 0			; number of screen blocks (2x2) in width
map_scr_blocks_h
		db 0			; number of screen blocks (2x2) in height

; start screen coordinates (upper left corner) in map coordinate system

x_pos		dw 0
y_pos		dw 0

; parameters of visible screen area

scr_w		equ 32	; horizontal screen size in CHRs
		
		IF	DEF_FULLSCREEN
scr_h		equ 24	; vertical screen size in CHRs
		ELSE
scr_h		equ 16	; vertical screen size in CHRs
		ENDIF	//DEF_FULLSCREEN

		IF TR_DATA_TILES4X4
scr_buff_tiles_w	equ scr_w >> 2	; screen width in tiles
scr_buff_tiles_h	equ scr_h >> 2	; screen height in tiles
		ELSE
scr_buff_tiles_w	equ scr_w >> 1	; screen width in tiles
scr_buff_tiles_h	equ scr_h >> 1	; screen height in tiles
		ENDIF //TR_DATA_TILES4X4

_x_tile_addr_tbl	block ( MAX_LEV_TILES_W << 1 ), 0	; address table for X coordinate

			align 256
tiles_addr_tbl		block 512,0	; tile graphics address table
tiles_clr_addr_tbl	block 512,0	; tile colors(attrbutes) address table


	macro	SCR_PUT_BLOCK2X2
		ld c, l		;4 save hl
		ld b, h		;4

		SCR_PUT_BLOCK2XN 4

		; next CHR line
		ld a, c		;4
		add #20		;7
		ld l, a		;4
		ld h, b		;4 = 27

		SCR_PUT_BLOCK2XN 4
	endm

	macro	SCR_PUT_BLOCK2X2_CLR
		pop de

		ld (hl), e
		inc l
		ld (hl), d

		ADD_HL_VAL SCR_CHR_WIDTH - 1

		pop de

		ld (hl), e
		inc l
		ld (hl), d
	endm

	macro	SCR_PUT_BLOCK2XN _cnt
		dup _cnt
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
		edup
	endm

	macro	SCR_PUT_HALF_BLOCK2XN _cnt
		dup _cnt
		pop de

		ld (hl), e
		inc h

		pop de

		ld (hl), e
		inc h		

		edup
	endm

	macro	FIX_SCR_ADDR_HL
		ld a, (_scr_trigg)
		and a
		jp z, .skip

		ld a, h
		or #80
		ld h, a
.skip
	endm

CODE_START

init	
		IF	DEF_128K_DBL_BUFFER

		; draw on the main screen area by default

		xor a
		ld (_scr_trigg), a
		ENDIF	//DEF_128K_DBL_BUFFER

		; write constants directly to the code

                ld hl, (map_tiles_h)
		ld (_hl_mptls_h_mn), hl

		IF	DEF_FULLSCREEN
		ld (_hl_mptls_h_fs), hl
		ENDIF	//DEF_FULLSCREEN
		
		IF	DEF_COLOR
		ld (_hl_mptls_h_cl), hl
		ENDIF	//DEF_COLOR

		; depending on the movement step, write the values into 
		; the procedure for controlling exit of 'camera' beyond a level

		IF	DEF_MOVE_STEP == MS_TILE
		ld hl, (map_chrs_w)
		sra h
		rr l
		IF TR_DATA_TILES4X4
		sra h
		rr l
		ENDIF //TR_DATA_TILES4X4
		ld (_de_map_st_w), hl
		ld hl, (map_chrs_h)
		sra h
		rr l
		IF TR_DATA_TILES4X4
		sra h
		rr l
		ENDIF //TR_DATA_TILES4X4
		ld (_de_map_st_h), hl
		ld hl, scr_w
		sra h
		rr l
		IF TR_DATA_TILES4X4
		sra h
		rr l
		ENDIF //TR_DATA_TILES4X4
		ld (_de_scr_st_w), hl
		ld hl, scr_h
		sra h
		rr l
		IF TR_DATA_TILES4X4
		sra h
		rr l
		ENDIF //TR_DATA_TILES4X4
		ld (_de_scr_st_h), hl
		ENDIF	//DEF_MOVE_STEP == MS_8b

		IF	DEF_MOVE_STEP == MS_8b
		ld hl, (map_chrs_w)
		ld (_de_map_st_w), hl
		ld hl, (map_chrs_h)
		ld (_de_map_st_h), hl
		ld hl, scr_w
		ld (_de_scr_st_w), hl
		ld hl, scr_h
		ld (_de_scr_st_h), hl
		ENDIF	//DEF_MOVE_STEP == MS_8b

		IF	DEF_MOVE_STEP == MS_4b
		ld hl, (map_chrs_w)
		add hl, hl
		ld (_de_map_st_w), hl
		ld hl, (map_chrs_h)
		add hl, hl
		ld (_de_map_st_h), hl
		ld hl, scr_w
		add hl, hl
		ld (_de_scr_st_w), hl
		ld hl, scr_h
		add hl, hl
		ld (_de_scr_st_h), hl
		ENDIF	//DEF_MOVE_STEP == MS_4b

		; fill the tilemap lookup table by X coordinate

		ld ix, _x_tile_addr_tbl
		ld hl, (map_data)
		ld de, (map_tiles_h)

		and a
		sbc hl, de
		
		ld bc, (map_tiles_w)

.loop1		add hl, de
		ld (ix + 0), l
		ld (ix + 1), h
		
		inc ix
		inc ix

		dec bc

		ld a, c
		or b
		jp nz, .loop1

		; fill the tile graphics lookup table
		
		ld a, (map_tiles_cnt)
		ld b, a

		ld ix, tiles_addr_tbl
		ld de, (map_tiles_gfx)

		xor a

.loop2		ld l, a
		ld h, 0

		dup 5
		add hl,hl
		edup

		add hl, de

		ld (ix + 0), l
		ld (ix + 1), h

		inc a
		
		inc ix
		inc ix

		djnz .loop2		

		IF	DEF_COLOR

		; fill the tile colors lookup table
		
		ld a, (map_tiles_cnt)
		ld b, a

		ld ix, tiles_clr_addr_tbl
		ld de, (map_tiles_clr)

		xor a

.loop3		ld l, a
		ld h, 0

		add hl,hl
		add hl,hl

		add hl, de

		ld (ix + 0), l
		ld (ix + 1), h

		inc a
		
		inc ix
		inc ix

		djnz .loop3

		ENDIF	//DEF_COLOR

		; calc X/Y coordinates by start screen index

		; x_pos = map_scr_blocks_w * ( map_start_scr % map_scr_width )

		ld a, (map_start_scr)
		ld c, a

		ld a, (map_scr_width)
		ld d, a

		call c_div_d

		ld c, a				; a - remainder
		ld a, (map_scr_blocks_w)

		IF TR_DATA_TILES4X4
		srl a
		ENDIF

		ld d, a

		call d_mul_c

		ld b, c
		ld c, a				; bc - product

		IF DEF_MOVE_STEP == MS_8b
		MUL_POW2_BC 1
		ELSE
		IF DEF_MOVE_STEP == MS_4b
		MUL_POW2_BC 2
		ENDIF //DEF_MOVE_STEP == MS_4b
		ENDIF //DEF_MOVE_STEP == MS_8b

		ld (x_pos), bc

		; y_pos = map_scr_blocks_h * ( map_start_scr / map_scr_width )

		ld a, (map_start_scr)
		ld c, a

		ld a, (map_scr_width)
		ld d, a

		call c_div_d			; c - result

		ld a, (map_scr_blocks_h)

		IF TR_DATA_TILES4X4
		srl a
		ENDIF
		
		ld d, a

		call d_mul_c

		ld b, c
		ld c, a				; bc - product

		IF DEF_MOVE_STEP == MS_8b
		MUL_POW2_BC 1
		ELSE
		IF DEF_MOVE_STEP == MS_4b
		MUL_POW2_BC 2
		ENDIF //DEF_MOVE_STEP == MS_4b
		ENDIF //DEF_MOVE_STEP == MS_8b

		ld (y_pos), bc

		ret

		IF	DEF_COLOR & (DEF_MOVE_STEP == MS_8b)
drw_prc_tbl	dw _draw_tiles_column, _draw_tiles_color_column, _draw_tiles_column_shifted_up_8b, _draw_tiles_color_column_shifted_up
		ENDIF	//DEF_COLOR & (DEF_MOVE_STEP == MS_8b)
		IF	!DEF_COLOR && DEF_MOVE_STEP == MS_8b
drw_prc_tbl	dw _draw_tiles_column, _draw_tiles_column_shifted_up_8b
		ENDIF	//!DEF_COLOR & DEF_MOVE_STEP == MS_8b

		IF	DEF_COLOR & (DEF_MOVE_STEP == MS_4b)
drw_prc_tbl	dw _draw_tiles_column, _draw_tiles_color_column, _draw_tiles_column_shifted_up_4b, _draw_tiles_color_column, _draw_tiles_column_shifted_up_8b, _draw_tiles_color_column_shifted_up, _draw_tiles_column_shifted_up_12b, _draw_tiles_color_column_shifted_up
		ENDIF	//DEF_COLOR & (DEF_MOVE_STEP == MS_4b)
		IF	!DEF_COLOR && DEF_MOVE_STEP == MS_4b
drw_prc_tbl	dw _draw_tiles_column, _draw_tiles_column_shifted_up_4b, _draw_tiles_column_shifted_up_8b, _draw_tiles_column_shifted_up_12b
		ENDIF	//!DEF_COLOR & DEF_MOVE_STEP == MS_4b

		IF	DEF_MOVE_STEP == MS_4b
drw_hlf_prc_tbl	dw _draw_half_tiles_column, _draw_half_tiles_column_shifted_up_4b, _draw_half_tiles_column_shifted_up_8b, _draw_half_tiles_column_shifted_up_12b
		ENDIF	//DEF_MOVE_STEP == MS_4b

draw_tiles
		IF	DEF_128K_DBL_BUFFER
		call _switch_active_screen
		ENDIF	//DEF_128K_DBL_BUFFER

		call _fix_x_y
		call _calc_tile_addr		; HL - tilemap start address

		IF	DEF_MOVE_STEP == MS_TILE

		push hl

		jp _draw_tiles_2x2_mode

		ELSE				; MS_8b / MS_4b

		ex de, hl
		ld ixh, scr_buff_tiles_w
		ld hl, #4000
		FIX_SCR_ADDR_HL
		ld a, l
		ld iyl, a
		ld a, h
		ld iyh, a
		ex de, hl

		ld de, (x_pos)

		ld a,e

		IF	DEF_MOVE_STEP == MS_8b
		and #01
		ENDIF	//DEF_MOVE_STEP == MS_8b

		IF	DEF_MOVE_STEP == MS_4b
		and #02
		ENDIF	//DEF_MOVE_STEP == MS_4b

		jp z, .cont1

		dec ixh
		inc iy

		ld bc, (map_tiles_h)
		add hl, bc

.cont1		push hl

		ld hl, (y_pos)

		ld h, 0
		ld a, l

		IF	DEF_MOVE_STEP == MS_8b
		and #01
		ENDIF	//DEF_MOVE_STEP == MS_8b
		IF	DEF_MOVE_STEP == MS_4b
		and #03
		ENDIF	//DEF_MOVE_STEP == MS_4b

		ld l, a
		add hl, hl

		IF	DEF_COLOR
		add hl, hl
		ENDIF	//DEF_COLOR
		
		ld de, drw_prc_tbl
		add hl, de

		ld a, (hl)
		ld (_drw_prc1), a

		IF	DEF_FULLSCREEN
		ld (_drw_prc2), a		
		ENDIF	//DEF_FULLSCREEN

		inc hl
		ld a, (hl)
		ld (_drw_prc1 + 1), a

		IF	DEF_FULLSCREEN
		ld (_drw_prc2 + 1), a	
		ENDIF	//DEF_FULLSCREEN	

		IF	DEF_COLOR
		inc hl
		ld a, (hl)
		ld (_drw_clr_prc), a
		inc hl
		ld a, (hl)
		ld (_drw_clr_prc + 1), a
		ENDIF	//DEF_COLOR
		
		IF	DEF_FULLSCREEN
		ld a, ixh
		ld (_scr_buff_tlw1), a

		ld a, iyl
		ld (_scr_buff_ptr), a

		ld a, iyh
		add #10
		ld (_scr_buff_ptr + 1 ), a
		ENDIF	//DEF_FULLSCREEN

		IF	DEF_COLOR
		ld a, ixh
		ld (_scr_buff_tlw2), a

		ld a, iyl
		ld (_clr_buff_ptr), a

		ld a, iyh
		add #18

		ld (_clr_buff_ptr + 1 ), a
		ENDIF   //DEF_COLOR

		; setup drawing of the rightmost half tiles column
		
		IF	DEF_MOVE_STEP == MS_4b
		ld hl, (x_pos)

		ld a, l
		and #03
		cp #03
		jp nz, .cont2			; if coordinate is not a multiple of 3, skip column drawing

		and #01
		jp z, .cont2			; if coordinate is even, then skip column drawing

		ld hl, (y_pos)

		ld a, l
		and #03

		ld l, a
		add hl, hl

		ld de, drw_hlf_prc_tbl
		add hl, de

		ld a, (hl)
		ld (_drw_hlf_prc1), a

		IF	DEF_FULLSCREEN
		ld (_drw_hlf_prc2), a
		ENDIF	//DEF_FULLSCREEN
		
		inc hl
		ld a, (hl)
		ld (_drw_hlf_prc1 + 1 ), a

		IF	DEF_FULLSCREEN
		ld (_drw_hlf_prc2 + 1 ), a
		ENDIF	//DEF_FULLSCREEN
		
		ENDIF	//DEF_MOVE_STEP == MS_4b
		
.cont2
		; draw the top two thirds of the screen

;;;		ld ixh, scr_buff_tiles_w
		ld hl, iy
		
		exx
		pop bc			; BC - tilemap start address for pos_x|pos_y

		IF	DEF_FULLSCREEN
		push bc			; save to draw the bottom of the screen (B\W)
		ENDIF	//DEF_FULLSCREEN

		IF	DEF_COLOR
		push bc			; save to draw color layer
		ENDIF	//DEF_COLOR

		exx

_drw_loop1	ld ixl, 8		; number of 2x2 tiles in the upper 2 thirds

		exx
		push bc
		exx
		push hl

		db #cd			; call ...
_drw_prc1	dw 0

		pop hl
		exx
		pop bc

		db #21			; ld hl, (map_tiles_h)
_hl_mptls_h_mn	dw 0

		add hl, bc
		ld bc, hl
		exx

		inc l
		inc l		

		dec ixh
		jp nz, _drw_loop1

		IF	DEF_FULLSCREEN

		; draw the bottom third of the screen

		db #dd, #26		; ld ixh, ...
_scr_buff_tlw1	db 0

		db #21			; ld hl, ...
_scr_buff_ptr	db 0, 0
		
		exx
		pop hl			; BC - tilemap start address for pos_x|pos_y
		ld de, 8
		add hl, de
		ld c, l
		ld b, h
		exx

_drw_loop2	ld ixl, 4		; number of 2x2 tiles in the bottom third of the screen

		exx
		push bc
		exx
		push hl

		db #cd                  ; call ...
_drw_prc2	dw 0

		pop hl
		exx
		pop bc

		db #21			; ld hl, (map_tiles_h)
_hl_mptls_h_fs	dw 0

		add hl, bc
		ld bc, hl
		exx

		inc l
		inc l		

		dec ixh
		jp nz, _drw_loop2

		ENDIF	//DEF_FULLSCREEN

		IF	DEF_COLOR

		; draw color

		db #dd, #26		; ld ixh, ...
_scr_buff_tlw2	db 0

		db #21                  ; ld hl, ...
_clr_buff_ptr	db 0, 0			
		
		exx
		pop bc
		exx

_drw_clr_loop
		IF	DEF_FULLSCREEN
		ld ixl, scr_buff_tiles_h
		ELSE
		ld ixl, 8		; for 2\3 of the screen
		ENDIF	//DEF_FULLSCREEN

		exx
		push bc
		exx
		push hl

		db #cd			; call ...
_drw_clr_prc	dw 0

		pop hl
		exx
		pop bc

		db #21			; ld hl, (map_tiles_h)
_hl_mptls_h_cl	dw 0

		add hl, bc
		ld bc, hl
		exx

		inc l
		inc l		

		dec ixh
		jp nz, _drw_clr_loop

		ENDIF	//DEF_COLOR

		call _draw_border

		IF	DEF_MOVE_STEP == MS_4b
		ld de, (x_pos)

		ld a, e
		and #01
		ret z
		
		ld a, e
		and #03
		cp #03
		jp nz, _skip_hlf_tile_drw

		; draw one CHR wide strip on the right side

		ld hl, #4000 + #20 - 1
		FIX_SCR_ADDR_HL
		ld ixl, 8

		IF	DEF_FULLSCREEN
		exx

		IF	!DEF_COLOR
		ld hl, bc
		ld de, 8
		and a
		sbc hl, de
		ld bc, hl
		ENDIF	//!DEF_COLOR

		push bc
		exx
		ENDIF	//DEF_FULLSCREEN

		db #cd			; call _draw_half_tiles_column etc...
_drw_hlf_prc1	dw 0

		IF	DEF_FULLSCREEN

		ld hl, #4000 + #1000 + #20 - 1
		FIX_SCR_ADDR_HL
		ld ixl, 4

		exx
		pop bc			; BC - tilemap start address for pos_x|pos_y
		ld hl, 8
		add hl, bc
		ld c, l
		ld b, h
		exx

		db #cd			; call _draw_half_tiles_column etc...
_drw_hlf_prc2	dw 0

		ENDIF	//DEF_FULLSCREEN  		
		
_skip_hlf_tile_drw

		IF	DEF_FULLSCREEN
		ld hl, #4000 + #1800 - 1
		ld b, 192
		ELSE
		ld hl, #4000 + #1000 - 1
		ld b, 128
		ENDIF //DEF_FULLSCREEN

		FIX_SCR_ADDR_HL
.shift_loop1
		xor a

                dup 32

		rld
		dec hl

		edup
		
		djnz .shift_loop1	

		ENDIF	//DEF_MOVE_STEP == MS_4b

		ret

		; draw two white lines (left/right) to hide drawing artefacts
_draw_border
		ld hl, #5800
		FIX_SCR_ADDR_HL

		IF	DEF_FULLSCREEN
		ld b, 24
		ELSE
		ld b, 16
		ENDIF	//DEF_FULLSCREEN

		ld de, 31
		ld a, 63
.brd_loop
		ld (hl), a

		add hl, de
		ld (hl), a

		inc hl

		djnz .brd_loop

		ret

		ENDIF	//DEF_MOVE_STEP == MS_TILE

		IF	DEF_COLOR

		; in: 	HL - screen addr
		;	IXL - number of tiles
		;	EX BC - tiles map addr
		; TR_DATA_TILES4X4
		;	EX DE - tiles 4x4 data addr

		IF TR_DATA_TILES4X4

_draw_tiles_color_column

		ld (_temp_sp), sp

.loop		exx
		ld a, (bc)		; get tile index
		inc bc

		ld l, a
		ld h, 0
		add hl, hl
		add hl, hl
		add hl, de		; hl = tiles4x4[ tile_ind * 4 ]

		ld sp, hl		;6
		pop hl			;10
		pop iy			;14
		ld a, h			;4
		ld (._2nd_block_val), a	;13
		
		; put 1st clr block
		ld a, l
		ADD_ADDR_AX2 tiles_clr_addr_tbl

		ld sp, hl
		pop hl			; get tile color data address from the table
		ld sp, hl
		exx
		SCR_PUT_BLOCK2X2_CLR

		SUB_HL_VAL SCR_CHR_WIDTH - 1

		; put 2nd clr block
		exx
		db #3e			;ld a, N
._2nd_block_val	db 0			;7
		ADD_ADDR_AX2 tiles_clr_addr_tbl

		ld sp, hl
		pop hl			; get tile color data address from the table
		ld sp, hl
		exx
		SCR_PUT_BLOCK2X2_CLR

		ADD_HL_VAL SCR_CHR_WIDTH - 1

		; put 4th clr block
		exx
		ld a, hy
		ADD_ADDR_AX2 tiles_clr_addr_tbl

		ld sp, hl
		pop hl			; get tile color data address from the table
		ld sp, hl
		exx
		SCR_PUT_BLOCK2X2_CLR

		SUB_HL_VAL SCR_CHR_WIDTH + 3

		; put 3rd clr block
		exx
		ld a, ly
		ADD_ADDR_AX2 tiles_clr_addr_tbl

		ld sp, hl
		pop hl			; get tile color data address from the table
		ld sp, hl
		exx
		SCR_PUT_BLOCK2X2_CLR
		
		ADD_HL_VAL SCR_CHR_WIDTH - 1

		dec ixl
		jp nz, .loop		

		ld sp, (_temp_sp)

		ret

		ELSE //TR_DATA_TILES4X4

_draw_tiles_color_column
	
		ld (_temp_sp), sp

.loop		exx
		ld a, (bc)		; get tile index
		inc bc
		
		ADD_ADDR_AX2 tiles_clr_addr_tbl

		ld sp, hl
		pop hl			; get tile color data address from the table
		
		ld sp, hl
		exx

		SCR_PUT_BLOCK2X2_CLR

		ADD_HL_VAL SCR_CHR_WIDTH - 1

		dec ixl
		jp nz, .loop		

		ld sp, (_temp_sp)

		ret

		ENDIF //TR_DATA_TILES4X4

_draw_tiles_color_column_shifted_up
		; in: 	HL - screen addr
		;	IXL - number of tiles
		;	EX BC - tiles map addr

		ld (_temp_sp), sp

		ld bc, 31
		
		exx
		ld a, (bc)		; get tile index
		inc bc
		
		ADD_ADDR_AX2 tiles_clr_addr_tbl

		ld sp, hl
		pop hl			; get tile color data address from the table

		inc hl			; we need the lower half of tile
		inc hl			; so we skip the first two colors
		
		ld sp, hl
		exx

.loop		pop de

		ld (hl), e
		inc l
		ld (hl), d

		add hl, bc

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		ADD_ADDR_AX2 tiles_clr_addr_tbl

		ld sp, hl
		pop hl			; get tile color data address from the table
		
		ld sp, hl
		exx

		pop de

		ld (hl), e
		inc l
		ld (hl), d

		add hl, bc

		dec ixl
		jp nz, .loop		

		ld sp, (_temp_sp)

		ret

		ENDIF	//DEF_COLOR

		; in: 	HL - screen addr
		;	IXL - number of tiles
		;	EX BC - tiles map addr
		; TR_DATA_TILES4X4
		;	EX DE - tiles 4x4 data addr

		IF TR_DATA_TILES4X4

_draw_tiles_column

		ld (_temp_sp), sp

.loop		exx
		ld a, (bc)		; get tile index
		inc bc

		ld l, a
		ld h, 0
		add hl, hl
		add hl, hl
		add hl, de		; hl = tiles4x4[ tile_ind * 4 ]

		ld sp, hl		;6
		pop hl			;10
		pop iy			;14
		ld a, h			;4
		ld (._2nd_block_val), a	;13

		; draw 1st block
		ld a, l
		ADD_ADDR_AX2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table
		ld sp, hl
		exx
		SCR_PUT_BLOCK2X2

		inc c
		inc c
		ld l, c
		ld h, b

		; draw 2nd block
		exx
		db #3e			;ld a, N
._2nd_block_val	db 0			;7
		ADD_ADDR_AX2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table
		ld sp, hl
		exx
		SCR_PUT_BLOCK2X2

		ld l, c
		ld h, b
		ADD_HL_VAL 64

		; draw 4th block
		exx
		ld a, hy
		ADD_ADDR_AX2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table
		ld sp, hl
		exx
		SCR_PUT_BLOCK2X2

		dec c
		dec c
		ld l, c
		ld h, b

		; draw 3th block
		exx
		ld a, ly
		ADD_ADDR_AX2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table
		ld sp, hl
		exx
		SCR_PUT_BLOCK2X2

		ld l, c
		ld h, b

		ADD_HL_VAL 64
		CHECK_NEXT_THIRD_CHR_LINE_HL

		dec ixl
		jp nz, .loop		

		ld sp, (_temp_sp)
		ret

		ELSE //TR_DATA_TILES4X4

_draw_tiles_column

		ld (_temp_sp), sp

.loop		exx
		ld a, (bc)		; get tile index
		inc bc

		ADD_ADDR_AX2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table
		
		ld sp, hl
		exx

		SCR_PUT_BLOCK2X2

		CHECK_NEXT_THIRD_LINE_HL

		dec ixl
		jp nz, .loop		

		ld sp, (_temp_sp)
		ret

		ENDIF //TR_DATA_TILES4X4

_draw_tiles_column_shifted_up_8b
		; in: 	HL - screen addr
		;	IXL - number of tiles
		;	EX BC - tiles map addr

		ld (_temp_sp), sp

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		ADD_ADDR_AX2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table

		ld de, 16
		add hl, de
		
		ld sp, hl
		exx
.loop
		ld c, l		;4 save hl
		ld b, h		;4

		SCR_PUT_BLOCK2XN 4

		; next CHR line
		ld a, c		;4
		add #20		;7
		ld l, a		;4
		ld h, b		;4 = 27

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		ADD_ADDR_AX2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table
		
		ld sp, hl
		exx

		SCR_PUT_BLOCK2XN 4

		CHECK_NEXT_THIRD_LINE_HL

		dec ixl
		jp nz, .loop		

		ld sp, (_temp_sp)
		ret

		IF	DEF_MOVE_STEP == MS_4b

_draw_half_tiles_column
		; in: 	HL - screen addr
		;	IXL - number of tiles
		;	EX BC - tiles map addr

		ld (_temp_sp), sp

.loop		exx
		ld a, (bc)		; get tile index
		inc bc
		
		ADD_ADDR_AX2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table
		
		ld sp, hl
		exx

		ld c, l		;4 save hl
		ld b, h		;4

		SCR_PUT_HALF_BLOCK2XN 4

		; next CHR line
		ld a, c		;4
		add #20		;7
		ld l, a		;4
		ld h, b		;4 = 27

		SCR_PUT_HALF_BLOCK2XN 4

		CHECK_NEXT_THIRD_LINE_HL

		dec ixl
		jp nz, .loop		

		ld sp, (_temp_sp)
		ret

_draw_half_tiles_column_shifted_up_4b
		; in: 	HL - screen addr
		;	IXL - number of tiles
		;	EX BC - tiles map addr

		ld (_temp_sp), sp

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		ADD_ADDR_AX2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table

		ld de, 8
		add hl, de
		
		ld sp, hl
		exx
.loop
		ld c, l		;4 save hl
		ld b, h		;4

		SCR_PUT_HALF_BLOCK2XN 4

		; next CHR line
		ld a, c		;4
		add #20		;7
		ld l, a		;4
		ld h, b		;4 = 27

		SCR_PUT_HALF_BLOCK2XN 2

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		ADD_ADDR_AX2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table
		
		ld sp, hl
		exx

		SCR_PUT_HALF_BLOCK2XN 2

		CHECK_NEXT_THIRD_LINE_HL

		dec ixl
		jp nz, .loop		

		ld sp, (_temp_sp)
		ret

_draw_half_tiles_column_shifted_up_8b
		; in: 	HL - screen addr
		;	IXL - number of tiles
		;	EX BC - tiles map addr

		ld (_temp_sp), sp

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		ADD_ADDR_AX2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table

		ld de, 16
		add hl, de
		
		ld sp, hl
		exx
.loop
		ld c, l		;4 save hl
		ld b, h		;4

		SCR_PUT_HALF_BLOCK2XN 4

		; next CHR line
		ld a, c		;4
		add #20		;7
		ld l, a		;4
		ld h, b		;4 = 27

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		ADD_ADDR_AX2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table
		
		ld sp, hl
		exx

		SCR_PUT_HALF_BLOCK2XN 4

		CHECK_NEXT_THIRD_LINE_HL

		dec ixl
		jp nz, .loop		

		ld sp, (_temp_sp)
		ret

_draw_half_tiles_column_shifted_up_12b
		; in: 	HL - screen addr
		;	IXL - number of tiles
		;	EX BC - tiles map addr

		ld (_temp_sp), sp

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		ADD_ADDR_AX2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table

		ld de, 24
		add hl, de
		
		ld sp, hl
		exx
.loop
		ld c, l		;4 save hl
		ld b, h		;4

		SCR_PUT_HALF_BLOCK2XN 2

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		ADD_ADDR_AX2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table
		
		ld sp, hl
		exx

		SCR_PUT_HALF_BLOCK2XN 2

		; next CHR line
		ld a, c		;4
		add #20		;7
		ld l, a		;4
		ld h, b		;4 = 27

		SCR_PUT_HALF_BLOCK2XN 4

		CHECK_NEXT_THIRD_LINE_HL

		dec ixl
		jp nz, .loop		

		ld sp, (_temp_sp)
		ret

_draw_tiles_column_shifted_up_4b
		; in: 	HL - screen addr
		;	IXL - number of tiles
		;	EX BC - tiles map addr

		ld (_temp_sp), sp

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		ADD_ADDR_AX2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table

		ld de, 8
		add hl, de
		
		ld sp, hl
		exx
.loop
		ld c, l		;4 save hl
		ld b, h		;4

		SCR_PUT_BLOCK2XN 4

		; next CHR line
		ld a, c		;4
		add #20		;7
		ld l, a		;4
		ld h, b		;4 = 27

		SCR_PUT_BLOCK2XN 2

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		ADD_ADDR_AX2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table
		
		ld sp, hl
		exx

		SCR_PUT_BLOCK2XN 2

		CHECK_NEXT_THIRD_LINE_HL

		dec ixl
		jp nz, .loop		

		ld sp, (_temp_sp)
		ret

_draw_tiles_column_shifted_up_12b
		; in: 	HL - screen addr
		;	IXL - number of tiles
		;	EX BC - tiles map addr

		ld (_temp_sp), sp

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		ADD_ADDR_AX2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table

		ld de, 24
		add hl, de
		
		ld sp, hl
		exx
.loop
		ld c, l		;4 save hl
		ld b, h		;4

		SCR_PUT_BLOCK2XN 2

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		ADD_ADDR_AX2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table
		
		ld sp, hl
		exx

		SCR_PUT_BLOCK2XN 2

		; next CHR line
		ld a, c		;4
		add #20		;7
		ld l, a		;4
		ld h, b		;4 = 27

		SCR_PUT_BLOCK2XN 4

		CHECK_NEXT_THIRD_LINE_HL

		dec ixl
		jp nz, .loop		

		ld sp, (_temp_sp)
		ret

		ENDIF	//DEF_MOVE_STEP == MS_4b
		
		IF	DEF_MOVE_STEP == MS_TILE

_draw_tiles_2x2_mode

		; draw the top two thirds of the screen

		ld ixh, scr_buff_tiles_w

		ld hl, #4000
		FIX_SCR_ADDR_HL
		
		exx
		pop bc			; BC - tilemap start address for pos_x|pos_y

		IF	DEF_FULLSCREEN
		push bc			; save to draw the bottom of the screen (B\W)
		ENDIF	//DEF_FULLSCREEN

		IF	DEF_COLOR
		push bc			; save to draw color layer
		ENDIF	//DEF_COLOR

		IF TR_DATA_TILES4X4
		ld hl, map_tiles4x4
		ld e, (hl)
		inc hl
		ld d, (hl)
		exx
_drw_loop1	ld ixl, 4		; number of 4x4 tiles in the upper 2 thirds
		ELSE //TR_DATA_TILES4X4
		exx
_drw_loop1	ld ixl, 8		; number of 2x2 tiles in the upper 2 thirds
		ENDIF //TR_DATA_TILES4X4

		exx
		push bc
		exx
		push hl

		call _draw_tiles_column

		pop hl
		exx
		pop bc

		db #21			; ld hl, (map_tiles_h)
_hl_mptls_h_mn	dw 0

		add hl, bc
		ld bc, hl
		exx

		IF TR_DATA_TILES4X4
		ld a, l
		add 4
		ld l, a
		ELSE //TR_DATA_TILES4X4
		inc l
		inc l		
		ENDIF //TR_DATA_TILES4X4

		dec ixh
		jp nz, _drw_loop1

		IF	DEF_FULLSCREEN

		; draw the bottom third of the screen

		ld ixh, scr_buff_tiles_w

		ld hl, #5000
		FIX_SCR_ADDR_HL
		
		exx
		pop hl			; BC - tilemap start address for pos_x|pos_y
		IF TR_DATA_TILES4X4
		ld de, 4
		ELSE //TR_DATA_TILES4X4
		ld de, 8
		ENDIF //TR_DATA_TILES4X4
		add hl, de
		ld c, l
		ld b, h

		IF TR_DATA_TILES4X4
		ld hl, map_tiles4x4
		ld e, (hl)
		inc hl
		ld d, (hl)
		exx
_drw_loop2	ld ixl, 2		; number of 4x4 tiles in the lower third
		ELSE //TR_DATA_TILES4X4
		exx
_drw_loop2	ld ixl, 4		; number of 2x2 tiles in the lower third
		ENDIF //TR_DATA_TILES4X4

		exx
		push bc
		exx
		push hl

		call _draw_tiles_column

		pop hl
		exx
		pop bc

		db #21			; ld hl, (map_tiles_h)
_hl_mptls_h_fs	dw 0

		add hl, bc
		ld bc, hl
		exx

		IF TR_DATA_TILES4X4
		ld a, l
		add 4
		ld l, a
		ELSE //TR_DATA_TILES4X4
		inc l
		inc l		
		ENDIF //TR_DATA_TILES4X4

		dec ixh
		jp nz, _drw_loop2

		ENDIF	//DEF_FULLSCREEN

		IF	DEF_COLOR

		; draw color

		ld ixh, scr_buff_tiles_w

		ld hl, #5800
		FIX_SCR_ADDR_HL
		
		exx
		pop bc

		IF TR_DATA_TILES4X4
		ld hl, map_tiles4x4
		ld e, (hl)
		inc hl
		ld d, (hl)
		ENDIF //TR_DATA_TILES4X4

		exx
_drw_loop_clr		
		IF	DEF_FULLSCREEN
		ld ixl, scr_buff_tiles_h
		ELSE
		IF TR_DATA_TILES4X4
		ld ixl, 4		; for 2/3 of the screen
		ELSE
		ld ixl, 8		; for 2/3 of the screen
		ENDIF //TR_DATA_TILES4X4
		ENDIF	//DEF_FULLSCREEN

		exx
		push bc
		exx
		push hl

		call _draw_tiles_color_column

		pop hl
		exx
		pop bc

		db #21			; ld hl, (map_tiles_h)
_hl_mptls_h_cl	dw 0

		add hl, bc
		ld bc, hl
		exx

		IF TR_DATA_TILES4X4
		ld a, l
		add 4
		ld l, a
		ELSE //TR_DATA_TILES4X4
		inc l
		inc l		
		ENDIF //TR_DATA_TILES4X4

		dec ixh
		jp nz, _drw_loop_clr

		ENDIF	//DEF_COLOR

		ret

		ENDIF	//DEF_MOVE_STEP == MS_TILE

_fix_x_y	
		; check coordinates so that the entire screen of tiles is always drawn

		ld hl, (x_pos)

		bit 7, h
		jp z, .cont1

		ld hl, 0
		jp __save_x

.cont1		
		db #11			; ld de, scr_steps_w
_de_scr_st_w	dw 0

		add hl, de

		db #11			; ld de, map_steps_w
_de_map_st_w	dw 0

		and a			; reset the carry flag, just in case...
		sbc hl, de  		; if ( x_pos + scr_w ) < map_chrs_w, then continue

		jp c, __cont2

		ld de, (x_pos)
		ex de, hl
		and a
		sbc hl, de		; subtract the overflow from x_pos, thereby adjusting the coordinate to desired value
				
__save_x	ld (x_pos), hl		
		
__cont2		ld hl, (y_pos)

		bit 7, h
		jp z, .cont3

		ld hl, 0
		jp __save_y
		
.cont3		
		db #11			; ld de, scr_steps_h
_de_scr_st_h	dw 0

		add hl, de

		db #11			; ld de, map_steps_h
_de_map_st_h	dw 0

		and a			; reset the carry flag, just in case...
		sbc hl, de

		ret c

		ld de, (y_pos)
		ex de, hl
		and a
		sbc hl, de		; subtract the overflow from y_pos, thereby adjusting the coordinate to desired value

__save_y	ld (y_pos), hl

.exit
		ret

_calc_tile_addr	
		; in: x_pos/y_pos
		; out: HL - tile address

		ld de, _x_tile_addr_tbl
		ld hl, (x_pos)

		IF	DEF_MOVE_STEP == MS_4b
		res 0, l
		
		sra h
		rr l
		sra h
		rr l
		ENDIF	//DEF_MOVE_STEP == MS_4b

		IF	DEF_MOVE_STEP == MS_8b
		res 0, l		; reset zero bit to consider coordinates multiple of tile size only

		sra h			; divide Y coordinate by 2 so that each even coordinate corresponds to an ordinal number of a tile in tiles column
		rr l
		ENDIF	//DEF_MOVE_STEP == MS_8b

		add hl, hl		; multiply x_pos by 2 to get an offset in 16-bit lookup table

		add hl, de

		ld e, (hl)
		inc hl
		ld d, (hl)

		ld hl, (y_pos)

		IF	DEF_MOVE_STEP == MS_4b
		res 0, l

		sra h
		rr l
		sra h
		rr l
		ENDIF	//DEF_MOVE_STEP == MS_4b

		IF	DEF_MOVE_STEP == MS_8b
		res 0, l		; reset zero bit to consider coordinates multiple of tile size only

		sra h			; divide the Y coordinate by 2 so that each even coordinate corresponds to the ordinal number of the tile in the tile column
		rr l
		ENDIF	//DEF_MOVE_STEP == MS_8b

		add hl, de

		ret

show_screen	
		ret

_temp_sp	dw 0				; the stack pointer temporary variable

		IF	DEF_128K_DBL_BUFFER

_switch_active_screen

		ld a, (_scr_trigg)
		xor #01
		ld (_scr_trigg), a

		and a

		jp z, .draw_scr0

		VSYNC

		; show the main screen and draw on the extended one

		jp _switch_Uscr

.draw_scr0

		VSYNC

		; show the extended screen and draw on the main one

		jp _switch_Escr

		ENDIF	//DEF_128K_DBL_BUFFER
CODE_END

		DISPLAY "Generated CODE size: ", /D, CODE_END - CODE_START, " org: ", /H, CODE_START, " (", /D, CODE_START, ")"

		ENDMODULE
