:################################################
:#
:# Copyright 2018-2019 0x8BitDev ( MIT license )
:#
:################################################

@set File=multidir_scroll
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