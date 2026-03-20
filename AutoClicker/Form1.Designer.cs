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
        button2 = new Button();
        label2 = new Label();
        btIniciar = new Button();
        label1 = new Label();
        panel1 = new Panel();
        textBox1 = new TextBox();
        label3 = new Label();
        label4 = new Label();
        panel1.SuspendLayout();
        SuspendLayout();
        // 
        // button2
        // 
        button2.Location = new Point(12, 12);
        button2.Name = "button2";
        button2.Size = new Size(152, 23);
        button2.TabIndex = 2;
        button2.Text = "Capturar local do click";
        button2.UseVisualStyleBackColor = true;
        button2.Click += button2_Click;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(12, 38);
        label2.Name = "label2";
        label2.Size = new Size(118, 15);
        label2.TabIndex = 3;
        label2.Text = "Aguardando Captura";
        // 
        // btIniciar
        // 
        btIniciar.Location = new Point(43, 61);
        btIniciar.Name = "btIniciar";
        btIniciar.Size = new Size(75, 23);
        btIniciar.TabIndex = 4;
        btIniciar.Text = "Iniciar";
        btIniciar.UseVisualStyleBackColor = true;
        btIniciar.Click += button1_Click;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(3, 0);
        label1.Name = "label1";
        label1.Size = new Size(164, 15);
        label1.TabIndex = 5;
        label1.Text = "Delay entre clickes (segundo):";
        // 
        // panel1
        // 
        panel1.Controls.Add(textBox1);
        panel1.Controls.Add(label1);
        panel1.Controls.Add(btIniciar);
        panel1.Location = new Point(12, 81);
        panel1.Name = "panel1";
        panel1.Size = new Size(175, 105);
        panel1.TabIndex = 6;
        panel1.Visible = false;
        // 
        // textBox1
        // 
        textBox1.Location = new Point(3, 18);
        textBox1.Name = "textBox1";
        textBox1.Size = new Size(100, 23);
        textBox1.TabIndex = 6;
        textBox1.Text = "2";
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Location = new Point(12, 426);
        label3.Name = "label3";
        label3.Size = new Size(38, 15);
        label3.TabIndex = 7;
        label3.Text = "label3";
        // 
        // label4
        // 
        label4.AutoSize = true;
        label4.Location = new Point(570, 426);
        label4.Name = "label4";
        label4.Size = new Size(38, 15);
        label4.TabIndex = 8;
        label4.Text = "label4";
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 450);
        Controls.Add(label4);
        Controls.Add(label3);
        Controls.Add(panel1);
        Controls.Add(label2);
        Controls.Add(button2);
        Name = "Form1";
        Text = "Form1";
        panel1.ResumeLayout(false);
        panel1.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
    private Button button2;
    private Label label2;
    private Button btIniciar;
    private Label label1;
    private Panel panel1;
    private TextBox textBox1;
    private Label label3;
    private Label label4;
}
