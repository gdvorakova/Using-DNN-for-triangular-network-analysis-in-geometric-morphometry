namespace _101meshEditor
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
            this.glControl1 = new OpenTK.GLControl();
            this.textParam = new System.Windows.Forms.TextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.saveButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.Open = new System.Windows.Forms.ToolStripMenuItem();
            this.loadTextureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportSelection = new System.Windows.Forms.ToolStripMenuItem();
            this.exportBatch = new System.Windows.Forms.ToolStripMenuItem();
            this.startLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.edit = new System.Windows.Forms.ToolStripDropDownButton();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.brushSelection = new System.Windows.Forms.ToolStripMenuItem();
            this.pointSelection = new System.Windows.Forms.ToolStripMenuItem();
            this.triangleSelection = new System.Windows.Forms.ToolStripMenuItem();
            this.button_View = new System.Windows.Forms.ToolStripDropDownButton();
            this.resetCamToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkAxes = new System.Windows.Forms.ToolStripMenuItem();
            this.checkTexture = new System.Windows.Forms.ToolStripMenuItem();
            this.checkSmooth = new System.Windows.Forms.ToolStripMenuItem();
            this.checkWireframe = new System.Windows.Forms.ToolStripMenuItem();
            this.checkTwosided = new System.Windows.Forms.ToolStripMenuItem();
            this.checkShaders = new System.Windows.Forms.ToolStripMenuItem();
            this.checkGlobalColor = new System.Windows.Forms.ToolStripMenuItem();
            this.checkAmbient = new System.Windows.Forms.ToolStripMenuItem();
            this.checkDiffuse = new System.Windows.Forms.ToolStripMenuItem();
            this.checkSpecular = new System.Windows.Forms.ToolStripMenuItem();
            this.checkPhong = new System.Windows.Forms.ToolStripMenuItem();
            this.checkVsync = new System.Windows.Forms.ToolStripMenuItem();
            this.checkOrientation = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.labelFile = new System.Windows.Forms.ToolStripLabel();
            this.label_percentageReport = new System.Windows.Forms.ToolStripLabel();
            this.stopButton = new System.Windows.Forms.ToolStripButton();
            this.labelFps = new System.Windows.Forms.ToolStripLabel();
            this.saveLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.toolStrip3.SuspendLayout();
            this.SuspendLayout();
            // 
            // glControl1
            // 
            this.glControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.glControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.glControl1.BackColor = System.Drawing.Color.Black;
            this.glControl1.Location = new System.Drawing.Point(13, 35);
            this.glControl1.Margin = new System.Windows.Forms.Padding(4);
            this.glControl1.Name = "glControl1";
            this.glControl1.Size = new System.Drawing.Size(857, 508);
            this.glControl1.TabIndex = 17;
            this.glControl1.VSync = false;
            this.glControl1.Load += new System.EventHandler(this.glControl1_Load);
            this.glControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl1_Paint);
            this.glControl1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.glControl1_KeyDown);
            this.glControl1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.glControl1_KeyUp);
            this.glControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseDown);
            this.glControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseMove);
            this.glControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseUp);
            this.glControl1.Resize += new System.EventHandler(this.glControl1_Resize);
            // 
            // textParam
            // 
            this.textParam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textParam.Location = new System.Drawing.Point(811, 550);
            this.textParam.Name = "textParam";
            this.textParam.Size = new System.Drawing.Size(60, 20);
            this.textParam.TabIndex = 29;
            this.textParam.Visible = false;
            this.textParam.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textParam_KeyPress);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.White;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveButton,
            this.edit,
            this.toolStripDropDownButton1,
            this.button_View});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(889, 25);
            this.toolStrip1.TabIndex = 56;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.MouseEnter += new System.EventHandler(this.toolStrip1_MouseEnter);
            this.toolStrip1.MouseLeave += new System.EventHandler(this.toolStrip1_MouseLeave);
            // 
            // saveButton
            // 
            this.saveButton.AutoToolTip = false;
            this.saveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.saveButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Open,
            this.loadTextureToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.exportSelection,
            this.exportBatch,
            this.startLogToolStripMenuItem,
            this.stopLogToolStripMenuItem});
            this.saveButton.ForeColor = System.Drawing.Color.Black;
            this.saveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveButton.Name = "saveButton";
            this.saveButton.ShowDropDownArrow = false;
            this.saveButton.Size = new System.Drawing.Size(29, 22);
            this.saveButton.Text = "File";
            this.saveButton.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.saveButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            // 
            // Open
            // 
            this.Open.Name = "Open";
            this.Open.Size = new System.Drawing.Size(190, 22);
            this.Open.Text = "Load";
            this.Open.Click += new System.EventHandler(this.loadSceneToolStripMenuItem_Click);
            // 
            // loadTextureToolStripMenuItem
            // 
            this.loadTextureToolStripMenuItem.Name = "loadTextureToolStripMenuItem";
            this.loadTextureToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.loadTextureToolStripMenuItem.Text = "Load Texture";
            this.loadTextureToolStripMenuItem.Click += new System.EventHandler(this.loadTextureToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Image = global::_101meshEditor.Properties.Resources.save;
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // exportSelection
            // 
            this.exportSelection.Name = "exportSelection";
            this.exportSelection.Size = new System.Drawing.Size(190, 22);
            this.exportSelection.Text = "Export selection";
            this.exportSelection.Click += new System.EventHandler(this.exportSelectionToolStripMenuItem_Click);
            // 
            // exportBatch
            // 
            this.exportBatch.Name = "exportBatch";
            this.exportBatch.Size = new System.Drawing.Size(190, 22);
            this.exportBatch.Text = "Export selection batch";
            this.exportBatch.ToolTipText = "Exports selection from multiple files in selected path";
            this.exportBatch.Click += new System.EventHandler(this.exportBatch_Click);
            // 
            // startLogToolStripMenuItem
            // 
            this.startLogToolStripMenuItem.Name = "startLogToolStripMenuItem";
            this.startLogToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.startLogToolStripMenuItem.Text = "Start Log";
            this.startLogToolStripMenuItem.Click += new System.EventHandler(this.startLogToolStripMenuItem_Click);
            // 
            // stopLogToolStripMenuItem
            // 
            this.stopLogToolStripMenuItem.Name = "stopLogToolStripMenuItem";
            this.stopLogToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.stopLogToolStripMenuItem.Text = "Stop Log";
            this.stopLogToolStripMenuItem.Click += new System.EventHandler(this.stopLogToolStripMenuItem_Click);
            // 
            // edit
            // 
            this.edit.AutoToolTip = false;
            this.edit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.edit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem});
            this.edit.ForeColor = System.Drawing.Color.Black;
            this.edit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.edit.Name = "edit";
            this.edit.ShowDropDownArrow = false;
            this.edit.Size = new System.Drawing.Size(31, 22);
            this.edit.Text = "Edit";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Enabled = false;
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoButton_Click);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.AutoToolTip = false;
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.brushSelection,
            this.pointSelection,
            this.triangleSelection});
            this.toolStripDropDownButton1.ForeColor = System.Drawing.Color.Black;
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.ShowDropDownArrow = false;
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(42, 22);
            this.toolStripDropDownButton1.Text = "Select";
            // 
            // brushSelection
            // 
            this.brushSelection.CheckOnClick = true;
            this.brushSelection.Image = global::_101meshEditor.Properties.Resources.brush;
            this.brushSelection.Name = "brushSelection";
            this.brushSelection.Size = new System.Drawing.Size(166, 22);
            this.brushSelection.Text = "Brush selection";
            this.brushSelection.CheckedChanged += new System.EventHandler(this.brushSelection_CheckedChanged);
            this.brushSelection.Click += new System.EventHandler(this.button_RingSelection_CheckedChanged);
            // 
            // pointSelection
            // 
            this.pointSelection.CheckOnClick = true;
            this.pointSelection.Image = global::_101meshEditor.Properties.Resources.vertex;
            this.pointSelection.Name = "pointSelection";
            this.pointSelection.Size = new System.Drawing.Size(166, 22);
            this.pointSelection.Text = "Point selection";
            this.pointSelection.CheckedChanged += new System.EventHandler(this.pointSelection_CheckedChanged);
            this.pointSelection.Click += new System.EventHandler(this.checkVertices_CheckedChanged);
            // 
            // triangleSelection
            // 
            this.triangleSelection.CheckOnClick = true;
            this.triangleSelection.Image = global::_101meshEditor.Properties.Resources.triangle;
            this.triangleSelection.Name = "triangleSelection";
            this.triangleSelection.Size = new System.Drawing.Size(166, 22);
            this.triangleSelection.Text = "Triangle selection";
            this.triangleSelection.CheckedChanged += new System.EventHandler(this.triangleSelection_CheckedChanged);
            this.triangleSelection.Click += new System.EventHandler(this.checkTriangles_CheckedChanged);
            // 
            // button_View
            // 
            this.button_View.AutoToolTip = false;
            this.button_View.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.button_View.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetCamToolStripMenuItem,
            this.checkAxes,
            this.checkTexture,
            this.checkSmooth,
            this.checkWireframe,
            this.checkTwosided,
            this.checkShaders,
            this.checkGlobalColor,
            this.checkAmbient,
            this.checkDiffuse,
            this.checkSpecular,
            this.checkPhong,
            this.checkVsync,
            this.checkOrientation});
            this.button_View.ForeColor = System.Drawing.Color.Black;
            this.button_View.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.button_View.Name = "button_View";
            this.button_View.ShowDropDownArrow = false;
            this.button_View.Size = new System.Drawing.Size(36, 22);
            this.button_View.Text = "View";
            // 
            // resetCamToolStripMenuItem
            // 
            this.resetCamToolStripMenuItem.Image = global::_101meshEditor.Properties.Resources.cam2;
            this.resetCamToolStripMenuItem.Name = "resetCamToolStripMenuItem";
            this.resetCamToolStripMenuItem.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.resetCamToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.resetCamToolStripMenuItem.Text = "Reset cam";
            this.resetCamToolStripMenuItem.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // checkAxes
            // 
            this.checkAxes.CheckOnClick = true;
            this.checkAxes.Name = "checkAxes";
            this.checkAxes.Size = new System.Drawing.Size(138, 22);
            this.checkAxes.Text = "Axes";
            // 
            // checkTexture
            // 
            this.checkTexture.CheckOnClick = true;
            this.checkTexture.Name = "checkTexture";
            this.checkTexture.Size = new System.Drawing.Size(138, 22);
            this.checkTexture.Text = "Texture";
            // 
            // checkSmooth
            // 
            this.checkSmooth.Checked = true;
            this.checkSmooth.CheckOnClick = true;
            this.checkSmooth.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkSmooth.Name = "checkSmooth";
            this.checkSmooth.Size = new System.Drawing.Size(138, 22);
            this.checkSmooth.Text = "Smooth";
            // 
            // checkWireframe
            // 
            this.checkWireframe.Checked = true;
            this.checkWireframe.CheckOnClick = true;
            this.checkWireframe.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkWireframe.Name = "checkWireframe";
            this.checkWireframe.Size = new System.Drawing.Size(138, 22);
            this.checkWireframe.Text = "Wire";
            // 
            // checkTwosided
            // 
            this.checkTwosided.Checked = true;
            this.checkTwosided.CheckOnClick = true;
            this.checkTwosided.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkTwosided.Name = "checkTwosided";
            this.checkTwosided.Size = new System.Drawing.Size(138, 22);
            this.checkTwosided.Text = "2-Sided";
            // 
            // checkShaders
            // 
            this.checkShaders.CheckOnClick = true;
            this.checkShaders.Name = "checkShaders";
            this.checkShaders.Size = new System.Drawing.Size(138, 22);
            this.checkShaders.Text = "GLSL";
            // 
            // checkGlobalColor
            // 
            this.checkGlobalColor.CheckOnClick = true;
            this.checkGlobalColor.Name = "checkGlobalColor";
            this.checkGlobalColor.Size = new System.Drawing.Size(138, 22);
            this.checkGlobalColor.Text = "Global color";
            // 
            // checkAmbient
            // 
            this.checkAmbient.Checked = true;
            this.checkAmbient.CheckOnClick = true;
            this.checkAmbient.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkAmbient.Name = "checkAmbient";
            this.checkAmbient.Size = new System.Drawing.Size(138, 22);
            this.checkAmbient.Text = "Ambient";
            // 
            // checkDiffuse
            // 
            this.checkDiffuse.Checked = true;
            this.checkDiffuse.CheckOnClick = true;
            this.checkDiffuse.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkDiffuse.Name = "checkDiffuse";
            this.checkDiffuse.Size = new System.Drawing.Size(138, 22);
            this.checkDiffuse.Text = "Diffuse";
            // 
            // checkSpecular
            // 
            this.checkSpecular.Checked = true;
            this.checkSpecular.CheckOnClick = true;
            this.checkSpecular.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkSpecular.Name = "checkSpecular";
            this.checkSpecular.Size = new System.Drawing.Size(138, 22);
            this.checkSpecular.Text = "Specular";
            // 
            // checkPhong
            // 
            this.checkPhong.CheckOnClick = true;
            this.checkPhong.Name = "checkPhong";
            this.checkPhong.Size = new System.Drawing.Size(138, 22);
            this.checkPhong.Text = "Phong";
            // 
            // checkVsync
            // 
            this.checkVsync.CheckOnClick = true;
            this.checkVsync.Name = "checkVsync";
            this.checkVsync.Size = new System.Drawing.Size(138, 22);
            this.checkVsync.Text = "V-Sync";
            // 
            // checkOrientation
            // 
            this.checkOrientation.CheckOnClick = true;
            this.checkOrientation.Name = "checkOrientation";
            this.checkOrientation.Size = new System.Drawing.Size(138, 22);
            this.checkOrientation.Text = "Orientation";
            // 
            // toolStrip3
            // 
            this.toolStrip3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip3.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labelFile,
            this.label_percentageReport,
            this.stopButton,
            this.labelFps});
            this.toolStrip3.Location = new System.Drawing.Point(0, 567);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size(889, 25);
            this.toolStrip3.TabIndex = 58;
            this.toolStrip3.Text = "toolStrip3";
            this.toolStrip3.MouseEnter += new System.EventHandler(this.toolStrip1_MouseEnter);
            this.toolStrip3.MouseLeave += new System.EventHandler(this.toolStrip1_MouseLeave);
            // 
            // labelFile
            // 
            this.labelFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.labelFile.ForeColor = System.Drawing.Color.Black;
            this.labelFile.Name = "labelFile";
            this.labelFile.Size = new System.Drawing.Size(81, 22);
            this.labelFile.Text = "No file loaded";
            // 
            // label_percentageReport
            // 
            this.label_percentageReport.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.label_percentageReport.Name = "label_percentageReport";
            this.label_percentageReport.Size = new System.Drawing.Size(0, 22);
            // 
            // stopButton
            // 
            this.stopButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.stopButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.stopButton.Image = global::_101meshEditor.Properties.Resources.stop;
            this.stopButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(24, 24);
            this.stopButton.ToolTipText = "Stop";
            this.stopButton.Visible = false;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // labelFps
            // 
            this.labelFps.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.labelFps.Name = "labelFps";
            this.labelFps.Size = new System.Drawing.Size(28, 22);
            this.labelFps.Text = "Fps:";
            // 
            // saveLogToolStripMenuItem
            // 
            this.saveLogToolStripMenuItem.Name = "saveLogToolStripMenuItem";
            this.saveLogToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // newLogToolStripMenuItem
            // 
            this.newLogToolStripMenuItem.Name = "newLogToolStripMenuItem";
            this.newLogToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Enabled = false;
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.redoToolStripMenuItem.Text = "Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(889, 592);
            this.Controls.Add(this.toolStrip3);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.textParam);
            this.Controls.Add(this.glControl1);
            this.MinimumSize = new System.Drawing.Size(860, 349);
            this.Name = "Form1";
            this.Text = "Mesh Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseWheel);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OpenTK.GLControl glControl1;
        private System.Windows.Forms.TextBox textParam;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton saveButton;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportSelection;
        private System.Windows.Forms.ToolStripMenuItem Open;
        private System.Windows.Forms.ToolStripMenuItem loadTextureToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem brushSelection;
        private System.Windows.Forms.ToolStripMenuItem pointSelection;
        private System.Windows.Forms.ToolStripMenuItem triangleSelection;
        private System.Windows.Forms.ToolStripDropDownButton edit;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteSelectionToolStripMenuItem1;
        private System.Windows.Forms.ToolStripDropDownButton button_View;
        private System.Windows.Forms.ToolStripMenuItem resetCamToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkSmooth;
        private System.Windows.Forms.ToolStripMenuItem checkWireframe;
        private System.Windows.Forms.ToolStripMenuItem checkTwosided;
        private System.Windows.Forms.ToolStripMenuItem checkAxes;
        private System.Windows.Forms.ToolStripMenuItem checkTexture;
        private System.Windows.Forms.ToolStripMenuItem checkShaders;
        private System.Windows.Forms.ToolStripMenuItem checkGlobalColor;
        private System.Windows.Forms.ToolStripMenuItem checkAmbient;
        private System.Windows.Forms.ToolStripMenuItem checkDiffuse;
        private System.Windows.Forms.ToolStripMenuItem checkSpecular;
        private System.Windows.Forms.ToolStripMenuItem checkPhong;
        private System.Windows.Forms.ToolStripMenuItem checkVsync;
        private System.Windows.Forms.ToolStrip toolStrip3;
        private System.Windows.Forms.ToolStripLabel labelFile;
        private System.Windows.Forms.ToolStripButton stopButton;
        private System.Windows.Forms.ToolStripLabel label_percentageReport;
        private System.Windows.Forms.ToolStripLabel labelFps;
        private System.Windows.Forms.ToolStripMenuItem checkOrientation;
        private System.Windows.Forms.ToolStripMenuItem exportBatch;
        private System.Windows.Forms.ToolStripMenuItem newLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
    }
}

