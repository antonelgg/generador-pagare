using System.Windows;

namespace GeneradorPDF.WPF;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new PagareViewModel();
    }
}
