;#######################################################
;
; Generated by MAPeD-PCE Copyright 2017-2022 0x8BitDev
;
;#######################################################


; *** GLOBAL DATA ***

_chr0:	.incbin "_tilemap__chr0.bin"		; (6752)


_mpd_CHRs_size:
	.word 6752	; (_chr0)

_mpd_Props:	.incbin "_tilemap_Props.bin"	; (520) block properties array of all exported data banks ( 4 bytes per block )

_mpd_PropsOffs:
	.word 0	; (chr0)
	.word 520	; data end

_mpd_Tiles:	.incbin "_tilemap_Tiles.bin"	; (872) 4x4 tiles array of all exported data banks ( 4 bytes per tile )

_mpd_TilesOffs:
	.word 0	; (chr0)

_mpd_BlocksOffs:
	.word 0	; (chr0)

_mpd_Attrs:	.incbin "_tilemap_Attrs.bin"	; (1040) 2x2 tiles attributes array of all exported data banks ( 2 bytes per attribute )
_mpd_TilesScr:	.incbin "_tilemap_TilesScr.bin"	; (1232) 4x4 tiles array for each screen ( 56 bytes per screen \ 1 byte per tile )
_mpd_Plts:	.incbin "_tilemap_Plts.bin"	; (512) palettes array of all exported data banks ( data offset = chr_id * 512 )
