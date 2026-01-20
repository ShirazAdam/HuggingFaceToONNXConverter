using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HuggingFaceToOnnx.App.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace HuggingFaceToOnnx.App.ViewModels
{
    public partial class MainViewModel(IConverterService converterService, IFileService fileService) : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ConversionItemViewModel> _models = new();

        [ObservableProperty]
        private string _outputDirectory = Directory.GetCurrentDirectory();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ConvertCommand))]
        private bool _isBusy;

        [ObservableProperty]
        private string _logOutput = "Ready to convert.\n";

        [RelayCommand(CanExecute = nameof(CanConvert))]
        private async Task ConvertAsync()
        {
            IsBusy = true;
            LogOutput = string.Empty;
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
                    await converterService.ConvertModelAsync(modelDir, specificOutput, (msg) => AppendLog($"[{modelName}] {msg}"));
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
            var files = fileService.OpenFiles("Model Files|*.safetensors;*.bin;*.pt;*.h5|All Files|*.*");
            foreach (var file in files)
            {
                if (Models.All(m => m.FilePath != file))
                {
                    Models.Add(new ConversionItemViewModel(file));
                }
            }
            ConvertCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand]
        private void BrowseOutputFolder()
        {
            var folder = fileService.PickFolder();
            if (!string.IsNullOrWhiteSpace(folder))
            {
                OutputDirectory = folder;
            }
        }

        private void AppendLog(string message)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                LogOutput += $"{message}\n";
            });
        }

        [RelayCommand]
        private void OpenUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return;
            
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch
            {
                AppendLog($"Failed to open URL: {url}");
            }
        }
    }
}
