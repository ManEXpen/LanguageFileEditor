using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using System.Text.RegularExpressions;

namespace LanguageFileEditor {
    public partial class Form1 : MetroForm {
        
        public Form1() {
            InitializeComponent();

            this.selectLangType.Items.Add("item");
            this.selectLangType.Items.Add("chat");
            this.selectLangType.Items.Add("tile");
            this.selectLangType.Items.Add("gui");

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) {
            if (e.ColumnIndex == this.dataGridView1.Columns["DelBtn"].Index) {
                try { this.dataGridView1.Rows.RemoveAt(e.RowIndex); } catch { }
            }
        }

        private void metroButton1_Click(object sender, EventArgs e) {
            if (this.metroTextBox1.Text != "") this.selectLangType.Items.Add(metroTextBox1.Text);

        }

        private void button1_Click(object sender, EventArgs e) {
            StringBuilder sb = new StringBuilder();
            SaveFileDialog sfd = new SaveFileDialog();
            try {
                if (metroComboBox1.SelectedItem == null) {
                    MessageBox.Show("言語を選択してください");
                    return;
                }

                sfd.FileName = metroComboBox1.SelectedItem.ToString();
                sfd.Filter = "言語ファイル|*.lang";

                if (sfd.ShowDialog() == DialogResult.OK) { 
                    using (var sw = new StreamWriter(sfd.FileName, false, System.Text.Encoding.GetEncoding("UTF-8"))) {

                        for (int i = 0; i < dataGridView1.RowCount - 1; i++) {
                            var tmp = dataGridView1.Rows[i];
                            sb.Append(tmp.Cells[1].Value.ToString()).Append(".");
                            sb.Append(tmp.Cells[2].Value.ToString());
                            if (tmp.Cells[1].Value.ToString() == "tile" || tmp.Cells[1].Value.ToString() == "item") sb.Append(".name");
                            sb.Append("=");
                            sb.Append(tmp.Cells[3].Value.ToString());
                            sb.Append("\n");
                        }
                        sw.Write(sb.ToString().TrimEnd('\r', '\n'));
                    }
                MessageBox.Show("保存しました");
            }
            } catch (SystemException ex) {
                System.Console.WriteLine(ex.Message);
            }

        }

        private void button2_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "言語ファイル|*.lang";

            if(ofd.ShowDialog() == DialogResult.OK) {
                using(var sr = new StreamReader(new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read), Encoding.GetEncoding("UTF-8"))) {
                    var str = sr.ReadToEnd();
                    str = Regex.Replace(str, "^\n", "");
                    str = str.TrimEnd('\r', '\n');

                    String[] lines = str.Split('\n');

                    this.dataGridView1.Rows.Clear();
                    foreach(var tmp in lines) {
                        if (tmp == "\n") continue;
                        String[] list = tmp.Split('.');
                        switch (list[0]) {
                            case "item":
                                this.dataGridView1.Rows.Add(new String[] {"", "item", GetUnTranslatedText(tmp, "item"), GetTranslatedText(tmp)});
                                break;
                            case "tile":
                                this.dataGridView1.Rows.Add(new String[] { "", "tile", GetUnTranslatedText(tmp, "tile"), GetTranslatedText(tmp) });
                                break;
                            case "chat":
                                this.dataGridView1.Rows.Add(new String[] { "", "chat", GetUnTranslatedText(tmp, "chat"), GetTranslatedText(tmp) });
                                break;
                            case "gui":
                                this.dataGridView1.Rows.Add(new String[] { "", "gui", GetUnTranslatedText(tmp, "gui"), GetTranslatedText(tmp) });
                                break;
                            default:
                                int idx = this.selectLangType.Items.IndexOf(list[0]);
                                if (idx == -1) this.selectLangType.Items.Add(list[0]);
                                this.dataGridView1.Rows.Add(new String[] { "", list[0], GetUnTranslatedText(tmp, list[0]), GetTranslatedText(tmp) });
                                break;
                        }
                    }
                }
            }
        }

        private static string GetUnTranslatedText (string str, string type){
            var a = str.Replace(type + ".", "");
            Console.WriteLine(str);
            a = a.Substring(0, a.IndexOf('='));
            if (type == "tile" || type == "item") a = a.Replace(".name", "");
            return a;
        }
        private static string GetTranslatedText(string str) {
            var a = str.Substring(str.IndexOf('=')+1).Replace('=', '\0');
            return a;
        }

    }
}
