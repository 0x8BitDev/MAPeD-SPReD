;###################################################################
;
; Copyright 2018-2019 0x8BitDev ( MIT license )
;
;###################################################################
;
; DESC: Multidirectional scroller example.
;

; Public procs:
;
; tilemap_render.init		- render init
; tilemap_render.draw_tiles  	- draw tiles to shadow buffer
; tilemap_render.show_screen 	- draw shadow buffer to screen
;

		MODULE	tilemap_render

		assert ( MAP_DATA_MAGIC&MAP_FLAG_RLE ) == 0, The sample doesn't support compressed data!
		assert ( MAP_DATA_MAGIC&MAP_FLAG_DIR_ROWS ) == 0, The sample doesn't support rows ordered data!

TR_DATA_TILES4X4 equ MAP_DATA_MAGIC&MAP_FLAG_TILES4X4

		IF TR_DATA_TILES4X4
		assert ( DEF_MOVE_STEP == MS_TILE ), The tiles 4x4 mode is only supported when the MS_TILE step mode is active! Please, change the sample settings!
		ENDIF

; level drawing data

; be sure to define in the main code!
;
; max_lev_tiles_w	equ	Lev0_wtls	; max width of a level in tiles
;

; this data must be filled in for each map in the main code
;
map_data	dw 0			; Lev0_map	game level tile map address
map_tiles_gfx	dw 0			; Lev0_tlg 	tile graphics data
map_tiles_clr	dw 0			; Lev0_tlc 	tile colors data
map_data_cnt	dw 0 			; Lev0_t_tiles 	number of tiles in map
map_tiles_cnt	db 0			; Lev0_u_tiles 	number of unique tiles in map

		IF TR_DATA_TILES4X4
map_tiles4x4	dw 0			; Lev0_tli 	tiles 4x4 data
		ENDIF //TR_DATA_TILES4X4

map_tiles_w	dw 0			; Lev0_wtls number of tiles in map in width
map_tiles_h	dw 0			; Lev0_htls number of tiles in map in height
map_chrs_w	dw 0 			; Lev0_wchr number of CHRs in map in width
map_chrs_h	dw 0 			; Lev0_hchr number of CHRs in map in height

; start screen coordinates (upper left corner) in map coordinate system

x_pos		dw 0
y_pos		dw 0

; shadow buffer parameters and visible screen area

scr_w		equ 32	; horizontal screen size in CHRs
		
		IF	DEF_FULLSCREEN
scr_h		equ 24	; vertical screen size in CHRs
		ELSE
scr_h		equ 16	; vertical screen size in CHRs
		ENDIF	//DEF_FULLSCREEN

		IF TR_DATA_TILES4X4
scr_buff_tiles_w	equ scr_w >> 2	; the shadow buffer width in tiles
scr_buff_tiles_h	equ scr_h >> 2	; the shadow buffer height in tiles
		ELSE
scr_buff_tiles_w	equ scr_w >> 1	; the shadow buffer width in tiles
scr_buff_tiles_h	equ scr_h >> 1	; the shadow buffer height in tiles
		ENDIF //TR_DATA_TILES4X4

_x_tile_addr_tbl	block ( max_lev_tiles_w << 1 ), 0	; address table for X coordinate

			align 256
tiles_addr_tbl		block 512,0	; tile graphics address table
tiles_clr_addr_tbl	block 512,0	; tile colors(attrbutes) address table

		; add 'A *= 2' to an input address
		; OUT: hl

	macro	add_addr_ax2 _addr
;		ld h, high _addr	<-- optimized version when a tile index is premultiplied by 2
;		ld l, a			<-- (a) is premultiplied by 2 (max 128 tiles)

		ld h, high _addr
		add a, a
		jp nc, .skip
		inc h 
.skip		
		ld l, a
	endm

	macro	add_hl_a
		add a, l
		jp nc, .skip
		inc h
.skip
		ld l, a
	endm

	macro	scr_buff_put_block2x2
		dup 8			;<---
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

	macro	scr_buff_put_block2x2_clr
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

DATA_HOLE_START

		; the valid scr_buff addresses are #8000 or #C000

		align 8192

DATA_HOLE_END

BACK_BUFFER_START

; B/W and color buffers must go one after the other
;
scr_buff	block 4096, 0		; graphics buffer of the first 2 thirds

		IF DEF_FULLSCREEN
		block 4096, 0		; graphics buffer of the lower third
		ENDIF	//DEF_FULLSCREEN

		IF	DEF_COLOR
		IF	DEF_FULLSCREEN
clr_buff	block 768, 0		; color buffer
		ELSE
clr_buff	block 512, 0		; color buffer
		ENDIF	//DEF_FULLSCREEN
		ENDIF	//DEF_COLOR

scr_buff_next_tile_row_offs = -4064

BACK_BUFFER_END

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
		di
		
		call _fix_x_y
		call _calc_tile_addr		; HL - tilemap start address

		IF	DEF_MOVE_STEP == MS_TILE

		push hl

		jp _draw_tiles_2x2_mode

		ELSE				; MS_8b / MS_4b

		ld ixh, scr_buff_tiles_w
		ld iy, scr_buff

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
		add a, #10
		ld (_scr_buff_ptr + 1 ), a
		ENDIF	//DEF_FULLSCREEN

		IF	DEF_COLOR
		ld a, ixh
		ld (_scr_buff_tlw2), a

		ld a, iyl
		ld (_clr_buff_ptr), a

		ld a, iyh

		IF	DEF_FULLSCREEN
		add a, #20
		ELSE
		add a, #10
		ENDIF	//DEF_FULLSCREEN

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

		ld hl, scr_buff + #20 - 1
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

		ld hl, scr_buff + #1000 + #20 - 1
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

		ld hl, scr_buff + #1000 - 1
		ld b, 128
.shift_loop1
		xor a

                dup 32

		rld
		dec hl

		edup
		
		djnz .shift_loop1	

		IF	DEF_FULLSCREEN
		ld hl, scr_buff + #1000 + #20 - 1
		ld de, scr_buff_next_tile_row_offs
		ld b, 4
.shift_loop2
		ld c, 16

.shift_loop3
		xor a

                dup 32

		rld
		dec hl

		edup

		inc h

		ld a, l		; <--
		add a, #20
		ld l, a

		ld a, 0
		adc a, h
		ld h, a		; 30t

		dec c
		jp nz, .shift_loop3

		add hl, de
		
		djnz .shift_loop2
		ENDIF	//DEF_FULLSCREEN

		ENDIF	//DEF_MOVE_STEP == MS_4b

		ret

		ENDIF	//DEF_MOVE_STEP == MS_TILE

		IF	DEF_COLOR

		; in: 	HL - shadow buffer addr
		;	IXL - number of tiles
		;	EX BC - tiles map addr
		; TR_DATA_TILES4X4
		;	EX DE - tiles 4x4 data addr

		IF TR_DATA_TILES4X4

tile4x4_block1_clr	db 0
tile4x4_block2_clr	db 0
tile4x4_block3_clr	db 0
tile4x4_block4_clr	db 0
tile4x4_clr_data	db 0

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

		ld sp, hl
		pop hl
		pop af
		ld sp, tile4x4_clr_data
		push af
		push hl
		
		; put 1st clr block
		ld a, l
		add_addr_ax2 tiles_clr_addr_tbl

		ld sp, hl
		pop hl			; get tile color data address from the table
		ld sp, hl
		exx
		scr_buff_put_block2x2_clr
		ld bc, -31
		add hl, bc

		; put 2nd clr block
		exx
		ld a, (tile4x4_block2_clr)
		add_addr_ax2 tiles_clr_addr_tbl

		ld sp, hl
		pop hl			; get tile color data address from the table
		ld sp, hl
		exx
		scr_buff_put_block2x2_clr
		ld a, 31
		add_hl_a

		; put 4th clr block
		exx
		ld a, (tile4x4_block4_clr)
		add_addr_ax2 tiles_clr_addr_tbl

		ld sp, hl
		pop hl			; get tile color data address from the table
		ld sp, hl
		exx
		scr_buff_put_block2x2_clr
		ld bc, -35
		add hl, bc

		; put 3nd clr block
		exx
		ld a, (tile4x4_block3_clr)
		add_addr_ax2 tiles_clr_addr_tbl

		ld sp, hl
		pop hl			; get tile color data address from the table
		ld sp, hl
		exx
		scr_buff_put_block2x2_clr
		ld a, 31
		add_hl_a

		dec ixl
		jp nz, .loop		

		ld sp, (_temp_sp)

		ret

		ELSE //TR_DATA_TILES4X4

_draw_tiles_color_column

		ld (_temp_sp), sp

		ld bc, 31
		
.loop		exx
		ld a, (bc)		; get tile index
		inc bc
		
		add_addr_ax2 tiles_clr_addr_tbl

		ld sp, hl
		pop hl			; get tile color data address from the table
		
		ld sp, hl
		exx

		scr_buff_put_block2x2_clr

		add hl, bc

		dec ixl
		jp nz, .loop		

		ld sp, (_temp_sp)

		ret

		ENDIF //TR_DATA_TILES4X4

_draw_tiles_color_column_shifted_up
		; in: 	HL - shadow buffer addr
		;	IXL - number of tiles
		;	EX BC - tiles map addr

		ld (_temp_sp), sp

		ld bc, 31
		
		exx
		ld a, (bc)		; get tile index
		inc bc
		
		add_addr_ax2 tiles_clr_addr_tbl

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
		
		add_addr_ax2 tiles_clr_addr_tbl

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

		; in: 	HL - shadow buffer addr
		;	IXL - number of tiles
		;	EX BC - tiles map addr
		; TR_DATA_TILES4X4
		;	EX DE - tiles 4x4 data addr

		IF TR_DATA_TILES4X4

tile4x4_block1	db 0
tile4x4_block2	db 0
tile4x4_block3	db 0
tile4x4_block4	db 0
tile4x4_data	db 0

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

		ld sp, hl
		pop hl
		pop af
		ld sp, tile4x4_data
		push af
		push hl

		; draw 1st block
		ld a, l
		add_addr_ax2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table
		ld sp, hl
		exx
		scr_buff_put_block2x2
		ld bc, -4094
		add hl, bc

		; draw 2nd block
		exx
		ld a, (tile4x4_block2)
		add_addr_ax2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table
		ld sp, hl
		exx
		scr_buff_put_block2x2
		ld bc, -4064
		add hl, bc

		; draw 4th block
		exx
		ld a, (tile4x4_block4)
		add_addr_ax2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table
		ld sp, hl
		exx
		scr_buff_put_block2x2
		ld bc, -4098
		add hl, bc

		; draw 3th block
		exx
		ld a, (tile4x4_block3)
		add_addr_ax2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table
		ld sp, hl
		exx
		scr_buff_put_block2x2
		ld bc, -4064
		add hl, bc

		dec ixl
		jp nz, .loop		

		ld sp, (_temp_sp)
		ret

		ELSE //TR_DATA_TILES4X4

_draw_tiles_column

		ld (_temp_sp), sp

		ld bc, scr_buff_next_tile_row_offs

.loop		exx
		ld a, (bc)		; get tile index
		inc bc

		add_addr_ax2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table
		
		ld sp, hl
		exx

		scr_buff_put_block2x2

		add hl, bc

		dec ixl
		jp nz, .loop		

		ld sp, (_temp_sp)
		ret

		ENDIF //TR_DATA_TILES4X4

_draw_tiles_column_shifted_up_8b
		; in: 	HL - shadow buffer addr
		;	IXL - number of tiles
		;	EX BC - tiles map addr

		ld (_temp_sp), sp

		ld bc, scr_buff_next_tile_row_offs

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		add_addr_ax2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table

		ld de, 16
		add hl, de
		
		ld sp, hl
		exx
.loop
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

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		add_addr_ax2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table
		
		ld sp, hl
		exx

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

		add hl, bc

		dec ixl
		jp nz, .loop		

		ld sp, (_temp_sp)
		ret

		IF	DEF_MOVE_STEP == MS_4b

_draw_half_tiles_column
		; in: 	HL - shadow buffer addr
		;	IXL - number of tiles
		;	EX BC - tiles map addr

		ld (_temp_sp), sp

		ld bc, scr_buff_next_tile_row_offs

.loop		exx
		ld a, (bc)		; get tile index
		inc bc
		
		add_addr_ax2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table
		
		ld sp, hl
		exx

		dup 8			;<---
		pop de

		ld (hl), e
		inc h

		pop de

		ld (hl), e
		inc h		

		edup			;<---

		add hl, bc

		dec ixl
		jp nz, .loop		

		ld sp, (_temp_sp)
		ret

_draw_half_tiles_column_shifted_up_4b
		; in: 	HL - shadow buffer addr
		;	IXL - number of tiles
		;	EX BC - tiles map addr

		ld (_temp_sp), sp

		ld bc, scr_buff_next_tile_row_offs

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		add_addr_ax2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table

		ld de, 8
		add hl, de
		
		ld sp, hl
		exx
.loop
		dup 6			;<---
		pop de

		ld (hl), e
		inc h

		pop de

		ld (hl), e
		inc h		

		edup			;<---

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		add_addr_ax2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table
		
		ld sp, hl
		exx

		dup 2			;<---
		pop de

		ld (hl), e
		inc h

		pop de

		ld (hl), e
		inc h		

		edup			;<---

		add hl, bc

		dec ixl
		jp nz, .loop		

		ld sp, (_temp_sp)
		ret

_draw_half_tiles_column_shifted_up_8b
		; in: 	HL - shadow buffer addr
		;	IXL - number of tiles
		;	EX BC - tiles map addr

		ld (_temp_sp), sp

		ld bc, scr_buff_next_tile_row_offs

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		add_addr_ax2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table

		ld de, 16
		add hl, de
		
		ld sp, hl
		exx
.loop
		dup 4			;<---
		pop de

		ld (hl), e
		inc h

		pop de

		ld (hl), e
		inc h		

		edup			;<---

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		add_addr_ax2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table
		
		ld sp, hl
		exx

		dup 4			;<---
		pop de

		ld (hl), e
		inc h

		pop de

		ld (hl), e
		inc h		

		edup			;<---

		add hl, bc

		dec ixl
		jp nz, .loop		

		ld sp, (_temp_sp)
		ret

_draw_half_tiles_column_shifted_up_12b
		; in: 	HL - shadow buffer addr
		;	IXL - number of tiles
		;	EX BC - tiles map addr

		ld (_temp_sp), sp

		ld bc, scr_buff_next_tile_row_offs

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		add_addr_ax2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table

		ld de, 24
		add hl, de
		
		ld sp, hl
		exx
.loop
		dup 2			;<---
		pop de

		ld (hl), e
		inc h

		pop de

		ld (hl), e
		inc h		

		edup			;<---

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		add_addr_ax2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table
		
		ld sp, hl
		exx

		dup 6			;<---
		pop de

		ld (hl), e
		inc h

		pop de

		ld (hl), e
		inc h		

		edup			;<---

		add hl, bc

		dec ixl
		jp nz, .loop		

		ld sp, (_temp_sp)
		ret

_draw_tiles_column_shifted_up_4b
		; in: 	HL - shadow buffer addr
		;	IXL - number of tiles
		;	EX BC - tiles map addr

		ld (_temp_sp), sp

		ld bc, scr_buff_next_tile_row_offs

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		add_addr_ax2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table

		ld de, 8
		add hl, de
		
		ld sp, hl
		exx
.loop
		dup 6			;<---
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

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		add_addr_ax2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table
		
		ld sp, hl
		exx

		dup 2			;<---
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

		add hl, bc

		dec ixl
		jp nz, .loop		

		ld sp, (_temp_sp)
		ret

_draw_tiles_column_shifted_up_12b
		; in: 	HL - shadow buffer addr
		;	IXL - number of tiles
		;	EX BC - tiles map addr

		ld (_temp_sp), sp

		ld bc, scr_buff_next_tile_row_offs

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		add_addr_ax2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table

		ld de, 24
		add hl, de
		
		ld sp, hl
		exx
.loop
		dup 2			;<---
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

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		add_addr_ax2 tiles_addr_tbl

		ld sp, hl
		pop hl			; get tile graphics data address from the table
		
		ld sp, hl
		exx

		dup 6			;<---
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

		add hl, bc

		dec ixl
		jp nz, .loop		

		ld sp, (_temp_sp)
		ret

		ENDIF	//DEF_MOVE_STEP == MS_4b
		
		IF	DEF_MOVE_STEP == MS_TILE

_draw_tiles_2x2_mode

		; draw the top two thirds of the screen

		ld ixh, scr_buff_tiles_w

		ld hl, scr_buff
		
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
		inc l
		inc l		
		inc l
		inc l		
		ELSE //TR_DATA_TILES4X4
		inc l
		inc l		
		ENDIF //TR_DATA_TILES4X4

		dec ixh
		jp nz, _drw_loop1

		IF	DEF_FULLSCREEN

		; draw the bottom third of the screen

		ld ixh, scr_buff_tiles_w

		ld hl, scr_buff + 4096
		
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
		inc l
		inc l		
		inc l
		inc l		
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

		ld hl, clr_buff
		
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
		inc l
		inc l		
		inc l
		inc l		
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

		IF	DEF_MOVE_STEP == MS_TILE
LR_border_CHRs	= 0
		ELSE	// SM_8b / SM_4b
LR_border_CHRs	= 1
		ENDIF	//DEF_MOVE_STEP == MS_TILE

show_screen	
		IF	DEF_VERT_SYNC
		ei
		halt
		ENDIF	//DEF_VERT_SYNC

		di

		IF	DEF_128K_DBL_BUFFER

		ld a, (_scr_trigg)

		xor #01
		cp #01

		ld (_scr_trigg), a

		jp nz, .draw_scr0

		call _switch_Uscr		; show the main screen and draw on the extended one

		ld a, #80			; OR val to a high byte of a screen address ( the extended screen address )
		ld (_drw_sthb1), a
		
		; draw black/white image

		ld hl, #c000 + #10 + LR_border_CHRs
		exx
		ld hl, scr_buff + LR_border_CHRs
		exx
		call _draw_scr_32x24

		IF	DEF_COLOR
		; draw color

		ld hl, #d800 + #10 + LR_border_CHRs
		exx
		ld hl, clr_buff + LR_border_CHRs
		exx
		call _draw_clr_32x24

		ENDIF	//DEF_COLOR

		ret

.draw_scr0
		call _switch_Escr		; show the extended screen and draw on the main one

		ld a, #00			; OR val to a high byte of a screen address ( the main screen address )
		ld (_drw_sthb1), a
		
		ENDIF	//DEF_128K_DBL_BUFFER

		; draw black/white image

		ld hl, #4000 + #10 + LR_border_CHRs
		exx
		ld hl, scr_buff + LR_border_CHRs
		exx
		call _draw_scr_32x24

		IF	DEF_COLOR
		; draw color

		ld hl, #5800 + #10 + LR_border_CHRs
		exx
		ld hl, clr_buff + LR_border_CHRs
		exx
		call _draw_clr_32x24

		ENDIF	//DEF_COLOR
		
		ret

		IF	DEF_COLOR
_draw_clr_32x24

		ld (_temp_sp), sp

.loop		exx

		ld sp, hl

		ld bc, 16
		add hl, bc

		pop af
		pop bc
		pop de
		exx
		ex af,af'
		pop af
		pop bc
		pop de
		pop ix
		pop iy
		
		ld sp, hl

		push iy
		push ix
		push de
		push bc
		push af

		ld bc, 16 - ( LR_border_CHRs << 1 )
		add hl, bc

		exx
		ex af,af'
		push de
		push bc
		push af

		ld sp, hl

		ld bc, 16
		add hl, bc

		pop af
		pop bc
		pop de
		exx
		ex af,af'
		pop af
		pop bc
		pop de
		pop ix

		IF DEF_MOVE_STEP == MS_TILE
		pop iy
		ENDIF
		
		ld sp, hl

		IF DEF_MOVE_STEP == MS_TILE
		push iy
		ENDIF

		push ix
		push de
		push bc
		push af

		ld bc, 16 + ( LR_border_CHRs << 1 )
		add hl, bc

		exx
		ex af,af'
		push de
		push bc
		push af

		exx
		ld a, h
		and #03

		IF	DEF_FULLSCREEN
		cp #03
		ELSE
		cp #02
		ENDIF	//DEF_FULLSCREEN
		
		jp nz, .loop
		
		ld sp, (_temp_sp)		

		ret

		ENDIF	//DEF_COLOR

_draw_scr_32x24
		ld a, #64			; bit 4, h
		ld (_drw_tchk1), a

		ld (_temp_sp), sp

_loop32x24	exx
		
		dup 7

		ld sp, hl
		inc h
		pop af
		pop bc
		pop de
		exx
		ex af,af'
		pop af
		pop bc
		pop de
		pop ix
		pop iy
		
		ld sp, hl
		push iy
		push ix
		push de
		push bc
		push af
		inc h
		exx
		ex af,af'
		push de
		push bc
		push af

		edup

		ld sp, hl
		inc h
		pop af
		pop bc
		pop de
		exx
		ex af,af'
		pop af
		pop bc
		pop de
		pop ix
		pop iy
		
		ld sp, hl

		push iy
		push ix
		push de
		push bc
		push af
		exx
		ex af,af'
		push de
		push bc
		push af

		exx			
		ld bc, -1776 - ( LR_border_CHRs << 1 )
		add hl, bc
		exx
		ld bc, -2032
		add hl, bc

		dup 7

		ld sp, hl
		inc h
		pop af
		pop bc
		pop de
		exx
		ex af,af'
		pop af
		pop bc
		pop de
		pop ix

		IF DEF_MOVE_STEP == MS_TILE
		pop iy
		ENDIF
		
		ld sp, hl

		IF DEF_MOVE_STEP == MS_TILE
		push iy
		ENDIF

		push ix
		push de
		push bc
		push af
		inc h
		exx
		ex af,af'
		push de
		push bc
		push af

		edup

		ld sp, hl
		inc h

		db #cb
_drw_tchk1	db 0			; bit 4, h / bit 5, h
		jp z, .cont

		ld bc, scr_buff_next_tile_row_offs
		add hl, bc

.cont		pop af
		pop bc
		pop de
		exx
		ex af,af'
		pop af
		pop bc
		pop de
		pop ix

		IF DEF_MOVE_STEP == MS_TILE
		pop iy
		ENDIF
		
		ld sp, hl

		IF DEF_MOVE_STEP == MS_TILE
		push iy
		ENDIF

		push ix
		push de
		push bc
		push af
		exx
		ex af,af'
		push de
		push bc
		push af

		ld bc, -16
		add hl, bc

		exx
		dec hl			
		ld a, h			; go to line below in video memory
		sub 7
		ld h, a
		ld a, l
		add 16 + ( LR_border_CHRs << 1 )
		ld l, a
		inc hl

		jp nc, _loop32x24

		ld a, h

		IF	DEF_128K_DBL_BUFFER
		and #7F			; convert the screen address to the main one - #4000
		ENDIF	//DEF_128K_DBL_BUFFER

		ld h, #48		; check the second third
		cp #40

		IF	DEF_128K_DBL_BUFFER
		jp z, _fix_scr_addr
		ELSE
		jp z, _loop32x24
		ENDIF	//DEF_128K_DBL_BUFFER

		IF	DEF_FULLSCREEN
		cp #48			; check the third third
		jp z, _set_3th_data
		ENDIF	//DEF_FULLSCREEN

		ld sp, (_temp_sp)		

		ret

		IF	DEF_FULLSCREEN
_set_3th_data
		ld hl, #5000 + #10 + LR_border_CHRs
		ld a, #6C				; bit 5, h
		ld (_drw_tchk1), a
		exx
		ld hl, scr_buff + #1000 + LR_border_CHRs
		exx

		IF	DEF_128K_DBL_BUFFER
		jp _fix_scr_addr
		ELSE
		jp _loop32x24
		ENDIF	//DEF_128K_DBL_BUFFER

		ENDIF	//DEF_FULLSCREEN

		IF	DEF_128K_DBL_BUFFER
_fix_scr_addr
		ld a, h
		db #F6
_drw_sthb1	db 0			; [or #00|#80] switching between the main screen address and the extended one
		ld h, a

		jp _loop32x24

		ENDIF	//DEF_128K_DBL_BUFFER

		IF	DEF_128K_DBL_BUFFER
_scr_trigg	db 0	; screen switching trigger

_switch_Uscr	; show the main screen and switch to the 7th bank to draw the shadow buffer

		ld bc, #7ffd
		ld a, (23388)
		ld a, %00000111
		ld (23388), a
		out (c), a

		ret

_switch_Escr	; show the extended screen to draw on the main one

		ld bc, #7ffd
		ld a, (23388)
		ld a, %00001000
		ld (23388), a
		out (c), a

		ret

		ENDIF	//DEF_128K_DBL_BUFFER

_temp_sp	dw 0				; the stack pointer temporary variable

CODE_END

		DISPLAY "Data hole size: ", /D, DATA_HOLE_END - DATA_HOLE_START, " org: ", /H, DATA_HOLE_START, " (", /D, DATA_HOLE_START, ")"
		DISPLAY "Generated BACK BUFFER size: ", /D, BACK_BUFFER_END - BACK_BUFFER_START, " org: ", /H, BACK_BUFFER_START, " (", /D, BACK_BUFFER_START, ")"
		DISPLAY "Generated CODE size: ", /D, CODE_END - CODE_START, " org: ", /H, CODE_START, " (", /D, CODE_START, ")"

		ENDMODULE
