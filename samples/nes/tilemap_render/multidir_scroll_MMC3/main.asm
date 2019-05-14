;###############################################
;
; Copyright 2018-2019 0x8BitDev ( MIT license )
;
;###############################################
;
; Multidirectional scroller example
;

.segment "HDR"	

.define TR_MIRRORING_VERTICAL 0	; 1 - vertical, 0 - 4-screen mirroring

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

	.include "tilemap.asm"


.segment "CODE"

	.include "../../common/common.asm"
	.include "../../common/mmc3.asm"
	.include "../../common/ppu.asm"
	.include "../../common/init.asm"
	.include "../../common/jpad.asm"
	
	.include "../../common/rle.asm"
	.include "tilemap_render_MODE_multidir_scroll.asm"

RESET:

	init_hardware_clear_mem

; MMC3 init

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

	lda #%10010000		; NMI, background tiles from Pattern Table 1, 8x8
	jsr ppu_set_2000

	lda #%00001110		; enable background drawing, NO CLIPPING for sprites and background
	jsr ppu_set_2001

	jpad1_init

;-----------------------------

	; reset the scroll registers
	lda #$00
	sta $2005
	sta $2005

forever:

	jsr TR_ms::update_jpad

	; waiting for the next frame
	lda #$01
	SKIP_FRAMES

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

	jsr TR_ms::update_scroll_reg

	jsr TR_ms::update_nametable

	DEC_FRAMES_CNT

	pop_FAXY
	rti

IRQ:
	rti

.segment "VECTORS"

	.word NMI		; NMI routine ptr, happens once per frame if enabled
	.word RESET		; when the processor turns on and a reset occurs
	.word IRQ

.segment "CHR"

; *** CHR BANKS ***

	; банки: 0, 1, 2, 3, 4, 5, 6, 7
	.incbin "data/tilemap_Lev0_CHR.bin"

; *** END OF CHR BANKS ***