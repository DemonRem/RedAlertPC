namespace Oref1
{
    partial class NotificationWindow
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
            this.components = new System.ComponentModel.Container();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this._closeTimer = new System.Windows.Forms.Timer(this.components);
            this._opacityTimer = new System.Windows.Forms.Timer(this.components);
            this.specialButton1 = new WindowsFormsApplication1.SpecialButton();
            this.label2 = new System.Windows.Forms.Label();
            this.specialButton1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label3.Location = new System.Drawing.Point(12, 33);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this.label3.MaximumSize = new System.Drawing.Size(400, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 18);
            this.label3.TabIndex = 2;
            this.label3.Text = "גוף";
            this.label3.MouseEnter += new System.EventHandler(this.label3_MouseEnter);
            this.label3.MouseLeave += new System.EventHandler(this.label3_MouseLeave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 0, 33, 0);
            this.label1.MaximumSize = new System.Drawing.Size(400, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 24);
            this.label1.TabIndex = 3;
            this.label1.Text = "כותרת";
            this.label1.MouseEnter += new System.EventHandler(this.label1_MouseEnter);
            this.label1.MouseLeave += new System.EventHandler(this.label1_MouseLeave);
            // 
            // _closeTimer
            // 
            this._closeTimer.Tick += new System.EventHandler(this._closeTimer_Tick);
            // 
            // _opacityTimer
            // 
            this._opacityTimer.Interval = 1;
            this._opacityTimer.Tick += new System.EventHandler(this._opacityTimer_Tick);
            // 
            // specialButton1
            // 
            this.specialButton1.AutoSize = true;
            this.specialButton1.BackColor = System.Drawing.Color.Transparent;
            this.specialButton1.Controls.Add(this.label2);
            this.specialButton1.DialogResult = System.Windows.Forms.DialogResult.None;
            this.specialButton1.Location = new System.Drawing.Point(0, 0);
            this.specialButton1.Name = "specialButton1";
            this.specialButton1.Size = new System.Drawing.Size(20, 20);
            this.specialButton1.TabIndex = 4;
            this.specialButton1.Click += new System.EventHandler(this.specialButton1_Click);
            this.specialButton1.MouseEnter += new System.EventHandler(this.specialButton1_MouseEnter);
            this.specialButton1.MouseLeave += new System.EventHandler(this.specialButton1_MouseLeave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.specialButton1.SetForwardClickEvents(this.label2, false);
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "X";
            // 
            // NotificationWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(323, 61);
            this.Controls.Add(this.specialButton1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "NotificationWindow";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "NotificationWindow";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.NotificationWindow_Load);
            this.Shown += new System.EventHandler(this.NotificationWindow_Shown);
            this.MouseEnter += new System.EventHandler(this.NotificationWindow_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.NotificationWindow_MouseLeave);
            this.specialButton1.ResumeLayout(false);
            this.specialButton1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer _closeTimer;
        private System.Windows.Forms.Timer _opacityTimer;
        private WindowsFormsApplication1.SpecialButton specialButton1;
        private System.Windows.Forms.Label label2;

    }
}