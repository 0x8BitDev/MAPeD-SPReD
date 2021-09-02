cls
rmdir out /s /q
copy ..\..\..\common\mpd_common.h .\inc\mpd_common.h
copy ..\..\..\common\mpd_screen.h .\inc\mpd_screen.h
%SGDK_PATH%\bin\make -f %SGDK_PATH%\makefile.gen