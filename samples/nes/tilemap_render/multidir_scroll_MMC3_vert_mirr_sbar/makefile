################################################
#
# Copyright 2018-2020 0x8BitDev ( MIT license )
#
################################################

out_file = tilemap_render_multidir_scroll_MMC3_vert_mirr_sbar
out_dir = ../../bin/
inc_dir1 = ../../common/
inc_dir2 = ./data/
cfg = MMC3.cfg
curr_dir = ../multidir_scroll_MMC3_vert_mirr_sbar/
proj_dir = ../multidir_scroll_MMC3_vert_mirr/

CA65 = ca65
LD65 = ld65
EMU = fceux

$(out_file).nes: $(out_file).o
	cd $(curr_dir); \
	$(LD65) -o $(out_dir)$(out_file).nes -C $(cfg) $(out_dir)$(out_file).o

$(out_file).o: $(proj_dir)main.asm
	cd $(proj_dir); \
	$(CA65) -D mirror_vert=1 -D mmc3_status_bar=1 -I $(inc_dir1) -I $(inc_dir2) $< -o $(out_dir)$(out_file).o

.PHONY: run clean

clean:
	rm $(out_dir)$(out_file).o $(out_dir)$(out_file).nes

run: $(out_dir)$(out_file).nes
	$(EMU) $<
