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

                    HtmlNodeCollection senseblocks = group.SelectNodes(".//div[@class='sense-block']");
                    int last_block_y = 3;
                    foreach(HtmlNode senseblock in senseblocks)
                    {
                        HtmlNodeCollection difdef = senseblock.SelectNodes(".//h4[@class='txt-block txt-block--alt2']");
                        if (difdef != null && difdef.Count > 0)
                        {
                            Label sense_info = new Label();
                            sense_info.AutoSize = true;
                            sense_info.Location = new Point(6, last_block_y);
                            sense_info.Name = "lbl_senseinfo_" + tp.Text + "_" + tabControl1.TabPages.Count;
                            //sense_info.Size = new System.Drawing.Size(79, 29);
                            //sense_info.TabIndex = 4;
                            sense_info.Text = difdef[0].SelectNodes(".//span[@class='guideword']")[0].ChildNodes[1].InnerText;
                            //sense_info.BackColor = Color.AliceBlue;
                            tp.Controls.Add(sense_info);

                            last_block_y = sense_info.Location.Y + sense_info.Height;
                        }
                        
                        HtmlNodeCollection defblocks = senseblock.SelectNodes(".//div[@class='def-block pad-indent']");
                        if (defblocks == null)
                        {
                            process_err("Error while parsing");
                            return;
                        }

                        ListView lv_def = new ListView();
                        lv_def.Columns.Add(new ColumnHeader() );
                        lv_def.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        lv_def.FullRowSelect = true;
                        lv_def.HeaderStyle = ColumnHeaderStyle.Clickable;
                        lv_def.Location = new Point(0, last_block_y);
                        lv_def.Name = "lv_" + tp.Text + "_" + tp.Controls.Count;
                        lv_def.Size = new Size(396, 479);
                        //lv_def.TabIndex = 3;
                        //lv_def.UseCompatibleStateImageBehavior = false;
                        lv_def.View = View.Details;

                        foreach (HtmlNode def in defblocks)
                        {
                            lv_def.Items.Add(def.SelectNodes(".//b[@class='def']")[0].InnerText);
                        }
                        tp.Controls.Add(lv_def);
                        last_block_y = lv_def.Location.Y + lv_def.Height;
                    }
                    //if (!defs.Contains(link.InnerText.Replace(":", "")))
                    //    defs.Add(link.InnerText.Replace(":", ""));
                }

                //foreach (string elt in defs)
                //    Console.WriteLine(elt);

                listBox1.Items.Clear();
                listBox1.Items.AddRange(defs.ToArray());
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
