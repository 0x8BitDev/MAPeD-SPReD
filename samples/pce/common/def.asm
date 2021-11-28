;###################################################################
;
; Copyright 2021 0x8BitDev ( MIT license )
;
;###################################################################
;
; Hardware ports & useful memory addresses
;

;--- VDC

VDC_REG 	= $0000
VDC_DATA	= $0002
VDC_DATA_L	= $0002
VDC_DATA_H	= $0003

VDC_SR_COLLISION_DETECT	= %00000001	; sprite #0 hit
VDC_SR_OVER_DETECT	= %00000010	; ...
VDC_SR_SCANLINE_DETECT	= %00000100	; ...
VDC_SR_VRAM_SATB_END	= %00001000	; ...
VDC_SR_VRAM_VRAM_END	= %00010000	; ...
VDC_SR_VBLANK		= %00100000	; ...
VDC_SR_BUSY		= %01000000	; ...

VDC_R00_MAWR	= $00	; memory address write rigister
VDC_R01_MARR	= $01	; memory address read register
VDC_R02_VWR	= $02	; VRAM data write register
VDC_R02_VRR	= $03	; VRAM data read register
VDC_R05_CR	= $05	; control register

VDC_CR_COLLISION_DETECT	= %00000000_00000001	; collision detection
VDC_CR_OVER_DETECT	= %00000000_00000010	; over detect
VDC_CR_SCANLINE_DETECT	= %00000000_00000100	; scanline detect
VDC_CR_VBLANK		= %00000000_00001000	; VBLANK
VDC_CR_EX01		= %00000000_00010000	; VSYNC and HSYNC work as input and are synchronized to external signals
VDC_CR_EX11		= %00000000_00110000	; both VSYNC and HSYNC work as output
VDC_CR_SHOW_SPR		= %00000000_01000000	; show sprites
VDC_CR_SHOW_BACKGR	= %00000000_10000000	; show background
VDC_CR_TE01		= %00000001_00000000	; BURST Indicates the position in which Color Burst is inserted
VDC_CR_TE10		= %00000010_00000000	; INTHSYNC Internal horizontal SYNC
VDC_CR_DR		= %00000100_00000000	; dynamic RAM refresh
VDC_CR_RW_INC1		= %00000000_00000000	; ???
VDC_CR_RW_INC32		= %00001000_00000000	; ...
VDC_CR_RW_INC64		= %00010000_00000000	; ...
VDC_CR_RW_INC128	= %00011000_00000000	; ...

VDC_R06_RCR	= $06	; scanline detection register
VDC_R07_BXR	= $07	; BGX scroll register 10 bit
VDC_R08_BYR	= $08	; BGY scroll register 9 bit
VDC_R09_MWR	= $09	; memory access width register

VDC_MWR_VM01		= %00000001	; 2 BAT CPU CGO CG1
VDC_MWR_VM10		= %00000010	; 2 BAT CPU CGO CG1
VDC_MWR_VM11		= %00000011	; 4 BAT CGO or CG1
VDC_MWR_SM01		= %00000100
VDC_MWR_SM10		= %00001000
VDC_MWR_SM11		= %00001100
VDC_MWR_BAT32x32	= %00000000	; ???
VDC_MWR_BAT64x32	= %00010000
VDC_MWR_BAT128x32	= %00110000
VDC_MWR_BAT32x64	= %01000000
VDC_MWR_BAT64x64	= %01010000
VDC_MWR_BAT128x64	= %01110000
VDC_MWR_CG		= %10000000	; 1: (CG 1 )CH2, CH3; 0:(CGO)CHO, CH 1

VDC_R0A_HSR	= $0a	; horizontal sync register

; \1 - HDS, \2 - HSW
VDC_HSR	.func	( (\1 << 8) | \2 )

VDC_R0B_HDR	= $0b	; horizontal display register

; \1 - HDE, \2 - HDW
VDC_HDR	.func	( (\1 << 8) | \2 )

VDC_R0C_VPR	= $0c	; vertical sync register

; \1 - VDS, \2 - VSW
VDC_VPR	.func	( (\1 << 8) | \2 )

VDC_R0D_VDR	= $0d	; vertical display register
VDC_R0E_VCR	= $0e	; vertical display end position register
VDC_R0F_DCR	= $0f	; block transfer control register ()

VDC_DCR_DSC	= %00000001	; VRAM-SATB Transfer Complete Interrupt Request Enable
VDC_DCR_DVC	= %00000010	; VRAM-VRAM Transfer Complete Interrupt Request Enable
VDC_DCR_SI_D	= %00000100	; source Address INC/DEC
VDC_DCR_DI_D	= %00001000	; destination Address INC/DEC
VDC_DCR_DSR	= %00010000	; VRAM-SATB Transfer Auto-Repeat

VDC_R10_SOUR	= $10	; block transfer source address register
VDC_R11_DESR	= $11	; block transfer destination address register
VDC_R12_LENR	= $12	; block transfer length register
VDC_R13_DVSSR	= $13	; VRAM-SATB block transfer source address register

;--- VCE

VCE_CR			= $0400	; 16 bit
VCE_ADDR		= $0402
VCE_ADDR_L		= $0402
VCE_ADDR_H		= $0403
VCE_WRITE_DATA		= $0404
VCE_WRITE_DATA_L	= $0404
VCE_WRITE_DATA_H	= $0405
VCE_READ_DATA		= $0406
VCE_READ_DATA_L		= $0406
VCE_READ_DATA_H		= $0407

;--- IRQ

IRQ_DISABLE	= $1402 ; write only
IRQ_STATUS	= $1403	; read only

IRQ_FLAG_IRQ2	= $01
IRQ_FLAG_IRQ1	= $02
IRQ_FLAG_TIMER	= $04

;--- TIMER

TIMER_CNT	= $0c00	; %01111111
TIMER_CNTRL	= $0c01	; 1-start; 0-stop

;--- I\O

JPAD_PORT	= $1000

; (Write)
; D0   1=selects upper 4bits, 0=selects lower 4bits
; D1   resets the multiplexed buffer/counter (needed for TAP reading)
;           
; (Read)
; D0-3 4bit controller values
; D4-5 unused
; D6   Region code. 0=Japan,1=US/other
; D7   unused
;

JPAD_BTN_BTN1	= %00000001
JPAD_BTN_BTN2	= %00000010
JPAD_BTN_SELECT	= %00000100
JPAD_BTN_RUN	= %00001000
JPAD_BTN_UP	= %00010000
JPAD_BTN_RIGHT	= %00100000
JPAD_BTN_DOWN	= %01000000
JPAD_BTN_LEFT	= %10000000
