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
        bool open_editor = true;
        bool to_close = false;

        public frm_preview(string[] d, bool preview, bool od)
        {
            InitializeComponent();

            open_editor = od;
            to_close = !preview;
            if (!preview)
            {
                Visible = false;

                string add = "";
                for (int i = 0; i < d.Length; i++)
                    add += d[i] + (i < d.Length - 1 ? "\t" : "");
                File.AppendAllText(path, add + Environment.NewLine);
            }
            arr = d;
        }

        private void frm_preview_Load(object sender, EventArgs e)
        {
            if (to_close)
            {
                Close();
                if (open_editor)
                    (new frm_editor(path)).Show();
                return;
            }
            Visible = true;
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
            if (open_editor)
                (new frm_editor(path)).Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
