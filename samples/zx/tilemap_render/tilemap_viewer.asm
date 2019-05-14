;###################################################################
;
; Copyright 2018-2019 0x8BitDev ( MIT license )
;
;###################################################################
;
; Multidirectional scroller test.
;

		org 25000

		include "MOD_tilemap2x2_clr_render_settings.asm"

		include "data/tilemap.zxa"

		IF !COLORED_MAP
		UNDEFINE DEF_COLOR
		DEFINE DEF_COLOR 0
		ENDIF

		include "MOD_tilemap2x2_clr_render.asm"

max_lev_tiles_w	equ	Lev0_wtls		; max width of a level in tiles

	macro load_wdata data, addr
	    	ld hl, addr
		ld a, low data
		ld (hl), a
		inc hl
		ld a, high data
		ld (hl), a
	endm

	macro load_bdata data, addr
	    	ld hl, addr
		ld a, data
		ld (hl), a
	endm

main
		di	

		ld sp, 24576	;!!!!!!!

		load_wdata Lev0_map, 	tilemap_render.map_data		; game level tile map address
		load_wdata Lev0_tl,	tilemap_render.map_tiles_data	; tile graphics data

		IF COLORED_MAP
		load_wdata Lev0_tlc,	tilemap_render.map_tiles_clr	; tile colors data
		ENDIF

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

		savesna "../bin/tilemap_viewer.sna", main