;###############################################
;
; Copyright 2018-2019 0x8BitDev ( MIT license )
;
;###############################################
;
; Static screens switching example
;

.segment "HDR"	

INES_MAPPER 	= 5 ; MMC5
INES_MIRROR 	= 0 ; 0 = Horizontal mirroring, 1 = Vertical mirroring
INES_SRAM   	= 0 ; 1 = "Battery" and other non-volatile memory at $6000-7FFF
INES_TRAINER	= 0 ; 1 = 512-byte Trainer
INES_4SCR	= 0 ; 1 = Hard-wired four screen mode

.byte "NES", $1A
.byte $02	; 16K PRG count
.byte $02	; 8K CHR count
.byte INES_MIRROR | ( INES_SRAM << 1 ) | ( INES_TRAINER << 2 ) | ( INES_4SCR << 3 ) | ( ( INES_MAPPER & $0f ) << 4 )
.byte ( INES_MAPPER & %11110000 )
.byte $00, $00, $00, $00, $00, $00, $00, $00


.segment "ExRAM"

; expansion RAM

.segment "WRAM"

; extra RAM
; first PRG RAM 8Кб

.segment "DATA"

; second PRG ROM 32Кб
	.include "tilemap.asm"

.segment "CODE"

	.include "../../common/common.asm"
	.include "../../common/init.asm"
	.include "../../common/ppu.asm"
	.include "../../common/jpad.asm"
	
	.include "../common/rle.asm"
	.include "tilemap_render_MODE_static_screens.asm"

mmc5_CHR_bankswitching:
	sta $5123
	rts

RESET:

	init_hardware_clear_mem

; MMC5 init

	; PRG mode 0
	; CPU $6000-$7FFF: 8 KB switchable PRG RAM bank
	; CPU $8000-$FFFF: 32 KB switchable PRG ROM bank	

	; PRG mode 1
	; CPU $6000-$7FFF: 8 KB switchable PRG RAM bank
	; CPU $8000-$BFFF: 16 KB switchable PRG ROM/RAM bank
	; CPU $C000-$FFFF: 16 KB switchable PRG ROM bank

	lda #01
	sta $5100

	; Switch data bank if #01 mode ($5100) is active
	lda #%10000000
	sta $5115

	; Enable writing to WRAM ( just for example )
;	lda $02
;	sta $5102
;	lda $01
;	sta $5103

	; CHR mode 1 - 4Kb CHR pages
	lda #01
	sta $5101

	.IF MAP_CHECK(MAP_FLAG_ATTRS_PER_CHR)
	; Extended RAM mode 1 - Use as extended attribute data (can also be used as extended nametable)
	lda #01
	.ELSE
	; Extended RAM mode 2 - Use as ordinary RAM
	lda #02
	.ENDIF	; MAP_FLAG_ATTRS_PER_CHR
	sta $5104

	; Nametable mapping (mirroring) - Single-screen CIRAM 0
	lda #$00
	sta $5105

	; Reset CHR pages high bits
	lda #$00
	sta $5130

	; Disable vertical split scroll
	lda #$00
	sta $5200
	sta $5201
	sta $5202
	; Disable scanline IRQ
	sta $5204

	; Switch zero CHR bank at $1000 PPU address
;	lda #$00
;	sta $5127

	; Set zero CHR bank by default
	.IF !( MAP_CHECK(MAP_FLAG_ATTRS_PER_CHR) )
	lda #$00
	sta $5123
	.ENDIF	; MAP_FLAG_ATTRS_PER_CHR

	; reset scroll registers
	lda #$00
	sta $2005
	sta $2005

	; clean up PPU address registers
	sta $2006
	sta $2006

; tilemap render init

	load_data_ptr tilemap_PPUScr, 	TR_ss::tr_ppu_scr
	load_data_ptr tilemap_Plts,	TR_ss::tr_palettes

	.IF MAP_CHECK(MAP_FLAG_ATTRS_PER_CHR)
	load_data_ptr tilemap_AttrsScr, TR_ss::tr_attrs_scr
	.ENDIF	; MAP_FLAG_ATTRS_PER_CHR

	.IF MAP_CHECK(MAP_FLAG_LAYOUT_ADJACENT_SCR_INDS)
	load_data_ptr Lev0_ScrArr, TR_utils::tr_screens_ptr_arr
	.ENDIF	; MAP_FLAG_LAYOUT_ADJACENT_SCR_INDS

	ldx Lev0_StartScr
	ldy Lev0_StartScr + 1

	jsr TR_ss::init_screen

	lda #%10001000			; enable NMI, tiles from Pattern Table 0, sprites from Pattern Table 1
	jsr ppu_set_2000

	lda #%00001110			; enable background drawing, NO CLIPPING for sprites and background
	jsr ppu_set_2001

	jpad1_init

;-----------------------------

forever:

	jsr TR_ss::update_jpad

	jsr TR_ss::update_PPU

	.IF MAP_CHECK(MAP_FLAG_ATTRS_PER_CHR)
	jsr TR_ss::update_ExRAM
	.ENDIF	; MAP_FLAG_ATTRS_PER_CHR

	jsr ppu_get_2001
	sta $2001

	; reset scroll registers
	lda #$00
	sta $2005
	sta $2005

	; skip 10 frames to make some delay between screen switching
	lda #$0a
	SKIP_FRAMES

	jmp forever

NMI:
	push_FAXY

	DEC_FRAMES_CNT

	; ...

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
	.incbin "data/tilemap_chr0.bin"

.segment "CHR2"
	.incbin "data/tilemap_chr1.bin"

.segment "CHR3"
	.incbin "data/tilemap_chr2.bin"

.segment "CHR4"

; *** END OF CHR BANKS ***