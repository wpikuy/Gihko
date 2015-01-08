using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Gihko.Setting;

namespace Gihko.Core {
    public class Core {

        public Core (Presentation presentation){
            _presentation = presentation;
        }

        public void init(){

            _closing = false;
            _bubbleTexts = new Queue<string>();

            loadProps();

            _presentation.Left = (double)Props["Presentation.Left"];
            _presentation.Top = (double)Props["Presentation.Top"];
            rebuildSettings();

            _bubble = new Bubble.Bubble();
            _quickFunc = new QuickFunc.QuickFunc();

            _quickFunc.init(this);
            _bubble.init(this);

            _presentation.OnBubbleExit += pushText;

            initTray();
        }

        private void initTray(){
            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = Properties.Resources.logo;
            _notifyIcon.MouseClick += (sender, args) => reposMascot();
            _notifyIcon.ContextMenu = new ContextMenu(new []{
                new MenuItem("调教Gihko酱", delegate {openSettings();}), 
                new MenuItem("找回丢失的Gihko酱", delegate {reposMascot();}), 
                new MenuItem("-"), 
                new MenuItem("把Gihko酱扔回她的小窝", delegate{exit();}),
            });
            _notifyIcon.Visible = true;
            _notifyIcon.ShowBalloonTip(3000, "Gihko", "Gihko已经出来活动了哦~ 找不到我的话就点这儿吧。", ToolTipIcon.Info);
        }

        private void reposMascot(){
            var posLeftAnim = new DoubleAnimation{
                From = _presentation.Left,
                To = 100,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new CubicEase()
            };
            var posTopAnim = new DoubleAnimation {
                From = _presentation.Top,
                To = 100,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new CubicEase()
            };
            posLeftAnim.FillBehavior = FillBehavior.Stop;
            posTopAnim.FillBehavior = FillBehavior.Stop;
            _presentation.BeginAnimation(Window.LeftProperty, posLeftAnim);
            _presentation.BeginAnimation(Window.TopProperty, posTopAnim);
        }

        private void loadProps(){
            if (File.Exists(_propsPath)){
                using (var stream = File.OpenRead(_propsPath)) {
                    var formatter = new BinaryFormatter();
                    Props = formatter.Deserialize(stream) as Hashtable;
                }
            }
            else{
                Props = new Hashtable();
            }

            if (!Props.Contains("Presentation.Left")) Props.Add("Presentation.Left", _presentation.Left);
            if (!Props.Contains("Presentation.Top")) Props.Add("Presentation.Top", _presentation.Top);
        }

        private void writeProps(){
            using (var stream = File.Create(_propsPath)) {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, Props);
            }
        }

        private void rebuildSettings() {
            _settings = new Settings();
            _settings.setCore(this);
            _settings.loadProps();
            _presentation.loadProps();
            writeProps();
            _settings.Closed += (sender, args) =>{
                if (!_closing){
                    rebuildSettings();
                }
            };
        }

        // for bubble
        public void showText(string text) {
            _settings.loadProps();
            if (!_presentation.showText(text)){
                _bubbleTexts.Enqueue(text);
            }
        }

        private void pushText(){
            if (_bubbleTexts.Count > 0){
                _presentation.showText(_bubbleTexts.Dequeue());
            }
        }

        public void exit(){

            _closing = true;

            Props["Presentation.Left"] = _presentation.Left;
            Props["Presentation.Top"] = _presentation.Top;
            _notifyIcon.Visible = false;

            if (_settings != null){
                _settings.Close();
            }

            writeProps();

            _presentation.Close();
        }

        public void QuickFunc(int index) {
            _quickFunc.execute(index);
        }

        public void openSettings(){
            _settings.Show();
            _settings.Focus();
        }

        //Field
        private Queue<string> _bubbleTexts;

        private Presentation _presentation;
        private Bubble.Bubble _bubble;
        private QuickFunc.QuickFunc _quickFunc;
        private Setting.Settings _settings;

        private bool _closing;

        // Props
        public Hashtable Props;

        private string _propsPath = "./gihko.prop";
        private NotifyIcon _notifyIcon;
    }
}
