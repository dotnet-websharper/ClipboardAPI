﻿namespace WebSharper.Clipboard

open WebSharper
open WebSharper.JavaScript
open WebSharper.InterfaceGenerator

module Definition =

    module Enum = 
        let PresentationStyle = 
            Pattern.EnumStrings "PresentationStyle" [
                "unspecified"
                "inline"
                "attachment"
            ]

        let ClipboardEventType =
            Pattern.EnumStrings "ClipboardEventType" [
                "copy"
                "cut"
                "paste"
            ]

    let ClipboardItemOptions =
        Pattern.Config "ClipboardItemOptions" {
            Required = []
            Optional = [
                "presentationStyle", Enum.PresentationStyle.Type
            ]
        }

    let ClipboardItem =
        Class "ClipboardItem"
        |+> Static [ 
            Constructor (T<obj>?data * !?ClipboardItemOptions?options)

            "supports" => T<string>?``type`` ^-> T<bool>
        ]
        |+> Instance [
            "presentationStyle" =? Enum.PresentationStyle.Type
            "types" =? !|T<string>  

            "getType" => T<string>?``type`` ^-> T<Promise<Blob>>  
        ]

    let ClipboardReadFormats =
        Pattern.Config "ClipboardReadFormats" {
            Required = []
            Optional = [
                "unsanitized", !|T<string>  
            ]
        }

    let Clipboard =
        Class "Clipboard"
        |=> Inherits T<Dom.EventTarget>
        |+> Instance [
            "read" => !?ClipboardReadFormats?formats ^-> T<Promise<_>>[!|ClipboardItem]  
            "readText" => T<unit> ^-> T<Promise<string>>  
            "write" => (!|ClipboardItem)?data ^-> T<Promise<unit>>  
            "writeText" => T<string>?newClipText ^-> T<Promise<unit>>  
        ]

    let ClipboardEventOptions =
        Pattern.Config "ClipboardEventOptions" {
            Required = []
            Optional = [
                "clipboardData", T<obj>  
                "dataType", T<string>  
                "data", T<string>  
            ]
        }

    let ClipboardEvent =
        Class "ClipboardEvent"
        |=> Inherits T<Dom.Event>
        |+> Static [
            Constructor (Enum.ClipboardEventType?``type`` * !?ClipboardEventOptions?options) 
        ]
        |+> Instance [
            "clipboardData" =? T<obj>
        ]

    let Navigator =
        Class "Navigator"
        |+> Instance [
            "clipboard" =? Clipboard.Type  
        ]

    let ELement = 
        Class "Element"
        |+> Instance [
            "oncopy" =@ T<unit> ^-> T<unit>
            |> ObsoleteWithMessage "Use OnCopy instead"
            "oncopy" =@ ClipboardEvent ^-> T<unit>
            |> WithSourceName "OnCopy"
            "oncut" =@ T<unit> ^-> T<unit>
            |> ObsoleteWithMessage "Use OnCut instead"
            "oncut" =@ ClipboardEvent ^-> T<unit>
            |> WithSourceName "OnCut"
            "onpaste" =@ T<unit> ^-> T<unit>
            |> ObsoleteWithMessage "Use OnPaste instead"
            "onpaste" =@ ClipboardEvent ^-> T<unit>
            |> WithSourceName "OnPaste"
        ]

    let Assembly =
        Assembly [
            Namespace "WebSharper.Clipboard" [
                ELement
                Navigator
                ClipboardEvent
                ClipboardEventOptions
                Clipboard
                ClipboardReadFormats
                ClipboardItem
                ClipboardItemOptions

                Enum.ClipboardEventType
                Enum.PresentationStyle
            ]
        ]

[<Sealed>]
type Extension() =
    interface IExtension with
        member ext.Assembly =
            Definition.Assembly

[<assembly: Extension(typeof<Extension>)>]
do ()
