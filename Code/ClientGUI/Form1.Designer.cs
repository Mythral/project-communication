namespace ClientGUI
{
	partial class Client
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.logBox = new System.Windows.Forms.TextBox();
			this.chatBox = new System.Windows.Forms.TextBox();
			this.Send = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// logBox
			// 
			this.logBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.logBox.Enabled = false;
			this.logBox.Location = new System.Drawing.Point(12, 12);
			this.logBox.Multiline = true;
			this.logBox.Name = "logBox";
			this.logBox.ReadOnly = true;
			this.logBox.Size = new System.Drawing.Size(540, 186);
			this.logBox.TabIndex = 0;
			// 
			// chatBox
			// 
			this.chatBox.Location = new System.Drawing.Point(13, 218);
			this.chatBox.Name = "chatBox";
			this.chatBox.Size = new System.Drawing.Size(411, 20);
			this.chatBox.TabIndex = 1;
			// 
			// Send
			// 
			this.Send.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.Send.Location = new System.Drawing.Point(430, 205);
			this.Send.Name = "Send";
			this.Send.Size = new System.Drawing.Size(122, 44);
			this.Send.TabIndex = 2;
			this.Send.Text = "Send";
			this.Send.UseVisualStyleBackColor = true;
			this.Send.Click += new System.EventHandler(this.Send_Click);
			// 
			// Client
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(564, 261);
			this.Controls.Add(this.Send);
			this.Controls.Add(this.chatBox);
			this.Controls.Add(this.logBox);
			this.Name = "Client";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Client";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox logBox;
		private System.Windows.Forms.TextBox chatBox;
		private System.Windows.Forms.Button Send;
	}
}

