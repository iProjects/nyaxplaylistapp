namespace nyaxplaylistapp_ui
{
    partial class create_playlist
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(create_playlist));
            this.lblmediatype = new System.Windows.Forms.Label();
            this.cbomediatype = new System.Windows.Forms.ComboBox();
            this.lblbuildversion = new System.Windows.Forms.Label();
            this.txtlog = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.lblprogresspercentage = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbldatetime = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbltimelapsed = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btncancelworker = new System.Windows.Forms.Button();
            this.btngenerateforalltypes = new System.Windows.Forms.Button();
            this.btnexit = new System.Windows.Forms.Button();
            this.lbltotalfilesprocessed = new System.Windows.Forms.Label();
            this.btngenerate_playlist_json = new System.Windows.Forms.Button();
            this.lblprogresscounta = new System.Windows.Forms.Label();
            this.notifyIconntharene = new System.Windows.Forms.NotifyIcon(this.components);
            this.txtdestination = new System.Windows.Forms.TextBox();
            this.txtsource = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chkcreatefilesifnotexists = new System.Windows.Forms.CheckBox();
            this.chkcreatefoldersifnotexist = new System.Windows.Forms.CheckBox();
            this.btnreports = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblmediatype
            // 
            this.lblmediatype.AutoSize = true;
            this.lblmediatype.BackColor = System.Drawing.Color.Black;
            this.lblmediatype.ForeColor = System.Drawing.Color.Lime;
            this.lblmediatype.Location = new System.Drawing.Point(37, 204);
            this.lblmediatype.Name = "lblmediatype";
            this.lblmediatype.Size = new System.Drawing.Size(58, 13);
            this.lblmediatype.TabIndex = 1;
            this.lblmediatype.Text = "media type";
            // 
            // cbomediatype
            // 
            this.cbomediatype.BackColor = System.Drawing.Color.Black;
            this.cbomediatype.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbomediatype.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbomediatype.ForeColor = System.Drawing.Color.Lime;
            this.cbomediatype.FormattingEnabled = true;
            this.cbomediatype.Location = new System.Drawing.Point(101, 201);
            this.cbomediatype.Name = "cbomediatype";
            this.cbomediatype.Size = new System.Drawing.Size(182, 21);
            this.cbomediatype.TabIndex = 2;
            this.cbomediatype.SelectedIndexChanged += new System.EventHandler(this.cbomediatype_SelectedIndexChanged);
            // 
            // lblbuildversion
            // 
            this.lblbuildversion.AutoSize = true;
            this.lblbuildversion.BackColor = System.Drawing.Color.Black;
            this.lblbuildversion.ForeColor = System.Drawing.Color.Lime;
            this.lblbuildversion.Location = new System.Drawing.Point(332, 209);
            this.lblbuildversion.Name = "lblbuildversion";
            this.lblbuildversion.Size = new System.Drawing.Size(66, 13);
            this.lblbuildversion.TabIndex = 3;
            this.lblbuildversion.Text = "build version";
            // 
            // txtlog
            // 
            this.txtlog.BackColor = System.Drawing.Color.Black;
            this.txtlog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtlog.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtlog.ForeColor = System.Drawing.Color.Lime;
            this.txtlog.Location = new System.Drawing.Point(0, 0);
            this.txtlog.Multiline = true;
            this.txtlog.Name = "txtlog";
            this.txtlog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtlog.Size = new System.Drawing.Size(498, 195);
            this.txtlog.TabIndex = 4;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar,
            this.lblprogresspercentage,
            this.lbldatetime,
            this.lbltimelapsed});
            this.statusStrip1.Location = new System.Drawing.Point(0, 423);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.statusStrip1.Size = new System.Drawing.Size(498, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.toolStripProgressBar.ForeColor = System.Drawing.Color.Lime;
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.Size = new System.Drawing.Size(150, 16);
            this.toolStripProgressBar.Step = 1;
            this.toolStripProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // lblprogresspercentage
            // 
            this.lblprogresspercentage.Name = "lblprogresspercentage";
            this.lblprogresspercentage.Size = new System.Drawing.Size(66, 17);
            this.lblprogresspercentage.Text = "percentage";
            // 
            // lbldatetime
            // 
            this.lbldatetime.BackColor = System.Drawing.Color.White;
            this.lbldatetime.ForeColor = System.Drawing.Color.Black;
            this.lbldatetime.Name = "lbldatetime";
            this.lbldatetime.Size = new System.Drawing.Size(31, 17);
            this.lbldatetime.Text = "time";
            // 
            // lbltimelapsed
            // 
            this.lbltimelapsed.BackColor = System.Drawing.Color.White;
            this.lbltimelapsed.ForeColor = System.Drawing.Color.Black;
            this.lbltimelapsed.Name = "lbltimelapsed";
            this.lbltimelapsed.Size = new System.Drawing.Size(74, 17);
            this.lbltimelapsed.Text = "time elapsed";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.btncancelworker);
            this.groupBox1.Controls.Add(this.btngenerateforalltypes);
            this.groupBox1.Controls.Add(this.btnexit);
            this.groupBox1.Controls.Add(this.lbltotalfilesprocessed);
            this.groupBox1.Controls.Add(this.btngenerate_playlist_json);
            this.groupBox1.Controls.Add(this.lblprogresscounta);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(0, 352);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(498, 71);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            // 
            // btncancelworker
            // 
            this.btncancelworker.BackColor = System.Drawing.Color.Black;
            this.btncancelworker.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btncancelworker.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.btncancelworker.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkMagenta;
            this.btncancelworker.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Navy;
            this.btncancelworker.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btncancelworker.ForeColor = System.Drawing.Color.Lime;
            this.btncancelworker.Location = new System.Drawing.Point(240, 43);
            this.btncancelworker.Name = "btncancelworker";
            this.btncancelworker.Size = new System.Drawing.Size(90, 23);
            this.btncancelworker.TabIndex = 12;
            this.btncancelworker.Text = "cancel worker";
            this.btncancelworker.UseVisualStyleBackColor = false;
            this.btncancelworker.Click += new System.EventHandler(this.btncancelworker_Click);
            // 
            // btngenerateforalltypes
            // 
            this.btngenerateforalltypes.BackColor = System.Drawing.Color.Black;
            this.btngenerateforalltypes.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.btngenerateforalltypes.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkMagenta;
            this.btngenerateforalltypes.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Navy;
            this.btngenerateforalltypes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btngenerateforalltypes.ForeColor = System.Drawing.Color.Lime;
            this.btngenerateforalltypes.Location = new System.Drawing.Point(240, 19);
            this.btngenerateforalltypes.Name = "btngenerateforalltypes";
            this.btngenerateforalltypes.Size = new System.Drawing.Size(146, 23);
            this.btngenerateforalltypes.TabIndex = 5;
            this.btngenerateforalltypes.Text = "generate for all media types";
            this.btngenerateforalltypes.UseVisualStyleBackColor = false;
            this.btngenerateforalltypes.Click += new System.EventHandler(this.btngenerateforalltypes_Click);
            // 
            // btnexit
            // 
            this.btnexit.BackColor = System.Drawing.Color.Black;
            this.btnexit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnexit.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.btnexit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkMagenta;
            this.btnexit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Navy;
            this.btnexit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnexit.ForeColor = System.Drawing.Color.Lime;
            this.btnexit.Location = new System.Drawing.Point(392, 19);
            this.btnexit.Name = "btnexit";
            this.btnexit.Size = new System.Drawing.Size(44, 23);
            this.btnexit.TabIndex = 0;
            this.btnexit.Text = "&exit";
            this.btnexit.UseVisualStyleBackColor = false;
            this.btnexit.Click += new System.EventHandler(this.btnexit_Click);
            // 
            // lbltotalfilesprocessed
            // 
            this.lbltotalfilesprocessed.AutoSize = true;
            this.lbltotalfilesprocessed.BackColor = System.Drawing.Color.Black;
            this.lbltotalfilesprocessed.ForeColor = System.Drawing.Color.Lime;
            this.lbltotalfilesprocessed.Location = new System.Drawing.Point(91, 48);
            this.lbltotalfilesprocessed.Name = "lbltotalfilesprocessed";
            this.lbltotalfilesprocessed.Size = new System.Drawing.Size(100, 13);
            this.lbltotalfilesprocessed.TabIndex = 11;
            this.lbltotalfilesprocessed.Text = "total files processed";
            // 
            // btngenerate_playlist_json
            // 
            this.btngenerate_playlist_json.BackColor = System.Drawing.Color.Black;
            this.btngenerate_playlist_json.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.btngenerate_playlist_json.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkMagenta;
            this.btngenerate_playlist_json.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Navy;
            this.btngenerate_playlist_json.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btngenerate_playlist_json.ForeColor = System.Drawing.Color.Lime;
            this.btngenerate_playlist_json.Location = new System.Drawing.Point(6, 19);
            this.btngenerate_playlist_json.Name = "btngenerate_playlist_json";
            this.btngenerate_playlist_json.Size = new System.Drawing.Size(228, 23);
            this.btngenerate_playlist_json.TabIndex = 0;
            this.btngenerate_playlist_json.Text = "generate playlist json for selected media type";
            this.btngenerate_playlist_json.UseVisualStyleBackColor = false;
            this.btngenerate_playlist_json.Click += new System.EventHandler(this.btngenerate_playlist_json_Click);
            // 
            // lblprogresscounta
            // 
            this.lblprogresscounta.AutoSize = true;
            this.lblprogresscounta.BackColor = System.Drawing.Color.Black;
            this.lblprogresscounta.ForeColor = System.Drawing.Color.Lime;
            this.lblprogresscounta.Location = new System.Drawing.Point(5, 48);
            this.lblprogresscounta.Name = "lblprogresscounta";
            this.lblprogresscounta.Size = new System.Drawing.Size(83, 13);
            this.lblprogresscounta.TabIndex = 4;
            this.lblprogresscounta.Text = "progress counta";
            // 
            // notifyIconntharene
            // 
            this.notifyIconntharene.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIconntharene.Icon")));
            this.notifyIconntharene.Visible = true;
            this.notifyIconntharene.Click += new System.EventHandler(this.notifyIconntharene_Click);
            this.notifyIconntharene.DoubleClick += new System.EventHandler(this.notifyIconntharene_DoubleClick);
            // 
            // txtdestination
            // 
            this.txtdestination.BackColor = System.Drawing.Color.Black;
            this.txtdestination.ForeColor = System.Drawing.Color.Lime;
            this.txtdestination.Location = new System.Drawing.Point(4, 328);
            this.txtdestination.Name = "txtdestination";
            this.txtdestination.Size = new System.Drawing.Size(461, 20);
            this.txtdestination.TabIndex = 7;
            // 
            // txtsource
            // 
            this.txtsource.BackColor = System.Drawing.Color.Black;
            this.txtsource.ForeColor = System.Drawing.Color.Lime;
            this.txtsource.Location = new System.Drawing.Point(3, 284);
            this.txtsource.Name = "txtsource";
            this.txtsource.Size = new System.Drawing.Size(461, 20);
            this.txtsource.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Black;
            this.label1.ForeColor = System.Drawing.Color.Lime;
            this.label1.Location = new System.Drawing.Point(5, 266);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "source";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Black;
            this.label2.ForeColor = System.Drawing.Color.Lime;
            this.label2.Location = new System.Drawing.Point(6, 309);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "destination";
            // 
            // chkcreatefilesifnotexists
            // 
            this.chkcreatefilesifnotexists.AutoSize = true;
            this.chkcreatefilesifnotexists.BackColor = System.Drawing.Color.Black;
            this.chkcreatefilesifnotexists.Checked = true;
            this.chkcreatefilesifnotexists.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkcreatefilesifnotexists.ForeColor = System.Drawing.Color.White;
            this.chkcreatefilesifnotexists.Location = new System.Drawing.Point(224, 238);
            this.chkcreatefilesifnotexists.Name = "chkcreatefilesifnotexists";
            this.chkcreatefilesifnotexists.Size = new System.Drawing.Size(127, 17);
            this.chkcreatefilesifnotexists.TabIndex = 12;
            this.chkcreatefilesifnotexists.Text = "create files if not exist";
            this.chkcreatefilesifnotexists.UseVisualStyleBackColor = false;
            // 
            // chkcreatefoldersifnotexist
            // 
            this.chkcreatefoldersifnotexist.AutoSize = true;
            this.chkcreatefoldersifnotexist.BackColor = System.Drawing.Color.Black;
            this.chkcreatefoldersifnotexist.Checked = true;
            this.chkcreatefoldersifnotexist.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkcreatefoldersifnotexist.ForeColor = System.Drawing.Color.White;
            this.chkcreatefoldersifnotexist.Location = new System.Drawing.Point(40, 238);
            this.chkcreatefoldersifnotexist.Name = "chkcreatefoldersifnotexist";
            this.chkcreatefoldersifnotexist.Size = new System.Drawing.Size(140, 17);
            this.chkcreatefoldersifnotexist.TabIndex = 13;
            this.chkcreatefoldersifnotexist.Text = "create folders if not exist";
            this.chkcreatefoldersifnotexist.UseVisualStyleBackColor = false;
            // 
            // btnreports
            // 
            this.btnreports.BackColor = System.Drawing.Color.Black;
            this.btnreports.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.btnreports.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkMagenta;
            this.btnreports.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Navy;
            this.btnreports.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnreports.ForeColor = System.Drawing.Color.Lime;
            this.btnreports.Location = new System.Drawing.Point(392, 238);
            this.btnreports.Name = "btnreports";
            this.btnreports.Size = new System.Drawing.Size(52, 23);
            this.btnreports.TabIndex = 13;
            this.btnreports.Text = "reports";
            this.btnreports.UseVisualStyleBackColor = false;
            this.btnreports.Click += new System.EventHandler(this.btnreports_Click);
            // 
            // create_playlist
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::nyaxplaylistapp_ui.Properties.Resources.powermage;
            this.ClientSize = new System.Drawing.Size(498, 445);
            this.Controls.Add(this.btnreports);
            this.Controls.Add(this.chkcreatefoldersifnotexist);
            this.Controls.Add(this.chkcreatefilesifnotexists);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtsource);
            this.Controls.Add(this.txtdestination);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.txtlog);
            this.Controls.Add(this.lblbuildversion);
            this.Controls.Add(this.cbomediatype);
            this.Controls.Add(this.lblmediatype);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "create_playlist";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "playlist manager";
            this.Load += new System.EventHandler(this.create_playlist_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblmediatype;
        private System.Windows.Forms.ComboBox cbomediatype;
        private System.Windows.Forms.Label lblbuildversion;
        private System.Windows.Forms.TextBox txtlog;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnexit;
        private System.Windows.Forms.Button btngenerate_playlist_json;
        private System.Windows.Forms.ToolStripStatusLabel lbldatetime;
        private System.Windows.Forms.NotifyIcon notifyIconntharene;
        private System.Windows.Forms.ToolStripStatusLabel lbltimelapsed;
        private System.Windows.Forms.Label lblprogresscounta;
        private System.Windows.Forms.TextBox txtdestination;
        private System.Windows.Forms.TextBox txtsource;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btngenerateforalltypes;
        private System.Windows.Forms.Label lbltotalfilesprocessed;
        private System.Windows.Forms.ToolStripStatusLabel lblprogresspercentage;
        private System.Windows.Forms.CheckBox chkcreatefilesifnotexists;
        private System.Windows.Forms.CheckBox chkcreatefoldersifnotexist;
        private System.Windows.Forms.Button btncancelworker;
        private System.Windows.Forms.Button btnreports;

    }
}

