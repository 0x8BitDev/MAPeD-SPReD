;#######################################################
;
; Generated by MAPeD-PCE Copyright 2017-2022 0x8BitDev
;
;#######################################################

; *** GLOBAL DATA ***

_chr0:	.incbin "_tilemap__chr0.bin"		; (17376)
_chr1:	.incbin "_tilemap__chr1.bin"		; (13248)


_mpd_CHRs_size:
	.word 17376	; (_chr0)
	.word 13248	; (_chr1)

_mpd_Attrs:	.incbin "_tilemap_Attrs.bin"	; (3360) attributes array per block ( 2 bytes per attribute; 8 bytes per block ) of all exported data banks

_mpd_BlocksOffs:
	.word 0	; (chr0)
	.word 1968	; (chr1)

_mpd_Props:	.incbin "_tilemap_Props.bin"	; (1680) blocks properties array ( 4 bytes per block ) of all exported data banks

_mpd_PropsOffs:
	.word 0	; (chr0)
	.word 984	; (chr1)
	.word 1680	; data end

_mpd_Maps:	.incbin "_tilemap_Maps.bin"	; (13888) game levels blocks (2x2) array of all exported data banks

_mpd_MapsOffs:
	.word 0
	.word 4480

_mpd_MapsTbl:	.incbin "_tilemap_MapsTbl.bin"	; (320) lookup table for fast calculation of tile addresses; columns by X coordinate ( 16 bit offset per column of tiles ) of all exported data banks

_mpd_MapsTblOffs:
	.word 0
	.word 128

_mpd_Plts:	.incbin "_tilemap_Plts.bin"	; (1024) palettes array of all exported data banks ( data offset = chr_id * 512 )

