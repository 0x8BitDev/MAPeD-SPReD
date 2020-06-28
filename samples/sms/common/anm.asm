;###################################################################
;
; Copyright 2019-2020 0x8BitDev ( MIT license )
;
;###################################################################
;
; Animation functions
; 

; *** animation data init ***
; IN: 	HL - runtime_anm_data
;	DE - static_anm_data
;	BC - C-X / B-Y
;	A  - 1-skip X/Y; 0-save X/Y
; [a/b/h/l/d/e]

anm_init:

	and $01
	jr nz, _skip_XY

	ld (hl), c
	inc hl
	ld (hl), b
	inc hl

	jr _cont

_skip_XY:

	inc hl		; skip X
	inc hl		; skip Y

_cont:

	xor a
	ld (hl), a	; curr tick
	inc hl
	ld (hl), a	; curr frame
	inc hl

	ld b, _sizeof_static_anm_data

_copy_loop:

	ld a, (de)
	inc de

	ld (hl), a
	inc hl

	djnz _copy_loop

	ret

; *** animation update ***
; IN:	HL - runtime_anm_data
; [a/c/d/e/h/l]

anm_update:

	; skip X/Y

	inc hl
	inc hl

	inc (hl)	; ticks counter increment
	ld c, (hl)	; c - current tick

	push hl

	ld d, 0
	ld e, runtime_anm_data.num_ticks - runtime_anm_data.curr_tick	; get offset to .num_ticks data
	add hl, de

	ld a, (hl)	; a - number of ticks

	pop hl

	cp c
	ret nz		; exit if a - c != 0

	; next frame

	xor a
	ld (hl), a	; reset current tick

	inc hl
	inc (hl)
	ld c, (hl)	; c - current frame

	push hl

	ld e, runtime_anm_data.num_frames - runtime_anm_data.curr_frame
	add hl, de

	; check frame

	ld a, (hl)	; a - total frames
	cp c

	ex de, hl	; de - runtime_anm_data.num_frames
	pop hl		; hl - runtime_anm_data.curr_frame

	ret nz		; exit if a - c != 0

	; loop frame

	inc de		; de - loop frame
	ld a, (de)
	ld (hl), a

	ret

; *** get an animation frame gfx data ***
; IN:	HL - runtime anm data
;	BC - array of tile addresses
; OUT:	HL - sprite data address
;	B - sprite data size
;	DE - tiles data address

anm_get_frame_gfx_data:

	push bc

	; skip X/Y

	inc hl
	inc hl

	inc hl
	ld b, (hl)	; b - frame index

	ld de, runtime_anm_data.frames_ptr - runtime_anm_data.curr_frame
	add hl, de	; hl - frame data address

	; get HL data address

	ld e, (hl)
	inc hl
	ld d, (hl)	; de - frame data

	; get frame data offset depending on frame index

	ld a, b

	; A *= 4 ( size of frame_data )

	sla a
	sla a

	ld l, a
	ld h, 0

	add hl, de	; hl - frame data

	ld e, (hl)
	inc hl
	ld d, (hl)	; de - sprite data address

	inc hl
	ld b, (hl)	; b - sprite data size

	inc hl
	ld a, (hl)	; a - index of tiles data

	ex de, hl	; hl - sprite data address
	exx

	pop hl		; hl - array of tile addresses

	sla a		; x2 - data size
	sla a		; x2 - tiles addr

	ld e, a
	ld d, 0
	add hl, de	; hl' - tiles data ( size and tiles addr ) address

	push hl

	exx

	pop de		; de - tiles data ( size and tiles addr ) address

	ret