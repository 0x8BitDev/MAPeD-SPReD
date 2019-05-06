;###################################################################
;
; Copyright 2018-2019 0x8BitDev ( MIT license )
;
;###################################################################
;
; DESC: Multidirectional scroller example.
;
;###################################################################
;
; LIMITATIONS: The maximum number of unique tiles in a level - 128
;
;###################################################################

; Public procs:
;
; tilemap_render.init		- render init
; tilemap_render.draw_tiles  	- draw tiles to shadow buffer
; tilemap_render.show_screen 	- draw shadow buffer to screen
;

		MODULE	tilemap_render

; level drawing data

; be sure to define in the main code!
;
; max_lev_tiles_w	equ	Lev0_wtls	; max width of a level in tiles
;

; this data must be filled in for each map in the main code
;
map_data	dw 0			; Lev0_map	game level tile map address
map_tiles_data	dw 0			; Lev0_tl 	tile graphics data
map_tiles_clr	dw 0			; Lev0_tlc 	tile colors data
map_data_cnt	dw 0 			; Lev0_t_tiles 	number of tiles in map
map_tiles_cnt	db 0			; Lev0_u_tiles 	number of unique tiles in map

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

scr_buff_tiles_w	equ scr_w >> 1	; the shadow buffer width in tiles
scr_buff_tiles_h	equ scr_h >> 1	; the shadow buffer height in tiles

_x_tile_addr_tbl	block ( max_lev_tiles_w << 1 ), 0	; address table for X coordinate

			align 256
tiles_addr_tbl		block 256,0	; tile graphics address table
tiles_clr_addr_tbl      block 256,0     ; tile colors(attrbutes) address table

DATA_HOLE_START

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

		IF	DEF_MOVE_STEP == MS_16b
		ld hl, (map_chrs_w)
		sra h
		rr l
		ld (_de_map_st_w), hl
		ld hl, (map_chrs_h)
		sra h
		rr l
		ld (_de_map_st_h), hl
		ld hl, scr_w
		sra h
		rr l
		ld (_de_scr_st_w), hl
		ld hl, scr_h
		sra h
		rr l
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
		jr nz, .loop1

		; fill the tile graphics lookup table
		
		ld a, (map_tiles_cnt)
		ld b, a

		ld ix, tiles_addr_tbl
		ld de, (map_tiles_data)

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

		IF	DEF_MOVE_STEP == MS_16b

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

		jr z, .cont1

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
		jr nz, .cont2			; if coordinate is not a multiple of 3, skip column drawing

		and #01
		jr z, .cont2			; if coordinate is even, then skip column drawing

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
		jr nz, _drw_loop1

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
		jr nz, _drw_loop2

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
		jr nz, _drw_clr_loop

		ENDIF	//DEF_COLOR

		IF	DEF_MOVE_STEP == MS_4b
		ld de, (x_pos)

		ld a, e
		and #01
		ret z
		
		ld a, e
		and #03
		cp #03
		jr nz, _skip_hlf_tile_drw

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
		ld de, #f020
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
		jr nz, .shift_loop3

		add hl, de
		
		djnz .shift_loop2
		ENDIF	//DEF_FULLSCREEN

		ENDIF	//DEF_MOVE_STEP == MS_4b

		ret

		ENDIF	//DEF_MOVE_STEP == MS_16b

		IF	DEF_COLOR
_draw_tiles_color_column
		; in: 	HL - shadow buffer addr
		;	IXL - number of tiles
		;	EX BC - tiles map addr

		ld (_temp_sp), sp

		ld bc, 31
		
.loop		exx
		ld a, (bc)		; get tile index
		inc bc
		
		ld h, high tiles_clr_addr_tbl
		ld l, a

		ld sp, hl
		pop hl			; get tile color data address from the table
		
		ld sp, hl
		exx

		pop de

		ld (hl), e
		inc l
		ld (hl), d

		add hl, bc

		pop de

		ld (hl), e
		inc l
		ld (hl), d

		add hl, bc

		dec ixl
		jp nz, .loop		

		ld sp, (_temp_sp)

		ret

_draw_tiles_color_column_shifted_up
		; in: 	HL - shadow buffer addr
		;	IXL - number of tiles
		;	EX BC - tiles map addr

		ld (_temp_sp), sp

		ld bc, 31
		
		exx
		ld a, (bc)		; get tile index
		inc bc
		
		ld h, high tiles_clr_addr_tbl
		ld l, a

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
		
		ld h, high tiles_clr_addr_tbl
		ld l, a

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

_draw_tiles_column
		; in: 	HL - shadow buffer addr
		;	IXL - number of tiles
		;	EX BC - tiles map addr

		ld (_temp_sp), sp

		ld bc, #f020

.loop		exx
		ld a, (bc)		; get tile index
		inc bc
		
		ld h, high tiles_addr_tbl
		ld l, a

		ld sp, hl
		pop hl			; get tile graphics data address from the table
		
		ld sp, hl
		exx

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

		add hl, bc

		dec ixl
		jp nz, .loop		

		ld sp, (_temp_sp)
		ret

_draw_tiles_column_shifted_up_8b
		; in: 	HL - shadow buffer addr
		;	IXL - number of tiles
		;	EX BC - tiles map addr

		ld (_temp_sp), sp

		ld bc, #f020

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		ld h, high tiles_addr_tbl
		ld l, a

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
		
		ld h, high tiles_addr_tbl
		ld l, a

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

		ld bc, #f020

.loop		exx
		ld a, (bc)		; get tile index
		inc bc
		
		ld h, high tiles_addr_tbl
		ld l, a

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

		ld bc, #f020

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		ld h, high tiles_addr_tbl
		ld l, a

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
		
		ld h, high tiles_addr_tbl
		ld l, a

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

		ld bc, #f020

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		ld h, high tiles_addr_tbl
		ld l, a

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
		
		ld h, high tiles_addr_tbl
		ld l, a

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

		ld bc, #f020

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		ld h, high tiles_addr_tbl
		ld l, a

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
		
		ld h, high tiles_addr_tbl
		ld l, a

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

		ld bc, #f020

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		ld h, high tiles_addr_tbl
		ld l, a

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
		
		ld h, high tiles_addr_tbl
		ld l, a

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

		ld bc, #f020

		exx
		ld a, (bc)		; get tile index
		inc bc
		
		ld h, high tiles_addr_tbl
		ld l, a

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
		
		ld h, high tiles_addr_tbl
		ld l, a

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
		
        	IF	DEF_MOVE_STEP == MS_16b

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

		exx

_drw_loop1	ld ixl, 8		; number of 2x2 tiles in the upper 2 thirds

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

		inc l
		inc l		

		dec ixh
		jr nz, _drw_loop1

		IF	DEF_FULLSCREEN

		; draw the bottom third of the screen

		ld ixh, scr_buff_tiles_w

		ld hl, scr_buff + 4096
		
		exx
		pop hl			; BC - tilemap start address for pos_x|pos_y
		ld de, 8
		add hl, de
		ld c, l
		ld b, h
		exx

_drw_loop2	ld ixl, 4		; number of 2x2 tiles in the lower third

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

		inc l
		inc l		

		dec ixh
		jr nz, _drw_loop2

		ENDIF	//DEF_FULLSCREEN

		IF	DEF_COLOR

		; draw color

		ld ixh, scr_buff_tiles_w

		ld hl, clr_buff
		
		exx
		pop bc
		exx

_drw_loop_clr		
		IF	DEF_FULLSCREEN
		ld ixl, scr_buff_tiles_h
		ELSE
		ld ixl, 8		; for 2/3 of the screen
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

		inc l
		inc l		

		dec ixh
		jr nz, _drw_loop_clr

		ENDIF	//DEF_COLOR

		ret

		ENDIF	//DEF_MOVE_STEP == MS_16b

_fix_x_y	
		; check coordinates so that the entire screen of tiles is always drawn

		ld hl, (x_pos)

		bit 7, h
		jr z, .cont1

		ld hl, 0
		jr __save_x

.cont1		
		db #11			; ld de, scr_steps_w
_de_scr_st_w	dw 0

		add hl, de

		db #11			; ld de, map_steps_w
_de_map_st_w	dw 0

		and a			; reset the carry flag, just in case...
		sbc hl, de  		; if ( x_pos + scr_w ) < map_chrs_w, then continue

		jr c, __cont2

		ld de, (x_pos)
		ex de, hl
		and a
		sbc hl, de		; subtract the overflow from x_pos, thereby adjusting the coordinate to desired value
				
__save_x	ld (x_pos), hl		
		
__cont2		ld hl, (y_pos)

		bit 7, h
		jr z, .cont3

		ld hl, 0
		jr __save_y
		
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

; this bold piece of code must be inlined in two places 
; and I didn't want to duplicate it, so I did the macro

	MACRO _128K_DBL_BUFFERING offset, drw_r_u, drw_r_d, drw_clr_r, drw_sthb1, drw_sthb2

		IF	DEF_128K_DBL_BUFFER
		ld a, (_scr_trigg)

		xor #01
		cp #01

		ld (_scr_trigg), a

		jr nz, .draw_scr0

		call _switch_Uscr		; show the main screen and draw on the extended one

		ld a, #c8
		ld (drw_sthb1), a
		ld (drw_sthb2), a
		
		; the top left part of the screen

		ld hl, #c000 + #10 + offset
		exx
		ld hl, scr_buff + offset
		exx
		call _draw_scr_16x16

		; the top right part of the screen

		ld hl, #c000 + #20 - offset
		exx
		ld hl, scr_buff + #10 + offset
		exx
		call drw_r_u	;_draw_scr_14x16

		IF	DEF_FULLSCREEN

		; the bottom left part of the screen

		ld hl, #d000 + #10 + offset
		exx
		ld hl, scr_buff + #1000 + offset
		exx
		call _draw_scr_16x8

		; the bottom right part of the screen

		ld hl, #d000 + #20 - offset
		exx
		ld hl, scr_buff + #1000 + #10 + offset
		exx
		call drw_r_d	;_draw_scr_14x8

		ENDIF	//DEF_FULLSCREEN

		IF	DEF_COLOR		

		; draw color

		ld hl, #d800 + #10 + offset
		exx
		ld hl, clr_buff + offset
		exx
		call _draw_clr_16x24

		ld hl, #d800 + #20 - offset
		exx
		ld hl, clr_buff + #10 + offset
		exx
		call drw_clr_r	;_draw_clr_14x24

		ENDIF	//DEF_COLOR

		ret

.draw_scr0
		call _switch_Escr		; show the extended screen and draw on the main one

		ld a, #48
		ld (drw_sthb1), a
		ld (drw_sthb2), a
		
		ENDIF	//DEF_128K_DBL_BUFFER
		
	ENDM

		IF	DEF_MOVE_STEP == MS_16b
show_screen	
		IF	DEF_VERT_SYNC
		ei
		halt
		ENDIF	//DEF_VERT_SYNC

		di

		_128K_DBL_BUFFERING 0, _draw_scr_16x16, _draw_scr_16x8, _draw_clr_16x24, _drw_sthb1, _drw_sthb1

		; the top left part of the screen

		ld hl, #4000 + #10
		exx
		ld hl, scr_buff
		exx
		call _draw_scr_16x16

		; the top right part of the screen

		ld hl, #4000 + #20
		exx
		ld hl, scr_buff + #10
		exx
		call _draw_scr_16x16

		IF	DEF_FULLSCREEN

		; the bottom left part of the screen

		ld hl, #5000 + #10
		exx
		ld hl, scr_buff + #1000
		exx
		call _draw_scr_16x8

		; the bottom right part of the screen

		ld hl, #5000 + #20
		exx
		ld hl, scr_buff + #1000 + #10
		exx
		call _draw_scr_16x8

		ENDIF	//DEF_FULLSCREEN

		IF	DEF_COLOR		

		; draw color

		ld hl, #5800 + #10
		exx
		ld hl, clr_buff
		exx
		call _draw_clr_16x24

		ld hl, #5800 + #20
		exx
		ld hl, clr_buff + #10
		exx
		call _draw_clr_16x24

		ENDIF	//DEF_COLOR
		
		ret

		IF	DEF_COLOR
_draw_clr_16x24

		ld (_temp_sp), sp

.loop		exx

		ld sp, hl

		ld bc, 32
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

		ld bc, 32
		add hl, bc

		exx
		ex af,af'
		push de
		push bc
		push af

		exx
		dec hl  	; <-- this address correction allows to stop drawing on the right part of the screen correctly
		ld a, h
		inc hl          ; <-- this address correction allows to stop drawing on the right part of the screen correctly
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

_draw_scr_16x16
		ld (_temp_sp), sp

_loop16x16	exx

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

		bit 4, h
		jr z, .cont

		ld de, #f020
		add hl, de

.cont		pop af
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
		dec hl			; <-- this address correction allows to stop drawing on the right part of the screen correctly
		ld a, h                 ; go to line below in video memory
		sub 7
		ld h, a
		ld a, l
		add 32
		ld l, a
		inc hl			; <-- this address correction allows to stop drawing on the right part of the screen correctly

		jp nc, _loop16x16

		bit 3, h
		IF	DEF_128K_DBL_BUFFER
		db #26
_drw_sthb1	db 0
		ELSE
		ld h, #48		; go to the second third
		ENDIF	//DEF_128K_DBL_BUFFER
		jp z, _loop16x16

		ld sp, (_temp_sp)		

		ret

_draw_scr_16x8
		ld (_temp_sp), sp

.loop		exx

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

		bit 5, h
		jr z, .cont

		ld de, #f020
		add hl, de

.cont		pop af
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
		dec hl			; <-- this address correction allows to stop drawing on the right part of the screen correctly
		ld a, h                 ; go to line below in video memory
		sub 7
		ld h, a
		ld a, l
		add 32
		ld l, a
		inc hl			; <-- this address correction allows to stop drawing on the right part of the screen correctly

		jp nc, .loop		

		ld sp, (_temp_sp)		

		ret
		
		ELSE	// SM_8b / SM_4b

show_screen	
		IF	DEF_VERT_SYNC
		ei
		halt
		ENDIF	//DEF_VERT_SYNC

		di

		_128K_DBL_BUFFERING 1, _draw_scr_14x16, _draw_scr_14x8, _draw_clr_14x24, _drw_sthb1, _drw_sthb2

		; the top left part of the screen

		ld hl, #4000 + #10 + 1
		exx
		ld hl, scr_buff + 1
		exx
		call _draw_scr_16x16

		; the top right part of the screen

		ld hl, #4000 + #20 - 1
		exx
		ld hl, scr_buff + #10 + 1
		exx
		call _draw_scr_14x16

		IF	DEF_FULLSCREEN

		; the bottom left part of the screen

		ld hl, #5000 + #10 + 1
		exx
		ld hl, scr_buff + #1000 + 1
		exx
		call _draw_scr_16x8

		; the bottom right part of the screen

		ld hl, #5000 + #20 - 1
		exx
		ld hl, scr_buff + #1000 + #10 + 1
		exx
		call _draw_scr_14x8

		ENDIF	//DEF_FULLSCREEN

		IF	DEF_COLOR		

		; draw color

		ld hl, #5800 + #10 + 1
		exx
		ld hl, clr_buff + 1
		exx
		call _draw_clr_16x24

		ld hl, #5800 + #20 - 1
		exx
		ld hl, clr_buff + #10 + 1
		exx
		call _draw_clr_14x24

		ENDIF	//DEF_COLOR
		
		ret

		IF	DEF_COLOR
_draw_clr_16x24

		ld (_temp_sp), sp

.loop		exx

		ld sp, hl

		ld bc, 32
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

		ld bc, 32
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

_draw_clr_14x24

		ld (_temp_sp), sp

.loop		exx

		ld sp, hl

		ld bc, 32
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
		
		ld sp, hl

		push ix
		push de
		push bc
		push af

		ld bc, 32
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

_draw_scr_16x16
		ld (_temp_sp), sp

_loop16x16	exx

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

		bit 4, h
		jr z, .cont

		ld de, #f020
		add hl, de

.cont		pop af
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
		ld a, h                 ; go to line below in video memory
		sub 7
		ld h, a
		ld a, l
		add 32
		ld l, a

		jp nc, _loop16x16

		bit 3, h

		IF	DEF_128K_DBL_BUFFER
		db #26
_drw_sthb1	db 0
		ELSE
		ld h, #48		; go to the second third
		ENDIF	//DEF_128K_DBL_BUFFER

		jp z, _loop16x16

		ld sp, (_temp_sp)		

		ret

_draw_scr_14x16
		ld (_temp_sp), sp

_loop14x16	exx

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
		
		ld sp, hl
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

		bit 4, h
		jr z, .cont

		ld de, #f020
		add hl, de

.cont		pop af
		pop bc
		pop de
		exx
		ex af,af'
		pop af
		pop bc
		pop de
		pop ix
		
		ld sp, hl
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
		ld a, h                 ; go to line below in video memory
		sub 7
		ld h, a
		ld a, l
		add 32
		ld l, a

		jp nc, _loop14x16

		bit 3, h

		IF	DEF_128K_DBL_BUFFER
		db #26
_drw_sthb2	db 0
		ELSE
		ld h, #48		; go to the second third
		ENDIF	//DEF_128K_DBL_BUFFER

		jp z, _loop14x16

		ld sp, (_temp_sp)		

		ret

_draw_scr_16x8
		ld (_temp_sp), sp

.loop		exx

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

		bit 5, h
		jr z, .cont

		ld de, #f020
		add hl, de

.cont		pop af
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
		ld a, h                 ; go to line below in video memory
		sub 7
		ld h, a
		ld a, l
		add 32
		ld l, a

		jp nc, .loop		

		ld sp, (_temp_sp)		

		ret

_draw_scr_14x8
		ld (_temp_sp), sp

.loop		exx

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
		
		ld sp, hl
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

		bit 5, h
		jr z, .cont

		ld de, #f020
		add hl, de

.cont		pop af
		pop bc
		pop de
		exx
		ex af,af'
		pop af
		pop bc
		pop de
		pop ix
		
		ld sp, hl
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
		ld a, h                 ; go to line below in video memory
		sub 7
		ld h, a
		ld a, l
		add 32
		ld l, a

		jp nc, .loop		

		ld sp, (_temp_sp)		

		ret
		
		ENDIF	//DEF_MOVE_STEP == MS_16b

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
