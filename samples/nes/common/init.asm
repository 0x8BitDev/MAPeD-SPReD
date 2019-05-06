; MACROSES
	; init NES hardware
	.macro init_hardware_clear_mem
	sei		; disable IRQ
	cld		; disable decimal mode
	ldx #$40
	stx $4017	; disable APU
	ldx #$ff
	txs 		; set the stack
	inx		; now X = 0
	
	stx $2000	; disable NMI
	stx $2001	; disable rendering
	stx $4010	; disable DMC IRQs
	stx $4015	; disable sound

; waiting for a new frame
	jsr vblankwait

; clear memory
@clrmem:
  	lda #$00
  	sta $0000, x
  	sta $0100, x
  	sta $0200, x
  	sta $0400, x
  	sta $0500, x
  	sta $0600, x
  	sta $0700, x
  	lda #$fe
  	sta $0300, x
  	inx
  	bne @clrmem
   
; waiting for a new frame
  	jsr vblankwait	; PPU is ready after the second vblank
  	.endmacro