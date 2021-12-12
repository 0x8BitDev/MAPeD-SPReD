//######################################################################################################
//
// This file is a part of the MAPeD-SMD Copyright 2017-2021 0x8BitDev ( MIT license. See LICENSE.txt )
// Desc: It contains some utility HuC functions and structures
//
//######################################################################################################


/* void mpd_set_tile_data( unsigned int _bank, unsigned int addr, unsigned int _num_tiles, unsigned char _type )*/
#pragma fastcall mpd_set_tile_data( word __ax, word __bx, word __cx, byte __dl )

/* int mpd_farpeekw( far void* _addr, unsigned int _offset )*/
#pragma fastcall mpd_farpeekw( farptr __fbank:__fptr, word __ax )

/* char mpd_farpeekb( far void* _addr, unsigned int _offset )*/
#pragma fastcall mpd_farpeekb( farptr __fbank:__fptr, word __ax )

#asm
_mpd_set_tile_data.4:
	stb	<__al,maptilebank
	stw	<__bx,maptileaddr
	stw	<__cx,mapnbtile
	stb	<__dl,maptiletype
	rts
#endasm

#asm
_mpd_farpeekw.2:
	asl <__al
	rol <__ah	

	clc
	lda <__al
	adc <__fptr
	sta <__fptr
	lda <__ah
	adc <__fptr+1
	sta <__fptr+1
	jmp _farpeekw.1
#endasm

#asm
_mpd_farpeekb.2:
	clc
	lda <__al
	adc <__fptr
	sta <__fptr
	lda <__ah
	adc <__fptr+1
	sta <__fptr+1
	jmp _farpeekb.1
#endasm

/* entities */

typedef struct
{
	unsigned char	id;
	unsigned char	width;
	unsigned char	height;
	unsigned char	pivot_x;
	unsigned char	pivot_y;
	unsigned char	props_cnt;
	unsigned char	props[];
} mpd_BASE_ENTITY;

typedef struct
{
	unsigned char		id;
	mpd_BASE_ENTITY*	base_ent;
	void*			target_ent;	//mpd_ENTITY_INSTANCE
	unsigned int		x_pos;
	unsigned int		y_pos;
	unsigned char		props_cnt;
	unsigned char		props[];
} mpd_ENTITY_INSTANCE;
