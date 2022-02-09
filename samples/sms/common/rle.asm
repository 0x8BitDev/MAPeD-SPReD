;########################################################################
;
; RLE decompressor based on the Shiru's 6502 implementation
;
; Copyright 2020 0x8BitDev ( MIT license )
;
;########################################################################


; *** data decoder ***
; IN:	DE - encoded data address
; 	HL - decoded data address
;

unrle:

	ld a, (de)
	inc de

	ld c, a
_L1:
	ld a, (de)
	inc de

	cp c
	jr z, _L2

	ld (hl), a
	inc hl

	ld b, a

	jr _L1
_L2:
	ld a, (de)
	inc de

	and a
	ret z
_L3:
	ld (hl), b
	inc hl

	dec a
	jr nz, _L3
	jr _L1