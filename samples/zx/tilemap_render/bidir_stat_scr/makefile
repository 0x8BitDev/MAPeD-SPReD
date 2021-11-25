################################################
#
# Copyright 2021 0x8BitDev ( MIT license )
#
################################################

out_file = tilemap_render_bidir_stat_scr
out_dir = ../../bin/

ASM = sjasmplus
EMU = fuse

asm_file: main.asm
	$(ASM) $<

.PHONY: run clean

clean:
	rm $(out_dir)$(out_file).sna

run: $(out_dir)$(out_file).sna
	$(EMU) $<
