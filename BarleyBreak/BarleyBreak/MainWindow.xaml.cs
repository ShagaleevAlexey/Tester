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

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

using PuzzleSpace;
using System.Windows.Media.Animation;
using System.Windows.Markup;
using System.Globalization;

namespace BarleyBreak
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        PuzzleElementForWPF PuzzleElement;
        
        public MainWindow()
        {
            InitializeComponent();
            PuzzleElement = new PuzzleElementForWPF();
            vis_b_mainBorder.Child = PuzzleElement;
            PuzzleElement.CellMove += PuzzleElement_CellMove;
            PuzzleElement.Completed += PuzzleElement_Completed;
        }

        void PuzzleElement_Completed()
        {
            vis_btn_startLogical.IsEnabled = true;
            vis_btn_undo.IsEnabled = true;            
        }

        async void PuzzleElement_CellMove(BaseProgressItem ProgressItem)
        {
            vis_lb_history.Items.Add(ProgressItem.ToString());
            vis_lb_history.ScrollIntoView(vis_lb_history.Items[vis_lb_history.Items.Count - 1]);

            if (PuzzleElement.PuzzleObject.IsWIN())
            {
                await this.ShowMessageAsync("Status", "You WIN!");
            }
        }

        private void w_mainWindow_Activated(object sender, EventArgs e)
        {
            
        }

        private void w_mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void vis_btn_startLogical_Click(object sender, RoutedEventArgs e)
        {
            PuzzleElement.StartLogical();

            vis_btn_startLogical.IsEnabled = false;
            vis_btn_undo.IsEnabled = false;
        }

        private void vis_lb_history_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int id = vis_lb_history.SelectedIndex;

            if ((id != -1) && (id == PuzzleElement.PuzzleObject.History.Count - 1))
            {
                vis_lb_history.Items.RemoveAt(id);

                PuzzleElement.UndoMove();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            PuzzleElement.UndoMove();
            if (vis_lb_history.Items.Count > 0)
                vis_lb_history.Items.RemoveAt(vis_lb_history.Items.Count - 1);
        }

        private void w_mainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!e.HeightChanged || !e.WidthChanged)
                if (e.WidthChanged)
                    this.MaxHeight = cd_mainField.ActualWidth + vis_b_mainBorder.Margin.Top + vis_b_mainBorder.Margin.Bottom;
        }
    }
}

namespace BarleyBreak.Converters
{
    public class PercentageConverter : MarkupExtension, IValueConverter
    {
        private static PercentageConverter _instance;

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDouble(value) * System.Convert.ToDouble(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new PercentageConverter());
        }
    }
}