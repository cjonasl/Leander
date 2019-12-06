using System;
using System.IO;
using System.Windows.Forms;

namespace Adhoc
{
    public partial class Form1 : Form
    {
        public Form1()
        {       
            InitializeComponent();
            this.textBox1.Text = "C:\\Adhoc\\AdhocFile.html";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string fileNameFullPath = this.textBox1.Text.Trim();
                DateTime dt;

                if (string.IsNullOrEmpty(fileNameFullPath))
                {
                    MessageBox.Show("Please give an adhoc html file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (!File.Exists(fileNameFullPath))
                {
                    MessageBox.Show("The given adhoc html file does not exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    File.WriteAllText(fileNameFullPath, this.textBox2.Text.Trim());

                    dt = DateTime.Now;
                    fileNameFullPath = string.Format("{0}File{1}.html", "C:\\git_cjonasl\\Leander\\Adhoc\\", dt.ToString("yyyyMMddHHmmss"));
                    File.WriteAllText(fileNameFullPath, this.textBox2.Text.Trim());

                    this.textBox2.Clear();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("An exception happened: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
