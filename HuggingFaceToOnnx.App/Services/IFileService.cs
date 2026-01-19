namespace HuggingFaceToOnnx.App.Services
{
    public interface IFileService
    {
        IEnumerable<string> OpenFiles(string filter);
        
        string? PickFolder();
    }
}
