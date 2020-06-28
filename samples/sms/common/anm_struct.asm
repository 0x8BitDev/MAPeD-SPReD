;###################################################################
;
; Copyright 2019-2020 0x8BitDev ( MIT license )
;
;###################################################################
;
; Animation data structures
;

.struct runtime_anm_data

	x		byte	; 0
	y		byte	; 1

	curr_tick	byte	; 2
	curr_frame	byte	; 3
	num_ticks	byte	; 4 num ticks to change a frame
	num_frames	byte	; 5 num frames to loop an animation
	loop_frame	byte	; 6
	frames_ptr	word	; 7

.endst

.struct static_anm_data

	num_ticks	byte
	num_frames	byte
	loop_frame	byte
	frames_ptr	word

.endst

.struct frame_data

	gfx_ptr		word
	data_size	byte
	chr_id		byte

.endst