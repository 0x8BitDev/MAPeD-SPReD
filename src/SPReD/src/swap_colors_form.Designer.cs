﻿/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 24.08.2020
 * Time: 18:09
 */
namespace SPReD
{
	partial class swap_colors_form
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.BtnSwap = new System.Windows.Forms.Button();
			this.BtnClose = new System.Windows.Forms.Button();
			this.PixBoxPalette = new System.Windows.Forms.PictureBox();
			this.CheckBoxColorA = new System.Windows.Forms.CheckBox();
			this.CheckBoxColorB = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.PixBoxPalette)).BeginInit();
			this.SuspendLayout();
			// 
			// BtnSwap
			// 
			this.BtnSwap.Location = new System.Drawing.Point(100, 40);
			this.BtnSwap.Name = "BtnSwap";
			this.BtnSwap.Size = new System.Drawing.Size(81, 24);
			this.BtnSwap.TabIndex = 0;
			this.BtnSwap.Text = "&Swap";
			this.BtnSwap.UseVisualStyleBackColor = true;
			this.BtnSwap.Click += new System.EventHandler(this.BtnSwapClick_Event);
			// 
			// BtnClose
			// 
			this.BtnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.BtnClose.Location = new System.Drawing.Point(187, 78);
			this.BtnClose.Name = "BtnClose";
			this.BtnClose.Size = new System.Drawing.Size(82, 23);
			this.BtnClose.TabIndex = 0;
			this.BtnClose.Text = "&Close";
			this.BtnClose.UseVisualStyleBackColor = true;
			// 
			// PixBoxPalette
			// 
			this.PixBoxPalette.BackColor = System.Drawing.Color.Black;
			this.PixBoxPalette.Location = new System.Drawing.Point(11, 11);
			this.PixBoxPalette.Name = "PixBoxPalette";
			this.PixBoxPalette.Size = new System.Drawing.Size(258, 18);
			this.PixBoxPalette.TabIndex = 1;
			this.PixBoxPalette.TabStop = false;
			// 
			// CheckBoxColorA
			// 
			this.CheckBoxColorA.Appearance = System.Windows.Forms.Appearance.Button;
			this.CheckBoxColorA.BackColor = System.Drawing.SystemColors.Control;
			this.CheckBoxColorA.FlatAppearance.BorderSize = 2;
			this.CheckBoxColorA.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.CheckBoxColorA.ForeColor = System.Drawing.SystemColors.ControlText;
			this.CheckBoxColorA.Location = new System.Drawing.Point(11, 40);
			this.CheckBoxColorA.Name = "CheckBoxColorA";
			this.CheckBoxColorA.Size = new System.Drawing.Size(82, 24);
			this.CheckBoxColorA.TabIndex = 3;
			this.CheckBoxColorA.Text = "Color A";
			this.CheckBoxColorA.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.CheckBoxColorA.UseVisualStyleBackColor = false;
			this.CheckBoxColorA.CheckedChanged += new System.EventHandler(this.CheckBoxColorAChanged_Event);
			// 
			// CheckBoxColorB
			// 
			this.CheckBoxColorB.Appearance = System.Windows.Forms.Appearance.Button;
			this.CheckBoxColorB.BackColor = System.Drawing.SystemColors.Control;
			this.CheckBoxColorB.FlatAppearance.BorderSize = 2;
			this.CheckBoxColorB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.CheckBoxColorB.Location = new System.Drawing.Point(187, 40);
			this.CheckBoxColorB.Name = "CheckBoxColorB";
			this.CheckBoxColorB.Size = new System.Drawing.Size(82, 24);
			this.CheckBoxColorB.TabIndex = 3;
			this.CheckBoxColorB.Text = "Color B";
			this.CheckBoxColorB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.CheckBoxColorB.UseVisualStyleBackColor = false;
			this.CheckBoxColorB.CheckedChanged += new System.EventHandler(this.CheckBoxColorBChanged_Event);
			// 
			// swap_colors_form
			// 
			this.AcceptButton = this.BtnSwap;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.BtnClose;
			this.ClientSize = new System.Drawing.Size(281, 113);
			this.Controls.Add(this.CheckBoxColorB);
			this.Controls.Add(this.CheckBoxColorA);
			this.Controls.Add(this.PixBoxPalette);
			this.Controls.Add(this.BtnClose);
			this.Controls.Add(this.BtnSwap);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "swap_colors_form";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Swap Colors";
			((System.ComponentModel.ISupportInitialize)(this.PixBoxPalette)).EndInit();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.CheckBox CheckBoxColorB;
		private System.Windows.Forms.CheckBox CheckBoxColorA;
		private System.Windows.Forms.PictureBox PixBoxPalette;
		private System.Windows.Forms.Button BtnClose;
		private System.Windows.Forms.Button BtnSwap;
	}
}
