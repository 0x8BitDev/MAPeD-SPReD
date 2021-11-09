;###################################################################
;
; Copyright 2018-2021 0x8BitDev ( MIT license )
;
;###################################################################
;
; Bidirectional static screens switching test.
; Keys: Q, A, O, P
;

		device zxspectrum48

		org 25000
PROG_START

TR_DATA_TILES4X4	equ MAP_DATA_MAGIC&MAP_FLAG_TILES4X4
TR_COLORED_MAP		equ MAP_DATA_MAGIC&MAP_FLAG_COLOR_TILES
TR_ADJ_SCREENS		equ MAP_DATA_MAGIC&MAP_FLAG_LAYOUT_ADJACENT_SCREENS

		include "../../common/tilemap_bidir_render.asm"

main
		di	

		ld sp, 24999

		; interrupt handler

		ld a,24			; JR code
		ld (65535),a
		ld a,195		; JP code
		ld (65524),a
		ld hl,im_prc		; handler address
		ld (65525),hl
		ld hl,#FE00
		ld de,#FE01
		ld bc,#0100
		ld (hl),#FF
		ld a,h
		ldir
		ld i,a
		im 2

		; setup map data

		load_wdata tilemap_TilesScr,	tilemap_render.map_screens
		load_wdata tilemap_Gfx,		tilemap_render.map_gfx_arr

		IF TR_DATA_TILES4X4
		load_wdata tilemap_Tiles,					tilemap_render.map_tiles4x4	; tiles 4x4 data
		load_wdata tilemap_TilesOffs,					tilemap_render.map_tiles_offs
		load_bdata ( SCR_BLOCKS2x2_WIDTH * SCR_BLOCKS2x2_HEIGHT ) >> 2,	tilemap_render.map_scr_size
		ELSE
		load_bdata SCR_BLOCKS2x2_WIDTH * SCR_BLOCKS2x2_HEIGHT,		tilemap_render.map_scr_size
		ENDIF //TR_DATA_TILES4X4

		load_wdata tilemap_BlocksPropsOffs,				tilemap_render.map_props_arr

		IF TR_COLORED_MAP
		load_wdata tilemap_Clrs,	tilemap_render.map_clrs
		ENDIF //TR_COLORED_MAP

		IF !TR_ADJ_SCREENS
		load_wdata  Lev0_ScrArr,	tilemap_render.map_scr_arr
		ENDIF //!TR_ADJ_SCREENS

		load_wdata Lev0_StartScr,	tilemap_render.map_curr_scr

		; data init

		call tilemap_render.init
		call tilemap_render.draw_screen

		xor a
		ld (kb_state), a
loop		
		; keyboard handler QAOP
		; Q

		ld a, #fb
		in a, (#fe)
		and 1
		jr nz, next1

		ld a, (kb_state)
		and a
		jp nz, loop

		call tilemap_render.check_up_screen
		ld a, kb_flag_up
		jp c, new_screen

next1		; A
		
		ld a, #fd
		in a, (#fe)
		and 1
		jr nz, next2

		ld a, (kb_state)
		and a
		jp nz, loop

		call tilemap_render.check_down_screen
		ld a, kb_flag_down
		jp c, new_screen

next2		; O

		ld a, #df
		in a, (#fe)
		and 2
		jr nz, next3

		ld a, (kb_state)
		and a
		jp nz, loop

		call tilemap_render.check_left_screen
		ld a, kb_flag_left
		jp c, new_screen

next3		; P

		ld a, #df
		in a, (#fe)
		and 1
		jr nz, res_kb_state

		ld a, (kb_state)
		and a
		jp nz, loop

		call tilemap_render.check_right_screen
		ld a, kb_flag_right
		jp c, new_screen

		jp loop

new_screen
		ld (kb_state), a
		call tilemap_render.draw_screen
		jp loop
res_kb_state
		xor a
		ld (kb_state), a
		jp loop

im_prc		ei
		reti

kb_flag_up	= %00000001
kb_flag_down	= %00000010
kb_flag_left	= %00000100
kb_flag_right	= %00001000

kb_state	db 0

DATA_START
		include "data/tilemap.zxa"
DATA_END
PROG_END
		DISPLAY "Program start address: ", /D, PROG_START, " (#", /H, PROG_START, ")"
		DISPLAY "Program end address: ", /D, PROG_END, " (#", /H, PROG_END, ")"
		DISPLAY "Program size: ", /D, PROG_END-PROG_START, " (#", /H, PROG_END-PROG_START, ")"
		DISPLAY "Data size: ", /D, DATA_END-DATA_START, " (#", /H, DATA_END-DATA_START, ")"

		savesna "../../bin/bidir_stat_scr.sna", main