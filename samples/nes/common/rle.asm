;RLE decompressor by Shiru
;decompress data from an address in X/Y to PPU_DATA or RLE_DECOMP_ADDR

.segment "ZP"

RLE_LOW:	.res 1
RLE_HIGH:	.res 1
RLE_TAG:	.res 1
RLE_BYTE:	.res 1

RLE_DECOMP_ADDR:	.res 2
PPU_DATA		= $2007

.segment "CODE"

	.macro save_byte
	sta (<RLE_DECOMP_ADDR), y
	inc RLE_DECOMP_ADDR
	bne @L1
	inc RLE_DECOMP_ADDR + 1
@L1:
	.endmacro

unrle_to_mem:
	stx RLE_LOW
	sty RLE_HIGH
	ldy #0
	jsr rle_byte
	sta RLE_TAG
L1:
	jsr rle_byte
	cmp RLE_TAG
	beq L2
	save_byte
	sta RLE_BYTE
	bne L1
L2:
	jsr rle_byte
	cmp #0
	beq L4
	tax
	lda RLE_BYTE
L3:
	save_byte
	dex
	bne L3
	beq L1
L4:
	rts

rle_byte:
	lda (<RLE_LOW),y
	inc RLE_LOW
	bne L5
	inc RLE_HIGH
L5:
	rts

unrle_to_PPU:
	stx RLE_LOW
	sty RLE_HIGH
	ldy #0
	jsr rle_byte_p
	sta RLE_TAG
L1_p:
	jsr rle_byte_p
	cmp RLE_TAG
	beq L2_p
	sta PPU_DATA
	sta RLE_BYTE
	bne L1_p
L2_p:
	jsr rle_byte_p
	cmp #0
	beq L4_p
	tax
	lda RLE_BYTE
L3_p:
	sta PPU_DATA
	dex
	bne L3_p
	beq L1_p
L4_p:
	rts

rle_byte_p:
	lda (<RLE_LOW),y
	inc RLE_LOW
	bne L5_p
	inc RLE_HIGH
L5_p:
	rts