:################################################
:#
:# Copyright 2019-2022 0x8BitDev ( MIT license )
:#
:################################################

@set OutFile=tilemap_render_stat_scr_VDC_data
@set OutDir=..\..\..\bin\

@del %OutDir%%OutFile%.pce

@echo compiling...
pceas -S -O main.asm
@if ERRORLEVEL 1 goto failure
@echo Ok!

copy main.pce %OutDir%%OutFile%.pce

@del *.pce *.sym

%OutDir%%OutFile%.pce
@goto exit

:failure
@echo Build error!
:exit