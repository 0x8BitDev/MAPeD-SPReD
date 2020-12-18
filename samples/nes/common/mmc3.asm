; Bank select ($8000-$9FFE, even)
; 7  bit  0
; ---- ----
; CPMx xRRR
; |||   |||
; |||   +++- Specify which bank register to update on next write to Bank Data register
; |||        0: Select 2 KB CHR bank at PPU $0000-$07FF (or $1000-$17FF);
; |||        1: Select 2 KB CHR bank at PPU $0800-$0FFF (or $1800-$1FFF);
; |||        2: Select 1 KB CHR bank at PPU $1000-$13FF (or $0000-$03FF);
; |||        3: Select 1 KB CHR bank at PPU $1400-$17FF (or $0400-$07FF);
; |||        4: Select 1 KB CHR bank at PPU $1800-$1BFF (or $0800-$0BFF);
; |||        5: Select 1 KB CHR bank at PPU $1C00-$1FFF (or $0C00-$0FFF);
; |||        6: Select 8 KB PRG ROM bank at $8000-$9FFF (or $C000-$DFFF);
; |||        7: Select 8 KB PRG ROM bank at $A000-$BFFF
; ||+------- Nothing on the MMC3, see MMC6
; |+-------- PRG ROM bank mode (0: $8000-$9FFF swappable,
; |                                $C000-$DFFF fixed to second-last bank;
; |                             1: $C000-$DFFF swappable,
; |                                $8000-$9FFF fixed to second-last bank)
; +--------- CHR A12 inversion (0: two 2 KB banks at $0000-$0FFF,
;                                  four 1 KB banks at $1000-$1FFF;
;                               1: two 2 KB banks at $1000-$1FFF,
;                                  four 1 KB banks at $0000-$0FFF)

; Mirroring ($A000-$BFFE, even)
; 7  bit  0
; ---- ----
; xxxx xxxM
;         |
;         +- Nametable mirroring (0: vertical; 1: horizontal)
;
; $8000 - control register ( select memory region )
; $8001 - data register ( select bank number )

; IN: X - mode
;     Y - bank number
mmc3_exec_command:
   stx $8000
   sty $8001
   rts

mmc3_horiz_mirror:
   lda #1
   sta $a000
   rts

mmc3_vert_mirror:
   lda #0
   sta $a000
   rts

mmc3_enable_wram:
   lda #$80
   sta $a001      ; enable WRAM: $6000 - $7fff (8K)
   rts

mmc3_disable_wram:
   lda #0
   sta $a001
   rts

   .macro mmc3_IRQ_disable
   sta $e000
   .endmacro

   .macro mmc3_IRQ_enable
   sta $e001
   .endmacro

   .macro mmc3_IRQ_reload val
   lda val
   sta $c000
   sta $c001
   .endmacro
