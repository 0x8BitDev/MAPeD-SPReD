//######################################################################################################
//
// This file is a part of the MAPeD-SMD Copyright 2017-2021 0x8BitDev ( MIT license. See LICENSE.txt )
// Desc: This sample demonstrates working with exported data.
// (exported data checking, working with screen data and data arrays)
//
//######################################################################################################

#include "genesis.h"

#include "mpd_common.h"
#include "tilemap.h"
#include "mpd_screen.h"

//////////////
// macroses //
//////////////

#define DUMP_PU8_DATA( _data_arr, _dump_cnt, _x, _y )	dump_pu8_data( #_data_arr, _data_arr.count, _data_arr.data, _dump_cnt, _x, _y )
#define DUMP_U8_DATA( _data_arr, _dump_cnt, _x, _y )	dump_u8_data( #_data_arr, _data_arr.count, _data_arr.data, _dump_cnt, _x, _y )
#define DUMP_U16_DATA( _data_arr, _dump_cnt, _x, _y )	dump_u16_data( #_data_arr, _data_arr.count, _data_arr.data, _dump_cnt, _x, _y )
#define DUMP_U32_DATA( _data_arr, _dump_cnt, _x, _y )	dump_u32_data( #_data_arr, _data_arr.count, _data_arr.data, _dump_cnt, _x, _y )

///////////////////
// forward decls //
///////////////////

mpd_SCREEN*	dump_main_data( int* _y_offs );
void		refresh_data( mpd_SCREEN* _scr, int _y_pos );
void		joy_wait_btn_up( u16 _btn_id );

void 		dump_screen( mpd_SCREEN* _scr, int* _y_offs );
mpd_SCREEN*	get_adjacent_screen_data( mpd_SCREEN* _scr, int _dir_ind );
void		dump_exp_flags( int* _y_offs );

void		dump_pu8_data( const char* _name, int _count, u8** _data, int _elems_cnt, int _x, int _y );
void		dump_u8_data( const char* _name, int _count, u8* _data, int _elems_cnt, int _x, int _y );
void		dump_u16_data( const char* _name, int _count, u16* _data, int _elems_cnt, int _x, int _y );
void		dump_u32_data( const char* _name, int _count, u32* _data, int _elems_cnt, int _x, int _y );

//////////////////
// some defines //
//////////////////

#define DEF_STR_DATA_Y_OFFSET 2

/////////////////
// global vars //
/////////////////

#if CHECK_MAP_FLAG( MAP_FLAG_LAYOUT_ADJACENT_SCREENS ) || CHECK_MAP_FLAG( MAP_FLAG_LAYOUT_ADJACENT_SCR_INDS )
static char	adj_scr_dir[ 4 ]	= "LURD";
#endif

static char	str_buff[ 64 ]		= "";
static u16	joy_data;


int main( bool hardReset )
{
	int y_pos = DEF_STR_DATA_Y_OFFSET;

	mpd_SCREEN* scr_data = dump_main_data( &y_pos );

	dump_screen( scr_data, &y_pos );

#if !CHECK_MAP_FLAG( MAP_FLAG_LAYOUT_MATRIX )
	VDP_drawText( "<Use L/R/U/D buttons>", 1, 25 );
#endif

	while( TRUE )
	{
		joy_data = JOY_readJoypad( JOY_1 );

		if( joy_data == BUTTON_UP )
		{
			scr_data = get_adjacent_screen_data( scr_data, SCR_IND_UP );
			refresh_data( scr_data, DEF_STR_DATA_Y_OFFSET );

			joy_wait_btn_up( BUTTON_UP );
		}
		else
		if( joy_data == BUTTON_DOWN )
		{
			scr_data = get_adjacent_screen_data( scr_data, SCR_IND_DOWN );
			refresh_data( scr_data, DEF_STR_DATA_Y_OFFSET );

			joy_wait_btn_up( BUTTON_DOWN );
		}
		else
		if( joy_data == BUTTON_LEFT )
		{
			scr_data = get_adjacent_screen_data( scr_data, SCR_IND_LEFT );
			refresh_data( scr_data, DEF_STR_DATA_Y_OFFSET );

			joy_wait_btn_up( BUTTON_LEFT );
		}
		else
		if( joy_data == BUTTON_RIGHT )
		{
			scr_data = get_adjacent_screen_data( scr_data, SCR_IND_RIGHT );
			refresh_data( scr_data, DEF_STR_DATA_Y_OFFSET );

			joy_wait_btn_up( BUTTON_RIGHT );
		}

		// always call this method at the end of the frame
        	SYS_doVBlankProcess();
	}

	return 0;
}

void joy_wait_btn_up( u16 _btn_id )
{
	while( TRUE )
	{
		joy_data = JOY_readJoypad( JOY_1 );

		if( ( joy_data & _btn_id ) != _btn_id )
		{
			return;
		}

		SYS_doVBlankProcess();
	}
}

////////////////////////////////
// main data dumping function //
////////////////////////////////

mpd_SCREEN* dump_main_data( int* _y_offs )
{
	mpd_SCREEN* start_scr = NULL;

	VDP_drawText( "MAPeD-SMD data viewer:", 1, 0 );

	dump_exp_flags( _y_offs );
	(*_y_offs)++;

#if CHECK_MAP_FLAG( MAP_FLAG_MODE_MULTIDIR_SCROLL )

	VDP_drawText( "Mode: 'multidir scroll'", 1, (*_y_offs)++ );

	DUMP_U8_DATA( mpd_Lev0_CHR, 4, 0, (*_y_offs)++ );

#if CHECK_MAP_FLAG( MAP_FLAG_TILES4X4 )

	DUMP_U32_DATA( mpd_Lev0_Tiles, 4, 0, (*_y_offs)++ );

#endif

	DUMP_U16_DATA( mpd_Lev0_Attrs, 8, 0, (*_y_offs)++ );
	DUMP_U8_DATA( mpd_Lev0_Props, 8, 0, (*_y_offs)++ );
	DUMP_U16_DATA( mpd_Lev0_Map, 8, 0, (*_y_offs)++ );
	DUMP_U16_DATA( mpd_Lev0_MapTbl, 4, 0, (*_y_offs)++ );
	DUMP_U16_DATA( mpd_Lev0_Palette, 4, 0, (*_y_offs)++ );

#if CHECK_MAP_FLAG( MAP_FLAG_ENTITIES )

	start_scr = mpd_Lev0_Layout.data[ Lev0_StartScr ];

#endif

#elif CHECK_MAP_FLAG( MAP_FLAG_MODE_BIDIR_SCROLL )

	VDP_drawText( "Mode: 'bidir scroll'", 1, (*_y_offs)++ );

	DUMP_PU8_DATA( mpd_tilemap_CHRs, 4, 0, (*_y_offs)++ );
	DUMP_U16_DATA( mpd_tilemap_CHRs_size, 1, 0, (*_y_offs)++ );
	DUMP_U8_DATA( mpd_tilemap_Props, 4, 0, (*_y_offs)++ );
	DUMP_U16_DATA( mpd_tilemap_BlocksPropsOffs, 1, 0, (*_y_offs)++ );

#if CHECK_MAP_FLAG( MAP_FLAG_TILES4X4 )

	DUMP_U32_DATA( mpd_tilemap_Tiles, 4, 0, (*_y_offs)++ );

#endif

	DUMP_U16_DATA( mpd_tilemap_TilesOffs, 1, 0, (*_y_offs)++ );
	DUMP_U16_DATA( mpd_tilemap_Attrs, 4, 0, (*_y_offs)++ );
	DUMP_U16_DATA( mpd_tilemap_TilesScr, 4, 0, (*_y_offs)++ );
	DUMP_U16_DATA( mpd_tilemap_Plts, 4, 0, (*_y_offs)++ );

#if CHECK_MAP_FLAG( MAP_FLAG_LAYOUT_ADJACENT_SCREENS )

	start_scr = mpd_Lev0_StartScr.data[ 0 ];

#elif CHECK_MAP_FLAG( MAP_FLAG_LAYOUT_ADJACENT_SCR_INDS )

	start_scr = mpd_Lev0_StartScr.data[ 0 ];

#elif CHECK_MAP_FLAG( MAP_FLAG_LAYOUT_MATRIX )

	start_scr = mpd_Lev0_Layout.data[ Lev0_StartScr ];

#endif

#else
	VDP_drawText( "Mode: 'static screens'", 1, (*_y_offs)++ );

	DUMP_PU8_DATA( mpd_tilemap_CHRs, 4, 0, (*_y_offs)++ );
	DUMP_U16_DATA( mpd_tilemap_CHRs_size, 1, 0, (*_y_offs)++ );
	DUMP_U8_DATA( mpd_tilemap_Props, 4, 0, (*_y_offs)++ );
	DUMP_U16_DATA( mpd_tilemap_BlocksPropsOffs, 1, 0, (*_y_offs)++ );
	DUMP_U16_DATA( mpd_tilemap_Plts, 4, 0, (*_y_offs)++ );
	DUMP_U8_DATA( mpd_tilemap_VDPScr, 4, 0, (*_y_offs)++ );

#if CHECK_MAP_FLAG( MAP_FLAG_LAYOUT_ADJACENT_SCREENS )

	start_scr = mpd_Lev0_StartScr.data[ 0 ];

#elif CHECK_MAP_FLAG( MAP_FLAG_LAYOUT_ADJACENT_SCR_INDS )

	start_scr = mpd_Lev0_StartScr.data[ 0 ];

#elif CHECK_MAP_FLAG( MAP_FLAG_LAYOUT_MATRIX )

	start_scr = mpd_Lev0_Layout.data[ Lev0_StartScr ];

#endif

#endif
	return start_scr;
}

void refresh_data( mpd_SCREEN* _scr, int _y_pos )
{
	if( _scr != NULL )
	{	
		VDP_clearPlane( VDP_getTextPlane(), TRUE );

		dump_main_data( &_y_pos );
		dump_screen( _scr, &_y_pos );

#if !CHECK_MAP_FLAG( MAP_FLAG_LAYOUT_MATRIX )
		VDP_drawText( "<Use L/R/U/D buttons>", 1, 25 );
#endif
	}
}

///////////////////////////
// screen data functions //
///////////////////////////

void dump_screen( mpd_SCREEN* _scr, int* _y_offs )
{
	if( _scr != NULL )
	{
		int i;
		char tmp_str[ 64 ] = "";

		(*_y_offs)++;
		VDP_drawText( "Screen data:", 1, (*_y_offs)++ );

		strclr( str_buff );

#if CHECK_MAP_FLAG( MAP_FLAG_MODE_BIDIR_SCROLL ) || CHECK_MAP_FLAG( MAP_FLAG_MODE_STATIC_SCREENS )
//		CHR data index
//		u16		chr_ind;

		sprintf( tmp_str, "CHR: %d/", _scr->chr_ind );
		strcat( str_buff, tmp_str );
#endif

#if CHECK_MAP_FLAG( MAP_FLAG_MARKS )
//		(marks) bits: 7-4 - bit mask of user defined adjacent screens ( Down(7)-Right(6)-Up(5)-Left(4) ); 3-0 - screen property
//		u16		marks;

		sprintf( tmp_str, "Marks: %X/", _scr->marks );
		strcat( str_buff, tmp_str );
#endif

#if CHECK_MAP_FLAG( MAP_FLAG_MODE_STATIC_SCREENS )
//		u16		VDP_data_offset;

		sprintf( tmp_str, "VDPoffs: %d/", _scr->VDP_data_offset );
		strcat( str_buff, tmp_str );
#endif

#if CHECK_MAP_FLAG( MAP_FLAG_MODE_BIDIR_SCROLL ) || CHECK_MAP_FLAG( MAP_FLAG_MODE_STATIC_SCREENS )
//		screen index
//		u16		scr_ind;

		sprintf( tmp_str, "Scr: %d", _scr->scr_ind );
		strcat( str_buff, tmp_str );
#endif

		if( strlen( str_buff ) > 0 )
		{
			VDP_drawText( str_buff, 1, (*_y_offs)++ );
		}

#if CHECK_MAP_FLAG( MAP_FLAG_LAYOUT_ADJACENT_SCREENS )
//		union
//		{
//			struct mpd_SCREEN*	left;
//			struct mpd_SCREEN*	up;
//			struct mpd_SCREEN*	right;
//			struct mpd_SCREEN*	down;
//
//			struct mpd_SCREEN*	arr[4];
//		} adj_scr;

		sprintf( str_buff, "Adj scr:" );

		for( i = 0; i < SCR_IND_MAX; i++ )
		{
			sprintf( tmp_str, " %c:%X", adj_scr_dir[ i ], ( unsigned int )_scr->adj_scr.arr[ i ] );
			strcat( str_buff, tmp_str );
		}

		VDP_drawText( str_buff, 1, (*_y_offs)++ );		
#endif

#if CHECK_MAP_FLAG( MAP_FLAG_LAYOUT_ADJACENT_SCR_INDS )
//		union
//		{
//			u8	left;
//			u8	up;
//			u8	right;
//			u8	down;
//
//			u8	arr[4];
//		} adj_scr_inds;

		sprintf( str_buff, "Adj scr inds:" );

		for( i = 0; i < SCR_IND_MAX; i++ )
		{
			sprintf( tmp_str, " %c:%X", adj_scr_dir[ i ], _scr->adj_scr_inds.arr[ i ] );
			strcat( str_buff, tmp_str );
		}

		VDP_drawText( str_buff, 1, (*_y_offs)++ );
#endif

#if CHECK_MAP_FLAG( MAP_FLAG_ENTITIES )
//		entities count
//		u16			ent_count;
//		entities array
//		struct mpd_ENTITY_INSTANCE*	ents;

		sprintf( str_buff, "Entities: %d/", _scr->ent_count );

		for( i = 0; i < _scr->ent_count; i++ )
		{
			sprintf( tmp_str, "%X ", _scr->ents[ i ].id );
			strcat( str_buff, tmp_str );
		}

		VDP_drawText( str_buff, 1, (*_y_offs)++ );
#endif
	}
	else
	{
		VDP_drawText( "No screen data!", 1, (*_y_offs)++ );
	}
}

mpd_SCREEN* get_adjacent_screen_data( mpd_SCREEN* _scr, int _dir_ind )
{
#if CHECK_MAP_FLAG( MAP_FLAG_LAYOUT_ADJACENT_SCREENS )
//	union
//	{
//		struct mpd_SCREEN*	left;
//		struct mpd_SCREEN*	up;
//		struct mpd_SCREEN*	right;
//		struct mpd_SCREEN*	down;
//
//		struct mpd_SCREEN*	arr[4];
//	} adj_scr;

	mpd_SCREEN* new_scr = _scr->adj_scr.arr[ _dir_ind ];

	return ( new_scr != NULL ) ? new_scr:_scr;
#endif

#if CHECK_MAP_FLAG( MAP_FLAG_LAYOUT_ADJACENT_SCR_INDS )
//	union
//	{
//		u8	left;
//		u8	up;
//		u8	right;
//		u8	down;
//
//		u8	arr[4];
//	} adj_scr_inds;

	u8 scr_ind = _scr->adj_scr_inds.arr[ _dir_ind ];

	return ( scr_ind != 0xff ) ? mpd_Lev0_ScrArr.data[ scr_ind ]:_scr;
#else
	return NULL;
#endif
}

///////////////////////////////////////
// export options detection function //
///////////////////////////////////////

void dump_exp_flags( int* _y_offs )
{
	VDP_drawText( "Export options:", 1, (*_y_offs)++ );

	strclr( str_buff );
	if( CHECK_MAP_FLAG( MAP_FLAG_TILES2X2 ) )
	{
		strcat( str_buff, "- Blocks 2x2" );
	}
	else
	{
		strcat( str_buff, "- Tiles 4x4" );
	}

	if( CHECK_MAP_FLAG( MAP_FLAG_DIR_COLUMNS ) )
	{
		strcat( str_buff, "/columns" );
	}
	else
	{
		strcat( str_buff, "/rows" );
	}

	if( CHECK_MAP_FLAG( MAP_FLAG_RLE ) )
	{
		strcat( str_buff, "/RLE" );
	}

	VDP_drawText( str_buff, 1, (*_y_offs)++ );

	strclr( str_buff );
	if( CHECK_MAP_FLAG( MAP_FLAG_PROP_ID_PER_BLOCK ) )
	{
		strcat( str_buff, "- Props per block" );
	}
	else
	{
		strcat( str_buff, "- Props per CHR" );
	}

	VDP_drawText( str_buff, 1, (*_y_offs)++ );

	strclr( str_buff );
	if( CHECK_MAP_FLAG( MAP_FLAG_LAYOUT_ADJACENT_SCREENS ) )
	{
		strcat( str_buff, "- Layout: adjacent screens" );
	}
	else
	if( CHECK_MAP_FLAG( MAP_FLAG_LAYOUT_ADJACENT_SCR_INDS ) )
	{
		strcat( str_buff, "- Layout: adjacent screen indices" );
	}
	else
	{
		strcat( str_buff, "- Layout: matrix" );
	}

	strclr( str_buff );
	if( CHECK_MAP_FLAG( MAP_FLAG_LAYOUT_ADJACENT_SCREENS ) )
	{
		strcat( str_buff, "- Layout: adj screens" );
	}
	else
	if( CHECK_MAP_FLAG( MAP_FLAG_LAYOUT_ADJACENT_SCR_INDS ) )
	{
		strcat( str_buff, "- Layout: adj screen inds" );
	}
	else
	{
		strcat( str_buff, "- Layout: matrix" );
	}

	if( CHECK_MAP_FLAG( MAP_FLAG_MARKS ) )
	{
		strcat( str_buff, "/MARKS" );
	}
	else
	{
		strcat( str_buff, "/NO MARKS" );
	}

	VDP_drawText( str_buff, 1, (*_y_offs)++ );

	strclr( str_buff );
	if( CHECK_MAP_FLAG( MAP_FLAG_ENTITIES ) )
	{
		strcat( str_buff, "- Entities" );

		if( CHECK_MAP_FLAG( MAP_FLAG_ENTITY_SCREEN_COORDS ) )
		{
			strcat( str_buff, "/screen coords" );
		}
		else
		{
			strcat( str_buff, "/map coords" );
		}
	}
	else
	{
		strcat( str_buff, "- No entities" );
	}

	VDP_drawText( str_buff, 1, (*_y_offs)++ );

	sprintf( str_buff, "- CHRs offset: %d", CHRS_OFFSET );

	VDP_drawText( str_buff, 1, (*_y_offs)++ );
}

///////////////////////////////////
// data arrays dumping functions //
///////////////////////////////////

void dump_pu8_data( const char* _name, int _count, u8** _data, int _dump_cnt, int _x, int _y )
{
	sprintf( str_buff, "-%s: %d/[0] ", _name, _count );

	VDP_drawText( str_buff, _x, _y );

	int x_offs = strlen( str_buff );

	for( int i = 0; i < _dump_cnt; i++ )
	{
		sprintf( str_buff, "%X ", _data[ 0 ][ i ] );
		VDP_drawText( str_buff, _x + x_offs, _y );

		x_offs += strlen( str_buff );
	}
}

void dump_u8_data( const char* _name, int _count, u8* _data, int _dump_cnt, int _x, int _y )
{
	sprintf( str_buff, "-%s: %d/", _name, _count );

	VDP_drawText( str_buff, _x, _y );

	int size = _count < _dump_cnt ? _count:_dump_cnt;
	int x_offs = strlen( str_buff );

	for( int i = 0; i < size; i++ )
	{
		sprintf( str_buff, "%X ", _data[ i ] );
		VDP_drawText( str_buff, _x + x_offs, _y );

		x_offs += strlen( str_buff );
	}
}

void dump_u16_data( const char* _name, int _count, u16* _data, int _dump_cnt, int _x, int _y )
{
	sprintf( str_buff, "-%s: %d/", _name, _count );

	VDP_drawText( str_buff, _x, _y );

	int size = _count < _dump_cnt ? _count:_dump_cnt;
	int x_offs = strlen( str_buff );

	for( int i = 0; i < size; i++ )
	{
		sprintf( str_buff, "%X ", _data[ i ] );
		VDP_drawText( str_buff, _x + x_offs, _y );

		x_offs += strlen( str_buff );
	}
}

void dump_u32_data( const char* _name, int _count, u32* _data, int _dump_cnt, int _x, int _y )
{
	sprintf( str_buff, "-%s: %d/", _name, _count );

	VDP_drawText( str_buff, _x, _y );

	int size = _count < _dump_cnt ? _count:_dump_cnt;
	int x_offs = strlen( str_buff );

	for( int i = 0; i < size; i++ )
	{
		sprintf( str_buff, "%X ", ( unsigned int )_data[ i ] );
		VDP_drawText( str_buff, _x + x_offs, _y );

		x_offs += strlen( str_buff );
	}
}