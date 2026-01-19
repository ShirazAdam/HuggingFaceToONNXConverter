namespace HuggingFaceToOnnx.App;

public partial class MainWindow
{
    public MainWindow(ViewModels.MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}