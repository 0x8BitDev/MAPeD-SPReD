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

; TR_MIRRORING_HORIZONTAL 1 or 0 (1 - horizontal, 0 - 4-screen mirroring) passed as command line parameter
;
	.IF mirror_horiz
.define TR_MIRRORING_HORIZONTAL 1
	.warning "[horizontal mirroring]"
	.ELSE
.define TR_MIRRORING_HORIZONTAL 0
	.warning "[4-screen mirroring]]"
	.ENDIF

INES_MAPPER 	= 4 ; MMC3
INES_SRAM   	= 0 ; 1 = "Battery" and other non-volatile memory at $6000-7FFF
INES_TRAINER	= 0 ; 1 = 512-byte Trainer

	.IF !TR_MIRRORING_HORIZONTAL
INES_MIRROR 	= 0	; 0 = Horizontal mirroring, 1 = Vertical mirroring
INES_4SCR	= 1	; 1 = Hard-wired four screen mode
	.ELSE
INES_MIRROR 	= !TR_MIRRORING_HORIZONTAL ; 0 = Horizontal mirroring, 1 = Vertical mirroring
INES_4SCR	= 0
	.ENDIF	; !TR_MIRRORING_HORIZONTAL

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
	.include "../../common/tilemap_render_MODE_multidir_scroll_4scr_horiz_mirr.asm"

	.IF TR_MIRRORING_HORIZONTAL
_right_blank_col:
	.byte $00, $00, $00, $F8
	.byte $10, $00, $00, $F8
	.byte $20, $00, $00, $F8
	.byte $30, $00, $00, $F8
	.byte $40, $00, $00, $F8
	.byte $50, $00, $00, $F8
	.byte $60, $00, $00, $F8
	.byte $70, $00, $00, $F8
	.byte $80, $00, $00, $F8
	.byte $90, $00, $00, $F8
	.byte $A0, $00, $00, $F8
	.byte $B0, $00, $00, $F8
	.byte $C0, $00, $00, $F8
	.byte $D0, $00, $00, $F8
	.byte $E0, $00, $00, $F8
_right_blank_col_end:

_right_blank_col_size:
	.word _right_blank_col_end - _right_blank_col

	.ENDIF ;TR_MIRRORING_HORIZONTAL

RESET:

	init_hardware_clear_mem

; MMC3 init

	.IF ::TR_DATA_RLE
	jsr mmc3_enable_wram
	.ENDIF	

	.IF TR_MIRRORING_HORIZONTAL
	jsr mmc3_horiz_mirror
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

	.IF TR_MIRRORING_HORIZONTAL

	; save sprite palette color

	ldx #$11		; low
	ldy #$3f		; high

	lda $2002	; read PPU state to reset HIGH\LOW byte latch
	sty $2006	; save the high byte of the addr
	stx $2006	; save the low byte of the addr

	ldy #$00
	lda (<TR_ms::tr_palette), y
	sta $2007	

	; transfer clipping column's sprite data

	jsr clear_sprite_mem_256b_0x0200

	load_data_ptr _right_blank_col, data_addr
	load_data_word _right_blank_col_size, data_size

	ldx #$00
	ldy #$00

	jsr ppu_load_sprite_0x0200
	jsr ppu_DMA_transf_256b_0x0200
	.ENDIF ;TR_MIRRORING_HORIZONTAL

; load palette
; WARNING: palette can be loaded during VBlank only!

	load_data_word TR_ms::tr_palette, data_addr
	lda #$10		; first 16 colors for background
	sta data_size
	; PPU address
	ldx #$00		; low
	ldy #$3f		; high

	jsr ppu_load_palettes

	.IF TR_MIRRORING_HORIZONTAL
	lda #%10110000		; NMI, background tiles from Pattern Table 1, 8x16
	.ELSE
	lda #%10010000		; NMI, background tiles from Pattern Table 1, 8x8
	.ENDIF ;TR_MIRRORING_HORIZONTAL
	jsr ppu_set_2000

	.IF TR_MIRRORING_HORIZONTAL
	lda #%00011000		; enable background drawing, CLIPPING for sprites and background
	.ELSE
	lda #%00001110		; enable background drawing, NO CLIPPING for sprites and background
	.ENDIF ;TR_MIRRORING_HORIZONTAL
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

	; banks: 0, 1, 2, 3, 4, 5, 6, 7
	.incbin "data/tilemap_Lev0_CHR.bin"

	.IF TR_MIRRORING_HORIZONTAL

	; two black CHRs for the left clipping column

	.ALIGN 4096		; align sprites CHR data

	.REPEAT 8
	.byte $ff
	.ENDREPEAT
	.REPEAT 8
	.byte $00
	.ENDREPEAT
	.REPEAT 8
	.byte $ff
	.ENDREPEAT
	.REPEAT 8
	.byte $00
	.ENDREPEAT
	.ENDIF ;TR_MIRRORING_HORIZONTAL

; *** END OF CHR BANKS ***