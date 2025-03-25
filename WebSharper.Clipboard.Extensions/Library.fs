namespace WebSharper.Clipboard

open WebSharper
open WebSharper.JavaScript

[<JavaScript;AutoOpen>]
module Extensions = 
    type Navigator with
        [<Inline "$this.clipboard">]
        member this.Clipboard with get():Clipboard = X<Clipboard>

    type Dom.Element with
        [<Inline "$this.oncopy">]
        member this.OnCopy with get(): (ClipboardEvent -> unit) = ignore
        [<Inline "$this.oncopy = $callback">]
        member this.OnCopy with set(callback:ClipboardEvent -> unit) = ()

        [<Inline "$this.oncut">]
        member this.OnCut with get(): (ClipboardEvent -> unit) = ignore
        [<Inline "$this.oncut = $callback">]
        member this.OnCut with set(callback:ClipboardEvent -> unit) = ()

        [<Inline "$this.onpaste">]
        member this.OnPaste with get(): (ClipboardEvent -> unit) = ignore
        [<Inline "$this.onpaste = $callback">]
        member this.OnPaste with set(callback:ClipboardEvent -> unit) = ()