################################################
#
# Copyright 2019-2020 0x8BitDev ( MIT license )
#
################################################

out_file = tilemap_render_bidir_stat_scr_ADJ_SCR_MASKS
out_dir = ../../bin/
inc_dir1 = ../../common/
inc_dir2 = ./data/
curr_dir = ../bidir_stat_scr_ADJ_SCR_MASKS/
proj_dir = ../bidir_scroll_ADJ_SCR_MASKS/

WLAZ80 = wla-z80
WLALINK = wlalink
EMU = mednafen

$(out_file).sms: $(out_file).o
	cd $(curr_dir); \
	$(WLALINK) -i -r -v main.link $(out_dir)$(out_file).sms

$(out_file).o: $(proj_dir)main.asm
	cd $(proj_dir); \
	$(WLAZ80) -I $(inc_dir1) -I $(inc_dir2) -D TR_BIDIR_STAT_SCR -o $(out_dir)$(out_file).o $<

.PHONY: run clean

clean:
	rm $(out_dir)$(out_file).o $(out_dir)$(out_file).sms

run: $(out_dir)$(out_file).sms
	$(EMU) $<
