using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Parser
{
    public partial class frm_preview : Form
    {
        string[] arr;
        string path = "cd2anki.txt";

        public frm_preview(string[] d)
        {
            InitializeComponent();

            arr = d;
        }

        private void frm_preview_Load(object sender, EventArgs e)
        {
            if (arr.Length == 0)
            {
                MessageBox.Show("An empty data received!");
                Close();
                return;
            }
            dataGridView1.ColumnCount = arr.Length;
            dataGridView1.Rows.Clear();
            dataGridView1.Rows.Add(arr);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string add = "";
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
                add += dataGridView1.Rows[0].Cells[i].Value + (i < dataGridView1.ColumnCount - 1 ? "\t" : "");
            File.AppendAllText(path, add + Environment.NewLine);
            Close();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
