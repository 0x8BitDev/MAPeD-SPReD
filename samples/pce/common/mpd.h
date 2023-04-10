//######################################################################################################
//
// This file is a part of the MAPeD-PCE Copyright 2017-2023 0x8BitDev ( MIT license. See LICENSE.txt )
// Desc: Tilemaps rendering library written in HuC+ASM.
//
// *MAKE GAMES, NOT WAR!)*
//######################################################################################################

// external HuC/MagicKit data/procs used by MPD library
/*
data:
~~~~~
vdc_crl
vdc_crh
video_data_l
video_data_h
__ax...__dx

procs:
~~~~~~
map_data
unmap_data
load_palette
load_vram
load_bat
mulu16
*/

/*/	MPD-render v0.9
History:

2023.04.09 - an array of map data offsets is replaced by an array of 24-bit pointers to map data to prevent 64K overflow in large projects

v0.9
2023.04.06 - added 'Dynamic tilemaps' section
2023.04.05 - opened new public read-only variables: mpd_map_tiles_width and mpd_map_tiles_height
2023.04.04 - added 'u16 mpd_tiles_cnt' variable - tiles count of a current data bank (2x2 or 4x4 it depends on export options)
2023.04.03 - added 'General information' section
2023.04.01 - ASM MPD_DEBUG changed to HuC '#define MPD_DEBUG'
2023.03.31 - added mpd_get_tile_property(...) and mpd_set_tile_property(...) functions and 'u16 mpd_tile_props_arr_size' read-only variable
2023.03.30 - added mpd_set_tile(...) and mpd_get_tile(...) functions
2023.03.28 - optimized access to a tile properties array: #define MPD_RAM_TILE_PROPS
2023.03.26 - optimized call of '__mpd_fill_row_data' and '__mpd_fill_column_data'
2023.03.26 - optimized access to tilemap data: #define MPD_RAM_MAP and #define MPD_RAM_MAP_TBL (support for dynamic multidir maps)

v0.8
2023.03.10 - added external dependencies info (HuC/MagicKit)
2023.03.05 - performance-critical functions: map scrolling and getting a map tile property were wrapped with .procgroup/.endprocgroup
2023.03.05 - reduced the number of HuC functions (.proc/.endp) [25]:
		[inlined]	mpd_get_CR_val()
		[inlined]	__mpd_get_entity_by_offs(...)
		[inlined]	__mpd_get_entity_by_addr(...)
		[inlined]	__mpd_get_BAT_params()
		[#define]	mpd_get_curr_screen_ind()
		[#define]	mpd_clear_update_flags()
		[#define]	mpd_copy_screen(...)
		[#define]	mpd_get_map_size(...)
		[#define]	mpd_get_start_screen(...)
		[#define]	mpd_init_base_ent_arr()
		[#define]	mpd_get_base_ent_cnt()
		[#define]	__mpd_draw_block2x2(...)
		[#define]	mpd_draw_screen_by_ind(...)
		[#define]	mpd_draw_screen_by_data(...)
		[#define]	mpd_draw_screen()
		[#define]	__mpd_calc_scr_pos_by_scr_ind(...)
		[macro]		mpd_load_vram2(...)
		[macro]		mpd_load_vram(...)
		[macro]		mpd_load_palette(...)
		[macro]		mpd_load_bat(...)
		[macro]		mpd_farpeekw.3(...)
		[unused]	mpd_farpeekb.3(...)
		[macro]		mpd_memcpy_fast(...)
		[macro]		mpd_farpeekb.2(...)
		[macro]		__mpd_calc_skip_CHRs_cnt(...)
2023.03.01 - optimized use of local variables and function arguments
2023.02.26 - fixed 'mpd_load_palette' to work with all palette slots
2023.02.25 - the mpd_ax-fx variables moved to zero-page
2022.11.25 - fixed bug when 'mpd_init_screen_arr' is called before 'mpd_init(...)'
2022.10.06 - asm optimization - '__mpd_calc_skip_CHRs_cnt(...)' and '__mpd_get_VRAM_addr(...)'
2022.10.04 - asm optimization - 'mpd_get_property(...)'

v0.7
2022.09.27 - updated 'Working with screens/entities - General information' / item 2
2022.09.27 - removed deprecated functions
2022.09.07 - added 'mpd_get_curr_screen_ind()' for multidirectional maps
2022.09.06 - added open variables to replace deprecated functions
2022.08.31 - fixed missed tile 4x4 drawing in '__mpd_fill_row_column_data' and also fixed BAT overflow in '__mpd_draw_tiled_screen( _BAT_offset )'
2022.08.30 - added 'mpd_active_map_width()' and 'mpd_active_map_height()' for multidirectional maps
2022.08.23 - optimized screen scrolling, re-written in assembler; 3x faster than before
2022.08.21 - removed display list + pre-optimization of screen scrolling
2022.08.07 - added 'mpd_get_scroll_step_x'/'mpd_get_scroll_step_y'
2022.08.07 - renamed 'mpd_scroll_step_x'/'mpd_scroll_step_y' to 'mpd_set_scroll_step_x'/'mpd_set_scroll_step_y'

v0.6
2022.07.01 - added MPD_DEBUG flag that shows how long screen drawing/scrolling takes by border colors (use Mednafen)
2022.06.26 - asm routines are wrapped in .proc/.endp
2022.06.23 - added 'Working with screens/entities' info and 'mpd_get_screen_data' for bi-dir maps and 'mpd_copy_screen'
2022.06.21 - added functions for working with base entities
2022.06.19 - screen/entity data moved from .h file to .asm, and added interface functions
2022.06.02 - fixed _mpd_farptr_add_offset
2022.05.27 - fixed 'mpd_draw_screen_by_pos' and removed 'mpd_draw_screen_by_pos_offs' as unsafe and useless
2022.05.26 - 'mpd_draw_screen_by_ind' and 'mpd_draw_screen_by_ind_offs' - changed screen index from u16 to u8
2022.05.25 - fixed scroll values overflow in the 'mpd_move_right/left/up/down' for multi-dir mode
2022.05.20 - 'mpd_draw_CHR' removed _CHR_ind clamping  (_CHR_ind & 0x03)

v0.5
2022.05.19 - added 'mpd_draw_CHR( u16 _x, u16 _y, u8 _block2x2_ind, u8 _CHR_ind )', 'mpd_draw_block2x2( u8 _x, u8 _y, u8 _block2x2_ind )' and 'mpd_draw_tile4x4( u8 _x, u8 _y, u8 _tile4x4_ind )'

v0.4
2022.05.12 - 'mpd_update_screen( bool _vsync )' changed to 'mpd_update_screen()'
2022.05.12 - removed VDC's scroll registers update on 'mpd_init' and 'mpd_update_screen'
2022.05.10 - added 'mpd_draw_screen_by_data_offs( mpd_SCREEN* _scr_data, u16 _BAT_offset )'
2022.05.10 - 'mpd_draw_screen_by_scr_data' renamed to 'mpd_draw_screen_by_data'
2022.05.10 - added 'mpd_draw_screen_by_ind_offs( u16 _scr_ind, u16 _BAT_offset, bool _reset_scroll )' and 'mpd_draw_screen_by_pos_offs( u16 _x, u16 _y, u16 _BAT_offset, bool _reset_scroll )'
2022.05.10 - 'mpd_draw_screen_by_scr_ind' renamed to 'mpd_draw_screen_by_ind'
2022.05.10 - added support for free step scrolling (1-7 pix) for both bi- and multi-dir modes
2022.05.09 - added 'mpd_scroll_step_x( u8 _step_pix )/mpd_scroll_step_y( u8 _step_pix )' for scrollable maps
2022.05.08 - removed 'mpd_scroll_step' enum, now 'mpd_init( u8 _map_ind, u8 _step )' as scroll step value receives numbers 1-7 pix
2022.05.08 - mpd_scroll_x()/mpd_scroll_y() return signed word

v0.3
2022.05.07 - added 'mpd_get_adj_screen( mpd_SCREEN* _scr_data, u8 _ind )' for bidir maps

v0.2
2022.05.05 - added 'mpd_draw_screen_by_scr_data( mpd_SCREEN* _scr_data )' for bidir maps
2022.05.05 - added 'mpd_draw_screen_by_pos( u16 _x, u16 _y )' for multidir maps
2022.05.04 - added 'mpd_draw_screen_by_scr_ind( u16 _scr_ind )' for multidir maps

v0.1
2022.02.16 - initial release
*/

/*/
debug info (use Mednafen):
 - green border color	- screen scrolling
 - blue border color	- static screen drawing
 - yellow border color	- getting a tile property

#define MPD_DEBUG

General information
~~~~~~~~~~~~~~~~~~~

MPD library is designed to work with data exported from MAPeD-PCE.

The main functionality of the library includes rendering and scrolling tile maps, rendering individual tiles and map screens,
getting a tile property and working with entities. Dynamic tile maps are also supported.

MPD library allows to draw screens at any place in BAT and can be used for various purposes:

    a) For double-buffered screen switching without the black screen flashing ( disp_off()/disp_on() ) within one map.
    There is a sample that shows double-buffered screen switching for multi-directional maps: './samples/pce/tilemap_render/multidir_stat_scr_multimap'

    b) For custom scrolling between screens without MPD library.
    The same technique as (a), but you can make your own scrolling between screens (see info below).

    c) If your game map fits into any BAT size, you can fill the BAT with a map screens and make your own map scrolling (see info below).
    It's faster than using MPD library for scrolling!

    So, the rule is as follows: if each of your game maps fits into BAT, DON'T USE the MPD library for scrolling! It doesn't make any sense!
    Use it for filling BAT with screens and getting a tile property!


Map scrolling types:
~~~~~~~~~~~~~~~~~~~~

NOTE:	Since v0.4 the library doesn`t interact with VDC`s scroll registers in any way!	It just provides scroll values X/Y: mpd_scroll_x, mpd_scroll_y.
	Thus, user must set scroll values in his program using these global variables. This is for scrollable maps only!

	There are two ways:

	1. Fullscreen scrolling. Write directly to the bgx1(0x220c)/bgy1(0x2210) system variables:

		for(;;)
		{
			...your code here...

			// update VDC's scroll registers on VBLANK via bgx1/bgy1 variables
			pokew( 0x220c, mpd_scroll_x );
			pokew( 0x2210, mpd_scroll_y );
			vsync();
		}

	2. Area scrolling. If you need to combine a static HUD and the scrollable area for your map, you can do this using the HuC's scroll library - 'scroll(...)':

		NOTE: This will work for linear horizontal bi-directional maps!

		// init HUD out of the visible BAT area
		scroll( 0, 0, 224, 0, 31, 0x80 );

		for(;;)
		{
			...your code here...

			// map scrolling
			scroll( 1, mpd_scroll_x, 32, 32, 223, 0xC0 );
			vsync();
		}

	Thus, the main option is (1). But if you need to limit the scrollable area you need to use the (2) option.

	NOTE: To avoid conflicts, these options can not be used together. You must use either (1) or (2).


Dynamic tilemaps:
~~~~~~~~~~~~~~~~~~
All information below applies to multi-directional maps only!

By default, all maps data are stored in ROM. Accessing them requires constant switching of the memory banks.
This can be avoided by placing the map data in RAM. This greatly speeds up data access and gives new features such as:
dynamically changing map and tile properties.

Dynamic map changing can be used to change map topology, procedural generation of levels, also it's fastest way for storing collectable items etc...

There are three types of data that can be located in RAM:

- tilemap data
- tilemap LUT
- tile properties

This data can be initialized into RAM independently of each other. It all depends on the amount of free RAM available.

You can do this by placing the following declarations before the MPD library is included in your program:

#define MPD_RAM_MAP
#define MPD_RAM_MAP_TBL
#define MPD_RAM_TILE_PROPS

All these defines speed up getting a tile property and slightly speed up static screens drawing and scrolling.

MPD_RAM_MAP - enables the following functions:

[macro] u8	mpd_get_tile( u16 _x, u16 _y, bool _pixels ) / _pixels = TRUE - pixel coordinates, FALSE - tile coordinates
[macro] void	mpd_set_tile( u16 _x, u16 _y, bool _pixels, u8 _tile_ind ) / _pixels = TRUE - pixel coordinates, FALSE - tile coordinates, _tile_ind - 0..255

MPD_RAM_TILE_PROPS - enables the following functions:

#if	FLAG_PROP_ID_PER_CHR

[macro] u8	mpd_get_tile_property( u8 _tile_ind, u8 _CHR_ind ) / _tile_ind - 0..255, _CHR_ind - 0..3
[macro] void	mpd_set_tile_property( u8 _tile_ind, u8 _CHR_ind, u8 _new_prop ) / _tile_ind - 0..255, _CHR_ind - 0..3, _new_prop - a new property value

#else	//FLAG_PROP_ID_PER_BLOCK

[macro] u8	mpd_get_tile_property( u8 _tile_ind ) / _tile_ind - 0..255
[macro] void	mpd_set_tile_property( u8 _tile_ind, u8 _new_prop ) / _tile_ind - 0..255, _new_prop - a new property value

#endif	//FLAG_PROP_ID_PER_CHR

By default, implementations of these functions are unsafe. You can write data outside the arrays by mistake.
Use MPD_DEBUG when developing, to enable safe function implementations. If you write data outside of the allocated memory, you will get an error message, function name and its arguments.

The amount of memory that will be allocated for the data can be seen in <my_exported_tilemap>.h

#define	MAX_MAP_SIZE		...
#define	MAX_MAP_TBL_SIZE	...
#define	MAX_TILE_PROPS_SIZE	...

NOTE: If several maps are exported, memory will be allocated for the largest map(!)

Transferring this data to RAM will have a positive effect on performance, even if you don't intend to use the dynamic maps and tile properties features.
It all depends on the amount of free RAM in your project and the size of your maps. The size of a map can far exceed the amount of available RAM.
So plan and allocate memory in your project carefully to effectively use the MPD library.

There are two example projects that demonstrate dynamic maps functionality:

Simple map editor:
./samples/pce/tilemap_render/multidir_scroll_map_editor/

Procedural generation of a random maze (3x3 screens):
./samples/pce/tilemap_render/multidir_scroll_maze_generator/


Working with screens/entities:
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

General information:

1. Regardless of a map type, all entities data is stored in screens.
2. The maximum number of allowed entities per screen is 255. But the maximum number of allowed entities per map is 256.
3. There are base entities and instances. The base entities you customize in MAPeD in the 'Entities' tab. The instances are base entities that have been placed on a map.
4. Entities can be sorted during the data export process in two ways: left to right or bottom to top. The sorting goes by pivot points.
5. So, for a multi-directional map, whichever screen you take, you will always have sorted entity data.
6. Accessing screen/entity data requires banks switching and data copying. So it is recommended to cache the data in optimal way for your project when initializing a map data.

Data structures:

typedef struct
{
	u8	id;
	u8	width;
	u8	height;
	u8	pivot_x;
	u8	pivot_y;
	u8	props_cnt;

} mpd_ENTITY_BASE;

typedef struct
{
	u8	id;
	u16	base_ent_addr;	// for library use only
	u16	targ_ent_addr;	// for library use only
	u16	x_pos;
	u16	y_pos;
	u8	props_cnt;

} mpd_ENTITY_INSTANCE;

typedef struct
{
	mpd_ENTITY_INSTANCE	inst;
	u8			inst_props[ ENT_MAX_PROPS_CNT ];

	mpd_ENTITY_BASE		base;
	u8			base_props[ ENT_MAX_PROPS_CNT ];

} mpd_ENTITY;

typedef struct
{
	mpd_PTR24		scr_arr_ptr;	// for library use only
	u16			scr_offset;	// for library use only

	mpd_SCREEN		scr;		// depends on exported options, see exported .h file for details

#if	FLAG_ENTITIES
	mpd_ENTITY		inst_ent;
	mpd_ENTITY		targ_ent;
#endif	//FLAG_ENTITIES

} mpd_SCR_DATA;


The functions for accessing base entities:

void	mpd_init_base_ent_arr() - must be called once for all maps(!)
u16	mpd_get_base_ent_cnt()
void	mpd_get_base_entity( mpd_SCR_DATA* _scr_data, u16 _ent_ind ) - RES: _scr_data->inst_ent.base

Examples of using the library for multi-dir maps:
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

void	mpd_init_screen_arr( mpd_SCR_DATA* _scr_data, u8 _map_ind ) - must be called once for each map(!)

Getting a map size in screens:

	u16	map_size;
	u8	scr_width;
	u8	scr_height;

	map_size	= mpd_get_map_size( map_ind );

	scr_width	= map_size & 0x00ff;
	scr_height	= ( map_size & 0xff00 ) >> 8;

	or for a current map use these global variables:

	mpd_map_scr_width
	mpd_map_scr_height

Accessing screen/entity data:

	mpd_SCR_DATA	scr_data;

	u8	scr_cnt;
	u8	scr_n;
	u8	ent_n;

	scr_cnt = mpd_map_scr_width * mpd_map_scr_height;

	for( scr_n = 0; scr_n < scr_cnt; scr_n++ )
	{
		mpd_get_screen_data( &scr_data, scr_n );

		for( ent_n = 0; ent_n < scr_data.scr.ents_cnt; ent_n++ )
		{
			mpd_get_entity( &scr_data, ent_n );

			AND/OR

			if( mpd_find_entity_by_base_id( &scr_data, base_ent_id ) )
			{
				...
			}

			AND/OR

			if( mpd_find_entity_by_inst_id( &scr_data, inst_ent_id ) )
			{
				...
			}

			...
		}
	}


Examples of using the library for bi-dir maps:
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Use 'mpd_curr_scr' to access the current screen, but
make a copy 'mpd_copy_screen( mpd_SCR_DATA* _src_scr, mpd_SCR_DATA* _dst_scr )'
before using 'mpd_get_screen_data( mpd_SCR_DATA* _scr_data, u16 _scr_ind )'

Iterating a map screens:

	u16		adj_scr;
	mpd_SCR_DATA	scr_data;

	mpd_get_start_screen( &scr_data );

	adj_scr = mpd_get_adj_screen( &scr_data, ADJ_SCR_LEFT/ADJ_SCR_RIGHT/ADJ_SCR_UP/ADJ_SCR_DOWN );

	if( adj_scr != 0xffff )
	{
		mpd_get_screen_data( &scr_data, adj_scr );
		...
	...


Accessing entities:

	u8	ent_n;

	mpd_curr_scr <- read only!

	OR

	mpd_SCR_DATA	scr_data;
	mpd_get_start_screen( &scr_data );

	for( ent_n = 0; ent_n < scr_data.scr.ents_cnt; ent_n++ )
	{
		mpd_get_entity( &scr_data, ent_n );

		AND/OR

		if( mpd_find_entity_by_base_id( &scr_data, base_ent_id ) )
		{
			...
		}

		AND/OR

		if( mpd_find_entity_by_inst_id( &scr_data, inst_ent_id ) )
		{
			...
		}

		...
	}

The rest functions see below.

Public functions and variables:
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Open variables and their deprecated functions:
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

#if	FLAG_MODE_BIDIR_SCROLL

R: mpd_SCR_DATA	mpd_curr_scr -> mpd_curr_screen() - DEPRECATED

#endif	//FLAG_MODE_BIDIR_SCROLL

#if	FLAG_MODE_MULTIDIR_SCROLL

Map width/height in screens:

R: u8	mpd_map_scr_width
R: u8	mpd_map_scr_height

Map width/height in tiles:

R: u16	mpd_map_tiles_width
R: u16	mpd_map_tiles_height

Active map width/height in pixels = map width/height in pixels - screen width/height in pixels

R: u16	mpd_map_active_width  -> mpd_active_map_width() - DEPRECATED
R: u16	mpd_map_active_height -> mpd_active_map_height() - DEPRECATED

Size of the tile properties array of a current map when declaring the MPD_RAM_TILE_PROPS

R: u16	mpd_tile_props_arr_size

#endif	//FLAG_MODE_MULTIDIR_SCROLL

#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL

R: u16	mpd_tiles_cnt - tiles count of a current data bank (2x2 or 4x4 it depends on export options)

Map scrolling step in the range 0..7

RW: u8	mpd_scroll_step_x [0..7] -> mpd_get_scroll_step_x()/mpd_set_scroll_step_x(...) - DEPRECATED
RW: u8	mpd_scroll_step_y [0..7] -> mpd_get_scroll_step_y()/mpd_set_scroll_step_y(...) - DEPRECATED

Map scrolling values:

R: s16	mpd_scroll_x -> mpd_scroll_x() - DEPRECATED
R: s16	mpd_scroll_y -> mpd_scroll_y() - DEPRECATED

#endif	//FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL

*R-read only; RW-read/write

Public functions:
~~~~~~~~~~~~~~~~~

#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
void	mpd_init( u8 _map_ind, u8 _step ) / _step - default step for both axes
DEPRECATED -->	void	mpd_set_scroll_step_x( u8 _step_pix ) / _step_pix - 1-7 pix
DEPRECATED -->	void	mpd_set_scroll_step_y( u8 _step_pix ) / _step_pix - 1-7 pix
DEPRECATED -->	u8	mpd_get_scroll_step_x()
DEPRECATED -->	u8	mpd_get_scroll_step_y()
void	mpd_clear_update_flags()
void	mpd_move_left()
void	mpd_move_right()
void	mpd_move_up()
void	mpd_move_down()
void	mpd_update_screen()
DEPRECATED -->	s16	mpd_scroll_x()
DEPRECATED -->	s16	mpd_scroll_y()
#else
void	mpd_init( u8 _map_ind )
#endif	//FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL

// NOTE: 'mpd_draw_...' - for scrollable maps add 'mpd_scroll_x' to _x and 'mpd_scroll_y' to _y
void	mpd_draw_CHR( u16 _x, u16 _y, u8 _block2x2_ind, u8 _CHR_ind ) / _x/_y - coordinates in pixels* , _block2x2_ind - 0..255, _CHR_ind - 0..3
void	mpd_draw_block2x2( u8 _x, u8 _y, u8 _block2x2_ind ) / _x/_y - coordinates in pixels* , _block2x2_ind - 0..255

#if	FLAG_TILES4X4
void	mpd_draw_tile4x4( u8 _x, u8 _y, u8 _tile4x4_ind ) / _x/_y - coordinates in pixels* , _tile4x4_ind - 0..255
#endif	//FLAG_TILES4X4

u8	mpd_get_property( u16 _x, u16 _y ) / _x/_y - coordinates in pixels* ; RES: property id
void	mpd_draw_screen()

* screen space coordinates for bi-directional maps and map space coordinates for multi-directional maps

#if	FLAG_MODE_MULTIDIR_SCROLL
DEPRECATED -->	u16	mpd_active_map_width() / map screens width in pixels - screen width in pixels
DEPRECATED -->	u16	mpd_active_map_height() / map screens height in pixels - screen height in pixels
void	mpd_draw_screen_by_ind( u8 _scr_ind )
void	mpd_draw_screen_by_ind_offs( u8 _scr_ind, u16 _BAT_offset, bool _reset_scroll )
void	mpd_draw_screen_by_pos( u16 _x, u16 _y )
u16	mpd_get_map_size( u8 _map_ind ) / RES: low byte - screens in width, high byte - screens in height
void	mpd_get_screen_data( mpd_SCR_DATA* _scr_data, u8 _scr_ind )
u8	mpd_get_start_screen_ind( u8 _map_ind )
void	mpd_get_start_screen( mpd_SCR_DATA* _scr_data, u8 _map_ind )
u8	mpd_get_curr_screen_ind()

#ifdef	MPD_RAM_MAP
u8	mpd_get_tile( u16 _x, u16 _y, bool _pixels ) / _pixels = TRUE - pixel coordinates, FALSE - tile coordinates
void	mpd_set_tile( u16 _x, u16 _y, bool _pixels, u8 _tile_ind ) / _pixels = TRUE - pixel coordinates, FALSE - tile coordinates, _tile_ind - 0..255
#endif	//MPD_RAM_MAP
#endif	//FLAG_MODE_MULTIDIR_SCROLL

#ifdef	MPD_RAM_TILE_PROPS
#if	FLAG_PROP_ID_PER_CHR
u8	mpd_get_tile_property( u8 _tile_ind, u8 _CHR_ind ) / _tile_ind - 0..255, _CHR_ind - 0..3
void	mpd_set_tile_property( u8 _tile_ind, u8 _CHR_ind, u8 _new_prop ) / _tile_ind - 0..255, _CHR_ind - 0..3, _new_prop - a new property value
#else	//FLAG_PROP_ID_PER_BLOCK
u8	mpd_get_tile_property( u8 _tile_ind ) / _tile_ind - 0..255
void	mpd_set_tile_property( u8 _tile_ind, u8 _new_prop ) / _tile_ind - 0..255, _new_prop - a new property value
#endif	//FLAG_PROP_ID_PER_CHR
#endif	//MPD_RAM_TILE_PROPS

#if	FLAG_MODE_BIDIR_SCROLL + FLAG_MODE_BIDIR_STAT_SCR
DEPRECATED -->	mpd_SCR_DATA*	mpd_curr_screen()
u16	mpd_get_adj_screen( mpd_SCR_DATA* _scr_data, u8 _ind ) / _ind: ADJ_SCR_LEFT,ADJ_SCR_RIGHT,ADJ_SCR_UP,ADJ_SCR_DOWN; RES: 0xffff - no screen; screen index (FLAG_LAYOUT_ADJ_SCR_INDS) OR screen address (FLAG_LAYOUT_ADJ_SCR)
bool	mpd_check_adj_screen( u8 _ind ) / _ind: ADJ_SCR_LEFT,ADJ_SCR_RIGHT,ADJ_SCR_UP,ADJ_SCR_DOWN;
void	mpd_draw_screen_by_data( mpd_SCR_DATA* _scr_data )
void	mpd_draw_screen_by_data_offs( mpd_SCR_DATA* _scr_data, u16 _BAT_offset )
void	mpd_get_start_screen( mpd_SCR_DATA* _scr_data )
void	mpd_get_screen_data( mpd_SCR_DATA* _scr_data, u16 _scr_ind )
#endif	//FLAG_MODE_BIDIR_SCROLL + FLAG_MODE_BIDIR_STAT_SCR

void	mpd_init_screen_arr( mpd_SCR_DATA* _scr_data, u8 _map_ind ) - must be called once for each map(!)
void	mpd_copy_screen( mpd_SCR_DATA* _src_scr, mpd_SCR_DATA* _dst_scr )

#if	FLAG_ENTITIES
void	mpd_init_base_ent_arr() - must be called once for all maps(!)
u16	mpd_get_base_ent_cnt()
void	mpd_get_base_entity( mpd_SCR_DATA* _scr_data, u16 _ent_ind ) - RES: _scr_data->inst_ent.base

void	mpd_get_entity( mpd_SCR_DATA* _scr_data, u8 _ent_ind )
bool	mpd_find_entity_by_base_id( mpd_SCR_DATA* _scr_data, u8 _id )
bool	mpd_find_entity_by_inst_id( mpd_SCR_DATA* _scr_data, u8 _id )
#endif	//FLAG_ENTITIES

--------------------------------------------------------
*/

// Some re-defines to ASM
#if	FLAG_TILES4X4
#asm
FLAG_TILES4X4
#endasm
#endif

#if	FLAG_TILES2X2
#asm
FLAG_TILES2X2
#endasm
#endif

#if	FLAG_DIR_COLUMNS
#asm
FLAG_DIR_COLUMNS
#endasm
#endif

#if	FLAG_DIR_ROWS
#asm
FLAG_DIR_ROWS
#endasm
#endif


/*********************/
/*		     */
/* Utility functions */
/*		     */
/*********************/

u16	__fastcall mpd_farpeekw( u16 far* _addr<__bl:__si>, u16 _offset<__ax> );

u16	__fastcall __macro mpd_farpeekw( u8 _bank<__bl>, u16 _addr<__si>, u16 _offset<__ax> );

u8	__fastcall __macro mpd_farpeekb( u8 far* _addr<__bl:__si>, u16 _offset<__ax> );

u8	__fastcall __macro mpd_farpeekb( u8 _bank<__bl>, u16 _addr<__si>, u16 _offset<__ax> );
// fast copy of data less than 32 bytes!
void	__fastcall mpd_farmemcpy_fast( u8 _bank<__bl>, u16 _addr<__si>, u16 _offset<__ax>, void* _dst_addr<__dx>, u8 _size<__bh> );
// fast copy of data less than 32 bytes!
void	__fastcall __macro mpd_memcpy_fast( u16* _src_addr<__ax>, u16 _dst_addr<__bx>, u8 _size<__cl> );

void	__fastcall mpd_get_ptr24( void far* _addr<__bl:__si>, u8 _ind<__al>, void* _dst_addr<__dx> );

void	__fastcall __nop mpd_farptr( u8 _bank<__bl>, u16 _addr<__si> );

/************************/
/*			*/
/* HuC helper functions	*/
/*			*/
/************************/

void	__fastcall __macro mpd_load_vram2( u16 _vaddr<__di>, u8 _bank<__bl>, u16 _addr<__si>, u16 _words_cnt<__cx> );

void	__fastcall __macro mpd_load_vram( u16 _vaddr<__di>, u8 far* _addr<__bl:__si>, u16 _offset<__ax>, u16 _words_cnt<__cx> );

void	__fastcall __macro mpd_load_palette( u8 _sub_plt<__bh>, u8 far* _addr<__bl:__si>, u16 _offset<__dx>, u8 _sub_plts_cnt<__cl> );

void	__fastcall __macro mpd_load_bat( u16 _vaddr<__di>, u8 far* _addr<__bl:__si>, u16 _offset<__ax>, u8 _width<__cl>, u8 _height<__ch> );

// FOR INTERNAL USE:

u16	__fastcall __mpd_get_VRAM_addr( u16 _x<__ax>, u16 _y<acc> );

#if	FLAG_MODE_MULTIDIR_SCROLL
u8	__fastcall __macro __mpd_calc_skip_CHRs_cnt( u8 _pos<acc> );

#if	FLAG_TILES4X4
// HuC	return ( ( ( _pos >> 4 ) & 0x01 ) << 1 ) + ( ( _pos >> 3 ) & 0x01 );
#asm
	.macro ___mpd_calc_skip_CHRs_cnt.1

	txa
	tay

	lsr a
	lsr a
	lsr a
	lsr a
	and #1
	asl a

	sta <__al		; ( ( ( _pos >> 4 ) & 0x01 ) << 1 )

	tya

	lsr a
	lsr a
	lsr a
	and #1			; ( ( _pos >> 3 ) & 0x01 )

	clc
	adc <__al

	tax
	cla

	.endm
#endasm
#else
// HuC	return ( ( _pos >> 3 ) & 0x01 );
#asm
	.macro ___mpd_calc_skip_CHRs_cnt.1

	txa

	lsr a
	lsr a
	lsr a
	and #1

	tax
	cla

	.endm
#endasm
#endif	//FLAG_TILES4X4
#endif	//FLAG_MODE_MULTIDIR_SCROLL

/* asm implementations */

#asm
; macroses

; \1 *= 2
	.macro mpd_mul2_word
	asl low_byte \1
	rol high_byte \1
	.endm	

; \2 = \1 + \2
	.macro mpd_add_word_to_word
	clc
	lda low_byte \1
	adc low_byte \2
	sta low_byte \2
	lda high_byte \1
	adc high_byte \2
	sta high_byte \2
	.endm

; \3 = \1 + \2
	.macro mpd_add_word_to_word2
	clc
	lda low_byte \1
	adc low_byte \2
	sta low_byte \3
	lda high_byte \1
	adc high_byte \2
	sta high_byte \3
	.endm

; \1 += A
	.macro mpd_add_a_to_word
	clc
	adc low_byte \1
	sta low_byte \1

	bcc .cont\@
	inc high_byte \1
.cont\@:
	.endm

; \2 = \1 + A
	.macro mpd_add_a_to_word2
	clc
	adc low_byte \1
	sta low_byte \2
	lda high_byte \1
	adc #$00
	sta high_byte \2
	.endm

; \2 = \1 + AX
	.macro mpd_add_ax_to_word2
	clc
	sax
	adc low_byte \1
	sta low_byte \2
	sax
	adc high_byte \1
	sta high_byte \2
	.endm

; if( xa < 0 ) { xa = 0; }
	.macro	mpd_clamp_neg_xa
	bit #$80
	beq .cont\@

	cla
	clx
.cont\@:
	.endm

; VDC macroses

	.macro mpd_vreg
	lda \1
	sta <vdc_reg
	st0 \1
	.endm

	.zp

_mpd_ax:
_mpd_axs:
_mpd_al:	.ds 1
_mpd_ah:	.ds 1

_mpd_bx:
_mpd_bxs:
_mpd_bl:	.ds 1
_mpd_bh:	.ds 1

_mpd_cx:
_mpd_cxs:
_mpd_cl:	.ds 1
_mpd_ch:	.ds 1

_mpd_dx:
_mpd_dxs:
_mpd_dl:	.ds 1
_mpd_dh:	.ds 1

_mpd_ex:
_mpd_exs:
_mpd_el:	.ds 1
_mpd_eh:	.ds 1

_mpd_fx:
_mpd_fxs:
_mpd_fl:	.ds 1
_mpd_fh:	.ds 1

	.bss

__mTII:		.ds 1	; $73 tii
__mbsrci:	.ds 2
__mbdsti:	.ds 2
__mbleni:	.ds 2
__mtiirts	.ds 1	; $60 rts

	.code

;u16 __fastcall __macro mpd_farpeekw( u8 _bank<__bl>, u16 _addr<__si>, u16 _offset<__ax> )
;
	.macro _mpd_farpeekw.3

	call _mpd_farpeekw.2

	.endm

;u8 __fastcall __macro mpd_farpeekb( u8 far* _addr<__bl:__si>, u16 _offset<__ax> )
;
	.macro _mpd_farpeekb.2

	_mpd_farpeekb.3

	.endm

;u8 __fastcall __macro mpd_farpeekb( u8 _bank<__bl>, u16 _addr<__si>, u16 _offset<__ax> )
;
	.macro _mpd_farpeekb.3

	call _mpd_farptr_add_offset

	lda	<__bl
	tam	#3

	lda	[__si]

	tax
	cla

	.endm

;void __fastcall mpd_farmemcpy_fast( u8 _bank<__bl>, u16 _addr<__si>, u16 _offset<__ax>, void* _dst_addr<__dx>, u8 _size<__bh> )
;
	.proc _mpd_farmemcpy_fast.5

	lda <__bh
	beq .exit

	call _mpd_farptr_add_offset

	lda <__bh
	sta __mbleni
	stz __mbleni + 1

	jsr map_data

	stw <__si, __mbsrci
	stw <__dx, __mbdsti

	jsr __mTII

	jmp unmap_data
.exit:
	rts
	.endp

;void __fastcall __macro mpd_memcpy_fast( u16* _src_addr<__ax>, u16 _dst_addr<__bx>, u8 _size<__cl> )
;
	.macro _mpd_memcpy_fast.3

	stw <__ax, __mbsrci
	stw <__bx, __mbdsti
	
	lda <__cl
	sta __mbleni
	stz __mbleni + 1

	jsr __mTII

	.endm

;void __fastcall mpd_get_ptr24( void far* _addr<__bl:__si>, u8 _ind<__al>, void* _dst_addr<__dx> )
;
	.proc _mpd_get_ptr24.3

	stz <__ah
	stw <__ax, <__cx

	mpd_mul2_word <__ax
	mpd_add_word_to_word <__cx, <__ax

	call _mpd_farptr_add_offset

	jsr map_data

	cly

	; copy address

	lda [<__si], y
	sta [<__dx], y
	iny

	lda [<__si], y
	sta [<__dx], y
	iny

	; copy bank

	lda [<__si], y
	sta [<__dx], y

	jmp unmap_data

	.endp

; void __fastcall __macro mpd_load_vram2( u16 _vaddr<__di>, u8 _bank<__bl>, u16 _addr<__si>, u16 _words_cnt<__cx> )
;
	.macro _mpd_load_vram2.4

	jsr _load_vram.3

	.endm

;void __fastcall __macro mpd_load_vram( u16 _vaddr<__di>, u8* _addr<__bl:__si>, u16 _offset<__ax>, u16 _words_cnt<__cx> )
;
	.macro _mpd_load_vram.4

	call _mpd_farptr_add_offset

	jsr _load_vram.3

	.endm

;void __fastcall __macro mpd_load_palette( u8 _sub_plt<__bh>, u8 far* _addr<__bl:__si>, u16 _offset<__dx>, u8 _sub_plts_cnt<__cl> )
;
	.macro _mpd_load_palette.4

	stw <__dx, <__ax
	call _mpd_farptr_add_offset

	lda <__bh
	sta <__al

	jsr _load_palette.3

	.endm

;void __fastcall __macro mpd_load_bat( u16 _vaddr<__di>, u8 far* _addr<__bl:__si>, u16 _offset<__ax>, u8 _width<__cl>, u8 _height<__ch> )
;	
	.macro _mpd_load_bat.5

	call _mpd_farptr_add_offset

	jsr _load_bat.4

	.endm

#endasm

// variables for local use in functions and for assembly optimizations
// NOTE: MPD functions can change these vars, so be careful when using them outside the library

extern u16	mpd_ax;
extern s16	mpd_axs;
extern u8	mpd_al;
extern u8	mpd_ah;

extern u16	mpd_bx;
extern s16	mpd_bxs;
extern u8	mpd_bl;
extern u8	mpd_bh;

extern u16	mpd_cx;
extern s16	mpd_cxs;
extern u8	mpd_cl;
extern u8	mpd_ch;

extern u16	mpd_dx;
extern s16	mpd_dxs;
extern u8	mpd_dl;
extern u8	mpd_dh;

extern u16	mpd_ex;
extern s16	mpd_exs;
extern u8	mpd_el;
extern u8	mpd_eh;

extern u16	mpd_fx;
extern s16	mpd_fxs;
extern u8	mpd_fl;
extern u8	mpd_fh;

// built-in MagicKit variables

extern u16	_ax;
extern u8	_al;
extern u8	_ah;
extern u16	_bx;
extern u8	_bl;
extern u8	_bh;
extern u16	_cx;
extern u8	_cl;
extern u8	_ch;
extern u16	_dx;
extern u8	_dl;
extern u8	_dh;

/********************/
/*		    */
/* Screens/Entities */
/*		    */
/********************/

typedef struct
{
	u16	addr;
	u8	bank;

} mpd_PTR24;

#if	FLAG_ENTITIES
typedef struct
{
	u8	id;
	u8	width;
	u8	height;
	u8	pivot_x;
	u8	pivot_y;
	u8	props_cnt;

} mpd_ENTITY_BASE;

typedef struct
{
	u8	id;
	u16	base_ent_addr;
	u16	targ_ent_addr;	// mpd_ENTITY_INSTANCE
	u16	x_pos;
	u16	y_pos;
	u8	props_cnt;

} mpd_ENTITY_INSTANCE;

typedef struct
{
	mpd_ENTITY_INSTANCE	inst;
	u8			inst_props[ ENT_MAX_PROPS_CNT ];

	mpd_ENTITY_BASE		base;
	u8			base_props[ ENT_MAX_PROPS_CNT ];

} mpd_ENTITY;

mpd_PTR24	__base_ent_arr;

#endif	//FLAG_ENTITIES

typedef struct
{
	mpd_PTR24		scr_arr_ptr;

	u16			scr_offset;

	mpd_SCREEN		scr;

#if	FLAG_ENTITIES
	mpd_ENTITY		inst_ent;
	mpd_ENTITY		targ_ent;
#endif	//FLAG_ENTITIES

} mpd_SCR_DATA;


#if	!FLAG_MODE_MULTIDIR_SCROLL
mpd_SCR_DATA	mpd_curr_scr;

#if	FLAG_LAYOUT_ADJ_SCR_INDS
mpd_PTR24	__scr_arr;
#endif	//FLAG_LAYOUT_ADJ_SCR_INDS

void	__mpd_get_screen_data( u16 _scr_ind, mpd_SCREEN* _scr )
{
#if	FLAG_LAYOUT_ADJ_SCR_INDS
	mpd_curr_scr.scr_offset	= mpd_farpeekw( __scr_arr.bank, __scr_arr.addr, ( _scr_ind << 1 ) ) - mpd_curr_scr.scr_arr_ptr.addr;
#else
	mpd_curr_scr.scr_offset	= _scr_ind - mpd_curr_scr.scr_arr_ptr.addr;	// _scr_ind is a screen address for FLAG_LAYOUT_ADJ_SCR
#endif
	mpd_farmemcpy_fast( mpd_curr_scr.scr_arr_ptr.bank, mpd_curr_scr.scr_arr_ptr.addr, mpd_curr_scr.scr_offset, _scr, sizeof( mpd_SCREEN ) );
}

void	mpd_get_screen_data( mpd_SCR_DATA* _scr_data, u16 _scr_ind )
{
#if	FLAG_LAYOUT_ADJ_SCR_INDS
	_scr_data->scr_offset	= mpd_farpeekw( __scr_arr.bank, __scr_arr.addr, ( _scr_ind << 1 ) ) - _scr_data->scr_arr_ptr.addr;
#else
	_scr_data->scr_offset	= _scr_ind - _scr_data->scr_arr_ptr.addr;	// _scr_ind is a screen address for FLAG_LAYOUT_ADJ_SCR
#endif
	mpd_farmemcpy_fast( _scr_data->scr_arr_ptr.bank, _scr_data->scr_arr_ptr.addr, _scr_data->scr_offset, _scr_data->scr, sizeof( mpd_SCREEN ) );
}
#endif	//!FLAG_MODE_MULTIDIR_SCROLL

void	mpd_init_screen_arr( mpd_SCR_DATA* _scr_data, u8 _map_ind )
{
	// init MPD's TII
	#asm
	lda #$73
	sta __mTII
	lda #$60
	sta __mtiirts
	#endasm

	mpd_get_ptr24( mpd_LayoutsArr, _map_ind, _scr_data->scr_arr_ptr );
}

#define	mpd_copy_screen( _src_scr, _dst_scr )	mpd_memcpy_fast( _src_scr, _dst_scr, sizeof( mpd_SCR_DATA ) )
//void	mpd_copy_screen( mpd_SCR_DATA* _src_scr, mpd_SCR_DATA* _dst_scr )
//{
//	mpd_memcpy_fast( _src_scr, _dst_scr, sizeof( mpd_SCR_DATA ) );
//}

#if	FLAG_MODE_MULTIDIR_SCROLL
#define	mpd_get_map_size( _map_ind )	mpd_farpeekw( mpd_LayoutsDimArr, ( _map_ind << 1 ) )
//u16	mpd_get_map_size( u8 _map_ind )
//{
//	return mpd_farpeekw( mpd_LayoutsDimArr, ( _map_ind << 1 ) );
//}

#define	mpd_get_start_screen_ind( _map_ind )	mpd_farpeekb( mpd_StartScrArr, _map_ind )
//u8	mpd_get_start_screen_ind( u8 _map_ind )
//{
//	return mpd_farpeekb( mpd_StartScrArr, _map_ind );
//}

void	mpd_get_screen_data( mpd_SCR_DATA* _scr_data, u8 _scr_ind )
{
	_scr_data->scr_offset	= mpd_farpeekw( _scr_data->scr_arr_ptr.bank, _scr_data->scr_arr_ptr.addr, ( _scr_ind << 1 ) ) - _scr_data->scr_arr_ptr.addr;

	mpd_farmemcpy_fast( _scr_data->scr_arr_ptr.bank, _scr_data->scr_arr_ptr.addr, _scr_data->scr_offset, _scr_data->scr, sizeof( mpd_SCREEN ) );
}

#define	mpd_get_start_screen( _scr_data, _map_ind )	mpd_get_screen_data( _scr_data, mpd_get_start_screen_ind( _map_ind ) )
//void	mpd_get_start_screen( mpd_SCR_DATA* _scr_data, u8 _map_ind )
//{
//	mpd_get_screen_data( _scr_data, mpd_get_start_screen_ind( _map_ind ) );
//}
#else	//FLAG_MODE_MULTIDIR_SCROLL

void	mpd_get_start_screen( mpd_SCR_DATA* _scr_data )
{
	_scr_data->scr_offset	= mpd_farpeekw( _scr_data->scr_arr_ptr.bank, _scr_data->scr_arr_ptr.addr, 0 ) - _scr_data->scr_arr_ptr.addr;

	mpd_farmemcpy_fast( _scr_data->scr_arr_ptr.bank, _scr_data->scr_arr_ptr.addr, _scr_data->scr_offset, _scr_data->scr, sizeof( mpd_SCREEN ) );
}
#endif	//FLAG_MODE_MULTIDIR_SCROLL

#if	FLAG_ENTITIES
#define	mpd_init_base_ent_arr()	mpd_get_ptr24( mpd_BaseEntities, 0, &__base_ent_arr )
//void	mpd_init_base_ent_arr()
//{
//	mpd_get_ptr24( mpd_BaseEntities, 0, &__base_ent_arr );
//}

#define	mpd_get_base_ent_cnt()	mpd_farpeekw( __base_ent_arr.bank, __base_ent_arr.addr, 0 )
//u16	mpd_get_base_ent_cnt()
//{
//	return mpd_farpeekw( __base_ent_arr.bank, __base_ent_arr.addr, 0 );
//}

void	mpd_get_base_entity( mpd_SCR_DATA* _scr_data, u16 _ent_ind )
{
//	u16	ent_offset;	mpd_bx

	mpd_bx = mpd_farpeekw( __base_ent_arr.bank, __base_ent_arr.addr, ( _ent_ind << 1 ) + 2 ) - _scr_data->scr_arr_ptr.addr;

	// copy base entity
	mpd_farmemcpy_fast( _scr_data->scr_arr_ptr.bank, _scr_data->scr_arr_ptr.addr, mpd_bx, _scr_data->inst_ent.base, sizeof( mpd_ENTITY_BASE ) + ENT_MAX_PROPS_CNT );
}

void	mpd_get_entity( mpd_SCR_DATA* _scr_data, u8 _ent_ind )
{
//	u16	ent_offset;	mpd_bx
	static mpd_ENTITY*	ent;

	// entity instance address
	mpd_bx	= _scr_data->scr_offset + sizeof( mpd_SCREEN ) + ( _ent_ind << 1 );

//	__mpd_get_entity_by_offs( _scr_data, mpd_bx, _scr_data->inst_ent );
//[inlined]
//void	__mpd_get_entity_by_offs( mpd_SCR_DATA* _scr_data, u16 /*_offset*/mpd_bx, mpd_ENTITY* /*_ent*/_scr_data->inst_ent )
//{
//	u16	ent_offset;	mpd_cx

	ent = _scr_data->inst_ent;

	mpd_ah = _scr_data->scr_arr_ptr.bank;
	mpd_dx = _scr_data->scr_arr_ptr.addr;

	mpd_el = sizeof( mpd_ENTITY_INSTANCE ) + ENT_MAX_PROPS_CNT;
	mpd_eh = sizeof( mpd_ENTITY_BASE ) + ENT_MAX_PROPS_CNT;

	// entity address offset
	mpd_cx	= mpd_farpeekw( mpd_ah, mpd_dx, mpd_bx ) - mpd_dx;

	// copy entity instance
	mpd_farmemcpy_fast( mpd_ah, mpd_dx, mpd_cx, ent->inst, mpd_el );

	// base entity offset
	mpd_cx	= ent->inst.base_ent_addr - mpd_dx;

	// copy base entity
	mpd_farmemcpy_fast( mpd_ah, mpd_dx, mpd_cx, ent->base, mpd_eh );
//}
	if( ent.inst.targ_ent_addr )
	{
//		__mpd_get_entity_by_addr( _scr_data, _scr_data->inst_ent.inst.targ_ent_addr, _scr_data->targ_ent );
//[inlined]
//void	__mpd_get_entity_by_addr( mpd_SCR_DATA* _scr_data, u16 /*_addr*/_scr_data->inst_ent.inst.targ_ent_addr, mpd_ENTITY* /*_ent*/_scr_data->targ_ent )
//{
//		u16	ent_offset;	mpd_cx

		// entity address offset
		mpd_cx	= ent->inst.targ_ent_addr - mpd_dx;

		ent = _scr_data->targ_ent;

		// copy entity instance
		mpd_farmemcpy_fast( mpd_ah, mpd_dx, mpd_cx, ent->inst, mpd_el );

		// base entity offset
		mpd_cx	= ent->inst.base_ent_addr - mpd_dx;

		// copy base entity
		mpd_farmemcpy_fast( mpd_ah, mpd_dx, mpd_cx, ent->base, mpd_eh );
//}
	}
#if 0
	put_string( "ent_inst:", 0, 0 );
	put_hex( _scr_data->inst_ent.inst.id,		4, 0, 1 );
	put_hex( _scr_data->inst_ent.inst.base_ent_addr,4, 0, 2 );
	put_hex( _scr_data->inst_ent.inst.targ_ent_addr,4, 0, 3 );
	put_hex( _scr_data->inst_ent.inst.x_pos,	4, 0, 4 );
	put_hex( _scr_data->inst_ent.inst.y_pos,	4, 0, 5 );
	put_hex( _scr_data->inst_ent.inst.props_cnt, 	4, 0, 6 );

	put_string( "ent_base:", 0, 7 );
	put_hex( _scr_data->inst_ent.base.id,		4, 0, 8 );
	put_hex( _scr_data->inst_ent.base.width,	4, 0, 9 );
	put_hex( _scr_data->inst_ent.base.height,	4, 0, 10 );
	put_hex( _scr_data->inst_ent.base.pivot_x,	4, 0, 11 );
	put_hex( _scr_data->inst_ent.base.pivot_y,	4, 0, 12 );
	put_hex( _scr_data->inst_ent.base.props_cnt, 	4, 0, 13 );

	if( _scr_data->inst_ent.inst.targ_ent_addr )
	{
		put_string( "targ_ent_inst:", 10, 0 );
		put_hex( _scr_data->targ_ent.inst.id,		4, 10, 1 );
		put_hex( _scr_data->targ_ent.inst.base_ent_addr,4, 10, 2 );
		put_hex( _scr_data->targ_ent.inst.targ_ent_addr,4, 10, 3 );
		put_hex( _scr_data->targ_ent.inst.x_pos,	4, 10, 4 );
		put_hex( _scr_data->targ_ent.inst.y_pos,	4, 10, 5 );
		put_hex( _scr_data->targ_ent.inst.props_cnt, 	4, 10, 6 );

		put_string( "targ_ent_base:", 10, 7 );
		put_hex( _scr_data->targ_ent.base.id,		4, 10, 8 );
		put_hex( _scr_data->targ_ent.base.width,	4, 10, 9 );
		put_hex( _scr_data->targ_ent.base.height,	4, 10, 10 );
		put_hex( _scr_data->targ_ent.base.pivot_x,	4, 10, 11 );
		put_hex( _scr_data->targ_ent.base.pivot_y,	4, 10, 12 );
		put_hex( _scr_data->targ_ent.base.props_cnt, 	4, 10, 13 );
	}
#endif
}

bool	mpd_find_entity_by_base_id( mpd_SCR_DATA* _scr_data, u8 _id )
{
//	u8	i;	mpd_al

	for( mpd_al = 0; mpd_al < _scr_data->scr.ents_cnt; mpd_al++ )
	{
		mpd_get_entity( _scr_data, mpd_al );

		if( _scr_data->inst_ent.base.id == _id )
		{
			return TRUE;
		}
	}

	return FALSE;
}

bool	mpd_find_entity_by_inst_id( mpd_SCR_DATA* _scr_data, u8 _id )
{
//	u8	i;	mpd_al

	for( mpd_al = 0; mpd_al < _scr_data->scr.ents_cnt; mpd_al++ )
	{
		mpd_get_entity( _scr_data, mpd_al );

		if( _scr_data->inst_ent.inst.id == _id )
		{
			return TRUE;
		}
	}

	return FALSE;
}
#endif	//FLAG_ENTITIES


/*********************/
/*		     */
/* Debug info colors */
/*		     */
/*********************/

#define	DBG_COLOR_PRE_ROW_COL_FILLING	320	// light green
#define	DBG_COLOR_ROW_COL_FILLING	256	// green
#define	DBG_COLOR_DRAW_SCREEN		4	// blue
#define	DBG_COLOR_GET_TILE_PROP		504	// yellow

#define	DBG_BORDER_COLOR_GET_TILE_PROP		DBG_MPD_BORDER_COLOR( DBG_COLOR_GET_TILE_PROP )
#define	DBG_BORDER_COLOR_DRAW_SCREEN		DBG_MPD_BORDER_COLOR( DBG_COLOR_DRAW_SCREEN )
#define	DBG_BORDER_COLOR_PRE_ROW_COL_FILLING	DBG_MPD_BORDER_COLOR( DBG_COLOR_PRE_ROW_COL_FILLING )
#define	DBG_BORDER_COLOR_ROW_COL_FILLING	DBG_MPD_BORDER_COLOR( DBG_COLOR_ROW_COL_FILLING )
#define	DBG_BORDER_COLOR_RESET			DBG_MPD_BORDER_COLOR( 0 )

#define DBG_MPD_BORDER_COLOR( _clr )		pokew( 0x0402, 0x0100 ); pokew( 0x0404, _clr );


/*****************/
/*		 */
/* RLE functions */
/*		 */
/*****************/
#if	FLAG_RLE * FLAG_MODE_STAT_SCR
u16	__stat_scr_buff[ ScrGfxDataSize >> 1 ];

void	__mpd_UNRLE_stat_scr( u16 _offset )
{
	static u16	src;
	static u16	dst;
	static u16 	val1;
	static u16 	val2;
	static u16	val3;
	static u16	cnt;

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
/* Tilemap rendering functions	*/
/*				*/
/********************************/

const u8 mpd_ver[] = { "M", "P", "D", "0", "9", 0 };

/* flags */

const u8 ADJ_SCR_LEFT	= 0;
const u8 ADJ_SCR_UP	= 1;
const u8 ADJ_SCR_RIGHT	= 2;
const u8 ADJ_SCR_DOWN	= 3;

/* variables */

u16		__VDC_CR_IW;
u16		__VDC_CR;

#if	!FLAG_MODE_MULTIDIR_SCROLL
u16		__scr_offset;

u16		__scr_tiles_width_tbl[ ScrTilesHeight ];
u16		__scr_tiles_height_tbl[ ScrTilesWidth ];
#endif	//!FLAG_MODE_MULTIDIR_SCROLL

#if	FLAG_MODE_MULTIDIR_SCROLL
u16		mpd_map_tiles_width;
u16		mpd_map_tiles_height;
u8		mpd_map_scr_width;
u8		mpd_map_scr_height;
u16		mpd_map_active_width;	// active map area = map_width_in_pixels - scr_width_in_pixels
u16		mpd_map_active_height;	// the same in height
u16		__init_tiles_offset;	// to draw a start screen

u16		__map_ind_mul2;

#if	FLAG_DIR_ROWS
u16		__height_scr_step;
#endif	//FLAG_DIR_ROWS
#endif	//FLAG_MODE_MULTIDIR_SCROLL

u8		__curr_chr_id_mul2;
u8		__BAT_width		= 0xff;	// 0xff initial value to avoid BAT data initialization every time on mpd_init(...)
u16		__BAT_width_dec1;
u16		__BAT_width_dec1_inv;
u8		__BAT_height_dec1;
u8		__BAT_width_pow2;
u16		__BAT_size;
u16		__BAT_size_dec1;	// ( BAT_width * BAT_height ) - 1

u16		__blocks_offset;
#if	FLAG_TILES4X4
u16		__tiles_offset;
#endif

u16		mpd_tiles_cnt;		// 2x2 or 4x4 it depends on export options

/* constants */

const u16	__c_scr_tiles_size	= ScrTilesWidth * ScrTilesHeight;

const u8	SCR_CHRS_WIDTH	 = SCR_BLOCKS2x2_WIDTH << 1;
const u8	SCR_CHRS_HEIGHT	 = SCR_BLOCKS2x2_HEIGHT << 1;

#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
#if	FLAG_MODE_MULTIDIR_SCROLL
const u8	COLUMN_CHRS_CNT_INC1	= ( SCR_CHRS_HEIGHT ) + 1;
const u8	COLUMN_CHRS_CNT		= SCR_CHRS_HEIGHT;
const u8	ROW_CHRS_CNT		= ( SCR_CHRS_WIDTH ) + 1;
#else
const u8	COLUMN_CHRS_CNT	= SCR_CHRS_HEIGHT;
const u8	ROW_CHRS_CNT	= SCR_CHRS_WIDTH;
#endif

#define	UPD_FLAG_DRAW_LEFT	0x01
#define	UPD_FLAG_DRAW_RIGHT	0x02
#define	UPD_FLAG_DRAW_UP	0x04
#define	UPD_FLAG_DRAW_DOWN	0x08
#define	UPD_FLAG_DRAW_MASK	UPD_FLAG_DRAW_LEFT|UPD_FLAG_DRAW_RIGHT|UPD_FLAG_DRAW_UP|UPD_FLAG_DRAW_DOWN

u8	mpd_scroll_step_x;
u8	mpd_scroll_step_y;
s16	mpd_scroll_x;
s16	mpd_scroll_y;

#if	FLAG_MODE_BIDIR_SCROLL
s16	__horiz_dir_pos;
s16	__vert_dir_pos;
#endif

u8	__upd_flags;
#endif

/* RAM copy of a map data */

#if	FLAG_MODE_MULTIDIR_SCROLL

/* Map LUT */

#ifdef	MPD_RAM_MAP_TBL

#ifndef	MPD_RAM_DATA_COPY
#define	MPD_RAM_DATA_COPY
#endif	//MPD_RAM_DATA_COPY

#asm
MPD_RAM_MAP_TBL
#endasm

u8	__RAM_MapTbl[ MAX_MAP_TBL_SIZE ];	// RAM copy of a map LUT

u16	__fastcall __macro __mpd_get_map_tbl_val( u16 _offset<__ax> );
#asm
	.macro ___mpd_get_map_tbl_val.1

	mpd_add_word_to_word #___RAM_MapTbl, <__ax
	lda [<__ax]
	tax

	ldy #$01
	lda [<__ax], y

	.endm
#endasm

#else	//!MPD_RAM_MAP_TBL

u16	__map_tbl_offset;

#define	__mpd_get_map_tbl_val( _offs ) mpd_farpeekw( mpd_MapsTbl, __map_tbl_offset + ( _offs ) )

#endif	//MPD_RAM_MAP_TBL

/* Tilemap data */

#ifdef	MPD_RAM_MAP

#ifndef	MPD_RAM_DATA_COPY
#define	MPD_RAM_DATA_COPY
#endif	//MPD_RAM_DATA_COPY

#asm
MPD_RAM_MAP
#endasm

u8	__RAM_Map[ MAX_MAP_SIZE ];		// RAM copy of a map data

u8	__fastcall __macro __mpd_get_map_tile( u16 _offset<__dx> );
#asm
	.macro	___mpd_get_map_tile.1

	mpd_add_word_to_word #___RAM_Map, <__dx
	lda [<__dx]
	tax
	cla

	.endm
#endasm

void	__fastcall __macro __mpd_put_map_tile( u16 _offset<__dx>, u8 _tile_ind<acc> );
#asm
	.macro ___mpd_put_map_tile.2

	mpd_add_word_to_word #___RAM_Map, <__dx

	txa		; _tile_id
	sta [<__dx]

	.endm
#endasm

void	__fastcall __macro __mpd_calc_map_offset( u16 _x<__ax>, u16 _y<__bx>, bool _pixels<__cl> );
#asm
	.macro ___mpd_calc_map_offset.3

	; if( _pixels )

	lda <__cl

	beq .cont\@

	lda <__ah	; _x

	lsr a
	ror <__al
	lsr a
	ror <__al
	lsr a
	ror <__al
	lsr a
	ror <__al

.ifdef	FLAG_TILES4X4
	lsr a
	ror <__al
.endif

	sta <__ah	; _x

	lda <__bh	; _y

	lsr a
	ror <__bl
	lsr a
	ror <__bl
	lsr a
	ror <__bl
	lsr a
	ror <__bl

.ifdef	FLAG_TILES4X4
	lsr a
	ror <__bl
.endif

	sta <__bh	; _y

.cont\@:

.ifdef	FLAG_DIR_COLUMNS
.ifdef	MPD_RAM_MAP_TBL

	; __dx = __RAM_MapTbl[ __ax << 1 ] + __bx;

	lda <__al
	asl a
	rol <__ah
	sta <__al

	___mpd_get_map_tbl_val.1

	mpd_add_ax_to_word2 <__bx, <__dx

.else	;!MPD_RAM_MAP_TBL

	; __dx = ( mpd_map_tiles_height * __ax ) + __bx;

	stw <__bx, <_mpd_bx
	stw _mpd_map_tiles_height, <__bx
	jsr mulu16

	mpd_add_word_to_word2 <_mpd_bx, <__cx, <__dx

.endif	;MPD_RAM_MAP_TBL
.else	;FLAG_DIR_ROWS
.ifdef	MPD_RAM_MAP_TBL

	; __dx = __RAM_MapTbl[ __bx << 1 ] + __ax;

	stw <__ax, <_mpd_ax

	lda <__bl
	ldx <__bh

	asl a
	sax
	rol a
	sta <__ah
	sax
	sta <__al

	___mpd_get_map_tbl_val.1

	mpd_add_ax_to_word2 <_mpd_ax, <__dx

.else	;!MPD_RAM_MAP_TBL

	; __dx = ( mpd_map_tiles_width * __bx ) + __ax;

	stw <__ax, <_mpd_ax
	stw _mpd_map_tiles_width, <__ax
	jsr mulu16

	mpd_add_word_to_word2 <_mpd_ax, <__cx, <__dx

.endif	;MPD_RAM_MAP_TBL
.endif	;FLAG_DIR_COLUMNS

	.endm
#endasm

#ifdef	MPD_DEBUG
void	mpd_set_tile( u16 _x, u16 _y, bool _pixels, u8 _tile_ind )	// _pixels = TRUE - pixel coordinates, FALSE - tile coordinates
{
	__mpd_calc_map_offset( _x, _y, _pixels );

	if( _dx >= ( mpd_map_tiles_width * mpd_map_tiles_height ) )
	{
		__mpd_err_msg( "ERROR: OUT OF RANGE!", "mpd_set_tile", _x, _y, _pixels );
	}

	__mpd_put_map_tile( _dx, _tile_ind );
}

u8	mpd_get_tile( u16 _x, u16 _y, bool _pixels )	// _pixels = TRUE - pixel coordinates, FALSE - tile coordinates
{
	__mpd_calc_map_offset( _x, _y, _pixels );

	if( _dx >= ( mpd_map_tiles_width * mpd_map_tiles_height ) )
	{
		__mpd_err_msg( "ERROR: OUT OF RANGE!", "mpd_get_tile", _x, _y, _pixels );
	}

	__mpd_get_map_tile( _dx );
}
#else	//!MPD_DEBUG
void	__fastcall __macro mpd_set_tile( u16 _x<__ax>, u16 _y<__bx>, bool _pixels<__cl>, u8 _tile_ind<acc> );
#asm
	.macro _mpd_set_tile.4

	phx

	___mpd_calc_map_offset.3

	plx

	___mpd_put_map_tile.2

	.endm
#endasm

u8	__fastcall __macro mpd_get_tile( u16 _x<__ax>, u16 _y<__bx>, bool _pixels<__cl> );
#asm
	.macro _mpd_get_tile.3

	___mpd_calc_map_offset.3

	___mpd_get_map_tile.1

	.endm
#endasm
#endif	//MPD_DEBUG

#else	//!MPD_RAM_MAP

mpd_PTR24	__map_ptr;

#define	__mpd_get_map_tile( _offs ) mpd_farpeekb( __map_ptr.bank, __map_ptr.addr, (_offs) )

#endif	//MPD_RAM_MAP
#endif	//FLAG_MODE_MULTIDIR_SCROLL

/* Tile properties data */

#ifdef	MPD_RAM_TILE_PROPS

#ifndef	MPD_RAM_DATA_COPY
#define	MPD_RAM_DATA_COPY
#endif	//MPD_RAM_DATA_COPY

#asm
MPD_RAM_TILE_PROPS
#endasm

u8	__RAM_TileProps[ MAX_TILE_PROPS_SIZE ];

u16	mpd_tile_props_arr_size;

#ifdef	MPD_DEBUG
#if	FLAG_PROP_ID_PER_CHR
u8	mpd_get_tile_property( u8 _tile_ind, u8 _CHR_ind )
{
	mpd_ex = _tile_ind;	// 8-bit to 16-bit

	mpd_dx = ( mpd_ex << 2 ) + _CHR_ind;

	if( mpd_dx >= mpd_tile_props_arr_size )
	{
		__mpd_err_msg( "ERROR: OUT OF RANGE!", "mpd_get_tile_property", mpd_ex, _CHR_ind, -1 );
	}

	return __RAM_TileProps[ mpd_dx ];
}

void	mpd_set_tile_property( u8 _tile_ind, u8 _CHR_ind, u8 _new_prop )
{
	mpd_ex = _tile_ind;	// 8-bit to 16-bit

	mpd_dx = ( mpd_ex << 2 ) + _CHR_ind;

	if( mpd_dx >= mpd_tile_props_arr_size )
	{
		__mpd_err_msg( "ERROR: OUT OF RANGE!", "mpd_set_tile_property", mpd_ex, _CHR_ind, _new_prop );
	}

	__RAM_TileProps[ mpd_dx ] = _new_prop;
}
#else	//FLAG_PROP_ID_PER_BLOCK
u8	mpd_get_tile_property( u8 _tile_ind )
{
	if( _tile_ind >= mpd_tile_props_arr_size )
	{
		__mpd_err_msg( "ERROR: OUT OF RANGE!", "mpd_get_tile_property", _tile_ind, -1, -1 );
	}

	return __RAM_TileProps[ _tile_ind ];
}

void	mpd_set_tile_property( u8 _tile_ind, u8 _new_prop )
{
	if( _tile_ind >= mpd_tile_props_arr_size )
	{
		__mpd_err_msg( "ERROR: OUT OF RANGE!", "mpd_set_tile_property", _tile_ind, _new_prop, -1 );
	}

	__RAM_TileProps[ _tile_ind ] = _new_prop;
}
#endif	//FLAG_PROP_ID_PER_CHR
#else	//!MPD_DEBUG
#if	FLAG_PROP_ID_PER_CHR
u8	__fastcall __macro mpd_get_tile_property( u8 _tile_ind<__al>, u8 _CHR_ind<acc> );
#asm
	.macro _mpd_get_tile_property.2

	; __ax = _tile_ind << 2

	lda <__al
	stz <__ah

	asl a
	rol <__ah
	asl a
	rol <__ah

	sta <__al

	; __ax += _CHR_ind

	txa				; _CHR_ind

	mpd_add_a_to_word <__ax

	; ax = ___RAM_TileProps[ __ax ]

	mpd_add_word_to_word #___RAM_TileProps, <__ax

	lda [<__ax]
	tax
	cla

	.endm
#endasm

void	__fastcall __macro mpd_set_tile_property( u8 _tile_ind<__al>, u8 _CHR_ind<__bl>, u8 _new_prop<acc> );
#asm
	.macro _mpd_set_tile_property.3

	; __ax = _tile_ind << 2

	lda <__al
	stz <__ah

	asl a
	rol <__ah
	asl a
	rol <__ah

	sta <__al

	; __ax += _CHR_ind

	lda <__bl			; _CHR_ind

	mpd_add_a_to_word <__ax

	; ___RAM_TileProps[ __ax ] = _new_prop

	mpd_add_word_to_word #___RAM_TileProps, <__ax

	txa				; _new_prop
	sta [<__ax]

	.endm
#endasm
#else	//FLAG_PROP_ID_PER_BLOCK
u8	__fastcall __macro mpd_get_tile_property( u8 _tile_ind<acc> );
#asm
	.macro _mpd_get_tile_property.1

	txa

	mpd_add_a_to_word2 #___RAM_TileProps, <__ax

	lda [<__ax]
	tax
	cla

	.endm
#endasm
	
void	__fastcall __macro mpd_set_tile_property( u8 _tile_ind<__al>, u8 _new_prop<acc> );
#asm
	.macro _mpd_set_tile_property.2

	lda <__al

	mpd_add_a_to_word2 #___RAM_TileProps, <__ax

	txa				; _new_prop
	sta [<__ax]

	.endm
#endasm
#endif	//FLAG_PROP_ID_PER_CHR
#endif	//MPD_DEBUG

#else	//!MPD_RAM_TILE_PROPS

u16	__props_offset;

#endif	//MPD_RAM_TILE_PROPS

#ifdef	MPD_RAM_DATA_COPY

#ifdef	MPD_DEBUG
void	__mpd_err_msg( char* _err, char* _func, s16 _val1, s16 _val2, s16 _val3 )
{
	disp_off();
	vsync();

	cls();

	init_satb();
	satb_update();

	load_default_font( 0, 0x2c00 );

	set_color( 0, 0 );
	set_color( 1, 511 );

	pokew( 0x220c, 0 );
	pokew( 0x2210, 0 );

	put_string( _err, 0, 0 );
	put_string( _func, 0, 1 );

	if( _val1 >= 0 )
	{
		put_number( _val1, 5, 0, 2 );

		if( _val2 >= 0 )
		{
			put_number( _val2, 5, 0, 3 );

			if( _val3 >= 0 )
			{
				put_number( _val3, 5, 0, 4 );
			}
		}
	}

	disp_on();
	vsync();

	for(;;){}
}
#endif	//MPD_DEBUG

void	__fastcall __macro mpd_farmemcpy( u8 _bank<__bl>, u16 _addr<__si>, u16 _offset<__ax>, void* _dst_addr<__dx>, u16 _size<__cx> );
#asm
	.macro _mpd_farmemcpy.5

	call _mpd_farmemcpy.4

	.endm
#endasm

void	__fastcall mpd_farmemcpy( u16 far* _addr<__bl:__si>, u16 _offset<__ax>, void* _dst_addr<__dx>, u16 _size<__cx> );
#asm
	.proc _mpd_farmemcpy.4

	call _mpd_farptr_add_offset

	jsr map_data

	ldx <__si
	stx __mbsrci
	ldy <__si + 1
	sty __mbsrci + 1

	stw <__dx, __mbdsti

	lda #$20
	sta __mbleni
	stz __mbleni + 1

	; __cx /= 32 - get num 32-byte chunks

	lda <__cl
	lsr <__ch
	ror a
	lsr <__ch
	ror a
	lsr <__ch
	ror a
	lsr <__ch
	ror a
	lsr <__ch
	ror a

	sax			; x=chunks-lo
	beq .l4			; a=source-lo, y=source-hi

	; copy 32-byte chunks

.l1:	jsr __mTII		; transfer 32-bytes

	pha
	lda #$20
	mpd_add_a_to_word __mbdsti
	pla

	clc			; increment source
	adc #$20
	sta __mbsrci
	bcc .l3
	iny

	bpl .l2			; remap_data
	tay
	tma4
	tam3
	inc a
	tam4
	tya
	ldy #$60
.l2:	sty __mbsrci + 1

.l3:	dex
	bne .l1
.l4:	dec <__ch
	bpl .l1

	; copy data remainder

	lda <__cl
	and #31
	beq .l5

	sta __mbleni + 0
	jsr __mTII		; transfer remainder
.l5:
	jmp unmap_data

	.endp
#endasm

#endif	//MPD_RAM_DATA_COPY

void	__mpd_update_data_offsets()
{
	u8	next_chr_id_mul2;

	next_chr_id_mul2 = ( ( __curr_chr_id_mul2 >> 1 ) + 1 ) << 1;

#ifdef	MPD_RAM_TILE_PROPS
	mpd_ax = mpd_farpeekw( mpd_PropsOffs, __curr_chr_id_mul2 );
	mpd_cx = mpd_farpeekw( mpd_PropsOffs, next_chr_id_mul2 );

	mpd_tile_props_arr_size = mpd_cx - mpd_ax;

	mpd_farmemcpy( mpd_Props, mpd_ax, __RAM_TileProps, mpd_tile_props_arr_size );
#else
	__props_offset	= mpd_farpeekw( mpd_PropsOffs,	__curr_chr_id_mul2 );
#endif	//MPD_RAM_MAP

	__blocks_offset	= mpd_farpeekw( mpd_BlocksOffs,	__curr_chr_id_mul2 );

#if	FLAG_TILES4X4
	__tiles_offset	= mpd_farpeekw( mpd_TilesOffs,	__curr_chr_id_mul2 );

	mpd_tiles_cnt = ( mpd_farpeekw( mpd_TilesOffs, next_chr_id_mul2 ) - __tiles_offset ) >> 2;
#else	//FLAG_TILES2X2
	mpd_tiles_cnt = ( mpd_farpeekw( mpd_BlocksOffs, next_chr_id_mul2 ) - __blocks_offset ) >> 3;
#endif	//FLAG_TILES4X4
}

#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL + FLAG_MODE_BIDIR_STAT_SCR
void	mpd_draw_CHR( u16 _x, u16 _y, u8 _block2x2_ind, u8 _CHR_ind )
{
	mpd_load_vram( __mpd_get_VRAM_addr( _x, _y ), mpd_Attrs, __blocks_offset + ( _block2x2_ind << 3 ) + ( _CHR_ind << 1 ), 1 );
}

void	mpd_draw_block2x2( u16 _x, u16 _y, u8 _block2x2_ind )
{
//	u16	offs;	mpd_fx

	static u16 pos_x;
	static u16 pos_y;

	pos_x = _x;
	pos_y = _y;

	mpd_fx	= __blocks_offset + ( _block2x2_ind << 3 );

	mpd_load_vram( __mpd_get_VRAM_addr( pos_x, pos_y ), mpd_Attrs, mpd_fx, 1 );

	mpd_fx += 2;
	pos_x  += 8;
	mpd_load_vram( __mpd_get_VRAM_addr( pos_x, pos_y ), mpd_Attrs, mpd_fx, 1 );

	mpd_fx += 4;
	pos_y  += 8;
	mpd_load_vram( __mpd_get_VRAM_addr( pos_x, pos_y ), mpd_Attrs, mpd_fx, 1 );

	mpd_fx -= 2;
	pos_x  -= 8;
	mpd_load_vram( __mpd_get_VRAM_addr( pos_x, pos_y ), mpd_Attrs, mpd_fx, 1 );
}

#define	__mpd_draw_block2x2( _vaddr, _offset )	mpd_load_vram( _vaddr, mpd_Attrs, _offset, 2 );	mpd_load_vram( _vaddr + __BAT_width, mpd_Attrs, _offset + 4, 2 )
//void	__mpd_draw_block2x2( u16 _vaddr, u16 _offset )
//{
//	mpd_load_vram( _vaddr, mpd_Attrs, _offset, 2 );
//	mpd_load_vram( _vaddr + __BAT_width, mpd_Attrs, _offset + 4, 2 );
//}

#if	FLAG_TILES4X4
void	mpd_draw_tile4x4( u16 _x, u16 _y, u8 _tile4x4_ind )
{
//	u16	tiles12;	mpd_dx
//	u16	tiles34;	mpd_ex
//	u16	offs;		mpd_cx

	mpd_ax = _x;
	mpd_bx = _y;

	mpd_cx	= __tiles_offset + ( _tile4x4_ind << 2 );

	mpd_dx = mpd_farpeekw( mpd_Tiles, mpd_cx );
	mpd_ex = mpd_farpeekw( mpd_Tiles, mpd_cx + 2 );

	mpd_draw_block2x2( mpd_ax, mpd_bx,	mpd_dx );
	mpd_draw_block2x2( mpd_ax + 16, mpd_bx,	( mpd_dx >> 8 ) );
	mpd_bx += 16;
	mpd_draw_block2x2( mpd_ax, mpd_bx,	mpd_ex );
	mpd_draw_block2x2( mpd_ax + 16, mpd_bx,	( mpd_ex >> 8 ) );
}

void	__mpd_draw_tile4x4( u16 _vaddr, u16 _offset )
{
	static u16	tiles12;
	static u16	tiles34;
	static u16	vaddr;

	vaddr = _vaddr;

	tiles12 = mpd_farpeekw( mpd_Tiles, _offset );
	tiles34 = mpd_farpeekw( mpd_Tiles, _offset + 2 );

	__mpd_draw_block2x2( vaddr,	__blocks_offset + ( ( tiles12 & 0x00ff ) << 3 ) );
	__mpd_draw_block2x2( vaddr + 2,	__blocks_offset + ( ( tiles12 & 0xff00 ) >> 5 ) );
	vaddr += ( __BAT_width << 1 );
	__mpd_draw_block2x2( vaddr,	__blocks_offset + ( ( tiles34 & 0x00ff ) << 3 ) );
	__mpd_draw_block2x2( vaddr + 2,	__blocks_offset + ( ( tiles34 & 0xff00 ) >> 5 ) );
}
#endif	//FLAG_TILES4X4
#endif	//FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL + FLAG_MODE_BIDIR_STAT_SCR

#if	FLAG_MODE_BIDIR_SCROLL + FLAG_MODE_BIDIR_STAT_SCR
void	__mpd_draw_tiled_screen( u16 _BAT_offset )
{
//	u8	scr_tile;	mpd_al
//	u8	w, h;		mpd_bl / mpd_bh
//	u16	h_acc;		mpd_cx
//	u16	n;		mpd_dx

	mpd_ex = _BAT_offset;
	mpd_dx = 0;

#if	FLAG_DIR_COLUMNS

	for( mpd_bl = 0; mpd_bl < ScrTilesWidth; mpd_bl++ )
	{
		mpd_cx = 0;

		for( mpd_bh = 0; mpd_bh < ScrTilesHeight; mpd_bh++ )
		{
			mpd_al = mpd_farpeekb( mpd_TilesScr, __scr_offset + mpd_dx );
#if	FLAG_TILES2X2
			__mpd_draw_block2x2( mpd_ex + ( ( mpd_cx + mpd_bl ) << 1 ), __blocks_offset + ( mpd_al << 3 ) );
#elif	FLAG_TILES4X4
			__mpd_draw_tile4x4( mpd_ex + ( ( mpd_cx + mpd_bl ) << 2 ), __tiles_offset + ( mpd_al << 2 ) );
#endif
			mpd_cx += __BAT_width;
			++mpd_dx;
		}
	}
#elif	FLAG_DIR_ROWS

	mpd_cx = 0;

	for( mpd_bh = 0; mpd_bh < ScrTilesHeight; mpd_bh++ )
	{
		for( mpd_bl = 0; mpd_bl < ScrTilesWidth; mpd_bl++ )
		{
			mpd_al = mpd_farpeekb( mpd_TilesScr, __scr_offset + mpd_dx );
#if	FLAG_TILES2X2
			__mpd_draw_block2x2( mpd_ex + ( ( mpd_cx + mpd_bl ) << 1 ), __blocks_offset + ( mpd_al << 3 ) );
#elif	FLAG_TILES4X4
			__mpd_draw_tile4x4( mpd_ex + ( ( mpd_cx + mpd_bl ) << 2 ), __tiles_offset + ( mpd_al << 2 ) );
#endif
			++mpd_dx;
		}

		mpd_cx += __BAT_width;
	}

#endif	//FLAG_DIR_COLUMNS|FLAG_DIR_ROWS
}
#endif	//FLAG_MODE_BIDIR_SCROLL + FLAG_MODE_BIDIR_STAT_SCR

#if	FLAG_MODE_MULTIDIR_SCROLL
#define	__mpd_calc_scr_pos_by_scr_ind( _scr_ind, _reset_scroll )	__mpd_calc_scr_pos_by_LUT_pos( ScrTilesWidth * ( _scr_ind % mpd_map_scr_width ), ScrTilesHeight * ( _scr_ind / mpd_map_scr_width ), _reset_scroll )
//void	__mpd_calc_scr_pos_by_scr_ind( u8 _scr_ind, bool _reset_scroll )
//{
//	__mpd_calc_scr_pos_by_LUT_pos( ScrTilesWidth * ( _scr_ind % mpd_map_scr_width ), ScrTilesHeight * ( _scr_ind / mpd_map_scr_width ), _reset_scroll );
//}

void	__mpd_calc_scr_pos_by_LUT_pos( u16 _LUT_pos_x, u16 _LUT_pos_y, bool _reset_scroll )
{
	mpd_ax = _LUT_pos_x;
	mpd_bx = _LUT_pos_y;

	if( _reset_scroll )
	{
		mpd_scroll_x	= 0;
		mpd_scroll_y	= 0;
	}
	else
	{
#if	FLAG_TILES2X2
		mpd_scroll_x	= mpd_ax << 4;
		mpd_scroll_y	= mpd_bx << 4;
#elif	FLAG_TILES4X4
		mpd_scroll_x	= mpd_ax << 5;
		mpd_scroll_y	= mpd_bx << 5;
#endif	//FLAG_TILES2X2|FLAG_TILES4X4
	}

#if	FLAG_DIR_COLUMNS
	__init_tiles_offset	= __mpd_get_map_tbl_val( mpd_ax << 1 ) + mpd_bx;
#elif	FLAG_DIR_ROWS
	__height_scr_step	= mpd_map_tiles_width * ScrTilesHeight;
	__init_tiles_offset	= __mpd_get_map_tbl_val( mpd_bx << 1 ) + mpd_ax;
#endif	//FLAG_DIR_COLUMNS|FLAG_DIR_ROWS
}

void	__mpd_draw_tiled_screen( u16 _BAT_offset )
{
//	u8	w, h;		mpd_cl / mpd_ch
//	u16	h_acc;		mpd_bx
//	u16	vaddr;		mpd_ax

	static u16	n;

	mpd_ax	= _BAT_offset + ( ( mpd_scroll_x >> 3 ) & __BAT_width_dec1 ) + ( ( ( mpd_scroll_y >> 3 ) /*& __BAT_height_dec1*/ ) << __BAT_width_pow2 );

#if	FLAG_DIR_COLUMNS

	for( mpd_cl = 0; mpd_cl < ScrTilesWidth; mpd_cl++ )
	{
		mpd_bx = 0;

		n = mpd_cl * mpd_map_tiles_height;

		for( mpd_ch = 0; mpd_ch < ScrTilesHeight; mpd_ch++ )
		{
#asm
			; <__cx = ( mpd_ax + ( mpd_bx << 1 ) )

			lda <_mpd_bl
			asl a
			sta <__bl
			lda <_mpd_bh
			rol a
			sta <__bh
#endasm

#if	FLAG_TILES4X4
#asm
			mpd_mul2_word <__bx
#endasm
#endif

#asm
			mpd_add_word_to_word2 <_mpd_ax, <__bx, <__cx

			; <__al = mpd_dx & __BAT_width_dec1_inv

			lda <__cl
			and ___BAT_width_dec1_inv
			sta <__al

			; mpd_dx = ( ( ( <__cx + ( mpd_cl << 1 ) ) & __BAT_width_dec1 ) | <__al ) & __BAT_size_dec1

			lda <_mpd_cl
			asl a
#endasm

#if	FLAG_TILES4X4
#asm
			asl a
#endasm
#endif

#asm
			clc
			adc <__cl

			and ___BAT_width_dec1
			ora <__al

			sta <_mpd_dl

			lda <__ch
			and ___BAT_size_dec1 + 1
			sta <_mpd_dh
#endasm

#if	FLAG_TILES2X2
			__mpd_draw_block2x2( mpd_dx, __blocks_offset + ( __mpd_get_map_tile( __init_tiles_offset + n ) << 3 ) );
#elif	FLAG_TILES4X4
			__mpd_draw_tile4x4( mpd_dx, __tiles_offset + ( __mpd_get_map_tile( __init_tiles_offset + n ) << 2 ) );
#endif
			mpd_bx += __BAT_width;
			++n;
		}
	}
#elif	FLAG_DIR_ROWS

	mpd_bx = 0;

	for( mpd_ch = 0; mpd_ch < ScrTilesHeight; mpd_ch++ )
	{
		n = mpd_ch * mpd_map_tiles_width;

#if	FLAG_TILES2X2
		mpd_dx	= mpd_ax + ( mpd_bx << 1 );
#elif	FLAG_TILES4X4
		mpd_dx	= mpd_ax + ( mpd_bx << 2 );
#endif
		mpd_ex	= mpd_dx & __BAT_width_dec1_inv;

		for( mpd_cl = 0; mpd_cl < ScrTilesWidth; mpd_cl++ )
		{
#if	FLAG_TILES2X2
#asm
			; mpd_fx = ( ( ( mpd_dx + ( mpd_cl << 1 ) ) & __BAT_width_dec1 ) | mpd_ex ) & __BAT_size_dec1

			lda <_mpd_cl
			asl a
#endasm
#elif	FLAG_TILES4X4
#asm
			; mpd_fx = ( ( ( mpd_dx + ( mpd_cl << 2 ) ) & __BAT_width_dec1 ) | mpd_ex ) & __BAT_size_dec1

			lda <_mpd_cl
			asl a
			asl a
#endasm
#endif

#asm
			clc
			adc <_mpd_dl

			and ___BAT_width_dec1
			ora <_mpd_el

			sta <_mpd_fl

			lda <_mpd_dh
			and ___BAT_size_dec1 + 1
			sta <_mpd_fh
#endasm

#if	FLAG_TILES2X2
			__mpd_draw_block2x2( mpd_fx, __blocks_offset + ( __mpd_get_map_tile( __init_tiles_offset + n ) << 3 ) );
#elif	FLAG_TILES4X4
			__mpd_draw_tile4x4( mpd_fx, __tiles_offset + ( __mpd_get_map_tile( __init_tiles_offset + n ) << 2 ) );
#endif
			++n;
		}

		mpd_bx += __BAT_width;
	}

#endif	//FLAG_DIR_COLUMNS|FLAG_DIR_ROWS
}

void	__mpd_draw_free_tiled_screen()
{
//	u8	x_beg_CHR;		mpd_fl
//	u8	y_beg_CHR;		mpd_fh
//	u8	w, h;			mpd_el / mpd_eh
//	u8	width_cnt, height_cnt;	mpd_al / mpd_ah
//	u16	CHR_x;			mpd_bx
//	u16	CHR_y;			mpd_cx
//	u16	n;			mpd_dx

	static u8	scr_tile;

	mpd_bx	= mpd_scroll_x;
	mpd_cx	= mpd_scroll_y;

	mpd_fl	= __mpd_calc_skip_CHRs_cnt( mpd_scroll_x );
	mpd_fh	= __mpd_calc_skip_CHRs_cnt( mpd_scroll_y );

	mpd_al	= mpd_fl + SCR_CHRS_WIDTH + ( mpd_scroll_x & 0x07 ? 1:0 );
	mpd_ah	= mpd_fh + SCR_CHRS_HEIGHT + ( mpd_scroll_y & 0x07 ? 1:0 );

#if	FLAG_DIR_COLUMNS

	for( mpd_el = mpd_fl; mpd_el < mpd_al; mpd_el++ )
	{
#if	FLAG_TILES2X2
		mpd_dx = ( mpd_el >> 1 ) * mpd_map_tiles_height;
#else
		mpd_dx = ( mpd_el >> 2 ) * mpd_map_tiles_height;
#endif
		mpd_cx = mpd_scroll_y;

		for( mpd_eh = mpd_fh; mpd_eh < mpd_ah; mpd_eh++ )
		{
			scr_tile = __mpd_get_map_tile( __init_tiles_offset + mpd_dx );
#if	FLAG_TILES4X4
			scr_tile = mpd_farpeekw( mpd_Tiles, __tiles_offset + ( scr_tile << 2 ) + ( mpd_eh & 0x02 ) + ( ( mpd_el & 0x02 ) >> 1 ) );
#endif
			mpd_load_vram( __mpd_get_VRAM_addr( mpd_bx, mpd_cx ), mpd_Attrs, __blocks_offset + ( scr_tile << 3 ) + ( ( mpd_eh & 0x01 ) << 2 ) + ( ( mpd_el & 0x01 ) << 1 ), 1 );
#if	FLAG_TILES2X2
			mpd_dx += mpd_eh & 0x01;
#elif	FLAG_TILES4X4
			mpd_dx += ( ( mpd_eh & 0x03 ) == 0x03 ) ? 1:0;
#endif
			mpd_cx += 8;
		}

		mpd_bx += 8;
	}
#elif	FLAG_DIR_ROWS

	for( mpd_eh = mpd_fh; mpd_eh < mpd_ah; mpd_eh++ )
	{
#if	FLAG_TILES2X2
		mpd_dx = ( mpd_eh >> 1 ) * mpd_map_tiles_width;
#else
		mpd_dx = ( mpd_eh >> 2 ) * mpd_map_tiles_width;
#endif
		mpd_bx = mpd_scroll_x;

		for( mpd_el = mpd_fl; mpd_el < mpd_al; mpd_el++ )
		{
			scr_tile = __mpd_get_map_tile( __init_tiles_offset + mpd_dx );
#if	FLAG_TILES4X4
			scr_tile = mpd_farpeekw( mpd_Tiles, __tiles_offset + ( scr_tile << 2 ) + ( mpd_eh & 0x02 ) + ( ( mpd_el & 0x02 ) >> 1 ) );
#endif
			mpd_load_vram( __mpd_get_VRAM_addr( mpd_bx, mpd_cx ), mpd_Attrs, __blocks_offset + ( scr_tile << 3 ) + ( ( mpd_eh & 0x01 ) << 2 ) + ( ( mpd_el & 0x01 ) << 1 ), 1 );
#if	FLAG_TILES2X2
			mpd_dx += mpd_el & 0x01;
#elif	FLAG_TILES4X4
			mpd_dx += ( ( mpd_el & 0x03 ) == 0x03 ) ? 1:0;
#endif
			mpd_bx += 8;
		}

		mpd_cx += 8;
	}

#endif	//FLAG_DIR_COLUMNS|FLAG_DIR_ROWS
}

#define	mpd_draw_screen_by_ind( _scr_ind )	mpd_draw_screen_by_ind_offs( _scr_ind, 0, FALSE )
//void	mpd_draw_screen_by_ind( u8 _scr_ind )
//{
//	mpd_draw_screen_by_ind_offs( _scr_ind, 0, FALSE );
//}

void	mpd_draw_screen_by_ind_offs( u8 _scr_ind, u16 _BAT_offset, bool _reset_scroll )
{
	__upd_flags = 0;

	__mpd_calc_scr_pos_by_scr_ind( _scr_ind, _reset_scroll );

	__mpd_draw_screen( _BAT_offset, 0 );
}

void	mpd_draw_screen_by_pos( u16 _x, u16 _y )
{
//	u16	LUT_pos_x;	mpd_ax
//	u16	LUT_pos_y;	mpd_bx

	__upd_flags = 0;

#if	FLAG_TILES2X2
	mpd_ax = _x >> 4;
	mpd_bx = _y >> 4;
#elif	FLAG_TILES4X4
	mpd_ax = _x >> 5;
	mpd_bx = _y >> 5;
#endif

	__mpd_calc_scr_pos_by_LUT_pos( mpd_ax, mpd_bx, FALSE );

	mpd_scroll_x = _x;
	mpd_scroll_y = _y;

#if	FLAG_TILES2X2
	if( !( mpd_scroll_x & 0x0f ) && !( mpd_scroll_y & 0x0f ) )
#elif	FLAG_TILES4X4
	if( !( mpd_scroll_x & 0x1f ) && !( mpd_scroll_y & 0x1f ) )
#endif
	{
		__mpd_draw_screen( 0, 0 );
	}
	else
	{
		__mpd_draw_screen( 0, 1 );
	}
}
#endif	//FLAG_MODE_MULTIDIR_SCROLL

#if	FLAG_MODE_BIDIR_SCROLL + FLAG_MODE_BIDIR_STAT_SCR
#define	mpd_draw_screen_by_data( _scr_data )	mpd_draw_screen_by_data_offs( _scr_data, 0 )
//void	mpd_draw_screen_by_data( mpd_SCR_DATA* _scr_data )
//{
//	mpd_draw_screen_by_data_offs( _scr_data, 0 );
//}

void	mpd_draw_screen_by_data_offs( mpd_SCR_DATA* _scr_data, u16 _BAT_offset )
{
	mpd_copy_screen( _scr_data, &mpd_curr_scr );

	__scr_offset	= mpd_curr_scr.scr.scr_ind * __c_scr_tiles_size;

#if	FLAG_MODE_BIDIR_SCROLL
	mpd_scroll_x	= 0;
	mpd_scroll_y	= 0;
	__horiz_dir_pos	= 0;
	__vert_dir_pos	= 0;
	__upd_flags	= 0;
#endif	//FLAG_MODE_BIDIR_SCROLL

	__mpd_draw_screen( _BAT_offset, 0 );
}
#endif	//FLAG_MODE_BIDIR_SCROLL + FLAG_MODE_BIDIR_STAT_SCR

#define	mpd_draw_screen()	__mpd_draw_screen( 0, 0 )
//void	mpd_draw_screen()
//{
//	__mpd_draw_screen( 0, 0 );
//}

void	__mpd_draw_screen( u16 _BAT_offset, u8 _free_scr )
{
//	u16		num_CHRs;	mpd_bx
	static mpd_PTR24 CHRs_ptr;

#ifdef	MPD_DEBUG
	DBG_BORDER_COLOR_DRAW_SCREEN
#endif
	// load tiles & palette

#if	FLAG_MODE_MULTIDIR_SCROLL
	{
		mpd_bx = mpd_farpeekw( mpd_CHRs_size, __curr_chr_id_mul2 ) >> 5;

#else
	mpd_bx = mpd_farpeekw( mpd_CHRs_size, ( mpd_curr_scr.scr.chr_id << 1 ) ) >> 5;

	if( mpd_curr_scr.scr.chr_id != ( __curr_chr_id_mul2 >> 1 ) )
	{
		__curr_chr_id_mul2 = mpd_curr_scr.scr.chr_id << 1;

		__mpd_update_data_offsets();

#endif	//FLAG_MODE_MULTIDIR_SCROLL

		mpd_get_ptr24( mpd_CHRs, __curr_chr_id_mul2 >> 1, &CHRs_ptr );

		mpd_load_vram2( CHRS_OFFSET << 5, CHRs_ptr.bank, CHRs_ptr.addr, mpd_bx << 4 );
		mpd_load_palette( 0, mpd_Plts, __curr_chr_id_mul2 << 8, 16 );
	}

	// load BAT

#if	FLAG_MODE_STAT_SCR
#if	FLAG_RLE
	__mpd_UNRLE_stat_scr( mpd_curr_scr.scr.VDC_data_offset );
	mpd_load_bat( 0, __stat_scr_buff, 0, SCR_CHRS_WIDTH, SCR_CHRS_HEIGHT );
#else
	mpd_load_bat( 0, mpd_VDCScr, mpd_curr_scr.scr.VDC_data_offset, SCR_CHRS_WIDTH, SCR_CHRS_HEIGHT );
#endif	//FLAG_RLE
#elif	FLAG_MODE_MULTIDIR_SCROLL
	
	if( _free_scr )
	{
		__mpd_draw_free_tiled_screen();
	}
	else
	{
		__mpd_draw_tiled_screen( _BAT_offset );
	}

#elif	FLAG_MODE_BIDIR_SCROLL + FLAG_MODE_BIDIR_STAT_SCR
	__mpd_draw_tiled_screen( _BAT_offset );
#endif

#ifdef	MPD_DEBUG
	DBG_BORDER_COLOR_RESET
#endif
}

#if	!FLAG_MODE_MULTIDIR_SCROLL
u16	mpd_get_adj_screen( mpd_SCR_DATA* _scr_data, u8 _ind )
{
//	u16	adj_scr;	mpd_bx

#if	FLAG_MARKS
	if( !( _scr_data->scr.marks & ( 1 << ( _ind + 4 ) ) ) )
	{
		return 0xffff;
	}
#endif

	mpd_bx = _scr_data->scr.adj_scr[ _ind ];

#if	FLAG_LAYOUT_ADJ_SCR
	if( mpd_bx != NULL )
#else
	if( mpd_bx != 0xff )
#endif
	{
		return mpd_bx;
	}

	return 0xffff;
}

bool	mpd_check_adj_screen( u8 _ind )
{
//	u16	adj_scr;	mpd_bx

	mpd_bx = mpd_get_adj_screen( &mpd_curr_scr, _ind );

	if( mpd_bx != 0xffff )
	{
		__mpd_get_screen_data( mpd_bx, mpd_curr_scr.scr );

		__scr_offset	= mpd_curr_scr.scr.scr_ind * __c_scr_tiles_size;

		return TRUE;
	}

	return FALSE;
}
#endif	//!FLAG_MODE_MULTIDIR_SCROLL

/*********************/
/*		     */
/* Map init function */
/*		     */
/*********************/

#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
void	mpd_init( u8 _map_ind, u8 _step )
#else
void	mpd_init( u8 _map_ind )
#endif
{
#ifdef	MPD_RAM_MAP
	mpd_PTR24	map_ptr;
#endif
//	__mpd_get_BAT_params();
//[inlined]
//void	__mpd_get_BAT_params()
//{
//	u16 BAT_height;	mpd_ax

	if( __BAT_width == 0xff )
	{
		// init MPD's TII
#asm
		lda #$73
		sta __mTII
		lda #$60
		sta __mtiirts
#endasm

		mpd_ax = BAT_INDEX & 0x04 ? 64:32;

		if( BAT_INDEX & 0x02 )
		{
			__BAT_width = 128;
			__BAT_width_pow2 = 7;

#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
			__VDC_CR_IW = 0x18 << 8;
#endif
		}
		else
		if( BAT_INDEX & 0x01 )
		{
			__BAT_width = 64;
			__BAT_width_pow2 = 6;

#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
			__VDC_CR_IW = 0x10 << 8;
#endif
		}
		else
		{
			__BAT_width = 32;
			__BAT_width_pow2 = 5;

#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
			__VDC_CR_IW = 0x08 << 8;
#endif
		}

		__BAT_width_dec1	= __BAT_width - 1;
		__BAT_width_dec1_inv	= ~__BAT_width_dec1;
		__BAT_height_dec1	= mpd_ax - 1;
		__BAT_size		= __BAT_width * mpd_ax;
		__BAT_size_dec1		= __BAT_size - 1;

		set_screen_size( BAT_INDEX );

#if	!FLAG_MODE_MULTIDIR_SCROLL

		mpd_bx = 0;
		for( mpd_ax = 0; mpd_ax < ScrTilesHeight; mpd_ax++ )
		{
			__scr_tiles_width_tbl[ mpd_ax ] = mpd_bx;
			mpd_bx += ScrTilesWidth;
		}

		mpd_bx = 0;
		for( mpd_ax = 0; mpd_ax < ScrTilesWidth; mpd_ax++ )
		{
			__scr_tiles_height_tbl[ mpd_ax ] = mpd_bx;
			mpd_bx += ScrTilesHeight;
		}

#endif	//!FLAG_MODE_MULTIDIR_SCROLL
	}
//}

#if	!FLAG_MODE_MULTIDIR_SCROLL

	mpd_init_screen_arr( &mpd_curr_scr, _map_ind );
	mpd_get_start_screen( &mpd_curr_scr );

	__scr_offset	= mpd_curr_scr.scr.scr_ind * __c_scr_tiles_size;

#endif	//!FLAG_MODE_MULTIDIR_SCROLL

#if	FLAG_LAYOUT_ADJ_SCR_INDS
	mpd_get_ptr24( mpd_LayoutsScrArr, _map_ind, &__scr_arr );
#endif	//FLAG_LAYOUT_ADJ_SCR_INDS


#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
	mpd_scroll_step_x = _step;
	mpd_scroll_step_y = _step;
	mpd_scroll_x	= 0;
	mpd_scroll_y	= 0;
#if	FLAG_MODE_BIDIR_SCROLL
	__horiz_dir_pos	= 0;
	__vert_dir_pos	= 0;
#endif
	__upd_flags	= 0;

#endif	//FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL

#if	FLAG_MODE_MULTIDIR_SCROLL

	__map_ind_mul2		= _map_ind << 1;

	__curr_chr_id_mul2	= mpd_farpeekb( mpd_MapsCHRBanks, _map_ind ) << 1;

	mpd_ax			= mpd_get_map_size( _map_ind );
	mpd_map_scr_width	= mpd_ax & 0x00ff;
	mpd_map_scr_height	= ( mpd_ax & 0xff00 ) >> 8;

	mpd_map_tiles_width	= mpd_map_scr_width * ScrTilesWidth;
	mpd_map_tiles_height	= mpd_map_scr_height * ScrTilesHeight;

	mpd_map_active_width	= ( mpd_map_scr_width * ScrPixelsWidth ) - ScrPixelsWidth;
	mpd_map_active_height	= ( mpd_map_scr_height * ScrPixelsHeight ) - ScrPixelsHeight;

#ifdef	MPD_RAM_MAP
	mpd_get_ptr24( mpd_MapsArr, _map_ind, &map_ptr );
	mpd_farmemcpy( map_ptr.bank, map_ptr.addr, 0, __RAM_Map, mpd_map_tiles_width * mpd_map_tiles_height );
#else	//!MPD_RAM_MAP
	mpd_get_ptr24( mpd_MapsArr, _map_ind, &__map_ptr );
#endif	//MPD_RAM_MAP

#ifdef	MPD_RAM_MAP_TBL
#if	FLAG_DIR_ROWS
	mpd_farmemcpy( mpd_MapsTbl, mpd_farpeekw( mpd_MapsTblOffs, __map_ind_mul2 ), __RAM_MapTbl, mpd_map_tiles_height << 1 );
#else	//FLAG_DIR_COLUMNS
	mpd_farmemcpy( mpd_MapsTbl, mpd_farpeekw( mpd_MapsTblOffs, __map_ind_mul2 ), __RAM_MapTbl, mpd_map_tiles_width << 1 );
#endif	// FLAG_DIR_ROWS
#else	//!MPD_RAM_MAP_TBL
	__map_tbl_offset	= mpd_farpeekw( mpd_MapsTblOffs, __map_ind_mul2 );
#endif	//MPD_RAM_MAP_TBL

	__mpd_calc_scr_pos_by_scr_ind( mpd_get_start_screen_ind( _map_ind ), FALSE );

	__mpd_update_data_offsets();

#else	//FLAG_MODE_MULTIDIR_SCROLL
	__curr_chr_id_mul2	= 0xff;
#endif	//FLAG_MODE_MULTIDIR_SCROLL
}

/********************************/
/*				*/
/* Tilemap scrolling functions	*/
/*				*/
/********************************/

#if	FLAG_MODE_BIDIR_SCROLL
void	mpd_move_left()
{
//	u8	tmp_pos;	mpd_al

	if( __vert_dir_pos != 0 )
	{
		return;
	}

	if( __horiz_dir_pos == 0 )
	{
		if( mpd_get_adj_screen( &mpd_curr_scr, ADJ_SCR_LEFT ) == 0xffff )
		{
			return;
		}
	}

	mpd_al = mpd_scroll_x;
	mpd_al &= 0x07;

	__horiz_dir_pos -= mpd_scroll_step_x;
	mpd_scroll_x	-= mpd_scroll_step_x;

	if( __horiz_dir_pos < 0 )
	{
		__horiz_dir_pos += ScrPixelsWidth;

		if( mpd_check_adj_screen( ADJ_SCR_LEFT ) == FALSE )
		{
			mpd_scroll_x	+= ScrPixelsWidth - __horiz_dir_pos;
			__horiz_dir_pos = 0;

			return;
		}
	}

	if( ( ( mpd_al - mpd_scroll_step_x ) & 0x08 ) || !mpd_al )
	{
		__upd_flags |= UPD_FLAG_DRAW_LEFT;
	}
}

void	mpd_move_right()
{
//	u8	tmp_pos;	mpd_al

	if( __vert_dir_pos != 0 )
	{
		return;
	}

	if( __horiz_dir_pos == 0 )
	{
		if( mpd_get_adj_screen( &mpd_curr_scr, ADJ_SCR_RIGHT ) == 0xffff )
		{
			return;
		}
	}

	mpd_al = mpd_scroll_x;
	mpd_al &= 0x07;

	__horiz_dir_pos += mpd_scroll_step_x;
	mpd_scroll_x	+= mpd_scroll_step_x;

	if( __horiz_dir_pos >= ScrPixelsWidth )
	{
		__horiz_dir_pos -= ScrPixelsWidth;

		if( mpd_check_adj_screen( ADJ_SCR_RIGHT ) == FALSE )
		{
			mpd_scroll_x	-= __horiz_dir_pos;
			__horiz_dir_pos = 0;

			return;
		}
		else
		if( mpd_get_adj_screen( &mpd_curr_scr, ADJ_SCR_RIGHT ) == 0xffff )
		{
			mpd_scroll_x	-= __horiz_dir_pos;
			__horiz_dir_pos = 0;

			return;
		}
	}

	if( ( ( mpd_al + mpd_scroll_step_x ) & 0x08 ) || !mpd_al )
	{
		__upd_flags |= UPD_FLAG_DRAW_RIGHT;
	}
}

void	mpd_move_up()
{
//	u8	tmp_pos;	mpd_al

	if( __horiz_dir_pos != 0 )
	{
		return;
	}

	if( __vert_dir_pos == 0 )
	{
		if( mpd_get_adj_screen( &mpd_curr_scr, ADJ_SCR_UP ) == 0xffff )
		{
			return;
		}
	}

	mpd_al = mpd_scroll_y;
	mpd_al &= 0x07;

	__vert_dir_pos	-= mpd_scroll_step_y;
	mpd_scroll_y	-= mpd_scroll_step_y;

	if( __vert_dir_pos < 0 )
	{
		__vert_dir_pos += ScrPixelsHeight;

		if( mpd_check_adj_screen( ADJ_SCR_UP ) == FALSE )
		{
			mpd_scroll_y	+= ScrPixelsHeight - __vert_dir_pos;
			__vert_dir_pos = 0;

			return;
		}
	}

	if( ( ( mpd_al - mpd_scroll_step_y ) & 0x08 ) || !mpd_al )
	{
		__upd_flags |= UPD_FLAG_DRAW_UP;
	}
}

void	mpd_move_down()
{
//	u8	tmp_pos;	mpd_al

	if( __horiz_dir_pos != 0 )
	{
		return;
	}

	if( __vert_dir_pos == 0 )
	{
		if( mpd_get_adj_screen( &mpd_curr_scr, ADJ_SCR_DOWN ) == 0xffff )
		{
			return;
		}
	}

	mpd_al = mpd_scroll_y;
	mpd_al &= 0x07;

	__vert_dir_pos	+= mpd_scroll_step_y;
	mpd_scroll_y	+= mpd_scroll_step_y;

	if( __vert_dir_pos >= ScrPixelsHeight )
	{
		__vert_dir_pos -= ScrPixelsHeight;

		if( mpd_check_adj_screen( ADJ_SCR_DOWN ) == FALSE )
		{
			mpd_scroll_y	-= __vert_dir_pos;
			__vert_dir_pos = 0;

			return;
		}
		else
		if( mpd_get_adj_screen( &mpd_curr_scr, ADJ_SCR_DOWN ) == 0xffff )
		{
			mpd_scroll_y	-= __vert_dir_pos;
			__vert_dir_pos = 0;

			return;
		}
	}

	if( ( ( mpd_al + mpd_scroll_step_y ) & 0x08 ) || !mpd_al )
	{
		__upd_flags |= UPD_FLAG_DRAW_DOWN;
	}
}
#endif	//FLAG_MODE_BIDIR_SCROLL

#if	FLAG_MODE_MULTIDIR_SCROLL
void	mpd_move_left()
{
//	u8	tmp_pos;	mpd_al

	mpd_al = mpd_scroll_x;
	mpd_al &= 0x07;

	mpd_scroll_x -= mpd_scroll_step_x;

	if( mpd_scroll_x <= 0 )
	{
		mpd_scroll_x = 0;
		return;
	}

	if( ( ( mpd_al - mpd_scroll_step_x ) & 0x08 ) || !mpd_al )
	{
		__upd_flags |= UPD_FLAG_DRAW_LEFT;
	}
}

void	mpd_move_right()
{
//	u8	tmp_pos;	mpd_al

	mpd_al = mpd_scroll_x;
	mpd_al &= 0x07;

	mpd_scroll_x += mpd_scroll_step_x;

	if( mpd_scroll_x >= mpd_map_active_width )
	{
		mpd_scroll_x = mpd_map_active_width;
		return;
	}

	if( ( ( mpd_al + mpd_scroll_step_x ) & 0x08 ) || !mpd_al )
	{
		__upd_flags |= UPD_FLAG_DRAW_RIGHT;
	}
}

void	mpd_move_up()
{
//	u8	tmp_pos;	mpd_al

	mpd_al = mpd_scroll_y;
	mpd_al &= 0x07;

	mpd_scroll_y -= mpd_scroll_step_y;

	if( mpd_scroll_y <= 0 )
	{
		mpd_scroll_y = 0;
		return;
	}

	if( ( ( mpd_al - mpd_scroll_step_y ) & 0x08 ) || !mpd_al )
	{
		__upd_flags |= UPD_FLAG_DRAW_UP;
	}
}

void	mpd_move_down()
{
//	u8	tmp_pos;	mpd_al

	mpd_al = mpd_scroll_y;
	mpd_al &= 0x07;

	mpd_scroll_y += mpd_scroll_step_y;

	if( mpd_scroll_y >= mpd_map_active_height )
	{
		mpd_scroll_y = mpd_map_active_height;
		return;
	}

	if( ( ( mpd_al + mpd_scroll_step_y ) & 0x08 ) || !mpd_al )
	{
		__upd_flags |= UPD_FLAG_DRAW_DOWN;
	}
}

#define	mpd_get_curr_screen_ind()	( ( ( mpd_scroll_y / ScrPixelsHeight ) * mpd_map_scr_width ) + ( mpd_scroll_x / ScrPixelsWidth ) )
//u8	mpd_get_curr_screen_ind()
//{
//	return ( ( mpd_scroll_y / ScrPixelsHeight ) * mpd_map_scr_width ) + ( mpd_scroll_x / ScrPixelsWidth );
//}
#endif	//FLAG_MODE_MULTIDIR_SCROLL

/**********************************/
/*				  */
/* Performance-critical functions */
/*				  */
/**********************************/
#asm
	.procgroup	mpd_map_scrolling_tile_property_funcs

; *** farptr += offset ***
;
; IN:
;
; __ax - offset
; __bl - bank number
; __si - address
;
	.proc _mpd_farptr_add_offset

	; add an offset

	clc
	lda <__ax
	adc <__si
	sta <__si
	lda <__si+1
	and #$1f
	adc <__ax+1

	tay

	; increment a bank number

	lsr a
	lsr a
	lsr a
	lsr a
	lsr a
	clc
	adc <__bl
	sta <__bl

	; save high byte of a bank address

	tya
	and #$1f
	ora #$60
	sta <__si+1
	
	rts

	.endp

;u16 __fastcall mpd_farpeekw( u16 far* _addr<__bl:__si>, u16 _offset<__ax> )
;
	.proc _mpd_farpeekw.2

	call _mpd_farptr_add_offset

	lda	<__bl
	tam	#3

	lda	[__si]
	tax
	inc	<__si
	bne	.cont1

	inc	<__si + 1
	bpl	.cont1

	lda	#$60
	sta	<__si + 1

	; inc bank
	
	lda	<__bl
	inc
	tam #3

.cont1:
	lda	[__si]
	rts

	.endp

#endasm

#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
#define mpd_clear_update_flags()	__upd_flags &= ~(UPD_FLAG_DRAW_MASK)
//void	mpd_clear_update_flags()
//{
//	__upd_flags &= ~(UPD_FLAG_DRAW_MASK);
//}

void	mpd_update_screen()
{
#ifdef	MPD_DEBUG
	DBG_BORDER_COLOR_PRE_ROW_COL_FILLING
#endif
	if( __upd_flags & UPD_FLAG_DRAW_MASK )
	{
		// __VDC_CR = vdc_cr <- save the CR value
#asm
		lda <vdc_crl
		sta ___VDC_CR
		lda <vdc_crh
		sta ___VDC_CR + 1
#endasm
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

		// restore CR
		vreg( 5, __VDC_CR );
	}
#ifdef	MPD_DEBUG
	DBG_BORDER_COLOR_RESET
#endif
}

//*** scroll routine vars ***

u16	__CHR_offset;
u8	__CHR_xy_pos;

u16	__last_data_addr;
u8	__data_size;

#if	FLAG_TILES4X4
u8	__block_xy_pos;
u16	__tile4x4_offset;
u8	__tiles_cache[ ScrTilesWidth + 1 ];
#endif

u8	__blocks_cache[ SCR_BLOCKS2x2_WIDTH + 2 ];

u16	__tmp_vaddr;
u16	__tmp_tiles_offset;
u8	__tmp_CHRs_cnt;

#if	FLAG_MODE_MULTIDIR_SCROLL
u8	__tmp_skip_CHRs_cnt;
u16	__tile_n;
#else
u8	__tile_n;
#endif

u8	__tile_ind;
u8	__block_ind;
u16	__tile_step;

u8	__block_CHR_offset;
u8	__tile_block_offset;
u16	__vaddr_mask;

//***************************

void	__mpd_draw_left_tiles_column()
{
#if	FLAG_MODE_BIDIR_SCROLL
	__tmp_tiles_offset	= __horiz_dir_pos;

#if	FLAG_TILES4X4
	__tmp_tiles_offset >>= 5;
#else
	__tmp_tiles_offset >>= 4;
#endif
	// tiles column offset
#if	FLAG_DIR_COLUMNS
	__tmp_tiles_offset = __scr_tiles_height_tbl[ __tmp_tiles_offset ];		// __tmp_tiles_offset *= ScrTilesHeight;
#endif
	__tmp_tiles_offset += ( mpd_curr_scr.scr.scr_ind * __c_scr_tiles_size );

	__tmp_vaddr		= __mpd_get_VRAM_addr( mpd_scroll_x, mpd_scroll_y );
	__tmp_CHRs_cnt		= COLUMN_CHRS_CNT;

	__mpd_fill_column_data();

#else	//FLAG_MODE_MULTIDIR_SCROLL
//	u16	map_pos_x;	mpd_bx
//	u16	map_pos_y;	mpd_cx

	mpd_bx	= mpd_scroll_x;
	mpd_cx	= mpd_scroll_y;

	__tmp_skip_CHRs_cnt	= __mpd_calc_skip_CHRs_cnt( mpd_cl );

#if	FLAG_TILES4X4
	mpd_bx >>= 5;
	mpd_cx >>= 5;
#else
	mpd_bx >>= 4;
	mpd_cx >>= 4;
#endif

#if	FLAG_DIR_ROWS
	__tmp_tiles_offset	= __mpd_get_map_tbl_val( mpd_cx << 1 ) + mpd_bx;
#else
	__tmp_tiles_offset	= __mpd_get_map_tbl_val( mpd_bx << 1 ) + mpd_cx;
#endif
	__tmp_vaddr		= __mpd_get_VRAM_addr( mpd_scroll_x, mpd_scroll_y );
	__tmp_CHRs_cnt		= ( mpd_scroll_y & 0x0f ) ? COLUMN_CHRS_CNT_INC1:COLUMN_CHRS_CNT;

	__mpd_fill_column_data();

#endif	//FLAG_MODE_BIDIR_SCROLL
}

void	__mpd_draw_right_tiles_column()
{
#if	FLAG_MODE_BIDIR_SCROLL
	static mpd_SCREEN 	tmp_scr;

	__mpd_get_screen_data( mpd_get_adj_screen( &mpd_curr_scr, ADJ_SCR_RIGHT ), &tmp_scr );
	__tmp_tiles_offset	= __horiz_dir_pos;

#if	FLAG_TILES4X4
	__tmp_tiles_offset >>= 5;
#else
	__tmp_tiles_offset >>= 4;
#endif
	// tiles column offset
#if	FLAG_DIR_COLUMNS
	__tmp_tiles_offset = __scr_tiles_height_tbl[ __tmp_tiles_offset ];		// __tmp_tiles_offset *= ScrTilesHeight;
#endif
	__tmp_tiles_offset += ( tmp_scr.scr_ind * __c_scr_tiles_size );

	__tmp_vaddr		= __mpd_get_VRAM_addr( mpd_scroll_x + ScrPixelsWidth, mpd_scroll_y );
	__tmp_CHRs_cnt		= COLUMN_CHRS_CNT;

	__mpd_fill_column_data();

#else	//FLAG_MODE_MULTIDIR_SCROLL
//	u16	map_pos_x;	mpd_bx
//	u16	map_pos_y;	mpd_cx

	mpd_bx	= mpd_scroll_x;
	mpd_cx	= mpd_scroll_y;

	__tmp_skip_CHRs_cnt	= __mpd_calc_skip_CHRs_cnt( mpd_cl );

#if	FLAG_TILES4X4
	mpd_bx >>= 5;
	mpd_cx >>= 5;
#else
	mpd_bx >>= 4;
	mpd_cx >>= 4;
#endif

#if	FLAG_DIR_ROWS
	__tmp_tiles_offset	= __mpd_get_map_tbl_val( mpd_cx << 1 ) + mpd_bx + ScrTilesWidth;
#else	
	__tmp_tiles_offset	= __mpd_get_map_tbl_val( ( mpd_bx + ScrTilesWidth ) << 1 ) + mpd_cx;
#endif
	__tmp_vaddr		= __mpd_get_VRAM_addr( mpd_scroll_x + ScrPixelsWidth, mpd_scroll_y );
	__tmp_CHRs_cnt		= ( mpd_scroll_y & 0x0f ) ? COLUMN_CHRS_CNT_INC1:COLUMN_CHRS_CNT;

	__mpd_fill_column_data();

#endif	//FLAG_MODE_BIDIR_SCROLL
}

void	__mpd_draw_up_tiles_row()
{
#if	FLAG_MODE_BIDIR_SCROLL
	__tmp_tiles_offset	= __vert_dir_pos;

#if	FLAG_TILES4X4
	__tmp_tiles_offset >>= 5;
#else
	__tmp_tiles_offset >>= 4;
#endif
	// tiles column offset
#if	FLAG_DIR_ROWS
	__tmp_tiles_offset = __scr_tiles_width_tbl[ __tmp_tiles_offset ];		// __tmp_tiles_offset *= ScrTilesWidth;
#endif
	__tmp_tiles_offset += ( mpd_curr_scr.scr.scr_ind * __c_scr_tiles_size );

	__tmp_vaddr		= __mpd_get_VRAM_addr( mpd_scroll_x, mpd_scroll_y );
	__tmp_CHRs_cnt		= ROW_CHRS_CNT;

	__mpd_fill_row_data();

#else	//FLAG_MODE_MULTIDIR_SCROLL
//	u16	map_pos_x;	mpd_bx
//	u16	map_pos_y;	mpd_cx

	mpd_bx	= mpd_scroll_x;
	mpd_cx	= mpd_scroll_y;

	__tmp_skip_CHRs_cnt	= __mpd_calc_skip_CHRs_cnt( mpd_bl );

#if	FLAG_TILES4X4
	mpd_bx >>= 5;
	mpd_cx >>= 5;
#else
	mpd_bx >>= 4;
	mpd_cx >>= 4;
#endif

#if	FLAG_DIR_ROWS
	__tmp_tiles_offset	= __mpd_get_map_tbl_val( mpd_cx << 1 ) + mpd_bx;
#else
	__tmp_tiles_offset	= __mpd_get_map_tbl_val( mpd_bx << 1 ) + mpd_cx;
#endif
	__tmp_vaddr		= __mpd_get_VRAM_addr( mpd_scroll_x, mpd_scroll_y );
	__tmp_CHRs_cnt		= ROW_CHRS_CNT;

	__mpd_fill_row_data();

#endif	//FLAG_MODE_BIDIR_SCROLL
}

void	__mpd_draw_down_tiles_row()
{
#if	FLAG_MODE_BIDIR_SCROLL
	static mpd_SCREEN 	tmp_scr;

	__mpd_get_screen_data( mpd_get_adj_screen( &mpd_curr_scr, ADJ_SCR_DOWN ), &tmp_scr );
	__tmp_tiles_offset	= __vert_dir_pos;

#if	FLAG_TILES4X4
	__tmp_tiles_offset >>= 5;
#else
	__tmp_tiles_offset >>= 4;
#endif
	// tiles column offset
#if	FLAG_DIR_ROWS
	__tmp_tiles_offset = __scr_tiles_width_tbl[ __tmp_tiles_offset ];		// __tmp_tiles_offset *= ScrTilesWidth;
#endif
	__tmp_tiles_offset += ( tmp_scr.scr_ind * __c_scr_tiles_size );

	__tmp_vaddr		= __mpd_get_VRAM_addr( mpd_scroll_x, mpd_scroll_y + ScrPixelsHeight );
	__tmp_CHRs_cnt		= ROW_CHRS_CNT;

	__mpd_fill_row_data();

#else	//FLAG_MODE_MULTIDIR_SCROLL
//	u16	map_pos_x;	mpd_bx
//	u16	map_pos_y;	mpd_cx

	mpd_bx	= mpd_scroll_x;
	mpd_cx	= mpd_scroll_y;

	__tmp_skip_CHRs_cnt	= __mpd_calc_skip_CHRs_cnt( mpd_bl );

#if	FLAG_TILES4X4
	mpd_bx >>= 5;
	mpd_cx >>= 5;
#else
	mpd_bx >>= 4;
	mpd_cx >>= 4;
#endif

#if	FLAG_DIR_ROWS
	__tmp_tiles_offset	= __mpd_get_map_tbl_val( mpd_cx << 1 ) + mpd_bx + __height_scr_step;
#else
	__tmp_tiles_offset	= __mpd_get_map_tbl_val( mpd_bx << 1 ) + mpd_cx + ScrTilesHeight;
#endif
	__tmp_vaddr		= __mpd_get_VRAM_addr( mpd_scroll_x, mpd_scroll_y + ScrPixelsHeight );
	__tmp_CHRs_cnt		= ROW_CHRS_CNT;

	__mpd_fill_row_data();

#endif	//FLAG_MODE_BIDIR_SCROLL
}

void	__mpd_fill_column_data()
{
#if	FLAG_TILES4X4
	__block_xy_pos	= ( mpd_scroll_x >> 4 ) & 0x01;
#endif
	__data_size	= __tmp_CHRs_cnt;

	__CHR_xy_pos	= ( ( mpd_scroll_x >> 3 ) & 0x01 ) << 1;

	// loop the _vaddr vertically
	__last_data_addr	= __tmp_vaddr + ( __data_size << __BAT_width_pow2 );

	if( __last_data_addr > __BAT_size )
	{
		__data_size -= ( ( __last_data_addr - __BAT_size ) & ~__BAT_width_dec1 ) >> __BAT_width_pow2;
	}

#if	FLAG_DIR_COLUMNS
	__tile_step = 1;
#else	//FLAG_DIR_ROWS
#if	FLAG_MODE_BIDIR_SCROLL
	__tile_step = ScrTilesWidth;
#else
	__tile_step = mpd_map_tiles_width;
#endif	//FLAG_MODE_BIDIR_SCROLL
#endif

#asm
	; set data step

	mpd_vreg #$05

	; video_data = ( ___VDC_CR & ~___CR_IW_MASK ) | ___VDC_CR_IW

	lda ___VDC_CR
	sta video_data_l
	lda ___VDC_CR + 1
	and #~$18
	ora ___VDC_CR_IW + 1
	sta video_data_h

	; set write mode

	mpd_vreg #$00

	lda ___tmp_vaddr
	sta video_data_l
	lda ___tmp_vaddr + 1
	sta video_data_h

	mpd_vreg #$02
#endasm

	__block_CHR_offset	= 4;
	__tile_block_offset	= 2;
	__vaddr_mask		= __BAT_width_dec1;

	__mpd_fill_row_column_data();
}

void	__mpd_fill_row_data()
{
#if	FLAG_TILES4X4
	__block_xy_pos	= ( ( mpd_scroll_y >> 4 ) & 0x01 ) << 1;
#endif
	__data_size	= __tmp_CHRs_cnt;

#if	FLAG_MODE_MULTIDIR_SCROLL + ( FLAG_MODE_BIDIR_SCROLL * ( SCR_BLOCKS2x2_WIDTH != 16 ) )
	{
		// loop the _vaddr horizontally
		__last_data_addr	= __tmp_vaddr + __tmp_CHRs_cnt;

		if( ( __last_data_addr & __BAT_width_dec1_inv ) != ( __tmp_vaddr & __BAT_width_dec1_inv ) )
		{
			__data_size -= __last_data_addr & __BAT_width_dec1;
		}
	}
#endif

#if	FLAG_DIR_COLUMNS
#if	FLAG_MODE_BIDIR_SCROLL
	__tile_step = ScrTilesHeight;
#else
	__tile_step = mpd_map_tiles_height;
#endif	//FLAG_MODE_BIDIR_SCROLL
#else	//FLAG_DIR_ROWS
	__tile_step = 1;
#endif

#asm
	; set data step

	mpd_vreg #$05

	; video_data = ___VDC_CR & ~___CR_IW_MASK

	lda ___VDC_CR
	sta video_data_l
	lda ___VDC_CR + 1
	and #~$18
	sta video_data_h

	; set write mode

	mpd_vreg #$00

	lda ___tmp_vaddr
	sta video_data_l
	lda ___tmp_vaddr + 1
	sta video_data_h

	mpd_vreg #$02
#endasm

	__CHR_xy_pos	= ( ( mpd_scroll_y >> 3 ) & 0x01 ) << 2;

	__block_CHR_offset	= 2;
	__tile_block_offset	= 1;
	__vaddr_mask		= __BAT_width_dec1_inv;

	__mpd_fill_row_column_data();
}

void	__mpd_fill_row_column_data()
{
#ifdef	MPD_DEBUG
	DBG_BORDER_COLOR_ROW_COL_FILLING
#endif

#if	FLAG_TILES4X4

#if	FLAG_MODE_BIDIR_SCROLL
#asm
	__farptr _mpd_TilesScr, __bl, __si

	; switch data banks

	stw ___tmp_tiles_offset, <__ax

	call _mpd_farptr_add_offset
	jsr map_data
#endasm
#else	//!FLAG_MODE_BIDIR_SCROLL
#ifdef	MPD_RAM_MAP
#asm
	mpd_add_word_to_word2 #___RAM_Map, ___tmp_tiles_offset, <__si
#endasm
#else	//!MPD_RAM_MAP
	mpd_farptr( __map_ptr.bank, __map_ptr.addr );
#asm
	; switch data banks

	stw ___tmp_tiles_offset, <__ax

	call _mpd_farptr_add_offset
	jsr map_data
#endasm
#endif	//MPD_RAM_MAP
#endif	//FLAG_MODE_BIDIR_SCROLL

	// tiles cache filling
#asm
	; X = ( ( __tmp_CHRs_cnt + 3 ) >> 2 ) + 1	[HuC: ( __tmp_CHRs_cnt + 3 ) >> 2 ]

	lda ___tmp_CHRs_cnt
	inc a
	inc a
	inc a
	lsr a
	lsr a
	inc a
	tax

	stz ___tile_n
	stz ___tile_n + 1

	stw #___tiles_cache, <__cx
	cly

.tiles_cache_loop:

	; __tiles_cache[ __tile_ind ] = lda [__si], __tile_n;

	mpd_add_word_to_word2 <__si, ___tile_n, <__ax
	lda [<__ax]

	sta [<__cx], y
	iny

	mpd_add_word_to_word ___tile_step, ___tile_n

	dex
	bne .tiles_cache_loop
#endasm

#ifndef	MPD_RAM_MAP
#asm
	jsr unmap_data
#endasm
#endif
/*
	__tile_n = 0;

	// tiles cache filling
	for( __tile_ind = 0; __tile_ind < ( ( __tmp_CHRs_cnt + 3 ) >> 2 ); )
	{
#if	FLAG_MODE_BIDIR_SCROLL
		__tiles_cache[ __tile_ind++ ] = mpd_farpeekb( mpd_TilesScr, __tmp_tiles_offset + __tile_n );
#else
		__tiles_cache[ __tile_ind++ ] = mpd_farpeekb( __maps_ptr, __tmp_tiles_offset + __tile_n );
#endif	//FLAG_MODE_BIDIR_SCROLL

		__tile_n += __tile_step;
	}
*/
	// blocks cache filling
#asm
	; switch data banks

	__farptr _mpd_Tiles, __bl, __si

	stw ___tiles_offset, <__ax

	call _mpd_farptr_add_offset
	jsr map_data

	; x = ( ( __tmp_CHRs_cnt + 1 ) >> 2 ) + 1	[HuC: ( __tmp_CHRs_cnt + 1 ) >> 1 ]

	lda ___tmp_CHRs_cnt
	inc a
	lsr a
	lsr a
	inc a
	tax

	stz ___tile_ind
	stz ___block_ind

	stw #___blocks_cache, <__cx
	stw #___tiles_cache, <__dx

.blocks_cache_loop:

	; A = __tiles_cache[ __tile_ind++ ]

	ldy ___tile_ind
	lda [<__dx], y
	cly

	inc ___tile_ind

	; __ax = ( A/Y << 2 ) + __block_xy_pos

	asl a
	say
	rol a
	say
	asl a
	say
	rol a
	say

	clc
	adc ___block_xy_pos
	sta <__al
	say
	adc #00
	sta <__ah

	; A = mpd_Tiles[ __ax ]

	mpd_add_word_to_word <__si, <__ax
	lda [<__ax]

	; __blocks_cache[ __block_ind++ ] = A

	ldy ___block_ind
	sta [<__cx], y

	inc ___block_ind

	; A = mpd_Tiles[ __ax + 2 ]

	lda ___tile_block_offset
	mpd_add_a_to_word <__ax

	lda [<__ax]

	; __blocks_cache[ __block_ind++ ] = A

	iny
	sta [<__cx], y

	inc ___block_ind

	dex
	bne .blocks_cache_loop

	jsr unmap_data
#endasm
/*
	__tile_ind = 0;

	// fill blocks cache
	for( __block_ind = 0; __block_ind < ( ( __tmp_CHRs_cnt + 1 ) >> 1 ); )
	{
		__tile4x4_offset = ( __tiles_cache[ __tile_ind++ ] << 2 ) + __block_xy_pos;

//#if	MAPS_CNT != 1
		__tile4x4_offset += __tiles_offset;
//#endif
		__blocks_cache[ __block_ind++ ] = mpd_farpeekb( mpd_Tiles, __tile4x4_offset );
		__blocks_cache[ __block_ind++ ] = mpd_farpeekb( mpd_Tiles, __tile4x4_offset + 2/1 );
	}
*/
	// put CHRs into BAT
#asm
	; switch data banks

	__farptr _mpd_Attrs, __bl, __si

	stw ___blocks_offset, <__ax

	call _mpd_farptr_add_offset
	jsr map_data

	stz ___block_ind
	ldx ___data_size

	lda ___tmp_CHRs_cnt
	sta <__dl
#endasm

#if	FLAG_MODE_MULTIDIR_SCROLL
#asm
	lda ___tmp_skip_CHRs_cnt
	sta <__dh
#endasm
#endif

//	__block_ind = 0;

#asm
CHRs_proc_loop:
#endasm
//	while( TRUE )
//	{
		// block 1
//		__CHR_offset = ( __blocks_cache[ __block_ind++ ] << 3 ) + __CHR_xy_pos;
#asm
		stw #___blocks_cache, <__ax
		ldy ___block_ind
		lda [<__ax], y
		cly

		inc ___block_ind

		; __cx(__CHR_offset) = ( A/Y << 3 ) + __CHR_xy_pos

		asl a
		say
		rol a
		say
		asl a
		say
		rol a
		say
		asl a
		say
		rol a
		say

		clc
		adc ___CHR_xy_pos
		sta <__cl		;___CHR_offset
		say
		adc #00
		sta <__ch		;___CHR_offset + 1
#endasm
//#if	MAPS_CNT != 1
//		__CHR_offset += __blocks_offset;
//#endif
#if	FLAG_MODE_MULTIDIR_SCROLL
//		if( !__tmp_skip_CHRs_cnt )
//		{
#asm
		lda <__dh	;___tmp_skip_CHRs_cnt
		beq .CHRs_proc_put_CHR1

		dec <__dh	;___tmp_skip_CHRs_cnt
		bra .CHRs_proc_cont11
.CHRs_proc_put_CHR1:
#endasm
#endif
//			mpd_farpeekw( mpd_Attrs, __CHR_offset );
#asm
			mpd_add_word_to_word2 <__si, <__cx, <__ax	;__cx = ___CHR_offset
			lda [<__ax]
			sta video_data_l

			ldy #1
			lda [<__ax], y
			sta video_data_h
#endasm
//			if( !--__tmp_CHRs_cnt )	{ break; }
//			if( !--__data_size )
#asm
			dec <__dl	;___tmp_CHRs_cnt
			bne .CHRs_proc_cont01
			jmp CHRs_proc_exit
.CHRs_proc_cont01:
			dex		;___data_size
			bne .CHRs_proc_cont11
#endasm
			{
				// set write mode
#asm
				mpd_vreg #$00

				lda ___tmp_vaddr
				and ___vaddr_mask
				sta video_data_l

				lda ___tmp_vaddr + 1
				and ___vaddr_mask + 1
				sta video_data_h

				mpd_vreg #$02
.CHRs_proc_cont11:
#endasm
			}

#if	FLAG_MODE_MULTIDIR_SCROLL
//		}
//		else
//		{ --__tmp_skip_CHRs_cnt; }

//		if( !__tmp_skip_CHRs_cnt )
//		{
#asm
		lda <__dh	;___tmp_skip_CHRs_cnt
		beq .CHRs_proc_put_CHR2
		
		dec <__dh	;___tmp_skip_CHRs_cnt
		bra .CHRs_proc_cont22
.CHRs_proc_put_CHR2:
#endasm
#endif
//			mpd_farpeekw( mpd_Attrs, __CHR_offset + 4/2 );
#asm
			mpd_add_word_to_word2 <__si, <__cx, <__ax	;__cx = ___CHR_offset
			lda ___block_CHR_offset
			mpd_add_a_to_word <__ax 

			lda [<__ax]
			sta video_data_l

			ldy #1
			lda [<__ax], y
			sta video_data_h
#endasm
//			if( !--__tmp_CHRs_cnt )	{ break; }
//			if( !--__data_size )
#asm
			dec <__dl	;___tmp_CHRs_cnt
			bne .CHRs_proc_cont02
			jmp CHRs_proc_exit
.CHRs_proc_cont02:
			dex		;___data_size
			bne .CHRs_proc_cont22
#endasm
			{
				// set write mode
#asm
				mpd_vreg #$00

				lda ___tmp_vaddr
				and ___vaddr_mask
				sta video_data_l

				lda ___tmp_vaddr + 1
				and ___vaddr_mask + 1
				sta video_data_h

				mpd_vreg #$02
.CHRs_proc_cont22:
#endasm
			}
//#if	FLAG_MODE_MULTIDIR_SCROLL
//		}
//		else
//		{ --__tmp_skip_CHRs_cnt; }
//#endif
		// block 2
//		__CHR_offset = ( __blocks_cache[ __block_ind++ ] << 3 ) + __CHR_xy_pos;
#asm
		stw #___blocks_cache, <__ax
		ldy ___block_ind
		lda [<__ax], y
		cly

		inc ___block_ind

		; __cx(__CHR_offset) = ( A/Y << 3 ) + __CHR_xy_pos

		asl a
		say
		rol a
		say
		asl a
		say
		rol a
		say
		asl a
		say
		rol a
		say

		clc
		adc ___CHR_xy_pos
		sta <__cl		;___CHR_offset
		say
		adc #00
		sta <__ch		;___CHR_offset + 1
#endasm
//#if	MAPS_CNT != 1
//		__CHR_offset += __blocks_offset;
//#endif
#if	FLAG_MODE_MULTIDIR_SCROLL
//		if( !__tmp_skip_CHRs_cnt )
//		{
#asm
		lda <__dh	;___tmp_skip_CHRs_cnt
		beq .CHRs_proc_put_CHR3
		
		dec <__dh	;___tmp_skip_CHRs_cnt
		bra .CHRs_proc_cont33
.CHRs_proc_put_CHR3:
#endasm
#endif
//			mpd_farpeekw( mpd_Attrs, __CHR_offset );
#asm
			mpd_add_word_to_word2 <__si, <__cx, <__ax	;__cx = ___CHR_offset
			lda [<__ax]
			sta video_data_l

			ldy #1
			lda [<__ax], y
			sta video_data_h
#endasm
//			if( !--__tmp_CHRs_cnt )	{ break; }
//			if( !--__data_size )
#asm
			dec <__dl	;___tmp_CHRs_cnt
			bne .CHRs_proc_cont03
			jmp CHRs_proc_exit
.CHRs_proc_cont03:
			dex		;___data_size
			bne .CHRs_proc_cont33
#endasm
			{
				// set write mode
#asm
				mpd_vreg #$00

				lda ___tmp_vaddr
				and ___vaddr_mask
				sta video_data_l

				lda ___tmp_vaddr + 1
				and ___vaddr_mask + 1
				sta video_data_h

				mpd_vreg #$02
.CHRs_proc_cont33
#endasm
			}

#if	FLAG_MODE_MULTIDIR_SCROLL
//		}
//		else
//		{ --__tmp_skip_CHRs_cnt; }

//		if( !__tmp_skip_CHRs_cnt )
//		{
#asm
		lda <__dh	;___tmp_skip_CHRs_cnt
		beq .CHRs_proc_put_CHR4
		
		dec <__dh	;___tmp_skip_CHRs_cnt
		bra .CHRs_proc_cont44
.CHRs_proc_put_CHR4:
#endasm
#endif
//			mpd_farpeekw( mpd_Attrs, __CHR_offset + 4/2 );
#asm
			mpd_add_word_to_word2 <__si, <__cx, <__ax	;__cx = ___CHR_offset
			lda ___block_CHR_offset
			mpd_add_a_to_word <__ax 

			lda [<__ax]
			sta video_data_l

			ldy #1
			lda [<__ax], y
			sta video_data_h
#endasm
//			if( !--__tmp_CHRs_cnt )	{ break; }
//			if( !--__data_size )
#asm
			dec <__dl	;___tmp_CHRs_cnt
			bne .CHRs_proc_cont04
			bra CHRs_proc_exit
.CHRs_proc_cont04:
			dex		;___data_size
			bne .CHRs_proc_cont44
#endasm
			{
				// set write mode
#asm
				mpd_vreg #$00

				lda ___tmp_vaddr
				and ___vaddr_mask
				sta video_data_l

				lda ___tmp_vaddr + 1
				and ___vaddr_mask + 1
				sta video_data_h

				mpd_vreg #$02
.CHRs_proc_cont44
#endasm
			}

//#if	FLAG_MODE_MULTIDIR_SCROLL
//		}
//		else
//		{ --__tmp_skip_CHRs_cnt; }
//#endif
//	}

#asm
	jmp CHRs_proc_loop

CHRs_proc_exit:

	jsr unmap_data
#endasm
#else	//FLAG_TILES2X2

#if	FLAG_MODE_BIDIR_SCROLL
#asm
	__farptr _mpd_TilesScr, __bl, __si

	; switch data banks

	stw ___tmp_tiles_offset, <__ax

	call _mpd_farptr_add_offset
	jsr map_data
#endasm
#else	//!FLAG_MODE_BIDIR_SCROLL
#ifdef	MPD_RAM_MAP
#asm
	mpd_add_word_to_word2 #___RAM_Map, ___tmp_tiles_offset, <__si
#endasm
#else	//!MPD_RAM_MAP
	mpd_farptr( __map_ptr.bank, __map_ptr.addr );
#asm
	; switch data banks

	stw ___tmp_tiles_offset, <__ax

	call _mpd_farptr_add_offset
	jsr map_data
#endasm
#endif	//MPD_RAM_MAP
#endif	//FLAG_MODE_BIDIR_SCROLL

#asm
	; X = ( __tmp_CHRs_cnt + 1 ) >> 1

	lda ___tmp_CHRs_cnt
	inc a
	lsr a
	tax

	stz ___tile_n
	stz ___tile_n + 1

	stw #___blocks_cache, <__cx
	cly

.blocks_cache_loop:

	; __blocks_cache[ __block_ind ] = lda [__si], __tile_n;

	mpd_add_word_to_word2 <__si, ___tile_n, <__ax
	lda [<__ax]

	sta [<__cx], y
	iny

	mpd_add_word_to_word ___tile_step, ___tile_n

	dex
	bne .blocks_cache_loop
#endasm

#ifndef	MPD_RAM_MAP
#asm
	jsr unmap_data
#endasm
#endif
/*
	// blocks cache filling
	__tile_n = 0;

	for( __block_ind = 0; __block_ind < ( ( __tmp_CHRs_cnt + 1 ) >> 1 ); )
	{
#if	FLAG_MODE_BIDIR_SCROLL
		__blocks_cache[ __block_ind++ ] = mpd_farpeekb( mpd_TilesScr, __tmp_tiles_offset + __tile_n );
#else
		__blocks_cache[ __block_ind++ ] = mpd_farpeekb( __maps_ptr, __tmp_tiles_offset + __tile_n );
#endif	//FLAG_MODE_BIDIR_SCROLL

		__tile_n += __tile_step;
	}
*/
	// put CHRs into BAT
#asm
	; switch data banks

	__farptr _mpd_Attrs, __bl, __si

	stw ___blocks_offset, <__ax

	call _mpd_farptr_add_offset
	jsr map_data

	stz ___block_ind
	ldx ___data_size

	lda ___tmp_CHRs_cnt
	sta <__dl
#endasm

#if	FLAG_MODE_MULTIDIR_SCROLL
#asm
	lda ___tmp_skip_CHRs_cnt
	sta <__dh
#endasm
#endif

//	__block_ind = 0;

#asm
CHRs_proc_loop:
#endasm
//	while( TRUE )
//	{
		// block 1
//		__CHR_offset = ( __blocks_cache[ __block_ind++ ] << 3 ) + __CHR_xy_pos;
#asm
		stw #___blocks_cache, <__ax
		ldy ___block_ind
		lda [<__ax], y
		cly

		inc ___block_ind

		; __cx(__CHR_offset) = ( A/Y << 3 ) + __CHR_xy_pos

		asl a
		say
		rol a
		say
		asl a
		say
		rol a
		say
		asl a
		say
		rol a
		say

		clc
		adc ___CHR_xy_pos
		sta <__cl		;___CHR_offset
		say
		adc #00
		sta <__ch		;___CHR_offset + 1
#endasm
//#if	MAPS_CNT != 1
//		__CHR_offset += __blocks_offset;
//#endif
#if	FLAG_MODE_MULTIDIR_SCROLL
//		if( !__tmp_skip_CHRs_cnt )
//		{
#asm
		lda <__dh	;___tmp_skip_CHRs_cnt
		beq .CHRs_proc_put_CHR1
		
		dec <__dh	;___tmp_skip_CHRs_cnt
		bra .CHRs_proc_cont11
.CHRs_proc_put_CHR1:
#endasm
#endif
//			mpd_farpeekw( mpd_Attrs, __CHR_offset );
#asm
			mpd_add_word_to_word2 <__si, <__cx, <__ax	;__cx = ___CHR_offset

			lda [<__ax]
			sta video_data_l

			ldy #1
			lda [<__ax], y
			sta video_data_h
#endasm

//			if( !--__tmp_CHRs_cnt )	{ break; }
//			if( !--__data_size )
#asm
			dec <__dl	;___tmp_CHRs_cnt
			bne .CHRs_proc_cont01
			bra CHRs_proc_exit
.CHRs_proc_cont01:
			dex		;___data_size
			bne .CHRs_proc_cont11
#endasm
			{
				// set write mode
#asm
				mpd_vreg #$00

				lda ___tmp_vaddr
				and ___vaddr_mask
				sta video_data_l

				lda ___tmp_vaddr + 1
				and ___vaddr_mask + 1
				sta video_data_h

				mpd_vreg #$02
.CHRs_proc_cont11:
#endasm
			}

#if	FLAG_MODE_MULTIDIR_SCROLL
//		}
//		else
//		{ --__tmp_skip_CHRs_cnt; }
#endif
//		mpd_farpeekw( mpd_Attrs, __CHR_offset + 4/2 );
#asm
		mpd_add_word_to_word2 <__si, <__cx, <__ax	;__cx = ___CHR_offset
		lda ___block_CHR_offset
		mpd_add_a_to_word <__ax 

		lda [<__ax]
		sta video_data_l

		ldy #1
		lda [<__ax], y
		sta video_data_h
#endasm

//		if( !--__tmp_CHRs_cnt )	{ break; }
//		if( !--__data_size )
#asm
		dec <__dl	;___tmp_CHRs_cnt
		bne .CHRs_proc_cont02
		bra CHRs_proc_exit
.CHRs_proc_cont02:
		dex		;___data_size
		bne .CHRs_proc_cont22
#endasm
		{
			// set write mode
#asm
			mpd_vreg #$00

			lda ___tmp_vaddr
			and ___vaddr_mask
			sta video_data_l

			lda ___tmp_vaddr + 1
			and ___vaddr_mask + 1
			sta video_data_h

			mpd_vreg #$02
.CHRs_proc_cont22:
#endasm
		}
//	}
#asm
	jmp CHRs_proc_loop

CHRs_proc_exit:

	jsr unmap_data
#endasm
#endif	//FLAG_TILES4X4
}
#endif	//FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL

#asm

;//u16	__mpd_get_VRAM_addr( u16 _x<__ax>, u16 _y<acc> )

; HuC	return ( ( ( _x >> 3 ) & __BAT_width_dec1 ) + ( ( ( _y >> 3 ) & __BAT_height_dec1 ) << __BAT_width_pow2 ) ) & __BAT_size_dec1;

	.proc ___mpd_get_VRAM_addr.2

	; _y

	__lsrw
	__lsrw
	__lsrw

	sax
	and ___BAT_height_dec1

	clx
	sax

	ldy ___BAT_width_pow2
.loop:
	__aslw

	dey
	bne .loop

	sta <__bh
	stx <__bl

	; _x

	lda <__ah
	ldx <__al

	__lsrw
	__lsrw
	__lsrw

	sax
	and ___BAT_width_dec1

	clc
	adc <__bl
	sax

	cla

	adc <__bh

	and ___BAT_size_dec1 + 1

	sax
	and ___BAT_size_dec1

	sax

	rts

	.endp
#endasm

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

//	u16	tiles_offset;	mpd_cx
//	u8	tile_id;	mpd_dx

	// _x = __cx
	// _y = __dx

//	u8	block_pos_x;	mpd_al
//	u8	block_pos_y;	mpd_ah
//	u8	CHR_pos_x;	mpd_bl
//	u8	CHR_pos_y;	mpd_bh

#if	FLAG_MODE_BIDIR_SCROLL		/* !!! */

	static mpd_SCREEN	tmp_scr;
//	u16		scr_offs;	mpd_ex

//	s16		dx;		mpd_axs
//	s16		dy;		mpd_bxs
#endif

#ifdef	MPD_DEBUG
	DBG_BORDER_COLOR_GET_TILE_PROP
#endif

#if	FLAG_MODE_BIDIR_SCROLL		/* !!! */

	mpd_ex	= __scr_offset;

	if( !__horiz_dir_pos && __vert_dir_pos )
	{
		mpd_bxs = _y - ( ScrPixelsHeight - __vert_dir_pos );

		if( mpd_bxs >= 0 )
		{
			__mpd_get_screen_data( mpd_get_adj_screen( &mpd_curr_scr, ADJ_SCR_DOWN ), &tmp_scr );
			mpd_ex	= tmp_scr.scr_ind * __c_scr_tiles_size;
			_y	= mpd_bxs;
		}
		else
		{
			_y += __vert_dir_pos;
		}
	}
	else
	if( !__vert_dir_pos && __horiz_dir_pos )
	{
		mpd_axs = _x - ( ScrPixelsWidth - __horiz_dir_pos );

		if( mpd_axs >= 0 )
		{
			__mpd_get_screen_data( mpd_get_adj_screen( &mpd_curr_scr, ADJ_SCR_RIGHT ), &tmp_scr );
			mpd_ex	= tmp_scr.scr_ind * __c_scr_tiles_size;
			_x	= mpd_axs;
		}
		else
		{
			_x += __horiz_dir_pos;
		}
	}
#endif	//FLAG_MODE_BIDIR_SCROLL

	// _x = __cx
	// _y = __dx

#if	FLAG_TILES4X4
#if	FLAG_PROP_ID_PER_BLOCK
//	_x >>= 4;
//	block_pos_x = _x & 0x01;
//	_x >>= 1;

#asm
	__ldw_s 2	; _x

	mpd_clamp_neg_xa

	__lsrw
	__lsrw
	__lsrw
	__lsrw

	tay
	txa
	and #1
	sta <_mpd_al	; block_pos_x
	tya
	
	__lsrw

	stx <__cl
	sta <__ch	; _x
#endasm

//	_y >>= 4;
//	block_pos_y = _y & 0x01;
//	_y >>= 1;

#asm
	__ldw_s 0	; _y

	mpd_clamp_neg_xa

	__lsrw
	__lsrw
	__lsrw
	__lsrw
	
	tay
	txa
	and #1
	sta <_mpd_ah	; block_pos_y
	tya
	
	__lsrw

	stx <__dl
	sta <__dh	; _y
#endasm

#else	//FLAG_PROP_ID_PER_CHR
//	_x >>= 3;
//	CHR_pos_x = _x & 0x01;
//	_x >>= 1;
//	block_pos_x = _x & 0x01;
//	_x >>= 1;

#asm
	__ldw_s 2	; _x

	mpd_clamp_neg_xa

	__lsrw
	__lsrw
	__lsrw

	tay
	txa
	and #1
	sta <_mpd_bl	; CHR_pos_x
	tya

	__lsrw

	tay
	txa
	and #1
	sta <_mpd_al	; block_pos_x
	tya
	
	__lsrw

	stx <__cl
	sta <__ch	; _x
#endasm

//	_y >>= 3;
//	CHR_pos_y = _y & 0x01;
//	_y >>= 1;
//	block_pos_y = _y & 0x01;
//	_y >>= 1;

#asm
	__ldw_s 0	; _y

	mpd_clamp_neg_xa

	__lsrw
	__lsrw
	__lsrw

	tay
	txa
	and #1
	sta <_mpd_bh	; CHR_pos_y
	tya

	__lsrw
	
	tay
	txa
	and #1
	sta <_mpd_ah	; block_pos_y
	tya
	
	__lsrw

	stx <__dl
	sta <__dh	; _y
#endasm
#endif	//FLAG_PROP_ID_PER_BLOCK
#else	//FLAG_TILES2X2
#if	FLAG_PROP_ID_PER_BLOCK
//	_x >>= 4;

#asm
	__ldw_s 2	; _x

	mpd_clamp_neg_xa

	__lsrw
	__lsrw
	__lsrw
	__lsrw

	stx <__cl
	sta <__ch	; _x
#endasm

//	_y >>= 4;

#asm
	__ldw_s 0	; _y

	mpd_clamp_neg_xa

	__lsrw
	__lsrw
	__lsrw
	__lsrw

	stx <__dl
	sta <__dh	; _y
#endasm
#else	//FLAG_PROP_ID_PER_CHR
//	_x >>= 3;
//	CHR_pos_x = _x & 0x01;
//	_x >>= 1;

#asm
	__ldw_s 2	; _x

	mpd_clamp_neg_xa

	__lsrw
	__lsrw
	__lsrw

	tay
	txa
	and #1
	sta <_mpd_bl	; CHR_pos_x
	tya

	__lsrw

	stx <__cl
	sta <__ch	; _x
#endasm

//	_y >>= 3;
//	CHR_pos_y = _y & 0x01;
//	_y >>= 1;

#asm
	__ldw_s 0	; _y

	mpd_clamp_neg_xa

	__lsrw
	__lsrw
	__lsrw

	tay
	txa
	and #1
	sta <_mpd_bh	; CHR_pos_y
	tya

	__lsrw

	stx <__dl
	sta <__dh	; _y
#endasm
#endif	//FLAG_PROP_ID_PER_BLOCK
#endif	//FLAG_TILES4X4

#if	FLAG_MODE_MULTIDIR_SCROLL	/* !!! */

#if	FLAG_DIR_ROWS
//	tiles_offset	= mpd_farpeekw( mpd_MapsTbl, __map_tbl_offset + ( _y << 1 ) ) + _x;
#asm
	; __ax = y << 1

	lda <__dl
	asl a
	sta <__al
	lda <__dh
	rol a
	sta <__ah
#endasm

#ifdef	MPD_RAM_MAP_TBL
#asm
	___mpd_get_map_tbl_val.1
#endasm
#else	//!MPD_RAM_MAP_TBL
#asm
	__farptr _mpd_MapsTbl, __bl, __si

	mpd_add_word_to_word ___map_tbl_offset, <__ax	; __map_tbl_offset + ( _y << 1 )

	call _mpd_farpeekw.2
#endasm
#endif	//MPD_RAM_MAP_TBL

#asm
	sax

	clc
	adc <__cl		; _x.l
	sta <_mpd_cl		; tiles_offset.l

	sax

	adc <__ch		; _x.h
	sta <_mpd_ch		; tiles_offset.h
#endasm
#else	//FLAG_DIR_COLUMNS
//	tiles_offset	= mpd_farpeekw( mpd_MapsTbl, __map_tbl_offset + ( _x << 1 ) ) + _y;
#asm
	; __ax = _x << 1

	lda <__cl
	asl a
	sta <__al
	lda <__ch
	rol a
	sta <__ah
#endasm

#ifdef	MPD_RAM_MAP_TBL
#asm
	___mpd_get_map_tbl_val.1
#endasm
#else	//!MPD_RAM_MAP_TBL
#asm
	__farptr _mpd_MapsTbl, __bl, __si

	mpd_add_word_to_word ___map_tbl_offset, <__ax	; __map_tbl_offset + ( _x << 1 )

	call _mpd_farpeekw.2
#endasm
#endif	//MPD_RAM_MAP_TBL

#asm
	sax

	clc
	adc <__dl		; _y.l
	sta <_mpd_cl		; tiles_offset.l

	sax

	adc <__dh		; _y.h
	sta <_mpd_ch		; tiles_offset.h
#endasm
#endif	//FLAG_DIR_ROWS
//	tile_id		= mpd_farpeekb( __maps_ptr, tiles_offset );
#ifdef	MPD_RAM_MAP
#asm
	mpd_add_word_to_word2 #___RAM_Map, <_mpd_cx, <__si
#endasm
#else	//!MPD_RAM_MAP
	mpd_farptr( __map_ptr.bank, __map_ptr.addr );
#asm
	stw <_mpd_cx, <__ax	; tiles_offset

	call _mpd_farptr_add_offset

	lda	<__bl
	tam	#3
#endasm
#endif	//MPD_RAM_MAP

#asm
	lda [<__si]
	tax			; x = tile_id
#endasm

#elif	FLAG_MODE_BIDIR_SCROLL + FLAG_MODE_BIDIR_STAT_SCR + FLAG_MODE_STAT_SCR	/* !!! */

#if	FLAG_DIR_ROWS
//	tiles_offset	= __scr_tiles_width_tbl[ _y ] + _x;
#asm
	lda <__dl
	asl a
	sta <__al
	lda <__dh
	rol a
	sta <__ah		; __ax = _y << 1

	mpd_add_word_to_word #___scr_tiles_width_tbl, <__ax

	lda [<__ax]

	clc
	adc <__cl
	sta <_mpd_cl

	ldy #1
	lda [<__ax], y

	adc <__ch
	sta <_mpd_ch		; tiles_offset = __scr_tiles_width_tbl[ _y ] + _x
#endasm
#else	//FLAG_DIR_COLUMNS
//	tiles_offset	= __scr_tiles_height_tbl[ _x ] + _y;
#asm
	lda <__cl
	asl a
	sta <__al
	lda <__ch
	rol a
	sta <__ah		; __ax = _x << 1

	mpd_add_word_to_word #___scr_tiles_height_tbl, <__ax

	lda [<__ax]

	clc
	adc <__dl
	sta <_mpd_cl

	ldy #1
	lda [<__ax], y

	adc <__dh
	sta <_mpd_ch		; tiles_offset = __scr_tiles_height_tbl[ _x ] + _y
#endasm
#endif	//FLAG_DIR_ROWS

#if	FLAG_MODE_BIDIR_SCROLL
//	tile_id		= mpd_farpeekb( mpd_TilesScr, mpd_ex(scr_offs) + tiles_offset );
#asm
	mpd_add_word_to_word2 <_mpd_ex, <_mpd_cx, <__ax		; scr_offs + tiles_offset
#endasm
#else	//FLAG_MODE_BIDIR_STAT_SCR + FLAG_MODE_STAT_SCR
//	tile_id		= mpd_farpeekb( mpd_TilesScr, __scr_offset + tiles_offset );
#asm
	mpd_add_word_to_word2 ___scr_offset, <_mpd_cx, <__ax	; __scr_offset + tiles_offset
#endasm
#endif	//FLAG_MODE_BIDIR_SCROLL

#asm
	__farptr _mpd_TilesScr, __bl, __si

	call _mpd_farptr_add_offset

	lda	<__bl
	tam	#3

	lda [<__si]				; a = tile_id
#endasm
#endif	//FLAG_MODE_MULTIDIR_SCROLL

#if	FLAG_TILES4X4
//	tile_id		= mpd_farpeekb( mpd_Tiles, __tiles_offset + ( tile_id << 2 ) + ( block_pos_y << 1 ) + block_pos_x );
#asm
	; __ax = __tiles_offset + ( tile_id << 2 ) + ( block_pos_y << 1 ) + block_pos_x

	stz <__ah

	; a = tile_id

	asl a
	rol <__ah
	asl a
	rol <__ah
	sta <__al				; tile_id << 2

	lda <_mpd_ah				; block_pos_y << 1
	asl a

	clc
	adc <_mpd_al				; block_pos_x

	mpd_add_a_to_word <__ax

	mpd_add_word_to_word ___tiles_offset, <__ax

	__farptr _mpd_Tiles, __bl, __si

	call _mpd_farptr_add_offset

	lda	<__bl
	tam	#3

	lda [<__si]				; a = tile_id
#endasm
#endif	//FLAG_TILES4X4

#if	FLAG_PROP_ID_PER_BLOCK
//	tile_id		= mpd_farpeekb( mpd_Props, __props_offset + tile_id );
#ifdef	MPD_RAM_TILE_PROPS
#asm
	mpd_add_a_to_word2 #___RAM_TileProps, <__si
#endasm
#else	//!MPD_RAM_TILE_PROPS
#asm
	; __ax = __props_offset + tile_id

	mpd_add_a_to_word2 ___props_offset, <__ax

	__farptr _mpd_Props, __bl, __si

	call _mpd_farptr_add_offset

	lda	<__bl
	tam	#3
#endasm
#endif	//MPD_RAM_TILE_PROPS
#else	//FLAG_PROP_ID_PER_CHR
//	tile_id		= mpd_farpeekb( mpd_Props, __props_offset + ( tile_id << 2 ) + ( CHR_pos_y << 1 ) + CHR_pos_x );
#asm
	; __ax = ( tile_id << 2 ) + ( CHR_pos_y << 1 ) + CHR_pos_x

	stz <__ah

	; a = tile_id

	asl a
	rol <__ah
	asl a
	rol <__ah
	sta <__al				; tile_id << 2

	lda <_mpd_bh				; CHR_pos_y << 1
	asl a

	clc
	adc <_mpd_bl				; CHR_pos_x

	mpd_add_a_to_word <__ax
#endasm

#ifdef	MPD_RAM_TILE_PROPS
#asm
	mpd_add_word_to_word2 #___RAM_TileProps, <__ax, <__si
#endasm
#else	//!MPD_RAM_TILE_PROPS
#asm
	mpd_add_word_to_word ___props_offset, <__ax

	__farptr _mpd_Props, __bl, __si

	call _mpd_farptr_add_offset

	lda	<__bl
	tam	#3
#endasm
#endif	//MPD_RAM_TILE_PROPS
#endif	//FLAG_PROP_ID_PER_BLOCK

#ifdef	MPD_DEBUG
	DBG_BORDER_COLOR_RESET
#endif

#asm
	lda [<__si]
	tax
	cla
#endasm
}

#asm
	.endprocgroup
#endasm