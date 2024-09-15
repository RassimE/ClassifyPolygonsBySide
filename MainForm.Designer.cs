namespace TestApp
{
	partial class MainForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsBitmapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.drawGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.zoomToExtendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openMapData = new System.Windows.Forms.OpenFileDialog();
			this.saveMapData = new System.Windows.Forms.SaveFileDialog();
			this.saveAsBitmap = new System.Windows.Forms.SaveFileDialog();
			this.mapToolStrip1 = new MiniMap.MapToolStrip();
			this.mapControl1 = new MiniMap.MapControl();
			this.mapStatus1 = new MiniMap.MapStatusStrip();
			this.scaleToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.lblTime3 = new System.Windows.Forms.Label();
			this.btnBenc3 = new System.Windows.Forms.Button();
			this.lblTime2 = new System.Windows.Forms.Label();
			this.btnBenc2 = new System.Windows.Forms.Button();
			this.lblTime1 = new System.Windows.Forms.Label();
			this.btnBenc1 = new System.Windows.Forms.Button();
			this.btnReverse = new System.Windows.Forms.Button();
			this.btnRemove = new System.Windows.Forms.Button();
			this.btnClaasify = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.methodUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.labelCnt = new System.Windows.Forms.Label();
			this.menuStrip1.SuspendLayout();
			this.mapStatus1.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.methodUpDown1)).BeginInit();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolStripMenuItem2,
            this.helpToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(945, 24);
			this.menuStrip1.TabIndex = 2;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsBitmapToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
			this.openToolStripMenuItem.Text = "&Open ...";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
			this.saveToolStripMenuItem.Text = "&Save...";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// saveAsBitmapToolStripMenuItem
			// 
			this.saveAsBitmapToolStripMenuItem.Name = "saveAsBitmapToolStripMenuItem";
			this.saveAsBitmapToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
			this.saveAsBitmapToolStripMenuItem.Text = "Save as bitmap...";
			this.saveAsBitmapToolStripMenuItem.Click += new System.EventHandler(this.saveAsBitmapToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(159, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.drawGridToolStripMenuItem,
            this.zoomToExtendToolStripMenuItem});
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(44, 20);
			this.toolStripMenuItem2.Text = "View";
			// 
			// drawGridToolStripMenuItem
			// 
			this.drawGridToolStripMenuItem.Name = "drawGridToolStripMenuItem";
			this.drawGridToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
			this.drawGridToolStripMenuItem.Text = "Draw grid";
			this.drawGridToolStripMenuItem.Click += new System.EventHandler(this.drawGridToolStripMenuItem_Click);
			// 
			// zoomToExtendToolStripMenuItem
			// 
			this.zoomToExtendToolStripMenuItem.Name = "zoomToExtendToolStripMenuItem";
			this.zoomToExtendToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
			this.zoomToExtendToolStripMenuItem.Text = "Zoom to full extend";
			this.zoomToExtendToolStripMenuItem.Click += new System.EventHandler(this.zoomToExtendToolStripMenuItem_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.helpToolStripMenuItem.Text = "Help";
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
			this.aboutToolStripMenuItem.Text = "About...";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
			// 
			// openMapData
			// 
			this.openMapData.Filter = "Text files (*.txt)|*.txt|GeoJson files (*.GeoJson)|*.GeoJson|All files (*.*)|*.*";
			// 
			// saveMapData
			// 
			this.saveMapData.Filter = "Text files (*.txt)|*.txt|GeoJson files (*.GeoJson)|*.GeoJson|All files (*.*)|*.*";
			// 
			// saveAsBitmap
			// 
			this.saveAsBitmap.Filter = "Bimap files (*.bmp)|*.bmp|Gif files (*.gif)|*.gif|Png files (*.png)|*.png|Jpeg fi" +
    "les (*.jpg)|*.jpg|All files (*.*)|*.*";
			// 
			// mapToolStrip1
			// 
			this.mapToolStrip1.Location = new System.Drawing.Point(0, 24);
			this.mapToolStrip1.mapControl = this.mapControl1;
			this.mapToolStrip1.Name = "mapToolStrip1";
			this.mapToolStrip1.Size = new System.Drawing.Size(945, 25);
			this.mapToolStrip1.TabIndex = 3;
			this.mapToolStrip1.Text = "mapToolStrip1";
			// 
			// mapControl1
			// 
			this.mapControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mapControl1.DrawGrid = false;
			this.mapControl1.Location = new System.Drawing.Point(155, 49);
			this.mapControl1.Mode = MiniMap.ViewerMode.Pan;
			this.mapControl1.Name = "mapControl1";
			this.mapControl1.Size = new System.Drawing.Size(790, 534);
			this.mapControl1.Step = -1D;
			this.mapControl1.TabIndex = 0;
			this.mapControl1.Units = MiniMap.MapUnits.Meters;
			this.mapControl1.ScaleChanged += new System.EventHandler(this.mapControl1_ScaleChanged);
			// 
			// mapStatus1
			// 
			this.mapStatus1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scaleToolStripStatusLabel});
			this.mapStatus1.Location = new System.Drawing.Point(0, 583);
			this.mapStatus1.mapControl = this.mapControl1;
			this.mapStatus1.Name = "mapStatus1";
			this.mapStatus1.Size = new System.Drawing.Size(945, 24);
			this.mapStatus1.TabIndex = 1;
			this.mapStatus1.Text = "mapStatus1";
			this.mapStatus1.UserFields = false;
			// 
			// scaleToolStripStatusLabel
			// 
			this.scaleToolStripStatusLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.scaleToolStripStatusLabel.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
			this.scaleToolStripStatusLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.scaleToolStripStatusLabel.Name = "scaleToolStripStatusLabel";
			this.scaleToolStripStatusLabel.Size = new System.Drawing.Size(41, 19);
			this.scaleToolStripStatusLabel.Text = "Scale:";
			this.scaleToolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.labelCnt);
			this.panel1.Controls.Add(this.methodUpDown1);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.lblTime3);
			this.panel1.Controls.Add(this.btnBenc3);
			this.panel1.Controls.Add(this.lblTime2);
			this.panel1.Controls.Add(this.btnBenc2);
			this.panel1.Controls.Add(this.lblTime1);
			this.panel1.Controls.Add(this.btnBenc1);
			this.panel1.Controls.Add(this.btnReverse);
			this.panel1.Controls.Add(this.btnRemove);
			this.panel1.Controls.Add(this.btnClaasify);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel1.Location = new System.Drawing.Point(0, 49);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(155, 534);
			this.panel1.TabIndex = 4;
			// 
			// lblTime3
			// 
			this.lblTime3.AutoSize = true;
			this.lblTime3.Location = new System.Drawing.Point(18, 426);
			this.lblTime3.Name = "lblTime3";
			this.lblTime3.Size = new System.Drawing.Size(70, 13);
			this.lblTime3.TabIndex = 8;
			this.lblTime3.Text = "Elapsed time:";
			// 
			// btnBenc3
			// 
			this.btnBenc3.Enabled = false;
			this.btnBenc3.Location = new System.Drawing.Point(18, 389);
			this.btnBenc3.Name = "btnBenc3";
			this.btnBenc3.Size = new System.Drawing.Size(75, 23);
			this.btnBenc3.TabIndex = 7;
			this.btnBenc3.Text = "Benchmark3";
			this.btnBenc3.UseVisualStyleBackColor = true;
			this.btnBenc3.Click += new System.EventHandler(this.btnBenc3_Click);
			// 
			// lblTime2
			// 
			this.lblTime2.AutoSize = true;
			this.lblTime2.Location = new System.Drawing.Point(15, 353);
			this.lblTime2.Name = "lblTime2";
			this.lblTime2.Size = new System.Drawing.Size(70, 13);
			this.lblTime2.TabIndex = 6;
			this.lblTime2.Text = "Elapsed time:";
			// 
			// btnBenc2
			// 
			this.btnBenc2.Enabled = false;
			this.btnBenc2.Location = new System.Drawing.Point(15, 316);
			this.btnBenc2.Name = "btnBenc2";
			this.btnBenc2.Size = new System.Drawing.Size(75, 23);
			this.btnBenc2.TabIndex = 5;
			this.btnBenc2.Text = "Benchmark2";
			this.btnBenc2.UseVisualStyleBackColor = true;
			this.btnBenc2.Click += new System.EventHandler(this.btnBenc2_Click);
			// 
			// lblTime1
			// 
			this.lblTime1.AutoSize = true;
			this.lblTime1.Location = new System.Drawing.Point(12, 276);
			this.lblTime1.Name = "lblTime1";
			this.lblTime1.Size = new System.Drawing.Size(70, 13);
			this.lblTime1.TabIndex = 4;
			this.lblTime1.Text = "Elapsed time:";
			// 
			// btnBenc1
			// 
			this.btnBenc1.Enabled = false;
			this.btnBenc1.Location = new System.Drawing.Point(12, 239);
			this.btnBenc1.Name = "btnBenc1";
			this.btnBenc1.Size = new System.Drawing.Size(75, 23);
			this.btnBenc1.TabIndex = 3;
			this.btnBenc1.Text = "Benchmark1";
			this.btnBenc1.UseVisualStyleBackColor = true;
			this.btnBenc1.Click += new System.EventHandler(this.btnBenc1_Click);
			// 
			// btnReverse
			// 
			this.btnReverse.Location = new System.Drawing.Point(12, 184);
			this.btnReverse.Name = "btnReverse";
			this.btnReverse.Size = new System.Drawing.Size(75, 23);
			this.btnReverse.TabIndex = 2;
			this.btnReverse.Text = "Reverse";
			this.btnReverse.UseVisualStyleBackColor = true;
			this.btnReverse.Click += new System.EventHandler(this.btnReverse_Click);
			// 
			// btnRemove
			// 
			this.btnRemove.Enabled = false;
			this.btnRemove.Location = new System.Drawing.Point(12, 135);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(75, 23);
			this.btnRemove.TabIndex = 1;
			this.btnRemove.Text = "Remove result";
			this.btnRemove.UseVisualStyleBackColor = true;
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// btnClaasify
			// 
			this.btnClaasify.Enabled = false;
			this.btnClaasify.Location = new System.Drawing.Point(12, 90);
			this.btnClaasify.Name = "btnClaasify";
			this.btnClaasify.Size = new System.Drawing.Size(75, 23);
			this.btnClaasify.TabIndex = 0;
			this.btnClaasify.Text = "Classify";
			this.btnClaasify.UseVisualStyleBackColor = true;
			this.btnClaasify.Click += new System.EventHandler(this.btnClaasify_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(52, 13);
			this.label1.TabIndex = 9;
			this.label1.Text = "Mehthod:";
			// 
			// methodUpDown1
			// 
			this.methodUpDown1.Location = new System.Drawing.Point(79, 19);
			this.methodUpDown1.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
			this.methodUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.methodUpDown1.Name = "methodUpDown1";
			this.methodUpDown1.Size = new System.Drawing.Size(49, 20);
			this.methodUpDown1.TabIndex = 10;
			this.methodUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// labelCnt
			// 
			this.labelCnt.AutoSize = true;
			this.labelCnt.Location = new System.Drawing.Point(18, 56);
			this.labelCnt.Name = "labelCnt";
			this.labelCnt.Size = new System.Drawing.Size(13, 13);
			this.labelCnt.TabIndex = 11;
			this.labelCnt.Text = "0";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(945, 607);
			this.Controls.Add(this.mapControl1);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.mapToolStrip1);
			this.Controls.Add(this.mapStatus1);
			this.Controls.Add(this.menuStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainForm";
			this.Text = "Test Application";
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.mapStatus1.ResumeLayout(false);
			this.mapStatus1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.methodUpDown1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private MiniMap.MapControl mapControl1;
		private MiniMap.MapStatusStrip mapStatus1;
		private System.Windows.Forms.ToolStripStatusLabel scaleToolStripStatusLabel;
		private System.Windows.Forms.OpenFileDialog openMapData;
		private System.Windows.Forms.SaveFileDialog saveMapData;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem zoomToExtendToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem drawGridToolStripMenuItem;
		private MiniMap.MapToolStrip mapToolStrip1;
		private System.Windows.Forms.ToolStripMenuItem saveAsBitmapToolStripMenuItem;
		private System.Windows.Forms.SaveFileDialog saveAsBitmap;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button btnClaasify;
		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.Button btnReverse;
		private System.Windows.Forms.Button btnBenc1;
		private System.Windows.Forms.Label lblTime1;
		private System.Windows.Forms.Label lblTime2;
		private System.Windows.Forms.Button btnBenc2;
		private System.Windows.Forms.Label lblTime3;
		private System.Windows.Forms.Button btnBenc3;
		private System.Windows.Forms.NumericUpDown methodUpDown1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelCnt;
	}
}

