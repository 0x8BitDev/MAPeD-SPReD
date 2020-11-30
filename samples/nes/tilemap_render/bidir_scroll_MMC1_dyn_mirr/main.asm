;###############################################
;
; Copyright 2018-2020 0x8BitDev ( MIT license )
;
;###############################################
;
; Screen to screen scroller example
;

.debuginfo	-	; Generate debug info ( if '+' then ld65 -Ln symbols.txt )

.define TR_DYNAMIC_MIRRORING	1

.segment "HDR"

INES_MAPPER 	= 1 ; MMC1
INES_MIRROR 	= 0 ; 0 = Horizontal mirroring, 1 = Vertical mirroring
INES_SRAM   	= 0 ; 1 = "Battery" and other non-volatile memory at $6000-7FFF
INES_TRAINER	= 0 ; 1 = 512-byte Trainer
INES_4SCR	= 0 ; 1 = Hard-wired four screen mode

.byte "NES", $1A
.byte $02	; 16K PRG count
.byte $01	; 8K CHR count
.byte INES_MIRROR | ( INES_SRAM << 1 ) | ( INES_TRAINER << 2 ) | ( INES_4SCR << 3 ) | ( ( INES_MAPPER & $0f ) << 4 )
.byte ( INES_MAPPER & $f0 )
.byte $00, $00, $00, $00, $00, $00, $00, $00


.segment "DATA"

	.include "data/tilemap.asm"

.segment "CODE"

	.include "../../common/common.asm"
	.include "../../common/mmc1.asm"
	.include "../../common/init.asm"
	.include "../../common/ppu.asm"
	.include "../../common/jpad.asm"
	.include "../../common/anm.asm"

	.include "../../common/rle.asm"
	.include "../../common/tilemap_render_MODE_bidir_scroll.asm"


MMC1_MIRROR_VERT	= $02
MMC1_MIRROR_HORIZ	= $03
MMC1_CONFIG		= %00011100	; separate 4 KB CHR, switch 16 KB bank at $8000-$c000

mmc1_mirroring_vertical:
	lda #( MMC1_CONFIG | MMC1_MIRROR_VERT )
	jmp mmc1_config_write

mmc1_mirroring_horizontal:
	lda #( MMC1_CONFIG | MMC1_MIRROR_HORIZ )
	jmp mmc1_config_write

RESET:

	init_hardware_clear_mem

	; MMC1 init

	mmc1_reset

	; switch separate 4 KB CHR, switch 16 KB bank at $8000-$c000, vertical mirroring
	jsr mmc1_mirroring_vertical

	; bank 0 $8000-$c000
	lda #$00
	jsr mmc1_prg_bank_write	

	; clean up PPU address registers
	lda #$00
	sta $2006
	sta $2006

	; clean up scroll registers
	sta $2005
	sta $2005

; tilemap render init

	load_data_ptr tilemap_Plts,		TR_sts::tr_palettes

	.IF TR_DATA_TILES4X4
	load_data_ptr tilemap_Tiles,		TR_sts::tr_tiles
	load_data_ptr tilemap_TilesOffs,	TR_sts::tr_tiles_offs
	load_data_ptr tilemap_Attrs,		TR_sts::tr_attrs
	.ELSE
	load_data_ptr tilemap_AttrsScr,		TR_sts::tr_attrs_scr
	.ENDIF	; TR_DATA_TILES4X4

	load_data_ptr tilemap_TilesScr,		TR_sts::tr_tiles_scr

	load_data_ptr tilemap_BlocksPropsOffs,	TR_sts::tr_blocks_props_offs
	load_data_ptr tilemap_Blocks,		TR_sts::tr_blocks

	.IF MAP_CHECK(MAP_FLAG_LAYOUT_ADJACENT_SCR_INDS)
	load_data_ptr Lev0_ScrArr, TR_utils::tr_screens_ptr_arr
	.ENDIF	; MAP_FLAG_LAYOUT_ADJACENT_SCR_INDS

	ldx Lev0_StartScr
	ldy Lev0_StartScr + 1

	jsr TR_sts::init_draw

	lda #%10010000			; NMI, background tiles from Pattern Table 1, 8x8
	jsr ppu_set_2000

	lda #%00001110			; enable background drawing, NO CLIPPING for sprites and background
	jsr ppu_set_2001

	jpad1_init

;-----------------------------

forever:

	jsr TR_sts::update_jpad

	; waiting for the next frame
	lda #$01
	SKIP_FRAMES

	jmp forever

NMI:
	push_FAXY

	jsr TR_utils::need_draw
	beq nmi_exit

	jsr TR_utils::buff_apply

nmi_exit:

	jsr TR_sts::update_scroll_reg

	jsr TR_sts::update_nametable

	DEC_FRAMES_CNT

	pop_FAXY
	rti

IRQ:
	rti	


.segment "VECTORS"

	.word NMI		; NMI routine ptr, happens once per frame if enabled
	.word RESET		; when the processor turns on and a reset occurs
	.word IRQ

; *** CHR BANKS ***

.segment "CHR1"
	; 4 KB
	.incbin "data/tilemap_chr0.bin"

.segment "CHR2"
	; 4 KB
	.incbin "data/tilemap_chr1.bin"
	
; *** END OF CHR BANKS ***