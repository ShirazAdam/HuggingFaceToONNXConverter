using CommunityToolkit.Mvvm.ComponentModel;

namespace HuggingFaceToOnnx.App.ViewModels
{
    public partial class ConversionItemViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _filePath;

        [ObservableProperty]
        private string _status;

        public ConversionItemViewModel(string filePath)
        {
            _filePath = filePath;
            _status = "Pending";
        }
    }
}
