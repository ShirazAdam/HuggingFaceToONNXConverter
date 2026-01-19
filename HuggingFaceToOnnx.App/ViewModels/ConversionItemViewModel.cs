using CommunityToolkit.Mvvm.ComponentModel;

namespace HuggingFaceToOnnx.App.ViewModels
{
    public partial class ConversionItemViewModel(string filePath) : ObservableObject
    {
        [ObservableProperty]
        private string _filePath = filePath;

        [ObservableProperty]
        private string _status = "Pending";
    }
}
