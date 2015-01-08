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
using System.Windows.Threading;

namespace MascotMetroTabLibrary {
    /// <summary>
    /// LMTabController.xaml 的交互逻辑
    /// </summary>
    public partial class LMTabController : UserControl {
        public LMTabController() {
            InitializeComponent();
            Loaded += (sender, args) => initController();
        }

        private void initController(){
            if (Window.GetWindow(this) == null || !Window.GetWindow(this).IsActive) return;
            _items = GetChildObjects<LMTabItem>();
            _selectedItem = _items[0];

            foreach (LMTabItem item in _items){
                item.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                item.MouseEnter += (sender, args) =>{
                    if (_selectedItem != item){
                        item.Background = new SolidColorBrush(HoverColor);
                    }
                };
                item.MouseLeave += (sender, args) => {
                    if (_selectedItem != item){
                        item.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    }
                };
                item.MouseLeftButtonUp += (sender, args) => {
                    foreach (LMTabItem it in _items){
                        it.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                        var grid = LogicalTreeHelper.FindLogicalNode(Window.GetWindow(this), it.PageName) as Grid;
                        if (grid != null)
                            grid.Visibility = Visibility.Hidden;
                    }
                    _selectedItem = item;
                    _selectedItem.Background = new SolidColorBrush(PressedColor);
                    var grid1 = LogicalTreeHelper.FindLogicalNode(Window.GetWindow(this), _selectedItem.PageName) as Grid;
                    if (grid1 !=
                        null)
                        grid1.Visibility = Visibility.Visible;
                };
            }
            _selectedItem.Background = new SolidColorBrush(PressedColor);
            var grid2 = LogicalTreeHelper.FindLogicalNode(Window.GetWindow(this), _selectedItem.PageName) as Grid;
            if (grid2 != null)
                grid2.Visibility = Visibility.Visible;
        }

        public List<T> GetChildObjects<T>() where T : UserControl {
            Object child = null;
            var children = new List<T>();
            var grandpa = LogicalTreeHelper.GetChildren(this).GetEnumerator();
            grandpa.MoveNext();
            var father = LogicalTreeHelper.GetChildren(grandpa.Current as FrameworkElement).GetEnumerator();

            while (father.MoveNext()) {
                child = father.Current;
                if (child is T) {
                    children.Add(child as T);
                }
            }

            return children;
        }

        private LMTabItem _selectedItem;
        private List<LMTabItem> _items; 

        public Color HoverColor { get; set; }
        public Color PressedColor { get; set; }
    }
}
