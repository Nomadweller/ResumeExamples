namespace View
{
    partial class GameView
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
            this.lblServer = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.txtBoxServer = new System.Windows.Forms.TextBox();
            this.txtBoxName = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.Location = new System.Drawing.Point(12, 9);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(50, 17);
            this.lblServer.TabIndex = 0;
            this.lblServer.Text = "Server";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(230, 13);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(49, 17);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Name:";
            // 
            // txtBoxServer
            // 
            this.txtBoxServer.Location = new System.Drawing.Point(67, 11);
            this.txtBoxServer.Name = "txtBoxServer";
            this.txtBoxServer.Size = new System.Drawing.Size(156, 22);
            this.txtBoxServer.TabIndex = 2;
            this.txtBoxServer.Text = "localhost";
            // 
            // txtBoxName
            // 
            this.txtBoxName.Location = new System.Drawing.Point(286, 13);
            this.txtBoxName.Name = "txtBoxName";
            this.txtBoxName.Size = new System.Drawing.Size(223, 22);
            this.txtBoxName.TabIndex = 3;
            this.txtBoxName.Text = "player";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(515, 13);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(154, 29);
            this.btnConnect.TabIndex = 4;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // GameView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(832, 803);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.txtBoxName);
            this.Controls.Add(this.txtBoxServer);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblServer);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GameView";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtBoxName;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtBoxServer;
       // private DrawingPanel.DrawingPanel drawingPanelGame;
    }
}

