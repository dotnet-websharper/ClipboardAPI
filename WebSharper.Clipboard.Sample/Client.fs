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

    let statusMessage = Var.Create ""
    let textInput = Var.Create ""

    let clipboard = As<Navigator>(JS.Window.Navigator).Clipboard

    let copyText() =
        promise {
            try
                do! clipboard.WriteText(textInput.Value) 
                statusMessage := "Copied to clipboard!"
            with ex ->
                Console.Error("Failed to copy:", ex.Message)
                statusMessage := "Copy failed!"
        }
    
    let pasteText() =
        promise {
            try
                let! text = clipboard.ReadText()
                textInput := text
                statusMessage := "Pasted from clipboard!"
            with ex ->
                Console.Error("Failed to paste:", ex.Message)
                statusMessage := "Paste failed!"
        }

    [<SPAEntryPoint>]
    let Main () =

        IndexTemplate.Main()
            .copyText(fun _ -> 
                async {
                    do! copyText().AsAsync()
                }
                |> Async.Start
            )
            .pasteText(fun _ -> 
                async {
                    do! pasteText().AsAsync()
                }
                |> Async.Start
            )
            .status(statusMessage.V)
            .textInput(textInput)
            .Doc()
        |> Doc.RunById "main"
