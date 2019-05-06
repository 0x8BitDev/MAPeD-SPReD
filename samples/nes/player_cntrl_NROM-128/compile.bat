@set OutFile=player_cntrl_NROM-128
@set OutDir=..\bin\
@set IncDir1=..\common\
@set IncDir2=.\data\

@del %OutDir%%OutFile%.nes

@echo compiling...
ca65 -I %IncDir1% -I %IncDir2% main.asm  -o %OutDir%%OutFile%.o
@if ERRORLEVEL 1 goto failure

@echo linking...
ld65 -o %OutDir%%OutFile%.nes -C NROM-128.cfg %OutDir%%OutFile%.o
@if ERRORLEVEL 1 goto failure
@echo Ok!

@del %OutDir%%OutFile%.o

@cd ..\bin\
%OutFile%.nes
@goto exit

:failure
@echo Build error!
:exit