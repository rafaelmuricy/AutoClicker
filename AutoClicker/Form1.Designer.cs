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
        label3 = new Label();
        SuspendLayout();
        // 
        // btCapture
        // 
        btCapture.Location = new Point(12, 12);
        btCapture.Name = "btCapture";
        btCapture.Size = new Size(152, 23);
        btCapture.TabIndex = 2;
        btCapture.Text = "Capture click";
        btCapture.UseVisualStyleBackColor = true;
        btCapture.Click += btCapture_Click;
        // 
        // btIniciar
        // 
        btStartStop.Location = new Point(38, 387);
        btStartStop.Name = "btIniciar";
        btStartStop.Size = new Size(149, 36);
        btStartStop.TabIndex = 4;
        btStartStop.Text = "Start";
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
        panel1.AutoScroll = true;
        panel1.Location = new Point(12, 56);
        panel1.Name = "panel1";
        panel1.Size = new Size(211, 325);
        panel1.TabIndex = 6;
        panel1.Visible = false;
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Location = new Point(12, 426);
        label3.Name = "label3";
        label3.Size = new Size(49, 15);
        label3.TabIndex = 7;
        label3.Text = "00:00:00";
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(246, 450);
        Controls.Add(label1);
        Controls.Add(label3);
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
    private Button btStartStop;
    private Label label1;
    private Panel panel1;
    private Label label3;
}
