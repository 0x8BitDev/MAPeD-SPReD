/*
 * Created by SharpDevelop.
 * User: sutr
 * Date: 22.02.2021
 * Time: 18:33
 */
namespace MAPeD
{
	partial class progress_form
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
			this.StatusLabel = new System.Windows.Forms.Label();
			this.ProgressBar = new System.Windows.Forms.ProgressBar();
			this.OperationLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// StatusLabel
			// 
			this.StatusLabel.Location = new System.Drawing.Point(12, 33);
			this.StatusLabel.Name = "StatusLabel";
			this.StatusLabel.Size = new System.Drawing.Size(233, 19);
			this.StatusLabel.TabIndex = 0;
			this.StatusLabel.Text = "...";
			this.StatusLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// ProgressBar
			// 
			this.ProgressBar.Location = new System.Drawing.Point(12, 55);
			this.ProgressBar.Name = "ProgressBar";
			this.ProgressBar.Size = new System.Drawing.Size(233, 19);
			this.ProgressBar.Step = 5;
			this.ProgressBar.TabIndex = 0;
			// 
			// OperationLabel
			// 
			this.OperationLabel.Location = new System.Drawing.Point(12, 9);
			this.OperationLabel.Name = "OperationLabel";
			this.OperationLabel.Size = new System.Drawing.Size(233, 19);
			this.OperationLabel.TabIndex = 0;
			this.OperationLabel.Text = "...";
			this.OperationLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// progress_form
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(257, 85);
			this.ControlBox = false;
			this.Controls.Add(this.ProgressBar);
			this.Controls.Add(this.OperationLabel);
			this.Controls.Add(this.StatusLabel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "progress_form";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Label OperationLabel;
		private System.Windows.Forms.ProgressBar ProgressBar;
		private System.Windows.Forms.Label StatusLabel;
	}
}
