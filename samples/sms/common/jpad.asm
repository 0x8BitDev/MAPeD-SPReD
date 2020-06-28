;###################################################################
;
; Copyright 2019-2020 0x8BitDev ( MIT license )
;
;###################################################################
;
; Joypad macroses
;

; jpad ports

.define	JPAD_REG1	$dc
.define	JPAD_REG2	$dd

; jpad button bits

.define JPAD1_UP	%00000001
.define JPAD1_DOWN	%00000010
.define JPAD1_LEFT	%00000100
.define JPAD1_RIGHT	%00001000
.define JPAD1_SW1	%00010000
.define JPAD1_SW2	%00100000

.define JPAD2_UP	%01000000
.define JPAD2_DOWN	%10000000
.define JPAD2_LEFT	%00000001
.define JPAD2_RIGHT	%00000010
.define JPAD2_SW1	%00000100
.define JPAD2_SW2	%00001000

.define JPAD_RESET	%00010000


.macro	JPAD_LOAD_STATE

	in a, (JPAD_REG1)
	xor $ff
	ld (JPAD_STATE), a

	in a, (JPAD_REG2)
	xor $ff
	ld (JPAD_STATE + 1), a

.endm

; OUT: Z - no flag, otherwise - NZ

.macro	JPAD1_CHECK_LEFT

	ld a, (JPAD_STATE)

	and JPAD1_LEFT

.endm	

.macro	JPAD1_CHECK_RIGHT

	ld a, (JPAD_STATE)

	and JPAD1_RIGHT

.endm	

.macro	JPAD1_CHECK_UP

	ld a, (JPAD_STATE)

	and JPAD1_UP

.endm	

.macro	JPAD1_CHECK_DOWN

	ld a, (JPAD_STATE)

	and JPAD1_DOWN

.endm	

.macro	JPAD1_CHECK_SW1

	ld a, (JPAD_STATE)

	and JPAD1_SW1

.endm	

.macro	JPAD1_CHECK_SW2

	ld a, (JPAD_STATE)

	and JPAD1_SW2

.endm	

.macro	JPAD2_CHECK_LEFT

	ld a, (JPAD_STATE + 1)

	and JPAD2_LEFT

.endm	

.macro	JPAD2_CHECK_RIGHT

	ld a, (JPAD_STATE + 1)

	and JPAD2_RIGHT

.endm	

.macro	JPAD2_CHECK_UP

	ld a, (JPAD_STATE)

	and JPAD2_UP

.endm	

.macro	JPAD2_CHECK_DOWN

	ld a, (JPAD_STATE)

	and JPAD2_DOWN

.endm	

.macro	JPAD2_CHECK_SW1

	ld a, (JPAD_STATE + 1)

	and JPAD2_SW1

.endm	

.macro	JPAD2_CHECK_SW2

	ld a, (JPAD_STATE + 1)

	and JPAD2_SW2

.endm

.macro	JPAD_CHECK_RESET

	ld a, (JPAD_STATE + 1)

	and JPAD_RESET

.endm