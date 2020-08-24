/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2020 ( MIT license. See LICENSE.txt )
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
		
		public const string CONST_STR_EXP_SMS_RLE_COMPRESSION 	= " (RLE COMPRESSION)\nThe map data will be compressed.";
		
		public const string CONST_STR_EXP_DATA_ORDER			= "\nDATA ORDER: ";
		
		public const string CONST_STR_EXP_NES_DATA_ORDER_COLS	= "Columns\nAll the map\\screens data are stored in a column order except of PPU-ready data ( static screens mode ).";
		public const string CONST_STR_EXP_NES_DATA_ORDER_ROWS	= "Rows\nAll the map\\screens data are stored in a row order.";

		public const string CONST_STR_EXP_SMS_DATA_ORDER_COLS	= "Columns\nAll the map\\screens data are stored in a column order except of VDP-ready data ( static screens mode ).";
		public const string CONST_STR_EXP_SMS_DATA_ORDER_ROWS	= "Rows\nAll the map\\screens data are stored in a row order.";
		
		public const string CONST_STR_EXP_ZX_DATA_ORDER_COLS	= "Columns\nAll the map data are stored in a column order.";
		
		public const string CONST_STR_EXP_NES_ATTRS				= "\n\nATTRIBUTES per "; 
		public const string CONST_STR_EXP_NES_ATTRS_PER_BLOCK	= "BLOCK. This is a usual case.";
		public const string CONST_STR_EXP_NES_ATTRS_PER_CHR		= "CHR. MMC5 extended attributes mode. Also you can specify a base CHR bank index (4K) for tiles index expansion ( see MMC5 guide for details ).";
		
		public const string CONST_STR_EXP_TILES_2X2				= "TILES: 2x2";
		public const string CONST_STR_EXP_TILES_4X4				= "TILES: 4x4";
		
		public const string CONST_STR_EXP_PROP					= "\n\nPROPERTIES Id per "; 
		public const string CONST_STR_EXP_PROP_PER_BLOCK		= "BLOCK ( 1 byte per block )\nThe top left CHR property of each block will be used.";
		public const string CONST_STR_EXP_PROP_PER_CHR			= "CHR ( 4 bytes per block )";

		public const string CONST_STR_EXP_SMS_MOVE_PROP_TO_SCR_MAP_ON	= "\nProperties 0-7 (3 bits) will be moved to a screen map data.";
		public const string CONST_STR_EXP_SMS_MOVE_PROP_TO_SCR_MAP_OFF	= "\nAll properties will be stored in a separate file.";
		
		public const string CONST_STR_EXP_PROP_IN_FRONT_OF_SPRITES	= "\n'In front of sprites' property: Which property will be used as the 'In front of sprites' flag in a screen map data.";
		
		public const string CONST_STR_EXP_MODE					= "\n\nMODE: "; 
		public const string CONST_STR_EXP_MODE_MULTIDIR			= "Multidirectional scrolling\nAll screens data are stored in a common array of tiles. Suitable for map scrolling in any direction.";
		public const string CONST_STR_EXP_MODE_BIDIR			= "Bidirectional scrolling\nAll screens data are stored sequentially as tiles in a common array. Suitable for [bi/uni]directional scrolling.";
		
		public const string CONST_STR_EXP_NES_MODE_STAT_SCR		= "Static Screens\nAll screens data are stored sequentially in a common array. Graphics data are PPU-ready - 1024 bytes per screen ( 960 bytes of CHR data and 64 bytes of attributes ). Suitable for static screens switching.";
		
		public const string CONST_STR_EXP_SMS_MODE_STAT_SCR		= "Static Screens\nAll screens data are stored sequentially in a common array. Graphics data are VDP-ready - 1536 bytes per screen. Suitable for static screens switching.";

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

		public const string CONST_STR_EXP_ZX_INK_FACTOR			= "\n\nNOTE: Try to change the 'Ink Factor' value to achieve the best result.";		
		
		public const string CONST_STR_EXP_CHR_OFFSET			= "\n\nCHR offset: This value will be added to each CHR index in a screen map. In other words, it's a free space at the beginning of a CHR bank.";
		
		public const string CONST_STR_EXP_WARNING				= "\n\nWARNING: To reduce the amount of exported data, please make a global data optimization.";
	}
}
