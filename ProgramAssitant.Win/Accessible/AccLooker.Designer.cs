namespace AccessibleProject
{
    partial class AccLooker
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.button_Scan = new System.Windows.Forms.Button();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.InforPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button_Scan
            // 
            this.button_Scan.Location = new System.Drawing.Point(12, 12);
            this.button_Scan.Name = "button_Scan";
            this.button_Scan.Size = new System.Drawing.Size(53, 50);
            this.button_Scan.TabIndex = 0;
            this.button_Scan.Text = "T";
            this.button_Scan.UseVisualStyleBackColor = true;
            this.button_Scan.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button_Scan_MouseDown);
            this.button_Scan.MouseMove += new System.Windows.Forms.MouseEventHandler(this.button_Scan_MouseMove);
            this.button_Scan.MouseUp += new System.Windows.Forms.MouseEventHandler(this.button_Scan_MouseUp);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Location = new System.Drawing.Point(64, 127);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(8, 8);
            this.propertyGrid1.TabIndex = 1;
            // 
            // InforPropertyGrid
            // 
            this.InforPropertyGrid.Location = new System.Drawing.Point(12, 68);
            this.InforPropertyGrid.Name = "InforPropertyGrid";
            this.InforPropertyGrid.Size = new System.Drawing.Size(650, 414);
            this.InforPropertyGrid.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(104, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(484, 27);
            this.label1.TabIndex = 3;
            this.label1.Text = "按下 T，移动鼠标捕获窗体控件属性";
            // 
            // AccLooker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(674, 494);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.InforPropertyGrid);
            this.Controls.Add(this.propertyGrid1);
            this.Controls.Add(this.button_Scan);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Name = "AccLooker";
            this.Text = "AccLooker";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_Scan;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.PropertyGrid InforPropertyGrid;
        private System.Windows.Forms.Label label1;
    }
}

