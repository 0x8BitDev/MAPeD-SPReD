;###############################################
;
; Copyright 2018-2020 0x8BitDev ( MIT license )
;
;###############################################
;
; Multidirectional scroller example
;

.debuginfo	-	; Generate debug info ( if '+' then ld65 -Ln symbols.txt )

.segment "HDR"	

; TR_MIRRORING_VERTICAL 1 or 0 (1 - vertical, 0 - 4-screen mirroring) passed as command line parameter
;

	.IF mmc3_status_bar
.define TR_MMC3_IRQ_STATUS_BAR	1
	.ELSE
.define TR_MMC3_IRQ_STATUS_BAR	0
	.ENDIF

	.IF mirror_vert
.define TR_MIRRORING_VERTICAL	1
	.warning "[vertical mirroring]"
	.ELSE
.define TR_MIRRORING_VERTICAL 	0
	.warning "[4-screen mirroring]]"
	.ENDIF

INES_MAPPER 	= 4 ; MMC3
INES_SRAM   	= 0 ; 1 = "Battery" and other non-volatile memory at $6000-7FFF
INES_TRAINER	= 0 ; 1 = 512-byte Trainer

	.IF !TR_MIRRORING_VERTICAL
INES_MIRROR 	= 0	; 0 = Horizontal mirroring, 1 = Vertical mirroring
INES_4SCR	= 1	; 1 = Hard-wired four screen mode
	.ELSE
INES_MIRROR 	= TR_MIRRORING_VERTICAL ; 0 = Horizontal mirroring, 1 = Vertical mirroring
INES_4SCR	= 0
	.ENDIF	; !TR_MIRRORING_VERTICAL

.byte "NES", $1A
.byte $02	; 16K PRG count
.byte $01	; 8K CHR count
.byte INES_MIRROR | ( INES_SRAM << 1 ) | ( INES_TRAINER << 2 ) | ( INES_4SCR << 3 ) | ( ( INES_MAPPER & $0f ) << 4 )
.byte ( INES_MAPPER & %11110000 )
.byte $00, $00, $00, $00, $00, $00, $00, $00


.segment "WRAM"

; optional extra RAM

.segment "MAP_DATA"

	.include "../multidir_scroll_MMC3_4scr_mirr/data/tilemap.asm"


.segment "CODE"

	.include "../../common/common.asm"
	.include "../../common/mmc3.asm"
	.include "../../common/ppu.asm"
	.include "../../common/init.asm"
	.include "../../common/jpad.asm"
	
	.include "../../common/rle.asm"
	.include "../../common/tilemap_render_MODE_multidir_scroll_4scr_vert_mirr.asm"

RESET:

	init_hardware_clear_mem

; MMC3 init

	mmc3_IRQ_disable

	.IF ::TR_DATA_RLE
	jsr mmc3_enable_wram
	.ENDIF	

	.IF TR_MIRRORING_VERTICAL
	jsr mmc3_vert_mirror
	.ENDIF

	; bank 0 8000-9fff - swappable!
	; bank 1 a000-bfff - swappable!
	; bank 2 c000-dfff - fixed!
	; bank 3 e000-ffff - fixed!

	ldx #%10000000		; CHR: 	two  2 KB banks at $1000-$1FFF,
				; four 1 KB banks at $0000-$0FFF
	ldy #$00		; ...
	jsr mmc3_exec_command

	; clean up the PPU address registers
	lda #$00
	sta $2006
	sta $2006			

; tilemap render init

	.IF ::TR_DATA_TILES4X4

	load_data_ptr Lev0_Tiles, 	TR_ms::tr_tiles
	load_data_ptr Lev0_Attrs, 	TR_ms::tr_attrs

	.ELSE

	load_data_ptr Lev0_AttrsMap, TR_ms::tr_attrs_map

	lda #Lev0_HScrCnt * ::SCR_HATTRS_CNT	; number of attrs in a map in height
	sta TR_ms::tr_hattrs_cnt

	.ENDIF; TR_DATA_TILES4X4

	load_data_ptr Lev0_Blocks, 	TR_ms::tr_blocks
	load_data_ptr Lev0_Props, 	TR_ms::tr_props
	load_data_ptr Lev0_Map, 	TR_ms::tr_map
	load_data_ptr Lev0_MapTbl,	TR_ms::tr_map_tbl
	load_data_ptr Lev0_Palette, 	TR_ms::tr_palette

	lda #Lev0_WScrCnt
	sta TR_ms::tr_wscr_cnt

	lda #Lev0_HScrCnt
	sta TR_ms::tr_hscr_cnt

	lda #Lev0_WTilesCnt
	sta TR_ms::tr_wtiles

	lda #Lev0_HTilesCnt
	sta TR_ms::tr_htiles

	lda #Lev0_StartScr
	jsr TR_ms::init_draw

	set_ppu_1_inc

; load palette
; WARNING: palette can be loaded during VBlank only!

	load_data_word TR_ms::tr_palette, data_addr
	lda #$10		; first 16 colors for background
	sta data_size
	; PPU address
	ldx #$00		; low
	ldy #$3f		; high

	jsr ppu_load_palettes

	lda #%10010000			; NMI, background tiles from Pattern Table 1, 8x8
	sta ppu_2000
	sta $2000			; enable NMI at startup

	lda #%00001110			; enable background drawing, NO CLIPPING for sprites and background
	sta ppu_2001

	ppu_set_flag ppu_flag_nmi_upd_regs

	jpad1_init

;-----------------------------

	; reset the scroll registers
	lda #$00
	sta $2005
	sta $2005

	.IF TR_MMC3_IRQ_STATUS_BAR
	cli
	.ENDIF ;TR_MMC3_IRQ_STATUS_BAR

forever:

	jsr TR_ms::update_jpad

	WAIT_FRAME

	.IF TR_MMC3_IRQ_STATUS_BAR

	; update PPUCTRL and nametable changed by IRQ routine
	jsr TR_ms::update_nametable

	mmc3_IRQ_reload #$df	; scan-line on which the IRQ procedure executes
	mmc3_IRQ_enable

	.ENDIF ;TR_MMC3_IRQ_STATUS_BAR

_bg_drw_wait_loop:
	jsr TR_utils::need_draw
	bne _bg_drw_wait_loop

	jmp forever

NMI:
	push_FAXY

	jsr TR_utils::need_draw
	beq nmi_exit

	jsr TR_utils::buff_apply

nmi_exit:

	ppu_check_and_update_regs

	.IF TR_MMC3_IRQ_STATUS_BAR
	jsr update_scroll_reg_shift_y_8pix
	.ELSE
	jsr TR_ms::update_scroll_reg
	.ENDIF ;TR_MMC3_IRQ_STATUS_BAR

	jsr TR_ms::update_nametable

	FRAME_PASSED

	pop_FAXY
	rti

IRQ:
	.IF TR_MMC3_IRQ_STATUS_BAR
	mmc3_IRQ_disable

	pha
	push_x

	; some delay to reduce scan-line glitches
	ldx #$0f
@delay_loop:
	dex
	bne @delay_loop

	; in real project you need to reserve an empty
	; CHR bank to make an empty status bar
	lda ppu_2000
	and #<~%00010000		; switch to Table 0 cause it is empty
	sta $2000

	pop_x
	pla
	.ENDIF ;TR_MMC3_IRQ_STATUS_BAR
	rti

	.IF TR_MMC3_IRQ_STATUS_BAR
update_scroll_reg_shift_y_8pix:

	lda TR_ms::inner_vars::_tr_upd_flags
	and #TR_ms::inner_vars::TR_UPD_FLAG_SCROLL
	beq @_skip_upd_scroll

	; clean up PPU address registers
	lda #$00
	sta $2006
	sta $2006			

	lda TR_ms::inner_vars::_tr_pos_x
	sta $2005

	; shift Y pos by 8 pixels
	lda TR_ms::inner_vars::_tr_pos_y
	clc
	adc #$08
	tax

	; check Y overflow
	and #%11110000
	cmp #%11110000
	bne @cont

	txa
	and #%00000111
	tax
@cont:
	txa
	sta $2005

@_skip_upd_scroll:

	rts
	.ENDIF ;TR_MMC3_IRQ_STATUS_BAR

.segment "VECTORS"

	.word NMI		; NMI routine ptr, happens once per frame if enabled
	.word RESET		; when the processor turns on and a reset occurs
	.word IRQ

.segment "CHR"

; *** CHR BANKS ***

	; banks: 0, 1, 2, 3, 4, 5, 6, 7
	.incbin "../multidir_scroll_MMC3_4scr_mirr/data/tilemap_Lev0_CHR.bin"

; *** END OF CHR BANKS ***