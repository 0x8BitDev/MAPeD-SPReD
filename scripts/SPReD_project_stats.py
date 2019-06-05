#################################################################################################
#
# Copyright 2019 0x8BitDev ( MIT license )
#
# This is an example of using of SPReD API functions which are available for custom data export
#
#################################################################################################

def dump_CHR_bank( _CHR_bank ):
	if _CHR_bank != None:
		print '\n\t\t\tCHR_bank:'
		# link_cnt - is a number of sprites which share the same CHR data
		print '\t\t\tid: ' + str( _CHR_bank.id ) + ' link_count: ' + str( _CHR_bank.link_cnt )
# dump CHR arrays ( 8x8 tiles )
#		if _CHR_bank.CHR_data != None:
#			for chr_n in xrange( _CHR_bank.CHR_data.Count ):
#				print '\t\t\t\tArray size: ' + str( _CHR_bank.CHR_data[ chr_n ].data.Count ) + ' -> ' + str( _CHR_bank.CHR_data[ chr_n ].data )

def dump_sprite_attrs( _attrs_arr ):
	if _attrs_arr != None:
		print '\n\t\t\tAttributes:'
		for attr_n in xrange( _attrs_arr.Count ):
			sprite_attr = _attrs_arr[ attr_n ]
			print '\t\t\tx: ' + str( sprite_attr.x ) + ' y: ' + str( sprite_attr.y ) + ' palette_ind: ' + str( sprite_attr.palette_ind ) + ' CHR_ind: ' + str( sprite_attr.CHR_ind ) + ' flip_flag: ' + str( sprite_attr.flip_flag )

def dump_sprite_data( _spr ):
	print '\n\t' + _spr.name
	print '\t\toffset_x: ' + str( _spr.offset_x )
	print '\t\toffset_y: ' + str( _spr.offset_y )
	print '\t\twidth: ' + str( _spr.width )
	print '\t\theight: ' + str( _spr.height )
	dump_sprite_attrs( _spr.attrs )
	dump_CHR_bank( _spr.CHR_bank )

print 'The current project stats:\n'

print '8x16_mode: ' + str( spd_8x16_mode_enabled() ) + '\n'

# Dump palette values ( 4x4 )
for plt_n in xrange( 4 ):
	print 'Palette' + str( plt_n ) + ': ' + str( spd_get_palette( plt_n ) )

# Dump sprites data
num_sprites = spd_num_sprites();
print '\nNumber of sprites: ' + str( num_sprites )
for spr_n in xrange( num_sprites ):
	dump_sprite_data( spd_get_sprite_data( spr_n ) )