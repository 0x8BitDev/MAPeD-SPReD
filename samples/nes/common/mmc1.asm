; 	MMC1 configuration
;	43210
;	-----
;	CPRMM
;	|||||
;	|||++- Mirroring (0: one-screen, lower bank; 1: one-screen, upper bank;
;	|||               2: vertical; 3: horizontal)
;	||+--- PRG swap range (0: switch 16 KB bank at $C000; 1: switch 16 KB bank at $8000;
;	||                            only used when PRG bank mode bit below is set to 1)
;	|+---- PRG size (0: switch 32 KB at $8000, ignoring low bit of bank number;
;	|                         1: switch 16 KB at address specified by location bit above)
;	+----- CHR size (0: switch 8 KB at a time; 1: switch two separate 4 KB banks)
;
	.macro mmc1_reset
	ldx #$ff
	stx $fff2
	.endmacro

mmc1_config_write:
	ldx #$80
	stx $8000      ; reset the shift register

	sta $8000      ; first data bit
	lsr a          ; shift to next bit
	sta $8000      ; second data bit
	lsr a          ; etc
	sta $8000
	lsr a
	sta $8000
	lsr a
	sta $8000
	rts

; PRG bank switching
;	A - bank number
mmc1_prg_bank_write:

	sta $e000       ; first data bit
	lsr a           ; shift to next bit
	sta $e000
	lsr a
	sta $e000
	lsr a
	sta $e000
	lsr a
	sta $e000
	rts

; example:
;
;	lda #%00001110 ; 8KB CHR, 16KB PRG, $8000-BFFF swappable, vertical mirroring
;	jsr mmc1_config_write
;	lda #$00	; 0 bank!
;	jsr mmc1_prg_bank_write

; CHR bank switching $0000
;	A - bank number
mmc1_chr_bank0_write:

	sta $a000       ; first data bit
	lsr a           ; shift to next bit
	sta $a000
	lsr a
	sta $a000
	lsr a
	sta $a000
	lsr a
	sta $a000
	rts

; CHR bank switching $1000
;	A - bank number
mmc1_chr_bank1_write:

	sta $c000       ; first data bit
	lsr a           ; shift to next bit
	sta $c000
	lsr a
	sta $c000
	lsr a
	sta $c000
	lsr a
	sta $c000
	rts