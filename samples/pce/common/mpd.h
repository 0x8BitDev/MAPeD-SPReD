//######################################################################################################
//
// This file is a part of the MAPeD-PCE Copyright 2017-2021 0x8BitDev ( MIT license. See LICENSE.txt )
// Desc: It contains some HuC helper functions, structures and a tilemap rendering implementation
//
//######################################################################################################

/* HuC helper functions */

/* void mpd_load_vram2( unsigned int _vaddr, unsigned char _bank, unsigned int addr, unsigned int _words_cnt ) */
#pragma fastcall mpd_load_vram2( word __di, byte __bl, word __si, word __cx )

/* void mpd_load_vram( unsigned int _vaddr, far void* _addr, unsigned int _offset, unsigned int _words_cnt ) */
#pragma fastcall mpd_load_vram( word __di, farptr __bl:__si, word __ax, word __cx )

/* void mpd_load_palette( unsigned char _sub_plt, far void* _addr, unsigned int _offset, unsigned char _sub_plts_cnt ) */
#pragma fastcall mpd_load_palette( byte __al, farptr __bl:__si, word __dx, byte __cl )

/* void mpd_load_bat( unsigned int _vaddr, far void* _addr, unsigned int _offset, unsigned char _width, unsigned char _height ) */
#pragma fastcall mpd_load_bat( word __di, farptr __bl:__si, word __ax, byte __cl, byte __ch )

/* int mpd_farpeekw( far void* _addr, unsigned int _offset )*/
#pragma fastcall mpd_farpeekw( farptr __fbank:__fptr, word __ax )

/* char mpd_farpeekb( far void* _addr, unsigned int _offset )*/
#pragma fastcall mpd_farpeekb( farptr __fbank:__fptr, word __ax )

/* macros(es) */

#asm
	; farptr += offset
	;
	; IN:
	;
	; \1 - offset
	; \2 - bank number
	; \3 - address
	;

	.macro farptr_add_offset  ; \1 - offset, \2 - bank number, \3 - address

	; add an offset

	clc     
	lda <\1
	adc <\3
	sta <\3
	lda <\1+1
	adc <\3+1

	tay

	; increment a bank number

	lsr a
	lsr a
	lsr a
	lsr a
	lsr a
	clc
	adc <\2
	sta <\2

	; save high byte of a bank address

	tya
	and #$9f
	sta <\3+1	
	.endm
#endasm

/* asm implementations */

#asm
_mpd_load_palette.4

	farptr_add_offset __dx, __bl, __si

	maplibfunc	lib2_load_palette
	rts
#endasm

#asm
_mpd_load_vram2.4:
	jmp _load_vram.3
#endasm

#asm
_mpd_load_vram.4:

	farptr_add_offset __ax, __bl, __si

	jmp _load_vram.3
#endasm

#asm
_mpd_load_bat.5:

	farptr_add_offset __ax, __bl, __si

	maplibfunc	lib2_load_bat
	rts
#endasm

#asm
_mpd_farpeekw.2:
	asl <__al
	rol <__ah	

	farptr_add_offset __ax, __fbank, __fptr

	jmp _farpeekw.1
#endasm

#asm
_mpd_farpeekb.2:

	farptr_add_offset __ax, __fbank, __fptr

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

/* some useful definitions */

#define TRUE	1
#define FALSE	0
#define NULL	0

/* flags */

const unsigned char ADJ_SCR_LEFT	= 0;
const unsigned char ADJ_SCR_UP		= 1;
const unsigned char ADJ_SCR_RIGHT	= 2;
const unsigned char ADJ_SCR_DOWN	= 3;

/* constants */

const unsigned int __c_scr_tiles_size	= ScrTilesWidth * ScrTilesHeight;

/* variables */

mpd_SCREEN*	__curr_scr;
unsigned char	__curr_chr_id;
unsigned char	__BAT_width;

/* tilemap rendering functions */

//
// public functions:
//
// void			mpd_init( mpd_SCREEN* _start_scr )
// void			mpd_draw_screen()
// unsigned char	mpd_check_adj_screen( unsigned char _ind ) / _ind: ADJ_SCR_LEFT,ADJ_SCR_RIGHT,ADJ_SCR_UP,ADJ_SCR_DOWN; result: 0/1
// unsigned char	mpd_get_property( unsigned int _x, unsigned int _y ) / _x/_y - coordinates; result: property id
//

void __mpd_get_BAT_width()
{
	if( BAT_INDEX & 0x02 )
	{
		__BAT_width = 128;
		return;
	}

	if( BAT_INDEX & 0x01 )
	{
		__BAT_width = 64;
		return;
	}

	__BAT_width = 32;
}

void mpd_init( mpd_SCREEN* _start_scr )
{
	__curr_scr	= _start_scr;
	__curr_chr_id	= 0xff;

	__mpd_get_BAT_width();

	set_screen_size( BAT_INDEX );
}

#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
void __mpd_draw_block2x2( unsigned int _vaddr, unsigned int _offset )
{
	mpd_load_vram( _vaddr, tilemap_Attrs, _offset, 2 );
	mpd_load_vram( _vaddr + __BAT_width, tilemap_Attrs, _offset + 4, 2 );
}

#if	FLAG_TILES4X4
void __mpd_draw_tile4x4( unsigned int _vaddr, unsigned int _offset )
{
	unsigned int tiles12;
	unsigned int tiles34;

	unsigned int offset_div2;
	unsigned int blocks_offset;

	offset_div2 = _offset >> 1;

	tiles12 = mpd_farpeekw( tilemap_Tiles, offset_div2 );
	++offset_div2;
	tiles34 = mpd_farpeekw( tilemap_Tiles, offset_div2 );

	blocks_offset = mpd_farpeekw( tilemap_BlocksOffs, __curr_scr->chr_id );

	__mpd_draw_block2x2( _vaddr, blocks_offset + ( ( tiles12 & 0x00ff ) << 3 ) );
	_vaddr += 2;
	__mpd_draw_block2x2( _vaddr, blocks_offset + ( ( tiles12 & 0xff00 ) >> 5 ) );
	_vaddr += ( __BAT_width << 1 ) - 2;
	__mpd_draw_block2x2( _vaddr, blocks_offset + ( ( tiles34 & 0x00ff ) << 3 ) );
	_vaddr += 2;
	__mpd_draw_block2x2( _vaddr, blocks_offset + ( ( tiles34 & 0xff00 ) >> 5 ) );
}
#endif	//FLAG_TILES4X4
#endif	//FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL

#if	FLAG_MODE_BIDIR_SCROLL
void __mpd_draw_tiled_screen()
{
	unsigned char	scr_tile;
	unsigned int	scr_offset;
	unsigned int	w, h;
	unsigned int	h_acc;
	unsigned int	n;

	n = 0;

	scr_offset = __curr_scr->scr_ind * __c_scr_tiles_size;

#if	FLAG_DIR_COLUMNS

	for( w = 0; w < ScrTilesWidth; w++ )
	{
		h_acc = 0;

		for( h = 0; h < ScrTilesHeight; h++ )
		{
			scr_tile = mpd_farpeekb( tilemap_TilesScr, scr_offset + n );
#if	FLAG_TILES2X2
			__mpd_draw_block2x2( ( h_acc + w ) << 1, mpd_farpeekw( tilemap_BlocksOffs, __curr_scr->chr_id ) + ( scr_tile << 3 ) );
#elif	FLAG_TILES4X4
			__mpd_draw_tile4x4( ( h_acc + w ) << 2, mpd_farpeekw( tilemap_TilesOffs, __curr_scr->chr_id ) + ( scr_tile << 2 ) );
#endif
			h_acc += __BAT_width;
			++n;
		}
	}
#elif	FLAG_DIR_ROWS

	h_acc = 0;

	for( h = 0; h < ScrTilesHeight; h++ )
	{
		for( w = 0; w < ScrTilesWidth; w++ )
		{
			scr_tile = mpd_farpeekb( tilemap_TilesScr, scr_offset + n );
#if	FLAG_TILES2X2
			__mpd_draw_block2x2( ( h_acc + w ) << 1, mpd_farpeekw( tilemap_BlocksOffs, __curr_scr->chr_id ) + ( scr_tile << 3 ) );
#elif	FLAG_TILES4X4
			__mpd_draw_tile4x4( ( h_acc + w ) << 2, mpd_farpeekw( tilemap_TilesOffs, __curr_scr->chr_id ) + ( scr_tile << 2 ) );
#endif
			++n;
		}

		h_acc += __BAT_width;
	}

#endif	//FLAG_DIR_COLUMNS|FLAG_DIR_ROWS
}
#endif	//FLAG_MODE_BIDIR_SCROLL

void mpd_draw_screen()
{
	int num_CHRs;
	unsigned char chr_id_mul2;

	num_CHRs	= mpd_farpeekw( tilemap_CHRs_size, __curr_scr->chr_id ) >> 5;
	chr_id_mul2	= __curr_scr->chr_id << 1;

	// load BAT

#if	FLAG_MODE_STAT_SCR
	mpd_load_bat( 0, tilemap_VDCScr, __curr_scr->VDC_data_offset, SCR_BLOCKS2x2_WIDTH << 1, SCR_BLOCKS2x2_HEIGHT << 1 );
#elif	FLAG_MODE_BIDIR_SCROLL
	__mpd_draw_tiled_screen();
#endif

	// load tiles & palette

	if( __curr_scr->chr_id != __curr_chr_id )
	{
		__curr_chr_id	= __curr_scr->chr_id;

		mpd_load_vram2( CHRS_OFFSET << 5, tilemap_CHRs[ chr_id_mul2 ], tilemap_CHRs[ chr_id_mul2 + 1 ], num_CHRs << 4 );
		mpd_load_palette( 0, tilemap_Plts, chr_id_mul2 << 8, 16 );
	}
}

unsigned char mpd_check_adj_screen( unsigned char _ind )
{
#if	FLAG_MARKS
	if( !( __curr_scr->marks & ( 1 << ( _ind + 4 ) ) ) )
	{
		return FALSE;
	}
#endif

#if	FLAG_LAYOUT_ADJ_SCR
		
	if( __curr_scr->adj_scr[ _ind ] != NULL )
	{
		__curr_scr = __curr_scr->adj_scr[ _ind ];

		return TRUE;
	}
#endif
#if	FLAG_LAYOUT_ADJ_SCR_INDS

	if( __curr_scr->adj_scr[ _ind ] != 0xff )
	{
		__curr_scr = Lev0_ScrArr[ __curr_scr->adj_scr[ _ind ] ];

		return TRUE;
	}
#endif
	return FALSE;
}

unsigned char mpd_get_property( unsigned int _x, unsigned int _y )
{
	// TODO:
//	FLAG_TILES2X2
//	FLAG_TILES4X4
//	FLAG_DIR_COLUMNS
//	FLAG_DIR_ROWS
//	FLAG_PROP_ID_PER_BLOCK
//	FLAG_PROP_ID_PER_CHR

	//...
}