using System.Threading.Tasks;

namespace HuggingFaceToOnnx.App.Services
{
    public interface IConverterService
    {
        Task ConvertModelAsync(string modelId, string outputDirectory, System.Action<string> logCallback);
        Task<bool> IsPythonAvailableAsync();
    }
}
