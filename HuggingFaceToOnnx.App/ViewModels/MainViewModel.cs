using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HuggingFaceToOnnx.App.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HuggingFaceToOnnx.App.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IConverterService _converterService;
        private readonly IFileService _fileService;

        [ObservableProperty]
        private ObservableCollection<ConversionItemViewModel> _models = new();

        [ObservableProperty]
        private string _outputDirectory;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ConvertCommand))]
        private bool _isBusy;

        [ObservableProperty]
        private string _logOutput;

        public MainViewModel(IConverterService converterService, IFileService fileService)
        {
            _converterService = converterService;
            _fileService = fileService;
            _logOutput = "Ready to convert.\n";
            _outputDirectory = System.IO.Directory.GetCurrentDirectory();
        }

        [RelayCommand(CanExecute = nameof(CanConvert))]
        private async Task ConvertAsync()
        {
            IsBusy = true;
            LogOutput = "";
            AppendLog("Starting parallel conversion...");

            var tasks = Models.Where(m => m.Status != "Success").Select(async model =>
            {
                model.Status = "Converting...";
                var modelDir = Directory.GetParent(model.FilePath)?.FullName ?? model.FilePath;
                var modelName = Path.GetFileName(model.FilePath);
                var folderName = Path.GetFileNameWithoutExtension(model.FilePath);
                var specificOutput = Path.Combine(OutputDirectory, folderName);

                try
                {
                    // We log with prefix
                    await _converterService.ConvertModelAsync(modelDir, specificOutput, (msg) => AppendLog($"[{modelName}] {msg}"));
                    model.Status = "Success";
                }
                catch (Exception ex)
                {
                    model.Status = "Failed";
                    AppendLog($"[{modelName}] Check failed: {ex.Message}");
                }
            });

            await Task.WhenAll(tasks);

            IsBusy = false;
            AppendLog("All conversions finished.");
        }

        private bool CanConvert()
        {
            return Models.Any() && !IsBusy;
        }

        [RelayCommand]
        private void BrowseFiles()
        {
            var files = _fileService.OpenFiles("Model Files|*.safetensors;*.bin;*.pt;*.h5|All Files|*.*");
            foreach (var file in files)
            {
                if (!Models.Any(m => m.FilePath == file))
                {
                    Models.Add(new ConversionItemViewModel(file));
                }
            }
            ConvertCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand]
        private void BrowseOutputFolder()
        {
            var folder = _fileService.PickFolder();
            if (!string.IsNullOrEmpty(folder))
            {
                OutputDirectory = folder;
            }
        }

        private void AppendLog(string message)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                LogOutput += message + "\n";
            });
        }
    }
}
