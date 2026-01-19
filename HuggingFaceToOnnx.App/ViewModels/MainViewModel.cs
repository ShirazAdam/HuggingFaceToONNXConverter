using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HuggingFaceToOnnx.App.Services;
using System.Threading.Tasks;

namespace HuggingFaceToOnnx.App.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IConverterService _converterService;

        [ObservableProperty]
        private string _modelId;

        [ObservableProperty]
        private string _outputDirectory;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ConvertCommand))]
        private bool _isBusy;

        [ObservableProperty]
        private string _logOutput;

        public MainViewModel(IConverterService converterService)
        {
            _converterService = converterService;
            _logOutput = "Ready to convert.\n";
            _outputDirectory = System.IO.Directory.GetCurrentDirectory(); // Default
        }

        [RelayCommand(CanExecute = nameof(CanConvert))]
        private async Task ConvertAsync()
        {
            IsBusy = true;
            LogOutput = ""; // Clear or append? Let's clear for new run.
            AppendLog($"Starting conversion for {ModelId}...");

            await _converterService.ConvertModelAsync(ModelId, OutputDirectory, AppendLog);

            IsBusy = false;
        }

        private bool CanConvert()
        {
            return !string.IsNullOrWhiteSpace(ModelId) && !IsBusy;
        }

        [RelayCommand]
        private void BrowseFolder()
        {
            // TODO: Implement proper FolderPicker for WPF without WinForms dependency
            // For now, we rely on manual entry or pasting.
            // Alternatively, could use Windows APICodePack or similar.
            _logOutput += "Folder browsing not implemented in this version (WinForms dep removed). Please type path.\n";
            OnPropertyChanged(nameof(LogOutput));
        }

        private void AppendLog(string message)
        {
            // Marshall to UI thread if needed? properties are usually fine if set, 
            // but ObservableProperty raises PropertyChanged.
            // CommunityToolkit.Mvvm doesn't automatically marshal.
            // In WPF, we might need Application.Current.Dispatcher.
            
            System.Windows.Application.Current.Dispatcher.Invoke(() => 
            {
                LogOutput += message + "\n";
            });
        }
    }
}
