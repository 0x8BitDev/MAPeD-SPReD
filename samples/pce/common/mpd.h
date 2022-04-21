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
// void		mpd_init( u8 _map_ind, mpd_scroll_step _step )
// #else
// void		mpd_init( u8 _map_ind )
// #endif	//FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
//
// void		mpd_draw_screen()
// bool		mpd_check_adj_screen( u8 _ind ) / _ind: ADJ_SCR_LEFT,ADJ_SCR_RIGHT,ADJ_SCR_UP,ADJ_SCR_DOWN;
// u8		mpd_get_property( u16 _x, u16 _y ) / _x/_y - coordinates; result: property id
//
// #if  	!FLAG_MODE_MULTIDIR_SCROLL
// mpd_SCREEN*	mpd_curr_screen()
// #endif	//!FLAG_MODE_MULTIDIR_SCROLL
//
// #if		FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
// void		mpd_clear_update_flags()
// void		mpd_move_left()
// void		mpd_move_right()
// void		mpd_move_up()
// void		mpd_move_down()
// void		mpd_update_screen( bool _vsync )
// u16		mpd_offset_x()
// u16		mpd_offset_y()
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

	.macro mpd_farptr_add_offset  ; \1 - offset, \2 - bank number, \3 - address

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

	mpd_farptr_add_offset __dx, __bl, __si

	maplibfunc	lib2_load_palette
	rts
#endasm

#asm
_mpd_load_vram2.4:
	jmp _load_vram.3
#endasm

#asm
_mpd_load_vram.4:

	mpd_farptr_add_offset __ax, __bl, __si

	jmp _load_vram.3
#endasm

#asm
_mpd_load_bat.5:

	mpd_farptr_add_offset __ax, __bl, __si

	maplibfunc	lib2_load_bat
	rts
#endasm

#asm
_mpd_farpeekw.2:
	mpd_farptr_add_offset __ax, __fbank, __fptr

	jmp _farpeekw.1
#endasm

#asm
_mpd_farpeekb.2:

	mpd_farptr_add_offset __ax, __fbank, __fptr

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

bool	__mpd_disp_list_empty()
{
	return __dl_pos != 0 ? FALSE:TRUE;
}

#endif	//FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL

/*****************/
/*		 */
/* RLE functions */
/*		 */
/*****************/
#if	FLAG_RLE * FLAG_MODE_STAT_SCR
u16	__stat_scr_buff[ ScrGfxDataSize >> 1 ];

void	__mpd_UNRLE_stat_scr( u16 _offset )
{
	u16	src;
	u16	dst;
	u16 	val1;
	u16 	val2;
	u16	val3;
	u16	cnt;

	src = _offset;
	dst = 0;

	val1 = mpd_farpeekw( mpd_VDCScr, src );
	src += 2;

	for(;;)
	{
		val2 = mpd_farpeekw( mpd_VDCScr, src );
		src += 2;

		if( val1 != val2 )
		{
			__stat_scr_buff[ dst++ ] = val2;

			val3 = val2;

			continue;
		}

		cnt = mpd_farpeekw( mpd_VDCScr, src );
		src += 2;

		if( !cnt )
		{
			return;
		}

		do
		{
			__stat_scr_buff[ dst++ ] = val3;
		}
		while( --cnt );
	}
}
#endif	//FLAG_RLE * FLAG_MODE_STAT_SCR

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
#if	!FLAG_MODE_MULTIDIR_SCROLL
mpd_SCREEN*	__curr_scr;
u16		__scr_offset;

u16		__scr_tiles_width_tbl[ ScrTilesHeight ];
u16		__scr_tiles_height_tbl[ ScrTilesWidth ];
#endif	//!FLAG_MODE_MULTIDIR_SCROLL

#if	FLAG_MODE_MULTIDIR_SCROLL
u16		__map_tiles_width;
u16		__map_tiles_height;
u16		__cropped_map_width;	// active map area = map_width_in_pixels - scr_width_in_pixels
u16		__cropped_map_height;	// the same in height
u16		__init_tiles_offset;	// to draw a start screen

u16		__maps_offset;
u16		__maps_tbl_offset;

u16		__map_ind_mul2;

#if	FLAG_DIR_ROWS
u16		__height_scr_step;
#endif	//FLAG_DIR_ROWS
#endif	//FLAG_MODE_MULTIDIR_SCROLL

u8		__curr_chr_id_mul2;
u8		__BAT_width;
u8		__BAT_width_dec1;
u8		__BAT_height_dec1;
u8		__BAT_width_pow2;
u16		__BAT_size_dec1;	// ( BAT_width * BAT_height ) - 1

u16		__props_offset;
u16		__blocks_offset;
#if	FLAG_TILES4X4
u16		__tiles_offset;
#endif

#define	SCR_CHRS_WIDTH	( SCR_BLOCKS2x2_WIDTH << 1 )
#define	SCR_CHRS_HEIGHT	( SCR_BLOCKS2x2_HEIGHT << 1 )

#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
#if	FLAG_MODE_MULTIDIR_SCROLL
#define	COLUMN_CHRS_CNT	SCR_CHRS_HEIGHT + 1
#define	ROW_CHRS_CNT	SCR_CHRS_WIDTH + 1
#else
#define	COLUMN_CHRS_CNT	SCR_CHRS_HEIGHT
#define	ROW_CHRS_CNT	SCR_CHRS_WIDTH
#endif

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

#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
void	mpd_init( u8 _map_ind, mpd_scroll_step _step )
#else
void	mpd_init( u8 _map_ind )
#endif
{
#if	FLAG_MODE_MULTIDIR_SCROLL
	u8	map_scr_width;
	u8	map_scr_height;
	u16	start_scr;
	u16	LUT_pos_x;
	u16	LUT_pos_y;
	u16	LUT_offset;
#endif	//FLAG_MODE_MULTIDIR_SCROLL

#if	!FLAG_MODE_MULTIDIR_SCROLL
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

	__curr_scr	= *mpd_MapsArr[ _map_ind ];
	__scr_offset	= __curr_scr->scr_ind * __c_scr_tiles_size;

#endif	//!FLAG_MODE_MULTIDIR_SCROLL

#if	FLAG_LAYOUT_ADJ_SCR_INDS
	__scr_arr	= mpd_MapsScrArr[ _map_ind ];
#endif	//FLAG_LAYOUT_ADJ_SCR_INDS

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
#endif	//FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL

#if	FLAG_MODE_MULTIDIR_SCROLL

	__map_ind_mul2		= _map_ind << 1;

	__curr_chr_id_mul2	= mpd_farpeekb( mpd_MapsCHRBanks, _map_ind ) << 1;

	__maps_offset		= mpd_farpeekw( mpd_MapsOffs,	__map_ind_mul2 );
	__maps_tbl_offset	= mpd_farpeekw( mpd_MapsTblOffs,__map_ind_mul2 );

	map_scr_width	= mpd_MapsDimArr[ __map_ind_mul2 ];
	map_scr_height	= mpd_MapsDimArr[ __map_ind_mul2 + 1 ];

	__map_tiles_width	= map_scr_width * ScrTilesWidth;
	__map_tiles_height	= map_scr_height * ScrTilesHeight;

	__cropped_map_width	= ( map_scr_width * ScrPixelsWidth ) - ScrPixelsWidth;
	__cropped_map_height	= ( map_scr_height * ScrPixelsHeight ) - ScrPixelsHeight;

	start_scr	= mpd_StartScrArr[ _map_ind ];

	LUT_pos_x	= ScrTilesWidth * ( start_scr % map_scr_width );

#if	FLAG_TILES2X2
	__scroll_x	= LUT_pos_x << 4;
#elif	FLAG_TILES4X4
	__scroll_x	= LUT_pos_x << 5;
#endif	//FLAG_TILES2X2|FLAG_TILES4X4

	LUT_pos_y	= ScrTilesHeight * ( start_scr / map_scr_width );

#if	FLAG_TILES2X2
	__scroll_y	= LUT_pos_y << 4;
#elif	FLAG_TILES4X4
	__scroll_y	= LUT_pos_y << 5;
#endif	//FLAG_TILES2X2|FLAG_TILES4X4

#if	FLAG_DIR_COLUMNS
	LUT_offset		= __maps_tbl_offset + ( LUT_pos_x << 1 );
	__init_tiles_offset	= mpd_farpeekw( mpd_MapsTbl, LUT_offset ) + LUT_pos_y;
#elif	FLAG_DIR_ROWS
	__height_scr_step	= __map_tiles_width * ScrTilesHeight;
	LUT_offset		= __maps_tbl_offset + ( LUT_pos_y << 1 );
	__init_tiles_offset	= mpd_farpeekw( mpd_MapsTbl, LUT_offset ) + LUT_pos_x;
#endif	//FLAG_DIR_COLUMNS|FLAG_DIR_ROWS

	__horiz_dir_pos	= __scroll_x;
	__vert_dir_pos	= __scroll_y;

	__mpd_update_data_offsets();

#else	//FLAG_MODE_MULTIDIR_SCROLL
	__curr_chr_id_mul2	= 0xff;
#endif	//FLAG_MODE_MULTIDIR_SCROLL

#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
	__mpd_update_scroll( TRUE );
#endif	//FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
}

void	__mpd_update_data_offsets()
{
	__props_offset	= mpd_farpeekw( mpd_PropsOffs,	__curr_chr_id_mul2 );
	__blocks_offset	= mpd_farpeekw( mpd_BlocksOffs,	__curr_chr_id_mul2 );

#if	FLAG_TILES4X4
	__tiles_offset	= mpd_farpeekw( mpd_TilesOffs,	__curr_chr_id_mul2 );
#endif
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

	tiles12 = mpd_farpeekw( mpd_Tiles, _offset );
	tiles34 = mpd_farpeekw( mpd_Tiles, _offset + 2 );

	__mpd_draw_block2x2( _vaddr, __blocks_offset + ( ( tiles12 & 0x00ff ) << 3 ) );
	_vaddr += 2;
	__mpd_draw_block2x2( _vaddr, __blocks_offset + ( ( tiles12 & 0xff00 ) >> 5 ) );
	_vaddr += ( __BAT_width << 1 ) - 2;
	__mpd_draw_block2x2( _vaddr, __blocks_offset + ( ( tiles34 & 0x00ff ) << 3 ) );
	_vaddr += 2;
	__mpd_draw_block2x2( _vaddr, __blocks_offset + ( ( tiles34 & 0xff00 ) >> 5 ) );
}
#endif	//FLAG_TILES4X4
#endif	//FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL + FLAG_MODE_BIDIR_STAT_SCR

#if	FLAG_MODE_BIDIR_SCROLL + FLAG_MODE_BIDIR_STAT_SCR
void	__mpd_draw_tiled_screen()
{
	u8	scr_tile;
	u16	w, h;
	u16	h_acc;
	u16	n;

	n = 0;

#if	FLAG_DIR_COLUMNS

	for( w = 0; w < ScrTilesWidth; w++ )
	{
		h_acc = 0;

		for( h = 0; h < ScrTilesHeight; h++ )
		{
			scr_tile = mpd_farpeekb( mpd_TilesScr, __scr_offset + n );
#if	FLAG_TILES2X2
			__mpd_draw_block2x2( ( h_acc + w ) << 1, __blocks_offset + ( scr_tile << 3 ) );
#elif	FLAG_TILES4X4
			__mpd_draw_tile4x4( ( h_acc + w ) << 2, __tiles_offset + ( scr_tile << 2 ) );
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
			scr_tile = mpd_farpeekb( mpd_TilesScr, __scr_offset + n );
#if	FLAG_TILES2X2
			__mpd_draw_block2x2( ( h_acc + w ) << 1, __blocks_offset + ( scr_tile << 3 ) );
#elif	FLAG_TILES4X4
			__mpd_draw_tile4x4( ( h_acc + w ) << 2, __tiles_offset + ( scr_tile << 2 ) );
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
	u8	scr_tile;
	u16	w, h;
	u16	h_acc;
	u16	n;
	u16	tiles_offset;
	u16	vaddr;
	u16	side_step;

	tiles_offset	= __maps_offset + __init_tiles_offset;

	vaddr		= ( ( __scroll_x >> 3 ) & __BAT_width_dec1 ) + ( ( ( __scroll_y >> 3 ) /*& __BAT_height_dec1*/ ) << __BAT_width_pow2 );

#if	FLAG_DIR_COLUMNS

	side_step	= ScrTilesHeight * mpd_MapsDimArr[ __map_ind_mul2 + 1 ];

	for( w = 0; w < ScrTilesWidth; w++ )
	{
		h_acc = 0;

		n = w * side_step;

		for( h = 0; h < ScrTilesHeight; h++ )
		{
			scr_tile = mpd_farpeekb( mpd_Maps, tiles_offset + n );
#if	FLAG_TILES2X2
			__mpd_draw_block2x2( ( vaddr + ( ( h_acc + w ) << 1 ) ) & __BAT_size_dec1, __blocks_offset + ( scr_tile << 3 ) );
#elif	FLAG_TILES4X4
			__mpd_draw_tile4x4( ( vaddr + ( ( h_acc + w ) << 2 ) ) & __BAT_size_dec1, __tiles_offset + ( scr_tile << 2 ) );
#endif
			h_acc += __BAT_width;
			++n;
		}
	}
#elif	FLAG_DIR_ROWS

	side_step	= ScrTilesWidth * mpd_MapsDimArr[ __map_ind_mul2 ];

	h_acc = 0;

	for( h = 0; h < ScrTilesHeight; h++ )
	{
		n = h * side_step;

		for( w = 0; w < ScrTilesWidth; w++ )
		{
			scr_tile = mpd_farpeekb( mpd_Maps, tiles_offset + n );
#if	FLAG_TILES2X2
			__mpd_draw_block2x2( ( vaddr + ( ( h_acc + w ) << 1 ) ) & __BAT_size_dec1, __blocks_offset + ( scr_tile << 3 ) );
#elif	FLAG_TILES4X4
			__mpd_draw_tile4x4( ( vaddr + ( ( h_acc + w ) << 2 ) ) & __BAT_size_dec1, __tiles_offset + ( scr_tile << 2 ) );
#endif
			++n;
		}

		h_acc += __BAT_width;
	}

#endif	//FLAG_DIR_COLUMNS|FLAG_DIR_ROWS
}
#endif	//FLAG_MODE_MULTIDIR_SCROLL

void	mpd_draw_screen()
{
	u16	num_CHRs;

	// load tiles & palette

#if	FLAG_MODE_MULTIDIR_SCROLL
	{
		num_CHRs = mpd_farpeekw( mpd_CHRs_size, __curr_chr_id_mul2 ) >> 5;

#else
	u8	chr_id_mul2;

	chr_id_mul2	= __curr_scr->chr_id << 1;
	num_CHRs	= mpd_farpeekw( mpd_CHRs_size, chr_id_mul2 ) >> 5;

	if( __curr_scr->chr_id != ( __curr_chr_id_mul2 >> 1 ) )
	{
		__curr_chr_id_mul2 = __curr_scr->chr_id << 1;

		__mpd_update_data_offsets();

#endif	//FLAG_MODE_MULTIDIR_SCROLL

		mpd_load_vram2( CHRS_OFFSET << 5, mpd_CHRs[ __curr_chr_id_mul2 ], mpd_CHRs[ __curr_chr_id_mul2 + 1 ], num_CHRs << 4 );
		mpd_load_palette( 0, mpd_Plts, __curr_chr_id_mul2 << 8, 16 );
	}

	// load BAT

#if	FLAG_MODE_STAT_SCR
#if	FLAG_RLE
	__mpd_UNRLE_stat_scr( __curr_scr->VDC_data_offset );
	mpd_load_bat( 0, __stat_scr_buff, 0, SCR_CHRS_WIDTH, SCR_CHRS_HEIGHT );
#else
	mpd_load_bat( 0, mpd_VDCScr, __curr_scr->VDC_data_offset, SCR_CHRS_WIDTH, SCR_CHRS_HEIGHT );
#endif	//FLAG_RLE
#elif	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL + FLAG_MODE_BIDIR_STAT_SCR
	__mpd_draw_tiled_screen();
#endif
}

#if	!FLAG_MODE_MULTIDIR_SCROLL
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

bool	mpd_check_adj_screen( u8 _ind )
{
	mpd_SCREEN* adj_scr;

	if( adj_scr = __mpd_get_adj_screen( _ind ) )
	{
		__curr_scr	= adj_scr;
		__scr_offset	= __curr_scr->scr_ind * __c_scr_tiles_size;

		return TRUE;
	}

	return FALSE;
}

mpd_SCREEN*	mpd_curr_screen()
{
	return __curr_scr;
}
#endif	//!FLAG_MODE_MULTIDIR_SCROLL

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
	if( __horiz_dir_pos > 0 )
	{
		__horiz_dir_pos -= __scroll_step;
		__mpd_upd_scroll_left();
	}
}

void	mpd_move_right()
{
	if( __horiz_dir_pos < __cropped_map_width )
	{
		__horiz_dir_pos += __scroll_step;
		__mpd_upd_scroll_right();
	}
	else
	{
		__horiz_dir_pos = __cropped_map_width;
	}
}

void	mpd_move_up()
{
	if( __vert_dir_pos > 0 )
	{
		__vert_dir_pos -= __scroll_step;
		__mpd_upd_scroll_up();
	}
}

void	mpd_move_down()
{
	if( __vert_dir_pos < __cropped_map_height )
	{
		__vert_dir_pos += __scroll_step;
		__mpd_upd_scroll_down();
	}
	else
	{
		__vert_dir_pos = __cropped_map_height;
	}
}	
#endif	//FLAG_MODE_MULTIDIR_SCROLL

#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
void	mpd_clear_update_flags()
{
	__upd_flags &= ~(UPD_FLAG_DRAW_MASK);
}

void	mpd_update_screen( bool _vsync )
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
			__mpd_draw_up_tiles_row();
		}

		if( __upd_flags & UPD_FLAG_DRAW_DOWN )
		{
			__mpd_draw_down_tiles_row();
		}

		__mpd_disp_list_flush();
		__mpd_update_scroll( _vsync );
	}
}

void	__mpd_upd_scroll_left()
{
	bool need_column;

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
	bool need_column;

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
	bool need_column;

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
	bool need_column;

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

	__mpd_fill_column_data( __mpd_get_VRAM_addr( __scroll_x, __scroll_y ), tiles_offset, COLUMN_CHRS_CNT );

#else	//FLAG_MODE_MULTIDIR_SCROLL
	u16	map_pos_x;
	u16	map_pos_y;
	u8	u8_pos_y;

	map_pos_x	= __horiz_dir_pos;
	map_pos_y	= __vert_dir_pos;
	u8_pos_y	= map_pos_y;

#if	FLAG_TILES4X4
	map_pos_x >>= 5;
	map_pos_y >>= 5;
#else
	map_pos_x >>= 4;
	map_pos_y >>= 4;
#endif

#if	FLAG_DIR_ROWS
	tiles_offset	= __maps_tbl_offset + ( map_pos_y << 1 );
	tiles_offset	= mpd_farpeekw( mpd_MapsTbl, tiles_offset ) + map_pos_x;
#else
	tiles_offset	= __maps_tbl_offset + ( map_pos_x << 1 );
	tiles_offset	= mpd_farpeekw( mpd_MapsTbl, tiles_offset ) + map_pos_y;
#endif
	tiles_offset	+= __maps_offset;

	__mpd_fill_column_data( __mpd_get_VRAM_addr( __scroll_x, __scroll_y ), tiles_offset, COLUMN_CHRS_CNT, __mpd_calc_skip_CHRs_cnt( u8_pos_y ) );

#endif	//FLAG_MODE_BIDIR_SCROLL
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

	__mpd_fill_column_data( __mpd_get_VRAM_addr( __scroll_x + ScrPixelsWidth, __scroll_y ), tiles_offset, COLUMN_CHRS_CNT );

#else	//FLAG_MODE_MULTIDIR_SCROLL
	u16	map_pos_x;
	u16	map_pos_y;
	u8	u8_pos_y;

	map_pos_x	= __horiz_dir_pos;
	map_pos_y	= __vert_dir_pos;
	u8_pos_y	= map_pos_y;

#if	FLAG_TILES4X4
	map_pos_x >>= 5;
	map_pos_y >>= 5;
#else
	map_pos_x >>= 4;
	map_pos_y >>= 4;
#endif

#if	FLAG_DIR_ROWS
	tiles_offset	= __maps_tbl_offset + ( map_pos_y << 1 );
	tiles_offset	= mpd_farpeekw( mpd_MapsTbl, tiles_offset ) + map_pos_x + ScrTilesWidth;
#else	
	tiles_offset	= __maps_tbl_offset + ( ( map_pos_x + ScrTilesWidth ) << 1 );
	tiles_offset	= mpd_farpeekw( mpd_MapsTbl, tiles_offset ) + map_pos_y;
#endif
	tiles_offset	+= __maps_offset;

	__mpd_fill_column_data( __mpd_get_VRAM_addr( __scroll_x + ScrPixelsWidth, __scroll_y ), tiles_offset, COLUMN_CHRS_CNT, __mpd_calc_skip_CHRs_cnt( u8_pos_y ) );

#endif	//FLAG_MODE_BIDIR_SCROLL
}

void	__mpd_draw_up_tiles_row()
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

	__mpd_fill_row_data( __mpd_get_VRAM_addr( __scroll_x, __scroll_y ), tiles_offset, ROW_CHRS_CNT );

#else	//FLAG_MODE_MULTIDIR_SCROLL
	u16	map_pos_x;
	u16	map_pos_y;
	u8	u8_pos_x;

	map_pos_x	= __horiz_dir_pos;
	map_pos_y	= __vert_dir_pos;
	u8_pos_x	= map_pos_x;

#if	FLAG_TILES4X4
	map_pos_x >>= 5;
	map_pos_y >>= 5;
#else
	map_pos_x >>= 4;
	map_pos_y >>= 4;
#endif

#if	FLAG_DIR_ROWS
	tiles_offset	= __maps_tbl_offset + ( map_pos_y << 1 );
	tiles_offset	= mpd_farpeekw( mpd_MapsTbl, tiles_offset ) + map_pos_x;
#else
	tiles_offset	= __maps_tbl_offset + ( map_pos_x << 1 );
	tiles_offset	= mpd_farpeekw( mpd_MapsTbl, tiles_offset ) + map_pos_y;
#endif
	tiles_offset	+= __maps_offset;

	__mpd_fill_row_data( __mpd_get_VRAM_addr( __scroll_x, __scroll_y ), tiles_offset, ROW_CHRS_CNT, __mpd_calc_skip_CHRs_cnt( u8_pos_x ) );

#endif	//FLAG_MODE_BIDIR_SCROLL
}

void	__mpd_draw_down_tiles_row()
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

	__mpd_fill_row_data( __mpd_get_VRAM_addr( __scroll_x, __scroll_y + ScrPixelsHeight ), tiles_offset, ROW_CHRS_CNT );

#else	//FLAG_MODE_MULTIDIR_SCROLL
	u16	map_pos_x;
	u16	map_pos_y;
	u8	u8_pos_x;

	map_pos_x	= __horiz_dir_pos;
	map_pos_y	= __vert_dir_pos;
	u8_pos_x	= map_pos_x;

#if	FLAG_TILES4X4
	map_pos_x >>= 5;
	map_pos_y >>= 5;
#else
	map_pos_x >>= 4;
	map_pos_y >>= 4;
#endif

#if	FLAG_DIR_ROWS
	tiles_offset	= __maps_tbl_offset + ( map_pos_y << 1 );
	tiles_offset	= mpd_farpeekw( mpd_MapsTbl, tiles_offset ) + map_pos_x + __height_scr_step;
#else
	tiles_offset	= __maps_tbl_offset + ( map_pos_x << 1 );
	tiles_offset	= mpd_farpeekw( mpd_MapsTbl, tiles_offset ) + map_pos_y + ScrTilesHeight;
#endif
	tiles_offset	+= __maps_offset;

	__mpd_fill_row_data( __mpd_get_VRAM_addr( __scroll_x, __scroll_y + ScrPixelsHeight ), tiles_offset, ROW_CHRS_CNT, __mpd_calc_skip_CHRs_cnt( u8_pos_x ) );

#endif	//FLAG_MODE_BIDIR_SCROLL
}

#if	FLAG_MODE_MULTIDIR_SCROLL
u8	__mpd_calc_skip_CHRs_cnt( u8 _pos )
{
#if	FLAG_TILES4X4
	return ( ( ( _pos >> 4 ) & 0x01 ) << 1 ) + ( ( _pos >> 3 ) & 0x01 );
#else
	return ( ( _pos >> 3 ) & 0x01 );
#endif
}

void	__mpd_fill_column_data( u16 _vaddr, u16 _tiles_offset, u8 _CHRs_cnt, u8 _skip_CHRs_cnt )
#else
void	__mpd_fill_column_data( u16 _vaddr, u16 _tiles_offset, u8 _CHRs_cnt )
#endif	//FLAG_MODE_MULTIDIR_SCROLL
{
	u16	CHR_offset;
#if	FLAG_MODE_MULTIDIR_SCROLL
	u16	tile_n;
#else
	u8	tile_n;
#endif
	u8	CHR_x_pos;

	u16	BAT_size;
	u16	last_data_addr;
	u8	data_part1;
	u8	data_part2;

#if	FLAG_TILES4X4
	u8	block_x_pos;
	u16	tile4x4_offset;

	block_x_pos	= ( __horiz_dir_pos >> 4 ) & 0x01;
#endif
	data_part1	= _CHRs_cnt;
	data_part2	= 0;

	CHR_x_pos	= ( ( __horiz_dir_pos >> 3 ) & 0x01 ) << 1;

	// loop the _vaddr vertically
	BAT_size	= __BAT_size_dec1 + 1;
	last_data_addr	= _vaddr + ( data_part1 << __BAT_width_pow2 );

	if( last_data_addr > BAT_size )
	{
		data_part2 = ( ( last_data_addr - BAT_size ) & ~__BAT_width_dec1 ) >> __BAT_width_pow2;
		data_part1 -= data_part2;
	}

	__mpd_disp_list_push_hdr( DL_FLAG_DATA_INC_VERT, data_part1, _vaddr );

	tile_n = 0;

#if	FLAG_TILES4X4
	while( TRUE )
	{
#if	FLAG_MODE_BIDIR_SCROLL
		tile4x4_offset = ( mpd_farpeekb( mpd_TilesScr, _tiles_offset + tile_n ) << 2 ) + block_x_pos;
#else
		tile4x4_offset = ( mpd_farpeekb( mpd_Maps, _tiles_offset + tile_n ) << 2 ) + block_x_pos;
#endif	//FLAG_MODE_BIDIR_SCROLL

#if	FLAG_DIR_COLUMNS
		++tile_n;
#else	//FLAG_DIR_ROWS
#if	FLAG_MODE_BIDIR_SCROLL
		tile_n += ScrTilesWidth;
#else
		tile_n += __map_tiles_width;
#endif	//FLAG_MODE_BIDIR_SCROLL
#endif	//FLAG_DIR_COLUMNS
#if	MAPS_CNT != 1
		tile4x4_offset += __tiles_offset;
#endif
		// block 1
		CHR_offset = ( mpd_farpeekb( mpd_Tiles, tile4x4_offset ) << 3 ) + CHR_x_pos;
#if	MAPS_CNT != 1
		CHR_offset += __blocks_offset;
#endif
#if	FLAG_MODE_MULTIDIR_SCROLL
		if( !_skip_CHRs_cnt )
		{
#endif
		__mpd_disp_list_push_data( mpd_farpeekw( mpd_Attrs, CHR_offset ) );
		if( !--_CHRs_cnt )	{ break; }
		if( !--data_part1 )	{ __mpd_push_data2_col_hdr( _vaddr, data_part2 ); }
#if	FLAG_MODE_MULTIDIR_SCROLL
		}
		else
		{ --_skip_CHRs_cnt; }

		if( !_skip_CHRs_cnt )
		{
#endif
		__mpd_disp_list_push_data( mpd_farpeekw( mpd_Attrs, CHR_offset + 4 ) );
		if( !--_CHRs_cnt )	{ break; }
		if( !--data_part1 )	{ __mpd_push_data2_col_hdr( _vaddr, data_part2 ); }
#if	FLAG_MODE_MULTIDIR_SCROLL
		}
		else
		{ --_skip_CHRs_cnt; }
#endif
		// block 2
		CHR_offset = ( mpd_farpeekb( mpd_Tiles, tile4x4_offset + 2 ) << 3 ) + CHR_x_pos;
#if	MAPS_CNT != 1
		CHR_offset += __blocks_offset;
#endif
#if	FLAG_MODE_MULTIDIR_SCROLL
		if( !_skip_CHRs_cnt )
		{
#endif
		__mpd_disp_list_push_data( mpd_farpeekw( mpd_Attrs, CHR_offset ) );
		if( !--_CHRs_cnt )	{ break; }
		if( !--data_part1 )	{ __mpd_push_data2_col_hdr( _vaddr, data_part2 ); }
#if	FLAG_MODE_MULTIDIR_SCROLL
		}
		else
		{ --_skip_CHRs_cnt; }

		if( !_skip_CHRs_cnt )
		{
#endif
		__mpd_disp_list_push_data( mpd_farpeekw( mpd_Attrs, CHR_offset + 4 ) );
		if( !--_CHRs_cnt )	{ break; }
		if( !--data_part1 )	{ __mpd_push_data2_col_hdr( _vaddr, data_part2 ); }
#if	FLAG_MODE_MULTIDIR_SCROLL
		}
		else
		{ --_skip_CHRs_cnt; }
#endif
	}
#else	//FLAG_TILES2X2
	while( TRUE )
	{
#if	FLAG_MODE_BIDIR_SCROLL
		CHR_offset = ( mpd_farpeekb( mpd_TilesScr, _tiles_offset + tile_n ) << 3 ) + CHR_x_pos;
#else
		CHR_offset = ( mpd_farpeekb( mpd_Maps, _tiles_offset + tile_n ) << 3 ) + CHR_x_pos;
#endif	//FLAG_MODE_BIDIR_SCROLL

#if	FLAG_DIR_COLUMNS
		++tile_n;
#else	//FLAG_DIR_ROWS
#if	FLAG_MODE_BIDIR_SCROLL
		tile_n += ScrTilesWidth;
#else
		tile_n += __map_tiles_width;
#endif	//FLAG_MODE_BIDIR_SCROLL
#endif	//FLAG_DIR_COLUMNS
#if	MAPS_CNT != 1
		CHR_offset += __blocks_offset;
#endif
#if	FLAG_MODE_MULTIDIR_SCROLL
		if( !_skip_CHRs_cnt )
		{
#endif
		__mpd_disp_list_push_data( mpd_farpeekw( mpd_Attrs, CHR_offset ) );
		if( !--_CHRs_cnt )	{ break; }
		if( !--data_part1 )	{ __mpd_push_data2_col_hdr( _vaddr, data_part2 ); }
#if	FLAG_MODE_MULTIDIR_SCROLL
		}
		else
		{ --_skip_CHRs_cnt; }
#endif
		__mpd_disp_list_push_data( mpd_farpeekw( mpd_Attrs, CHR_offset + 4 ) );
		if( !--_CHRs_cnt )	{ break; }
		if( !--data_part1 )	{ __mpd_push_data2_col_hdr( _vaddr, data_part2 ); }
	}
#endif	//FLAG_TILES4X4

	__mpd_disp_list_end();
}

void	__mpd_push_data2_col_hdr( u16 _vaddr, u8 _size )
{
	__mpd_disp_list_push_hdr( DL_FLAG_DATA_INC_VERT, _size, _vaddr & __BAT_width_dec1 );
}

#if	FLAG_MODE_MULTIDIR_SCROLL
void	__mpd_fill_row_data( u16 _vaddr, u16 _tiles_offset, u8 _CHRs_cnt, u8 _skip_CHRs_cnt )
#else
void	__mpd_fill_row_data( u16 _vaddr, u16 _tiles_offset, u8 _CHRs_cnt )
#endif	//FLAG_MODE_MULTIDIR_SCROLL
{
	u16	CHR_offset;
#if	FLAG_MODE_MULTIDIR_SCROLL
	u16	tile_n;
#else
	u8	tile_n;
#endif
	u8	CHR_y_pos;

	u16	last_data_addr;
	u16	BAT_width_dec1_inv;
	u8	data_part1;
	u8	data_part2;

#if	FLAG_TILES4X4
	u8	block_y_pos;
	u16	tile4x4_offset;

	block_y_pos	= ( ( __vert_dir_pos >> 4 ) & 0x01 ) << 1;
#endif
	data_part1	= _CHRs_cnt;
	data_part2	= 0;

#if	FLAG_MODE_MULTIDIR_SCROLL + ( FLAG_MODE_BIDIR_SCROLL * ( SCR_BLOCKS2x2_WIDTH != 16 ) )
	{
		// loop the _vaddr horizontally
		last_data_addr	= _vaddr + _CHRs_cnt;

		BAT_width_dec1_inv	= __BAT_width_dec1;
		BAT_width_dec1_inv	= ~BAT_width_dec1_inv;

		if( ( last_data_addr & BAT_width_dec1_inv ) != ( _vaddr & BAT_width_dec1_inv ) )
		{
			data_part2 = last_data_addr & __BAT_width_dec1;
			data_part1 -= data_part2;
		}
	}
#endif
	__mpd_disp_list_push_hdr( DL_FLAG_DATA_INC1, data_part1, _vaddr );

	CHR_y_pos	= ( ( __vert_dir_pos >> 3 ) & 0x01 ) << 2;

	tile_n = 0;

#if	FLAG_TILES4X4
	while( TRUE )
	{
#if	FLAG_MODE_BIDIR_SCROLL		
		tile4x4_offset = ( mpd_farpeekb( mpd_TilesScr, _tiles_offset + tile_n ) << 2 ) + block_y_pos;
#else
		tile4x4_offset = ( mpd_farpeekb( mpd_Maps, _tiles_offset + tile_n ) << 2 ) + block_y_pos;
#endif	//FLAG_MODE_BIDIR_SCROLL

#if	FLAG_DIR_COLUMNS
#if	FLAG_MODE_BIDIR_SCROLL
		tile_n += ScrTilesHeight;
#else
		tile_n += __map_tiles_height;
#endif	//FLAG_MODE_BIDIR_SCROLL
#else	//FLAG_DIR_ROWS
		++tile_n;
#endif	//FLAG_DIR_COLUMNS
#if	MAPS_CNT != 1
		tile4x4_offset += __tiles_offset;
#endif
		// block 1
		CHR_offset = ( mpd_farpeekb( mpd_Tiles, tile4x4_offset ) << 3 ) + CHR_y_pos;
#if	MAPS_CNT != 1
		CHR_offset += __blocks_offset;
#endif
#if	FLAG_MODE_MULTIDIR_SCROLL
		if( !_skip_CHRs_cnt )
		{
#endif
		__mpd_disp_list_push_data( mpd_farpeekw( mpd_Attrs, CHR_offset ) );
		if( !--_CHRs_cnt ) { break; }
		if( !--data_part1 )	{ __mpd_push_data2_row_hdr( _vaddr, data_part2 ); }
#if	FLAG_MODE_MULTIDIR_SCROLL
		}
		else
		{ --_skip_CHRs_cnt; }

		if( !_skip_CHRs_cnt )
		{
#endif
		__mpd_disp_list_push_data( mpd_farpeekw( mpd_Attrs, CHR_offset + 2 ) );
		if( !--_CHRs_cnt ) { break; }
		if( !--data_part1 )	{ __mpd_push_data2_row_hdr( _vaddr, data_part2 ); }
#if	FLAG_MODE_MULTIDIR_SCROLL
		}
		else
		{ --_skip_CHRs_cnt; }
#endif

		// block 2
		CHR_offset = ( mpd_farpeekb( mpd_Tiles, tile4x4_offset + 1 ) << 3 ) + CHR_y_pos;
#if	MAPS_CNT != 1
		CHR_offset += __blocks_offset;
#endif
#if	FLAG_MODE_MULTIDIR_SCROLL
		if( !_skip_CHRs_cnt )
		{
#endif
		__mpd_disp_list_push_data( mpd_farpeekw( mpd_Attrs, CHR_offset ) );
		if( !--_CHRs_cnt ) { break; }
		if( !--data_part1 )	{ __mpd_push_data2_row_hdr( _vaddr, data_part2 ); }
#if	FLAG_MODE_MULTIDIR_SCROLL
		}
		else
		{ --_skip_CHRs_cnt; }

		if( !_skip_CHRs_cnt )
		{
#endif
		__mpd_disp_list_push_data( mpd_farpeekw( mpd_Attrs, CHR_offset + 2 ) );
		if( !--_CHRs_cnt ) { break; }
		if( !--data_part1 )	{ __mpd_push_data2_row_hdr( _vaddr, data_part2 ); }
#if	FLAG_MODE_MULTIDIR_SCROLL
		}
		else
		{ --_skip_CHRs_cnt; }
#endif
	}
#else	//FLAG_TILES2X2
	while( TRUE )
	{
#if	FLAG_MODE_BIDIR_SCROLL
		CHR_offset = ( mpd_farpeekb( mpd_TilesScr, _tiles_offset + tile_n ) << 3 ) + CHR_y_pos;
#else
		CHR_offset = ( mpd_farpeekb( mpd_Maps, _tiles_offset + tile_n ) << 3 ) + CHR_y_pos;
#endif	//FLAG_MODE_BIDIR_SCROLL

#if	FLAG_DIR_COLUMNS
#if	FLAG_MODE_BIDIR_SCROLL
		tile_n += ScrTilesHeight;
#else
		tile_n += __map_tiles_height;
#endif	//FLAG_MODE_BIDIR_SCROLL
#else	//FLAG_DIR_ROWS
		++tile_n;
#endif	//FLAG_DIR_COLUMNS
#if	MAPS_CNT != 1
		CHR_offset += __blocks_offset;
#endif
#if	FLAG_MODE_MULTIDIR_SCROLL
		if( !_skip_CHRs_cnt )
		{
#endif
		__mpd_disp_list_push_data( mpd_farpeekw( mpd_Attrs, CHR_offset ) );
		if( !--_CHRs_cnt ) { break; }
		if( !--data_part1 )	{ __mpd_push_data2_row_hdr( _vaddr, data_part2 ); }
#if	FLAG_MODE_MULTIDIR_SCROLL
		}
		else
		{ --_skip_CHRs_cnt; }
#endif
		__mpd_disp_list_push_data( mpd_farpeekw( mpd_Attrs, CHR_offset + 2 ) );
		if( !--_CHRs_cnt ) { break; }
		if( !--data_part1 )	{ __mpd_push_data2_row_hdr( _vaddr, data_part2 ); }
	}
#endif	//FLAG_TILES4X4

	__mpd_disp_list_end();
}

void	__mpd_push_data2_row_hdr( u16 _vaddr, u8 _size )
{
	__mpd_disp_list_push_hdr( DL_FLAG_DATA_INC1, _size, ( _vaddr & ~__BAT_width_dec1 ) );
}

u16	__mpd_get_VRAM_addr( u16 _x, u16 _y )
{
	return ( ( ( _x >> 3 ) & __BAT_width_dec1 ) + ( ( ( _y >> 3 ) & __BAT_height_dec1 ) << __BAT_width_pow2 ) ) & __BAT_size_dec1;
}

void	__mpd_update_scroll( bool _vsync )
{
	if( _vsync )
	{
		vsync();
	}

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

u8	mpd_get_property( u16 _x, u16 _y )
{
//	FLAG_TILES2X2
//	FLAG_TILES4X4
//	FLAG_DIR_COLUMNS
//	FLAG_DIR_ROWS
//	FLAG_PROP_ID_PER_BLOCK
//	FLAG_PROP_ID_PER_CHR
//
//	FLAG_MODE_MULTIDIR_SCROLL
//	FLAG_MODE_BIDIR_SCROLL
//	FLAG_MODE_BIDIR_STAT_SCR
//	FLAG_MODE_STAT_SCR

	u16	tiles_offset;
	u8	tile_id;

	u8	block_pos_x;
	u8	block_pos_y;
	u8	CHR_pos_x;
	u8	CHR_pos_y;

#if	FLAG_MODE_BIDIR_SCROLL		/* !!! */

	mpd_SCREEN*	tmp_scr;
	u16		scr_offs;

	s16		dx;
	s16		dy;

	tmp_scr		= __curr_scr;
	scr_offs	= __scr_offset;

	if( !__horiz_dir_pos && __vert_dir_pos )
	{
		dy = _y - ( ScrPixelsHeight - __vert_dir_pos );

		if( dy >= 0 )
		{
			tmp_scr		= __mpd_get_adj_screen( ADJ_SCR_DOWN );
			scr_offs	= tmp_scr->scr_ind * __c_scr_tiles_size;
			_y		= dy;
		}
		else
		{
			_y += __vert_dir_pos;
		}
	}
	else
	if( !__vert_dir_pos && __horiz_dir_pos )
	{
		dx = _x - ( ScrPixelsWidth - __horiz_dir_pos );

		if( dx >= 0 )
		{
			tmp_scr		= __mpd_get_adj_screen( ADJ_SCR_RIGHT );
			scr_offs	= tmp_scr->scr_ind * __c_scr_tiles_size;
			_x		= dx;
		}
		else
		{
			_x += __horiz_dir_pos;
		}
	}
#endif

#if	FLAG_TILES4X4
#if	FLAG_PROP_ID_PER_BLOCK
	_x >>= 4;
	_y >>= 4;

	block_pos_x = _x & 0x01;
	block_pos_y = _y & 0x01;

	_x >>= 1;
	_y >>= 1;
#else	//FLAG_PROP_ID_PER_CHR
	_x >>= 3;
	_y >>= 3;

	CHR_pos_x = _x & 0x01;
	CHR_pos_y = _y & 0x01;

	_x >>= 1;
	_y >>= 1;

	block_pos_x = _x & 0x01;
	block_pos_y = _y & 0x01;

	_x >>= 1;
	_y >>= 1;
#endif	
#else
#if	FLAG_PROP_ID_PER_BLOCK
	_x >>= 4;
	_y >>= 4;
#else	//FLAG_PROP_ID_PER_CHR
	_x >>= 3;
	_y >>= 3;

	CHR_pos_x = _x & 0x01;
	CHR_pos_y = _y & 0x01;

	_x >>= 1;
	_y >>= 1;
#endif
#endif

#if	FLAG_MODE_MULTIDIR_SCROLL	/* !!! */

#if	FLAG_DIR_ROWS
	tiles_offset	= __maps_tbl_offset + ( _y << 1 );
	tiles_offset	= mpd_farpeekw( mpd_MapsTbl, tiles_offset ) + _x;
#else
	tiles_offset	= __maps_tbl_offset + ( _x << 1 );
	tiles_offset	= mpd_farpeekw( mpd_MapsTbl, tiles_offset ) + _y;
#endif
	tiles_offset	+= __maps_offset;

	tile_id		= mpd_farpeekb( mpd_Maps, tiles_offset );

#elif	FLAG_MODE_BIDIR_SCROLL + FLAG_MODE_BIDIR_STAT_SCR + FLAG_MODE_STAT_SCR	/* !!! */

#if	FLAG_DIR_ROWS
	tiles_offset	= __scr_tiles_width_tbl[ _y ] + _x;
#else
	tiles_offset	= __scr_tiles_height_tbl[ _x ] + _y;
#endif

#if	FLAG_MODE_BIDIR_SCROLL
	tile_id		= mpd_farpeekb( mpd_TilesScr, scr_offs + tiles_offset );
#else
	tile_id		= mpd_farpeekb( mpd_TilesScr, __scr_offset + tiles_offset );
#endif
#endif

#if	FLAG_TILES4X4
	tile_id		= mpd_farpeekb( mpd_Tiles, __tiles_offset + ( tile_id << 2 ) + ( block_pos_y << 1 ) + block_pos_x );
#endif

#if	FLAG_PROP_ID_PER_BLOCK
	return mpd_farpeekb( mpd_Props, __props_offset + tile_id );
#else	//FLAG_PROP_ID_PER_CHR
	return mpd_farpeekb( mpd_Props, __props_offset + ( tile_id << 2 ) + ( CHR_pos_y << 1 ) + CHR_pos_x );
#endif
}