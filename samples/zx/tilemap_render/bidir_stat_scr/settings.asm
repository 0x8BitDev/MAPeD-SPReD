;###################################################################
;
; Copyright 2018-2021 0x8BitDev ( MIT license )
;
;###################################################################
;
; Bidirectional scroller settings.
;

		; the main settings

		DEFINE	DEF_SCR_SCROLL		0	; screen scrolling or switching
		DEFINE	DEF_VERT_SYNC		1	; rendering begins with HALT or not...
		
		IF	DEF_SCR_SCROLL
		DEFINE	DEF_128K_DBL_BUFFER	1
		device zxspectrum128
		ELSE
		DEFINE	DEF_128K_DBL_BUFFER	0
		device zxspectrum48
		ENDIF	//DEF_SCR_SCROLL
