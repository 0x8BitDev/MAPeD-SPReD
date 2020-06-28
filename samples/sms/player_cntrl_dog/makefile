################################################
#                
# Copyright 2019-2020 0x8BitDev ( MIT license )
#
################################################

out_file = player_cntrl_dog
out_dir = ../bin/
inc_dir1 = ../common/
inc_dir2 = ./data/

WLAZ80 = wla-z80
WLALINK = wlalink
EMU = mednafen

$(out_file).sms: $(out_file).o
	$(WLALINK) -i -r -v main.link $(out_dir)$(out_file).sms

$(out_file).o: main.asm
	$(WLAZ80) -I $(inc_dir1) -I $(inc_dir2) -o $(out_dir)$(out_file).o $<

.PHONY: run clean

clean:
	rm $(out_dir)$(out_file).o $(out_dir)$(out_file).sms

run: $(out_dir)$(out_file).sms
	$(EMU) $<
