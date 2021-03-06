:################################################
:#
:# Copyright 2018-2020 0x8BitDev ( MIT license )
:#
:################################################

@set OutFile=tilemap_render_bidir_scroll_MMC1_vert_mirr
@set OutDir=..\..\bin\
@set IncDir1=..\..\common\
@set IncDir2=.\data\

@del %OutDir%%OutFile%.nes

@echo compiling...
ca65 -I %IncDir1% -I %IncDir2% main.asm  -o %OutDir%%OutFile%.o
@if ERRORLEVEL 1 goto failure

@echo linking...
ld65 -o %OutDir%%OutFile%.nes -C MMC1.cfg %OutDir%%OutFile%.o
@if ERRORLEVEL 1 goto failure
@echo Ok!

@del %OutDir%%OutFile%.o

@cd %OutDir%
%OutFile%.nes
@goto exit

:failure
@echo Build error!
:exit