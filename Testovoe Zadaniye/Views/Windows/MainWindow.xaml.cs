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
using System.Windows.Shapes;

namespace Testovoe_Zadaniye.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ViewModels.MainWindowViewModel.MW = this; // отступление от правил mvvm , так как пока что могу не полностью отделить вьюшку от вьюмодели
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Payment.Visibility = Visibility.Hidden;
        }
    }
}
