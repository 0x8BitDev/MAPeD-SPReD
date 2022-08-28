; This is the HuC/MagicKit IRQ1 routine, but with a little improvement for the hardware collision detection

		.bank	LIB2_BANK

__lib2_irq1_hdwr_collisions:

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

		rts