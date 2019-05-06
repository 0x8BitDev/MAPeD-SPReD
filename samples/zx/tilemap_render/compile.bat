@set File=tilemap_viewer
@set OutDir=..\bin\

@del %OutDir%%File%.sna

@echo compiling...
sjasmplus %File%.asm 
@if ERRORLEVEL 1 goto failure

@cd ..\bin\
%File%.sna
@goto exit

:failure
@echo Build error!
:exit