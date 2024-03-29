#################################################################################################
#
# Copyright 2019-2021 0x8BitDev ( MIT license )
#
# This is an example of using of MAPeD API functions which are available for custom data export
# ( see 'doc\MAPeD_Data_Export_Python_API.html' for details )
#
#################################################################################################

def dump_inst_entity( _ent ):
	print '\t\tuid: ' + str( _ent.uid )
	print '\t\ttarget_uid: ' + str( _ent.target_uid )
	print '\t\tbase_ent_uid: ' + str( _ent.base_ent_uid )
	print '\t\tx: ' + str( _ent.x )
	print '\t\ty: ' + str( _ent.y )
	print '\t\tproperties: ' + _ent.properties + '\n'

def dump_base_entity( _ent ):
	print '\t\tname: ' + _ent.name
	print '\t\tuid: ' + str( _ent.uid )
	print '\t\twidth: ' + str( _ent.width )
	print '\t\theight: ' + str( _ent.height )
	print '\t\tpivot_x: ' + str( _ent.pivot_x )
	print '\t\tpivot_y: ' + str( _ent.pivot_y )
	print '\t\tproperties: ' + _ent.properties
	print '\t\tinst_properties: ' + _ent.inst_properties + '\n'

# Get the API version
api_ver = mpd_api_ver()
print 'API ver: ' + str( api_ver >> 8 ) + '.' + str( api_ver & 0xff )

print '*** The active project stats: ***\n'
num_banks = mpd_num_banks()
print 'Number of graphics banks: ' + str( num_banks )
print 'The active bank is ' + str( mpd_get_active_bank() )

# Run through all graphics banks
# Each graphics bank contains: CHR bank, blocks (256), tiles (256), palettes and screens
for bank_n in xrange( num_banks ):
	print '\tBank: ' + str( bank_n )
	for slot_n in xrange( mpd_get_palette_slots( bank_n ) ):
		print '\t\tPalette' + str( slot_n ) + ':'
		for plt_n in xrange( 4 ):
			print '\t\t\t' + str( plt_n ) + ': ' + str( mpd_get_palette( bank_n, plt_n, slot_n ) )

# CHRs/Blocks/Tiles count
	print '\n\t\tCHRs(1x1):\t' + str( mpd_get_CHRs_cnt( bank_n ) )
	print '\t\tBlocks(2x2):\t' + str( mpd_get_blocks_cnt( bank_n ) )
	print '\t\tTiles(4x4):\t' + str( mpd_get_tiles_cnt( bank_n ) )

# CHR bank
# CHR data is stored as image 128 x 128 x Pages count (8bit)
# Use mpd_get_CHRs_data( int _bank_n ) to get CHR's raw data
# NES: Use mpd_export_CHR_data( int _bank_n, string _filename, bool _need_padding ) to export NES-ready data
# SMS: Use mpd_export_CHR_data( int _bank_n, string _filename, int _bpp ) to export SMS-ready data
# PCE/ZX: Use mpd_export_CHR_data( int _bank_n, string _filename ) to export platform-ready data
	CHR_data = mpd_get_CHRs_data( bank_n )
	print '\n\t\tCHR data size: ' + str( CHR_data.Count ) + ' --> Array[Byte]' # + str( CHR_data )

# Dump 2x2 tiles (blocks) data
# Each block's value (UInt32) has the following bits description: 
# NES: 15.. [ property_id ](4) [ palette ind ](2) [X](2) [ CHR ind ](8) ..0
# SMS: 15.. [ property_id ](4) [ hv_flip ](2) [ palette ind ](1) [CHR ind](9) ..0
# PCE: 19.. [ property_id ](4) [ palette ind ](4) [CHR ind](12) ..0
# ZX: 15.. [ property_id ](4) [ palette ind ](1) [CHR ind](11)..0
# Four UInt32 values form one 2x2 tile (block)
	blocks_data = mpd_get_blocks( bank_n );
	print '\n\t\tBlocks: Array size: ' + str( blocks_data.Count ) + ' --> ' + str( blocks_data )

# Dump 4x4 tiles data
# Each tile value (UInt32) consists of blocks indices ordered left to right, up to down from low to high byte
	tiles_data = mpd_get_tiles( bank_n )
	print '\n\t\tTiles: Array size: ' + str( tiles_data.Count ) + ' --> ' + str( tiles_data )

# Dump screens data
# Each screen data consist of byte array of 4x4 or 2x2 tiles ordered left to right, up to down
	screen_mode_str = ''
	if mpd_screen_mode():
		screen_mode_str = 'Blocks2x2'
	else:
		screen_mode_str = 'Tiles4x4'

	n_screens = mpd_num_screens( bank_n )
	print '\n\t\tNumber of screens: ' + str( n_screens ) + ' / ' + screen_mode_str
	for scr_n in xrange( n_screens ):
		scr_data = mpd_get_screen_data( bank_n, scr_n )
		print '\t\tScreen' + str( scr_n ) + ': Array size: ' + str( scr_data.Count ) + ' --> ' + str( scr_data )

# Run through layouts
num_layouts = mpd_num_layouts()
print '\nNumber of layouts: ' + str( num_layouts )
for layout_n in xrange( num_layouts ):
	layout_width = mpd_layout_width( layout_n )
	layout_height = mpd_layout_height( layout_n )
	start_screen_cell = mpd_layout_start_screen_cell( layout_n )
	print '\tLayout' + str( layout_n ) + ': width: ' + str( layout_width ) + ' / height: ' + str( layout_height ) + ' ; Start screen cell: ' + str( start_screen_cell ) + ' ; Start screen ind: ' + ( str( mpd_layout_screen_ind( layout_n, start_screen_cell % layout_width, start_screen_cell / layout_width  ) ) if start_screen_cell >= 0 else str( start_screen_cell ) )

# Dump layout data
	for cell_y in xrange( layout_height ):
		for cell_x in xrange( layout_width ):
			num_screen_entities = mpd_layout_screen_num_entities( layout_n, cell_x, cell_y );
			print '\t\tCell: ' + str( cell_x + cell_y * layout_width ) + ' -> Screen index: ' + str( mpd_layout_screen_ind( layout_n, cell_x, cell_y ) ) + ' ; Marks: ' + str( hex( mpd_layout_screen_marks( layout_n, cell_x, cell_y ) ) ) + ' ; Entities: ' + str( num_screen_entities )
			
# Dump entity instances
			for ent_n in xrange( num_screen_entities ):
				dump_inst_entity( mpd_layout_get_inst_entity( layout_n, cell_x, cell_y, ent_n ) )

# Dump base entities
print '\n*** Base entities: ***'
ent_group_names = mpd_group_names_of_entities()
print 'Number of groups: ' + str( ent_group_names.Count )
for group_n in xrange( ent_group_names.Count ):
	grp_name = ent_group_names[ group_n ]
	grp_num_ents = mpd_group_num_entities( grp_name )
	print '\t' + str( grp_name ) + '(' + str( grp_num_ents ) +  '):'
	for ent_n in xrange( grp_num_ents ):
		dump_base_entity( mpd_group_get_entity_by_ind( grp_name, ent_n ) )
