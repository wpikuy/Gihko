using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Gihko.Bubble;
using Gihko.QuickFunc;
using MahApps.Metro.Controls;
using Button = System.Windows.Controls.Button;
using ComboBox = System.Windows.Controls.ComboBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using TextBox = System.Windows.Controls.TextBox;

namespace Gihko.Setting {
    /// <summary>
    /// Settings.xaml 的交互逻辑
    /// </summary>
    public partial class Settings : MetroWindow {
        public Settings() {
            InitializeComponent();
            Closing += (sender, args) => writeProps();
            _trialStrings = new[]{
                "碰碰Gihko就可以打开快捷菜单哦~ o>w<o",
                "如果找不到我了，在任务栏右边双击Gihko的标志，Gihko就会出现了哦~ o>w<o",
                "右键点Gihko，可以设置Gihko的功能哦~ o>w<o",
                "使用时出现黑色窗口，是因为这个功能的地址填写错误，Gihko真的很对不起~ o>w<o",
                "左键可以拖拽Gihko哦~ o>w<o"
            };
        }

        public void setCore(Core.Core core){
            _core = core;
        }

        private void qf_choose_click(object sender, RoutedEventArgs re){
            string index = (sender as Button).Name[4].ToString();
            string tbName = "qf_tb" + index;
            TextBox tb = FindName(tbName) as TextBox;
            string cbName = "qf_cb" + index;
            ComboBox cb = FindName(cbName) as ComboBox;
            if (cb.SelectedIndex == 0){
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dialog.Filter = "所有文件(*.*)|*.*";
                if (dialog.ShowDialog().GetValueOrDefault()){
                    tb.Text = dialog.FileName;
                }
            }
            else if (cb.SelectedIndex == 1) {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.RootFolder = Environment.SpecialFolder.MyDocuments;
                dialog.Description = "选择一个文件夹";
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    tb.Text = dialog.SelectedPath;
                }
            }
            else if (cb.SelectedIndex == 2){
                System.Windows.MessageBox.Show("请直接粘贴网址在地址栏。");
            }
        }

        private void qf_combobox_change(object sender, SelectionChangedEventArgs sce) {
            string index = (sender as ComboBox).Name[5].ToString();
            string tbName = "qf_tb" + index;
            TextBox tb = FindName(tbName) as TextBox;
            tb.Text = string.Empty;
        }

        public void loadProps(){
            
            // check
            Hashtable table = _core.Props;
            if (!table.Contains("QuickFunc.Funcs")) table.Add("QuickFunc.Funcs", new List<QuickFuncElem>(new[] { new QuickFuncElem(), new QuickFuncElem(), new QuickFuncElem(), new QuickFuncElem(), new QuickFuncElem(), new QuickFuncElem() }));
            if (!table.Contains("Bubble.EnableTrial")) table.Add("Bubble.EnableTrial", true);
            if (!table.Contains("Bubble.EnableTC")) table.Add("Bubble.EnableTC", true);
            if (!table.Contains("Bubble.IntervalMinutes")) table.Add("Bubble.IntervalMinutes", 5.0);
            if (!table.Contains("Bubble.Trial")) table.Add("Bubble.Trial", null);
            table["Bubble.Trial"] = new List<string>(_trialStrings);
            if (!table.Contains("Bubble.TC")) table.Add("Bubble.TC", new List<string>(new[] { "喵帕斯~！", "拳打南山敬老院", "连一百块都不给我", "JOJO我不做人了！"}));
            if (!table.Contains("Reminder.Reminder")) table.Add("Reminder.Reminder", new List<Reminder>(new[] { new Reminder(), new Reminder(), new Reminder(), new Reminder(), new Reminder(), new Reminder()}));

            // setter
            for (int i = 1; i < 7; i++){
                string index = i.ToString();

                QuickFuncElem qfe = (table["QuickFunc.Funcs"] as List<QuickFuncElem>)[i - 1];

                (FindName("qf_cb" + index) as ComboBox).SelectedIndex = qfe.Option;
                (FindName("qf_ntb" + index) as TextBox).Text = qfe.Description;
                (FindName("qf_tb" + index) as TextBox).Text = qfe.Cmd;

                Reminder re = (table["Reminder.Reminder"] as List<Reminder>)[i - 1];
                (FindName("rm_en" + index) as System.Windows.Controls.CheckBox).IsChecked = re.enable;
                (FindName("rm_dp" + index) as DatePicker).SelectedDate = re.time;
                (FindName("rm_h" + index) as MahApps.Metro.Controls.NumericUpDown).Value = re.time.Hour;
                (FindName("rm_m" + index) as MahApps.Metro.Controls.NumericUpDown).Value = re.time.Minute;
                (FindName("rm_tb" + index) as TextBox).Text = re.description;
            }

            (FindName("b_trial") as System.Windows.Controls.CheckBox).IsChecked = (bool)table["Bubble.EnableTrial"];
            (FindName("b_tc") as System.Windows.Controls.CheckBox).IsChecked = (bool)table["Bubble.EnableTC"];
            (FindName("b_interval") as MahApps.Metro.Controls.NumericUpDown).Value = (double)table["Bubble.IntervalMinutes"];

            string tcText = "";
            foreach (string s in (List<string>)table["Bubble.TC"]){
                tcText += "\n" + s;
            }
            tcText = tcText.Substring(1, tcText.Length - 1);
            (FindName("b_tcs") as System.Windows.Controls.TextBox).Text = tcText;

        }

        private void writeProps(){
            if (_core == null) return;
            Hashtable table = _core.Props;
            (table["QuickFunc.Funcs"] as List<QuickFuncElem>).Clear();
            (table["Reminder.Reminder"] as List<Reminder>).Clear();
            for (int i = 1; i < 7; i++) {
                string index = i.ToString();

                QuickFuncElem qfe = new QuickFuncElem();
                qfe.Option = (FindName("qf_cb" + index) as ComboBox).SelectedIndex;
                qfe.Description = (FindName("qf_ntb" + index) as TextBox).Text;
                qfe.Cmd = (FindName("qf_tb" + index) as TextBox).Text;
                (table["QuickFunc.Funcs"] as List<QuickFuncElem>).Add(qfe);

                Reminder re = new Reminder();
                re.enable = (FindName("rm_en" + index) as System.Windows.Controls.CheckBox).IsChecked.GetValueOrDefault();
                re.time = (FindName("rm_dp" + index) as DatePicker).SelectedDate.GetValueOrDefault();
                re.time = re.time.AddHours((FindName("rm_h" + index) as MahApps.Metro.Controls.NumericUpDown).Value.GetValueOrDefault());
                re.time = re.time.AddMinutes((FindName("rm_m" + index) as MahApps.Metro.Controls.NumericUpDown).Value.GetValueOrDefault());
                re.description = (FindName("rm_tb" + index) as TextBox).Text;
                (table["Reminder.Reminder"] as List<Reminder>).Add(re);
            }

            table["Bubble.EnableTrial"] = (FindName("b_trial") as System.Windows.Controls.CheckBox).IsChecked.GetValueOrDefault();
            table["Bubble.EnableTC"] = (FindName("b_tc") as System.Windows.Controls.CheckBox).IsChecked.GetValueOrDefault();
            table["Bubble.IntervalMinutes"] = (FindName("b_interval") as MahApps.Metro.Controls.NumericUpDown).Value.GetValueOrDefault();

            table["Bubble.TC"] = new List<string>((FindName("b_tcs") as System.Windows.Controls.TextBox).Text.Split('\n'));
        }

        private Core.Core _core;
        private string[] _trialStrings;
    }
}
