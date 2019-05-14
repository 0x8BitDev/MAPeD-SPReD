:################################################
:#
:# Copyright 2018-2019 0x8BitDev ( MIT license )
:#
:################################################

@set File=tilemap_viewer
@set OutDir=..\bin\

@del %OutDir%%File%.sna

@echo compiling...
sjasmplus %File%.asm 
@if ERRORLEVEL 1 goto failure

@cd %OutDir%
%File%.sna
@goto exit

:failure
@echo Build error!
:exit