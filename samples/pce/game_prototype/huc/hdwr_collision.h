//##################################################################
//
// Copyright 2021-2022 0x8BitDev ( MIT license. See LICENSE.txt )
//
// This file is a part of the game prototype demo.
//
//##################################################################

u8	__spr_collision = 0;

void	__fastcall wait_vsync();


#asm

; This is a usual HuC/MagicKit IRQ1 routine, but with a little improvement for the hardware collision detection

__user_irq1:
		pha			; save registers
		phx
		phy
		; --
		lda	video_reg	; get VDC status register
		sta	<vdc_sr		; save a copy
;~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		; check for sprite #0 collision event

		bbr0	<vdc_sr, .vsync

		tax

		lda	#$01
		sta 	___spr_collision

		txa
;~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		 ; ----
		 ; vsync interrupt
		 ;
.vsync:		bbr5	<vdc_sr,.hsync
		; --
		inc	irq_cnt		; increment IRQ counter
		; --
		st0	#5		; update display control (bg/sp)
		lda	<vdc_crl
		sta	video_data_l
		; --
		bbs5	<irq_m,.hsync
		; --
		jsr	vsync_hndl
		; --
		; ----
		; hsync interrupt
		;

.hsync:		bbr2	<vdc_sr,.exit
		bbs7	<irq_m,.exit
		; --
		jsr	hsync_hndl

		; ----
		; exit interrupt
		;
.exit:		lda	<vdc_reg	; restore VDC register index
		sta	video_reg
		; --
		ply
		plx
		pla
		rti

; HuC/MagicKit ignores vsync for user defined IRQ1. So I just re-use it for my re-used IRQ1

;// void wait_vsync()
;
		.proc _wait_vsync

		lda #$01		; num frames to skip

		sei			; disable interrupts
		cmp	irq_cnt		; calculate how many frames to wait
		beq	.l2
		bhs	.l3
		lda	irq_cnt
.l2:		inc	A
.l3:		sub	irq_cnt
		sta	vsync_cnt
		cli			; re-enable interrupts

.l4:		lda	irq_cnt		; wait loop
.l5:		incw  rndseed2
		cmp	irq_cnt
		beq	.l5
		dec	vsync_cnt
		bne	.l4

		stz	irq_cnt		; reset system interrupt counter
		inc	A		; return number of elapsed frames

		; ----
		; callback support

		pha
		lda	joycallback	; callback valid?
		bpl	.t3
		bit	#$01
		bne	.t3

		lda	joycallback+1	; get events for all the
		beq	.t3		; selected joypads
		sta	<__al
		cly
		cla
.t1:		lsr	<__al
		bcc	.t2
		ora	joybuf,Y
.t2:		iny
		cpy	#5
		blo	.t1

		and	joycallback+2	; compare with requested state
		beq	.t3

		inc	joycallback	; lock callback feature
		tax			; call user routine
		tma	#5
		pha
		lda	joycallback+3
		tam	#5
		cla
		jsr	.callback
		pla
		tam	#5
		dec	joycallback	; unlock
		; --
.t3:		plx
		cla
		rts

		; ----
		; user routine callback
		;
.callback:
		jmp	[joycallback+4]

		.endp

#endasm


void	hdwr_collisions_init()
{
#asm
	; enable collision interrupts

	lda	<vdc_crl
	ora	#$01
	sta	<vdc_crl

	; enable user irq1

	smb1	<irq_m
	stw	#__user_irq1, irq1_jmp
#endasm
}
