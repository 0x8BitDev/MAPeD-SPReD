/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 29.05.2019
 * Time: 18:27
 */
namespace SPSeD
{
	partial class py_editor_doc_page
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the control.
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
			this.LineNumberPixBox = new System.Windows.Forms.PictureBox();
			this.LineNumberScriptFieldSplitContainer = new System.Windows.Forms.SplitContainer();
			this.ScriptTextBox = new System.Windows.Forms.RichTextBox();
			((System.ComponentModel.ISupportInitialize)(this.LineNumberPixBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.LineNumberScriptFieldSplitContainer)).BeginInit();
			this.LineNumberScriptFieldSplitContainer.Panel1.SuspendLayout();
			this.LineNumberScriptFieldSplitContainer.Panel2.SuspendLayout();
			this.LineNumberScriptFieldSplitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// LineNumberPixBox
			// 
			this.LineNumberPixBox.BackColor = System.Drawing.Color.White;
			this.LineNumberPixBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LineNumberPixBox.Location = new System.Drawing.Point(0, 0);
			this.LineNumberPixBox.Name = "LineNumberPixBox";
			this.LineNumberPixBox.Size = new System.Drawing.Size(25, 419);
			this.LineNumberPixBox.TabIndex = 0;
			this.LineNumberPixBox.TabStop = false;
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
			this.LineNumberScriptFieldSplitContainer.Size = new System.Drawing.Size(497, 419);
			this.LineNumberScriptFieldSplitContainer.SplitterDistance = 25;
			this.LineNumberScriptFieldSplitContainer.SplitterWidth = 1;
			this.LineNumberScriptFieldSplitContainer.TabIndex = 4;
			this.LineNumberScriptFieldSplitContainer.TabStop = false;
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
			this.ScriptTextBox.Size = new System.Drawing.Size(471, 419);
			this.ScriptTextBox.TabIndex = 2;
			this.ScriptTextBox.Text = "";
			this.ScriptTextBox.WordWrap = false;
			this.ScriptTextBox.ContentsResized += new System.Windows.Forms.ContentsResizedEventHandler(this.ScriptTextBoxContentsResized);
			this.ScriptTextBox.SelectionChanged += new System.EventHandler(this.ScriptTextBoxSelectionChanged);
			this.ScriptTextBox.VScroll += new System.EventHandler(this.ScriptTextBoxVScroll);
			this.ScriptTextBox.SizeChanged += new System.EventHandler(this.ScriptTextBoxSizeChanged);
			this.ScriptTextBox.TextChanged += new System.EventHandler(this.ScriptTextBoxTextChanged);
			this.ScriptTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ScriptTextBoxKeyUp);
			this.ScriptTextBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ScriptTextBoxMouseDown);
			this.ScriptTextBox.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.ScriptTextBoxPreviewKeyDown);
			// 
			// py_editor_doc_page
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.LineNumberScriptFieldSplitContainer);
			this.Name = "py_editor_doc_page";
			this.Size = new System.Drawing.Size(497, 419);
			((System.ComponentModel.ISupportInitialize)(this.LineNumberPixBox)).EndInit();
			this.LineNumberScriptFieldSplitContainer.Panel1.ResumeLayout(false);
			this.LineNumberScriptFieldSplitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.LineNumberScriptFieldSplitContainer)).EndInit();
			this.LineNumberScriptFieldSplitContainer.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.RichTextBox ScriptTextBox;
		private System.Windows.Forms.SplitContainer LineNumberScriptFieldSplitContainer;
		private System.Windows.Forms.PictureBox LineNumberPixBox;
	}
}
