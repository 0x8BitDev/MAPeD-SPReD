//######################################################################################################
//
// This file is a part of the MAPeD-PCE Copyright 2017-2022 0x8BitDev ( MIT license. See LICENSE.txt )
// Desc: It contains a meta-sprite rendering routines using a local HuC's SATB
//
//######################################################################################################

/*/	SPD-render v0.1

NOTE:	The SPReD-PCE exports both meta-sprites and simple sprites (16x16,16x32,16x64,32x16,32x32,32x64). The CGX/CGY
	flags are automatically applied to exported sprites. So you don't need to configure anything in your HuC program.

*SG - sprite graphics data

The SPD sprite/meta-sprite render works as an extension to the HuC local SATB. So you can combine it with the HuC`s sprite functions.
There are two data types the SPD-render works with.

1. PACKED sprites data. All exported SG data are stored in a single file. It were packed in the SPReD-PCE before exporting.
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
The main logic is:

	// SPD-render initialization.
	spd_init();	

	// Initialization of exported sprite set.
	{
		// Load palette in a usual way.
		load_palette( 16 + <EXPORTED_NAME>_PALETTE_SLOT, <exported_name>_palette, <exported_name>_palette_size >> 4 );

		// Set up exported sprite set with SG data array and VRAM address to load SG data to.
		// NOTE: You can combine any number of exported sprite sets in your program.
		//	 Call the 'spd_sprite_params' to switch between them.
		spd_sprite_params( <exported_name>_SG_arr, <EXPORTED_NAME>_SPR_VADDR, 0 );
	}

	// HuC's SATB initialization.
	init_satb();

	// Here you can use HuC's sprite calls...

	// SPD calls
	{
		// Now you can set a local SATB position to push your exported sprites/meta-sprites to.
		// NOTE: The SATB position will be automatically incremented with each call to 'spd_SATB_push_sprite' (!)
		spd_SATB_set_pos( <SATB_pos[0...63]> );

		// There are two ways to show a sprite:
		// 1. By index in a sprite array. Suitable for animation sequences.
		// (see exported .h file for generated constants)
		spd_SATB_push_sprite( <exported_name>_frames_data, _ind, _x, _y );

		// 2. By sprite data pointer.
		// (see exported .h file for generated data)
		spd_SATB_push_sprite( <animation_name>_frame, _x, _y );

		// NOTE: SG data will be automatically loaded once to VRAM at first call to 'spd_SATB_push_sprite' (!)
		// NOTE: If meta-sprite does not fit into SATB, it will be ignored!
		// NOTE: 'spd_SATB_push_sprite' returns: 1-Ok; 0-SATB overflow
	}

	// Then call 'satb_update' to push your sprite data to VRAM SAT.
	// NOTE: After pushing all sprites, `spd_SATB_get_pos()` returns the number of sprites in SATB.
	satb_update( spd_SATB_get_pos() );


	// NOTE: As mentioned before, you can combine the SPD calls with the HuC ones.
	//	 For example, you can do the following for a simple sprite:

	spd_SATB_set_pos( 0 );

	spd_SATB_push_sprite( my_sprite_16x32, init_x, init_y );

	...

	spr_set( 0 );
	spr_x( new_x );
	spr_y( new_y );

	// WARNING: Take into account your sprite origin! It can be configured out of (0, 0) in the SPReD-PCE.


2. UNPACKED sprites data. All exported SG data are stored in separate files. It were not packed in the SPReD-PCE.
Unpacked data are suitable for dynamic SG data, that can be loaded to VRAM on VBLANK to save video memory. It`s
suitable when you have a lot of animations that don`t fit into VRAM, like in fighting games.
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
The main logic is:

	// SPD-render initialization.
	spd_init();	

	// Initialization of exported sprite set.
	{
		// Load palette in a usual way.
		load_palette( 16 + <EXPORTED_NAME>_PALETTE_SLOT, <exported_name>_palette, <exported_name>_palette_size >> 4 );

		// Set up exported sprite set with SG data array and VRAM address to load SG data to.
#if	DEF_SG_DBL_BUFF
		// NOTE: Using the `SPD_FLAG_DBL_BUFF` flag means double-buffering for sprite graphics.
		//	 It costs x2 of dynamic SG data in VRAM, but glitches free. You have to compare the results
		//	 of using 'SPD_FLAG_DBL_BUFF' and 'SPD_FLAG_PEND_SG_DATA' and decide which is better in your case.
		spd_sprite_params( <exported_name>_SG_arr, <EXPORTED_NAME>_SPR_VADDR, SPD_FLAG_DBL_BUFF );

		// Set the second VRAM address for double-buffering (SPD_FLAG_DBL_BUFF).
		spd_dbl_buff_VRAM_addr( VADDR_dbl_buff );
#else
		// NOTE: Using the `SPD_FLAG_PEND_SG_DATA` flag means that SG data will not be loaded
		//	 to VRAM automatically. You should do that manually on VBLANK.
		spd_sprite_params( <exported_name>_SG_arr, <EXPORTED_NAME>_SPR_VADDR, SPD_FLAG_PEND_SG_DATA );

		// Set pointers to SG data for delayed use (SPD_FLAG_PEND_SG_DATA).
		spd_SG_data_params( &SG_DATA_SRC_ADDR, &SG_DATA_SRC_BANK, &SG_DATA_DST_ADDR, &SG_DATA_LEN );
#endif
	}

	// HuC's SATB initialization.
	init_satb();

	// Here you can use HuC's sprite calls...

	// SPD calls
	{
		// Now you can set a local SATB position to push your exported sprites/meta-sprites to.
		// NOTE: The SATB position will be automatically incremented with each call to 'spd_SATB_push_sprite' (!)
		spd_SATB_set_pos( <SATB_pos[0...63]> );

		// There are two ways to show a sprite:
		// 1. By index in a sprite array. Suitable for animation sequences.
		// (see exported .h file for generated constants)
		spd_SATB_push_sprite( <exported_name>_frames_data, _ind, _x, _y );

		// 2. By sprite data pointer.
		// (see exported .h file for generated data)
		spd_SATB_push_sprite( <animation_name>_frame, _x, _y );

		// NOTE: If meta-sprite does not fit into SATB, it will be ignored!
		// NOTE: 'spd_SATB_push_sprite' returns: 1-Ok; 0-SATB overflow
	}

	// 'VRAM-SATB Transfer Auto-Repeat on VBLANK' (DCR ROF-$10) is enabled by default in HuC at startup, so we skip this step.

	// Main loop
	for( ;; )
	{
		// Update your sprite animation
		update_frame( &test_anim );

		// Here you can call the
		reset_satb();// to clear all the SATB data
		OR
		spd_SATB_clear_from( <SATB_pos[0...63]> );// to save sprites before 'SATB_pos', and clear memory after to avoid graphical glitches with variable sized meta-sprites

		// Set the SATB position to push your sprite to.
		spd_SATB_set_pos( <SATB_pos[0...63]> );

		// Push your sprite to the local RAM SATB.
		spd_SATB_push_sprite( <exported_name>_frames_data, test_anim.start_frame + test_anim.curr_frame, _x, _y );

		// Load all sprites to VRAM SAT.
		satb_update( spd_SATB_get_pos() );

		vsync();

#if	!DEF_SG_DBL_BUFF
		// The HuC doesn't allow to handle VBLANK directly, so we use 'vsync' instead for sprite graphics and inner SATB synchronization.
		// This may cause some graphical glitches at the upper part of the screen.

		// Delayed copying of SG data to VRAM to synchronize it with the inner SATB
		spd_copy_SG_data_to_VRAM( SG_DATA_SRC_ADDR, SG_DATA_SRC_BANK, SG_DATA_DST_ADDR, SG_DATA_LEN );
#endif
	}
	
	That`s it! :)
/*/

/* SPD flag(s) */

// Copies SG data parameters: src_addr/src_bank/vram_addr/len for delayed use on VBLANK;
// it's suitable for sprites with dynamic SG data like in fighting games.
// NOTE: THIS FLAG CAN BE USED WITH UNPACKED SPRITES ONLY! WHERE EACH SPRITE HAS A SEPARATE SG DATA!
const unsigned char SPD_FLAG_PEND_SG_DATA	= 0x01;

// Double-buffering. It costs x2 of dynamic SG data in VRAM, but glitches free.
// You have to compare the results of using 'SPD_FLAG_DBL_BUFF' and 'SPD_FLAG_PEND_SG_DATA' and decide which is better for you.
// Thanks to elmer/pcengine.proboards.com for suggesting this mode.
// NOTE: THIS FLAG CAN BE USED WITH UNPACKED SPRITES ONLY! WHERE EACH SPRITE HAS A SEPARATE SG DATA!
const unsigned char SPD_FLAG_DBL_BUFF		= 0x02;

/* main routines */

/* void	spd_init() */
#pragma fastcall spd_init()

/* void spd_sprite_params( far void* _SG_data, word _vaddr, byte _flags ) */
#pragma fastcall spd_sprite_params( farptr __bl:__si, word __dx, byte __al )

/* void	spd_SATB_set_pos( byte _pos ); _pos: 0-63 */
#pragma fastcall spd_SATB_set_pos( byte __al )

/* char	spd_SATB_get_pos() */
#pragma fastcall spd_SATB_get_pos()

/* void spd_SATB_clear_from( byte _pos ); _pos: 0-63 */
#pragma fastcall spd_SATB_clear_from( byte __al )

/* char	spd_SATB_push_sprite( far void* _addr,  byte _ind, word _pos_x, word _pos_y )
   OUT: 1-Ok!, 0-SATB overflow
*/
#pragma fastcall spd_SATB_push_sprite( farptr __bl:__si, byte __dl, word __ax, word __cx )

/* char	spd_SATB_push_sprite( far void* _addr, word _pos_x, word _pos_y )
   OUT: 1-Ok!, 0-SATB overflow
*/
#pragma fastcall spd_SATB_push_sprite( farptr __bl:__si, word __ax, word __cx )

/* void spd_SG_data_params( word _src_addr, word _src_bank, word _dst_addr, word _len ) */
#pragma fastcall spd_SG_data_params( word __ax, word __bx, word __cx, word __dx )

/* void spd_copy_SG_data_to_VRAM( word _src_addr, word _src_bank, word _dst_addr, word _len ) */
#pragma fastcall spd_copy_SG_data_to_VRAM( word __ax, word __bx, word __cx, word __dx )

/* void spd_dbl_buff_VRAM_addr( word _vram_addr ) */
#pragma fastcall spd_dbl_buff_VRAM_addr( word __ax )


#asm

; macros(es)

; val -> &((word)(*zp))
	.macro stw_zpii ; \1 - val, \2 - zp
	cly
	lda low_byte \1
	sta [\2], y
	iny
	lda high_byte \1
	sta [\2], y
	.endm

; (word(*zp)) -> addr
	.macro stw_zpii_rev ; \1 - zp, \2 - addr
	lda [<\1], y
	sta \2
	iny
	lda [<\1], y
	sta \2 + 1
	.endm

; \1 *= 2
	.macro mul2_word
	lda low_byte \1
	asl a	
	rol high_byte \1
	sta low_byte \1
	.endm	

; \1 *= 4
	.macro mul4_word
	lda low_byte \1
	asl a	
	rol high_byte \1
	asl a	
	rol high_byte \1
	sta low_byte \1
	.endm	

; \1 *= 8
	.macro mul8_word
	lda low_byte \1
	asl a	
	rol high_byte \1
	asl a	
	rol high_byte \1
	asl a	
	rol high_byte \1
	sta low_byte \1
	.endm	

; \1 /= 2
	.macro div2_word
	lda low_byte \1
	lsr high_byte \1
	ror a
	sta low_byte \1
	.endm

; \1 /= 8
	.macro div8_word
	lda low_byte \1
	lsr high_byte \1
	ror a
	lsr high_byte \1
	ror a
	lsr high_byte \1
	ror a
	sta low_byte \1
	.endm

; \1 /= 32
	.macro div32_word
	lda low_byte \1
	lsr high_byte \1
	ror a
	lsr high_byte \1
	ror a
	lsr high_byte \1
	ror a
	lsr high_byte \1
	ror a
	lsr high_byte \1
	ror a
	sta low_byte \1
	.endm

; -\1 /= 32
	.macro div32_neg_word
	lda low_byte \1
	sec
	ror high_byte \1
	ror a
	sec
	ror high_byte \1
	ror a
	sec
	ror high_byte \1
	ror a
	sec
	ror high_byte \1
	ror a
	sec
	ror high_byte \1
	ror a
	sta low_byte \1
	.endm

; \2 = \1 + \2
	.macro add_word_to_word
	clc
	lda low_byte \1
	adc low_byte \2
	sta low_byte \2
	lda high_byte \1
	adc high_byte \2
	sta high_byte \2
	.endm

; \2 = \1 - \2
	.macro sub_word_from_word
	lda low_byte \1
	sec
	sbc low_byte \2
	sta low_byte \2
	lda high_byte \1
	sbc high_byte \2
	sta high_byte \2
	.endm

; \1 += A
	.macro add_a_to_word
	clc
	adc low_byte \1
	sta low_byte \1
	cla
	adc high_byte \1
	sta high_byte \1
	.endm

; \1 -= \2 [0..255]
	.macro sub_N_from_word
	sec
	lda low_byte \1
	sbc \2
	sta low_byte \1
	lda high_byte \1
	sbc #$00
	sta high_byte \1
	.endm

	.macro get_SATB_flag
	lda <__SATB_flags
	and #\1
	.endm

	.macro inner_flags_dbl_buff_state
	lda <__inner_flags
	and #INNER_FLAGS_DBL_BUFF
	.endm

	.zp

SPD_FLAG_PEND_SG_DATA	= $01
SPD_FLAG_DBL_BUFF	= $02

SATB_SIZE	= 64

__SATB	= satb	; satb - HuC`s local satb

__SATB_pos	.ds 1
__SATB_flags	.ds 1
__last_SG_bank	.ds 1

INNER_FLAGS_DBL_BUFF	= %00000001

__inner_flags	.ds 1

; *** sprite data ***

__spr_VADDR	.ds 2	; sprite`s SG data address in VRAM
__spr_VADDR_dbl	.ds 2	; VRAM address for double-buffering
__spr_SG_offset
		.ds 2	; SG offset for pattern index correction when double-buffering is active

__spr_SG_data_addr
		.ds 2
__spr_SG_data_bank
		.ds 1

__spr_pos_x:	.ds 2
__spr_pos_y:	.ds 2

	.bss

__SG_DATA_SRC_ADDR	.ds 2
__SG_DATA_SRC_BANK	.ds 2
__SG_DATA_DST_ADDR	.ds 2
__SG_DATA_LEN		.ds 2

__TII:		.ds 1	; $73 tii
__bsrci:	.ds 2
__bdsti:	.ds 2
__bleni:	.ds 2
__tiirts	.ds 1	; $60 rts

	.code

; *** farptr += offset ***
;
; IN:
;
; __dx - offset
; __bl - bank number
; __si - address
;
_spd_farptr_add_offset:

	; add an offset

	clc     
	lda <__dx
	adc <__si
	sta <__si
	lda <__dx+1
	adc <__si+1

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
	and #$9f
	sta <__si+1	
	
	rts

_spd_init:

	cla
	sta <__inner_flags

	; init TII

	lda #$73
	sta __TII
	lda #$60
	sta __tiirts

	rts

;/* spd_sprite_params( farptr __bl:__si / SG_data, word __dx / vaddr, byte __al / flags ) */
;
_spd_sprite_params.3:

	stw <__si, <__spr_SG_data_addr
	lda <__bl
	sta <__spr_SG_data_bank

	stw <__dx, <__spr_VADDR

	lda <__al
	sta <__SATB_flags

	lda #$ff
	sta <__last_SG_bank

	rts

;/* spd_dbl_buff_VRAM_addr( word __ax / vram_addr ) */
;
_spd_dbl_buff_VRAM_addr.1:

	stw <__ax, <__spr_VADDR_dbl
	stw <__spr_VADDR, <__bx

	sub_word_from_word <__ax, <__bx

	lda <__bh
	bpl .pos_val

	; negative val

	; __spr_SG_offset = __bx / 32

	div32_neg_word <__bx
	stw <__bx, <__spr_SG_offset

	rts

	; positive val

.pos_val:

	; __spr_SG_offset = __bx / 32

	div32_word <__bx
	stw <__bx, <__spr_SG_offset

	rts

;/* spd_SATB_set_pos( byte __al / pos ) */
;
_spd_SATB_set_pos.1:

	lda <__al
	and #SATB_SIZE - 1		; clamp to 0-63
	sta __SATB_pos

	rts

_spd_SATB_get_pos:

	lda <__SATB_pos
	tax
	cla

	rts

;/* spd_SATB_clear_from( byte __al / pos ) */
;
_spd_SATB_clear_from.1:

	lda <__al
	and #SATB_SIZE - 1		; clamp to 0-63
	sta __bsrci
	tax

	; __bsrci

	stz __bsrci + 1
	mul8_word __bsrci

	add_word_to_word #__SATB, __bsrci

	; clear the first byte

	stw __bsrci, <__ax
	cly
	cla
	sta [<__ax], y

	; __bdsti

	stw __bsrci, __bdsti

	lda #$01
	add_a_to_word __bdsti

	; __bleni

	txa
	sec
	sbc #SATB_SIZE
	eor #$ff
	inc a

	sta __bleni
	stz __bleni + 1

	mul8_word __bleni

	sub_N_from_word __bleni, #$01

	jmp __TII

;// spd_SATB_push_sprite( farptr __bl:__si / addr, byte __dl / index, word __ax / x_pos, word __cx / y_pos )
;
_spd_SATB_push_sprite.4:

	; offset x6 -> sizeof( spd_ANM_FRAME )

	lda <__dl
	asl a
	asl a
	sta <__dh
	lda <__dl
	asl a
	clc
	adc <__dh
	sta <__dl
	stz <__dh

	jsr _spd_farptr_add_offset

;// spd_SATB_push_sprite( farptr __bl:__si / addr, word __ax / x_pos, word __cx / y_pos )
;
_spd_SATB_push_sprite.3:

	; XY coordinates correction

	lda #32
	clc
	adc <__ax
	sta <__spr_pos_x
	cla
	adc <__ax + 1
	sta <__spr_pos_x + 1

	lda #64
	clc
	adc <__cx
	sta <__spr_pos_y
	cla
	adc <__cx + 1
	sta <__spr_pos_y + 1

	jsr map_data			; map spd_ANM_FRAME data

	; get meta-sprite address and bank

	cly				; meta-sprite address offset
	stw_zpii_rev __si, <__cx

	ldy #$02			; meta-sprite bank offset
	lda [<__si], y
	sta <__al			; __al - bank

	; get data length

	iny				; meta-sprite length offset
	stw_zpii_rev __si, __bleni

	iny				; meta-sprite SG bank offset
	lda [<__si], y
	tay				; Y - SG bank index

	jsr unmap_data

	stw <__cx, <__si		; matasprite address
	lda <__al
	sta <__bl			; meta-sprite bank

	; check SATB overflow

	stw __bleni, <__ax
	div8_word <__ax			; __al - number of sprites
	clc
	adc <__SATB_pos
	cmp #SATB_SIZE + 1
	bcc .push_sprite

	; SATB overflow

	clx
	cla

	rts

.push_sprite:

	jsr map_data			; map meta-sprite data

	; _bsrci = meta-sprite address

	stw <__si, __bsrci

	; calc SATB address to copy sprite data to
	; _bdsti = __SATB + ( __SATB_pos * 8 )

	lda __SATB_pos
	sta <__cl
	stz <__ch
	mul8_word <__cx			; __cx - SATB offset in bytes

	add_word_to_word #__SATB, <__cx
	stw <__cx, __bdsti

	; copy meta-sprite data to the local SATB

	jsr __TII

	jsr unmap_data

	phy				; Y - SG bank index
;--- DBL-BUFF ---
	; check double-buffering state

	get_SATB_flag SPD_FLAG_DBL_BUFF
	beq .use_main_buff		; no double-buffering

	; double-buffering is enabled

	inner_flags_dbl_buff_state
	beq .use_main_buff
	
	jmp __attr_loop_XY_IND

.use_main_buff:

	jmp __attr_loop_XY

_push_SG_data:

	get_SATB_flag SPD_FLAG_DBL_BUFF
	bne .ignore_SG_data_checking	; when double-buffering is enabled we should ignore SG data checking to avoid glitches
;--- DBL-BUFF ---
	pla				; A - SG bank index

	; check if SG data already loaded to VRAM

	cmp __last_SG_bank
	bne .load_SG_data

	; SG data already loaded

	ldx #1
	cla

	rts
;--- DBL-BUFF ---
.ignore_SG_data_checking:

	pla
;--- DBL-BUFF ---
	; load SG data to VRAM

.load_SG_data:

	sta <__last_SG_bank

	; __dx = SG bank index x6 ( .word <data_length>, chrN, bank(chrN) )

	tax
	sta <__dl
	stz <__dh
	mul4_word <__dx

	txa
	sta <__cl
	stz <__ch
	mul2_word <__cx

	add_word_to_word <__cx, <__dx	

	stw __spr_SG_data_addr, <__si
	lda __spr_SG_data_bank
	sta <__bl

	jsr _spd_farptr_add_offset

	jsr map_data			; map SG data array

	; __cx = SG data length

	cly
	lda [<__si], y
	sta <__cl
	iny
	lda [<__si], y
	sta <__ch
	div2_word <__cx

	; __ax = SG data address

	iny
	lda [<__si], y
	sta <__al
	iny
	lda [<__si], y
	sta <__ah

	; __bl = SG data bank

	iny
	lda [<__si], y
	tax

	jsr unmap_data

	stx <__bl

	; __si = __ax

	stw <__ax, <__si

	; __di = VADDR
;--- DBL-BUFF ---
	get_SATB_flag SPD_FLAG_DBL_BUFF
	beq .cont_load_SG_data1		; no double-buffering

	; toggle the double-buffering flag

	lda <__inner_flags
	tax				; save the last state before modifying
	eor #INNER_FLAGS_DBL_BUFF
	sta <__inner_flags

	; check which buffer we'll use

	txa				; restore the previous state
	and #INNER_FLAGS_DBL_BUFF
	beq .cont_load_SG_data1		; use the main VADDR

	stw <__spr_VADDR_dbl, <__di	; use the alternative VADDR

	bra .cont_load_SG_data2

.cont_load_SG_data1:
;--- DBL-BUFF ---
	stw <__spr_VADDR, <__di

.cont_load_SG_data2:

	get_SATB_flag SPD_FLAG_PEND_SG_DATA
	bne .copy_SG_data_params

	jsr load_vram

	ldx #1
	cla
	
	rts

.copy_SG_data_params:

	; copy SG data parameters for delayed use on VBLANK

	stw __SG_DATA_SRC_ADDR, <__ax
	stw_zpii <__si, <__ax

	stw __SG_DATA_SRC_BANK, <__ax
	stw_zpii <__bx, <__ax

	stw __SG_DATA_DST_ADDR, <__ax
	stw_zpii <__di, <__ax

	stw __SG_DATA_LEN, <__ax
	stw_zpii <__cx, <__ax

	ldx #1
	cla
	
	rts

	; copy a sprite attributes and modify XY coordinates

__attr_loop_XY:

	cly

	; attr.Y += __spr_pos_y

	lda [<__cx], y
	tax
	iny
	lda [<__cx], y

	sax
	clc
	adc <__spr_pos_y
	dey
	sta [<__cx], y
	txa
	adc <__spr_pos_y + 1
	iny
	sta [<__cx], y

	iny	

	; attr.X += __spr_pos_x

	lda [<__cx], y
	tax
	iny
	lda [<__cx], y

	sax
	clc
	adc <__spr_pos_x
	dey
	sta [<__cx], y
	txa
	adc <__spr_pos_x + 1
	iny
	sta [<__cx], y

	; calc next attr offset

	lda #$08
	add_a_to_word <__cx

	inc __SATB_pos

	dec <__al			; __al - number of attrs
	bne __attr_loop_XY

	jmp _push_SG_data

	; copy a sprite attributes and modify XY coordinates and the sprite pattern code
	; this routine is used when double-buffering is active

__attr_loop_XY_IND:

	cly

	; attr.Y += __spr_pos_y

	lda [<__cx], y
	tax
	iny
	lda [<__cx], y

	sax
	clc
	adc <__spr_pos_y
	dey
	sta [<__cx], y
	txa
	adc <__spr_pos_y + 1
	iny
	sta [<__cx], y

	iny	

	; attr.X += __spr_pos_x

	lda [<__cx], y
	tax
	iny
	lda [<__cx], y

	sax
	clc
	adc <__spr_pos_x
	dey
	sta [<__cx], y
	txa
	adc <__spr_pos_x + 1
	iny
	sta [<__cx], y

	iny

	; attr.SG_ind += __spr_SG_offset

	lda [<__cx], y
	tax
	iny
	lda [<__cx], y

	sax
	clc
	adc <__spr_SG_offset
	dey
	sta [<__cx], y
	txa
	adc <__spr_SG_offset + 1
	iny
	sta [<__cx], y

	; calc next attr offset

	lda #$08
	add_a_to_word <__cx

	inc __SATB_pos

	dec <__al			; __al - number of attrs
	bne __attr_loop_XY_IND

	jmp _push_SG_data

;// spd_SG_data_params( word __ax / src_addr, word __bx / src_bank, word __cx / dst_addr, word __dx / len ) */
;
_spd_SG_data_params.4:

	stw <__ax, __SG_DATA_SRC_ADDR
	stw <__bx, __SG_DATA_SRC_BANK
	stw <__cx, __SG_DATA_DST_ADDR
	stw <__dx, __SG_DATA_LEN

	rts

;// spd_copy_SG_data_to_VRAM( word __ax / src_addr, word __bx / src_bank, word __cx / dst_addr, word __dx / len ) */
;
_spd_copy_SG_data_to_VRAM.4:

	stw <__ax, <__si
	stw <__cx, <__di
	stw <__dx, <__cx

	jmp load_vram

#endasm
