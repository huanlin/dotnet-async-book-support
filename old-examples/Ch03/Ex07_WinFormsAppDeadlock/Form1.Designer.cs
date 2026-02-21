namespace Ex06_WinFormsAppDeadlock
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSolution1 = new System.Windows.Forms.Button();
            this.btnSolution2 = new System.Windows.Forms.Button();
            this.btnSolution3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(36, 28);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(144, 40);
            this.button1.TabIndex = 0;
            this.button1.Text = "按我會當掉!";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 96);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // btnSolution1
            // 
            this.btnSolution1.Location = new System.Drawing.Point(204, 28);
            this.btnSolution1.Name = "btnSolution1";
            this.btnSolution1.Size = new System.Drawing.Size(112, 40);
            this.btnSolution1.TabIndex = 2;
            this.btnSolution1.Text = "解法一";
            this.btnSolution1.UseVisualStyleBackColor = true;
            this.btnSolution1.Click += new System.EventHandler(this.btnSolution1_Click);
            // 
            // btnSolution2
            // 
            this.btnSolution2.Location = new System.Drawing.Point(332, 28);
            this.btnSolution2.Name = "btnSolution2";
            this.btnSolution2.Size = new System.Drawing.Size(112, 40);
            this.btnSolution2.TabIndex = 3;
            this.btnSolution2.Text = "解法二";
            this.btnSolution2.UseVisualStyleBackColor = true;
            this.btnSolution2.Click += new System.EventHandler(this.btnSolution2_Click);
            // 
            // btnSolution3
            // 
            this.btnSolution3.Location = new System.Drawing.Point(460, 28);
            this.btnSolution3.Name = "btnSolution3";
            this.btnSolution3.Size = new System.Drawing.Size(112, 40);
            this.btnSolution3.TabIndex = 4;
            this.btnSolution3.Text = "解法三";
            this.btnSolution3.UseVisualStyleBackColor = true;
            this.btnSolution3.Click += new System.EventHandler(this.btnSolution3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 201);
            this.Controls.Add(this.btnSolution3);
            this.Controls.Add(this.btnSolution2);
            this.Controls.Add(this.btnSolution1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Font = new System.Drawing.Font("PMingLiU", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSolution1;
        private System.Windows.Forms.Button btnSolution2;
        private System.Windows.Forms.Button btnSolution3;
    }
}

