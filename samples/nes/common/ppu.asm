;###############################################
;
; Copyright 2018-2019 0x8BitDev ( MIT license )
;
;###############################################
;
; PPU related macroses and functions
;

; The PPU control registers
; PPUCTRL ($2000)
; 7  bit  0
; ---- ----
; VPHB SINN
; |||| ||||
; |||| ||++- Base nametable address
; |||| ||    (0 = $2000; 1 = $2400; 2 = $2800; 3 = $2C00)
; |||| |+--- VRAM address increment per CPU read/write of PPUDATA
; |||| |     (0: add 1, going across; 1: add 32, going down)
; |||| +---- Sprite pattern table address for 8x8 sprites
; ||||       (0: $0000; 1: $1000; ignored in 8x16 mode)
; |||+------ Background pattern table address (0: $0000; 1: $1000)
; ||+------- Sprite size (0: 8x8 pixels; 1: 8x16 pixels)
; |+-------- PPU master/slave select
; |          (0: read backdrop from EXT pins; 1: output color on EXT pins)
; +--------- Generate an NMI at the start of the
;            vertical blanking interval (0: off; 1: on)
;
; PPUMASK ($2001)
; 76543210
; ||||||||
; |||||||+- Grayscale (0: normal color; 1: AND all palette entries
; |||||||   with 0x30, effectively producing a monochrome display;
; |||||||   note that colour emphasis STILL works when this is on!)
; ||||||+-- Disable background clipping in leftmost 8 pixels of screen
; |||||+--- Disable sprite clipping in leftmost 8 pixels of screen
; ||||+---- Enable background rendering
; |||+----- Enable sprite rendering
; ||+------ Intensify reds (and darken other colors)
; |+------- Intensify greens (and darken other colors)
; +-------- Intensify blues (and darken other colors)
;
; PPUSTATUS ($2002)
; 7  bit  0
; ---- ----
; VSO. ....
; |||| ||||
; |||+-++++- Least significant bits previously written into a PPU register
; |||        (due to register not being updated for this address)
; ||+------- Sprite overflow. The intent was for this flag to be set
; ||         whenever more than eight sprites appear on a scanline, but a
; ||         hardware bug causes the actual behavior to be more complicated
; ||         and generate false positives as well as false negatives; see
; ||         PPU sprite evaluation. This flag is set during sprite
; ||         evaluation and cleared at dot 1 (the second dot) of the
; ||         pre-render line.
; |+-------- Sprite 0 Hit.  Set when a nonzero pixel of sprite 0 overlaps
; |          a nonzero background pixel; cleared at dot 1 of the pre-render
; |          line.  Used for raster timing.
; +--------- Vertical blank has started (0: not in vblank; 1: in vblank).
;            Set at dot 1 of line 241 (the line *after* the post-render
;            line); cleared after reading $2002 and at dot 1 of the
;            pre-render line.


; DMA flags
ppu_flag_dma_data_ready	= %00000001
ppu_flag_dma_free	= %00000010
ppu_flag_nmi_wait_frame	= %00000100
ppu_flag_nmi_upd_regs	= %00001000	; update 2000 and 2001 regs
ppu_flags_dma_mask	= ppu_flag_dma_data_ready | ppu_flag_dma_free
ppu_flags_dma_mask_inv	= ~ppu_flags_dma_mask

.segment "ZP"

_ppu_flags:		.res 1
_ppu_spr_pos:		.res 2	; X - LOW, Y - HIGH

; outer vars
ppu_2000:		.res 1	; $2000 last state
ppu_2001:		.res 1	; $2001 last state

.segment "OAM"

.segment "CODE"

; MACROSES

	.macro WAIT_FRAME
	ppu_set_flag ppu_flag_nmi_wait_frame
:	ppu_check_flag ppu_flag_nmi_wait_frame
	bne :-
	.endmacro

	.macro FRAME_PASSED
	ppu_res_flag ppu_flag_nmi_wait_frame
	.endmacro

	.macro set_ppu_addr
	lda $2002
	lda data_addr + 1
	sta $2006
	lda data_addr
	sta $2006
	.endmacro

; check DMA state
	.macro ppu_check_flag flag
	lda #flag
	bit _ppu_flags
	.endmacro

; set DMA flag
	.macro ppu_set_dma_state flag
	lda #<ppu_flags_dma_mask_inv
	and _ppu_flags
	sta _ppu_flags
	lda #flag
	ora _ppu_flags
	sta _ppu_flags
	.endmacro

	.macro ppu_set_flag flag
	lda _ppu_flags
	ora #flag
	sta _ppu_flags
	.endmacro

	.macro ppu_res_flag flag
	lda _ppu_flags
	and #<~flag
	sta _ppu_flags
	.endmacro

	.macro ppu_check_and_update_regs
	ppu_check_flag ppu_flag_nmi_upd_regs
	beq @skip_upd_regs

	lda ppu_2000
	sta $2000

	lda ppu_2001
	sta $2001

	ppu_res_flag ppu_flag_nmi_upd_regs

@skip_upd_regs:
	.endmacro

	.macro set_ppu_32_inc
	lda ppu_2000
	ora #%00000100
	sta $2000
	.endmacro

	.macro set_ppu_1_inc
	lda ppu_2000
	and #%11111011
	sta $2000
	.endmacro

; waiting for a new frame
vblankwait:
@vbwait: 	
	bit $2002
  	bpl @vbwait
  	rts

ppu_DMA_transf_256b_0x0200:

; transfer prepared sprites data which we stored at $0200 to PPU
; WARNING: this way you can transfer 256 bytes only!

	lda #$00
	sta $2003	; save the low byte of the addr
	lda #$02
	sta $4014	; save the high byte of the sprites data
			; and transfer it immediately
	rts

; IN: X - low byte of PPU addr
;     Y - high byte of PPU addr

ppu_load_palettes:
	lda $2002	; read PPU state to reset HIGH\LOW byte latch
	sty $2006	; save the high byte of the addr
	stx $2006	; save the low byte of the addr
	ldy #$00
@load_palettes_loop:
	lda (<data_addr), y	; load palette value
	sta $2007		; save it to PPU
	iny
	cpy data_size
	bne @load_palettes_loop	; if X != 32 -> continue
	rts

; *** clear 256 bytes at 0x0200 ***

clear_sprite_mem_256b_0x0200:
	ldx #$00
	lda #$ff
@clear_sprite_mem_256b_0x0200_loop:	
	sta $0200, x
	
	inx
	inx
	inx
	inx

	bne @clear_sprite_mem_256b_0x0200_loop

	rts

; load sprite data to $0200 ( MAX 256 bytes! )
; IN:
;	data_addr
;	data_size
;	X - x pos
;	Y - y pos

ppu_load_sprite_0x0200:
	
	stx _ppu_spr_pos
	sty _ppu_spr_pos + 1

	ldy #$00
@load_sprite_loop:

	; Y pos
	lda (<data_addr), y

	clc
	adc _ppu_spr_pos + 1

	sta $0200, y
	iny

	lda (<data_addr), y
	sta $0200, y
	iny

	lda (<data_addr), y
	sta $0200, y
	iny

	; X pos
	lda (<data_addr), y

	clc
	adc _ppu_spr_pos

	sta $0200, y
	iny

	cpy data_size
	bne @load_sprite_loop
	rts

; *** transfer gfx data of any length to PPU ***
; IN:
;	X - LOW byte of PPU addr
;	Y - HIGH byte of PPU addr
;	data_addr
;	data_size

ppu_load_data:
  	lda $2002				; read PPU state to reset the HIGH\LOW byte latch
  	sty $2006
  	stx $2006

	ldy #$00
	ldx data_size + 1
	beq @load_final

@load_pages:
	lda (<data_addr), y
	sta $2007
	iny
	bne @load_pages
	inc data_addr + 1
	dex
	bne @load_pages

@load_final:
	ldx data_size
	beq @load_done

	ldy #$00
@load_loop:
	lda (<data_addr), y
	sta $2007
	iny
	dex
	bne @load_loop

@load_done:
	rts

; *** transfer gfx data to PPU ***
; IN:
; 	A - a number of 256 bytes pages
;	X - LOW byte of PPU addr
;	Y - HIGH byte of PPU addr
;	data_addr

ppu_load_data_opt:
	pha
  	lda $2002			; read PPU state to reset the HIGH\LOW byte latch
  	sty $2006
  	stx $2006

  	ldy #$00
  	pla
  	tax

@load_data_opt_loop:
  	lda (<data_addr), y   		; copy from source pointer
  	sta $2007            		; to PPU RAM area
  	iny
  	bne @load_data_opt_loop   	; loop 256 times
  	inc data_addr + 1    		; then increment the high address byte
  	dex                  		
  	bne @load_data_opt_loop

	rts

; IN: A - nametable index (0-3)
ppu_calc_nametable_addr:

	asl a
	asl a
	clc
	adc #$20
	sta data_addr + 1
	lda #$00
	sta data_addr

	rts