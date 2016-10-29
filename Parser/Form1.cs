using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace Parser
{
    public partial class Form1 : Form
    {
        enum dic_type
        {
            British = 0,
            American = 1
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            search(textBox1.Text);
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
                search(textBox1.Text);
        }

        private void search(string q, dic_type dtype = dic_type.British)
        {
            // Dictionary choosing stuff
            string dselector = ".//";
            switch (dtype)
            {
                case dic_type.British:
                    dselector = ".//div[@data-tab='ds-british']";
                    break;
                case dic_type.American:
                    dselector = ".//div[@data-tab='ds-american-english']";
                    break;
            }
            try
            {
                // Receiving HTML-page stuff
                System.Net.WebClient web = new System.Net.WebClient();
                web.Encoding = UTF8Encoding.UTF8;
                string html = web.DownloadString("http://dictionary.cambridge.org/search/english/direct/?q=" + System.Web.HttpUtility.UrlEncode(q));
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);


                // Parsing
                List<string> defs = new List<string>();
                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(dselector)[0].SelectNodes(".//div[@class='entry-body__el clrd js-share-holder']");
                if (nodes == null)
                {
                    process_err("Error while parsing");
                    return;
                }

                tabControl1.TabPages.Clear();
                foreach (HtmlNode group in nodes)
                {
                    TabPage tp = new TabPage();
                    //tp.Location = new System.Drawing.Point(4, 38);
                    tp.Name = "tbpg_"+tabControl1.TabPages.Count;
                    //tp.Padding = new System.Windows.Forms.Padding(3);
                    //tp.Size = new System.Drawing.Size(809, 469);
                    //tp.TabIndex = 0;
                    tp.Text = group.SelectNodes(".//span[@class='headword']")[0].InnerText + " (" + group.SelectNodes(".//span[@class='posgram ico-bg']")[0].InnerText + ")";
                    tp.UseVisualStyleBackColor = true;
                    tabControl1.TabPages.Add(tp);

                    Panel pan = new Panel();
                    pan.Dock = DockStyle.Fill;
                    //pan.Location = new Point(3, 3);
                    pan.Name = "panel_" + tabControl1.TabCount;
                    //pan.Size = new Size(803, 463);
                    //pan.TabIndex = 0;
                    pan.AutoScroll = true;
                    tp.Controls.Add(pan);

                    HtmlNodeCollection senseblocks = group.SelectNodes(".//div[@class='sense-block']");
                    int last_block_y = 3;
                    foreach(HtmlNode senseblock in senseblocks)
                    {
                        HtmlNodeCollection difdef = senseblock.SelectNodes(".//h4[@class='txt-block txt-block--alt2']");
                        if (difdef != null && difdef.Count > 0)
                        {
                            Label lbl_sense = new Label();
                            lbl_sense.AutoSize = true;
                            lbl_sense.Location = new Point(0, last_block_y);
                            lbl_sense.Name = "lbl_senseinfo_" + tp.Text + "_" + tabControl1.TabPages.Count;
                            //lbl_sense.AutoSize = false;
                            lbl_sense.Text = difdef[0].SelectNodes(".//span[@class='guideword']")[0].ChildNodes[1].InnerText;
                            //lbl_sense.Width = pan.Width - SystemInformation.VerticalScrollBarWidth - 2;
                            //sense_info.TabIndex = 4;
                            lbl_sense.BackColor = Color.DarkBlue;
                            lbl_sense.ForeColor = Color.White;
                            pan.Controls.Add(lbl_sense);

                            last_block_y = lbl_sense.Location.Y + lbl_sense.Height + 5;
                        }
                        
                        HtmlNodeCollection defblocks = senseblock.SelectNodes(".//div[@class='def-block pad-indent']");
                        if (defblocks == null)
                        {
                            process_err("Error while parsing");
                            return;
                        }

                        ListView lv_def = new ListView();
                        lv_def.View = View.Details;
                        lv_def.Columns.Add(new ColumnHeader() );
                        lv_def.FullRowSelect = true;
                        lv_def.HeaderStyle = ColumnHeaderStyle.None;
                        //lv_def.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        //lv_def.Columns[0].Width = tp.Width;
                        //lv_def.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                        lv_def.Location = new Point(0, last_block_y);
                        lv_def.Name = "lv_" + tp.Text + "_" + tp.Controls.Count;
                        //lv_def.TabIndex = 3;
                        //lv_def.UseCompatibleStateImageBehavior = false;

                        foreach (HtmlNode def in defblocks)
                        {
                            ListViewItem lvi = new ListViewItem(new string[] { def.SelectNodes(".//b[@class='def']")[0].InnerText });
                            lv_def.Items.Add(lvi);
                            //lv_def.Items.Add(def.SelectNodes(".//b[@class='def']")[0].InnerText);
                        }
                        lv_def.Size = new Size(pan.Width - SystemInformation.VerticalScrollBarWidth - 2, 30 + lv_def.Items.Count * 32);
                        lv_def.Columns[0].Width = -2;
                        //lv_def.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                        pan.Controls.Add(lv_def);
                        last_block_y = lv_def.Location.Y + lv_def.Height + 10;
                    }
                    //foreach (Control lv in pan.Controls)
                    //    if (lv.GetType().Name == "ListView")
                    //        lv.Width = pan.Width - SystemInformation.VerticalScrollBarWidth - 2;
                }
            }
            catch (Exception ex)
            {
                process_err("This word was not found!");
            }
        }
        private void process_err(string msg = null)
        {
            MessageBox.Show("Произошла ошибка" + (msg != null ? ": \""+msg+"\"!" : "!"));
        }
    }
}
