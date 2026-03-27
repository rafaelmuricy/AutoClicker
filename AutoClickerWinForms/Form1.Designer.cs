namespace AutoClicker;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        btCapture = new Button();
        btStartStop = new Button();
        label1 = new Label();
        panel1 = new Panel();
        lblClock = new Label();
        SuspendLayout();
        // 
        // btCapture
        // 
        btCapture.Dock = DockStyle.Top;
        btCapture.Location = new Point(0, 0);
        btCapture.Name = "btCapture";
        btCapture.Size = new Size(246, 35);
        btCapture.TabIndex = 2;
        btCapture.Text = "Capture click";
        btCapture.UseVisualStyleBackColor = true;
        btCapture.Click += btCapture_Click;
        // 
        // btStartStop
        // 
        btStartStop.Dock = DockStyle.Bottom;
        btStartStop.Location = new Point(0, 414);
        btStartStop.Name = "btStartStop";
        btStartStop.Size = new Size(246, 36);
        btStartStop.TabIndex = 4;
        btStartStop.Text = "Start All";
        btStartStop.UseVisualStyleBackColor = true;
        btStartStop.Click += btStartStop_Click;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(12, 38);
        label1.Name = "label1";
        label1.Size = new Size(41, 15);
        label1.TabIndex = 5;
        label1.Text = "Clicks:";
        // 
        // panel1
        // 
        panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        panel1.AutoScroll = true;
        panel1.Location = new Point(12, 56);
        panel1.Name = "panel1";
        panel1.Size = new Size(222, 325);
        panel1.TabIndex = 6;
        panel1.Visible = false;
        // 
        // lblClock
        // 
        lblClock.AutoSize = true;
        lblClock.Dock = DockStyle.Bottom;
        lblClock.Location = new Point(0, 399);
        lblClock.Name = "lblClock";
        lblClock.Size = new Size(49, 15);
        lblClock.TabIndex = 7;
        lblClock.Text = "00:00:00";
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(246, 450);
        Controls.Add(label1);
        Controls.Add(lblClock);
        Controls.Add(btStartStop);
        Controls.Add(panel1);
        Controls.Add(btCapture);
        Name = "Form1";
        Text = "AutoClicker";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
    private Button btCapture;
    private Label lblClock;
    private Button btStartStop;
    private Label label1;
    private Panel panel1;
}
