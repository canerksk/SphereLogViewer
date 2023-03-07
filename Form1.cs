using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SphereLogViewer
{
    public partial class Form1 : Form
    {
        DataTable table;

        public static string baselogspath = Path.Combine(Application.StartupPath, "logs");
        public static int contentheight = 840;
        public static int saatheight = 55;

        public Form1()
        {
            InitializeComponent();

            table = new DataTable();
            table.Columns.Add("Saat", typeof(string));
            table.Columns.Add("Content", typeof(string));

        }

        private bool IsBlank(string inString)
        {
            if (!string.IsNullOrEmpty(inString)) inString = inString.Trim();
            return string.IsNullOrEmpty(inString);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(baselogspath))
            {
                MessageBox.Show("Logs klasörü bulunamadı!" + baselogspath);
                Close();
                return;
            }

            getAllLogFiles();
        }

        public void getLogContent(string filename)
        {
            string logfile = filename;

            if (!File.Exists(filename))
            {
                MessageBox.Show("Log dosyası bulunamadı!");
                return;
            }


            using (var streamReader = new StreamReader(filename))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    if (!IsBlank(line))
                    {
                        var values = line.Split('\t');
                        int len = 0;
                        string value = string.Empty;
                        string content = string.Empty;

                        for (int i = 0; i < values.Length; i++)
                        {
                            content = values[i];

                            len = content.Length;
                            if (content.Length > 0)
                                value = content.Substring(6);

                            string saat = content.Substring(0, 5);

                            if (!content.Contains("Indexing the client files...") && value != "WARNING:" && !content.Contains("contain errors, might be unstable or even destroy your shard as they are mostly untested!") && !content.Contains("Nightly builds are automatically made at every commit to the source code repository and might") && !content.Contains("DO NOT run this build on a live shard unless you know what you are doing!") && !content.Contains("-----------------------------------------------------------------") && !content.Contains("This is a nightly build of SphereServer.") && !string.IsNullOrEmpty(value))
                                table.Rows.Add(saat, value);
                        }
                    }

                }

            }

            dataGridView1.DataSource = table;
            dataGridView1.Columns[1].Width = contentheight;
            dataGridView1.Columns[0].Width = saatheight;

        }


        public void getAllLogFiles()
        {
            //checkedListBox1.Items.Clear();
            dataGridView2.Rows.Clear();

            DirectoryInfo d = new DirectoryInfo(baselogspath);
            FileInfo[] Files = d.GetFiles("*.log");
            foreach (FileInfo file in Files)
            {
                dataGridView2.Rows.Add(false, file.Name);
                //checkedListBox1.Items.Add(file.Name);
            }



        }

        private void buttonLogFilesRefresh_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            dataGridView1.Columns[1].Width = contentheight;
            dataGridView1.Columns[0].Width = saatheight;

            getAllLogFiles();
        }



        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            DataView dv = table.DefaultView;
            dv.RowFilter = "Content LIKE '%" + textBox1.Text + "%'";
            dataGridView1.DataSource = dv;

            dataGridView1.Columns[1].Width = contentheight;
            dataGridView1.Columns[0].Width = saatheight;
        }

        private void textBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (textBox1.Text == "Anahtar kelime giriniz.")
            {
                textBox1.Text = "";
            }
            dataGridView1.Columns[1].Width = contentheight;
            dataGridView1.Columns[0].Width = saatheight;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://arcaneuo.com");

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowindex = dataGridView2.CurrentCell.RowIndex;
            int columnindex = dataGridView2.CurrentCell.ColumnIndex;

            //if (dataGridView2.SelectedRows.Count <= 0)
            //{
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
                dataGridView1.Refresh();
            //}
            //else
            //{
            //    dataGridView1.Refresh();
            //    dataGridView1.DataSource = table;
            //    dataGridView1.Columns[1].Width = contentheight;
            //    dataGridView1.Columns[0].Width = saatheight;
            //}

            //if (e.ColumnIndex == Select.Index && e.RowIndex != -1)
            if (columnindex == 0)
            {
                textBox1.Text = "Anahtar kelime giriniz.";

                string filedir = "";
                for (int i = 0; i < dataGridView2.SelectedCells.Count; i++)
                {
                    filedir = Path.Combine(baselogspath, dataGridView2.Rows[rowindex].Cells[1].Value.ToString());
                    getLogContent(filedir);

                    //Console.WriteLine(filedir);
                }
                //MessageBox.Show(filedir);

            }
        }


    }
}
