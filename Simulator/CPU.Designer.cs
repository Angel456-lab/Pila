namespace Simulator
{
    partial class CPU
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
            this.Memory = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.BR = new System.Windows.Forms.DataGridView();
            this.label8 = new System.Windows.Forms.Label();
            this.X = new System.Windows.Forms.TextBox();
            this.Y = new System.Windows.Forms.TextBox();
            this.IR = new System.Windows.Forms.TextBox();
            this.PC = new System.Windows.Forms.TextBox();
            this.MAR = new System.Windows.Forms.TextBox();
            this.MDR = new System.Windows.Forms.TextBox();
            this.Z = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.Memory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BR)).BeginInit();
            this.SuspendLayout();
            // 
            // Memory
            // 
            this.Memory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Memory.Location = new System.Drawing.Point(672, 44);
            this.Memory.Name = "Memory";
            this.Memory.Size = new System.Drawing.Size(190, 304);
            this.Memory.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(372, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "MAR";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(372, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "MDR";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(372, 207);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "PC";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(373, 257);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(18, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "IR";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(69, 218);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "X";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(196, 218);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(14, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Y";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(133, 362);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(14, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "Z";
            // 
            // BR
            // 
            this.BR.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.BR.Location = new System.Drawing.Point(72, 80);
            this.BR.Name = "BR";
            this.BR.Size = new System.Drawing.Size(186, 119);
            this.BR.TabIndex = 8;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(69, 55);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(100, 13);
            this.label8.TabIndex = 9;
            this.label8.Text = "Banco de Registros";
            // 
            // X
            // 
            this.X.Location = new System.Drawing.Point(72, 237);
            this.X.Name = "X";
            this.X.ReadOnly = true;
            this.X.Size = new System.Drawing.Size(79, 20);
            this.X.TabIndex = 10;
            // 
            // Y
            // 
            this.Y.Location = new System.Drawing.Point(193, 237);
            this.Y.Name = "Y";
            this.Y.ReadOnly = true;
            this.Y.Size = new System.Drawing.Size(79, 20);
            this.Y.TabIndex = 11;
            // 
            // IR
            // 
            this.IR.Location = new System.Drawing.Point(368, 276);
            this.IR.Name = "IR";
            this.IR.ReadOnly = true;
            this.IR.Size = new System.Drawing.Size(130, 20);
            this.IR.TabIndex = 12;
            // 
            // PC
            // 
            this.PC.Location = new System.Drawing.Point(368, 229);
            this.PC.Name = "PC";
            this.PC.ReadOnly = true;
            this.PC.Size = new System.Drawing.Size(130, 20);
            this.PC.TabIndex = 13;
            // 
            // MAR
            // 
            this.MAR.Location = new System.Drawing.Point(368, 41);
            this.MAR.Name = "MAR";
            this.MAR.ReadOnly = true;
            this.MAR.Size = new System.Drawing.Size(130, 20);
            this.MAR.TabIndex = 14;
            // 
            // MDR
            // 
            this.MDR.Location = new System.Drawing.Point(368, 89);
            this.MDR.Name = "MDR";
            this.MDR.ReadOnly = true;
            this.MDR.Size = new System.Drawing.Size(130, 20);
            this.MDR.TabIndex = 15;
            // 
            // Z
            // 
            this.Z.Location = new System.Drawing.Point(128, 379);
            this.Z.Name = "Z";
            this.Z.ReadOnly = true;
            this.Z.Size = new System.Drawing.Size(79, 20);
            this.Z.TabIndex = 16;
            // 
            // CPU
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Simulator.Properties.Resources.R3;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(877, 526);
            this.Controls.Add(this.Z);
            this.Controls.Add(this.MDR);
            this.Controls.Add(this.MAR);
            this.Controls.Add(this.PC);
            this.Controls.Add(this.IR);
            this.Controls.Add(this.Y);
            this.Controls.Add(this.X);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.BR);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Memory);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CPU";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CPU";
            this.Load += new System.EventHandler(this.CPU_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Memory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BR)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView Memory;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DataGridView BR;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox X;
        private System.Windows.Forms.TextBox Y;
        private System.Windows.Forms.TextBox IR;
        private System.Windows.Forms.TextBox PC;
        private System.Windows.Forms.TextBox MAR;
        private System.Windows.Forms.TextBox MDR;
        private System.Windows.Forms.TextBox Z;
    }
}