//######################################################################################################
//
// This file is a part of the MAPeD-PCE Copyright 2017-2022 0x8BitDev ( MIT license. See LICENSE.txt )
// Desc: It contains a meta-sprite rendering routines using a local HuC's SATB
//
//######################################################################################################

// external HuC data used by SPD library
#asm
__SATB		= satb			; satb - HuC`s local satb
__VRAM_SAT_ADDR	= $7f00			; VRAM-SAT address
__TIA		= ram_hdwr_tia
__TIA_SRC	= ram_hdwr_tia_src
__TIA_DST	= ram_hdwr_tia_dest
__TIA_LEN	= ram_hdwr_tia_size
__TIA_RTS	= ram_hdwr_tia_rts
#endasm

/*/	SPD-render v0.6
History:

v0.6
2022.07.26 - added LUT for sprite/SG data indexing +minor changes (-100 cycles); removed unused math macros
2022.07.22 - changed 'General information' items 5, 6, 9
2022.07.19 - added 'General information' section
2022.07.18 - added 'spd_' prefix to macros
2022.07.18 - added 'spd_SATB_to_VRAM()' and 'spd_SATB_to_VRAM( _spr_cnt )'
2022.07.17 - optimized 'spd_SATB_clear_from', 'spd_dbl_buff_VRAM_addr', fixed XY correction in the 'spd_SATB_set_sprite_LT'
2022.07.16 - added functions for simple sprites: spd_set_palette_LT( ind ), spd_get_palette_LT(), spd_set_pri_LT( SPD_SPR_PRI_HIGH/SPD_SPR_PRI_LOW ), spd_set_x_LT( X ), spd_get_x_LT(), spd_set_y_LT( Y ), spd_get_y_LT(), spd_show_LT(), spd_hide_LT()
2022.07.16 - 'spd_SATB_push_simple_sprite' renamed to 'spd_SATB_set_sprite_LT'
2022.07-14 - the library code has been adapted to the new ASM data format; old projects need to be re-exported (!)
2022.07.14 - border color procedures replaced with macros
2022.07.13 - added a little note about using the 'SPD_DEBUG' flag
2022.07.12 - added 'SPD_TII_ATTR_XY' flag which speeds up transformation of meta-sprite attributes a bit, but may delay interrupts (!)
2022.07.12 - added cyan border color as attributes transformation indicator
2022.07.11 - added 'SPD_SG_BANK_INIT_VAL' as initial value for the '_last_bank_ind' in 'spd_sprite_params'
2022.07.11 - extremely simplified double-buffering logic for meta-sprites
2022.07.11 - the double-buffer index in 'spd_get_dbl_buff_ind' and the second argument in 'spd_dbl_buff_VRAM_addr' changed to a byte value
2022.07.08 - the type of the second argument for 'spd_copy_SG_data_to_VRAM' and 'spd_SG_data_params' has changed, now '_src_bank' is a byte and therefore a pointer to a byte
2022.07.08 - changed a sprite index register __dl -> __bh in 'spd_SATB_push_sprite' and 'spd_SATB_push_simple_sprite' for using HuC rand() as argument for X, Y
2022.07.08 - added 'spd_SATB_push_simple_sprite' for simple sprites, it takes a little less processing time compared to 'spd_SATB_push_sprite' and allows you to use sprite offset values unlike HuC sprite functions

v0.5
2022.07.03 - asm routines are wrapped in .proc/.endp
2022.07.03 - added 'spd_change_palette( _plt_ind )' function to change palette after the 'spd_SATB_push_sprite' function call
2022.06.28 - added 'spd_alt_VRAM_addr( _alt_VADDR )' function to use VRAM address other than exported one
2022.06.25 - optimized loading of SG data to VRAM for double-buffered meta-sprites; added 'SPD_DBL_BUFF_INIT_VAL' as initial value for a double-buffer index
2022.06.25 - small changes in 'SPD_DEBUG', the white border now shows how long the 'spd_SATB_push_sprite' takes
2022.06.23 - added second argument to the 'spd_dbl_buff_VRAM_addr( VADDR_dbl_buff, _dbl_buff_ind )' function and added 'spd_get_dbl_buff_ind()' function
2022.06.09 - small fix in 'SPD_DEBUG', the pink border now shows how long it takes to load graphics data to VRAM

v0.4
2022.06.08 - added debug flag 'SPD_DEBUG' that shows when gfx data is being loaded to VRAM (use Mednafen)
2022.06.08 - added flag 'SPD_SG_NEW_DATA' to the 'spd_SATB_push_sprite' function result
2022.06.07 - added fourth argument to the 'spd_sprite_params( <exported_name>_SG_arr, <EXPORTED_NAME>_SPR_VADDR, _flag, _last_bank_ind )' and also added 'spd_SG_bank_get_ind()'
2022.06.07 - changed 'spd_copy_SG_data_to_VRAM( _SG_ind )' to 'spd_copy_SG_data_to_VRAM( <exported_name>_frames_data, _spr_ind )' and added 'spd_copy_SG_data_to_VRAM( <animation_name>_frame )'
2022.06.04 - changed exported data, now '<exported_name>_PALETTE_SLOT' includes sprite palette offset (16) and '<exported_name>_palette_size' is the number of active palettes
2022.06.02 - fixed _spd_farptr_add_offset

v0.3
2022.05.03 - added 'spd_copy_SG_to_VRAM( _SG_ind )'

v0.2
2022.04.24 - optimization of '__attr_loop_XY', '__attr_loop_XY_IND' loops
2022.04.22 - added 'SPD_FLAG_IGNORE_SG'

v0.1
2022.04.22 - initial release

~~~~~~~~~~
debug info (use Mednafen):
 - pink border color - ROM-VRAM data copying
 - white border color - spd_SATB_push_sprite
 - cyan border color - attributes transformation
 
#asm
SPD_DEBUG
#endasm

[upd] v0.6
NOTE: 	After enabling you will see two border lines: pink - ROM-VRAM data copying and white/cyan - spd_SATB_push_sprite.
	Pay attention to the pink border line. The normal behaviour is when the pink line flashes sometimes (see sample
	projects). When you see a bright pink border, this means that SG data copies to VRAM each frame. If this is not
	expected behavior, then there is a bug somewhere in your program logic.

*SG - sprite graphics data


General information:
~~~~~~~~~~~~~~~~~~~~

1. The library works with both simple sprites and meta-sprites.
   The only difference is the use of the following functions: 'spd_SATB_push_sprite' for meta-sprites and 'spd_SATB_set_sprite_LT' for simple sprites.

   'spd_SATB_set_sprite_LT' - optimized to work with simple sprites. It supports sprite offset values, but does not support double-buffering and doesn't increment SATB position, unlike 'spd_SATB_push_sprite'.

2. The SPReD-PCE exports both meta-sprites and simple sprites (16x16,16x32,16x64,32x16,32x32,32x64).
   The CGX/CGY flags are automatically applied to exported sprites. So you don't need to configure anything in your HuC program.

3. The library supports an arbitrary number of sprite sets, which are switched between using the 'spd_sprite_params' function.

4. Double-buffering is supported for meta-sprites. This requires 2x video memory for SG data, but ensures that there are no glitches when synchronizing SG and VRAM inner SATB.
   Compare both meta-sprites with and without double-buffering and decide which one is better in your case. As a rule, double-buffering helps when meta-sprites are at the top of the screen.
   But it also depends on the amount of SG data is being loaded to VRAM.

5. Supports changing the color palette for meta-sprites. Use 'spd_change_palette' right after 'spd_SATB_push_sprite'.

6. Simple or meta-sprite graphics can be loaded to any VRAM address. Use 'spd_alt_VRAM_addr' right after 'spd_sprite_params' and before 'spd_copy_SG_data_to_VRAM', 'spd_SATB_push_sprite' and 'spd_SATB_set_sprite_LT'.

7. There are two ways to load SG data to VRAM:

   - automatically, when calling 'spd_SATB_push_sprite' or 'spd_SATB_set_sprite_LT';
   - manually with 'spd_copy_SG_data_to_VRAM' for graphics caching before further use, as a rule for simple sprites, or to load a meta-sprite graphics without double-buffering after VBLANK/vsync();

   Keep this in mind when planning the logic of your program.

8. The library can be used in combination with HuC sprite functions. This makes it much easier to initialize data for HuC sprites.
   You can also use similar SPD library functions:

(!) Functions with the '_LT' postfix are designed to work with simple sprites.

   spr_set( N )		-	spd_SATB_set_pos( N )
   spr_pal( _plt_ind )	-	spd_set_palette_LT( _plt_ind )
   spr_get_pal()	-	spd_get_palette_LT()
   spr_pri( val )	-	spd_set_pri_LT( SPD_SPR_PRI_HIGH/SPD_SPR_PRI_LOW )
   spr_x( _x )		-	spd_set_x_LT( _x ) - doesn't support sprite offset value
   spr_get_x()		-	spd_get_x_LT()
   spr_y( _y )		-	spd_set_y_LT( _y ) - doesn't support sprite offset value
   spr_get_y()		-	spd_get_y_LT()
   spr_show()		-	spd_show_LT()
   spr_hide()		-	spd_hide_LT()

   There are also following optimized similar functions:

   init_satb()/reset_satb()	-	spd_SATB_clear_from( N )
   satb_update()		-	spd_SATB_to_VRAM()

9. Performance. Starting with the fastest function:

   Direct writing to SATB using: 'spd_set_palette_LT', 'spd_set_pri_LT', 'spd_set_x_LT', 'spd_set_y_LT' etc. or HuC functions.

   spd_SATB_set_sprite_LT( <sprite_name>, X, Y )
   spd_SATB_set_sprite_LT( <exported_name>_frames_arr, spr_ind, X, Y )
   spd_SATB_push_sprite( <sprite_name>, X, Y )
   spd_SATB_push_sprite( <exported_name>_frames_arr, spr_ind, X, Y )

   Use the following functions, if you don't use animations:

   spd_SATB_set_sprite_LT( <sprite_name>, X, Y )
   spd_SATB_push_sprite( <sprite_name>, X, Y )

   This will work faster, than functions that use sprite indexing:

   spd_SATB_set_sprite_LT( <exported_name>_frames_arr, spr_ind, X, Y )
   spd_SATB_push_sprite( <exported_name>_frames_arr, spr_ind, X, Y )

   The sprite indexing is designed specifically to make working with animations easier. So use these functions only for animated sprites.

10. Use 'SPD_DEBUG' to keep track of how efficiently/frequently SG data is loaded to VRAM.


Examples of SPD library using:
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

There are two data types the SPD-render works with.

1. PACKED sprites data. All exported SG data are stored in a single file. It were packed in the SPReD-PCE before exporting.
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
The main logic is:

	// SPD-render initialization.
	spd_init();	

	// Initialization of exported sprite set.
	{
		// Load palette in the usual way.
		load_palette( <EXPORTED_NAME>_PALETTE_SLOT, <exported_name>_palette, <exported_name>_PALETTE_SIZE );

		// Set up exported sprite set with SG data array and VRAM address to load SG data to.
		// NOTE: You can combine any number of exported sprite sets in your program.
		//	 Call the 'spd_sprite_params' to switch between them.
[upd] v0.5	// NOTE: Passing ZERO as the third parameter, means that SG data will be automatically
--->		//	 loaded to VRAM on 'spd_SATB_push_sprite'.
[upd] v0.2	// NOTE: Passing 'SPD_FLAG_IGNORE_SG' as the third parameter will ignore loading SG to VRAM.
		//	 It's useful for PACKED(!) sprites when you are switching to a sprite set and SG data already loaded to VRAM.
--->		//	 Such way you avoid loading SG to VRAM twice.
[upd] v0.4	// NOTE: Passing 'last_bank_ind' allows to avoid loading SG data to VRAM twice when you are switching back from another data set.
[upd] v0.6	//	 The last value can be obtained using 'spd_SG_bank_get_ind()'. The initial value is 'SPD_SG_BANK_INIT_VAL'.
--->		spd_sprite_params( <exported_name>_SG_arr, <EXPORTED_NAME>_SPR_VADDR, 0, last_bank_ind );

[upd] v0.3	// NOTE: There are two ways to load SG data to VRAM:
		//	 1. Indirect loading, when you push the first sprite by calling 'spd_SATB_push_sprite'.
		//	 The third argument for the 'spd_sprite_params' must be ZERO.
--->		//
[upd] v0.4	//	 2. Direct loading, when you call 'spd_copy_SG_data_to_VRAM' with a sprite name/index.
		//	 The third argument for the 'spd_sprite_params' must be 'SPD_FLAG_IGNORE_SG'.
		//
--->		//	 spd_copy_SG_data_to_VRAM( <exported_name>_frames_data, _spr_ind )
[upd] v0.6	//	 spd_copy_SG_data_to_VRAM( <sprite_name> )
	}

[upd] v0.6
	// SATB initialization.
	spd_SATB_clear_from( 0 );
--->
	// Here you can use HuC's sprite calls...

	// SPD calls
	{
		// Now you can set a local SATB position to push your exported sprites/meta-sprites to.
		// NOTE: The SATB position will be automatically incremented with each call to 'spd_SATB_push_sprite' (!)
[upd] v0.6	// NOTE: The SATB position will NOT be automatically incremented when using 'spd_SATB_set_sprite_LT' (!)
		spd_SATB_set_pos( <SATB_pos[0...63]> );
[upd] v0.6
		// NOTE: There are two ways to push sprite to SATB:
		//	 1. spd_SATB_push_sprite - for meta-sprites
		//
		//	 2. spd_SATB_set_sprite_LT - for simple sprites, it takes a little less processing time
		//	 compared to 'spd_SATB_push_sprite' and allows you to use sprite offset values, that will be
		//	 zeroed out when using HuC sprite functions.
		//
		// NOTE: Double-buffering and 'spd_change_palette' aren't supported with 'spd_SATB_set_sprite_LT'.
		//	 Use 'spd_set_palette_LT' instead of 'spd_change_palette' for simple sprites.
--->
		// There are two ways to show a sprite:
		// 1. By index in a sprite array. Suitable for animation sequences.
		// (see exported .h file for generated constants)
		spd_SATB_push_sprite( <exported_name>_frames_data, _ind, _x, _y );
		OR
[upd] v0.6	spd_SATB_set_sprite_LT( <exported_name>_frames_data, _ind, _x, _y );

		// 2. By sprite data pointer or by sprite name (it is faster than by index).
		// (see exported .h file for generated data)
		spd_SATB_push_sprite( <sprite_name>, _x, _y );
		OR
		spd_SATB_set_sprite_LT( <sprite_name>, _x, _y );

		// NOTE: SG data will be automatically loaded once to VRAM at first call to the 'spd_SATB_push_sprite' or 'spd_SATB_set_sprite_LT',
--->		//	 when the third parameter passed to the 'spd_sprite_params' is ZERO (!)
		// NOTE: If meta-sprite does not fit into SATB, it will be ignored!
[upd] v0.4	// NOTE: 'spd_SATB_push_sprite' returns: 1-Ok + ORed flag 'SPD_SG_NEW_DATA' when a new SG data already or must be loaded to VRAM; 0-SATB overflow

		// Save SG bank index, especially if you are using more than one sprite set(!)
--->		last_bank_ind	= spd_SG_bank_get_ind();
	}

	// Then call 'spd_SATB_to_VRAM' to push your sprite data to VRAM-SAT.
[upd] v0.6
	// NOTE: After pushing all sprites, `spd_SATB_get_pos()` returns the number of sprites in SATB when using the 'spd_SATB_push_sprite'.
	// NOTE: But when using 'spd_SATB_set_sprite_LT' you need to know how many sprites were used.
	
	// Move the entire SATB - 64 sprites to VRAM-SAT
	spd_SATB_to_VRAM();
	OR
	// Move a certain number of sprites to VRAM-SAT
	spd_SATB_to_VRAM( spr_cnt );
--->
[upd] v0.6
	// NOTE: As mentioned before, you can combine the SPD calls with the HuC ones or use SPD analogue functions.
	//	 For example, you can do the following for simple static sprites:
	// Initialization at startup
	load_palette( ...
	spd_sprite_params( ...

	// SATB initialization.
	spd_SATB_clear_from( 0 );

	// set SATB position to initialize a sprite data to
	spd_SATB_set_pos( 0 );
	spd_SATB_set_sprite_LT( my_sprite_16x32, init_x, init_y );

	...

	// In update loop
	spd_SATB_set_pos( 0 );
	spd_set_x_LT( new_x );
	spd_set_y_LT( new_y );

	// I recommend using simple sprites this way, as it simplifies their initialization and is optimal for runtime.

	// NOTE: The functions for using with simple sprites:
	//
	//	 void		spd_set_palette_LT( unsigned char _plt_ind );
	//	 unsigned char	spd_get_palette_LT();
	//	 void		spd_set_pri_LT( SPD_SPR_PRI_HIGH/SPD_SPR_PRI_LOW );
	//	 void		spd_set_x_LT( unsigned short _x );	<--- doesn't support sprite offset value
	//	 unsigned short	spd_get_x_LT();
	//	 void		spd_set_y_LT( unsigned short _y );	<--- doesn't support sprite offset value
	//	 unsigned short	spd_get_y_LT();
	//	 void		spd_show_LT();
	//	 void		spd_hide_LT();
--->


2. UNPACKED sprites data. All exported SG data are stored in separate files. It were not packed in the SPReD-PCE.
Unpacked data are suitable for dynamic SG data, that can be loaded to VRAM dynamically to save video memory. It`s
useful when you have a lot of animations that don`t fit into VRAM, like in fighting games. Simple sprites can also
be stored as unpacked data.
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
The main logic is:

	// SPD-render initialization.
	spd_init();	

	// Initialization of exported sprite set.
	{
		// Load palette in the usual way.
		load_palette( <EXPORTED_NAME>_PALETTE_SLOT, <exported_name>_palette, <exported_name>_PALETTE_SIZE );

		// Set up exported sprite set with SG data array and VRAM address to load SG data to.
#if	DEF_SG_DBL_BUFF
		// NOTE: Using the `SPD_FLAG_DBL_BUFF` flag means double-buffering for sprite graphics.
		//	 It costs x2 of dynamic SG data in VRAM, but glitches free. You have to compare the results
		//	 of using 'SPD_FLAG_DBL_BUFF' and 'SPD_FLAG_PEND_SG_DATA' and decide which is better in your case.
[upd] v0.4	// NOTE: Passing 'last_bank_ind' allows to avoid loading SG data to VRAM twice when you are switching back from another data set.
[upd] v0.6	//	 The last value can be obtained using 'spd_SG_bank_get_ind()'. The initial value is 'SPD_SG_BANK_INIT_VAL'.
--->		spd_sprite_params( <exported_name>_SG_arr, <EXPORTED_NAME>_SPR_VADDR, SPD_FLAG_DBL_BUFF, last_bank_ind );

[upd] v0.5	// Set the second VRAM address for double-buffering (SPD_FLAG_DBL_BUFF) and the last double-buffer index value (initial value is 'SPD_DBL_BUFF_INIT_VAL').
--->		spd_dbl_buff_VRAM_addr( VADDR_dbl_buff, last_dbl_buff_ind );
#else
		// NOTE: Using the `SPD_FLAG_PEND_SG_DATA` flag means that SG data will not be loaded
		//	 to VRAM automatically. You should do that manually on VBLANK.
		spd_sprite_params( <exported_name>_SG_arr, <EXPORTED_NAME>_SPR_VADDR, SPD_FLAG_PEND_SG_DATA, last_bank_ind );

		// Set pointers to SG data for delayed use (SPD_FLAG_PEND_SG_DATA).
		spd_SG_data_params( &SG_DATA_SRC_ADDR, &SG_DATA_SRC_BANK, &SG_DATA_DST_ADDR, &SG_DATA_LEN );
#endif
		// NOTE: Single- and double-buffered meta-sprites can be combined in runtime.
		// NOTE: You can combine any number of exported sprite sets in your program.
		//	 Call the 'spd_sprite_params' to switch between them.

[upd] v0.5	// NOTE: To use multiple instances of the same sprite set, for example, for a sprite cache,
--->		//	 use 'spd_alt_VRAM_addr( _alt_VADDR )' which replaces the '<EXPORTED_NAME>_SPR_VADDR'.
	}

[upd] v0.6
	// SATB initialization.
	spd_SATB_clear_from( 0 );
--->
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

[upd] v0.6	// 2. By sprite data pointer or by sprite name (it is faster than by index).
		// (see exported .h file for generated data)
--->		spd_SATB_push_sprite( <sprite_name>, _x, _y );

		// NOTE: If meta-sprite does not fit into SATB, it will be ignored!
[upd] v0.4	// NOTE: 'spd_SATB_push_sprite' returns: 1-Ok + ORed flag 'SPD_SG_NEW_DATA' when a new SG data already or must be loaded to VRAM; 0-SATB overflow

		// Save SG bank index, especially if you are using more than one sprite set(!)
--->		last_bank_ind	= spd_SG_bank_get_ind();
[upd] v0.5
#if		DEF_SG_DBL_BUFF
		last_dbl_buff_ind	= spd_get_dbl_buff_ind();
#endif
--->
	}

	// 'VRAM-SATB Transfer Auto-Repeat on VBLANK' (DCR ROF-$10) is enabled by default in HuC at startup, so we skip this step.

	// Main loop
	for( ;; )
	{
		// Update your sprite animation
		update_frame( &test_anim );

		// clear SATB
		spd_SATB_clear_from( <SATB_pos[0...63]> );// to save sprites before 'SATB_pos', and clear memory after to avoid graphical glitches with variable sized meta-sprites

		// Set the SATB position to push your sprite to.
		spd_SATB_set_pos( <SATB_pos[0...63]> );

		// Push your sprite to the local RAM SATB.
[upd] v0.4	res_byte = spd_SATB_push_sprite( <exported_name>_frames_data, test_anim.start_frame + test_anim.curr_frame, _x, _y );

--->		last_bank_ind	= spd_SG_bank_get_ind();

[upd] v0.5	// NOTE: To change sprite palette, use 'spd_change_palette( _plt_ind )' function immediately after calling the 'spd_SATB_push_sprite',
		//	 if the result isn't zero. See './samples/pce/sprite_render/animation_test/huc3' and '/huc4' for details.
		if( res_byte )
		{
			spd_change_palette( _plt_ind );	// _plt_ind - 0...15
		}

#if		DEF_SG_DBL_BUFF
		last_dbl_buff_ind	= spd_get_dbl_buff_ind();
#endif
--->
[upd] v0.6	// Move the entire SATB - 64 sprites to VRAM-SAT
		spd_SATB_to_VRAM();
		OR
		// Move a certain number of sprites to VRAM-SAT
		spd_SATB_to_VRAM( spr_cnt );
--->		
		vsync();

#if	!DEF_SG_DBL_BUFF
		// The HuC doesn't allow to handle VBLANK directly, so we use 'vsync' instead for synchronization of sprite graphics and the inner SATB.
		// This may cause some graphical glitches at the upper part of the screen.

		// Delayed copying of SG data to VRAM to synchronize it with the inner SATB
[upd] v0.4	if( res_byte & SPD_SG_NEW_DATA )
		{
			spd_copy_SG_data_to_VRAM( SG_DATA_SRC_ADDR, SG_DATA_SRC_BANK, SG_DATA_DST_ADDR, SG_DATA_LEN );
		}
#endif
	}

[upd] v0.6
3. See the sample projects for implementation details.

[upd] v0.4
4. Also you can use PACKED and UNPACKED data in one data set (in one SPReD-PCE project) by combining the approaches described above.

Misc:
~~~~~

[upd] v0.6
#asm
SPD_TII_ATTR_XY	; speeds up transformation of meta-sprite attributes a bit, but may delay interrupts
#endasm
--->
	That`s it! :)
/*/

const unsigned char spd_ver[] = { "S", "P", "D", "0", "6", 0 };

/* SPD flag(s) */

// Copies SG data parameters: src_addr/src_bank/vram_addr/len for delayed use on VBLANK;
// it's suitable for sprites with dynamic SG data like in fighting games.
// NOTE: THIS FLAG CAN BE USED WITH UNPACKED SPRITES ONLY! WHERE EACH SPRITE HAS A SEPARATE SG DATA!
#define	SPD_FLAG_PEND_SG_DATA	0x01

// Double-buffering. It costs x2 of dynamic SG data in VRAM, but glitches free.
// You have to compare the results of using 'SPD_FLAG_DBL_BUFF' and 'SPD_FLAG_PEND_SG_DATA' and decide which is better for you.
// Thanks to elmer/pcengine.proboards.com for suggesting this mode.
// NOTE: THIS FLAG CAN BE USED WITH UNPACKED SPRITES ONLY! WHERE EACH SPRITE HAS A SEPARATE SG DATA!
#define	SPD_FLAG_DBL_BUFF	0x02

// Loading sprite graphics to VRAM will be ignored. It's useful for PACKED(!) sprites when you are switching to a sprite set and SG data already loaded to VRAM.
// Such way you avoid loading SG to VRAM twice.
#define	SPD_FLAG_IGNORE_SG	0x04

// ORed value returned by 'spd_SATB_push_sprite', means that new SG data already or must be loaded to VRAM
#define SPD_SG_NEW_DATA		0x80

// Initial value of a last double-buffer index
#define	SPD_DBL_BUFF_INIT_VAL	0x00

// Initial value for a SG data
#define	SPD_SG_BANK_INIT_VAL	0xff

// input parameters for spd_set_pri_LT
#define SPD_SPR_PRI_HIGH	0x80
#define SPD_SPR_PRI_LOW		0x00

/* main SPD-render routines */

void		__fastcall spd_init();

// SATB functions
void		__fastcall spd_SATB_set_pos( unsigned char _pos<acc> );// _pos: 0-63
unsigned char	__fastcall spd_SATB_get_pos();
void		__fastcall spd_SATB_clear_from( unsigned char _pos<acc> );// _pos: 0-63
void		__fastcall spd_SATB_to_VRAM();
void		__fastcall spd_SATB_to_VRAM( unsigned char _spr_cnt<acc> );// _spr_cnt: 1-64

// meta-sprites
unsigned char	__fastcall spd_SATB_push_sprite( unsigned char far* _frames_data<__bl:__si>, unsigned char _spr_ind<__bh>, unsigned short _x<__ax>, unsigned short _y<__cx> );// OUT: 1-Ok!, 0-SATB overflow
unsigned char	__fastcall spd_SATB_push_sprite( unsigned char far* _frame_addr<__bl:__si>, unsigned short _x<__ax>, unsigned short _y<__cx> );// OUT: 1-Ok!, 0-SATB overflow
void		__fastcall spd_change_palette( unsigned char _plt_ind<__al> );

// simple sprites
unsigned char	__fastcall spd_SATB_set_sprite_LT( unsigned char far* _frames_data<__bl:__si>, unsigned char _spr_ind<__bh>, unsigned short _x<__ax>, unsigned short _y<__cx> );// OUT: OUT: 1-Ok!, 1|SPD_SG_NEW_DATA
unsigned char	__fastcall spd_SATB_set_sprite_LT( unsigned char far* _frame_addr<__bl:__si>, unsigned short _x<__ax>, unsigned short _y<__cx> );// OUT: 1-Ok!, 1|SPD_SG_NEW_DATA
void		__fastcall spd_set_palette_LT( unsigned char _plt_ind<__al> );
unsigned char	__fastcall spd_get_palette_LT();
void		__fastcall spd_set_pri_LT( unsigned char _val<__al> );
void		__fastcall spd_set_x_LT( unsigned short _x<acc> );
unsigned short	__fastcall spd_get_x_LT();
void		__fastcall spd_set_y_LT( unsigned short _y<acc> );
unsigned short	__fastcall spd_get_y_LT();
void		__fastcall spd_show_LT();
void		__fastcall spd_hide_LT();

// common functions
void		__fastcall spd_sprite_params( unsigned short far* _SG_data<__bl:__si>, unsigned short _VADDR<__dx>, unsigned char _flags<__al>, unsigned char _last_SG_bank_ind<__ah> );
unsigned char	__fastcall spd_SG_bank_get_ind();

void		__fastcall spd_SG_data_params( unsigned short _src_addr<__ax>, unsigned short _src_bank<__bx>, unsigned short _dst_addr<__cx>, unsigned short _len<__dx> );
void		__fastcall spd_copy_SG_data_to_VRAM( unsigned short _src_addr<__ax>, unsigned char _src_bank<__bl>, unsigned short _dst_addr<__dx>, unsigned short _len<__cx> );
void		__fastcall spd_copy_SG_data_to_VRAM( unsigned char far* _frames_data<__bl:__si>, unsigned char _spr_ind<__bh> );
void		__fastcall spd_copy_SG_data_to_VRAM( unsigned char far* _frame_addr<__bl:__si> );

void		__fastcall spd_dbl_buff_VRAM_addr( unsigned short _VADDR<__ax>, unsigned short _dbl_buff_ind<acc> );
void		__fastcall spd_alt_VRAM_addr( unsigned short _VADDR<__ax> );
unsigned char	__fastcall spd_get_dbl_buff_ind();

#asm

; macros(es)

; getting data by pointers

; val -> &((byte)(*zp))
	.macro spd_stb_zpii ; \1 - val, \2 - zp
	cly
	lda low_byte \1
	sta [\2], y
	.endm

; val -> &((word)(*zp))
	.macro spd_stw_zpii ; \1 - val, \2 - zp
	cly
	lda low_byte \1
	sta [\2], y
	iny
	lda high_byte \1
	sta [\2], y
	.endm

; (byte(*zp)) -> *addr
	.macro spd_stb_zpii_rev ; \1 - zp, \2 - addr
	lda [<\1], y
	sta \2
	.endm

; (word(*zp)) -> *addr
	.macro spd_stw_zpii_rev ; \1 - zp, \2 - addr
	lda [<\1], y
	sta \2
	iny
	lda [<\1], y
	sta \2 + 1
	.endm

; ( (word(*zp)) / 2 ) -> *addr
	.macro spd_stw_div2_zpii_rev ; \1 - zp, \2 - addr
	lda [<\1], y		; high byte
	lsr a
	sta \2 + 1
	dey
	lda [<\1], y		; low byte
	ror a
	sta \2
	.endm

; math macroses

; \1 = Y * 3
	.macro spd_mul3_y
	lda __mul3_lb_LUT, y	;5
	sta low_byte \1		;4/5
	lda __mul3_hb_LUT, y	;5
	sta high_byte \1	;4/5 = 18(20)
	.endm

; \1 = Y * 6
	.macro spd_mul6_y
	lda __mul3_lb_LUT, y	;5
	asl a			;2
	sta low_byte \1		;4/5
	lda __mul3_hb_LUT, y	;5
	rol a			;2
	sta high_byte \1	;4/5 = 22(24)
	.endm

; \1 = \2 + ( A * 8 ), \2 - SATB
	.macro spd_calc_SATB_pos
	stz high_byte \1
	asl a
	asl a
	asl a
	rol high_byte \1

	adc low_byte \2
	sta low_byte \1
	lda high_byte \1
	adc high_byte \2
	sta high_byte \1
	.endm	

; \2 = \1 / 8
	.macro spd_div8_word
	lda high_byte \1	;4/5
	sta high_byte \2	;4/5
	lda low_byte \1		;4/5
	lsr high_byte \2	;6
	ror a			;2
	lsr high_byte \2	;6
	ror a			;2
	lsr high_byte \2	;6
	ror a			;2
	sta low_byte \2		;4/5 = 40(44)
	.endm

; \1 /= 32
	.macro spd_div32_word
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
	.macro spd_div32_neg_word
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

; \1 += A
	.macro spd_add_a_to_word
	clc
	adc low_byte \1
	sta low_byte \1

	bcc @cont
	inc high_byte \1
@cont:
	.endm

; \2 = \1 - \2
	.macro spd_sub_word_from_word
	lda low_byte \1
	sec
	sbc low_byte \2
	sta low_byte \2
	lda high_byte \1
	sbc high_byte \2
	sta high_byte \2
	.endm

; inner library flags macroses

	.macro spd_get_SATB_flag
	lda <__SATB_flags
	and #\1
	.endm

	.macro spd_inner_flags_dbl_buff_state
	lda <__inner_flags
	and #INNER_FLAGS_DBL_BUFF
	.endm

	.macro spd_inner_flags_switch_dbl_buff
	lda <__inner_flags
	eor #INNER_FLAGS_DBL_BUFF
	sta <__inner_flags
	.endm

; border color macroses

	.macro spd_set_border_color
	stw #$0100, $0402
	sty $0404
	stx $0405
	.endm

	.macro	spd_dbg_border_load_VRAM
	clx
	ldy #$ff
	spd_set_border_color
	.endm

	.macro	spd_dbg_border_push_sprite
	ldx #$01
	ldy #$ff
	spd_set_border_color
	.endm

	.macro spd_dbg_border_attrs_transf
	ldx #$01
	ldy #$e7
	spd_set_border_color
	.endm

	.macro	spd_dbg_border_black
	clx
	cly
	spd_set_border_color
	.endm

; VDC macroses

	.macro spd_vreg
	lda \1
	sta <vdc_reg
	st0 \1
	.endm

	.macro spd_VDC_set_write
	spd_vreg #$00

	st1 low_byte \1
	st2 high_byte \1

	spd_vreg #$02
	.endm	

	.zp

SPD_FLAG_PEND_SG_DATA	= $01
SPD_FLAG_DBL_BUFF	= $02
SPD_FLAG_IGNORE_SG	= $04
SPD_FLAG_ALT_VADDR	= $08	; is used when the 'spd_alt_VRAM_addr()' is called

SPD_SG_NEW_DATA		= $80	; 'spd_SATB_push_sprite' result

SATB_SIZE	= 64

__SATB_pos	.ds 1
__SATB_flags	.ds 1
__last_SG_bank	.ds 1

INNER_FLAGS_DBL_BUFF	= %00000001

__inner_flags	.ds 1

; *** sprite data ***

__spr_VADDR	.ds 2	; sprite`s SG data address in VRAM
__spr_VADDR_dbl	.ds 2	; VRAM address for double-buffering
__spr_dbf_SG_offset
		.ds 2	; SG offset for pattern index correction when double-buffering is active

__spr_alt_SG_offset
		.ds 2	; SG offset for pattern index correction for alternative VADDR

__spr_SG_data_addr
		.ds 2
__spr_SG_data_bank
		.ds 1

__spr_pos_x:	.ds 2
__spr_pos_y:	.ds 2

__spr_SATB_addr:
		.ds 2	; SATB entry address

	.bss

__SG_DATA_SRC_ADDR	.ds 2	; pointers
__SG_DATA_SRC_BANK	.ds 2	; to
__SG_DATA_DST_ADDR	.ds 2	; appropriate
__SG_DATA_LEN		.ds 2	; data

__TII:		.ds 1	; $73 tii
__bsrci:	.ds 2
__bdsti:	.ds 2
__bleni:	.ds 2
__tiirts	.ds 1	; $60 rts

	.code

;// spd_init()
;
	.proc _spd_init

	cla
	sta <__inner_flags

	; init TII

	lda #$73
	sta __TII
	lda #$60
	sta __tiirts

	rts

	.endp

;// spd_sprite_params( farptr __bl:__si / SG_data, word __dx / vaddr, byte __al / flags, byte __ah / last_SG_bank_ind )
;
	.proc _spd_sprite_params.4

	stw <__si, <__spr_SG_data_addr
	lda <__bl
	sta <__spr_SG_data_bank

	stw <__dx, <__spr_VADDR

	lda <__al
	sta <__SATB_flags

	lda <__ah
	sta <__last_SG_bank

	rts

	.endp

;// spd_SG_bank_get_ind()
;
	.proc _spd_SG_bank_get_ind

	ldx <__last_SG_bank
	cla

	rts

	.endp

	.procgroup

;// spd_dbl_buff_VRAM_addr( word __ax / vram_addr, word acc / dbl_buff_ind )
;
	.proc _spd_dbl_buff_VRAM_addr.2

	; set double-buffer index

	txa
	beq .reset

	; set

	ora <__inner_flags
	sta <__inner_flags

	bra .cont

.reset:

	lda <__inner_flags
	and #(~INNER_FLAGS_DBL_BUFF)
	sta <__inner_flags

.cont:
	; set alternative VADDR

	stw <__ax, <__spr_VADDR_dbl
	stw <__spr_VADDR, <__bx

	call _calc_SG_pattern_offset

	stw <__bx, <__spr_dbf_SG_offset

	rts

	.endp

;// void spd_alt_VRAM_addr( word __ax / vram_addr )
;
	.proc _spd_alt_VRAM_addr.1

	; apply alt VADDR flag

	lda #SPD_FLAG_ALT_VADDR
	ora <__SATB_flags
	sta <__SATB_flags

	; set alternative VADDR

	stw <__spr_VADDR, <__bx
	stw <__ax, <__spr_VADDR			; replace the sprite VADDR with the alternative one

	call _calc_SG_pattern_offset

	stw <__bx, <__spr_alt_SG_offset

	rts

	.endp

;
; IN:	__ax - VADDR1
;	__bx - VADDR2
;
; OUT:	__bx - pattern index offset
;
	.proc _calc_SG_pattern_offset

	spd_sub_word_from_word <__ax, <__bx

	lda <__bh
	bpl .pos_val

	; negative val

	; SG_offset = __bx / 32

	spd_div32_neg_word <__bx

	rts

	; positive val

.pos_val:

	; SG_offset = __bx / 32

	spd_div32_word <__bx

	rts

	.endp

	.endprocgroup

;// unsigned char spd_get_dbl_buff_ind()
;
	.proc _spd_get_dbl_buff_ind

	spd_inner_flags_dbl_buff_state
	tax
	cla
	
	rts

	.endp

	.procgroup

;// spd_SATB_set_pos( byte acc / pos )
;
	.proc _spd_SATB_set_pos.1

	txa
	and #SATB_SIZE - 1		; clamp to 0-63
	sta <__SATB_pos

	; calc SATB address to copy sprite data to
	; __spr_SATB_addr = __SATB + ( __SATB_pos * 8 )

	spd_calc_SATB_pos <__spr_SATB_addr, #__SATB

	rts

	.endp

	.proc _spd_SATB_get_pos

	ldx <__SATB_pos
	cla

	rts

	.endp

;// spd_SATB_clear_from( byte acc / pos )
;
	.proc _spd_SATB_clear_from.1

	txa
	and #SATB_SIZE - 1		; clamp to 0-63

	sec
	sbc #SATB_SIZE
	eor #$ff
	inc a
	tay				; Y - num sprites to clear

	ldx #$ff

.loop:

	stz __SATB + 256, x
	dex
	stz __SATB + 256, x
	dex
	stz __SATB + 256, x
	dex
	stz __SATB + 256, x
	dex
	stz __SATB + 256, x
	dex
	stz __SATB + 256, x
	dex
	stz __SATB + 256, x
	dex
	stz __SATB + 256, x

	beq .clear_2nd_part

	dex

	dey
	bne .loop

	rts

.clear_2nd_part:

	dey

	ldx #$ff

.loop2:

	stz __SATB, x
	dex
	stz __SATB, x
	dex
	stz __SATB, x
	dex
	stz __SATB, x
	dex
	stz __SATB, x
	dex
	stz __SATB, x
	dex
	stz __SATB, x
	dex
	stz __SATB, x
	dex

	dey
	bne .loop2

	rts

	.endp

;// spd_SATB_to_VRAM()
;
	.proc _spd_SATB_to_VRAM

	ldx #SATB_SIZE
	call _spd_SATB_to_VRAM.1

	rts

	.endp

;// spd_SATB_to_VRAM( unsigned char acc / spr_cnt )
;
	.proc _spd_SATB_to_VRAM.1

	txa

	; round up to the next group of 4 sprites

	dec a
	lsr a
	lsr a
	inc a
	tax

	; split data into chunks by 16 words

	stw #$20, __TIA_LEN
	stw #video_data, __TIA_DST
	stw #__SATB, __TIA_SRC

	spd_VDC_set_write #__VRAM_SAT_ADDR

.loop:
	jsr __TIA

	lda #$20
	spd_add_a_to_word __TIA_SRC

	dex
	bne .loop

	rts

	.endp

	.endprocgroup

	.procgroup

;// spd_change_palette( byte __al / plt_ind )
;
	.proc _spd_change_palette.1

	lda <__al
	and #%00001111			; clamp 16 colors area
	ora #%10000000			; set SPBG bit by default
	tax

	spd_div8_word __bleni, <__bx	; __bl - number of sprites

	stw __bdsti, <__cx
	lda #6				; offset to the first palette byte
	spd_add_a_to_word <__cx

	cly
.loop:
	txa
	sta [<__cx], y

	clc
	tya
	adc #8
	tay

	bne .cont

	inc <__ch
.cont:
	dec <__bl
	bne .loop

	rts

	.endp

;// spd_set_palette_LT( byte __al / plt_ind )
;
	.proc _spd_set_palette_LT.1

	ldy #6
	lda [<__spr_SATB_addr], y
	and #$80
	ora <__al
	sta [<__spr_SATB_addr], y

	rts

	.endp

;// unsigned char spd_get_palette_LT()
;
	.proc _spd_get_palette_LT

	ldy #6
	lda [<__spr_SATB_addr], y
	and #$0f
	tax
	cla

	rts

	.endp

;// spd_set_pri_LT( byte __al / val )
;
	.proc _spd_set_pri_LT.1

	ldy #6
	lda [<__spr_SATB_addr], y
	and #$7f
	ora <__al
	sta [<__spr_SATB_addr], y

	rts

	.endp

;// spd_set_x_LT( word acc / x )
	.proc _spd_set_x_LT.1

	sax				;3

	clc				;2
	adc #32				;2

	ldy #2				;2
	sta [<__spr_SATB_addr], y	;7

	txa				;2
	adc #00				;2

	iny				;2
	sta [<__spr_SATB_addr], y	;7 = 29

	rts

	.endp

;// unsigned short spd_get_x_LT()
	.proc _spd_get_x_LT

	ldy #2				;2
	lda [<__spr_SATB_addr], y	;7
	sec				;2
	sbc #32				;2
	tax				;2

	iny				;2
	lda [<__spr_SATB_addr], y	;7
	sbc #0				;2 = 26

	rts

	.endp

;// spd_set_y_LT( word acc / y )
	.proc _spd_set_y_LT.1

	sax				;3

	clc				;2
	adc #64				;2

	sta [<__spr_SATB_addr]		;7

	txa				;2
	adc #00				;2

	ldy #1				;2
	sta [<__spr_SATB_addr], y	;7 = 27

	rts

	.endp

;// unsigned short spd_get_y_LT()
	.proc _spd_get_y_LT

	lda [<__spr_SATB_addr]		;4
	sec				;2
	sbc #64				;2
	tax				;2

	ldy #1				;2
	lda [<__spr_SATB_addr], y	;7
	sbc #0				;2 = 21

	rts

	.endp

;// spd_show_LT()
	.proc _spd_show_LT

	ldy #1
	lda [<__spr_SATB_addr], y
	and #1
	sta [<__spr_SATB_addr], y

	rts

	.endp

;// spd_hide_LT()
	.proc _spd_hide_LT

	ldy #1
	lda [<__spr_SATB_addr], y
	ora #2
	sta [<__spr_SATB_addr], y

	rts

	.endp

; *** farptr += offset ***
;
; IN:
;
; __dx - offset
; __bl - bank number
; __si - address
;
	.proc _spd_farptr_add_offset

	; add an offset

	clc
	lda <__dx
	adc <__si
	sta <__si
	lda <__si+1
	and #$1f
	adc <__dx+1

;	tay		;2

	sta <__si+1

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

;	tya		;2
;	and #$1f	;2 will be
;	ora #$60	;2 done in
;	sta <__si+1	;4 map_data
	
	rts

	.endp

;// spd_SATB_push_sprite( farptr __bl:__si / addr, byte __bh / index, word __ax / x_pos, word __cx / y_pos )
;
	.proc _spd_SATB_push_sprite.4

.ifdef	SPD_DEBUG
	spd_dbg_border_push_sprite
.endif
	; offset x3 -> word:addr, byte:bank

	ldy <__bh
	spd_mul3_y <__dx

	call _spd_farptr_add_offset

	jsr map_data

	; __si - spd_SPRITE addr
	; __bl - spd_SPRITE bank

	cly

	spd_stw_zpii_rev __si, <__dx	; __dx - spd_SPRITE addr
	iny
	spd_stb_zpii_rev __si, <__bl	; __bl - spd_SPRITE bank

	stw <__dx, <__si		; __si - spd_SPRITE addr

	jsr unmap_data

	call _spd_SATB_push_sprite.3

	rts

	.endp

;// spd_SATB_push_sprite( farptr __bl:__si / addr, word __ax / x_pos, word __cx / y_pos )
;
	.proc _spd_SATB_push_sprite.3

.ifdef	SPD_DEBUG
	spd_dbg_border_push_sprite
.endif

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

	; map spd_SPRITE data

	jsr map_data

	; get meta-sprite length

	cly
	spd_stw_zpii_rev __si, __bleni

	iny

	; get meta-sprite SG bank index

	lda [<__si], y
	tay				; Y - SG bank index

	; move to the attributes data

	lda #3
	spd_add_a_to_word <__si		; __si - points to attributes

	; check SATB overflow
	spd_div8_word __bleni, <__ax	; __al - number of sprites
	clc
	adc <__SATB_pos
	cmp #SATB_SIZE + 1
	bcc .push_sprite

	; SATB overflow

.ifdef	SPD_DEBUG
	spd_dbg_border_black
.endif

	clx
	cla

	rts

.push_sprite:

.ifdef	SPD_TII_ATTR_XY
	; _bsrci = meta-sprite address
	
	stw <__si, __bsrci
.endif
	stw <__spr_SATB_addr, __bdsti	; for 'spd_change_palette'

	phy				; Y - SG bank index

;--- DBL-BUFF ---
	; check double-buffering state

.ifdef	SPD_DEBUG
	phy
	spd_dbg_border_attrs_transf
	ply
.endif
	spd_get_SATB_flag SPD_FLAG_DBL_BUFF
	beq .use_main_buff		; no double-buffering

	; double-buffering is enabled

	cpy <__last_SG_bank
	beq .skip_dbl_buff_switch	; we have the same SG bank data, so skip double-buffer switching and use the SG that already loaded to VRAM

	; toggle the double-buffering flag

	spd_inner_flags_switch_dbl_buff	; the new SG data will be loaded to opposite VADDR

.skip_dbl_buff_switch:

	spd_inner_flags_dbl_buff_state
	beq .use_main_buff
	
	jmp __attr_transf_XY_IND_dbf

.use_main_buff:

;--- DBL-BUFF ---

	jmp __attr_transf_XY

_push_SG_data:

.ifdef	SPD_DEBUG
	spd_dbg_border_push_sprite
.endif

	; calc SATB address for the next sprites

	lda <__SATB_pos
	spd_calc_SATB_pos <__spr_SATB_addr, #__SATB

	jsr unmap_data

	ply				; Y - SG bank index

;--- SPD_FLAG_IGNORE_SG ---
	spd_get_SATB_flag SPD_FLAG_IGNORE_SG
	bne .ignore_SG_data
;--- SPD_FLAG_IGNORE_SG ---

	; check if SG data already loaded to VRAM

	cpy <__last_SG_bank
	beq .ignore_SG_data

	call _load_SG_data

	rts

.ignore_SG_data:

	; SG data already loaded or must be ignored

.ifdef	SPD_DEBUG
	spd_dbg_border_black
.endif

	ldx #1
	cla

	rts

	; transform XY coordinates

__attr_transf_XY:

	cly

;--- SPD_FLAG_ALT_VADDR ---
	spd_get_SATB_flag SPD_FLAG_ALT_VADDR
	beq .__copy_attrs

	jmp __attr_transf_XY_IND_alt
;--- SPD_FLAG_ALT_VADDR ---

.__copy_attrs:

.ifdef	SPD_TII_ATTR_XY

	; copy meta-sprite attributes to the local SATB

	jsr __TII
.endif

.__attr_loop_XY:			;176(180)

	iny				;2

	; attr.Y += __spr_pos_y

	lda [<__si], y			;7 high byte
	tax				;2
	dey				;2
	lda [<__si], y			;7 low byte

	clc				;2
	adc <__spr_pos_y		;4
	sta [<__spr_SATB_addr], y	;7
	txa				;2
	adc <__spr_pos_y + 1		;4
	iny				;2
	sta [<__spr_SATB_addr], y	;7 = [46]

	iny				;2 = 50

	; attr.X += __spr_pos_x

	iny
	lda [<__si], y
	tax
	dey
	lda [<__si], y

	clc
	adc <__spr_pos_x
	sta [<__spr_SATB_addr], y
	txa
	adc <__spr_pos_x + 1
	iny
	sta [<__spr_SATB_addr], y

	; move to the next attr line

.ifdef	SPD_TII_ATTR_XY
	tya				;2
	clc				;2
	adc #05				;2
	tay				;2 = 8
.else
	iny				;2
	lda [<__si], y			;7
	sta [<__spr_SATB_addr], y	;7

	iny
	lda [<__si], y
	sta [<__spr_SATB_addr], y

	iny
	lda [<__si], y
	sta [<__spr_SATB_addr], y

	iny
	lda [<__si], y
	sta [<__spr_SATB_addr], y

	iny
.endif
	bne .cont			;2

	inc <__spr_SATB_addr + 1
.cont:
	inc <__SATB_pos

	dec <__al			; __al - number of attrs
	bne .__attr_loop_XY

	jmp _push_SG_data

	; transform XY coordinates and modify sprite pattern code
	; this routine is used for alternative VADDR

__attr_transf_XY_IND_alt:

	cly

.__attr_loop_XY_IND_alt:		;194(198)

	iny

	; attr.Y += __spr_pos_y

	lda [<__si], y
	tax
	dey
	lda [<__si], y

	clc
	adc <__spr_pos_y
	sta [<__spr_SATB_addr], y
	txa
	adc <__spr_pos_y + 1
	iny
	sta [<__spr_SATB_addr], y

	iny

	; attr.X += __spr_pos_x

	iny
	lda [<__si], y
	tax
	dey
	lda [<__si], y

	clc
	adc <__spr_pos_x
	sta [<__spr_SATB_addr], y
	txa
	adc <__spr_pos_x + 1
	iny
	sta [<__spr_SATB_addr], y

	iny

	; attr.SG_ind += __spr_alt_SG_offset

	iny
	lda [<__si], y
	tax
	dey
	lda [<__si], y

	clc
	adc <__spr_alt_SG_offset
	sta [<__spr_SATB_addr], y
	txa
	adc <__spr_alt_SG_offset + 1
	iny
	sta [<__spr_SATB_addr], y

	; copy pattern code/palette/CGX/CGY data

	iny				;2
	lda [<__si], y			;7
	sta [<__spr_SATB_addr], y	;7

	iny
	lda [<__si], y
	sta [<__spr_SATB_addr], y

	iny

	bne .cont			;2

	inc <__spr_SATB_addr + 1
.cont:
	inc <__SATB_pos

	dec <__al			; __al - number of attrs
	bne .__attr_loop_XY_IND_alt

	jmp _push_SG_data

	; transform XY coordinates and modify sprite pattern code
	; this routine is used when double-buffering is active

__attr_transf_XY_IND_dbf:

	cly

.__attr_loop_XY_IND_dbf:		;194(198)

	iny

	; attr.Y += __spr_pos_y

	lda [<__si], y
	tax
	dey
	lda [<__si], y

	clc
	adc <__spr_pos_y
	sta [<__spr_SATB_addr], y
	txa
	adc <__spr_pos_y + 1
	iny
	sta [<__spr_SATB_addr], y

	iny

	; attr.X += __spr_pos_x

	iny
	lda [<__si], y
	tax
	dey
	lda [<__si], y

	clc
	adc <__spr_pos_x
	sta [<__spr_SATB_addr], y
	txa
	adc <__spr_pos_x + 1
	iny
	sta [<__spr_SATB_addr], y

	iny

	; attr.SG_ind += __spr_dbf_SG_offset

	iny
	lda [<__si], y
	tax
	dey
	lda [<__si], y

	clc
	adc <__spr_dbf_SG_offset
	sta [<__spr_SATB_addr], y
	txa
	adc <__spr_dbf_SG_offset + 1
	iny
	sta [<__spr_SATB_addr], y

	; copy pattern code/palette/CGX/CGY data

	iny
	lda [<__si], y
	sta [<__spr_SATB_addr], y

	iny
	lda [<__si], y
	sta [<__spr_SATB_addr], y

	iny

	bne .cont

	inc <__spr_SATB_addr + 1
.cont:
	inc <__SATB_pos

	dec <__al			; __al - number of attrs
	bne .__attr_loop_XY_IND_dbf

	jmp _push_SG_data

	.endp

	; load SG data to VRAM

	.proc _load_SG_data

	sty <__last_SG_bank

	; __dx = SG bank index x6 ( .word <data_length>, chrN, bank(chrN) )

	spd_mul6_y <__dx

	stw <__spr_SG_data_addr, <__si
	lda <__spr_SG_data_bank
	sta <__bl

	call _spd_farptr_add_offset

	jsr map_data			; map SG data array

	; __cx = SG data length

	ldy #$01
	spd_stw_div2_zpii_rev __si, <__cx

	; __ax = SG data address

	ldy #$02
	spd_stw_zpii_rev __si, <__ax

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
	spd_get_SATB_flag SPD_FLAG_DBL_BUFF
	beq .use_main_VADDR		; no double-buffering

	; check which VADDR to use

	spd_inner_flags_dbl_buff_state
	beq .use_main_VADDR		; use the main VADDR

	stw <__spr_VADDR_dbl, <__di	; use opposite VADDR

	bra .cont_load_SG_data

.use_main_VADDR:

;--- DBL-BUFF ---

	stw <__spr_VADDR, <__di

.cont_load_SG_data:

	spd_get_SATB_flag SPD_FLAG_PEND_SG_DATA
	bne .copy_SG_data_params

.ifdef	SPD_DEBUG
	spd_dbg_border_load_VRAM
.endif
	jsr load_vram

.exit:

.ifdef	SPD_DEBUG
	spd_dbg_border_black
.endif

	ldx #( 1 | SPD_SG_NEW_DATA )
	cla
	
	rts

.copy_SG_data_params:

	; copy SG data parameters for delayed use on VBLANK

	stw __SG_DATA_SRC_ADDR, <__ax
	spd_stw_zpii <__si, <__ax

	stw __SG_DATA_SRC_BANK, <__ax
	spd_stb_zpii <__bl, <__ax

	stw __SG_DATA_DST_ADDR, <__ax
	spd_stw_zpii <__di, <__ax

	stw __SG_DATA_LEN, <__ax
	spd_stw_zpii <__cx, <__ax

	jmp .exit

	.endp

;// spd_SG_data_params( word __ax / src_addr, word __bx / src_bank, word __cx / dst_addr, word __dx / len )
;
	.proc _spd_SG_data_params.4

	stw <__ax, __SG_DATA_SRC_ADDR
	stw <__bx, __SG_DATA_SRC_BANK
	stw <__cx, __SG_DATA_DST_ADDR
	stw <__dx, __SG_DATA_LEN

	rts

	.endp

;// spd_copy_SG_data_to_VRAM( word __ax / src_addr, byte __bl / src_bank, word __dx / dst_addr, word __cx / len )
;
	.proc _spd_copy_SG_data_to_VRAM.4

	stw <__ax, <__si
	stw <__dx, <__di

.ifdef	SPD_DEBUG
	spd_dbg_border_load_VRAM

	jsr load_vram

	spd_dbg_border_black

	rts
.else
	jmp load_vram
.endif
	.endp

;// spd_copy_SG_data_to_VRAM( farptr __bl:__si / addr, byte __bh / index )
;
	.proc _spd_copy_SG_data_to_VRAM.2

	; offset x3 -> word:addr, byte:bank

	ldy <__bh
	spd_mul3_y <__dx

	call _spd_farptr_add_offset

	jsr map_data

	; __si - spd_SPRITE addr
	; __bl - spd_SPRITE bank

	cly

	spd_stw_zpii_rev __si, <__dx	; __dx - spd_SPRITE addr
	iny
	spd_stb_zpii_rev __si, <__bl	; __bl - spd_SPRITE bank

	stw <__dx, <__si		; __si - spd_SPRITE addr

	jsr unmap_data

	call _spd_copy_SG_data_to_VRAM.1

	rts

	.endp

;// spd_copy_SG_data_to_VRAM( farptr __bl:__si / addr )
;
	.proc _spd_copy_SG_data_to_VRAM.1

	jsr map_data			; map spd_SPRITE data

	ldy #$02
	lda [<__si], y
	tay				; Y - SG bank index

	jsr unmap_data

	call _load_SG_data

	rts

	.endp

;// spd_SATB_set_sprite_LT( farptr __bl:__si / addr, byte __bh / index, word __ax / x_pos, word __cx / y_pos )
;
	.proc _spd_SATB_set_sprite_LT.4

.ifdef	SPD_DEBUG
	spd_dbg_border_push_sprite
.endif

	; offset x3 -> word:addr, byte:bank

	ldy <__bh
	spd_mul3_y <__dx

	call _spd_farptr_add_offset

	jsr map_data

	; __si - spd_SPRITE addr
	; __bl - spd_SPRITE bank

	cly

	spd_stw_zpii_rev __si, <__dx	; __dx - spd_SPRITE addr
	iny
	spd_stb_zpii_rev __si, <__bl	; __bl - spd_SPRITE bank

	stw <__dx, <__si		; __si - spd_SPRITE addr

	jsr unmap_data

	call _spd_SATB_set_sprite_LT.3

	rts

	.endp

;// spd_SATB_set_sprite_LT( farptr __bl:__si / addr, word __ax / x_pos, word __cx / y_pos )
;
	.proc _spd_SATB_set_sprite_LT.3

.ifdef	SPD_DEBUG
	spd_dbg_border_push_sprite
.endif

	; XY coordinates correction

	lda #32				;2
	clc				;2
	adc <__ax			;4
	sta <__spr_pos_x		;4
	cla				;2
	adc <__ax + 1			;4
	sta <__spr_pos_x + 1		;4 = (22)

	lda #64
	clc
	adc <__cx
	sta <__spr_pos_y
	cla
	adc <__cx + 1
	sta <__spr_pos_y + 1

	; map spd_SPRITE data

	jsr map_data

	; get meta-sprite SG bank index

	ldy #2
	lda [<__si], y
	tay				; Y - SG bank index

	; move to the attributes data

	lda #3
	spd_add_a_to_word <__si		; __si - points to attributes

	phy				; Y - SG bank index

	; transform XY coordinates

.ifdef	SPD_DEBUG
	phy
	spd_dbg_border_attrs_transf
	ply
.endif

	ldy #1

	; transform XY coordinates for main VADDR

	; attr.Y += __spr_pos_y

	lda [<__si], y			;7 high byte
	tax				;2
	dey				;2
	lda [<__si], y			;7 low byte

	clc				;2
	adc <__spr_pos_y		;4
	sta [<__spr_SATB_addr], y	;7
	txa				;2
	adc <__spr_pos_y + 1		;4
	iny				;2
	sta [<__spr_SATB_addr], y	;7 = [46]

	iny				;2 = 50

	; attr.X += __spr_pos_x

	iny
	lda [<__si], y
	tax
	dey
	lda [<__si], y

	clc
	adc <__spr_pos_x
	sta [<__spr_SATB_addr], y
	txa
	adc <__spr_pos_x + 1
	iny
	sta [<__spr_SATB_addr], y

;--- SPD_FLAG_ALT_VADDR ---
	spd_get_SATB_flag SPD_FLAG_ALT_VADDR
	bne ._transf_pttrn_code

	; copy pattern code and palette +CGX/CGY

	iny				;2
	lda [<__si], y			;7
	sta [<__spr_SATB_addr], y	;7 = (16)

	iny
	lda [<__si], y
	sta [<__spr_SATB_addr], y

	iny
	lda [<__si], y
	sta [<__spr_SATB_addr], y

	iny
	lda [<__si], y
	sta [<__spr_SATB_addr], y

	bra ._push_SG_data

._transf_pttrn_code:

;--- SPD_FLAG_ALT_VADDR ---

	; transform pattern code for alternative VADDR

	iny

	; attr.SG_ind += __spr_alt_SG_offset

	iny
	lda [<__si], y
	tax
	dey
	lda [<__si], y

	clc
	adc <__spr_alt_SG_offset
	sta [<__spr_SATB_addr], y
	txa
	adc <__spr_alt_SG_offset + 1
	iny
	sta [<__spr_SATB_addr], y

	; copy palette and CGX/CGY

	iny
	lda [<__si], y
	sta [<__spr_SATB_addr], y

	iny
	lda [<__si], y
	sta [<__spr_SATB_addr], y

._push_SG_data:

.ifdef	SPD_DEBUG
	spd_dbg_border_push_sprite
.endif

	jsr unmap_data

	ply				; Y - SG bank index

;--- SPD_FLAG_IGNORE_SG ---
	spd_get_SATB_flag SPD_FLAG_IGNORE_SG
	bne .ignore_SG_data
;--- SPD_FLAG_IGNORE_SG ---

	; check if SG data already loaded to VRAM

	cpy <__last_SG_bank
	bne .load_SG_data

.ignore_SG_data:

	; SG data already loaded or must be ignored

.ifdef	SPD_DEBUG
	spd_dbg_border_black
.endif

	ldx #1
	cla

	rts

.load_SG_data:

	sty <__last_SG_bank

	; __dx = SG bank index x6 ( .word <data_length>, chrN, bank(chrN) )

	spd_mul6_y <__dx

	stw <__spr_SG_data_addr, <__si
	lda <__spr_SG_data_bank
	sta <__bl

	call _spd_farptr_add_offset

	jsr map_data			; map SG data array

	; __cx = SG data length

	ldy #$01
	spd_stw_div2_zpii_rev __si, <__cx

	; __ax = SG data address

	ldy #$02
	spd_stw_zpii_rev __si, <__ax

	; __bl = SG data bank

	iny
	lda [<__si], y
	tax

	jsr unmap_data

	stx <__bl

	; __si = __ax

	stw <__ax, <__si

	; __di = VADDR
	
	stw <__spr_VADDR, <__di

	spd_get_SATB_flag SPD_FLAG_PEND_SG_DATA
	beq .cont_SG_data_load

	; copy SG data parameters for delayed use on VBLANK

	stw __SG_DATA_SRC_ADDR, <__ax
	spd_stw_zpii <__si, <__ax

	stw __SG_DATA_SRC_BANK, <__ax
	spd_stb_zpii <__bl, <__ax

	stw __SG_DATA_DST_ADDR, <__ax
	spd_stw_zpii <__di, <__ax

	stw __SG_DATA_LEN, <__ax
	spd_stw_zpii <__cx, <__ax

	bra .exit

.cont_SG_data_load:

.ifdef	SPD_DEBUG
	spd_dbg_border_load_VRAM
.endif
	jsr load_vram

.exit:

.ifdef	SPD_DEBUG
	spd_dbg_border_black
.endif

	ldx #( 1 | SPD_SG_NEW_DATA )
	cla
	
	rts

	.endp

__mul3_lb_LUT:
	.byte $00,$03,$06,$09,$0C,$0F,$12,$15,$18,$1B,$1E,$21,$24,$27,$2A,$2D
	.byte $30,$33,$36,$39,$3C,$3F,$42,$45,$48,$4B,$4E,$51,$54,$57,$5A,$5D
	.byte $60,$63,$66,$69,$6C,$6F,$72,$75,$78,$7B,$7E,$81,$84,$87,$8A,$8D
	.byte $90,$93,$96,$99,$9C,$9F,$A2,$A5,$A8,$AB,$AE,$B1,$B4,$B7,$BA,$BD
	.byte $C0,$C3,$C6,$C9,$CC,$CF,$D2,$D5,$D8,$DB,$DE,$E1,$E4,$E7,$EA,$ED
	.byte $F0,$F3,$F6,$F9,$FC,$FF,$02,$05,$08,$0B,$0E,$11,$14,$17,$1A,$1D
	.byte $20,$23,$26,$29,$2C,$2F,$32,$35,$38,$3B,$3E,$41,$44,$47,$4A,$4D
	.byte $50,$53,$56,$59,$5C,$5F,$62,$65,$68,$6B,$6E,$71,$74,$77,$7A,$7D
	.byte $80,$83,$86,$89,$8C,$8F,$92,$95,$98,$9B,$9E,$A1,$A4,$A7,$AA,$AD
	.byte $B0,$B3,$B6,$B9,$BC,$BF,$C2,$C5,$C8,$CB,$CE,$D1,$D4,$D7,$DA,$DD
	.byte $E0,$E3,$E6,$E9,$EC,$EF,$F2,$F5,$F8,$FB,$FE,$01,$04,$07,$0A,$0D
	.byte $10,$13,$16,$19,$1C,$1F,$22,$25,$28,$2B,$2E,$31,$34,$37,$3A,$3D
	.byte $40,$43,$46,$49,$4C,$4F,$52,$55,$58,$5B,$5E,$61,$64,$67,$6A,$6D
	.byte $70,$73,$76,$79,$7C,$7F,$82,$85,$88,$8B,$8E,$91,$94,$97,$9A,$9D
	.byte $A0,$A3,$A6,$A9,$AC,$AF,$B2,$B5,$B8,$BB,$BE,$C1,$C4,$C7,$CA,$CD
	.byte $D0,$D3,$D6,$D9,$DC,$DF,$E2,$E5,$E8,$EB,$EE,$F1,$F4,$F7,$FA,$FD

__mul3_hb_LUT:
	.byte $00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00
	.byte $00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00
	.byte $00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00
	.byte $00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00
	.byte $00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00,$00
	.byte $00,$00,$00,$00,$00,$00,$01,$01,$01,$01,$01,$01,$01,$01,$01,$01
	.byte $01,$01,$01,$01,$01,$01,$01,$01,$01,$01,$01,$01,$01,$01,$01,$01
	.byte $01,$01,$01,$01,$01,$01,$01,$01,$01,$01,$01,$01,$01,$01,$01,$01
	.byte $01,$01,$01,$01,$01,$01,$01,$01,$01,$01,$01,$01,$01,$01,$01,$01
	.byte $01,$01,$01,$01,$01,$01,$01,$01,$01,$01,$01,$01,$01,$01,$01,$01
	.byte $01,$01,$01,$01,$01,$01,$01,$01,$01,$01,$01,$02,$02,$02,$02,$02
	.byte $02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02
	.byte $02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02
	.byte $02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02
	.byte $02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02
	.byte $02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02,$02

	.endprocgroup

#endasm