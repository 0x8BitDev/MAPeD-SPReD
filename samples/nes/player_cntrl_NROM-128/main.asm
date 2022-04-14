;###############################################
;
; Copyright 2018-2022 0x8BitDev ( MIT license )
;
;###############################################
;
; Simple character controller
;

.segment "HDR"

INES_MAPPER 	= 0 ; NROM-128
INES_MIRROR 	= 0 ; 0 = Horizontal mirroring, 1 = Vertical mirroring
INES_SRAM   	= 0 ; 1 = "Battery" and other non-volatile memory at $6000-7FFF
INES_TRAINER	= 0 ; 1 = 512-byte Trainer
INES_4SCR	= 0 ; 1 = Hard-wired four screen mode

.byte "NES", $1A
.byte $01	; 16K PRG count
.byte $01	; 8K CHR count
.byte INES_MIRROR | ( INES_SRAM << 1 ) | ( INES_TRAINER << 2 ) | ( INES_4SCR << 3 ) | ( ( INES_MAPPER & $0f ) << 4 )
.byte ( INES_MAPPER & $f0 )
.byte $00, $00, $00, $00, $00, $00, $00, $00


.segment "DATA"

	.include "player_anm.asm"

.segment "CODE"

	.include "../../common/common.asm"
	.include "../../common/mmc1.asm"
	.include "../../common/init.asm"
	.include "../../common/ppu.asm"
	.include "../../common/jpad.asm"
	.include "../../common/anm.asm"
	.include "../../common/character_cntrl.asm"

RESET:

	init_hardware_clear_mem

	; clean up PPU address registers
	lda #$00
	sta $2006
	sta $2006

	; clean up scroll registers
	lda #$00
	sta $2005
	sta $2005

; load palette
; WARNING: palette can be loaded during VBlank only!

	load_data_ptr dog_gfx_palette, data_addr
	lda #$10	; 16 indices for sprites
	sta data_size
	; PPU address
	ldx #$10	; low
	ldy #$3f	; high

	jsr ppu_load_palettes

	.IF DOG_GFX_SPR_MODE_8X16
	lda #%10110000			; enable NMI, sprites from Pattern Table 1, 8x16
	.ELSE
	lda #%10010000			; enable NMI, sprites from Pattern Table 1, 8x8
	.ENDIF
	sta ppu_2000
	sta $2000			; enable NMI at startup

	lda #%00011010			; enable drawing of sprites and background, sprites clipping
	sta ppu_2001

	ppu_set_flag ppu_flag_nmi_upd_regs

	; to avoid a zero tile drawing from uninitialized OAM memory
	jsr clear_sprite_mem_256b_0x0200
	jsr ppu_DMA_transf_256b_0x0200

	jpad1_init

; player init	

	ldx #$80	; X pos
	ldy #150	; Y pos

	lda #PLAYER_FLAG_DIR_RIGHT

	jsr player_init

;-----------------------------

	; set DMA ready flag by default
	ppu_set_dma_state ppu_flag_dma_free

forever:
	jsr jpad1_read_state

	jsr player_update

	ppu_check_flag ppu_flag_dma_free
	beq _skip_data_preparation		; if equal to zero -> NMI works

	; push an animation frame to the characters "pool"

	jsr clear_sprite_mem_256b_0x0200

	; preparing sprite data for transfer to PPU
	ldy #frame_data::gfx_ptr
	lda (<inner_vars::tmp_anm_addr), y
	sta data_addr
	ldy #frame_data::gfx_ptr + 1
	lda (<inner_vars::tmp_anm_addr), y
	sta data_addr + 1
	ldy #frame_data::data_size
	lda (<inner_vars::tmp_anm_addr), y
	sta data_size

	ldy #frame_data::chr_id
	lda (<inner_vars::tmp_anm_addr), y	; CHR bank index
	sta ppu_sprite_CHR_bank

	lda anm_pos_x
	tax
	lda anm_pos_y
	tay

	jsr ppu_load_sprite_0x0200

	; set data ready flag
	ppu_set_dma_state ppu_flag_dma_data_ready

_skip_data_preparation:

	WAIT_FRAME

	jmp forever

NMI:
	push_FAXY

	; check DMA ready flag
	ppu_check_flag ppu_flag_dma_data_ready
	beq nmi_exit

	jsr ppu_DMA_transf_256b_0x0200

	; reset DMA ready flag
	ppu_set_dma_state ppu_flag_dma_free

nmi_exit:

	ppu_check_and_update_regs

	FRAME_PASSED

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
	; 1 KB
	.incbin "data/dog_gfx_chr0.bin"
; *** END OF CHR BANKS ***