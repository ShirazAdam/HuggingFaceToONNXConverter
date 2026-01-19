using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;

namespace HuggingFaceToOnnx.App.Services
{
    public class FileService : IFileService
    {
        public IEnumerable<string> OpenFiles(string filter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = filter
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileNames;
            }

            return Enumerable.Empty<string>();
        }
        public string? PickFolder()
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Select Output Folder"
            };

            if (dialog.ShowDialog() == true)
            {
                return dialog.FolderName;
            }
            return null;
        }
    }
}
