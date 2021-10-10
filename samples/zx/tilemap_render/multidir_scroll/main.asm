;###################################################################
;
; Copyright 2018-2020 0x8BitDev ( MIT license )
;
;###################################################################
;
; Multidirectional scroller test.
;

		org 25000

		include "settings.asm"

TR_DATA_TILES4X4	equ MAP_DATA_MAGIC&MAP_FLAG_TILES4X4
TR_COLORED_MAP		equ MAP_DATA_MAGIC&MAP_FLAG_COLOR_TILES

		IF !TR_COLORED_MAP
		UNDEFINE DEF_COLOR
		DEFINE DEF_COLOR 0
		ENDIF

		include "tilemap_multidir_render.asm"

max_lev_tiles_w	equ	Lev0_wtls		; max width of a level in tiles

	macro load_wdata _data, _addr
		ld hl, _addr
		ld a, low _data
		ld (hl), a
		inc hl
		ld a, high _data
		ld (hl), a
	endm

	macro load_bdata _data, _addr
		ld hl, _addr
		ld a, _data
		ld (hl), a
	endm

main
		di	

		ld sp, 24999	;!!!!!!!

		load_wdata Lev0_map, 	tilemap_render.map_data		; game level tile map address
		load_wdata Lev0_tlg,	tilemap_render.map_tiles_gfx	; tile graphics data

		IF TR_COLORED_MAP
		load_wdata Lev0_tlc,	tilemap_render.map_tiles_clr	; tile colors data
		ENDIF //TR_COLORED_MAP

		IF TR_DATA_TILES4X4
		load_wdata Lev0_tli,	tilemap_render.map_tiles4x4	; tiles 4x4 data
		ENDIF //TR_DATA_TILES4X4

		load_wdata Lev0_t_tiles,tilemap_render.map_data_cnt	; number of tiles in map
		load_bdata Lev0_u_tiles,tilemap_render.map_tiles_cnt	; number of unique tiles in map
		                                                                                               
		load_wdata Lev0_wtls,	tilemap_render.map_tiles_w	; number of tiles in map in width
		load_wdata Lev0_htls,	tilemap_render.map_tiles_h	; number of tiles in map in height
		load_wdata Lev0_wchr, 	tilemap_render.map_chrs_w	; number of CHRs in map in width
		load_wdata Lev0_hchr,	tilemap_render.map_chrs_h	; number of CHRs in map in height

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