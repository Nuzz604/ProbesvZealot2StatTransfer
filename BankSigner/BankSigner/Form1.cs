namespace BankSigner
{
    using BankSigner.Properties;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class Form1 : Form
    {
        private Button buttonOpen;
        private Button buttonSave;
        private Button buttonSign;
        private IContainer components;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private TextBox textBoxAA;
        private TextBox textBoxBD;
        private TextBox textBoxBN;
        private TextBox textBoxUA;
        private ToolTip toolTip1;

        public Form1()
        {
            this.InitializeComponent();
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog {
                Title = "Open Bank",
                InitialDirectory = Program.FilePath.Remove(Program.FilePath.LastIndexOf('\\') + 1),
                Filter = "StarCraft II Banks (*.SC2Bank)|*.SC2Bank|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false,
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false,
                ValidateNames = true
            };
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                Program.FilePath = dialog.FileName;
                Program.OpenFile(Program.FilePath);
                this.textBoxAA.Text = Settings.Default.AAText = Program.AANumber.Number;
                this.textBoxUA.Text = Settings.Default.UAText = Program.UANumber.Number;
                Program.BankName = Program.BankName.Trim();
                if (Program.BankName.EndsWith(".SC2Bank", StringComparison.OrdinalIgnoreCase))
                {
                    Program.BankName = Program.BankName.Remove(Program.BankName.LastIndexOf(".SC2Bank", StringComparison.OrdinalIgnoreCase));
                }
                this.textBoxBN.Text = Settings.Default.BNText = Program.BankName;
                this.textBoxBD.Text = Settings.Default.BDText = Program.BankData.Replace("\r ", "\r\n ").Replace("\r<", "\r\n<");
                Settings.Default.FilePath = Program.FilePath;
                Settings.Default.Save();
            }
            else if (result != DialogResult.Cancel)
            {
                MessageBox.Show("There was an error opening that bank.", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog {
                Title = "Open Bank",
                InitialDirectory = Program.FilePath.Remove(Program.FilePath.LastIndexOf('\\') + 1),
                Filter = "StarCraft II Banks (*.SC2Bank)|*.SC2Bank|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false,
                AddExtension = true,
                DefaultExt = "SC2Bank",
                OverwritePrompt = true,
                ValidateNames = true
            };
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                Settings.Default.BDText = Program.BankData = this.textBoxBD.Text.Replace("\r ", "\r\n ").Replace("\r<", "\r\n<");
                Program.SaveFile(dialog.FileName);
                Settings.Default.FilePath = Program.FilePath = dialog.FileName;
                Settings.Default.Save();
            }
            else if (result != DialogResult.Cancel)
            {
                MessageBox.Show("There was an error saving that bank.", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void buttonSign_Click(object sender, EventArgs e)
        {
            bool flag = false;
            Program.AANumber.Number = Settings.Default.AAText = this.textBoxAA.Text.Trim();
            Program.UANumber.Number = Settings.Default.UAText = this.textBoxUA.Text.Trim();
            this.textBoxBN.Text = this.textBoxBN.Text.Trim();
            if (this.textBoxBN.Text.EndsWith(".SC2Bank", StringComparison.OrdinalIgnoreCase))
            {
                this.textBoxBN.Text = this.textBoxBN.Text.Remove(this.textBoxBN.Text.LastIndexOf(".SC2Bank", StringComparison.OrdinalIgnoreCase));
            }
            Program.BankName = Settings.Default.BNText = this.textBoxBN.Text.Trim();
            Program.BankData = Settings.Default.BDText = this.textBoxBD.Text.Replace("\r ", "\r\n ").Replace("\r<", "\r\n<");
            if (Program.AANumber.Number.Length == 0)
            {
                MessageBox.Show("Author account number cannot be blank", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                flag = true;
            }
            if (Program.UANumber.Number.Length == 0)
            {
                MessageBox.Show("User account number cannot be blank", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                flag = true;
            }
            if (Program.BankName.Length == 0)
            {
                MessageBox.Show("Bank name cannot be blank", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                flag = true;
            }
            if (Program.BankData.Trim().Length == 0)
            {
                MessageBox.Show("Bank contents cannot be blank", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                flag = true;
            }
            if (!flag)
            {
                this.textBoxBD.Text = Settings.Default.BDText = Program.BankData = Program.Sign(Program.AANumber.Number, Program.UANumber.Number, Program.BankName, Program.BankData);
            }
            Settings.Default.Save();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            Array data = (Array) e.Data.GetData(DataFormats.FileDrop);
            if (data != null)
            {
                string fileName = data.GetValue(0).ToString();
                Program.OpenFile(fileName);
                this.textBoxAA.Text = Settings.Default.AAText = Program.AANumber.Number;
                this.textBoxUA.Text = Settings.Default.UAText = Program.UANumber.Number;
                Program.BankName = Program.BankName.Trim();
                if (Program.BankName.EndsWith(".SC2Bank", StringComparison.OrdinalIgnoreCase))
                {
                    Program.BankName = Program.BankName.Remove(Program.BankName.LastIndexOf(".SC2Bank", StringComparison.OrdinalIgnoreCase));
                }
                this.textBoxBN.Text = Settings.Default.BNText = Program.BankName;
                this.textBoxBD.Text = Settings.Default.BDText = Program.BankData.Replace("\r ", "\r\n ").Replace("\r<", "\r\n<");
                Settings.Default.FilePath = Program.FilePath = fileName;
                Settings.Default.Save();
                base.Activate();
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.WindowSize = base.ClientSize;
            Settings.Default.WindowPos = base.DesktopLocation;
            Settings.Default.Save();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            base.ClientSize = Settings.Default.WindowSize;
            base.DesktopLocation = Settings.Default.WindowPos;
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            ComponentResourceManager manager = new ComponentResourceManager(typeof(Form1));
            this.label1 = new Label();
            this.label2 = new Label();
            this.label3 = new Label();
            this.label4 = new Label();
            this.buttonSign = new Button();
            this.buttonOpen = new Button();
            this.buttonSave = new Button();
            this.toolTip1 = new ToolTip(this.components);
            this.textBoxBN = new TextBox();
            this.textBoxUA = new TextBox();
            this.textBoxAA = new TextBox();
            this.textBoxBD = new TextBox();
            base.SuspendLayout();
            manager.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            manager.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            manager.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            manager.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            manager.ApplyResources(this.buttonSign, "buttonSign");
            this.buttonSign.Name = "buttonSign";
            this.toolTip1.SetToolTip(this.buttonSign, manager.GetString("buttonSign.ToolTip"));
            this.buttonSign.UseVisualStyleBackColor = true;
            this.buttonSign.Click += new EventHandler(this.buttonSign_Click);
            manager.ApplyResources(this.buttonOpen, "buttonOpen");
            this.buttonOpen.Name = "buttonOpen";
            this.toolTip1.SetToolTip(this.buttonOpen, manager.GetString("buttonOpen.ToolTip"));
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new EventHandler(this.buttonOpen_Click);
            manager.ApplyResources(this.buttonSave, "buttonSave");
            this.buttonSave.Name = "buttonSave";
            this.toolTip1.SetToolTip(this.buttonSave, manager.GetString("buttonSave.ToolTip"));
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new EventHandler(this.buttonSave_Click);
            this.toolTip1.AutoPopDelay = 0x2710;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ReshowDelay = 100;
            manager.ApplyResources(this.textBoxBN, "textBoxBN");
            this.textBoxBN.DataBindings.Add(new Binding("Text", Settings.Default, "BNText", true, DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxBN.Name = "textBoxBN";
            this.textBoxBN.Text = Settings.Default.BNText;
            this.toolTip1.SetToolTip(this.textBoxBN, manager.GetString("textBoxBN.ToolTip"));
            manager.ApplyResources(this.textBoxUA, "textBoxUA");
            this.textBoxUA.DataBindings.Add(new Binding("Text", Settings.Default, "UAText", true, DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxUA.Name = "textBoxUA";
            this.textBoxUA.Text = Settings.Default.UAText;
            this.toolTip1.SetToolTip(this.textBoxUA, manager.GetString("textBoxUA.ToolTip"));
            manager.ApplyResources(this.textBoxAA, "textBoxAA");
            this.textBoxAA.DataBindings.Add(new Binding("Text", Settings.Default, "AAText", true, DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxAA.Name = "textBoxAA";
            this.textBoxAA.Text = Settings.Default.AAText;
            this.toolTip1.SetToolTip(this.textBoxAA, manager.GetString("textBoxAA.ToolTip"));
            this.textBoxBD.AcceptsReturn = true;
            manager.ApplyResources(this.textBoxBD, "textBoxBD");
            this.textBoxBD.DataBindings.Add(new Binding("Text", Settings.Default, "BDText", true, DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxBD.Name = "textBoxBD";
            this.textBoxBD.Text = Settings.Default.BDText;
            this.AllowDrop = true;
            manager.ApplyResources(this, "$this");
            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = SystemColors.Control;
            base.Controls.Add(this.buttonSave);
            base.Controls.Add(this.buttonOpen);
            base.Controls.Add(this.buttonSign);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.label4);
            base.Controls.Add(this.label3);
            base.Controls.Add(this.label2);
            base.Controls.Add(this.textBoxBD);
            base.Controls.Add(this.textBoxBN);
            base.Controls.Add(this.textBoxUA);
            base.Controls.Add(this.textBoxAA);
            this.Cursor = Cursors.Default;
            this.DoubleBuffered = true;
            this.ForeColor = SystemColors.ControlText;
            base.Name = "Form1";
            base.SizeGripStyle = SizeGripStyle.Show;
            base.FormClosing += new FormClosingEventHandler(this.Form1_FormClosing);
            base.Load += new EventHandler(this.Form1_Load);
            base.DragDrop += new DragEventHandler(this.Form1_DragDrop);
            base.DragEnter += new DragEventHandler(this.Form1_DragEnter);
            base.ResumeLayout(false);
            base.PerformLayout();
        }
    }
}

