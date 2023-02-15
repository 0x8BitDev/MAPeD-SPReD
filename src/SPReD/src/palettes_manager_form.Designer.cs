/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 21.07.2022
 * Time: 16:45
 */
namespace SPReD
{
	partial class palettes_manager_form
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
			this.ListBoxPalettes = new System.Windows.Forms.ListBox();
			this.BtnClose = new System.Windows.Forms.Button();
			this.BtnMoveUp = new System.Windows.Forms.Button();
			this.BtnMoveDown = new System.Windows.Forms.Button();
			this.BtnCopy = new System.Windows.Forms.Button();
			this.BtnPaste = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// ListBoxPalettes
			// 
			this.ListBoxPalettes.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.ListBoxPalettes.FormattingEnabled = true;
			this.ListBoxPalettes.Location = new System.Drawing.Point(12, 12);
			this.ListBoxPalettes.Name = "ListBoxPalettes";
			this.ListBoxPalettes.Size = new System.Drawing.Size(234, 212);
			this.ListBoxPalettes.TabIndex = 0;
			// 
			// BtnClose
			// 
			this.BtnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.BtnClose.Location = new System.Drawing.Point(171, 304);
			this.BtnClose.Name = "BtnClose";
			this.BtnClose.Size = new System.Drawing.Size(75, 23);
			this.BtnClose.TabIndex = 5;
			this.BtnClose.Text = "&Close";
			this.BtnClose.UseVisualStyleBackColor = true;
			// 
			// BtnMoveUp
			// 
			this.BtnMoveUp.Location = new System.Drawing.Point(12, 239);
			this.BtnMoveUp.Name = "BtnMoveUp";
			this.BtnMoveUp.Size = new System.Drawing.Size(75, 23);
			this.BtnMoveUp.TabIndex = 1;
			this.BtnMoveUp.Text = "Move &Up";
			this.BtnMoveUp.UseVisualStyleBackColor = true;
			this.BtnMoveUp.Click += new System.EventHandler(this.BtnMoveUpClick);
			// 
			// BtnMoveDown
			// 
			this.BtnMoveDown.Location = new System.Drawing.Point(12, 268);
			this.BtnMoveDown.Name = "BtnMoveDown";
			this.BtnMoveDown.Size = new System.Drawing.Size(75, 23);
			this.BtnMoveDown.TabIndex = 2;
			this.BtnMoveDown.Text = "Move &Down";
			this.BtnMoveDown.UseVisualStyleBackColor = true;
			this.BtnMoveDown.Click += new System.EventHandler(this.BtnMoveDownClick);
			// 
			// BtnCopy
			// 
			this.BtnCopy.Location = new System.Drawing.Point(171, 239);
			this.BtnCopy.Name = "BtnCopy";
			this.BtnCopy.Size = new System.Drawing.Size(75, 23);
			this.BtnCopy.TabIndex = 3;
			this.BtnCopy.Text = "&Copy";
			this.BtnCopy.UseVisualStyleBackColor = true;
			this.BtnCopy.Click += new System.EventHandler(this.BtnCopyClick);
			// 
			// BtnPaste
			// 
			this.BtnPaste.Location = new System.Drawing.Point(171, 268);
			this.BtnPaste.Name = "BtnPaste";
			this.BtnPaste.Size = new System.Drawing.Size(75, 23);
			this.BtnPaste.TabIndex = 4;
			this.BtnPaste.Text = "&Paste";
			this.BtnPaste.UseVisualStyleBackColor = true;
			this.BtnPaste.Click += new System.EventHandler(this.BtnPasteClick);
			// 
			// palettes_manager_form
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(258, 339);
			this.Controls.Add(this.BtnPaste);
			this.Controls.Add(this.BtnCopy);
			this.Controls.Add(this.BtnMoveDown);
			this.Controls.Add(this.BtnMoveUp);
			this.Controls.Add(this.BtnClose);
			this.Controls.Add(this.ListBoxPalettes);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "palettes_manager_form";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Palettes Manager";
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button BtnPaste;
		private System.Windows.Forms.Button BtnCopy;
		private System.Windows.Forms.Button BtnMoveDown;
		private System.Windows.Forms.Button BtnMoveUp;
		private System.Windows.Forms.Button BtnClose;
		private System.Windows.Forms.ListBox ListBoxPalettes;
	}
}
