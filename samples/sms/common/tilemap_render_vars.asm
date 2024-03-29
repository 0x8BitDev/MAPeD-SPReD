;########################################################################
;
; Tilemap renderer variables
;
; Copyright 2020 0x8BitDev ( MIT license )
;
;########################################################################


.RAMSECTION "tilemap_render_vars" APPENDTO "ram_vars"

;.ifdef	TR_DATA_TILES4X4
TR_TILES4x4:
	dw

TR_TILES_OFFSETS:
	dw
;.endif
TR_BLOCKS_OFFSETS:
	dw

TR_BLOCK_ATTRS:
	dw

;.ifdef	TR_MULTIDIR_SCROLL
TR_TILES_MAP:
	dw

TR_MAP_LUT:
	dw

TR_MAP_SCR_WIDTH:
	db

TR_MAP_SCR_HEIGHT:
	db	
;.endif

TR_MAP_TILES_WIDTH:		; ScrTilesWidth for bidir mode
	dw

TR_MAP_TILES_HEIGHT:		; ScrTilesHeight for bidir mode
	dw

TR_START_SCR:
	dw

;.ifdef	TR_BIDIR_SCROLL
TR_CHR_ARR:
	dw

TR_CHR_SIZE_ARR:
	dw

TR_SCR_TILES_ARR:		; bidir scroll & stat scr (VDP-ready data)
	dw

TR_PALETTES_ARR:
	dw
;.endif

;.ifdef	TR_ADJACENT_SCR_INDS
TR_SCR_LABELS_ARR:
	dw
;.endif

; inner vars
;.ifdef	TR_MULTIDIR_SCROLL
tr_map_pix_cropped_width:	; map width in pixels minus screen width in pixels
	dw

tr_map_pix_cropped_height:	; map height in pixels minus screen height in pixels
	dw

tr_pos_x:
	dw
tr_pos_y:
	dw
;.endif

;.ifdef	TR_BIDIR_SCROLL
tr_curr_scr:
	dw
tr_horiz_dir_pos:
	dw
tr_vert_dir_pos:
	dw
tr_CHR_id:
	db
tr_tiles_offset:
	dw
tr_blocks_offset:
	dw
;.endif

;.ifdef	TR_BIDIR_STAT_SCR
tr_jpad_state:
	dw
;.endif

tr_scroll_x:
	db
tr_scroll_y:
	db

tr_flags:
	db

tr_data_flags:	
	db
tr_buff_addr:
	dw
tr_buff:
	dsb 256

decoded_map:
	db

.ENDS