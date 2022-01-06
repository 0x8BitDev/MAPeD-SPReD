//######################################################################################################
//
// This file is a part of the MAPeD-PCE Copyright 2017-2022 0x8BitDev ( MIT license. See LICENSE.txt )
// Desc: It contains some HuC helper functions, structures and a tilemap rendering implementation
//
//######################################################################################################

//
// public functions:
//
// #if		FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
// typedef enum
// {
// 	ms_1px = 1,
// 	ms_2px = 2,
// 	ms_4px = 4 
// } mpd_scroll_step;
//
// void		mpd_init( mpd_SCREEN* _start_scr, mpd_scroll_step _step )
// #else
// void		mpd_init( mpd_SCREEN* _start_scr )
// #endif	//FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
//
// void		mpd_draw_screen()
// u8		mpd_check_adj_screen( u8 _ind ) / _ind: ADJ_SCR_LEFT,ADJ_SCR_RIGHT,ADJ_SCR_UP,ADJ_SCR_DOWN; result: 0/1
// u8		mpd_get_property( u16 _x, u16 _y ) / _x/_y - coordinates; result: property id
// mpd_SCREEN*	mpd_curr_screen()
//
// #if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
// void		mpd_clear_update_flags()
// void		mpd_move_left()
// void		mpd_move_right()
// void		mpd_move_up()
// void		mpd_move_down()
// void		mpd_update_screen()
// u16		mpd_scroll_x()
// u16		mpd_scroll_y()
// #endif	//FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
//

/************************/
/*			*/
/* HuC helper functions	*/
/*			*/
/************************/

/* u16 mpd_get_CR_val() */
#pragma fastcall mpd_get_CR_val()

/* void mpd_load_vram2( u16 _vaddr, u8 _bank, u16 addr, u16 _words_cnt ) */
#pragma fastcall mpd_load_vram2( word __di, byte __bl, word __si, word __cx )

/* void mpd_load_vram( u16 _vaddr, far void* _addr, u16 _offset, u16 _words_cnt ) */
#pragma fastcall mpd_load_vram( word __di, farptr __bl:__si, word __ax, word __cx )

/* void mpd_load_palette( u8 _sub_plt, far void* _addr, u16 _offset, u8 _sub_plts_cnt ) */
#pragma fastcall mpd_load_palette( byte __al, farptr __bl:__si, word __dx, byte __cl )

/* void mpd_load_bat( u16 _vaddr, far void* _addr, u16 _offset, u8 _width, u8 _height ) */
#pragma fastcall mpd_load_bat( word __di, farptr __bl:__si, word __ax, byte __cl, byte __ch )

/* int mpd_farpeekw( far void* _addr, u16 _offset )*/
#pragma fastcall mpd_farpeekw( farptr __fbank:__fptr, word __ax )

/* char mpd_farpeekb( far void* _addr, u16 _offset )*/
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
_mpd_get_CR_val

	ldx <vdc_crl
	lda <vdc_crh

	rts
#endasm

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
	farptr_add_offset __ax, __fbank, __fptr

	jmp _farpeekw.1
#endasm

#asm
_mpd_farpeekb.2:

	farptr_add_offset __ax, __fbank, __fptr

	jmp _farpeekb.1
#endasm

/****************/
/*		*/
/* display list	*/
/*		*/
/****************/

#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL

#define DISP_LIST_SIZE	128
#define DISP_LIST_END	0

#define DL_FLAG_DATA_INC1	0x01
#define DL_FLAG_DATA_INC_VERT	0x02

#define	CR_IW_INV_MASK		~( 0x18 << 8 )

u8	__dl_pos;
u16	__dl_arr[ DISP_LIST_SIZE ];
u16	__CR_IW_val;

void	__mpd_disp_list_reset()
{
	__dl_pos = 0;
	__dl_arr[ __dl_pos ] = 0;
}

void	__mpd_disp_list_push_hdr( u8 _flag, u8 _size, u16 _vaddr )
{
	__dl_arr[ __dl_pos++ ] = ( _flag << 8 ) | _size;
	__dl_arr[ __dl_pos++ ] = _vaddr;
}

void	__mpd_disp_list_push_data( u16 _data )
{
	__dl_arr[ __dl_pos++ ] = _data;
}

void	__mpd_disp_list_end()
{
	__dl_arr[ __dl_pos ] = DISP_LIST_END;
}

void	__mpd_disp_list_flush()
{
	u16	CR_val;
	u16 	hdr;
	u16 	vaddr;
	u16	data_inc1;
	u16	data_inc_vert;
	u8	data_flag;
	u8	data_size;

	if( __mpd_disp_list_empty() == FALSE )
	{
		// save CR
		CR_val = mpd_get_CR_val();

		data_inc1	= CR_IW_INV_MASK & CR_val;
		data_inc_vert	= data_inc1 | __CR_IW_val;

		__dl_pos = 0;

		while( __dl_arr[ __dl_pos ] != DISP_LIST_END )
		{
			hdr	= __dl_arr[ __dl_pos++ ];
			vaddr	= __dl_arr[ __dl_pos++ ];

			data_flag = hdr >> 8;
			data_size = hdr & 0x00ff;

			if( data_flag == DL_FLAG_DATA_INC1 )
			{
				vreg( 5, data_inc1 );
			}
			else
			if( data_flag == DL_FLAG_DATA_INC_VERT )
			{
				vreg( 5, data_inc_vert );
			}

			mpd_load_vram( vaddr, __dl_arr, __dl_pos << 1, data_size );

			__dl_pos += data_size;
		}

		// restore CR
		vreg( 5, CR_val );

		__mpd_disp_list_reset();
	}
}

u8	__mpd_disp_list_empty()
{
	return __dl_pos != 0 ? FALSE:TRUE;
}

#endif	//FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL

/********************************/
/*				*/
/* tilemap rendering functions	*/
/*				*/
/********************************/

/* flags */

const u8 ADJ_SCR_LEFT	= 0;
const u8 ADJ_SCR_UP	= 1;
const u8 ADJ_SCR_RIGHT	= 2;
const u8 ADJ_SCR_DOWN	= 3;

/* constants */

const u16 __c_scr_tiles_size	= ScrTilesWidth * ScrTilesHeight;

/* variables */

mpd_SCREEN*	__curr_scr;
u8		__curr_chr_id;
u8		__BAT_width;
u8		__BAT_width_dec1;
u8		__BAT_height_dec1;
u8		__BAT_width_pow2;
u16		__BAT_size_dec1;	// ( BAT_width * BAT_height ) - 1

#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
#define	UPD_FLAG_DRAW_LEFT	0x01
#define	UPD_FLAG_DRAW_RIGHT	0x02
#define	UPD_FLAG_DRAW_UP	0x04
#define	UPD_FLAG_DRAW_DOWN	0x08
#define	UPD_FLAG_DRAW_MASK	UPD_FLAG_DRAW_LEFT|UPD_FLAG_DRAW_RIGHT|UPD_FLAG_DRAW_UP|UPD_FLAG_DRAW_DOWN

typedef enum
{
	ms_1px = 1,
	ms_2px = 2,
	ms_4px = 4 
} mpd_scroll_step;

u8	__scroll_step;
u16	__horiz_dir_pos;
u16	__vert_dir_pos;
u16	__scroll_x;
u16	__scroll_y;
u8	__upd_flags;
#endif

#if	FLAG_MODE_BIDIR_SCROLL
u16		__scr_tiles_width_tbl[ ScrTilesHeight ];
u16		__scr_tiles_height_tbl[ ScrTilesWidth ];
#endif	//FLAG_MODE_BIDIR_SCROLL

#if	FLAG_LAYOUT_ADJ_SCR_INDS
mpd_SCREEN**	__scr_arr;
#endif	//FLAG_LAYOUT_ADJ_SCR_INDS

void	__mpd_get_BAT_params()
{
	u16 BAT_height;

	BAT_height = BAT_INDEX & 0x04 ? 64:32;

	if( BAT_INDEX & 0x02 )
	{
		__BAT_width = 128;
		__BAT_width_pow2 = 7;

#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
		__CR_IW_val = 0x18 << 8;
#endif
	}
	else
	if( BAT_INDEX & 0x01 )
	{
		__BAT_width = 64;
		__BAT_width_pow2 = 6;

#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
		__CR_IW_val = 0x10 << 8;
#endif
	}
	else
	{
		__BAT_width = 32;
		__BAT_width_pow2 = 5;

#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
		__CR_IW_val = 0x08 << 8;
#endif
	}

	__BAT_width_dec1	= __BAT_width - 1;
	__BAT_height_dec1	= BAT_height - 1;
	__BAT_size_dec1		= ( __BAT_width * BAT_height ) - 1;
}

#if	FLAG_MODE_MULTIDIR_SCROLL
void	mpd_init( u8 _start_scr, mpd_scroll_step _step )
#else
#if	FLAG_MODE_BIDIR_SCROLL
void	mpd_init( mpd_SCREEN* _start_scr, mpd_SCREEN** _scr_arr, mpd_scroll_step _step )
#else
void	mpd_init( mpd_SCREEN* _start_scr, mpd_SCREEN** _scr_arr )
#endif
#endif
{
#if	FLAG_MODE_BIDIR_SCROLL
	u16	n;
	u16	data_acc;

	data_acc = 0;
	for( n = 0; n < ScrTilesHeight; n++ )
	{
		__scr_tiles_width_tbl[ n ] = data_acc;
		data_acc += ScrTilesWidth;
	}

	data_acc = 0;
	for( n = 0; n < ScrTilesWidth; n++ )
	{
		__scr_tiles_height_tbl[ n ] = data_acc;
		data_acc += ScrTilesHeight;
	}

#endif	//FLAG_MODE_BIDIR_SCROLL

#if	FLAG_LAYOUT_ADJ_SCR_INDS
	__scr_arr	= _scr_arr;
#endif	//FLAG_LAYOUT_ADJ_SCR_INDS

#if	!FLAG_MODE_MULTIDIR_SCROLL
	__curr_scr	= _start_scr;
#endif	//!FLAG_MODE_MULTIDIR_SCROLL

	__mpd_get_BAT_params();

	set_screen_size( BAT_INDEX );

#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
	__scroll_step	= _step;
	__scroll_x	= 0;
	__scroll_y	= 0;
	__horiz_dir_pos	= 0;
	__vert_dir_pos	= 0;
	__upd_flags	= 0;

	__mpd_disp_list_reset();
	__mpd_update_scroll();
#endif

	__curr_chr_id	= 0xff;
}

#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL + FLAG_MODE_BIDIR_STAT_SCR
void	__mpd_draw_block2x2( u16 _vaddr, u16 _offset )
{
	mpd_load_vram( _vaddr, mpd_Attrs, _offset, 2 );
	mpd_load_vram( _vaddr + __BAT_width, mpd_Attrs, _offset + 4, 2 );
}

#if	FLAG_TILES4X4
void	__mpd_draw_tile4x4( u16 _vaddr, u16 _offset )
{
	u16 tiles12;
	u16 tiles34;

	u16 blocks_offset;

	tiles12 = mpd_farpeekw( mpd_Tiles, _offset );
	tiles34 = mpd_farpeekw( mpd_Tiles, _offset + 2 );

	blocks_offset = mpd_farpeekw( mpd_BlocksOffs, __curr_chr_id << 1 );

	__mpd_draw_block2x2( _vaddr, blocks_offset + ( ( tiles12 & 0x00ff ) << 3 ) );
	_vaddr += 2;
	__mpd_draw_block2x2( _vaddr, blocks_offset + ( ( tiles12 & 0xff00 ) >> 5 ) );
	_vaddr += ( __BAT_width << 1 ) - 2;
	__mpd_draw_block2x2( _vaddr, blocks_offset + ( ( tiles34 & 0x00ff ) << 3 ) );
	_vaddr += 2;
	__mpd_draw_block2x2( _vaddr, blocks_offset + ( ( tiles34 & 0xff00 ) >> 5 ) );
}
#endif	//FLAG_TILES4X4
#endif	//FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL + FLAG_MODE_BIDIR_STAT_SCR

#if	FLAG_MODE_BIDIR_SCROLL + FLAG_MODE_BIDIR_STAT_SCR
void	__mpd_draw_tiled_screen()
{
	u8	scr_tile;
	u16	scr_offset;
	u16	chr_id_mul2;
	u16	w, h;
	u16	h_acc;
	u16	n;

	n = 0;

	scr_offset	= __curr_scr->scr_ind * __c_scr_tiles_size;
	chr_id_mul2	= __curr_chr_id << 1; 

#if	FLAG_DIR_COLUMNS

	for( w = 0; w < ScrTilesWidth; w++ )
	{
		h_acc = 0;

		for( h = 0; h < ScrTilesHeight; h++ )
		{
			scr_tile = mpd_farpeekb( mpd_TilesScr, scr_offset + n );
#if	FLAG_TILES2X2
			__mpd_draw_block2x2( ( h_acc + w ) << 1, mpd_farpeekw( mpd_BlocksOffs, chr_id_mul2 ) + ( scr_tile << 3 ) );
#elif	FLAG_TILES4X4
			__mpd_draw_tile4x4( ( h_acc + w ) << 2, mpd_farpeekw( mpd_TilesOffs, chr_id_mul2 ) + ( scr_tile << 2 ) );
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
			scr_tile = mpd_farpeekb( mpd_TilesScr, scr_offset + n );
#if	FLAG_TILES2X2
			__mpd_draw_block2x2( ( h_acc + w ) << 1, mpd_farpeekw( mpd_BlocksOffs, chr_id_mul2 ) + ( scr_tile << 3 ) );
#elif	FLAG_TILES4X4
			__mpd_draw_tile4x4( ( h_acc + w ) << 2, mpd_farpeekw( mpd_TilesOffs, chr_id_mul2 ) + ( scr_tile << 2 ) );
#endif
			++n;
		}

		h_acc += __BAT_width;
	}

#endif	//FLAG_DIR_COLUMNS|FLAG_DIR_ROWS
}
#endif	//FLAG_MODE_BIDIR_SCROLL + FLAG_MODE_BIDIR_STAT_SCR

#if	FLAG_MODE_MULTIDIR_SCROLL
void	__mpd_draw_tiled_screen()
{
	// NOT IMPLEMENTED!..
	//...
}
#endif	//FLAG_MODE_MULTIDIR_SCROLL

void	mpd_draw_screen()
{
	u16 num_CHRs;
	u8 chr_id_mul2;

	chr_id_mul2	= __curr_scr->chr_id << 1;
	num_CHRs	= mpd_farpeekw( mpd_CHRs_size, chr_id_mul2 ) >> 5;

	// load tiles & palette

	if( __curr_scr->chr_id != __curr_chr_id )
	{
		__curr_chr_id	= __curr_scr->chr_id;

		mpd_load_vram2( CHRS_OFFSET << 5, mpd_CHRs[ chr_id_mul2 ], mpd_CHRs[ chr_id_mul2 + 1 ], num_CHRs << 4 );
		mpd_load_palette( 0, mpd_Plts, chr_id_mul2 << 8, 16 );
	}

	// load BAT

#if	FLAG_MODE_STAT_SCR
	mpd_load_bat( 0, mpd_VDCScr, __curr_scr->VDC_data_offset, SCR_BLOCKS2x2_WIDTH << 1, SCR_BLOCKS2x2_HEIGHT << 1 );
#elif	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL + FLAG_MODE_BIDIR_STAT_SCR
	__mpd_draw_tiled_screen();
#endif
}

mpd_SCREEN*	__mpd_get_adj_screen( u8 _ind )
{
	mpd_SCREEN*	adj_scr;
	u8		adj_scr_ind;

#if	FLAG_MARKS
	if( !( __curr_scr->marks & ( 1 << ( _ind + 4 ) ) ) )
	{
		return NULL;
	}
#endif

#if	FLAG_LAYOUT_ADJ_SCR
		
	adj_scr = __curr_scr->adj_scr[ _ind ];

	if( adj_scr != NULL )
	{
		return adj_scr;
	}
#endif
#if	FLAG_LAYOUT_ADJ_SCR_INDS

	adj_scr_ind = __curr_scr->adj_scr[ _ind ];

	if( adj_scr_ind != 0xff )
	{
		return __scr_arr[ adj_scr_ind ];;
	}
#endif
	return NULL;
}

u8	mpd_check_adj_screen( u8 _ind )
{
	mpd_SCREEN* adj_scr;

	if( adj_scr = __mpd_get_adj_screen( _ind ) )
	{
		__curr_scr = adj_scr;

		return TRUE;
	}

	return FALSE;
}

/********************************/
/*				*/
/* tilemap scrolling functions	*/
/*				*/
/********************************/

#if	FLAG_MODE_BIDIR_SCROLL
void	mpd_move_left()
{
	mpd_SCREEN* adj_scr;

	if( __vert_dir_pos != 0 )
	{
		return;
	}

	if( __horiz_dir_pos == 0 )
	{
		if( __mpd_get_adj_screen( ADJ_SCR_LEFT ) == FALSE )
		{
			return;
		}
	}

	if( __horiz_dir_pos <= 0 )
	{
		__horiz_dir_pos = ScrPixelsWidth;

		if( mpd_check_adj_screen( ADJ_SCR_LEFT ) == FALSE )
		{
			return;
		}

		mpd_move_left();

		return;
	}

	__horiz_dir_pos -= __scroll_step;

	__mpd_upd_scroll_left();
}

void	mpd_move_right()
{
	mpd_SCREEN* adj_scr;

	if( __vert_dir_pos != 0 )
	{
		return;
	}

	if( __horiz_dir_pos == 0 )
	{
		if( __mpd_get_adj_screen( ADJ_SCR_RIGHT ) == FALSE )
		{
			return;
		}
	}

	if( __horiz_dir_pos >= ScrPixelsWidth )
	{
		__horiz_dir_pos = 0;

		if( mpd_check_adj_screen( ADJ_SCR_RIGHT ) == FALSE )
		{
			return;
		}

		mpd_move_right();

		return;
	}

	__horiz_dir_pos += __scroll_step;

	__mpd_upd_scroll_right();
}

void	mpd_move_up()
{
	mpd_SCREEN* adj_scr;

	if( __horiz_dir_pos != 0 )
	{
		return;
	}

	if( __vert_dir_pos == 0 )
	{
		if( __mpd_get_adj_screen( ADJ_SCR_UP ) == FALSE )
		{
			return;
		}
	}

	if( __vert_dir_pos <= 0 )
	{
		__vert_dir_pos = ScrPixelsHeight;

		if( mpd_check_adj_screen( ADJ_SCR_UP ) == FALSE )
		{
			return;
		}

		mpd_move_up();

		return;
	}

	__vert_dir_pos -= __scroll_step;

	__mpd_upd_scroll_up();
}

void	mpd_move_down()
{
	mpd_SCREEN* adj_scr;

	if( __horiz_dir_pos != 0 )
	{
		return;
	}

	if( __vert_dir_pos == 0 )
	{
		if( __mpd_get_adj_screen( ADJ_SCR_DOWN ) == FALSE )
		{
			return;
		}
	}

	if( __vert_dir_pos >= ScrPixelsHeight )
	{
		__vert_dir_pos = 0;

		if( mpd_check_adj_screen( ADJ_SCR_DOWN ) == FALSE )
		{
			return;
		}

		mpd_move_down();

		return;
	}

	__vert_dir_pos += __scroll_step;

	__mpd_upd_scroll_down();
}
#endif	//FLAG_MODE_BIDIR_SCROLL

#if	FLAG_MODE_MULTIDIR_SCROLL
void	mpd_move_left()
{
	//...
}

void	mpd_move_right()
{
	//...
}

void	mpd_move_up()
{
	//...
}

void	mpd_move_down()
{
	//...
}	
#endif	//FLAG_MODE_MULTIDIR_SCROLL

#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
void	mpd_clear_update_flags()
{
	__upd_flags &= ~(UPD_FLAG_DRAW_MASK);
}

void	mpd_update_screen()
{
	if( __upd_flags & UPD_FLAG_DRAW_MASK )
	{
		if( __upd_flags & UPD_FLAG_DRAW_LEFT )
		{
			__mpd_draw_left_tiles_column();
		}

		if( __upd_flags & UPD_FLAG_DRAW_RIGHT )
		{
			__mpd_draw_right_tiles_column();
		}

		if( __upd_flags & UPD_FLAG_DRAW_UP )
		{
			__mpd_draw_up_tiles_column();
		}

		if( __upd_flags & UPD_FLAG_DRAW_DOWN )
		{
			__mpd_draw_down_tiles_column();
		}

		__mpd_disp_list_flush();
		__mpd_update_scroll();
	}
}

void	__mpd_upd_scroll_left()
{
	u8 need_column;

	need_column = FALSE;

	if( ( __scroll_x & 0x07 ) == 0 )
	{
		need_column = TRUE;
	}

	__scroll_x -= __scroll_step;

	if( __scroll_x == 0 || need_column == FALSE )
	{
		return;
	}

	__upd_flags |= UPD_FLAG_DRAW_LEFT;
}

void	__mpd_upd_scroll_right()
{
	u8 need_column;

	need_column = FALSE;

	if( ( __scroll_x & 0x07 ) == 0 )
	{
		need_column = TRUE;
	}

	__scroll_x += __scroll_step;

	if( __scroll_x == 0 || need_column == FALSE )
	{
		return;
	}

	__upd_flags |= UPD_FLAG_DRAW_RIGHT;
}

void	__mpd_upd_scroll_up()
{
	u8 need_column;

	need_column = FALSE;

	if( ( __scroll_y & 0x07 ) == 0 )
	{
		need_column = TRUE;
	}

	__scroll_y -= __scroll_step;

	if( __scroll_y == 0 || need_column == FALSE )
	{
		return;
	}

	__upd_flags |= UPD_FLAG_DRAW_UP;
}

void	__mpd_upd_scroll_down()
{
	u8 need_column;

	need_column = FALSE;

	if( ( __scroll_y & 0x07 ) == 0 )
	{
		need_column = TRUE;
	}

	__scroll_y += __scroll_step;

	if( __scroll_y == 0 || need_column == FALSE )
	{
		return;
	}

	__upd_flags |= UPD_FLAG_DRAW_DOWN;
}

void	__mpd_draw_left_tiles_column()
{
	u16	tiles_offset;

#if	FLAG_MODE_BIDIR_SCROLL
	mpd_SCREEN* 	tmp_scr;

	tmp_scr		= __curr_scr;
	tiles_offset	= __horiz_dir_pos;

#if	FLAG_TILES4X4
	tiles_offset >>= 5;
#else
	tiles_offset >>= 4;
#endif
	// tiles column offset
#if	FLAG_DIR_COLUMNS
	tiles_offset = __scr_tiles_height_tbl[ tiles_offset ];		// tiles_offset *= ScrTilesHeight;
#endif
	tiles_offset += ( tmp_scr->scr_ind * __c_scr_tiles_size );

#else	//FLAG_MODE_BIDIR_SCROLL

	//...

#endif	//FLAG_MODE_BIDIR_SCROLL

	__mpd_fill_column_data( __mpd_get_VRAM_addr( __scroll_x, __scroll_y ), tiles_offset );
}

void	__mpd_draw_right_tiles_column()
{
	u16	tiles_offset;

#if	FLAG_MODE_BIDIR_SCROLL
	mpd_SCREEN* 	tmp_scr;

	tmp_scr		= __mpd_get_adj_screen( ADJ_SCR_RIGHT );
	tiles_offset	= __horiz_dir_pos;

#if	FLAG_TILES4X4
	tiles_offset >>= 5;
#else
	tiles_offset >>= 4;
#endif
	// tiles column offset
#if	FLAG_DIR_COLUMNS
	tiles_offset = __scr_tiles_height_tbl[ tiles_offset ];		// tiles_offset *= ScrTilesHeight;
#endif
	tiles_offset += ( tmp_scr->scr_ind * __c_scr_tiles_size );

#else	//FLAG_MODE_BIDIR_SCROLL

	//...

#endif	//FLAG_MODE_BIDIR_SCROLL

	__mpd_fill_column_data( __mpd_get_VRAM_addr( __scroll_x + ScrPixelsWidth, __scroll_y ), tiles_offset );
}

void	__mpd_draw_up_tiles_column()
{
	u16	tiles_offset;

#if	FLAG_MODE_BIDIR_SCROLL
	mpd_SCREEN* 	tmp_scr;

	tmp_scr		= __curr_scr;
	tiles_offset	= __vert_dir_pos;

#if	FLAG_TILES4X4
	tiles_offset >>= 5;
#else
	tiles_offset >>= 4;
#endif
	// tiles column offset
#if	FLAG_DIR_ROWS
	tiles_offset = __scr_tiles_width_tbl[ tiles_offset ];		// tiles_offset *= ScrTilesWidth;
#endif
	tiles_offset += ( tmp_scr->scr_ind * __c_scr_tiles_size );

#else	//FLAG_MODE_BIDIR_SCROLL

	//...

#endif	//FLAG_MODE_BIDIR_SCROLL

	__mpd_fill_row_data( __mpd_get_VRAM_addr( __scroll_x, __scroll_y ), tiles_offset );
}

void	__mpd_draw_down_tiles_column()
{
	u16	tiles_offset;

#if	FLAG_MODE_BIDIR_SCROLL
	mpd_SCREEN* 	tmp_scr;

	tmp_scr		= __mpd_get_adj_screen( ADJ_SCR_DOWN );
	tiles_offset	= __vert_dir_pos;

#if	FLAG_TILES4X4
	tiles_offset >>= 5;
#else
	tiles_offset >>= 4;
#endif
	// tiles column offset
#if	FLAG_DIR_ROWS
	tiles_offset = __scr_tiles_width_tbl[ tiles_offset ];		// tiles_offset *= ScrTilesWidth;
#endif
	tiles_offset += ( tmp_scr->scr_ind * __c_scr_tiles_size );

#else	//FLAG_MODE_BIDIR_SCROLL

	//...

#endif	//FLAG_MODE_BIDIR_SCROLL

	__mpd_fill_row_data( __mpd_get_VRAM_addr( __scroll_x, __scroll_y + ScrPixelsHeight ), tiles_offset );
}

void	__mpd_fill_column_data( u16 _vaddr, u16 _tiles_offset )
{
	u16	CHR_offset;
	u8	tile_y;
	u8	tile_n;
	u8	CHR_x_pos;
	u8	CHR_y_pos;

	u16	BAT_size;
	u16	last_data_addr;
	u8	data_part1;
	u8	data_part2;

	u8	chr_id_mul2;
	u16	blocks_offset_by_chr_id;

#if	FLAG_TILES4X4
	u8	block_x_pos;
	u16	tile4x4_offset;
	u16	tile4x4_offset_by_chr_id;

	block_x_pos	= ( __horiz_dir_pos >> 4 ) & 0x01;
	data_part1	= ScrTilesHeight << 2;
#else
	data_part1	= ScrTilesHeight << 1;
#endif
	chr_id_mul2	= __curr_chr_id << 1; 

	CHR_x_pos	= ( ( __horiz_dir_pos >> 3 ) & 0x01 ) << 1;

	BAT_size	= __BAT_size_dec1 + 1;
	last_data_addr	= _vaddr + ( data_part1 << __BAT_width_pow2 );

	if( last_data_addr > BAT_size )
	{
		data_part2 = ( ( last_data_addr - BAT_size ) & ~__BAT_width_dec1 ) >> __BAT_width_pow2;
		data_part1 -= data_part2;
	}
	else
	{
		data_part2 = 0;
	}

	__mpd_disp_list_push_hdr( DL_FLAG_DATA_INC_VERT, data_part1, _vaddr );

#if	MAPS_CNT != 1
	blocks_offset_by_chr_id	= mpd_farpeekw( mpd_BlocksOffs, chr_id_mul2 );
#endif
	tile_y = 0;

#if	FLAG_TILES4X4
#if	MAPS_CNT != 1
	tile4x4_offset_by_chr_id = mpd_farpeekw( mpd_TilesOffs, chr_id_mul2 );
#endif
	for( tile_n = 0; tile_n < ScrTilesHeight; tile_n++ )
	{
#if	FLAG_DIR_COLUMNS
		tile4x4_offset = ( mpd_farpeekb( mpd_TilesScr, _tiles_offset + tile_n ) << 2 ) + block_x_pos;
#else	//FLAG_DIR_ROWS
		tile4x4_offset = ( mpd_farpeekb( mpd_TilesScr, _tiles_offset + tile_y ) << 2 ) + block_x_pos;
		tile_y += ScrTilesWidth;
#endif
#if	MAPS_CNT != 1
		tile4x4_offset += tile4x4_offset_by_chr_id;
#endif
		// block 1
		CHR_offset = ( mpd_farpeekb( mpd_Tiles, tile4x4_offset ) << 3 ) + CHR_x_pos;
#if	MAPS_CNT != 1
		CHR_offset += blocks_offset_by_chr_id;
#endif
		__mpd_disp_list_push_data( mpd_farpeekw( mpd_Attrs, CHR_offset ) );
		if( !--data_part1 )	{ __mpd_push_data2_hdr( _vaddr, data_part2 ); }

		__mpd_disp_list_push_data( mpd_farpeekw( mpd_Attrs, CHR_offset + 4 ) );
		if( !--data_part1 )	{ __mpd_push_data2_hdr( _vaddr, data_part2 ); }

		// block 2
		CHR_offset = ( mpd_farpeekb( mpd_Tiles, tile4x4_offset + 2 ) << 3 ) + CHR_x_pos;
#if	MAPS_CNT != 1
		CHR_offset += blocks_offset_by_chr_id;
#endif
		__mpd_disp_list_push_data( mpd_farpeekw( mpd_Attrs, CHR_offset ) );
		if( !--data_part1 )	{ __mpd_push_data2_hdr( _vaddr, data_part2 ); }

		__mpd_disp_list_push_data( mpd_farpeekw( mpd_Attrs, CHR_offset + 4 ) );
		if( !--data_part1 )	{ __mpd_push_data2_hdr( _vaddr, data_part2 ); }
	}
#else	//FLAG_TILES2X2
	for( tile_n = 0; tile_n < ScrTilesHeight; tile_n++ )
	{
#if	FLAG_DIR_COLUMNS
		CHR_offset = ( mpd_farpeekb( mpd_TilesScr, _tiles_offset + tile_n ) << 3 ) + CHR_x_pos;
#else	//FLAG_DIR_ROWS
		CHR_offset = ( mpd_farpeekb( mpd_TilesScr, _tiles_offset + tile_y ) << 3 ) + CHR_x_pos;
		tile_y += ScrTilesWidth;
#endif
#if	MAPS_CNT != 1
		CHR_offset += blocks_offset_by_chr_id;
#endif
		__mpd_disp_list_push_data( mpd_farpeekw( mpd_Attrs, CHR_offset ) );
		if( !--data_part1 )	{ __mpd_push_data2_hdr( _vaddr, data_part2 ); }

		__mpd_disp_list_push_data( mpd_farpeekw( mpd_Attrs, CHR_offset + 4 ) );
		if( !--data_part1 )	{ __mpd_push_data2_hdr( _vaddr, data_part2 ); }
	}
#endif	//FLAG_TILES4X4

	__mpd_disp_list_end();
}

void	__mpd_push_data2_hdr( u16 _vaddr, u8 _size )
{
	__mpd_disp_list_push_hdr( DL_FLAG_DATA_INC_VERT, _size, _vaddr & __BAT_width_dec1 );
}

void	__mpd_fill_row_data( u16 _vaddr, u16 _tiles_offset )
{
	u16	CHR_offset;
	u8	tile_x;
	u8	tile_n;
	u8	CHR_y_pos;

	u8	chr_id_mul2;
	u16	blocks_offset_by_chr_id;

#if	FLAG_TILES4X4
	u8	block_y_pos;
	u16	tile4x4_offset;
	u16	tile4x4_offset_by_chr_id;

	block_y_pos	= ( ( __vert_dir_pos >> 4 ) & 0x01 ) << 1;

	__mpd_disp_list_push_hdr( DL_FLAG_DATA_INC1, ScrTilesWidth << 2, _vaddr );
#else
	__mpd_disp_list_push_hdr( DL_FLAG_DATA_INC1, ScrTilesWidth << 1, _vaddr );
#endif
	chr_id_mul2	= __curr_chr_id << 1; 

	CHR_y_pos	= ( ( __vert_dir_pos >> 3 ) & 0x01 ) << 2;

#if	MAPS_CNT != 1
	blocks_offset_by_chr_id	= mpd_farpeekw( mpd_BlocksOffs, chr_id_mul2 );
#endif
	tile_x = 0;

#if	FLAG_TILES4X4
#if	MAPS_CNT != 1
	tile4x4_offset_by_chr_id = mpd_farpeekw( mpd_TilesOffs, chr_id_mul2 );
#endif
	for( tile_n = 0; tile_n < ScrTilesWidth; tile_n++ )
	{
#if	FLAG_DIR_COLUMNS
		tile4x4_offset = ( mpd_farpeekb( mpd_TilesScr, _tiles_offset + tile_x ) << 2 ) + block_y_pos;
		tile_x += ScrTilesHeight;
#else	//FLAG_DIR_ROWS
		tile4x4_offset = ( mpd_farpeekb( mpd_TilesScr, _tiles_offset + tile_n ) << 2 ) + block_y_pos;
#endif
#if	MAPS_CNT != 1
		tile4x4_offset += tile4x4_offset_by_chr_id;
#endif
		// block 1
		CHR_offset = ( mpd_farpeekb( mpd_Tiles, tile4x4_offset ) << 3 ) + CHR_y_pos;
#if	MAPS_CNT != 1
		CHR_offset += blocks_offset_by_chr_id;
#endif
		__mpd_disp_list_push_data( mpd_farpeekw( mpd_Attrs, CHR_offset ) );
		__mpd_disp_list_push_data( mpd_farpeekw( mpd_Attrs, CHR_offset + 2 ) );

		// block 2
		CHR_offset = ( mpd_farpeekb( mpd_Tiles, tile4x4_offset + 1 ) << 3 ) + CHR_y_pos;
#if	MAPS_CNT != 1
		CHR_offset += blocks_offset_by_chr_id;
#endif
		__mpd_disp_list_push_data( mpd_farpeekw( mpd_Attrs, CHR_offset ) );
		__mpd_disp_list_push_data( mpd_farpeekw( mpd_Attrs, CHR_offset + 2 ) );
	}
#else	//FLAG_TILES2X2
	for( tile_n = 0; tile_n < ScrTilesWidth; tile_n++ )
	{
#if	FLAG_DIR_COLUMNS
		CHR_offset = ( mpd_farpeekb( mpd_TilesScr, _tiles_offset + tile_x ) << 3 ) + CHR_y_pos;
		tile_x += ScrTilesHeight;
#else	//FLAG_DIR_ROWS
		CHR_offset = ( mpd_farpeekb( mpd_TilesScr, _tiles_offset + tile_n ) << 3 ) + CHR_y_pos;
#endif
#if	MAPS_CNT != 1
		CHR_offset += blocks_offset_by_chr_id;
#endif
		__mpd_disp_list_push_data( mpd_farpeekw( mpd_Attrs, CHR_offset ) );
		__mpd_disp_list_push_data( mpd_farpeekw( mpd_Attrs, CHR_offset + 2 ) );
	}
#endif	//FLAG_TILES4X4

	__mpd_disp_list_end();
}

u16	__mpd_get_VRAM_addr( u16 _x, u16 _y )
{
	return ( ( ( _x >> 3 ) & __BAT_width_dec1 ) + ( ( ( _y >> 3 ) & __BAT_height_dec1 ) << __BAT_width_pow2 ) ) & __BAT_size_dec1;
}

void	__mpd_update_scroll()
{
	vsync();

	vreg( 7, __scroll_x );
	vreg( 8, __scroll_y );
}

u16	mpd_scroll_x()
{
	return __scroll_x;
}

u16	mpd_scroll_y()
{
	return __scroll_y;
}
#endif	//FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL

mpd_SCREEN*	mpd_curr_screen()
{
	return __curr_scr;
}

u8	mpd_get_property( u16 _x, u16 _y )
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