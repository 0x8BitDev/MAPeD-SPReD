################################################
#
# Copyright 2022 0x8BitDev ( MIT license )
#
################################################

out_file = animation_test_huc3
out_dir = ../../../bin/

EMU = mednafen
SRC = main.c
CFLAGS ?= -fno-recursive -msmall

CC = PCE_PCEAS=pceas huc

main.pce: $(SRC)
	$(CC) $(CFLAGS) $(SRC)
	mv -f main.pce $(out_dir)$(out_file).pce

.PHONY: run clean

clean:
	rm -f $(out_dir)$(out_file).pce *.lst *.sym *.s

run: $(out_dir)$(out_file).pce
	$(EMU) $<