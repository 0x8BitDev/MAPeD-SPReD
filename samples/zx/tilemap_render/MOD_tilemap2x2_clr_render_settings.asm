;###################################################################
;
; Copyright 2018-2019 0x8BitDev ( MIT license )
;
;###################################################################
;
; Multidirectional scroller settings.
;

; move step in bits

MS_TILE		equ	1	; [tile2x2 : 16bit], [tile4x4 : 32bit]
MS_8b		equ	2	; [tile2x2]
MS_4b		equ	3	; [tile2x2] vertical scrolling is "free", horizontal one is simple RLD, so it's quite slow implementation, just to get fast result
				; the best choice: pre shifted tiles or caching of shifted tiles in real-time... the best choice for real project, not for the data test demo )

		; the main settings

		DEFINE	DEF_128K_DBL_BUFFER	1	; 128K dbl buffering or usual 48K rendering
		DEFINE	DEF_FULLSCREEN		1	; fullscreen or upper 2/3 of the screen
		DEFINE	DEF_COLOR		1	; BW or CLR
		DEFINE	DEF_MOVE_STEP		MS_TILE	; MS_TILE / MS_8b / MS_4b
		DEFINE	DEF_VERT_SYNC		1	; rendering begins with HALT or not...
		
		IF	DEF_128K_DBL_BUFFER
		device zxspectrum128
		ELSE
		device zxspectrum48
		ENDIF	//DEF_128K_DBL_BUFFER
