################################################
#
# Copyright 2018-2019 0x8BitDev ( MIT license )
#
################################################

out_file = tilemap_render_static_screens_MMC5
out_dir = ../../bin/
inc_dir1 = ../../common/
inc_dir2 = ./data/
cfg = MMC5.cfg

CA65 = ca65
LD65 = ld65
EMU = fceux

$(out_file).nes: $(out_file).o
	$(LD65) -o $(out_dir)$(out_file).nes -C $(cfg) $(out_dir)$(out_file).o

$(out_file).o: main.asm
	$(CA65) -I $(inc_dir1) -I $(inc_dir2) $< -o $(out_dir)$(out_file).o

.PHONY: run clean

clean:
	rm $(out_dir)$(out_file).o $(out_dir)$(out_file).nes

run: $(out_dir)$(out_file).nes
	$(EMU) $<
