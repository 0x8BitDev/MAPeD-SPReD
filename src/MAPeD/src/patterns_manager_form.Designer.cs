/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2020 ( MIT license. See LICENSE.txt )
 * Date: 03.09.2020
 * Time: 16:34
 */
namespace MAPeD
{
	partial class patterns_manager_form
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
			this.components = new System.ComponentModel.Container();
			this.BtnClose = new System.Windows.Forms.Button();
			this.PixBoxPreview = new System.Windows.Forms.PictureBox();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.TreeViewPatterns = new System.Windows.Forms.TreeView();
			this.ContextMenuStripTreeViewGroup = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.addGroupToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.BtnRename = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.BtnGroupDelete = new System.Windows.Forms.Button();
			this.BtnGroupAdd = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.CheckBoxAddPattern = new System.Windows.Forms.CheckBox();
			this.BtnPatternDelete = new System.Windows.Forms.Button();
			this.ContextMenuStripGroupItem = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.addGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.renameToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.ContextMenuStripPatternItem = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.PixBoxPreview)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.ContextMenuStripTreeViewGroup.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.ContextMenuStripGroupItem.SuspendLayout();
			this.ContextMenuStripPatternItem.SuspendLayout();
			this.SuspendLayout();
			// 
			// BtnClose
			// 
			this.BtnClose.Location = new System.Drawing.Point(561, 335);
			this.BtnClose.Name = "BtnClose";
			this.BtnClose.Size = new System.Drawing.Size(75, 23);
			this.BtnClose.TabIndex = 8;
			this.BtnClose.Text = "&Close";
			this.BtnClose.UseVisualStyleBackColor = true;
			this.BtnClose.Click += new System.EventHandler(this.BtnCloseClick_event);
			// 
			// PixBoxPreview
			// 
			this.PixBoxPreview.BackColor = System.Drawing.Color.Black;
			this.PixBoxPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.PixBoxPreview.Location = new System.Drawing.Point(118, 3);
			this.PixBoxPreview.Name = "PixBoxPreview";
			this.PixBoxPreview.Size = new System.Drawing.Size(300, 300);
			this.PixBoxPreview.TabIndex = 1;
			this.PixBoxPreview.TabStop = false;
			// 
			// splitContainer1
			// 
			this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.splitContainer1.IsSplitterFixed = true;
			this.splitContainer1.Location = new System.Drawing.Point(12, 12);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.TreeViewPatterns);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.BtnRename);
			this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
			this.splitContainer1.Panel2.Controls.Add(this.PixBoxPreview);
			this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
			this.splitContainer1.Size = new System.Drawing.Size(624, 308);
			this.splitContainer1.SplitterDistance = 197;
			this.splitContainer1.TabIndex = 2;
			// 
			// TreeViewPatterns
			// 
			this.TreeViewPatterns.BackColor = System.Drawing.Color.LightGray;
			this.TreeViewPatterns.ContextMenuStrip = this.ContextMenuStripTreeViewGroup;
			this.TreeViewPatterns.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TreeViewPatterns.LabelEdit = true;
			this.TreeViewPatterns.Location = new System.Drawing.Point(0, 0);
			this.TreeViewPatterns.Name = "TreeViewPatterns";
			this.TreeViewPatterns.Size = new System.Drawing.Size(195, 306);
			this.TreeViewPatterns.TabIndex = 0;
			this.TreeViewPatterns.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.TreeViewNodeRename_Event);
			this.TreeViewPatterns.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeViewNodeSelect_Event);
			// 
			// ContextMenuStripTreeViewGroup
			// 
			this.ContextMenuStripTreeViewGroup.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.addGroupToolStripMenuItem1});
			this.ContextMenuStripTreeViewGroup.Name = "ContextMenuStripTreeViewGroup";
			this.ContextMenuStripTreeViewGroup.Size = new System.Drawing.Size(133, 26);
			// 
			// addGroupToolStripMenuItem1
			// 
			this.addGroupToolStripMenuItem1.Name = "addGroupToolStripMenuItem1";
			this.addGroupToolStripMenuItem1.Size = new System.Drawing.Size(132, 22);
			this.addGroupToolStripMenuItem1.Text = "Add Group";
			this.addGroupToolStripMenuItem1.Click += new System.EventHandler(this.BtnGroupAddClick_Event);
			// 
			// BtnRename
			// 
			this.BtnRename.Location = new System.Drawing.Point(21, 183);
			this.BtnRename.Name = "BtnRename";
			this.BtnRename.Size = new System.Drawing.Size(75, 23);
			this.BtnRename.TabIndex = 7;
			this.BtnRename.Text = "Rename";
			this.BtnRename.UseVisualStyleBackColor = true;
			this.BtnRename.Click += new System.EventHandler(this.BtnRenameClick_Event);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.BtnGroupDelete);
			this.groupBox1.Controls.Add(this.BtnGroupAdd);
			this.groupBox1.Location = new System.Drawing.Point(4, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(108, 84);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Group";
			// 
			// BtnGroupDelete
			// 
			this.BtnGroupDelete.Location = new System.Drawing.Point(17, 49);
			this.BtnGroupDelete.Name = "BtnGroupDelete";
			this.BtnGroupDelete.Size = new System.Drawing.Size(75, 23);
			this.BtnGroupDelete.TabIndex = 3;
			this.BtnGroupDelete.Text = "Delete";
			this.BtnGroupDelete.UseVisualStyleBackColor = true;
			this.BtnGroupDelete.Click += new System.EventHandler(this.BtnGroupDeleteClick_Event);
			// 
			// BtnGroupAdd
			// 
			this.BtnGroupAdd.Location = new System.Drawing.Point(17, 20);
			this.BtnGroupAdd.Name = "BtnGroupAdd";
			this.BtnGroupAdd.Size = new System.Drawing.Size(75, 23);
			this.BtnGroupAdd.TabIndex = 2;
			this.BtnGroupAdd.Text = "Add";
			this.BtnGroupAdd.UseVisualStyleBackColor = true;
			this.BtnGroupAdd.Click += new System.EventHandler(this.BtnGroupAddClick_Event);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.CheckBoxAddPattern);
			this.groupBox2.Controls.Add(this.BtnPatternDelete);
			this.groupBox2.Location = new System.Drawing.Point(4, 93);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(108, 84);
			this.groupBox2.TabIndex = 4;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Pattern";
			// 
			// CheckBoxAddPattern
			// 
			this.CheckBoxAddPattern.Appearance = System.Windows.Forms.Appearance.Button;
			this.CheckBoxAddPattern.AutoCheck = false;
			this.CheckBoxAddPattern.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.CheckBoxAddPattern.Location = new System.Drawing.Point(17, 20);
			this.CheckBoxAddPattern.Name = "CheckBoxAddPattern";
			this.CheckBoxAddPattern.Size = new System.Drawing.Size(75, 23);
			this.CheckBoxAddPattern.TabIndex = 5;
			this.CheckBoxAddPattern.Text = "Add";
			this.CheckBoxAddPattern.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.CheckBoxAddPattern.UseVisualStyleBackColor = true;
			this.CheckBoxAddPattern.Click += new System.EventHandler(this.CheckBoxAddPatternClick_Event);
			// 
			// BtnPatternDelete
			// 
			this.BtnPatternDelete.Location = new System.Drawing.Point(17, 49);
			this.BtnPatternDelete.Name = "BtnPatternDelete";
			this.BtnPatternDelete.Size = new System.Drawing.Size(75, 23);
			this.BtnPatternDelete.TabIndex = 6;
			this.BtnPatternDelete.Text = "Delete";
			this.BtnPatternDelete.UseVisualStyleBackColor = true;
			this.BtnPatternDelete.Click += new System.EventHandler(this.BtnPatternDeleteClick_Event);
			// 
			// ContextMenuStripGroupItem
			// 
			this.ContextMenuStripGroupItem.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.addGroupToolStripMenuItem,
									this.deleteGroupToolStripMenuItem,
									this.renameToolStripMenuItem1});
			this.ContextMenuStripGroupItem.Name = "ContextMenuStripPatternsGroup";
			this.ContextMenuStripGroupItem.Size = new System.Drawing.Size(144, 70);
			// 
			// addGroupToolStripMenuItem
			// 
			this.addGroupToolStripMenuItem.Name = "addGroupToolStripMenuItem";
			this.addGroupToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
			this.addGroupToolStripMenuItem.Text = "Add Group";
			this.addGroupToolStripMenuItem.Click += new System.EventHandler(this.BtnGroupAddClick_Event);
			// 
			// deleteGroupToolStripMenuItem
			// 
			this.deleteGroupToolStripMenuItem.Name = "deleteGroupToolStripMenuItem";
			this.deleteGroupToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
			this.deleteGroupToolStripMenuItem.Text = "Delete Group";
			this.deleteGroupToolStripMenuItem.Click += new System.EventHandler(this.BtnGroupDeleteClick_Event);
			// 
			// renameToolStripMenuItem1
			// 
			this.renameToolStripMenuItem1.Name = "renameToolStripMenuItem1";
			this.renameToolStripMenuItem1.Size = new System.Drawing.Size(143, 22);
			this.renameToolStripMenuItem1.Text = "Rename";
			this.renameToolStripMenuItem1.Click += new System.EventHandler(this.BtnRenameClick_Event);
			// 
			// ContextMenuStripPatternItem
			// 
			this.ContextMenuStripPatternItem.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.deleteToolStripMenuItem,
									this.renameToolStripMenuItem});
			this.ContextMenuStripPatternItem.Name = "ContextMenuStripPatternItem";
			this.ContextMenuStripPatternItem.Size = new System.Drawing.Size(118, 48);
			// 
			// deleteToolStripMenuItem
			// 
			this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			this.deleteToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
			this.deleteToolStripMenuItem.Text = "Delete";
			this.deleteToolStripMenuItem.Click += new System.EventHandler(this.BtnGroupDeleteClick_Event);
			// 
			// renameToolStripMenuItem
			// 
			this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
			this.renameToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
			this.renameToolStripMenuItem.Text = "Rename";
			this.renameToolStripMenuItem.Click += new System.EventHandler(this.BtnRenameClick_Event);
			// 
			// patterns_manager_form
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.Silver;
			this.ClientSize = new System.Drawing.Size(645, 370);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.BtnClose);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = global::MAPeD.Properties.Resources.MAPeD_icon;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "patterns_manager_form";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Patterns Manager";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Closing_event);
			this.Load += new System.EventHandler(this.BtnCloseClick_event);
			((System.ComponentModel.ISupportInitialize)(this.PixBoxPreview)).EndInit();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ContextMenuStripTreeViewGroup.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ContextMenuStripGroupItem.ResumeLayout(false);
			this.ContextMenuStripPatternItem.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem addGroupToolStripMenuItem1;
		private System.Windows.Forms.ContextMenuStrip ContextMenuStripTreeViewGroup;
		private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip ContextMenuStripPatternItem;
		private System.Windows.Forms.ToolStripMenuItem deleteGroupToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addGroupToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip ContextMenuStripGroupItem;
		private System.Windows.Forms.CheckBox CheckBoxAddPattern;
		private System.Windows.Forms.Button BtnRename;
		private System.Windows.Forms.Button BtnGroupAdd;
		private System.Windows.Forms.Button BtnGroupDelete;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button BtnPatternDelete;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TreeView TreeViewPatterns;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.PictureBox PixBoxPreview;
		private System.Windows.Forms.Button BtnClose;
	}
}
