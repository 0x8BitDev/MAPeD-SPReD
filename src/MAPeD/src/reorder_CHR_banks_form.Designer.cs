/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 17.05.2022
 * Time: 14:59
 */
namespace MAPeD
{
	partial class reorder_CHR_banks_form
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
			this.BtnClose = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.BtnDown = new System.Windows.Forms.Button();
			this.BtnUp = new System.Windows.Forms.Button();
			this.ListBoxCHRBanks = new System.Windows.Forms.ListBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.PBoxBlocks2x2 = new System.Windows.Forms.PictureBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.LabelPalettes = new System.Windows.Forms.Label();
			this.LabelScreens = new System.Windows.Forms.Label();
			this.LabelTiles4x4 = new System.Windows.Forms.Label();
			this.LabelBlocks2x2 = new System.Windows.Forms.Label();
			this.LabelCHRs = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.PBoxBlocks2x2)).BeginInit();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// BtnClose
			// 
			this.BtnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.BtnClose.Location = new System.Drawing.Point(333, 225);
			this.BtnClose.Name = "BtnClose";
			this.BtnClose.Size = new System.Drawing.Size(75, 23);
			this.BtnClose.TabIndex = 5;
			this.BtnClose.Text = "&Close";
			this.BtnClose.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.BtnDown);
			this.groupBox1.Controls.Add(this.BtnUp);
			this.groupBox1.Controls.Add(this.ListBoxCHRBanks);
			this.groupBox1.Location = new System.Drawing.Point(8, 6);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(95, 241);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "CHR Banks:";
			// 
			// BtnDown
			// 
			this.BtnDown.Location = new System.Drawing.Point(10, 209);
			this.BtnDown.Name = "BtnDown";
			this.BtnDown.Size = new System.Drawing.Size(75, 23);
			this.BtnDown.TabIndex = 3;
			this.BtnDown.Text = "&Down";
			this.BtnDown.UseVisualStyleBackColor = true;
			this.BtnDown.Click += new System.EventHandler(this.BtnDownClick);
			// 
			// BtnUp
			// 
			this.BtnUp.Location = new System.Drawing.Point(10, 183);
			this.BtnUp.Name = "BtnUp";
			this.BtnUp.Size = new System.Drawing.Size(75, 23);
			this.BtnUp.TabIndex = 2;
			this.BtnUp.Text = "&Up";
			this.BtnUp.UseVisualStyleBackColor = true;
			this.BtnUp.Click += new System.EventHandler(this.BtnUpClick);
			// 
			// ListBoxCHRBanks
			// 
			this.ListBoxCHRBanks.FormattingEnabled = true;
			this.ListBoxCHRBanks.Location = new System.Drawing.Point(10, 17);
			this.ListBoxCHRBanks.Name = "ListBoxCHRBanks";
			this.ListBoxCHRBanks.Size = new System.Drawing.Size(75, 160);
			this.ListBoxCHRBanks.TabIndex = 1;
			this.ListBoxCHRBanks.SelectedIndexChanged += new System.EventHandler(this.ListBoxCHRBanksChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.PBoxBlocks2x2);
			this.groupBox2.Location = new System.Drawing.Point(109, 6);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(180, 187);
			this.groupBox2.TabIndex = 4;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Blocks 2x2:";
			// 
			// PBoxBlocks2x2
			// 
			this.PBoxBlocks2x2.BackColor = System.Drawing.Color.Black;
			this.PBoxBlocks2x2.Location = new System.Drawing.Point(10, 17);
			this.PBoxBlocks2x2.Name = "PBoxBlocks2x2";
			this.PBoxBlocks2x2.Size = new System.Drawing.Size(160, 160);
			this.PBoxBlocks2x2.TabIndex = 0;
			this.PBoxBlocks2x2.TabStop = false;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.LabelPalettes);
			this.groupBox3.Controls.Add(this.LabelScreens);
			this.groupBox3.Controls.Add(this.LabelTiles4x4);
			this.groupBox3.Controls.Add(this.LabelBlocks2x2);
			this.groupBox3.Controls.Add(this.LabelCHRs);
			this.groupBox3.Controls.Add(this.label5);
			this.groupBox3.Controls.Add(this.label4);
			this.groupBox3.Controls.Add(this.label3);
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.Controls.Add(this.label1);
			this.groupBox3.Location = new System.Drawing.Point(295, 6);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(115, 187);
			this.groupBox3.TabIndex = 6;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Info:";
			// 
			// LabelPalettes
			// 
			this.LabelPalettes.Location = new System.Drawing.Point(72, 89);
			this.LabelPalettes.Name = "LabelPalettes";
			this.LabelPalettes.Size = new System.Drawing.Size(37, 18);
			this.LabelPalettes.TabIndex = 0;
			this.LabelPalettes.Text = "...";
			// 
			// LabelScreens
			// 
			this.LabelScreens.Location = new System.Drawing.Point(72, 71);
			this.LabelScreens.Name = "LabelScreens";
			this.LabelScreens.Size = new System.Drawing.Size(37, 18);
			this.LabelScreens.TabIndex = 0;
			this.LabelScreens.Text = "...";
			// 
			// LabelTiles4x4
			// 
			this.LabelTiles4x4.Location = new System.Drawing.Point(72, 53);
			this.LabelTiles4x4.Name = "LabelTiles4x4";
			this.LabelTiles4x4.Size = new System.Drawing.Size(37, 18);
			this.LabelTiles4x4.TabIndex = 0;
			this.LabelTiles4x4.Text = "...";
			// 
			// LabelBlocks2x2
			// 
			this.LabelBlocks2x2.Location = new System.Drawing.Point(72, 35);
			this.LabelBlocks2x2.Name = "LabelBlocks2x2";
			this.LabelBlocks2x2.Size = new System.Drawing.Size(37, 18);
			this.LabelBlocks2x2.TabIndex = 0;
			this.LabelBlocks2x2.Text = "...";
			// 
			// LabelCHRs
			// 
			this.LabelCHRs.Location = new System.Drawing.Point(72, 17);
			this.LabelCHRs.Name = "LabelCHRs";
			this.LabelCHRs.Size = new System.Drawing.Size(37, 18);
			this.LabelCHRs.TabIndex = 0;
			this.LabelCHRs.Text = "...";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(9, 89);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(67, 18);
			this.label5.TabIndex = 0;
			this.label5.Text = "Palettes:";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(9, 71);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(67, 18);
			this.label4.TabIndex = 0;
			this.label4.Text = "Screens:";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(9, 53);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(67, 18);
			this.label3.TabIndex = 0;
			this.label3.Text = "Tiles 4x4:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(9, 35);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(67, 18);
			this.label2.TabIndex = 0;
			this.label2.Text = "Blocks 2x2:";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(9, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(67, 18);
			this.label1.TabIndex = 0;
			this.label1.Text = "CHRs:";
			// 
			// reorder_CHR_banks_form
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(420, 260);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.BtnClose);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = global::MAPeD.Properties.Resources.MAPeD_icon;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "reorder_CHR_banks_form";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Reorder CHR Banks";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.PBoxBlocks2x2)).EndInit();
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label LabelCHRs;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label LabelBlocks2x2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label LabelTiles4x4;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label LabelScreens;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label LabelPalettes;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Button BtnUp;
		private System.Windows.Forms.Button BtnDown;
		private System.Windows.Forms.ListBox ListBoxCHRBanks;
		private System.Windows.Forms.PictureBox PBoxBlocks2x2;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button BtnClose;
	}
}
