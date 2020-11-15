:################################################
:#
:# Copyright 2019-2020 0x8BitDev ( MIT license )
:#
:################################################

@set OutFile=tilemap_render_bidir_stat_scr_ADJ_SCR_MASKS
@set OutDir=..\..\bin\
@set IncDir1=..\..\common\
@set IncDir2=.\data\

@del %OutDir%%OutFile%.sms

@echo compiling...
cd ..\bidir_scroll_ADJ_SCR_MASKS
wla-z80 -I %IncDir1% -I %IncDir2% -D TR_BIDIR_STAT_SCR -o %OutDir%%OutFile%.o main.asm
@if ERRORLEVEL 1 goto failure

@echo linking...
cd ..\bidir_stat_scr_ADJ_SCR_MASKS
wlalink -i -r -v main.link %OutDir%%OutFile%.sms
@if ERRORLEVEL 1 goto failure
@echo Ok!

@del %OutDir%%OutFile%.o

@cd %OutDir%
%OutFile%.sms
@goto exit

:failure
@echo Build error!
:exit