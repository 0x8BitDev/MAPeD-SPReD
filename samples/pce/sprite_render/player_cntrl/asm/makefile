################################################
#
# Copyright 2022 0x8BitDev ( MIT license )
#
################################################

out_file = player_cntrl_asm
out_dir = ../../../bin/

EMU = mednafen
SRC = main.asm
CFLAGS ?= -S

CC = pceas

main.pce: $(SRC)
	$(CC) $(CFLAGS) $(SRC)
	mv -f main.pce $(out_dir)$(out_file).pce

.PHONY: run clean

clean:
	rm -f $(out_dir)$(out_file).pce *.lst *.sym *.s

run: $(out_dir)$(out_file).pce
	$(EMU) $<