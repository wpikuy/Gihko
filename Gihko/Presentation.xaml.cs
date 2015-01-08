using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Gihko.QuickFunc;

namespace Gihko {
    /// <summary>
    /// Presentation.xaml 的交互逻辑
    /// </summary>
    public partial class Presentation : Window {
        public Presentation() {
            InitializeComponent();
            Loaded += (sender, args) =>{
                _core = new Core.Core(this);
                _core.init();
            };
            Loaded += (sender, args) => reset();
        }

        public bool showText(string text){
            if (_bubbleShown) return false;
            showBubble(text);
            return true;
        }

        public void loadProps(){
            List<QuickFuncElem> elems = _core.Props["QuickFunc.Funcs"] as List<QuickFuncElem>;
            for (int i = 1; i < 7; i++){
                (FindName("Pl" + i.ToString()) as Label).Content = elems[i - 1].Description;
            }
        }

        private void reset(){
            for (int i = 1; i < 7; i++){
                (FindName("Panel" + i.ToString()) as Grid).Margin = _origin;
                (FindName("Panel" + i.ToString()) as Grid).Opacity = 0;
            }
            Bubble.Opacity = 0;
            _buttonsShown = false;
            _mouseTimer = new DispatcherTimer();
            _mouseTimer.Interval = TimeSpan.FromSeconds(1);
            _mouseTimer.Tick += (sender, args) =>{
                if (_buttonsShown){
                    hideButtons();
                }
                _mouseTimer.Stop();
            };
            _buttonsAnimDone = true;
            _bubbleShown = false;
        }

        private void dragMove(object sender, RoutedEventArgs args){
            DragMove();
        }

        private void exit(object sender, RoutedEventArgs args) {
            if (_core == null){
                Close();
            }
            else{
                _core.exit();
            }
        }

        private void showBubble(string text){
            BubbleText.Text = text;
            var from = _bubbleMargin;
            from.Bottom -= 50;
            from.Top += 50;
            var thickAnim = new ThicknessAnimation{
                From = from,
                To = _bubbleMargin,
                Duration = TimeSpan.FromSeconds(0.6),
                EasingFunction = new CircleEase()
            };
            var opacityAnim = new DoubleAnimation{
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.6),
                EasingFunction = new CircleEase()
            };
            Bubble.BeginAnimation(Grid.MarginProperty, thickAnim);
            Bubble.BeginAnimation(Grid.OpacityProperty, opacityAnim);
            _bubbleShown = true;

            DispatcherTimer tmpTimer = new DispatcherTimer {
                Interval = TimeSpan.FromSeconds(6)
            };
            tmpTimer.Tick += (sender, args) => {
                hideBubble();
                tmpTimer.Stop();
            };
            tmpTimer.Start();
        }

        private void hideBubble(){
            var to = _bubbleMargin;
            to.Bottom -= 50;
            to.Top += 50;
            var thickAnim = new ThicknessAnimation {
                From = _bubbleMargin,
                To = to,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new CircleEase()
            };
            var opacityAnim = new DoubleAnimation {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new CircleEase()
            };
            Bubble.BeginAnimation(Grid.MarginProperty, thickAnim);
            Bubble.BeginAnimation(Grid.OpacityProperty, opacityAnim);

            DispatcherTimer tmpTimer = new DispatcherTimer {
                Interval = TimeSpan.FromSeconds(0.3)
            };
            tmpTimer.Tick += (sender, args) => {
                _bubbleShown = false;
                if(OnBubbleExit != null) OnBubbleExit.Invoke();
                tmpTimer.Stop();
            };
            tmpTimer.Start();
        }

        private void showButtons(){
            _buttonHelpIndex = 0;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.03);
            timer.Tick += (sender, args) =>{
                if (_buttonHelpIndex > 5){
                    timer.Stop();
                }
                else{
                    var from = _origin;
                    var thickAnim = new ThicknessAnimation{
                        From = from,
                        To = _buttonMargins[_buttonHelpIndex],
                        Duration = TimeSpan.FromSeconds(0.6),
                        EasingFunction = new CircleEase()
                    };
                    var opacityAnim = new DoubleAnimation{
                        From = 0,
                        To = 1,
                        Duration = TimeSpan.FromSeconds(0.6),
                        EasingFunction = new QuadraticEase()
                    };
                    (FindName("Panel" + (_buttonHelpIndex + 1).ToString()) as Grid).BeginAnimation(Grid.MarginProperty,
                        thickAnim);
                    (FindName("Panel" + (_buttonHelpIndex + 1).ToString()) as Grid).BeginAnimation(Grid.OpacityProperty,
                        opacityAnim);

                    _buttonHelpIndex++;
                }
            };
            timer.Start();

            _buttonsShown = true;
            _buttonsAnimDone = false;

            DispatcherTimer tmpTimer = new DispatcherTimer {
                Interval = TimeSpan.FromSeconds(1.1)
            };
            tmpTimer.Tick += (sender, args) => {
                _buttonsAnimDone = true;
                tmpTimer.Stop();
            };
            tmpTimer.Start();
        }

        private void hideButtons(){
            _buttonHelpIndex = 0;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.06);
            timer.Tick += (sender, args) => {
                if (_buttonHelpIndex > 5) {
                    timer.Stop();
                }
                else{
                    Thickness to;
                    if (_buttonHelpIndex < 3){
                        to = _buttonMargins[_buttonHelpIndex];
                        to.Left += 30;
                        to.Right -= 30;
                    }
                    else{
                        to = _buttonMargins[_buttonHelpIndex];
                        to.Left -= 30;
                        to.Right += 30;
                    }
                    var thickAnim = new ThicknessAnimation {
                        From = _buttonMargins[_buttonHelpIndex],
                        To = to,
                        Duration = TimeSpan.FromSeconds(0.6),
                        EasingFunction = new CubicEase()
                    };
                    var opacityAnim = new DoubleAnimation {
                        From = 1,
                        To = 0,
                        Duration = TimeSpan.FromSeconds(0.6),
                        EasingFunction = new CubicEase()
                    };
                    (FindName("Panel" + (_buttonHelpIndex + 1).ToString()) as Grid).BeginAnimation(Grid.MarginProperty,
                        thickAnim);
                    (FindName("Panel" + (_buttonHelpIndex + 1).ToString()) as Grid).BeginAnimation(Grid.OpacityProperty,
                        opacityAnim);

                    _buttonHelpIndex++;
                }
            };
            timer.Start();
            _buttonsAnimDone = false;

            DispatcherTimer tmpTimer = new DispatcherTimer{
                Interval = TimeSpan.FromSeconds(1.1)
            };
            tmpTimer.Tick += (sender, args) =>{
                _buttonsShown = false;
                _buttonsAnimDone = true;
                tmpTimer.Stop();
            };
            tmpTimer.Start();
        }

        private void mouseEnter(object sender, RoutedEventArgs args){
            if (!_buttonsShown){
                showButtons();
            }
            if (!_twiceBlinkPlaying){
                twiceBlink();
            }
            _mouseTimer.Stop();
            _mouseTimer.Interval = TimeSpan.FromSeconds(1);
        }

        private void mouseExit(object sender, RoutedEventArgs args) {
            _mouseTimer.Start();
        }

        private void buttonEnter(object sender, RoutedEventArgs args){
            if (!_buttonsShown) return;
            /*
            if (!_buttonsAnimDone){
                DispatcherTimer tmpTimer = new DispatcherTimer {
                    Interval = TimeSpan.FromSeconds(0.1)
                };
                tmpTimer.Tick += (sender2, args2) => {
                    buttonEnter(sender, args);
                    tmpTimer.Stop();
                };
                tmpTimer.Start();
                return;
            }
             */
            string index = (sender as Grid).Name[5].ToString();
            if (int.Parse(index) > 3){
                var to = _buttonMargins[int.Parse(index) - 1];
                to.Left -= 20;
                to.Right += 20;
                var thickAnim = new ThicknessAnimation {
                    From = (sender as Grid).Margin,
                    To = to,
                    Duration = TimeSpan.FromSeconds(0.6),
                    EasingFunction = new CubicEase()
                };
                (sender as Grid).BeginAnimation(Grid.MarginProperty, thickAnim);
            }
            else {
                var to = _buttonMargins[int.Parse(index) - 1];
                to.Left += 20;
                to.Right -= 20;
                var thickAnim = new ThicknessAnimation {
                    From = (sender as Grid).Margin,
                    To = to,
                    Duration = TimeSpan.FromSeconds(0.6),
                    EasingFunction = new CubicEase()
                };
                (sender as Grid).BeginAnimation(Grid.MarginProperty, thickAnim);
            }
        }

        private void buttonExit(object sender, RoutedEventArgs args) {
            string index = (sender as Grid).Name[5].ToString();
            var from = (sender as Grid).Margin;
            var thickAnim = new ThicknessAnimation {
                From = from,
                To = _buttonMargins[int.Parse(index) - 1],
                Duration = TimeSpan.FromSeconds(0.6),
                EasingFunction = new CubicEase()
            };
            (sender as Grid).BeginAnimation(Grid.MarginProperty, thickAnim);
        }

        private void buttonPress(object sender, RoutedEventArgs args){
            int index = int.Parse((sender as Grid).Name[5].ToString());
            if (_core != null){
                _core.QuickFunc(index);
            }
        }

        private void openSettings(object sender, RoutedEventArgs args){
            _core.openSettings();
        }

        private int _blinkHelper;
        private void playBlink(double speed){
            _blinkHelper = 0;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(speed);
            timer.Tick += (sender, args) => {
                if (_blinkHelper > 6){
                    timer.Stop();
                }
                else{
                    int index = _blinkHelper;
                    if (index > 3){
                        index = 6 - index;
                    }
                    Mascot.Source = new BitmapImage(new Uri("img/eyes-" + index + ".png", UriKind.Relative));
                    _blinkHelper++;
                }
            };
            timer.Start();
        }

        private int _shakeHeadHelper;
        private string[] _shakeHeadNames = new string[]{
            "center", "left-1", "left-2", "left-1" , "center", "right-1", "right-2", "right-1", "center"
        };
        private void playShakeHead(double speed){
            _shakeHeadHelper = 0;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(speed);
            timer.Tick += (sender, args) => {
                if (_shakeHeadHelper > 8) {
                    timer.Stop();
                }
                else {
                    int index = _shakeHeadHelper;
                    Mascot.Source = new BitmapImage(new Uri("img/" + _shakeHeadNames[index] + ".png", UriKind.Relative));
                    _shakeHeadHelper++;
                }
            };
            timer.Start();
        }

        private bool _twiceBlinkPlaying;
        private void twiceBlink(){
            _twiceBlinkPlaying = true;
            playBlink(0.05);
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.6);
            timer.Tick += (sender, args) => {
                timer.Stop();
                playBlink(0.05);
            };
            timer.Start();
            DispatcherTimer timer2 = new DispatcherTimer();
            timer2.Interval = TimeSpan.FromSeconds(1.2);
            timer2.Tick += (sender, args) => {
                timer2.Stop();
                _twiceBlinkPlaying = false;
            };
            timer2.Start();
        }

        private void test_1(object sender, RoutedEventArgs args){
            showBubble("喵帕斯~！");
            showButtons();
        }

        private void test_2(object sender, RoutedEventArgs args){
            twiceBlink();
        }

        private Thickness _bubbleMargin = new Thickness(78, 29, 78, 234);
        private Thickness _origin = new Thickness(176, 162, 176, 70);
        private Thickness[] _buttonMargins = new[]{
            new Thickness(280,152,70,168),
            new Thickness(304,213,46,107),
            new Thickness(280,271,70,49),
            new Thickness(70,271,280,49),
            new Thickness(46,213,304,107),
            new Thickness(70,152,280,168)
        };

        private System.Windows.Controls.ContextMenu _menu;
        private Core.Core _core;
        private bool _buttonsShown;
        private bool _bubbleShown;

        private int _buttonHelpIndex;
        private bool _buttonsAnimDone;

        private DispatcherTimer _mouseTimer;

        public event Action OnBubbleExit;
    }
}
