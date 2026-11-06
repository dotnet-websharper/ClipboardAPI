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
    // The template is loaded from the DOM, so editing index.html and refreshing
    // the browser is enough; a rebuild is only needed when holes change.
    type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>

    // Shape of the ClipboardItem data dictionary: a plain object keyed by MIME type.
    type ClipboardItemData =
        {
            ``text/html``: string
            ``text/plain``: string
        }

    let statusMessage = Var.Create "Ready."
    let textInput = Var.Create ""
    let richPreview = Var.Create ""
    let clipboardEvent = Var.Create ""

    let clipboard = JS.Window.Navigator.Clipboard

    // ClipboardItem.supports() is part of the Async Clipboard API surface.
    let supportsHtml =
        try
            ClipboardItem.Supports("text/html")
        with _ ->
            false

    let capabilities =
        if supportsHtml then
            "This browser supports rich text (text/html) on the clipboard."
        else
            "This browser does not support rich text (text/html) on the clipboard."

    let copyText () =
        promise {
            try
                do! clipboard.WriteText textInput.Value
                statusMessage := "Copied plain text to the clipboard."
            with ex ->
                Console.Error("Failed to copy text:", ex.Message)
                statusMessage := "Copy failed: " + ex.Message
        }

    let pasteText () =
        promise {
            try
                let! text = clipboard.ReadText()
                textInput := text
                statusMessage := "Pasted plain text from the clipboard."
            with ex ->
                Console.Error("Failed to paste text:", ex.Message)
                statusMessage := "Paste failed: " + ex.Message
        }

    let copyRichText () =
        promise {
            try
                let html = "<p><strong>Hello</strong> from the WebSharper Clipboard demo!</p>"
                let text = "Hello from the WebSharper Clipboard demo!"
                let item = ClipboardItem(box { ``text/html`` = html; ``text/plain`` = text })
                do! clipboard.Write [| item |]
                statusMessage := "Copied rich text (text/html + text/plain)."
            with ex ->
                Console.Error("Failed to copy rich text:", ex.Message)
                statusMessage := "Rich copy failed: " + ex.Message
        }

    let pasteRichText () =
        promise {
            try
                let! items = clipboard.Read()
                if items.Length = 0 then
                    statusMessage := "The clipboard is empty."
                else
                    let item = items.[0]
                    if Array.contains "text/html" item.Types then
                        let! blob = item.GetType("text/html")
                        let! html = blob.Text()
                        richPreview := html
                        statusMessage := "Pasted rich text from the clipboard."
                    else
                        statusMessage := "No text/html on the clipboard."
            with ex ->
                Console.Error("Failed to paste rich text:", ex.Message)
                statusMessage := "Rich paste failed: " + ex.Message
        }

    [<SPAEntryPoint>]
    let Main () =
        // The clipboardchange event is experimental; ignore failures on browsers
        // that do not expose it (or when the API is unavailable).
        try
            clipboard.OnClipboardChange <- fun _ ->
                clipboardEvent := "Received a clipboardchange event."
        with _ ->
            ()

        IndexTemplate.Main()
            .copyText(fun _ -> copyText() |> ignore)
            .pasteText(fun _ -> pasteText() |> ignore)
            .copyRichText(fun _ -> copyRichText() |> ignore)
            .pasteRichText(fun _ -> pasteRichText() |> ignore)
            .status(statusMessage.V)
            .textInput(textInput)
            .capabilities(capabilities)
            .richPreview(richPreview.V)
            .clipboardEvent(clipboardEvent.V)
            .Doc()
        |> Doc.RunById "main"
