﻿/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 30.07.2020
 * Time: 13:48
 */
using System;

namespace MAPeD
{
	/// <summary>
	/// Description of strings.
	/// </summary>
	public static class strings
	{
		public const string CONST_STR_EXP_NES_RLE_COMPRESSION 	= " (RLE COMPRESSION)\nCompressed tiles data must be decompressed to any free RAM address.\nCompressed PPU-ready data can be decompressed directly at the appropriate PPU memory.";
		
		public const string CONST_STR_EXP_RLE_COMPRESSION 		= " (RLE COMPRESSION)\nThe map data will be compressed.";
		
		public const string CONST_STR_EXP_DATA_ORDER			= "\nDATA ORDER: ";
		
		public const string CONST_STR_EXP_NES_DATA_ORDER_COLS	= "Columns\nAll the map\\screens data are stored in a column order except of PPU-ready data ( static screens mode ).";
		public const string CONST_STR_EXP_NES_DATA_ORDER_ROWS	= "Rows\nAll the map\\screens data are stored in a row order.";

		public const string CONST_STR_EXP_SMS_DATA_ORDER_COLS	= "Columns\nAll the map\\screens data are stored in a column order except of VDP-ready data ( static screens mode ).";
		public const string CONST_STR_EXP_SMD_DATA_ORDER_COLS	= CONST_STR_EXP_SMS_DATA_ORDER_COLS; 
		public const string CONST_STR_EXP_PCE_DATA_ORDER_COLS	= "Columns\nAll the map\\screens data are stored in a column order except of VDC-ready data ( static screens mode ).";
		
		public const string CONST_STR_EXP_DATA_ORDER_COLS		= "Columns\nAll the map\\screens data are stored in a column order.";
		public const string CONST_STR_EXP_DATA_ORDER_ROWS		= "Rows\nAll the map\\screens data are stored in a row order.";
		
		public const string CONST_STR_EXP_ZX_DATA_ORDER_COLS	= "Columns\nAll the map data are stored in a column order.";
		
		public const string CONST_STR_EXP_NES_ATTRS				= "\n\nATTRIBUTES per "; 
		public const string CONST_STR_EXP_NES_ATTRS_PER_BLOCK	= "BLOCK. This is a usual case.";
		public const string CONST_STR_EXP_NES_ATTRS_PER_CHR		= "CHR. MMC5 extended attributes mode. Also you can specify a base CHR bank index (4K) for tiles index expansion ( see MMC5 guide for details ).";
		
		public const string CONST_STR_EXP_TILES_2X2				= "TILES: 2x2";
		public const string CONST_STR_EXP_TILES_4X4				= "TILES: 4x4";

		public const string CONST_STR_EXP_ZX_TILES_2X2			= "TILES: 2x2\nThe CHR graphics is stored as array of tiles 2x2. The color information is stored in a separate file, if the 'Color' option is checked.";
		public const string CONST_STR_EXP_ZX_TILES_4X4			= "TILES: 4x4\nThe CHR graphics is stored as array of tiles 2x2. The tiles 4x4 data are stored in a separate file as array of indices of tiles 2x2 (left to right, up to down). The color information is stored in a separate file, if the 'Color' option is checked.";
		
		public const string CONST_STR_EXP_PROP					= "\n\nPROPERTY Id per "; 
		public const string CONST_STR_EXP_PROP_PER_BLOCK		= "BLOCK ( 1 byte per block )\nThe top left CHR property of each block will be used.";
		public const string CONST_STR_EXP_PROP_PER_CHR			= "CHR ( 4 bytes per block )";

		public const string CONST_STR_EXP_SMS_MOVE_PROP_TO_SCR_MAP_ON	= "\nThe properties 0-7 (3 bits) will be moved to a screen map data.";
		public const string CONST_STR_EXP_SMS_MOVE_PROP_TO_SCR_MAP_OFF	= "\nAll the properties will be stored in a separate file.";
		
		public const string CONST_STR_EXP_PROP_IN_FRONT_OF_SPRITES		= "\n'In front of sprites' property: What property will be used as the 'In front of sprites' flag in a screen map data.";
		public const string CONST_STR_EXP_SMD_PROP_IN_FRONT_OF_SPRITES	= "\n\n'Priority id' property: What block's property will be used as the 'Priority Bit' in a screen map data.";
		
		public const string CONST_STR_EXP_MODE					= "\n\nMODE: "; 
		public const string CONST_STR_EXP_MODE_MULTIDIR			= "Multidirectional scrolling\nAll screens data are stored in a common array of tiles. Suitable for map scrolling in any direction.";
		public const string CONST_STR_EXP_MODE_BIDIR			= "Bidirectional scrolling (max screens: {0})\nAll screens data are stored sequentially as tiles in a common array. Suitable for [bi/uni]directional scrolling.";
		
		public const string CONST_STR_EXP_ZX_MODE_MULTIDIR		= "\nUnlike to other platforms, ZX version can merge screens from different data banks into a single map.";
		
		public const string CONST_STR_EXP_NES_MODE_STAT_SCR		= "Static Screens (max screens: {0})\nAll screens data are stored sequentially in a common array. Graphics data are PPU-ready - 1024 bytes per screen ( 960 bytes of CHR data and 64 bytes of attributes ). Suitable for static screens switching.\nSwitching between 2x2/4x4 tiles makes sense when using blocks or CHRs to store tile properties.";
		
		public const string CONST_STR_EXP_MODE_STAT_SCR			= "Static Screens (max screens: {0})\nAll screens data are stored sequentially in a common array. Graphics data are <data> bytes per screen. The data can be compressed. Suitable for static screens switching.\nSwitching between 2x2/4x4 tiles makes sense when using blocks or CHRs to store tile properties.";

		public const string CONST_STR_EXP_LAYOUT				= "\n\nLAYOUT: "; 
		public const string CONST_STR_EXP_LAYOUT_ADJ_SCR		= "Adjacent Screens\nEach screen description stores 4 labels of adjacent screens ( calculates automatically during the export process ).";
		public const string CONST_STR_EXP_LAYOUT_ADJ_SCR_INDS	= "Adjacent Screen indices\nEach screen description stores 4 indices of adjacent screens ( calculates automatically during the export process ) in a screens array. It allows to save up to 25% of memory on adjacent screens data. The maximum number of screen cells ( NOT active screens! ) in a level layout: 256.";
		public const string CONST_STR_EXP_LAYOUT_MATRIX			= "Matrix\nEach level data are stored as matrix ( width x height ) of labels of screen descriptions.";
		
		public const string CONST_STR_EXP_MARKS					= "\nEXPORT MARKS\nEach mark stores a user defined mask of valid adjacent screens and a screen property value.";
		public const string CONST_STR_EXP_NO_MARKS				= "\nNO MARKS";
		
		public const string CONST_STR_EXP_ENTITIES				= "\n\nEXPORT ENTITIES";
		public const string CONST_STR_EXP_NO_ENTITIES			= "\n\nNO ENTITIES";
		
		public const string CONST_STR_EXP_ENT_COORDS			= "\nENTITY COORDINATES: ";
		public const string CONST_STR_EXP_ENT_COORDS_SCR		= "Screen space\nThe upper left corner of each screen is used as the origin of the coordinate space for an entity belonging to the screen."; 
		public const string CONST_STR_EXP_ENT_COORDS_MAP		= "Map space\nThe upper left corner of each level is used as the origin of the coordinate space for an entity.";
		
		public const string CONST_STR_EXP_ENT_SORTING			= "\nENTITIES SORTING: ";
		public static string[] CONST_STR_EXP_ENT_SORT_TYPES		= new string[] { "No sorting", "Left to right", "Bottom to top" };

		public const string CONST_STR_EXP_ZX_INK_FACTOR			= "\n\nNOTE: Try to change the 'Ink Factor' value to achieve the best result.";		
		
		public const string CONST_STR_EXP_CHR_OFFSET			= "\n\nCHR offset: This value will be added to each CHR index in a screen map. In other words, it's a free space at the beginning of a CHR bank.";		
		public const string CONST_STR_EXP_PCE_CHR_OFFSET		= "\n\nCHR offset: This value will be added to each CHR index in a screen map. In other words, it's a free space from the end of a BAT.";
		
		public const string CONST_STR_EXP_SMD_SGDK_DATA			= "\n\nExport SGDK Data: SGDK compatible '.h' file will be generated. The '.asm' file will be renamed to '.s'. Select ROOT directory of your project and all exported data will be automatically copied to the 'src/inc/data' directories.";
		
		public const string CONST_STR_EXP_WARNING				= "\n\nWARNING: To reduce the amount of exported data, please make a global data optimization.";
		
		public const string CONST_SCREEN_DATA_TYPE_INFO_CAPTION	= "Screen Data Type Info"; 
		public const string CONST_SCREEN_DATA_TYPE_INFO			= "Screen data can be stored as Tiles(4x4) or Blocks(2x2).\n\nTiles(4x4) are the most compact way to store screen data. Takes less memory and less graphics variety.\n\nBlocks(2x2) data takes up 4 times the memory, but brings more graphics variety. Suitable for dynamic maps.\n\nSwitching between these modes AFFECTS ALL THE CHR BANKS!";
		
		public const string CONST_SCREEN_DATA_CONV_BLOCKS2TILES_CAPTION	= "Screen Data Conversion: Blocks->Tiles";
		public const string CONST_SCREEN_DATA_CONV_BLOCKS2TILES = "The Tiles(4x4) list will be filled in with the screens data.\n\nWARNING: This operation affects all the CHR banks!\n\nAre you sure?";
		
		public const string CONST_SCREEN_DATA_CONV_TILES2BLOCKS_CAPTION	= "Screen Data Conversion: Tiles->Blocks";
		public const string CONST_SCREEN_DATA_CONV_TILES2BLOCKS = "All the Tiles(4x4) data will be used to update screens data and the Tiles(4x4) list will be cleared.\n\nWARNING: This operation affects all the CHR banks!\n\nAre you sure?";
	}
}
