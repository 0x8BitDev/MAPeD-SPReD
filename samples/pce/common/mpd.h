//######################################################################################################
//
// This file is a part of the MAPeD-PCE Copyright 2017-2022 0x8BitDev ( MIT license. See LICENSE.txt )
// Desc: It contains tilemap rendering library and some HuC helper functions
//
//######################################################################################################

/*/	MPD-render v0.7
History:

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

#asm
MPD_DEBUG
#endasm

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

Active map width/height in pixels = map width/height in pixels - screen width/height in pixels

R: u16	mpd_map_active_width  -> mpd_active_map_width() - DEPRECATED
R: u16	mpd_map_active_height -> mpd_active_map_height() - DEPRECATED

#endif	//FLAG_MODE_MULTIDIR_SCROLL

#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL

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
void	mpd_draw_CHR( u16 _x, u16 _y, u8 _block2x2_ind, u8 _CHR_ind ) / _x/_y - screen space coordinates in pixels, _block2x2_ind - 0..255, _CHR_ind - 0..3
void	mpd_draw_block2x2( u8 _x, u8 _y, u8 _block2x2_ind ) / _x/_y - screen space coordinates in pixels, _block2x2_ind - 0..255

#if	FLAG_TILES4X4
void	mpd_draw_tile4x4( u8 _x, u8 _y, u8 _tile4x4_ind ) / _x/_y - screen space coordinates in pixels, _tile4x4_ind - 0..255
#endif	//FLAG_TILES4X4

u8	mpd_get_property( u16 _x, u16 _y ) / _x/_y - coordinates; RES: property id
void	mpd_draw_screen()

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
#endif	//FLAG_MODE_MULTIDIR_SCROLL

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

/************************/
/*			*/
/* HuC helper functions	*/
/*			*/
/************************/

/* u16 mpd_get_CR_val() */
#pragma fastcall mpd_get_CR_val()

/* void mpd_load_vram2( u16 _vaddr, u8 _bank, u16 _addr, u16 _words_cnt ) */
#pragma fastcall mpd_load_vram2( word __di, byte __bl, word __si, word __cx )

/* void mpd_load_vram( u16 _vaddr, far void* _addr, u16 _offset, u16 _words_cnt ) */
#pragma fastcall mpd_load_vram( word __di, farptr __bl:__si, word __ax, word __cx )

/* void mpd_load_palette( u8 _sub_plt, far void* _addr, u16 _offset, u8 _sub_plts_cnt ) */
#pragma fastcall mpd_load_palette( byte __al, farptr __bl:__si, word __dx, byte __cl )

/* void mpd_load_bat( u16 _vaddr, far void* _addr, u16 _offset, u8 _width, u8 _height ) */
#pragma fastcall mpd_load_bat( word __di, farptr __bl:__si, word __ax, byte __cl, byte __ch )

/* u16 mpd_farpeekw( far void* _addr, u16 _offset )*/
#pragma fastcall mpd_farpeekw( farptr __bl:__si, word __ax )

/* u8 mpd_farpeekb( far void* _addr, u16 _offset )*/
#pragma fastcall mpd_farpeekb( farptr __bl:__si, word __ax )

/* u16 mpd_farpeekw( u8 _bank, u16 _addr, u16 _offset )*/
#pragma fastcall mpd_farpeekw( byte __bl, word __si, word __ax )

/* u8 mpd_farpeekb( u8 _bank, u16 _addr, u16 _offset )*/
#pragma fastcall mpd_farpeekb( byte __bl, word __si, word __ax )

/* void	mpd_farmemcpyb( u8 _bank, u16 _addr, u16 _offset, void* _dst_addr, u8 _size ) */
#pragma	fastcall mpd_farmemcpyb( byte __bl, word __si, word __ax, word __dx, byte __bh )

/* void mpd_memcpyb( u16 _src_addr, u16 _dst_addr, u8 _size ) */
#pragma	fastcall mpd_memcpyb( word __ax, word __bx, byte __cl );

/* void mpd_get_ptr24( far void* _addr, u8 _ind, void* _dst_addr ) */
#pragma	fastcall mpd_get_ptr24( farptr __bl:__si, byte __al, word __dx )

// FOR INTERNAL USE:

u16	__fastcall __mpd_get_VRAM_addr( u16 _x<__ax>, u16 _y<acc> );

#if	FLAG_MODE_MULTIDIR_SCROLL
u8	__fastcall __mpd_calc_skip_CHRs_cnt( u8 _pos<acc> );
#endif

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

	.bss

__mTII:		.ds 1	; $73 tii
__mbsrci:	.ds 2
__mbdsti:	.ds 2
__mbleni:	.ds 2
__mtiirts	.ds 1	; $60 rts

	.code

	.procgroup

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

;// u16 mpd_farpeekw( byte __bl / bank, word __si / addr, word __ax / offset )
;
	.proc _mpd_farpeekw.3

	call _mpd_farpeekw.2
	rts

	.endp

;// u16 mpd_farpeekw( farptr __bl:__si / addr, word __ax / offset )
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

;// u8 mpd_farpeekb( byte __bl / bank, word __si / addr, word __ax / offset )
;
	.proc _mpd_farpeekb.3

	lda <__si + 1
	and #$1f
	sta <__si + 1

	call _mpd_farpeekb.2
	rts

	.endp

;// u8 mpd_farpeekb( farptr __bl:__si / addr, word __ax / offset )
;
	.proc _mpd_farpeekb.2

	call _mpd_farptr_add_offset

	lda	<__bl
	tam	#3

	lda	[__si]

	tax
	cla
	rts

	.endp

;// void mpd_farmemcpy( byte __bl / bank, word __si / addr, word __ax / offset, word __dx / dst_addr, byte __bh / size )
;
	.proc _mpd_farmemcpyb.5

	lda <__bh
	bne .cont
	rts		; exit if data size is zero
.cont:
	call _mpd_farptr_add_offset

	lda <__bh
	sta __mbleni
	stz __mbleni + 1

	jsr map_data

	stw <__si, __mbsrci
	stw <__dx, __mbdsti

	jsr __mTII

	jmp unmap_data

	.endp

;// void mpd_memcpyb( word __ax / src_addr, word __bx / dst_addr, byte __cl / size ) */
;
	.proc _mpd_memcpyb.3

	stw <__ax, __mbsrci
	stw <__bx, __mbdsti
	
	lda <__cl
	sta __mbleni
	stz __mbleni + 1

	jmp __mTII

	.endp

;// void mpd_get_ptr24( farptr __bl:__si / addr, byte __al / index, word __dx / dst_addr )
;
	.proc _mpd_get_ptr24.3

	cla
	sta <__ah
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

	.endprocgroup

;// void mpd_load_vram2( word __di / vaddr, byte __bl / bank, word __si / addr, word __cx / words_cnt )
;
_mpd_load_vram2.4:
	jmp _load_vram.3

;// void mpd_load_vram( word __di / vaddr, farptr __bl:__si / addr, word __ax / offset, word __cx / words_cnt )
;
	.proc _mpd_load_vram.4

	call _mpd_farptr_add_offset

	jmp _load_vram.3

	.endp

;// void mpd_load_palette( byte __al / sub_plt, farptr __bl:__si / addr, word __dx / offset, byte __cl / sub_plts_cnt )
;
	.proc _mpd_load_palette.4

	stw <__dx, <__ax
	call _mpd_farptr_add_offset

	jmp _load_palette.3

	.endp

;// void mpd_load_bat( word __di / vaddr, farptr __bl:__si / addr, word __ax / offset, byte __cl / width, byte __ch / height )
;	
	.proc _mpd_load_bat.5

	call _mpd_farptr_add_offset

	jmp _load_bat.4

	.endp

;// u16 mpd_get_CR_val()
;
	.proc _mpd_get_CR_val

	ldx <vdc_crl
	lda <vdc_crh

	rts

	.endp

#endasm


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
	mpd_farmemcpyb( mpd_curr_scr.scr_arr_ptr.bank, mpd_curr_scr.scr_arr_ptr.addr, mpd_curr_scr.scr_offset, _scr, sizeof( mpd_SCREEN ) );
}

void	mpd_get_screen_data( mpd_SCR_DATA* _scr_data, u16 _scr_ind )
{
#if	FLAG_LAYOUT_ADJ_SCR_INDS
	_scr_data->scr_offset	= mpd_farpeekw( __scr_arr.bank, __scr_arr.addr, ( _scr_ind << 1 ) ) - _scr_data->scr_arr_ptr.addr;
#else
	_scr_data->scr_offset	= _scr_ind - _scr_data->scr_arr_ptr.addr;	// _scr_ind is a screen address for FLAG_LAYOUT_ADJ_SCR
#endif
	mpd_farmemcpyb( _scr_data->scr_arr_ptr.bank, _scr_data->scr_arr_ptr.addr, _scr_data->scr_offset, _scr_data->scr, sizeof( mpd_SCREEN ) );
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

	mpd_get_ptr24( mpd_MapsArr, _map_ind, _scr_data->scr_arr_ptr );
}

void	mpd_copy_screen( mpd_SCR_DATA* _src_scr, mpd_SCR_DATA* _dst_scr )
{
	mpd_memcpyb( _src_scr, _dst_scr, sizeof( mpd_SCR_DATA ) );
}

#if	FLAG_MODE_MULTIDIR_SCROLL
u16	mpd_get_map_size( u8 _map_ind )
{
	return mpd_farpeekw( mpd_MapsDimArr, ( _map_ind << 1 ) );
}

u8	mpd_get_start_screen_ind( u8 _map_ind )
{
	return mpd_farpeekb( mpd_StartScrArr, _map_ind );
}

void	mpd_get_screen_data( mpd_SCR_DATA* _scr_data, u8 _scr_ind )
{
	_scr_data->scr_offset	= mpd_farpeekw( _scr_data->scr_arr_ptr.bank, _scr_data->scr_arr_ptr.addr, ( _scr_ind << 1 ) ) - _scr_data->scr_arr_ptr.addr;

	mpd_farmemcpyb( _scr_data->scr_arr_ptr.bank, _scr_data->scr_arr_ptr.addr, _scr_data->scr_offset, _scr_data->scr, sizeof( mpd_SCREEN ) );
}

void	mpd_get_start_screen( mpd_SCR_DATA* _scr_data, u8 _map_ind )
{
	mpd_get_screen_data( _scr_data, mpd_get_start_screen_ind( _map_ind ) );
}
#else	//FLAG_MODE_MULTIDIR_SCROLL

void	mpd_get_start_screen( mpd_SCR_DATA* _scr_data )
{
	_scr_data->scr_offset	= mpd_farpeekw( _scr_data->scr_arr_ptr.bank, _scr_data->scr_arr_ptr.addr, 0 ) - _scr_data->scr_arr_ptr.addr;

	mpd_farmemcpyb( _scr_data->scr_arr_ptr.bank, _scr_data->scr_arr_ptr.addr, _scr_data->scr_offset, _scr_data->scr, sizeof( mpd_SCREEN ) );
}
#endif	//FLAG_MODE_MULTIDIR_SCROLL

#if	FLAG_ENTITIES
void	__mpd_get_entity_by_offs( mpd_SCR_DATA* _scr_data, u16 _offset, mpd_ENTITY* _ent )
{
	u16	ent_offset;
	u16	ent_addr;

	// entity address offset
	ent_offset	= mpd_farpeekw( _scr_data->scr_arr_ptr.bank, _scr_data->scr_arr_ptr.addr, _offset ) - _scr_data->scr_arr_ptr.addr;

	// copy entity instance
	mpd_farmemcpyb( _scr_data->scr_arr_ptr.bank, _scr_data->scr_arr_ptr.addr, ent_offset, _ent->inst, sizeof( mpd_ENTITY_INSTANCE ) + ENT_MAX_PROPS_CNT );

	// base entity offset
	ent_offset	= _ent->inst.base_ent_addr - _scr_data->scr_arr_ptr.addr;

	// copy base entity
	mpd_farmemcpyb( _scr_data->scr_arr_ptr.bank, _scr_data->scr_arr_ptr.addr, ent_offset, _ent->base, sizeof( mpd_ENTITY_BASE ) + ENT_MAX_PROPS_CNT );
}

void	__mpd_get_entity_by_addr( mpd_SCR_DATA* _scr_data, u16 _addr, mpd_ENTITY* _ent )
{
	u16	ent_offset;
	u16	ent_addr;

	// entity address offset
	ent_offset	= _addr - _scr_data->scr_arr_ptr.addr;

	// copy entity instance
	mpd_farmemcpyb( _scr_data->scr_arr_ptr.bank, _scr_data->scr_arr_ptr.addr, ent_offset, _ent->inst, sizeof( mpd_ENTITY_INSTANCE ) + ENT_MAX_PROPS_CNT );

	// base entity offset
	ent_offset	= _ent->inst.base_ent_addr - _scr_data->scr_arr_ptr.addr;

	// copy base entity
	mpd_farmemcpyb( _scr_data->scr_arr_ptr.bank, _scr_data->scr_arr_ptr.addr, ent_offset, _ent->base, sizeof( mpd_ENTITY_BASE ) + ENT_MAX_PROPS_CNT );
}

void	mpd_init_base_ent_arr()
{
	mpd_get_ptr24( mpd_BaseEntities, 0, &__base_ent_arr );
}

u16	mpd_get_base_ent_cnt()
{
	return mpd_farpeekw( __base_ent_arr.bank, __base_ent_arr.addr, 0 );
}

void	mpd_get_base_entity( mpd_SCR_DATA* _scr_data, u16 _ent_ind )
{
	u16	ent_offset;

	ent_offset = mpd_farpeekw( __base_ent_arr.bank, __base_ent_arr.addr, ( _ent_ind << 1 ) + 2 ) - _scr_data->scr_arr_ptr.addr;

	// copy base entity
	mpd_farmemcpyb( _scr_data->scr_arr_ptr.bank, _scr_data->scr_arr_ptr.addr, ent_offset, _scr_data->inst_ent.base, sizeof( mpd_ENTITY_BASE ) + ENT_MAX_PROPS_CNT );
}

void	mpd_get_entity( mpd_SCR_DATA* _scr_data, u8 _ent_ind )
{
	u16	ent_offset;

	// entity instance address
	ent_offset	= _scr_data->scr_offset + sizeof( mpd_SCREEN ) + ( _ent_ind << 1 );

	__mpd_get_entity_by_offs( _scr_data, ent_offset, _scr_data->inst_ent );

	if( _scr_data->inst_ent.inst.targ_ent_addr )
	{
		__mpd_get_entity_by_addr( _scr_data, _scr_data->inst_ent.inst.targ_ent_addr, _scr_data->targ_ent );
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
	u8	i;

	for( i = 0; i < _scr_data->scr.ents_cnt; i++ )
	{
		mpd_get_entity( _scr_data, i );

		if( _scr_data->inst_ent.base.id == _id )
		{
			return TRUE;
		}
	}

	return FALSE;
}

bool	mpd_find_entity_by_inst_id( mpd_SCR_DATA* _scr_data, u8 _id )
{
	u8	i;

	for( i = 0; i < _scr_data->scr.ents_cnt; i++ )
	{
		mpd_get_entity( _scr_data, i );

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

#define	DBG_COLOR_ROW_COL_FILLING	256	// green
#define	DBG_COLOR_DISP_LIST_FLUSH	184	// red
#define	DBG_COLOR_DRAW_SCREEN		4	// blue
#define	DBG_COLOR_GET_TILE_PROP		504	// yellow

#define	DBG_BORDER_COLOR_GET_TILE_PROP		DBG_MPD_BORDER_COLOR( DBG_COLOR_GET_TILE_PROP )
#define	DBG_BORDER_COLOR_DRAW_SCREEN		DBG_MPD_BORDER_COLOR( DBG_COLOR_DRAW_SCREEN )
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
/* Tilemap rendering functions	*/
/*				*/
/********************************/

const u8 mpd_ver[] = { "M", "P", "D", "0", "7", 0 };

/* flags */

const u8 ADJ_SCR_LEFT	= 0;
const u8 ADJ_SCR_UP	= 1;
const u8 ADJ_SCR_RIGHT	= 2;
const u8 ADJ_SCR_DOWN	= 3;

/* constants */

const u16 __c_scr_tiles_size	= ScrTilesWidth * ScrTilesHeight;

/* variables */
u16		__CR_IW_val;
u16		__CR_val;

#if	!FLAG_MODE_MULTIDIR_SCROLL
u16		__scr_offset;

u16		__scr_tiles_width_tbl[ ScrTilesHeight ];
u16		__scr_tiles_height_tbl[ ScrTilesWidth ];
#endif	//!FLAG_MODE_MULTIDIR_SCROLL

#if	FLAG_MODE_MULTIDIR_SCROLL
u16		__map_tiles_width;
u16		__map_tiles_height;
u8		mpd_map_scr_width;
u8		mpd_map_scr_height;
u16		mpd_map_active_width;	// active map area = map_width_in_pixels - scr_width_in_pixels
u16		mpd_map_active_height;	// the same in height
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
u16		__BAT_width_dec1;
u16		__BAT_width_dec1_inv;
u8		__BAT_height_dec1;
u8		__BAT_width_pow2;
u16		__BAT_size;
u16		__BAT_size_dec1;	// ( BAT_width * BAT_height ) - 1

u16		__props_offset;
u16		__blocks_offset;
#if	FLAG_TILES4X4
u16		__tiles_offset;
#endif

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

// variables for assembly optimization

union mpd_var16
{
	u16	w;

	struct
	{
		u8	l;
		u8	b;
	};

	struct
	{
		s16	ws;
	};
};

union mpd_var16	mpd_ax;
union mpd_var16	mpd_bx;
union mpd_var16	mpd_cx;
union mpd_var16	mpd_dx;
union mpd_var16	mpd_ex;
union mpd_var16	mpd_fx;


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
	__BAT_width_dec1_inv	= ~__BAT_width_dec1;
	__BAT_height_dec1	= BAT_height - 1;
	__BAT_size		= __BAT_width * BAT_height;
	__BAT_size_dec1		= __BAT_size - 1;
}

#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
void	mpd_init( u8 _map_ind, u8 _step )
#else
void	mpd_init( u8 _map_ind )
#endif
{
	u16	n;

#if	!FLAG_MODE_MULTIDIR_SCROLL
	u16	data_acc;
#endif	//!FLAG_MODE_MULTIDIR_SCROLL

	// init MPD's TII
	#asm
	lda #$73
	sta __mTII
	lda #$60
	sta __mtiirts
	#endasm

#if	!FLAG_MODE_MULTIDIR_SCROLL

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

	mpd_init_screen_arr( &mpd_curr_scr, _map_ind );
	mpd_get_start_screen( &mpd_curr_scr );

	__scr_offset	= mpd_curr_scr.scr.scr_ind * __c_scr_tiles_size;

#endif	//!FLAG_MODE_MULTIDIR_SCROLL

#if	FLAG_LAYOUT_ADJ_SCR_INDS
	mpd_get_ptr24( mpd_MapsScrArr, _map_ind, &__scr_arr );
#endif	//FLAG_LAYOUT_ADJ_SCR_INDS

	__mpd_get_BAT_params();

	set_screen_size( BAT_INDEX );

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

	__maps_offset		= mpd_farpeekw( mpd_MapsOffs,	__map_ind_mul2 );
	__maps_tbl_offset	= mpd_farpeekw( mpd_MapsTblOffs,__map_ind_mul2 );

	n = mpd_get_map_size( _map_ind );
	mpd_map_scr_width	= n & 0x00ff;
	mpd_map_scr_height	= ( n & 0xff00 ) >> 8;

	__map_tiles_width	= mpd_map_scr_width * ScrTilesWidth;
	__map_tiles_height	= mpd_map_scr_height * ScrTilesHeight;

	mpd_map_active_width	= ( mpd_map_scr_width * ScrPixelsWidth ) - ScrPixelsWidth;
	mpd_map_active_height	= ( mpd_map_scr_height * ScrPixelsHeight ) - ScrPixelsHeight;

	__mpd_calc_scr_pos_by_scr_ind( mpd_get_start_screen_ind( _map_ind ), FALSE );

#else	//FLAG_MODE_MULTIDIR_SCROLL
	__curr_chr_id_mul2	= 0xff;
#endif	//FLAG_MODE_MULTIDIR_SCROLL
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
void	mpd_draw_CHR( u16 _x, u16 _y, u8 _block2x2_ind, u8 _CHR_ind )
{
	u16	offs;

	offs = __blocks_offset + ( _block2x2_ind << 3 ) + ( _CHR_ind << 1 );

	mpd_load_vram( __mpd_get_VRAM_addr( _x, _y ), mpd_Attrs, offs, 1 );
}

void	mpd_draw_block2x2( u16 _x, u16 _y, u8 _block2x2_ind )
{
	u16	offs;

	offs	= __blocks_offset + ( _block2x2_ind << 3 );

	mpd_load_vram( __mpd_get_VRAM_addr( _x, _y ), mpd_Attrs, offs, 1 );

	offs += 2;
	_x   += 8;
	mpd_load_vram( __mpd_get_VRAM_addr( _x, _y ), mpd_Attrs, offs, 1 );

	offs += 4;
	_y   += 8;
	mpd_load_vram( __mpd_get_VRAM_addr( _x, _y ), mpd_Attrs, offs, 1 );

	offs -= 2;
	_x   -= 8;
	mpd_load_vram( __mpd_get_VRAM_addr( _x, _y ), mpd_Attrs, offs, 1 );
}

void	__mpd_draw_block2x2( u16 _vaddr, u16 _offset )
{
	mpd_load_vram( _vaddr, mpd_Attrs, _offset, 2 );
	mpd_load_vram( _vaddr + __BAT_width, mpd_Attrs, _offset + 4, 2 );
}

#if	FLAG_TILES4X4
void	mpd_draw_tile4x4( u16 _x, u16 _y, u8 _tile4x4_ind )
{
	u16	tiles12;
	u16	tiles34;
	u16	offs;

	offs	= __tiles_offset + ( _tile4x4_ind << 2 );

	tiles12 = mpd_farpeekw( mpd_Tiles, offs );
	tiles34 = mpd_farpeekw( mpd_Tiles, offs + 2 );

	mpd_draw_block2x2( _x, _y,	tiles12 );
	mpd_draw_block2x2( _x + 16, _y,	( tiles12 >> 8 ) );
	_y += 16;
	mpd_draw_block2x2( _x, _y,	tiles34 );
	mpd_draw_block2x2( _x + 16, _y,	( tiles34 >> 8 ) );
}

void	__mpd_draw_tile4x4( u16 _vaddr, u16 _offset )
{
	u16	tiles12;
	u16	tiles34;

	tiles12 = mpd_farpeekw( mpd_Tiles, _offset );
	tiles34 = mpd_farpeekw( mpd_Tiles, _offset + 2 );

	__mpd_draw_block2x2( _vaddr,		__blocks_offset + ( ( tiles12 & 0x00ff ) << 3 ) );
	__mpd_draw_block2x2( _vaddr + 2,	__blocks_offset + ( ( tiles12 & 0xff00 ) >> 5 ) );
	_vaddr += ( __BAT_width << 1 );
	__mpd_draw_block2x2( _vaddr, 		__blocks_offset + ( ( tiles34 & 0x00ff ) << 3 ) );
	__mpd_draw_block2x2( _vaddr + 2,	__blocks_offset + ( ( tiles34 & 0xff00 ) >> 5 ) );
}
#endif	//FLAG_TILES4X4
#endif	//FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL + FLAG_MODE_BIDIR_STAT_SCR

#if	FLAG_MODE_BIDIR_SCROLL + FLAG_MODE_BIDIR_STAT_SCR
void	__mpd_draw_tiled_screen( u16 _BAT_offset )
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
			__mpd_draw_block2x2( _BAT_offset + ( ( h_acc + w ) << 1 ), __blocks_offset + ( scr_tile << 3 ) );
#elif	FLAG_TILES4X4
			__mpd_draw_tile4x4( _BAT_offset + ( ( h_acc + w ) << 2 ), __tiles_offset + ( scr_tile << 2 ) );
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
			__mpd_draw_block2x2( _BAT_offset + ( ( h_acc + w ) << 1 ), __blocks_offset + ( scr_tile << 3 ) );
#elif	FLAG_TILES4X4
			__mpd_draw_tile4x4( _BAT_offset + ( ( h_acc + w ) << 2 ), __tiles_offset + ( scr_tile << 2 ) );
#endif
			++n;
		}

		h_acc += __BAT_width;
	}

#endif	//FLAG_DIR_COLUMNS|FLAG_DIR_ROWS
}
#endif	//FLAG_MODE_BIDIR_SCROLL + FLAG_MODE_BIDIR_STAT_SCR

#if	FLAG_MODE_MULTIDIR_SCROLL
void	__mpd_calc_scr_pos_by_scr_ind( u8 _scr_ind, bool _reset_scroll )
{
	u16	LUT_pos_x;
	u16	LUT_pos_y;

	LUT_pos_x	= ScrTilesWidth * ( _scr_ind % mpd_map_scr_width );
	LUT_pos_y	= ScrTilesHeight * ( _scr_ind / mpd_map_scr_width );

	__mpd_calc_scr_pos_by_LUT_pos( LUT_pos_x, LUT_pos_y, _reset_scroll );
}

void	__mpd_calc_scr_pos_by_LUT_pos( u16 _LUT_pos_x, u16 _LUT_pos_y, bool _reset_scroll )
{
	u16	LUT_offset;

	if( _reset_scroll )
	{
		mpd_scroll_x	= 0;
		mpd_scroll_y	= 0;
	}
	else
	{
#if	FLAG_TILES2X2
		mpd_scroll_x	= _LUT_pos_x << 4;
#elif	FLAG_TILES4X4
		mpd_scroll_x	= _LUT_pos_x << 5;
#endif	//FLAG_TILES2X2|FLAG_TILES4X4

#if	FLAG_TILES2X2
		mpd_scroll_y	= _LUT_pos_y << 4;
#elif	FLAG_TILES4X4
		mpd_scroll_y	= _LUT_pos_y << 5;
#endif	//FLAG_TILES2X2|FLAG_TILES4X4
	}

#if	FLAG_DIR_COLUMNS
	LUT_offset		= __maps_tbl_offset + ( _LUT_pos_x << 1 );
	__init_tiles_offset	= mpd_farpeekw( mpd_MapsTbl, LUT_offset ) + _LUT_pos_y;
#elif	FLAG_DIR_ROWS
	__height_scr_step	= __map_tiles_width * ScrTilesHeight;
	LUT_offset		= __maps_tbl_offset + ( _LUT_pos_y << 1 );
	__init_tiles_offset	= mpd_farpeekw( mpd_MapsTbl, LUT_offset ) + _LUT_pos_x;
#endif	//FLAG_DIR_COLUMNS|FLAG_DIR_ROWS

	__mpd_update_data_offsets();
}

void	__mpd_draw_tiled_screen( u16 _BAT_offset )
{
	u8	scr_tile;
	u16	w, h;
	u16	n;
	u16	tiles_offset;
	u16	side_step;

	tiles_offset	= __maps_offset + __init_tiles_offset;

	mpd_ax.w	= _BAT_offset + ( ( mpd_scroll_x >> 3 ) & __BAT_width_dec1 ) + ( ( ( mpd_scroll_y >> 3 ) /*& __BAT_height_dec1*/ ) << __BAT_width_pow2 );

#if	FLAG_DIR_COLUMNS

	side_step	= ScrTilesHeight * mpd_map_scr_height;

	for( mpd_cx.w = 0; mpd_cx.w < ScrTilesWidth; mpd_cx.w++ )
	{
		mpd_bx.w = 0;

		n = mpd_cx.w * side_step;

		for( h = 0; h < ScrTilesHeight; h++ )
		{
			scr_tile = mpd_farpeekb( mpd_Maps, tiles_offset + n );
#asm
			; <__cx = ( mpd_ax.w + ( mpd_bx.w << 1 ) )

			lda _mpd_bx
			asl a
			sta <__bl
			lda _mpd_bx + 1
			rol a
			sta <__bh
#endasm

#if	FLAG_TILES4X4
#asm
			mpd_mul2_word <__bx
#endasm
#endif

#asm
			mpd_add_word_to_word2 _mpd_ax, <__bx, <__cx

			; <__al = mpd_dx.w & __BAT_width_dec1_inv

			lda <__cl
			and ___BAT_width_dec1_inv
			sta <__al

			; mpd_dx.w = ( ( ( <__cx + ( mpd_cx.w << 1 ) ) & __BAT_width_dec1 ) | <__al ) & __BAT_size_dec1

			lda _mpd_cx
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

			sta _mpd_dx

			lda <__ch
			and ___BAT_size_dec1 + 1
			sta _mpd_dx + 1
#endasm

#if	FLAG_TILES2X2
			__mpd_draw_block2x2( mpd_dx.w, __blocks_offset + ( scr_tile << 3 ) );
#elif	FLAG_TILES4X4
			__mpd_draw_tile4x4( mpd_dx.w, __tiles_offset + ( scr_tile << 2 ) );
#endif
			mpd_bx.w += __BAT_width;
			++n;
		}
	}
#elif	FLAG_DIR_ROWS

	side_step	= ScrTilesWidth * mpd_map_scr_width;

	mpd_bx.w = 0;

	for( h = 0; h < ScrTilesHeight; h++ )
	{
		n = h * side_step;

#if	FLAG_TILES2X2
		mpd_dx.w	= mpd_ax.w + ( mpd_bx.w << 1 );
#elif	FLAG_TILES4X4
		mpd_dx.w	= mpd_ax.w + ( mpd_bx.w << 2 );
#endif
		mpd_ex.w	= mpd_dx.w & __BAT_width_dec1_inv;

		for( mpd_cx.w = 0; mpd_cx.w < ScrTilesWidth; mpd_cx.w++ )
		{
			scr_tile = mpd_farpeekb( mpd_Maps, tiles_offset + n );
#if	FLAG_TILES2X2
#asm
			; mpd_fx.w = ( ( ( mpd_dx.w + ( mpd_cx.w << 1 ) ) & __BAT_width_dec1 ) | mpd_ex.w ) & __BAT_size_dec1

			lda _mpd_cx
			asl a
#endasm
#elif	FLAG_TILES4X4
#asm
			; mpd_fx.w = ( ( ( mpd_dx.w + ( mpd_cx.w << 2 ) ) & __BAT_width_dec1 ) | mpd_ex.w ) & __BAT_size_dec1

			lda _mpd_cx
			asl a
			asl a
#endasm
#endif

#asm
			clc
			adc _mpd_dx

			and ___BAT_width_dec1
			ora _mpd_ex

			sta _mpd_fx

			lda _mpd_dx + 1
			and ___BAT_size_dec1 + 1
			sta _mpd_fx + 1
#endasm

#if	FLAG_TILES2X2
			__mpd_draw_block2x2( mpd_fx.w, __blocks_offset + ( scr_tile << 3 ) );
#elif	FLAG_TILES4X4
			__mpd_draw_tile4x4( mpd_fx.w, __tiles_offset + ( scr_tile << 2 ) );
#endif
			++n;
		}

		mpd_bx.w += __BAT_width;
	}

#endif	//FLAG_DIR_COLUMNS|FLAG_DIR_ROWS
}

void	__mpd_draw_free_tiled_screen()
{
	u8	scr_tile;
	u8	x_beg_CHR;
	u8	y_beg_CHR;
	u16	w, h;
	u16	width_cnt, height_cnt;
	u16	CHR_x;
	u16	CHR_y;
	u16	n;
	u16	tiles_offset;
	u16	side_step;

	CHR_x		= mpd_scroll_x;
	CHR_y		= mpd_scroll_y;

	x_beg_CHR	= __mpd_calc_skip_CHRs_cnt( mpd_scroll_x );
	y_beg_CHR	= __mpd_calc_skip_CHRs_cnt( mpd_scroll_y );

	width_cnt	= x_beg_CHR + SCR_CHRS_WIDTH + ( mpd_scroll_x & 0x07 ? 1:0 );
	height_cnt	= y_beg_CHR + SCR_CHRS_HEIGHT + ( mpd_scroll_y & 0x07 ? 1:0 );

	tiles_offset	= __maps_offset + __init_tiles_offset;

#if	FLAG_DIR_COLUMNS

	side_step	= ScrTilesHeight * mpd_map_scr_height;

	for( w = x_beg_CHR; w < width_cnt; w++ )
	{
#if	FLAG_TILES2X2
		n = ( w >> 1 ) * side_step;
#else
		n = ( w >> 2 ) * side_step;
#endif
		CHR_y = mpd_scroll_y;

		for( h = y_beg_CHR; h < height_cnt; h++ )
		{
			scr_tile = mpd_farpeekb( mpd_Maps, tiles_offset + n );
#if	FLAG_TILES4X4
			scr_tile = mpd_farpeekw( mpd_Tiles, __tiles_offset + ( scr_tile << 2 ) + ( h & 0x02 ) + ( ( w & 0x02 ) >> 1 ) );
#endif
			mpd_load_vram( __mpd_get_VRAM_addr( CHR_x, CHR_y ), mpd_Attrs, __blocks_offset + ( scr_tile << 3 ) + ( ( h & 0x01 ) << 2 ) + ( ( w & 0x01 ) << 1 ), 1 );
#if	FLAG_TILES2X2
			n += h & 0x01;
#elif	FLAG_TILES4X4
			n += ( ( h & 0x03 ) == 0x03 ) ? 1:0;
#endif
			CHR_y += 8;
		}

		CHR_x += 8;
	}
#elif	FLAG_DIR_ROWS

	side_step	= ScrTilesWidth * mpd_map_scr_width;

	for( h = y_beg_CHR; h < height_cnt; h++ )
	{
#if	FLAG_TILES2X2
		n = ( h >> 1 ) * side_step;
#else
		n = ( h >> 2 ) * side_step;
#endif
		CHR_x = mpd_scroll_x;

		for( w = x_beg_CHR; w < width_cnt; w++ )
		{
			scr_tile = mpd_farpeekb( mpd_Maps, tiles_offset + n );
#if	FLAG_TILES4X4
			scr_tile = mpd_farpeekw( mpd_Tiles, __tiles_offset + ( scr_tile << 2 ) + ( h & 0x02 ) + ( ( w & 0x02 ) >> 1 ) );
#endif
			mpd_load_vram( __mpd_get_VRAM_addr( CHR_x, CHR_y ), mpd_Attrs, __blocks_offset + ( scr_tile << 3 ) + ( ( h & 0x01 ) << 2 ) + ( ( w & 0x01 ) << 1 ), 1 );
#if	FLAG_TILES2X2
			n += w & 0x01;
#elif	FLAG_TILES4X4
			n += ( ( w & 0x03 ) == 0x03 ) ? 1:0;
#endif
			CHR_x += 8;
		}

		CHR_y += 8;
	}

#endif	//FLAG_DIR_COLUMNS|FLAG_DIR_ROWS
}

void	mpd_draw_screen_by_ind( u8 _scr_ind )
{
	mpd_draw_screen_by_ind_offs( _scr_ind, 0, FALSE );
}

void	mpd_draw_screen_by_ind_offs( u8 _scr_ind, u16 _BAT_offset, bool _reset_scroll )
{
	__upd_flags = 0;

	__mpd_calc_scr_pos_by_scr_ind( _scr_ind, _reset_scroll );

	__mpd_draw_screen( _BAT_offset, 0 );
}

void	mpd_draw_screen_by_pos( u16 _x, u16 _y )
{
	u16	LUT_pos_x;
	u16	LUT_pos_y;

	__upd_flags = 0;

#if	FLAG_TILES2X2
	LUT_pos_x = _x >> 4;
	LUT_pos_y = _y >> 4;
#elif	FLAG_TILES4X4
	LUT_pos_x = _x >> 5;
	LUT_pos_y = _y >> 5;
#endif

	__mpd_calc_scr_pos_by_LUT_pos( LUT_pos_x, LUT_pos_y, FALSE );

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
void	mpd_draw_screen_by_data( mpd_SCR_DATA* _scr_data )
{
	mpd_draw_screen_by_data_offs( _scr_data, 0 );
}

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

void	mpd_draw_screen()
{
	__mpd_draw_screen( 0, 0 );
}

void	__mpd_draw_screen( u16 _BAT_offset, u8 _free_scr )
{
	u16		num_CHRs;
	mpd_PTR24	CHRs_ptr;

#if	!FLAG_MODE_MULTIDIR_SCROLL
	u8		chr_id_mul2;
#endif

#asm
.ifdef MPD_DEBUG
#endasm
	DBG_BORDER_COLOR_DRAW_SCREEN
#asm
.endif
#endasm
	// load tiles & palette

#if	FLAG_MODE_MULTIDIR_SCROLL
	{
		num_CHRs = mpd_farpeekw( mpd_CHRs_size, __curr_chr_id_mul2 ) >> 5;

#else
	chr_id_mul2	= mpd_curr_scr.scr.chr_id << 1;
	num_CHRs	= mpd_farpeekw( mpd_CHRs_size, chr_id_mul2 ) >> 5;

	if( mpd_curr_scr.scr.chr_id != ( __curr_chr_id_mul2 >> 1 ) )
	{
		__curr_chr_id_mul2 = mpd_curr_scr.scr.chr_id << 1;

		__mpd_update_data_offsets();

#endif	//FLAG_MODE_MULTIDIR_SCROLL

		mpd_get_ptr24( mpd_CHRs, __curr_chr_id_mul2 >> 1, &CHRs_ptr );

		mpd_load_vram2( CHRS_OFFSET << 5, CHRs_ptr.bank, CHRs_ptr.addr, num_CHRs << 4 );
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

#asm
.ifdef MPD_DEBUG
#endasm
	DBG_BORDER_COLOR_RESET
#asm
.endif
#endasm
}

#if	!FLAG_MODE_MULTIDIR_SCROLL
u16	mpd_get_adj_screen( mpd_SCR_DATA* _scr_data, u8 _ind )
{
	u16	adj_scr;

#if	FLAG_MARKS
	if( !( _scr_data->scr.marks & ( 1 << ( _ind + 4 ) ) ) )
	{
		return 0xffff;
	}
#endif

	adj_scr = _scr_data->scr.adj_scr[ _ind ];

#if	FLAG_LAYOUT_ADJ_SCR
	if( adj_scr != NULL )
#else
	if( adj_scr != 0xff )
#endif
	{
		return adj_scr;
	}

	return 0xffff;
}

bool	mpd_check_adj_screen( u8 _ind )
{
	u16	adj_scr;

	adj_scr = mpd_get_adj_screen( &mpd_curr_scr, _ind );

	if( adj_scr != 0xffff )
	{
		__mpd_get_screen_data( adj_scr, mpd_curr_scr.scr );

		__scr_offset	= mpd_curr_scr.scr.scr_ind * __c_scr_tiles_size;

		return TRUE;
	}

	return FALSE;
}
#endif	//!FLAG_MODE_MULTIDIR_SCROLL

/********************************/
/*				*/
/* Tilemap scrolling functions	*/
/*				*/
/********************************/

#if	FLAG_MODE_BIDIR_SCROLL
void	mpd_move_left()
{
	u8	tmp_pos;

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

	tmp_pos = mpd_scroll_x;
	tmp_pos &= 0x07;

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

	if( ( ( tmp_pos - mpd_scroll_step_x ) & 0x08 ) || !tmp_pos )
	{
		__upd_flags |= UPD_FLAG_DRAW_LEFT;
	}
}

void	mpd_move_right()
{
	u8	tmp_pos;

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

	tmp_pos = mpd_scroll_x;
	tmp_pos &= 0x07;

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

	if( ( ( tmp_pos + mpd_scroll_step_x ) & 0x08 ) || !tmp_pos )
	{
		__upd_flags |= UPD_FLAG_DRAW_RIGHT;
	}
}

void	mpd_move_up()
{
	u8	tmp_pos;

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

	tmp_pos = mpd_scroll_y;
	tmp_pos &= 0x07;

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

	if( ( ( tmp_pos - mpd_scroll_step_y ) & 0x08 ) || !tmp_pos )
	{
		__upd_flags |= UPD_FLAG_DRAW_UP;
	}
}

void	mpd_move_down()
{
	u8	tmp_pos;

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

	tmp_pos = mpd_scroll_y;
	tmp_pos &= 0x07;

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

	if( ( ( tmp_pos + mpd_scroll_step_y ) & 0x08 ) || !tmp_pos )
	{
		__upd_flags |= UPD_FLAG_DRAW_DOWN;
	}
}
#endif	//FLAG_MODE_BIDIR_SCROLL

#if	FLAG_MODE_MULTIDIR_SCROLL
void	mpd_move_left()
{
	u8	tmp_pos;

	tmp_pos = mpd_scroll_x;
	tmp_pos &= 0x07;

	mpd_scroll_x -= mpd_scroll_step_x;

	if( mpd_scroll_x <= 0 )
	{
		mpd_scroll_x = 0;
		return;
	}

	if( ( ( tmp_pos - mpd_scroll_step_x ) & 0x08 ) || !tmp_pos )
	{
		__upd_flags |= UPD_FLAG_DRAW_LEFT;
	}
}

void	mpd_move_right()
{
	u8	tmp_pos;

	tmp_pos = mpd_scroll_x;
	tmp_pos &= 0x07;

	mpd_scroll_x += mpd_scroll_step_x;

	if( mpd_scroll_x >= mpd_map_active_width )
	{
		mpd_scroll_x = mpd_map_active_width;
		return;
	}

	if( ( ( tmp_pos + mpd_scroll_step_x ) & 0x08 ) || !tmp_pos )
	{
		__upd_flags |= UPD_FLAG_DRAW_RIGHT;
	}
}

void	mpd_move_up()
{
	u8	tmp_pos;

	tmp_pos = mpd_scroll_y;
	tmp_pos &= 0x07;

	mpd_scroll_y -= mpd_scroll_step_y;

	if( mpd_scroll_y <= 0 )
	{
		mpd_scroll_y = 0;
		return;
	}

	if( ( ( tmp_pos - mpd_scroll_step_y ) & 0x08 ) || !tmp_pos )
	{
		__upd_flags |= UPD_FLAG_DRAW_UP;
	}
}

void	mpd_move_down()
{
	u8	tmp_pos;

	tmp_pos = mpd_scroll_y;
	tmp_pos &= 0x07;

	mpd_scroll_y += mpd_scroll_step_y;

	if( mpd_scroll_y >= mpd_map_active_height )
	{
		mpd_scroll_y = mpd_map_active_height;
		return;
	}

	if( ( ( tmp_pos + mpd_scroll_step_y ) & 0x08 ) || !tmp_pos )
	{
		__upd_flags |= UPD_FLAG_DRAW_DOWN;
	}
}

u8	mpd_get_curr_screen_ind()
{
	return ( ( mpd_scroll_y / ScrPixelsHeight ) * mpd_map_scr_width ) + ( mpd_scroll_x / ScrPixelsWidth );
}
#endif	//FLAG_MODE_MULTIDIR_SCROLL

#if	FLAG_MODE_MULTIDIR_SCROLL + FLAG_MODE_BIDIR_SCROLL
void	mpd_clear_update_flags()
{
	__upd_flags &= ~(UPD_FLAG_DRAW_MASK);
}

void	mpd_update_screen()
{
#asm
.ifdef MPD_DEBUG
#endasm
	DBG_BORDER_COLOR_ROW_COL_FILLING
#asm
.endif
#endasm
	if( __upd_flags & UPD_FLAG_DRAW_MASK )
	{
		__CR_val = mpd_get_CR_val();

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
		vreg( 5, __CR_val );
	}
#asm
.ifdef MPD_DEBUG
#endasm
	DBG_BORDER_COLOR_RESET
#asm
.endif
#endasm
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

	__mpd_fill_column_data( __mpd_get_VRAM_addr( mpd_scroll_x, mpd_scroll_y ), COLUMN_CHRS_CNT );

#else	//FLAG_MODE_MULTIDIR_SCROLL
	u16	map_pos_x;
	u16	map_pos_y;
	u8	u8_pos_y;

	map_pos_x	= mpd_scroll_x;
	map_pos_y	= mpd_scroll_y;
	u8_pos_y	= map_pos_y;

#if	FLAG_TILES4X4
	map_pos_x >>= 5;
	map_pos_y >>= 5;
#else
	map_pos_x >>= 4;
	map_pos_y >>= 4;
#endif

#if	FLAG_DIR_ROWS
	__tmp_tiles_offset	= mpd_farpeekw( mpd_MapsTbl, __maps_tbl_offset + ( map_pos_y << 1 ) ) + map_pos_x;
#else
	__tmp_tiles_offset	= mpd_farpeekw( mpd_MapsTbl, __maps_tbl_offset + ( map_pos_x << 1 ) ) + map_pos_y;
#endif
	__tmp_tiles_offset	+= __maps_offset;

	__mpd_fill_column_data( __mpd_get_VRAM_addr( mpd_scroll_x, mpd_scroll_y ), ( ( mpd_scroll_y & 0x0f ) ? COLUMN_CHRS_CNT_INC1:COLUMN_CHRS_CNT ), __mpd_calc_skip_CHRs_cnt( u8_pos_y ) );

#endif	//FLAG_MODE_BIDIR_SCROLL
}

void	__mpd_draw_right_tiles_column()
{
#if	FLAG_MODE_BIDIR_SCROLL
	mpd_SCREEN 	tmp_scr;

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

	__mpd_fill_column_data( __mpd_get_VRAM_addr( mpd_scroll_x + ScrPixelsWidth, mpd_scroll_y ), COLUMN_CHRS_CNT );

#else	//FLAG_MODE_MULTIDIR_SCROLL
	u16	map_pos_x;
	u16	map_pos_y;
	u8	u8_pos_y;

	map_pos_x	= mpd_scroll_x;
	map_pos_y	= mpd_scroll_y;
	u8_pos_y	= map_pos_y;

#if	FLAG_TILES4X4
	map_pos_x >>= 5;
	map_pos_y >>= 5;
#else
	map_pos_x >>= 4;
	map_pos_y >>= 4;
#endif

#if	FLAG_DIR_ROWS
	__tmp_tiles_offset	= mpd_farpeekw( mpd_MapsTbl, __maps_tbl_offset + ( map_pos_y << 1 ) ) + map_pos_x + ScrTilesWidth;
#else	
	__tmp_tiles_offset	= mpd_farpeekw( mpd_MapsTbl, __maps_tbl_offset + ( ( map_pos_x + ScrTilesWidth ) << 1 ) ) + map_pos_y;
#endif
	__tmp_tiles_offset	+= __maps_offset;

	__mpd_fill_column_data( __mpd_get_VRAM_addr( mpd_scroll_x + ScrPixelsWidth, mpd_scroll_y ), ( ( mpd_scroll_y & 0x0f ) ? COLUMN_CHRS_CNT_INC1:COLUMN_CHRS_CNT ), __mpd_calc_skip_CHRs_cnt( u8_pos_y ) );

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

	__mpd_fill_row_data( __mpd_get_VRAM_addr( mpd_scroll_x, mpd_scroll_y ), ROW_CHRS_CNT );

#else	//FLAG_MODE_MULTIDIR_SCROLL
	u16	map_pos_x;
	u16	map_pos_y;
	u8	u8_pos_x;

	map_pos_x	= mpd_scroll_x;
	map_pos_y	= mpd_scroll_y;
	u8_pos_x	= map_pos_x;

#if	FLAG_TILES4X4
	map_pos_x >>= 5;
	map_pos_y >>= 5;
#else
	map_pos_x >>= 4;
	map_pos_y >>= 4;
#endif

#if	FLAG_DIR_ROWS
	__tmp_tiles_offset	= mpd_farpeekw( mpd_MapsTbl, __maps_tbl_offset + ( map_pos_y << 1 ) ) + map_pos_x;
#else
	__tmp_tiles_offset	= mpd_farpeekw( mpd_MapsTbl, __maps_tbl_offset + ( map_pos_x << 1 ) ) + map_pos_y;
#endif
	__tmp_tiles_offset	+= __maps_offset;

	__mpd_fill_row_data( __mpd_get_VRAM_addr( mpd_scroll_x, mpd_scroll_y ), ROW_CHRS_CNT, __mpd_calc_skip_CHRs_cnt( u8_pos_x ) );

#endif	//FLAG_MODE_BIDIR_SCROLL
}

void	__mpd_draw_down_tiles_row()
{
#if	FLAG_MODE_BIDIR_SCROLL
	mpd_SCREEN 	tmp_scr;

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

	__mpd_fill_row_data( __mpd_get_VRAM_addr( mpd_scroll_x, mpd_scroll_y + ScrPixelsHeight ), ROW_CHRS_CNT );

#else	//FLAG_MODE_MULTIDIR_SCROLL
	u16	map_pos_x;
	u16	map_pos_y;
	u8	u8_pos_x;

	map_pos_x	= mpd_scroll_x;
	map_pos_y	= mpd_scroll_y;
	u8_pos_x	= map_pos_x;

#if	FLAG_TILES4X4
	map_pos_x >>= 5;
	map_pos_y >>= 5;
#else
	map_pos_x >>= 4;
	map_pos_y >>= 4;
#endif

#if	FLAG_DIR_ROWS
	__tmp_tiles_offset	= mpd_farpeekw( mpd_MapsTbl,  __maps_tbl_offset + ( map_pos_y << 1 ) ) + map_pos_x + __height_scr_step;
#else
	__tmp_tiles_offset	= mpd_farpeekw( mpd_MapsTbl, __maps_tbl_offset + ( map_pos_x << 1 ) ) + map_pos_y + ScrTilesHeight;
#endif
	__tmp_tiles_offset	+= __maps_offset;

	__mpd_fill_row_data( __mpd_get_VRAM_addr( mpd_scroll_x, mpd_scroll_y + ScrPixelsHeight ), ROW_CHRS_CNT, __mpd_calc_skip_CHRs_cnt( u8_pos_x ) );

#endif	//FLAG_MODE_BIDIR_SCROLL
}

#if	FLAG_MODE_MULTIDIR_SCROLL

// u8	__mpd_calc_skip_CHRs_cnt( u8 _pos<acc> )

#if	FLAG_TILES4X4
// HuC	return ( ( ( _pos >> 4 ) & 0x01 ) << 1 ) + ( ( _pos >> 3 ) & 0x01 );
#asm
	.proc ___mpd_calc_skip_CHRs_cnt.1

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

	rts

	.endp
#endasm
#else
// HuC	return ( ( _pos >> 3 ) & 0x01 );
#asm
	.proc ___mpd_calc_skip_CHRs_cnt.1

	txa

	lsr a
	lsr a
	lsr a
	and #1

	tax
	cla

	rts

	.endp
#endasm
#endif

void	__mpd_fill_column_data( u16 _vaddr, u8 _CHRs_cnt, u8 _skip_CHRs_cnt )
#else
void	__mpd_fill_column_data( u16 _vaddr, u8 _CHRs_cnt )
#endif	//FLAG_MODE_MULTIDIR_SCROLL
{
	__tmp_CHRs_cnt		= _CHRs_cnt;
	__tmp_vaddr		= _vaddr;

#if	FLAG_MODE_MULTIDIR_SCROLL
	__tmp_skip_CHRs_cnt	= _skip_CHRs_cnt;
#endif

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
	__tile_step = __map_tiles_width;
#endif	//FLAG_MODE_BIDIR_SCROLL
#endif

#asm
	; set data step

	mpd_vreg #$05

	; video_data = ( ___CR_val & ~___CR_IW_MASK ) | ___CR_IW_val

	lda ___CR_val
	sta video_data_l
	lda ___CR_val + 1
	and #~$18
	ora ___CR_IW_val + 1
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

#if	FLAG_MODE_MULTIDIR_SCROLL
void	__mpd_fill_row_data( u16 _vaddr, u8 _CHRs_cnt, u8 _skip_CHRs_cnt )
#else
void	__mpd_fill_row_data( u16 _vaddr, u8 _CHRs_cnt )
#endif	//FLAG_MODE_MULTIDIR_SCROLL
{
	__tmp_CHRs_cnt		= _CHRs_cnt;
	__tmp_vaddr		= _vaddr;

#if	FLAG_MODE_MULTIDIR_SCROLL
	__tmp_skip_CHRs_cnt	= _skip_CHRs_cnt;
#endif

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
	__tile_step = __map_tiles_height;
#endif	//FLAG_MODE_BIDIR_SCROLL
#else	//FLAG_DIR_ROWS
	__tile_step = 1;
#endif

#asm
	; set data step

	mpd_vreg #$05

	; video_data = ___CR_val & ~___CR_IW_MASK

	lda ___CR_val
	sta video_data_l
	lda ___CR_val + 1
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
#if	FLAG_TILES4X4

#if	FLAG_MODE_BIDIR_SCROLL
#asm
	__farptr _mpd_TilesScr, __bl, __si
#endasm
#else
#asm
	__farptr _mpd_Maps, __bl, __si
#endasm
#endif	//FLAG_MODE_BIDIR_SCROLL

#asm
	; switch data banks

	stw ___tmp_tiles_offset, <__ax

	call _mpd_farptr_add_offset
	jsr map_data
#endasm

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

	jsr unmap_data
#endasm
/*
	__tile_n = 0;

	// tiles cache filling
	for( __tile_ind = 0; __tile_ind < ( ( __tmp_CHRs_cnt + 3 ) >> 2 ); )
	{
#if	FLAG_MODE_BIDIR_SCROLL
		__tiles_cache[ __tile_ind++ ] = mpd_farpeekb( mpd_TilesScr, __tmp_tiles_offset + __tile_n );
#else
		__tiles_cache[ __tile_ind++ ] = mpd_farpeekb( mpd_Maps, __tmp_tiles_offset + __tile_n );
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
#endasm
#else
#asm
	__farptr _mpd_Maps, __bl, __si
#endasm
#endif	//FLAG_MODE_BIDIR_SCROLL

#asm
	; switch data banks

	stw ___tmp_tiles_offset, <__ax

	call _mpd_farptr_add_offset
	jsr map_data
#endasm

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

	jsr unmap_data
#endasm
/*
	// blocks cache filling
	__tile_n = 0;

	for( __block_ind = 0; __block_ind < ( ( __tmp_CHRs_cnt + 1 ) >> 1 ); )
	{
#if	FLAG_MODE_BIDIR_SCROLL
		__blocks_cache[ __block_ind++ ] = mpd_farpeekb( mpd_TilesScr, __tmp_tiles_offset + __tile_n );
#else
		__blocks_cache[ __block_ind++ ] = mpd_farpeekb( mpd_Maps, __tmp_tiles_offset + __tile_n );
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

//	u16	tiles_offset;	// mpd_cx
//	u8	tile_id;	// mpd_dx

				// _x = __cx
				// _y = __dx

//	u8	block_pos_x;	// mpd_ax.l
//	u8	block_pos_y;	// mpd_ax.h
//	u8	CHR_pos_x;	// mpd_bx.l
//	u8	CHR_pos_y;	// mpd_bx.h

#if	FLAG_MODE_BIDIR_SCROLL		/* !!! */

	mpd_SCREEN	tmp_scr;
//	u16		scr_offs;	// mpd_ex

//	s16		dx;		// mpd_ax.ws
//	s16		dy;		// mpd_bx.ws
#endif

#asm
.ifdef MPD_DEBUG
#endasm
	DBG_BORDER_COLOR_GET_TILE_PROP
#asm
.endif
#endasm

#if	FLAG_MODE_BIDIR_SCROLL		/* !!! */

	mpd_ex	= __scr_offset;

	if( !__horiz_dir_pos && __vert_dir_pos )
	{
		mpd_bx.ws = _y - ( ScrPixelsHeight - __vert_dir_pos );

		if( mpd_bx.ws >= 0 )
		{
			__mpd_get_screen_data( mpd_get_adj_screen( &mpd_curr_scr, ADJ_SCR_DOWN ), &tmp_scr );
			mpd_ex	= tmp_scr.scr_ind * __c_scr_tiles_size;
			_y	= mpd_bx.ws;
		}
		else
		{
			_y += __vert_dir_pos;
		}
	}
	else
	if( !__vert_dir_pos && __horiz_dir_pos )
	{
		mpd_ax.ws = _x - ( ScrPixelsWidth - __horiz_dir_pos );

		if( mpd_ax.ws >= 0 )
		{
			__mpd_get_screen_data( mpd_get_adj_screen( &mpd_curr_scr, ADJ_SCR_RIGHT ), &tmp_scr );
			mpd_ex	= tmp_scr.scr_ind * __c_scr_tiles_size;
			_x	= mpd_ax.ws;
		}
		else
		{
			_x += __horiz_dir_pos;
		}
	}
#endif

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
	sta _mpd_ax	; block_pos_x
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
	sta _mpd_ax + 1	; block_pos_y
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
	sta _mpd_bx	; CHR_pos_x
	tya

	__lsrw

	tay
	txa
	and #1
	sta _mpd_ax	; block_pos_x
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
	sta _mpd_bx + 1	; CHR_pos_y
	tya

	__lsrw
	
	tay
	txa
	and #1
	sta _mpd_ax + 1	; block_pos_y
	tya
	
	__lsrw

	stx <__dl
	sta <__dh	; _y
#endasm
#endif	
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
	sta _mpd_bx	; CHR_pos_x
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
	sta _mpd_bx + 1	; CHR_pos_y
	tya

	__lsrw

	stx <__dl
	sta <__dh	; _y
#endasm
#endif
#endif

#if	FLAG_MODE_MULTIDIR_SCROLL	/* !!! */

#if	FLAG_DIR_ROWS
//	tiles_offset	= mpd_farpeekw( mpd_MapsTbl, __maps_tbl_offset + ( _y << 1 ) ) + _x;
#asm
	__farptr _mpd_MapsTbl, __bl, __si

	; __ax = __maps_tbl_offset + ( _y << 1 )

	lda <__dl
	asl a
	sta <__al
	lda <__dh
	rol a
	sta <__ah					; __ax = _y << 1

	mpd_add_word_to_word ___maps_tbl_offset, <__ax	; __maps_tbl_offset + ( _y << 1 )

	call _mpd_farpeekw.2

	sax

	clc
	adc <__cl		; _x.l
	sta _mpd_cx		; tiles_offset.l

	sax

	adc <__ch		; _x.h
	sta _mpd_cx + 1		; tiles_offset.h
#endasm
#else
//	tiles_offset	= mpd_farpeekw( mpd_MapsTbl, __maps_tbl_offset + ( _x << 1 ) ) + _y;
#asm
	__farptr _mpd_MapsTbl, __bl, __si

	; __ax = __maps_tbl_offset + ( _x << 1 )

	lda <__cl
	asl a
	sta <__al
	lda <__ch
	rol a
	sta <__ah					; __ax = _x << 1

	mpd_add_word_to_word ___maps_tbl_offset, <__ax	; __maps_tbl_offset + ( _x << 1 )

	call _mpd_farpeekw.2

	sax

	clc
	adc <__dl		; _y.l
	sta _mpd_cx		; tiles_offset.l

	sax

	adc <__dh		; _y.h
	sta _mpd_cx + 1		; tiles_offset.h
#endasm
#endif
//	tiles_offset	+= __maps_offset;

//	tile_id		= mpd_farpeekb( mpd_Maps, tiles_offset );
#asm
	mpd_add_word_to_word ___maps_offset, _mpd_cx

	__farptr _mpd_Maps, __bl, __si

	stw _mpd_cx, <__ax	; tiles_offset

	call _mpd_farptr_add_offset

	lda	<__bl
	tam	#3

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
	sta _mpd_cx

	ldy #1
	lda [<__ax], y

	adc <__ch
	sta _mpd_cx + 1		; tiles_offset = __scr_tiles_width_tbl[ _y ] + _x
#endasm
#else
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
	sta _mpd_cx

	ldy #1
	lda [<__ax], y

	adc <__dh
	sta _mpd_cx + 1		; tiles_offset = __scr_tiles_height_tbl[ _x ] + _y
#endasm
#endif

#if	FLAG_MODE_BIDIR_SCROLL
//	tile_id		= mpd_farpeekb( mpd_TilesScr, mpd_ex(scr_offs) + tiles_offset );
#asm
	__farptr _mpd_TilesScr, __bl, __si

	mpd_add_word_to_word2 _mpd_ex, _mpd_cx, <__ax	; scr_offs + tiles_offset

	call _mpd_farptr_add_offset

	lda	<__bl
	tam	#3

	lda [<__si]
	tax					; x = tile_id
#endasm
#else
//	tile_id		= mpd_farpeekb( mpd_TilesScr, __scr_offset + tiles_offset );
#asm
	__farptr _mpd_TilesScr, __bl, __si

	mpd_add_word_to_word2 ___scr_offset, _mpd_cx, <__ax	; __scr_offset + tiles_offset

	call _mpd_farptr_add_offset

	lda	<__bl
	tam	#3

	lda [<__si]
	tax					; x = tile_id
#endasm
#endif
#endif

#if	FLAG_TILES4X4
//	tile_id		= mpd_farpeekb( mpd_Tiles, __tiles_offset + ( tile_id << 2 ) + ( block_pos_y << 1 ) + block_pos_x );
#asm
	__farptr _mpd_Tiles, __bl, __si

	stw ___tiles_offset, <__ax		; __tiles_offset

	lda _mpd_ax + 1				; block_pos_y << 1
	asl a

	clc
	adc _mpd_ax				; block_pos_x

	mpd_add_a_to_word <__ax			; __tiles_offset + ( block_pos_y << 1 ) + block_pos_x

	cla
	sta _mpd_dx + 1

	txa					; a = tile_id
	asl a
	rol _mpd_dx + 1
	asl a
	rol _mpd_dx + 1
	sta _mpd_dx				; tile_id << 2

	mpd_add_word_to_word _mpd_dx, <__ax	; __tiles_offset + ( tile_id << 2 ) + ( block_pos_y << 1 ) + block_pos_x

	call _mpd_farptr_add_offset

	lda	<__bl
	tam	#3

	lda [<__si]
	tax					; x = tile_id
#endasm
#endif

#if	FLAG_PROP_ID_PER_BLOCK
//	tile_id		= mpd_farpeekb( mpd_Props, __props_offset + tile_id );
#asm
	__farptr _mpd_Props, __bl, __si

	stw ___props_offset, <__ax

	txa					; a = tile_id
	mpd_add_a_to_word <__ax			; __props_offset + tile_id

	call _mpd_farptr_add_offset

	lda	<__bl
	tam	#3

	lda [<__si]
	sta _mpd_dx				; tile_id
#endasm
#else	//FLAG_PROP_ID_PER_CHR
//	tile_id		= mpd_farpeekb( mpd_Props, __props_offset + ( tile_id << 2 ) + ( CHR_pos_y << 1 ) + CHR_pos_x );
#asm
	__farptr _mpd_Props, __bl, __si

	stw ___props_offset, <__ax		; __props_offset

	lda _mpd_bx + 1				; CHR_pos_y << 1
	asl a

	clc
	adc _mpd_bx				; CHR_pos_x

	mpd_add_a_to_word <__ax			; __props_offset + ( CHR_pos_y << 1 ) + CHR_pos_x

	cla
	sta _mpd_dx + 1

	txa					; a = tile_id
	asl a
	rol _mpd_dx + 1
	asl a
	rol _mpd_dx + 1
	sta _mpd_dx				; tile_id << 2

	mpd_add_word_to_word _mpd_dx, <__ax	; __props_offset + ( tile_id << 2 ) + ( CHR_pos_y << 1 ) + CHR_pos_x

	call _mpd_farptr_add_offset

	lda	<__bl
	tam	#3

	lda [<__si]
	sta _mpd_dx				; tile_id
#endasm
#endif

#asm
.ifdef MPD_DEBUG
#endasm
	DBG_BORDER_COLOR_RESET
#asm
.endif
#endasm
	return mpd_dx.l;//tile_id;
}