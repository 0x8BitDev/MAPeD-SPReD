:################################################
:#
:# Copyright 2019-2022 0x8BitDev ( MIT license )
:#
:################################################

@set OutFile=tilemap_render_bidir_scroll
@set OutDir=..\..\..\bin\

@del %OutDir%%OutFile%.pce

@echo compiling...
huc -fno-recursive -msmall main.c
pceas main.s
@if ERRORLEVEL 1 goto failure
@echo Ok!

copy main.pce %OutDir%%OutFile%.pce

@del *.pce *.lst *.sym *.s

%OutDir%%OutFile%.pce
@goto exit

:failure
@echo Build error!
:exit