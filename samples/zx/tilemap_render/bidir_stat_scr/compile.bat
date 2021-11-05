:################################################
:#
:# Copyright 2018-2021 0x8BitDev ( MIT license )
:#
:################################################

@set File=bidir_stat_scr
@set OutDir=..\..\bin\

@del %OutDir%%File%.sna

@echo compiling...
sjasmplus main.asm
@if ERRORLEVEL 1 goto failure

%OutDir%%File%.sna
@goto exit

:failure
@echo Build error!
:exit