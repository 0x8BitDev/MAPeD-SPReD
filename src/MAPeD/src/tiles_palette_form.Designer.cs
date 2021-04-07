/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2020 ( MIT license. See LICENSE.txt )
 * Date: 26.11.2018
 * Time: 17:38
 */
namespace MAPeD
{
	partial class tiles_palette_form
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
			this.PanelPaletteTiles = new System.Windows.Forms.FlowLayoutPanel();
			this.PanelPaletteBlocks = new System.Windows.Forms.FlowLayoutPanel();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.panel1 = new System.Windows.Forms.Panel();
			this.BtnBlocks = new System.Windows.Forms.Button();
			this.BtnTiles = new System.Windows.Forms.Button();
			this.BtnClose = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// PanelPaletteTiles
			// 
			this.PanelPaletteTiles.AutoScroll = true;
			this.PanelPaletteTiles.BackColor = System.Drawing.SystemColors.Info;
			this.PanelPaletteTiles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.PanelPaletteTiles.Cursor = System.Windows.Forms.Cursors.No;
			this.PanelPaletteTiles.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PanelPaletteTiles.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.PanelPaletteTiles.Location = new System.Drawing.Point(0, 0);
			this.PanelPaletteTiles.Name = "PanelPaletteTiles";
			this.PanelPaletteTiles.Size = new System.Drawing.Size(554, 512);
			this.PanelPaletteTiles.TabIndex = 3;
			// 
			// PanelPaletteBlocks
			// 
			this.PanelPaletteBlocks.AutoScroll = true;
			this.PanelPaletteBlocks.BackColor = System.Drawing.SystemColors.Info;
			this.PanelPaletteBlocks.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.PanelPaletteBlocks.Cursor = System.Windows.Forms.Cursors.No;
			this.PanelPaletteBlocks.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PanelPaletteBlocks.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.PanelPaletteBlocks.Location = new System.Drawing.Point(0, 0);
			this.PanelPaletteBlocks.Name = "PanelPaletteBlocks";
			this.PanelPaletteBlocks.Size = new System.Drawing.Size(554, 512);
			this.PanelPaletteBlocks.TabIndex = 3;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.IsSplitterFixed = true;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.PanelPaletteTiles);
			this.splitContainer1.Panel1.Controls.Add(this.PanelPaletteBlocks);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.panel1);
			this.splitContainer1.Size = new System.Drawing.Size(554, 546);
			this.splitContainer1.SplitterDistance = 512;
			this.splitContainer1.TabIndex = 5;
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.Silver;
			this.panel1.Controls.Add(this.BtnBlocks);
			this.panel1.Controls.Add(this.BtnTiles);
			this.panel1.Controls.Add(this.BtnClose);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(554, 30);
			this.panel1.TabIndex = 1;
			// 
			// BtnBlocks
			// 
			this.BtnBlocks.Location = new System.Drawing.Point(86, 3);
			this.BtnBlocks.Name = "BtnBlocks";
			this.BtnBlocks.Size = new System.Drawing.Size(75, 23);
			this.BtnBlocks.TabIndex = 1;
			this.BtnBlocks.Text = "&Blocks";
			this.BtnBlocks.UseVisualStyleBackColor = true;
			this.BtnBlocks.Click += new System.EventHandler(this.BtnBlocksClick_Event);
			// 
			// BtnTiles
			// 
			this.BtnTiles.Location = new System.Drawing.Point(5, 3);
			this.BtnTiles.Name = "BtnTiles";
			this.BtnTiles.Size = new System.Drawing.Size(75, 23);
			this.BtnTiles.TabIndex = 0;
			this.BtnTiles.Text = "&Tiles";
			this.BtnTiles.UseVisualStyleBackColor = true;
			this.BtnTiles.Click += new System.EventHandler(this.BtnTilesClick_Event);
			// 
			// BtnClose
			// 
			this.BtnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.BtnClose.Location = new System.Drawing.Point(474, 3);
			this.BtnClose.Name = "BtnClose";
			this.BtnClose.Size = new System.Drawing.Size(75, 23);
			this.BtnClose.TabIndex = 2;
			this.BtnClose.Text = "&Close";
			this.BtnClose.UseVisualStyleBackColor = true;
			this.BtnClose.Click += new System.EventHandler(this.BtnCloseClick_Event);
			// 
			// tiles_palette_form
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(554, 546);
			this.Controls.Add(this.splitContainer1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = global::MAPeD.Properties.Resources.MAPeD_icon;
			this.IsMdiContainer = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "tiles_palette_form";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Palette";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Closing_Event);
			this.Load += new System.EventHandler(this.BtnCloseClick_Event);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.FlowLayoutPanel PanelPaletteBlocks;
		private System.Windows.Forms.Button BtnBlocks;
		private System.Windows.Forms.Button BtnTiles;
		private System.Windows.Forms.Button BtnClose;
		private System.Windows.Forms.FlowLayoutPanel PanelPaletteTiles;
		private System.Windows.Forms.Panel panel1;
	}
}
