/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 24.04.2017
 * Time: 12:57
 */
namespace SPReD
{
	partial class create_sprite_form
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
			this.EditNewName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.BtnCancel = new System.Windows.Forms.Button();
			this.BtnOk = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.numericNewSpriteWidth = new System.Windows.Forms.NumericUpDown();
			this.numericNewSpriteHeight = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.numericNewSpriteWidth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericNewSpriteHeight)).BeginInit();
			this.SuspendLayout();
			// 
			// EditNewName
			// 
			this.EditNewName.Location = new System.Drawing.Point(58, 15);
			this.EditNewName.MaxLength = 64;
			this.EditNewName.Name = "EditNewName";
			this.EditNewName.Size = new System.Drawing.Size(175, 20);
			this.EditNewName.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(13, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 17);
			this.label1.TabIndex = 1;
			this.label1.Text = "Name:";
			// 
			// BtnCancel
			// 
			this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.BtnCancel.Location = new System.Drawing.Point(160, 80);
			this.BtnCancel.Name = "BtnCancel";
			this.BtnCancel.Size = new System.Drawing.Size(75, 23);
			this.BtnCancel.TabIndex = 4;
			this.BtnCancel.Text = "Cancel";
			this.BtnCancel.UseVisualStyleBackColor = true;
			// 
			// BtnOk
			// 
			this.BtnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.BtnOk.Location = new System.Drawing.Point(79, 80);
			this.BtnOk.Name = "BtnOk";
			this.BtnOk.Size = new System.Drawing.Size(75, 23);
			this.BtnOk.TabIndex = 3;
			this.BtnOk.Text = "Ok";
			this.BtnOk.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(13, 46);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(40, 17);
			this.label2.TabIndex = 1;
			this.label2.Text = "Width:";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(130, 46);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(42, 17);
			this.label3.TabIndex = 1;
			this.label3.Text = "Height:";
			// 
			// numericNewSpriteWidth
			// 
			this.numericNewSpriteWidth.Location = new System.Drawing.Point(58, 44);
			this.numericNewSpriteWidth.Maximum = new decimal(new int[] {
									16,
									0,
									0,
									0});
			this.numericNewSpriteWidth.Minimum = new decimal(new int[] {
									1,
									0,
									0,
									0});
			this.numericNewSpriteWidth.Name = "numericNewSpriteWidth";
			this.numericNewSpriteWidth.Size = new System.Drawing.Size(60, 20);
			this.numericNewSpriteWidth.TabIndex = 1;
			this.numericNewSpriteWidth.Value = new decimal(new int[] {
									1,
									0,
									0,
									0});
			// 
			// numericNewSpriteHeight
			// 
			this.numericNewSpriteHeight.Location = new System.Drawing.Point(173, 44);
			this.numericNewSpriteHeight.Maximum = new decimal(new int[] {
									16,
									0,
									0,
									0});
			this.numericNewSpriteHeight.Minimum = new decimal(new int[] {
									1,
									0,
									0,
									0});
			this.numericNewSpriteHeight.Name = "numericNewSpriteHeight";
			this.numericNewSpriteHeight.Size = new System.Drawing.Size(60, 20);
			this.numericNewSpriteHeight.TabIndex = 2;
			this.numericNewSpriteHeight.Value = new decimal(new int[] {
									1,
									0,
									0,
									0});
			// 
			// create_sprite_form
			// 
			this.AcceptButton = this.BtnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.BtnCancel;
			this.ClientSize = new System.Drawing.Size(245, 113);
			this.Controls.Add(this.numericNewSpriteHeight);
			this.Controls.Add(this.numericNewSpriteWidth);
			this.Controls.Add(this.BtnOk);
			this.Controls.Add(this.BtnCancel);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.EditNewName);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "create_sprite_form";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Create Sprite";
			((System.ComponentModel.ISupportInitialize)(this.numericNewSpriteWidth)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericNewSpriteHeight)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.NumericUpDown numericNewSpriteHeight;
		private System.Windows.Forms.NumericUpDown numericNewSpriteWidth;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button BtnOk;
		private System.Windows.Forms.Button BtnCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox EditNewName;
	}
}
