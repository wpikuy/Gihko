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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MascotMetroTabLibrary {
    /// <summary>
    /// MascotButton.xaml 的交互逻辑
    /// </summary>
    public partial class MascotButton : UserControl {
        public MascotButton() {
            InitializeComponent();
            initButton();
        }

        public Color HoverColor { get; set; }
        public Color PressedColor { get; set; }

        public event RoutedEventHandler Click;

        private void initButton(){
            Background = null;
            MouseEnter += (sender, args) => {
                Background = new SolidColorBrush(HoverColor);
            };
            MouseLeave += (sender, args) => {
                Background = null;
            };
            MouseLeftButtonDown += (sender, args) => {
                Background = new SolidColorBrush(PressedColor);
            };
            MouseLeftButtonUp += (sender, args) => {
                Background = new SolidColorBrush(HoverColor);
                if (Click != null){
                    Click.Invoke(this, args);
                }
            };
        }
    }
}
