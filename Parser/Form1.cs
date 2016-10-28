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
            label1.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            search(textBox1.Text, dic_type.British);
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
                search(textBox1.Text, dic_type.British);
        }

        private void search(string q, dic_type dtype)
        {
            string dselector = ".//div[@data-tab='ds-british']";
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
                System.Net.WebClient web = new System.Net.WebClient();
                web.Encoding = UTF8Encoding.UTF8;
                string html = web.DownloadString("http://dictionary.cambridge.org/search/english/direct/?q=" + System.Web.HttpUtility.UrlEncode(q));
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);


                List<string> defs = new List<string>();
                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(dselector)[0].SelectNodes(".//div[@class='entry-body__el clrd js-share-holder']");
                if (nodes == null)
                    process_err("Error while parsing");

                foreach (HtmlNode group in nodes)
                {
                    label1.Text = "Showing word: " + group.SelectNodes(".//span[@class='headword']")[0].InnerText;
                    defs.Add("<hw>: " + group.SelectNodes(".//span[@class='headword']")[0].InnerText + "(" + group.SelectNodes(".//span[@class='posgram ico-bg']")[0].InnerText + ")");
                    HtmlNodeCollection senseblocks = group.SelectNodes(".//div[@class='sense-block']");
                    foreach(HtmlNode senseblock in senseblocks)
                    {
                        HtmlNodeCollection difdef = senseblock.SelectNodes(".//h4[@class='txt-block txt-block--alt2']");
                        if (difdef != null && difdef.Count > 0)
                            defs.Add(">>" + difdef[0].InnerText);
                        HtmlNodeCollection defblock = senseblock.SelectNodes(".//div[@class='def-block pad-indent']");
                        foreach(HtmlNode def in defblock)
                        {
                            defs.Add(def.SelectNodes(".//b[@class='def']")[0].InnerText);
                        }
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
