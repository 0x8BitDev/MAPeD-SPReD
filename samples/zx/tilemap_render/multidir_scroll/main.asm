;###################################################################
;
; Copyright 2018-2021 0x8BitDev ( MIT license )
;
;###################################################################
;
; Multidirectional scroller test.
; Keys: Q, A, O, P
;

		org 25000

		include "settings.asm"

TR_DATA_TILES4X4	equ MAP_DATA_MAGIC&MAP_FLAG_TILES4X4
TR_COLORED_MAP		equ MAP_DATA_MAGIC&MAP_FLAG_COLOR_TILES

		IF !TR_COLORED_MAP
		UNDEFINE DEF_COLOR
		DEFINE DEF_COLOR 0
		ENDIF

		include "../../common/tilemap_multidir_render.asm"

max_lev_tiles_w	equ	Lev0_wtls		; max width of a level in tiles

main
		di	

		ld sp, 24999

		LOAD_WDATA Lev0_Map, 	tilemap_render.map_data		; game level tile map address
		LOAD_WDATA Lev0_Gfx,	tilemap_render.map_tiles_gfx	; tile graphics data

		IF TR_COLORED_MAP
		LOAD_WDATA Lev0_Clrs,	tilemap_render.map_tiles_clr	; tile colors data
		ENDIF //TR_COLORED_MAP

		IF TR_DATA_TILES4X4
		LOAD_WDATA Lev0_Tiles,	tilemap_render.map_tiles4x4	; tiles 4x4 data
		ENDIF //TR_DATA_TILES4X4

		LOAD_WDATA Lev0_t_tiles,tilemap_render.map_data_cnt	; number of tiles in map
		LOAD_BDATA Lev0_u_tiles,tilemap_render.map_tiles_cnt	; number of unique tiles in map
		                                                                                               
		LOAD_WDATA Lev0_wtls,	tilemap_render.map_tiles_w	; number of tiles in map in width
		LOAD_WDATA Lev0_htls,	tilemap_render.map_tiles_h	; number of tiles in map in height
		LOAD_WDATA Lev0_wchr, 	tilemap_render.map_chrs_w	; number of CHRs in map in width
		LOAD_WDATA Lev0_hchr,	tilemap_render.map_chrs_h	; number of CHRs in map in height

		LOAD_BDATA Lev0_StartScr,	tilemap_render.map_start_scr
		LOAD_BDATA Lev0_wscr,		tilemap_render.map_scr_width	; number of map screens in width
		LOAD_BDATA Lev0_hscr,		tilemap_render.map_scr_height	; number of map screens in height

		LOAD_BDATA SCR_BLOCKS2x2_WIDTH,		tilemap_render.map_scr_blocks_w	; number of screen blocks (2x2) in width
		LOAD_BDATA SCR_BLOCKS2x2_HEIGHT,	tilemap_render.map_scr_blocks_h ; number of screen blocks (2x2) in height

		IF	DEF_128K_DBL_BUFFER

		; interrupt init in used memory banks

		call tilemap_render._switch_Uscr

		; clear attributes of extended screen

		ld hl, #d800
		ld a, 7<<3
		ld (hl), a
		ld de, #d801
		ld bc, #300
		ldir

		ld   a,24      		; JR code
	        ld   (65535),a
	        ld   a,195     		; JP code
	        ld   (65524),a
	        ld   hl,im_prc    	; handler address
	        ld   (65525),hl
	        ld   hl,#FE00
	        ld   de,#FE01
	        ld   bc,#0100
	        ld   (hl),#FF
	        ld   a,h
	        ldir
	        ld   i,a
	        im   2

		call tilemap_render._switch_Escr

		ld   a,24      		; JR code
	        ld   (65535),a
	        ld   a,195     		; JP code
	        ld   (65524),a
	        ld   hl,im_prc    	; handler address
	        ld   (65525),hl
	        ld   hl,#FE00
	        ld   de,#FE01
	        ld   bc,#0100
	        ld   (hl),#FF
	        ld   a,h
	        ldir

		ENDIF	//DEF_128K_DBL_BUFFER

		call tilemap_render.init

.loop		call tilemap_render.draw_tiles
		call tilemap_render.show_screen

		; keyboard handler QAOP
		; Q

		ld a, #fb
		in a, (#fe)
		and 1
		jr nz, .next1

		ld hl, (tilemap_render.y_pos)
		dec hl
		ld (tilemap_render.y_pos), hl

.next1		; A
		
		ld a, #fd
		in a, (#fe)
		and 1
		jr nz, .next2

		ld hl, (tilemap_render.y_pos)
		inc hl
		ld (tilemap_render.y_pos), hl
		
.next2		; O

		ld a, #df
		in a, (#fe)
		and 2
		jr nz, .next3

		ld hl, (tilemap_render.x_pos)
		dec hl
		ld (tilemap_render.x_pos), hl

.next3		; P

		ld a, #df
		in a, (#fe)
		and 1
		jr nz, .next4

		ld hl, (tilemap_render.x_pos)
		inc hl
		ld (tilemap_render.x_pos), hl

.next4		jr .loop


im_prc		ei
		reti

		include "data/tilemap.zxa"
PROG_END
		DISPLAY "Program end address: ", /D, PROG_END, " (#", /H, PROG_END, ")"

		IF ( PROG_END > #C000 ) && DEF_128K_DBL_BUFFER
		DISPLAY "WARNING: The program overlaps extended memory address space! Double buffering may cause graphics artefacts!"
		ENDIF

		savesna "../../bin/multidir_scroll.sna", main