;###############################################
;
; Copyright 2018-2019 0x8BitDev ( MIT license )
;
;###############################################
;
; Some helpful tilemap renderers code
;

; Public procs:
;	check_adjacent_screen	- X-low/Y-high of a screen data ptr
;
;	[Dynamic PPU data buffering]
;	buff_reset
;	buff_push_hdr
;	buff_push_data
;	buff_end
;	buff_apply
;
;	need_draw
;	draw_screen_nametable
;	draw_screen_attrs


.define		MAP_CHECK(arg) ( ::MAP_DATA_MAGIC & arg )

TR_DATA_RLE		= MAP_CHECK(::MAP_FLAG_RLE)
TR_DATA_TILES4X4	= MAP_CHECK(::MAP_FLAG_TILES4X4)

	.IF TR_DATA_TILES4X4

SCR_WTILES_CNT	= 8	; number of tiles at screen in width
SCR_HTILES_CNT	= 8	; number of tiles at screen in height

SCR_WATTRS_CNT  = SCR_WTILES_CNT	; number of attrs at screen in width
SCR_HATTRS_CNT	= SCR_HTILES_CNT	; number of attrs at screen in height

SCR_WCHR_CNT	= SCR_WTILES_CNT << 2		; number of CHRs at screen in width 
SCR_HCHR_CNT	= ( SCR_HTILES_CNT << 2 ) - 2	; number of CHRs at screen in height \ [-2] - fullscreen PAL

	.ELSE	; TR_DATA_TILES4X4

SCR_WTILES_CNT	= 16 ; number of tiles at screen in width
SCR_HTILES_CNT	= 15 ; number of tiles at screen in height

SCR_WATTRS_CNT  = SCR_WTILES_CNT >> 1			; number of attrs at screen in width
SCR_HATTRS_CNT	= ( ( SCR_HTILES_CNT + 1 ) >> 1 )	; number of attrs at screen in height

SCR_WCHR_CNT	= SCR_WTILES_CNT << 1	; number of CHRs at screen in width 
SCR_HCHR_CNT	= SCR_HTILES_CNT << 1	; number of CHRs at screen in height \ fullscreen PAL

	.ENDIF; TR_DATA_TILES4X4

	.scope TR_utils

TR_SCROLLED_MAP	= MAP_CHECK(::MAP_FLAG_MODE_MULTIDIR_SCROLL) || MAP_CHECK(::MAP_FLAG_MODE_BIDIR_SCROLL)

.segment "ZP"

	.IF MAP_CHECK(::MAP_FLAG_LAYOUT_ADJACENT_SCR_INDS)
tr_screens_ptr_arr:	.res 2
	.ENDIF	; MAP_FLAG_LAYOUT_ADJACENT_SCR_INDS
	.scope inner_vars

	.IF TR_utils::TR_SCROLLED_MAP
_tile_addr:	.res 2
_block_addr:	.res 2
	.ENDIF

	.endscope

tr_curr_scr:	.res 2	; current screen ptr

	.IF !MAP_CHECK(::MAP_FLAG_MODE_STATIC_SCREENS)

TR_BUFF_FLAGS_INC1	= %00000001
TR_BUFF_FLAGS_INC8	= %00000010
TR_BUFF_FLAGS_INC32	= %00000100
TR_BUFF_END		= 0			; end of the buffer

tr_data_flags:	.res 1
tr_buff_pos:	.res 1

	.IF MAP_CHECK(::MAP_FLAG_MODE_MULTIDIR_SCROLL)
tr_buff:	.res 128		; 2 * ( 32 + 2*4 + 8 + 2*4 ) = 112
	.ELSE
tr_buff:	.res 70			; 16 + 32 + 8 + 4*3 = 68			
	.ENDIF	; MAP_FLAG_MODE_MULTIDIR_SCROLL

	.ENDIF	; !MAP_FLAG_MODE_STATIC_SCREENS

; adjacent screen masks
TR_MASK_ADJ_SCR_LEFT	= %00010000
TR_MASK_ADJ_SCR_UP	= %00100000
TR_MASK_ADJ_SCR_RIGHT	= %01000000
TR_MASK_ADJ_SCR_DOWN	= %10000000


.segment "CODE"

; MACROSES
	; load palette
	; IN: A - CHR id
	.macro	load_palette_16 _tr_palettes, _data_addr, _data_size, _PPU_addr_low_byte
	sta _data_addr
	lda #$00
	sta _data_addr + 1
	mul16_word _data_addr
	ldx _tr_palettes
	ldy _tr_palettes + 1
	add_xy_to_word _data_addr

	lda #$10		; save 16 indices of palette
	sta _data_size
	; PPU address
	ldx #_PPU_addr_low_byte		; low
	ldy #$3f			; high
	jsr ppu_load_palettes
	.endmacro

	.macro check_left_screen
	lda #$00	; left
	sta _tmp_val

	lda #TR_utils::TR_MASK_ADJ_SCR_LEFT
	sta _tmp_val + 1

	jsr TR_utils::check_adjacent_screen
	.endmacro

	.macro check_right_screen
	.IF MAP_CHECK(::MAP_FLAG_LAYOUT_ADJACENT_SCREENS)
	lda #$04	; right
	.ELSE
	lda #$02	; right
	.ENDIF	; MAP_FLAG_LAYOUT_ADJACENT_SCREENS

	sta _tmp_val

	lda #TR_utils::TR_MASK_ADJ_SCR_RIGHT
	sta _tmp_val + 1

	jsr TR_utils::check_adjacent_screen
	.endmacro

	.macro check_up_screen
	.IF MAP_CHECK(::MAP_FLAG_LAYOUT_ADJACENT_SCREENS)
	lda #$02	; up
	.ELSE
	lda #$01	; up
	.ENDIF	; MAP_FLAG_LAYOUT_ADJACENT_SCREENS

	sta _tmp_val

	lda #TR_utils::TR_MASK_ADJ_SCR_UP
	sta _tmp_val + 1

	jsr TR_utils::check_adjacent_screen
	.endmacro

	.macro check_down_screen
	.IF MAP_CHECK(::MAP_FLAG_LAYOUT_ADJACENT_SCREENS)
	lda #$06	; down
	.ELSE
	lda #$03	; down
	.ENDIF	; MAP_FLAG_LAYOUT_ADJACENT_SCREENS

	sta _tmp_val

	lda #TR_utils::TR_MASK_ADJ_SCR_DOWN
	sta _tmp_val + 1

	jsr TR_utils::check_adjacent_screen
	.endmacro


; IN: 	tr_curr_scr - scren data ptr
;	tr_screens_ptr_arr ( if MAP_FLAG_LAYOUT_ADJACENT_SCR_INDS is active )
;	_tmp_val - low: scr data offset, high: dir mask
; OUT: 	carry flag = 1 -> X/Y - new screen ptr
;	carry flag = 0 -> nothing happened

check_adjacent_screen:

	; get adjacent screens mask

	.IF MAP_CHECK(::MAP_FLAG_MARKS)
	ldy #$01
	lda (<tr_curr_scr), y		; mask

	and _tmp_val + 1
	beq _check_adjacent_screen_exit

	.IF MAP_CHECK(::MAP_FLAG_ATTRS_PER_CHR)
	.IF MAP_CHECK(::MAP_FLAG_RLE)
	ldy #$07
	.ELSE
	ldy #$05
	.ENDIF	; MAP_FLAG_RLE
	.ELSE
	.IF MAP_CHECK(::MAP_FLAG_MODE_STATIC_SCREENS)
	ldy #$05
	.ELSE
	ldy #$03
	.ENDIF	; MAP_FLAG_MODE_STATIC_SCREENS
	.ENDIF	; MAP_FLAG_ATTRS_PER_CHR

	.ELSE	; MAP_FLAG_MARKS

	.IF MAP_CHECK(::MAP_FLAG_ATTRS_PER_CHR)
	.IF MAP_CHECK(::MAP_FLAG_RLE)
	ldy #$06
	.ELSE
	ldy #$04
	.ENDIF	; MAP_FLAG_RLE
	.ELSE
	.IF MAP_CHECK(::MAP_FLAG_MODE_STATIC_SCREENS)
	ldy #$04
	.ELSE
	ldy #$02
	.ENDIF	; MAP_FLAG_MODE_STATIC_SCREENS
	.ENDIF	; MAP_FLAG_ATTRS_PER_CHR

	.ENDIF ; MAP_FLAG_MARKS

	; add the offset to get a proper screen index position
	tya
	clc
	adc _tmp_val
	tay

	.IF MAP_CHECK(::MAP_FLAG_LAYOUT_ADJACENT_SCR_INDS)

	lda (<tr_curr_scr), y
	tay
	cmp #$FF
	beq _check_adjacent_screen_exit

	; Y - valid screen index
	tya
	mul2_a_xy

	clc
	txa
	adc tr_screens_ptr_arr
	sta _tmp_val
	tya
	adc tr_screens_ptr_arr + 1
	sta _tmp_val + 1

	ldy #$00
	lda (<_tmp_val), y
	tax
	iny
	lda (<_tmp_val), y
	tay

	; test ZERO screen
	cmp #$00				; invalid screen, if the high byte is zero
	beq _check_adjacent_screen_exit

	sec

	rts

	.ELSE

	.IF MAP_CHECK(::MAP_FLAG_LAYOUT_ADJACENT_SCREENS)

	lda (<tr_curr_scr), y
	tax
	iny
	lda (<tr_curr_scr), y
	cmp #$00
	beq _check_adjacent_screen_exit
	tay

	sec

	rts

	.ENDIF	; MAP_FLAG_LAYOUT_ADJACENT_SCREENS
	.ENDIF	; MAP_FLAG_LAYOUT_ADJACENT_SCR_INDS

_check_adjacent_screen_exit:

	clc

	rts

	.IF !MAP_CHECK(::MAP_FLAG_MODE_STATIC_SCREENS)

; Dynamic PPU data buffer to transfer graphics data during NMI

buff_reset:

	lda #$00
	sta tr_buff
	sta tr_buff_pos
	sta tr_data_flags
	sta data_addr
	sta data_addr + 1

	rts

buff_push_hdr:
	; A - length
	; data_addr - data addr
	; tr_data_flags - flags

	ldy tr_buff_pos
	sta tr_buff, y
	iny

	lda tr_data_flags
	sta tr_buff, y
	iny

	lda data_addr + 1
	sta tr_buff, y
	iny
	lda data_addr
	sta tr_buff, y
	iny

	sty tr_buff_pos

	rts

buff_push_data:
	; A - data

	ldy tr_buff_pos
	sta tr_buff, y
	iny

	sty tr_buff_pos

	rts

buff_end:
	lda #TR_BUFF_END
	ldy tr_buff_pos
	sta tr_buff, y

	rts

buff_apply:
	ldy #$00

@loop1:
	lda tr_buff, y			; A = length
	bne @cont1

	jsr buff_reset

	rts

@cont1:
	iny

	pha

	lda #TR_BUFF_FLAGS_INC32
	and tr_buff, y
	beq @cont2			; jump if TR_BUFF_FLAGS_INC32 == 0

	set_ppu_32_inc
	jmp @cont4

@cont2:
	lda #TR_BUFF_FLAGS_INC8
	and tr_buff, y
	beq @cont3			; jump if TR_BUFF_FLAGS_INC8 == 0

	; fill attributes (+8)
	iny

	lda tr_buff, y
	sta _tmp_val
	iny
	lda tr_buff, y
	sta _tmp_val + 1
	iny

	lda $2002

	pla
	tax
@loop2:
	lda _tmp_val			; high
	sta $2006
	lda _tmp_val + 1		; low
	sta $2006

	lda tr_buff, y
	sta $2007
	iny

	lda _tmp_val + 1
	clc
	adc #$08
	sta _tmp_val + 1

	dex
	bne @loop2

	jmp @loop1

@cont3:
	set_ppu_1_inc

@cont4:
	iny
	lda $2002
	lda tr_buff, y
	sta $2006
	iny
	lda tr_buff, y
	sta $2006
	iny

	pla
	tax
@loop3:
	lda tr_buff, y
	sta $2007
	iny

	dex
	bne @loop3

	jmp @loop1

need_draw:
	; OUT: A != 0 - need draw
	lda tr_buff
	rts

	.ENDIF	; !MAP_FLAG_MODE_STATIC_SCREENS

	.IF TR_SCROLLED_MAP

; To draw fullscreen nametable\attributes some aliases need to be declared. For example:
;
;	TR_HTILES	= TR_ms::tr_htiles
;	TR_BLOCKS	= TR_ms::tr_blocks
;
;	.IF ::TR_DATA_TILES4X4
;	TR_TILES	= TR_ms::tr_tiles
;	TR_ATTRS	= TR_ms::tr_attrs
;	.ENDIF	; TR_DATA_TILES4X4

draw_screen_nametable:
	; IN: X - a number of tiles in width

	; data_addr	- PPU address to draw tiles
	; _tmp_val	- tiles address

@drw_tiles_scr_loop:
	push_x

	; to save memory we will keep the addresses on the stack
	push_word _tmp_val
	push_word data_addr

	jsr _drw_tiles_col

	; get data_addr from the stack and add 4 to it to draw adjacent column
	pop_word data_addr

	.IF ::TR_DATA_TILES4X4
	lda #$04			; the number of CHRs in the tile
	.ELSE
	lda #$02			; the number of CHRs in the tile
	.ENDIF; TR_DATA_TILES4X4
	add_a_to_word data_addr

	; get _tmp_val from the stack and add tr_htiles to it
	pop_word _tmp_val

	lda TR_HTILES	;tr_htiles
	add_a_to_word _tmp_val

	pop_x
	dex
	beq @exit

	jmp @drw_tiles_scr_loop

@exit:
	rts

_drw_tiles_col:

	; set address to transfer attrs to PPU
	set_ppu_addr

	; draw tiles column
	ldy #$00
	ldx #SCR_HTILES_CNT

@drw_tiles_col_loop:
	push_x

	; read first tile
	push_y				;<----

	lda (<_tmp_val), y

	.IF ::TR_DATA_TILES4X4
	jsr _draw_tile
	.ELSE
	jsr _draw_block_a
	.ENDIF; TR_DATA_TILES4X4

	pop_y

	addr_add_32 data_addr
	set_ppu_addr

	iny					;<----

	pop_x
	dex
	bne @drw_tiles_col_loop

	rts

	.IF ::TR_DATA_TILES4X4
_draw_tile:
	; A - tile index
	; PPU address already set

	; A *= 4
	mul4_a_xy

	add_word_to_xy TR_TILES				; now XY contain tile's blocks address
	stx inner_vars::_tile_addr
	sty inner_vars::_tile_addr + 1

	ldy #$00
	lda (<inner_vars::_tile_addr), y		; 1 block
	tax
	iny
	push_y
	lda (<inner_vars::_tile_addr), y		; 2 block
	tay

	jsr _draw_blocks_xy

	pop_y

	iny
	lda (<inner_vars::_tile_addr), y		; 3 block
	tax
	iny
	lda (<inner_vars::_tile_addr), y		; 4 block
	tay

	; data_addr += 32
	addr_add_32 data_addr
	; set PPU address to draw blocks
	set_ppu_addr

;	jmp _draw_blocks_xy

	; IN: XY - the blocks offset
_draw_blocks_xy:

	; sequential drawing of 2 blocks

	push_x
	push_y

	txa
	mul4_a_xy

	add_word_to_xy TR_BLOCKS			; now XY contain an address of blocks
	stx inner_vars::_block_addr
	sty inner_vars::_block_addr + 1

	ldy #$00
	lda (<inner_vars::_block_addr), y
	sta $2007
	iny
	lda (<inner_vars::_block_addr), y
	sta $2007

	pop_y
	pop_x

	tya
	mul4_a_xy

	add_word_to_xy TR_BLOCKS			; XY contain an address of blocks
	stx _tmp_val2
	sty _tmp_val2 + 1

	ldy #$00
	lda (<_tmp_val2), y
	sta $2007
	iny
	lda (<_tmp_val2), y
	sta $2007

	; data_addr += 32
	addr_add_32 data_addr
	; set PPU address to draw blocks
	set_ppu_addr

	ldy #$02
	lda (<inner_vars::_block_addr), y
	sta $2007
	iny
	lda (<inner_vars::_block_addr), y
	sta $2007

	ldy #$02
	lda (<_tmp_val2), y
	sta $2007
	iny
	lda (<_tmp_val2), y
	sta $2007

	rts

	.ELSE; TR_DATA_TILES4X4

	; IN: A - block id
_draw_block_a:

	; draw block

	mul4_a_xy

	add_word_to_xy TR_BLOCKS			; now XY contain an address of blocks
	stx inner_vars::_block_addr
	sty inner_vars::_block_addr + 1

	ldy #$00
	lda (<inner_vars::_block_addr), y
	sta $2007
	iny
	lda (<inner_vars::_block_addr), y
	sta $2007

	; data_addr += 32
	addr_add_32 data_addr
	; set PPU address to draw blocks
	set_ppu_addr

	ldy #$02
	lda (<inner_vars::_block_addr), y
	sta $2007
	iny
	lda (<inner_vars::_block_addr), y
	sta $2007

	rts

	.ENDIF	; TR_DATA_TILES4X4

	; IN: X - a number of tiles in width
	; data_addr	- PPU attrs address
	; _tmp_val	- tiles address

draw_screen_attrs:

@drw_attrs_scr_loop:
	push_x

	; to save memory we will keep the addresses on the stack
	push_word _tmp_val
	push_word data_addr

	; set PPU address to draw attrs
	set_ppu_addr

	; draw a column of attrs
	ldy #$00

	ldx #SCR_HATTRS_CNT

@drw_attrs_col_loop:
	push_x

	; get first tile
	push_y				;<----
	lda (<_tmp_val), y

	.IF ::TR_DATA_TILES4X4
	tay
	lda (<TR_ATTRS), y		; read attrs
	.ENDIF; TR_DATA_TILES4X4

	sta $2007

	addr_add_8 data_addr
	set_ppu_addr

	pop_y
	iny				;<----

	pop_x
	dex
	bne @drw_attrs_col_loop

	; get the data_addr from the stack and add 1 to it to draw adjacent column
	pop_word data_addr
	inc data_addr

	; get the _tmp_val from the stack and add tr_htiles to it
	pop_word _tmp_val

	.IF ::TR_DATA_TILES4X4
	lda TR_HTILES	;tr_htiles
	.ELSE
	lda TR_HATTRS_CNT
	.ENDIF; TR_DATA_TILES4X4

	add_a_to_word _tmp_val

	pop_x
	dex
	beq @exit

	jmp @drw_attrs_scr_loop

@exit:

	rts

	.ENDIF	; TR_SCROLLED_MAP

	.endscope