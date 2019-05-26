/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 20.05.2019
 * Time: 16:19
 */
namespace MAPeD
{
	partial class py_editor
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(py_editor));
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.LineNumberScriptFieldSplitContainer = new System.Windows.Forms.SplitContainer();
			this.LineNumberPixBox = new System.Windows.Forms.PictureBox();
			this.ScriptTextBox = new System.Windows.Forms.RichTextBox();
			this.OutputTextBox = new System.Windows.Forms.RichTextBox();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.toolStripStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripLnCol = new System.Windows.Forms.ToolStripStatusLabel();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.executeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.InBrowserDocToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.InAppDocToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.LineNumberScriptFieldSplitContainer)).BeginInit();
			this.LineNumberScriptFieldSplitContainer.Panel1.SuspendLayout();
			this.LineNumberScriptFieldSplitContainer.Panel2.SuspendLayout();
			this.LineNumberScriptFieldSplitContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.LineNumberPixBox)).BeginInit();
			this.statusStrip.SuspendLayout();
			this.menuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 24);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.LineNumberScriptFieldSplitContainer);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.OutputTextBox);
			this.splitContainer1.Panel2.Controls.Add(this.statusStrip);
			this.splitContainer1.Size = new System.Drawing.Size(500, 560);
			this.splitContainer1.SplitterDistance = 461;
			this.splitContainer1.TabIndex = 0;
			this.splitContainer1.TabStop = false;
			// 
			// LineNumberScriptFieldSplitContainer
			// 
			this.LineNumberScriptFieldSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LineNumberScriptFieldSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.LineNumberScriptFieldSplitContainer.IsSplitterFixed = true;
			this.LineNumberScriptFieldSplitContainer.Location = new System.Drawing.Point(0, 0);
			this.LineNumberScriptFieldSplitContainer.Name = "LineNumberScriptFieldSplitContainer";
			// 
			// LineNumberScriptFieldSplitContainer.Panel1
			// 
			this.LineNumberScriptFieldSplitContainer.Panel1.Controls.Add(this.LineNumberPixBox);
			this.LineNumberScriptFieldSplitContainer.Panel1MinSize = 20;
			// 
			// LineNumberScriptFieldSplitContainer.Panel2
			// 
			this.LineNumberScriptFieldSplitContainer.Panel2.Controls.Add(this.ScriptTextBox);
			this.LineNumberScriptFieldSplitContainer.Panel2MinSize = 150;
			this.LineNumberScriptFieldSplitContainer.Size = new System.Drawing.Size(500, 461);
			this.LineNumberScriptFieldSplitContainer.SplitterDistance = 25;
			this.LineNumberScriptFieldSplitContainer.SplitterWidth = 1;
			this.LineNumberScriptFieldSplitContainer.TabIndex = 3;
			this.LineNumberScriptFieldSplitContainer.TabStop = false;
			// 
			// LineNumberPixBox
			// 
			this.LineNumberPixBox.BackColor = System.Drawing.Color.White;
			this.LineNumberPixBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LineNumberPixBox.Location = new System.Drawing.Point(0, 0);
			this.LineNumberPixBox.Name = "LineNumberPixBox";
			this.LineNumberPixBox.Size = new System.Drawing.Size(25, 461);
			this.LineNumberPixBox.TabIndex = 0;
			this.LineNumberPixBox.TabStop = false;
			// 
			// ScriptTextBox
			// 
			this.ScriptTextBox.AcceptsTab = true;
			this.ScriptTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.ScriptTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ScriptTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.ScriptTextBox.Location = new System.Drawing.Point(0, 0);
			this.ScriptTextBox.MaxLength = 65535;
			this.ScriptTextBox.Name = "ScriptTextBox";
			this.ScriptTextBox.Size = new System.Drawing.Size(474, 461);
			this.ScriptTextBox.TabIndex = 2;
			this.ScriptTextBox.Text = "";
			this.ScriptTextBox.WordWrap = false;
			this.ScriptTextBox.VScroll += new System.EventHandler(this.ScriptTextBoxVScroll);
			this.ScriptTextBox.SizeChanged += new System.EventHandler(this.ScriptTextBoxSizeChanged);
			this.ScriptTextBox.TextChanged += new System.EventHandler(this.ScriptTextBoxTextChanged);
			this.ScriptTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ScriptTextBoxKeyUp);
			this.ScriptTextBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ScriptTextBoxMouseDown);
			this.ScriptTextBox.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.ScriptTextBoxPreviewKeyDown);
			// 
			// OutputTextBox
			// 
			this.OutputTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.OutputTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.OutputTextBox.Location = new System.Drawing.Point(0, 0);
			this.OutputTextBox.Name = "OutputTextBox";
			this.OutputTextBox.ReadOnly = true;
			this.OutputTextBox.Size = new System.Drawing.Size(500, 71);
			this.OutputTextBox.TabIndex = 3;
			this.OutputTextBox.Text = "";
			// 
			// statusStrip
			// 
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.toolStripStatus,
									this.toolStripLnCol});
			this.statusStrip.Location = new System.Drawing.Point(0, 71);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = new System.Drawing.Size(500, 24);
			this.statusStrip.TabIndex = 4;
			this.statusStrip.Text = "statusStrip";
			// 
			// toolStripStatus
			// 
			this.toolStripStatus.Name = "toolStripStatus";
			this.toolStripStatus.Size = new System.Drawing.Size(465, 19);
			this.toolStripStatus.Spring = true;
			this.toolStripStatus.Text = "...";
			this.toolStripStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// toolStripLnCol
			// 
			this.toolStripLnCol.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			this.toolStripLnCol.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripLnCol.Name = "toolStripLnCol";
			this.toolStripLnCol.Size = new System.Drawing.Size(20, 19);
			this.toolStripLnCol.Text = "...";
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.fileToolStripMenuItem,
									this.executeToolStripMenuItem,
									this.helpToolStripMenuItem});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(500, 24);
			this.menuStrip.TabIndex = 0;
			this.menuStrip.Text = "menuStrip";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.newToolStripMenuItem,
									this.openToolStripMenuItem,
									this.reloadToolStripMenuItem,
									this.saveToolStripMenuItem,
									this.saveAsToolStripMenuItem,
									this.toolStripSeparator1,
									this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// newToolStripMenuItem
			// 
			this.newToolStripMenuItem.Name = "newToolStripMenuItem";
			this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
			this.newToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
			this.newToolStripMenuItem.Text = "&New";
			this.newToolStripMenuItem.Click += new System.EventHandler(this.NewToolStripMenuItemClick);
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
			this.openToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
			this.openToolStripMenuItem.Text = "&Load";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.LoadToolStripMenuItemClick);
			// 
			// reloadToolStripMenuItem
			// 
			this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
			this.reloadToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
			this.reloadToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
			this.reloadToolStripMenuItem.Text = "&Reload";
			this.reloadToolStripMenuItem.Click += new System.EventHandler(this.ReloadToolStripMenuItemClick);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
			this.saveToolStripMenuItem.Text = "&Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItemClick);
			// 
			// saveAsToolStripMenuItem
			// 
			this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
			this.saveAsToolStripMenuItem.Text = "Save As...";
			this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAsToolStripMenuItemClick);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(148, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItemClick);
			// 
			// executeToolStripMenuItem
			// 
			this.executeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.runToolStripMenuItem});
			this.executeToolStripMenuItem.Name = "executeToolStripMenuItem";
			this.executeToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
			this.executeToolStripMenuItem.Text = "&Execute";
			// 
			// runToolStripMenuItem
			// 
			this.runToolStripMenuItem.Name = "runToolStripMenuItem";
			this.runToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
			this.runToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
			this.runToolStripMenuItem.Text = "&Run";
			this.runToolStripMenuItem.Click += new System.EventHandler(this.RunToolStripMenuItemClick);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.InBrowserDocToolStripMenuItem,
									this.InAppDocToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.helpToolStripMenuItem.Text = "&Help";
			// 
			// InBrowserDocToolStripMenuItem
			// 
			this.InBrowserDocToolStripMenuItem.Name = "InBrowserDocToolStripMenuItem";
			this.InBrowserDocToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
			this.InBrowserDocToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.InBrowserDocToolStripMenuItem.Text = "In-&Browser Doc";
			this.InBrowserDocToolStripMenuItem.Click += new System.EventHandler(this.InBrowserDocToolStripMenuItemClick);
			// 
			// InAppDocToolStripMenuItem
			// 
			this.InAppDocToolStripMenuItem.Name = "InAppDocToolStripMenuItem";
			this.InAppDocToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F1)));
			this.InAppDocToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.InAppDocToolStripMenuItem.Text = "In-&App Doc";
			this.InAppDocToolStripMenuItem.Click += new System.EventHandler(this.InAppDocToolStripMenuItemClick);
			// 
			// openFileDialog
			// 
			this.openFileDialog.DefaultExt = "py";
			this.openFileDialog.Filter = "Python script ( *.py )|*.py";
			this.openFileDialog.Title = "Load Script";
			this.openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.LoadOk_Event);
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.DefaultExt = "py";
			this.saveFileDialog.Filter = "Python script ( *.py )|*.py";
			this.saveFileDialog.Title = "Save Script";
			this.saveFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.SaveOk_Event);
			// 
			// py_editor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(500, 584);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.menuStrip);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip;
			this.Name = "py_editor";
			this.Text = "SPSeD";
			this.Shown += new System.EventHandler(this.Py_editorShown);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.LineNumberScriptFieldSplitContainer.Panel1.ResumeLayout(false);
			this.LineNumberScriptFieldSplitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.LineNumberScriptFieldSplitContainer)).EndInit();
			this.LineNumberScriptFieldSplitContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.LineNumberPixBox)).EndInit();
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.ToolStripMenuItem InBrowserDocToolStripMenuItem;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatus;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem reloadToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
		private System.Windows.Forms.PictureBox LineNumberPixBox;
		private System.Windows.Forms.SplitContainer LineNumberScriptFieldSplitContainer;
		private System.Windows.Forms.ToolStripStatusLabel toolStripLnCol;
		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.RichTextBox OutputTextBox;
		private System.Windows.Forms.ToolStripMenuItem InAppDocToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem executeToolStripMenuItem;
		private System.Windows.Forms.RichTextBox ScriptTextBox;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.SplitContainer splitContainer1;
	}
}
