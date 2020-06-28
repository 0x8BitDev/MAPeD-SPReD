:################################################
:#
:# Copyright 2019-2020 0x8BitDev ( MIT license )
:#
:################################################

@set OutFile=show_sprite
@set OutDir=..\bin\
@set IncDir1=..\common\
@set IncDir2=.\data\

@del %OutDir%%OutFile%.sms

@echo compiling...
wla-z80 -I %IncDir1% -I %IncDir2% -o %OutDir%%OutFile%.o main.asm
@if ERRORLEVEL 1 goto failure

@echo linking...
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