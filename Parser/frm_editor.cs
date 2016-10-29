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
    public partial class frm_editor : Form
    {
        string path = "";
        List<string[]> data = new List<string[]>();

        public frm_editor(string p)
        {
            InitializeComponent();

            path = p;
        }

        private void frm_editor_Load(object sender, EventArgs e)
        {
            Text = path;
            using (StreamReader sr = new StreamReader(path))
            {
                string line = sr.ReadLine();
                data.Add(line.Split('\t'));
                int l = data[0].Length;
                if (l < 2)
                {
                    MessageBox.Show("File is not valid!");
                    Close();
                    return;
                }
                dataGridView1.ColumnCount = l;
                dataGridView1.Rows.Clear();
                dataGridView1.Rows.Add(data[0]);
                bool flag = false;
                while ((line = sr.ReadLine()) != null)
                {
                    data.Add(line.Split('\t'));
                    int f = 0;
                    foreach (string s in data[data.Count - 1])
                        if (s == "")
                            f++;
                    flag = data[data.Count - 1].Length != l || f == l ? true : flag;
                    dataGridView1.Rows.Add(data[data.Count - 1]);
                    //for (int i = 0; i < l; i++)
                }
                if (flag)
                    MessageBox.Show("Empty rows has been found while reading the file!");
            }
            //int csum = 0;
            //foreach (DataGridViewColumn c in dataGridView1.Columns)
            //    csum += c.Width;
            //int h = dataGridView1.GetRowDisplayRectangle(dataGridView1.NewRowIndex, true).Bottom + dataGridView1.GetRowDisplayRectangle(dataGridView1.NewRowIndex, false).Height - 23;
            //dataGridView1.Size = new Size(csum + dataGridView1.RowHeadersWidth + 2, Math.Min(h, ClientRectangle.Height));
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string res = "";
            for (int i = 0; i < dataGridView1.RowCount - 1; i++)
            {
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    res += dataGridView1.Rows[i].Cells[j].Value + (j < dataGridView1.ColumnCount - 1 ? "\t" : "");
                }
                res += Environment.NewLine;
            }
            using (StreamWriter sw = new StreamWriter(path))
                sw.Write(res);
            Close();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
