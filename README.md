# HuggingFace to ONNX Converter

A Windows desktop application (WPF) that enables easy conversion of Hugging Face models to the ONNX format. Built with .NET 10 and C#.

## Features

- **Simple UI**: Clean interface to input Model IDs and select output locations.
- **Automated Conversion**: Wraps `optimum-cli` to handle the complex conversion process automatically.
- **Log Feedback**: Real-time console output view to monitor the conversion progress.
- **Modern Architecture**: Built using the MVVM pattern with `CommunityToolkit.Mvvm` and Dependency Injection via `Microsoft.Extensions.DependencyInjection`.

## Prerequisites

To run and use this application, you need:

1.  **.NET 10 SDK** (or a compatible runtime/SDK installed).
2.  **Python 3.8+** installed and added to your system PATH.
3.  **Optimum Library**: Install the necessary Python packages:
    ```powershell
    pip install optimum[onnxruntime]
    ```

## Installation & Build

1.  Clone the repository.
2.  Open the solution `HuggingFaceToOnnx.slnx` (or `.sln`) in Visual Studio or operate via CLI.
3.  Restore dependencies:
    ```powershell
    dotnet restore
    ```
4.  Build the project:
    ```powershell
    dotnet build
    ```

## Usage

1.  Run the application:
    ```bash
    dotnet run --project HuggingFaceToOnnx.App
    ```
2.  **Model ID**: Enter the Hugging Face model ID (e.g., `bert-base-uncased`, `gpt2`, `prajjwal1/bert-tiny`).
3.  **Output Directory**: Select the folder where you want the resulting `.onnx` files to be saved.
4.  Click **Convert**.
5.  Watch the logs for "Conversion completed successfully."

## Architecture

- **HuggingFaceToOnnx.App**: Main WPF Application.
    - **ViewModels**: Contains `MainViewModel` handling UI logic.
    - **Services**: `ConverterService` manages the Python process interop.
    - **App.xaml.cs**: Configures the DI container (`IServiceProvider`) and application entry point.

## Licence

MIT Licence
