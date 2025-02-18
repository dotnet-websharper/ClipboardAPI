# WebSharper Clipboard API Binding

This repository provides an F# [WebSharper](https://websharper.com/) binding for the [Clipboard API](https://developer.mozilla.org/en-US/docs/Web/API/Clipboard_API), enabling seamless clipboard access and management in WebSharper applications.

## Repository Structure

The repository consists of two main projects:

1. **Binding Project**:

   - Contains the F# WebSharper binding for the Clipboard API.

2. **Sample Project**:
   - Demonstrates how to use the Clipboard API with WebSharper syntax.
   - Includes a GitHub Pages demo: [View Demo](https://dotnet-websharper.github.io/ClipboardAPI/).

## Features

- WebSharper bindings for the Clipboard API.
- Read and write text from/to the clipboard.
- Example usage for copying and pasting content.
- Hosted demo to explore API functionality.

## Installation and Building

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) installed on your machine.

### Steps

1. Clone the repository:

   ```bash
   git clone https://github.com/dotnet-websharper/ClipboardAPI.git
   cd Clipboard
   ```

2. Build the Binding Project:

   ```bash
   dotnet build WebSharper.Clipboard/WebSharper.Clipboard.fsproj
   ```

3. Build and Run the Sample Project:

   ```bash
   cd WebSharper.Clipboard.Sample
   dotnet build
   dotnet run
   ```

4. Open the hosted demo to see the Sample project in action:
   [https://dotnet-websharper.github.io/ClipboardAPI/](https://dotnet-websharper.github.io/ClipboardAPI/)

## Why Use the Clipboard API

The Clipboard API allows web applications to interact with the system clipboard for improved user experience. Key benefits include:

1. **Copy and Paste Support**: Read and write clipboard contents programmatically.
2. **Enhanced Productivity**: Enable seamless clipboard interactions within web applications.
3. **Security and Permissions**: Ensure proper clipboard access based on user permissions.
4. **Asynchronous API**: Handle clipboard operations efficiently using promises.

**Note:** Clipboard access may require user permission, depending on the browser and context.

Integrating the Clipboard API with WebSharper allows developers to build interactive and efficient applications in F#.

## How to Use the Clipboard API

### Example Usage

Below is an example of how to use the Clipboard API in a WebSharper project:

```fsharp
namespace WebSharper.Clipboard.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Notation
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.Clipboard

[<JavaScript>]
module Client =
    // The templates are loaded from the DOM, so you just can edit index.html
    // and refresh your browser, no need to recompile unless you add or remove holes.
    type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>

    // Variable to store the clipboard status
    let statusMessage = Var.Create ""

    // Variable to hold text input
    let textInput = Var.Create ""

    // Access the browser's Clipboard API
    let clipboard = As<Navigator>(JS.Window.Navigator).Clipboard

    // Function to copy text to the clipboard
    let copyText() =
        promise {
            try
                // Write the text input value to the clipboard
                do! clipboard.WriteText(textInput.Value)
                statusMessage := "Copied to clipboard!"
            with ex ->
                // Log and update status in case of error
                Console.Error("Failed to copy:", ex.Message)
                statusMessage := "Copy failed!"
        }

    // Function to read text from the clipboard
    let pasteText() =
        promise {
            try
                // Read text from the clipboard
                let! text = clipboard.ReadText()
                textInput := text
                statusMessage := "Pasted from clipboard!"
            with ex ->
                // Log and update status in case of error
                Console.Error("Failed to paste:", ex.Message)
                statusMessage := "Paste failed!"
        }

    [<SPAEntryPoint>]
    let Main () =
        // Initialize the UI template and bind variables to UI elements
        IndexTemplate.Main()
            // Bind the copy action to a UI button
            .copyText(fun _ ->
                async {
                    do! copyText().AsAsync()
                }
                |> Async.Start
            )
            // Bind the paste action to a UI button
            .pasteText(fun _ ->
                async {
                    do! pasteText().AsAsync()
                }
                |> Async.Start
            )
            // Bind status message to display operation results
            .status(statusMessage.V)
            // Bind text input variable for user interaction
            .textInput(textInput)
            .Doc()
        |> Doc.RunById "main"
```

This example demonstrates how to copy and paste text using the Clipboard API.

For a complete implementation, refer to the [Sample Project](https://dotnet-websharper.github.io/ClipboardAPI/).
