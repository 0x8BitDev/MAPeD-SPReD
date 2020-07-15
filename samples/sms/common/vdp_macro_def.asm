;###################################################################
;
; Copyright 2019-2020 0x8BitDev ( MIT license )
;
;###################################################################
;
; VDP macroses and definitions
;

; user defined VDP flags to control the VDP state

.define	VDP_FLAG_FREE		$01
.define	VDP_FLAG_READY		$02

.define	VDP_FLAGS_MASK		VDP_FLAG_FREE | VDP_FLAG_READY
.define VDP_FLAGS_MASK_INV	$ff ~ VDP_FLAGS_MASK

; check VDP state

.macro	VDP_CHECK_FLAG	; \1 - VDP state flag
	ld a, (VDP_STATE_FLAGS)
	and \1		; Z - no flag, otherwise NZ
.endm

; set VDP flag

.macro	VDP_SET_FLAG	; \1 - VDP state flag
	ld a, (VDP_STATE_FLAGS)
	and VDP_FLAGS_MASK_INV
	or \1
	ld (VDP_STATE_FLAGS), a
.endm

.macro	VDP_WAITING_FREE_STATE
-
	VDP_CHECK_FLAG VDP_FLAG_FREE
	jr z, -
.endm

.define VDP_CMD_STATUS_REG	$bf
.define VDP_CMD_DATA_REG	$be

.define	VDP_CMD_READ_RAM	%0000000000000000
.define	VDP_CMD_WRITE_RAM	%0100000000000000
.define	VDP_CMD_WRITE_REG	%1000000000000000
.define	VDP_CMD_WRITE_CLR	%1100000000000000

; REG0 and REG1 set the VDP display and interrupt mode.
;
; REG0

.define	VDPR0_FIXED				%00000110

.define	VDPR0_SPRITE_SHIFT_8PIX			%00001000
.define	VDPR0_HLINE				%00010000
.define	VDPR0_BLANK_LEFT_COLUMN			%00100000
.define	VDPR0_HORIZ_SCROLL_FIX_TOP_2_ROWS       %01000000
.define	VDPR0_VERT_SCROLL_FIX_RIGHT_8_COLS	%10000000

; REG1

.define VDPR1_FIXED				%10000000

.define VDPR1_SPRITES_8x16			%00000010
.define VDPR1_EXT_SCR_6_ROWS			%00001000	; Extend screen by 6 rows (i.e. to 30 rows) / never used in games (?!)
.define VDPR1_EXT_SCR_4_ROWS			%00010000	; Extend screen by 4 rows (i.e. to 28 rows) / never used in games (?!)
.define VDPR1_VBLANK				%00100000
.define VDPR1_DISPLAY_ON			%01000000

; VDPR1_VBLANK 	VDPR0_HLINE
; 0		1		Raster Line
; 1		1		VBLANK and Raster Line

; REG2 sets the base address for the screen map. It positions the 2048 screen map to one 
; of eight starting addresses, on 2048 byte boundaries, according to the following table:

.define	REG2_SCR_MAP_ADDR_3800	$ff	; <-- normal setting
.define	REG2_SCR_MAP_ADDR_3000	$fd
.define	REG2_SCR_MAP_ADDR_2800	$fb
.define	REG2_SCR_MAP_ADDR_2000	$f9
.define	REG2_SCR_MAP_ADDR_1800	$f7
.define	REG2_SCR_MAP_ADDR_1000	$f5
.define	REG2_SCR_MAP_ADDR_0800	$f3
.define	REG2_SCR_MAP_ADDR_0000	$f1

; REG3 always $ff
; REG4 always $ff

; REG5 controls the base address for the Sprite Attribute Table. This 256 byte table can be
; positioned at one of 64 starting addresses in Video RAM.
;
; 1 A13 A12 A11 A10 A9 A8 1
;
; The normal setting is $ff, which positions the Sprite Attribute Table at $3F00.

.define VDPR5_SPR_ATTR_TBL_3f00	$ff

; REG6 sets the base address for the sprite patterns.

.define VDPR6_SPR_TILES_FIRST_8K	$fb
.define VDPR6_SPR_TILES_SECOND_8K	$ff

; REG7 sets the border color. The border color is taken from the second bank of colors in 
; VDP Color RAM.
;
; 1 1 1 1 C3 C2 C1 C0
;

; REG8 sets the horizontal scroll value $00 - $ff

; REG9 sets the vertical scroll value $00 - $ff

; REG10 controls the Raster Line Interrupt - HBLANK (~10 microseconds)
; The value loaded into REG10 should be one less than the raster line you wish to "trigger" 
; the interrupt.
; For example, if you load REG10 with 94, a Raster Line Interrupt Request will be generated 
; every time the 95th horizontal line finishes it's scan.
;
; There are two special cases: $FF turns off the interrupt requests, and $00 gives a Raster 
; Line interrupt request for every horizontal line.
;
; There is a one interrupt delay between loading R10 and having the value take effect. For 
; example, if you set R10 for line 20, and respond to the interrupt by resetting R10 to 150,
; the very next Raster Line Interrupt will occur at 20, and all subsequent ones will occur 
; at 150.


; The Maskable Interrupt (INT)
;
; This interrupt is enabled with an "EI" instruction, and disabled with a "DI" instruction.
; The Mk3 hardware supports the "mode 1" interrupt only. The boot ROM executes the "IM 1" 
; instruction at power-on.
;
; In mode 1, activating the Z80A INT pin causes a Restart instruction to be executed at 
; location $38 if interrupts are enabled.
;
; This interrupt can be activated by two events in the VDP chip: VBLANK, H-LINE
;
; The VDP_CMD_STATUS_REG ($BF) need to be read in interrupt response routine.
;
; Reading the I/O part at $BF does two things. First, it clears the interrupt request line 
; from the VDP chip. Second, it provides VDP information as follows:
;
; bit 7 — 1: VBLANK Interrupt, 0: H-Line Interrupt [if enabled].
;
; bit 6 — 1: 9 sprites on a raster line.
;
; bit 5 — 1: Sprite Collision.
;
; bits 4-0 (no significance).
;
; The VDP VBLANK and H-Line interrupts are enabled with the VDPR0_HLINE and VDPR1_VBLANK flags
; in VDP registers 0 and 1.
;

.macro	VDP_READ_RAM_CMD ; \1 - addr

	ld de, VDP_CMD_READ_RAM | \1
	ld a, e
	out (VDP_CMD_STATUS_REG), a
	ld a, d
	out (VDP_CMD_STATUS_REG), a
.endm

.macro	VDP_WRITE_RAM_CMD ; \1 - addr

	ld de, VDP_CMD_WRITE_RAM | \1
	ld a, e
	out (VDP_CMD_STATUS_REG), a
	ld a, d
	out (VDP_CMD_STATUS_REG), a
.endm

.macro	VDP_WRITE_REG_CMD ; \1 - reg index, \2 - reg value

	ld de, VDP_CMD_WRITE_REG | ( \1 << 8 ) | \2
	ld a, e
	out (VDP_CMD_STATUS_REG), a
	ld a, d
	out (VDP_CMD_STATUS_REG), a
.endm

.macro	VDP_WRITE_CLR_CMD ; \1 - color index

	ld de, VDP_CMD_WRITE_CLR | \1
	ld a, e
	out (VDP_CMD_STATUS_REG), a
	ld a, d
	out (VDP_CMD_STATUS_REG), a
.endm

.macro	VDP_WRITE_DATA_ARRAY ; \1 - data addr, \2 - data size

	ld hl, \1
	ld b, \2

	ld c, VDP_CMD_DATA_REG
	otir	
.endm

.macro	VDP_WRITE_DATA ; \1 - data addr

	ld hl, \1
	ld a, (hl)
	out (VDP_CMD_DATA_REG), a	
.endm

.macro	VDP_ADD_VRAM_ADDR_AND_SEND_CMD_WRITE_RAM

	ld a, >VDP_CMD_WRITE_RAM
	or d
	ld d, a

	ld a, <VDP_CMD_WRITE_RAM
	or e
	out (VDP_CMD_STATUS_REG), a

	ld a, d
	out (VDP_CMD_STATUS_REG), a
.endm
