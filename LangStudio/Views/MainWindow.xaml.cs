using LangStudio.Models;
using LangStudio.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace LangStudio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

        private void MaximizeRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private MainViewModel MainViewModel { get; set; } = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = MainViewModel;
        }

        private void TranslationTreeView_SelectedItemChanged(Object sender, RoutedPropertyChangedEventArgs<Object> e)
        {
            if (e.NewValue is TranslationNode node)
                MainViewModel.SelectedNode = node;
        }
    }
}