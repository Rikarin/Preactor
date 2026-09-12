declare namespace CS {
    namespace Preactor {
        class Document extends System.Object
        {
            protected [__keep_incompatibility]: never;
            public get ScriptEngine(): Preactor.IScriptEngine;
            public get Body(): Preactor.Dom;
            public get EventBaseCache(): System.Collections.Generic.Dictionary$2<string, System.Type>;
            public get Root(): UnityEngine.UIElements.VisualElement;
            public AddRuntimeUss ($uss: string) : void
            public ClearRuntimeStyleSheets () : void
            public CreateElement ($tagName: string) : Preactor.Dom
            public CreateTextNode ($text: string) : Preactor.Dom
            public GetElementById ($id: string) : Preactor.Dom
            public QuerySelectorAll ($selector: string) : System.Array$1<Preactor.Dom>
            public LoadImage ($path: string) : UnityEngine.Sprite
            public LoadFont ($path: string) : UnityEngine.Font
            public LoadFontDefinition ($path: string) : UnityEngine.UIElements.FontDefinition
            public constructor ($root: UnityEngine.UIElements.VisualElement, $scriptEngine: Preactor.IScriptEngine)
        }
        class Dom extends System.Object
        {
            protected [__keep_incompatibility]: never;
            public get Document(): Preactor.Document;
            public get VisualElement(): UnityEngine.UIElements.VisualElement;
            public get ChildNodes(): System.Array$1<Preactor.Dom>;
            public get FirstChild(): Preactor.Dom;
            public get ParentNode(): Preactor.Dom;
            public get NextSibling(): Preactor.Dom;
            public get NodeType(): number;
            public get Id(): string;
            public set Id(value: string);
            public get Key(): string;
            public set Key(value: string);
            public get Style(): Preactor.DomStyle;
            public get Value(): any;
            public get Checked(): boolean;
            public get Data(): any;
            public set Data(value: any);
            public get ClassName(): string;
            public set ClassName(value: string);
            public ClearChildren () : void
            public Contains ($dom: Preactor.Dom) : boolean
            public AddEventListener ($name: string, $callback: UnityEngine.UIElements.EventCallback$1<UnityEngine.UIElements.EventBase>, $useCapture?: boolean) : void
            public RemoveEventListener ($name: string, $callback: UnityEngine.UIElements.EventCallback$1<UnityEngine.UIElements.EventBase>, $useCapture?: boolean) : void
            public AppendChild ($node: Preactor.Dom) : void
            public RemoveChild ($child: Preactor.Dom) : void
            public InsertBefore ($a: Preactor.Dom, $b: Preactor.Dom) : void
            public SetAttribute ($name: string, $val: any) : void
            public RemoveAttribute ($name: string) : void
            public Focus () : void
            public First ($predicate: System.Func$2<Preactor.Dom, boolean>) : Preactor.Dom
            public constructor ($visualElement: UnityEngine.UIElements.VisualElement, $document: Preactor.Document)
        }
        class DomStyle extends System.Object
        {
            protected [__keep_incompatibility]: never;
            public get VisualElementStyle(): UnityEngine.UIElements.IStyle;
            public get alignContent(): any;
            public set alignContent(value: any);
            public get alignItems(): any;
            public set alignItems(value: any);
            public get alignSelf(): any;
            public set alignSelf(value: any);
            public get backgroundColor(): any;
            public set backgroundColor(value: any);
            public get backgroundImage(): any;
            public set backgroundImage(value: any);
            public get backgroundSize(): any;
            public set backgroundSize(value: any);
            public get backgroundRepeat(): any;
            public set backgroundRepeat(value: any);
            public get backgroundPosition(): any;
            public set backgroundPosition(value: any);
            public get backgroundPositionX(): any;
            public set backgroundPositionX(value: any);
            public get backgroundPositionY(): any;
            public set backgroundPositionY(value: any);
            public get borderColor(): any;
            public set borderColor(value: any);
            public get borderWidth(): any;
            public set borderWidth(value: any);
            public get borderRadius(): any;
            public set borderRadius(value: any);
            public get borderTopColor(): any;
            public set borderTopColor(value: any);
            public get borderRightColor(): any;
            public set borderRightColor(value: any);
            public get borderBottomColor(): any;
            public set borderBottomColor(value: any);
            public get borderLeftColor(): any;
            public set borderLeftColor(value: any);
            public get borderTopWidth(): any;
            public set borderTopWidth(value: any);
            public get borderRightWidth(): any;
            public set borderRightWidth(value: any);
            public get borderBottomWidth(): any;
            public set borderBottomWidth(value: any);
            public get borderLeftWidth(): any;
            public set borderLeftWidth(value: any);
            public get borderTopLeftRadius(): any;
            public set borderTopLeftRadius(value: any);
            public get borderTopRightRadius(): any;
            public set borderTopRightRadius(value: any);
            public get borderBottomRightRadius(): any;
            public set borderBottomRightRadius(value: any);
            public get borderBottomLeftRadius(): any;
            public set borderBottomLeftRadius(value: any);
            public get bottom(): any;
            public set bottom(value: any);
            public get color(): any;
            public set color(value: any);
            public get cursor(): any;
            public set cursor(value: any);
            public get display(): any;
            public set display(value: any);
            public get flexBasis(): any;
            public set flexBasis(value: any);
            public get flexDirection(): any;
            public set flexDirection(value: any);
            public get flexGrow(): any;
            public set flexGrow(value: any);
            public get flexShrink(): any;
            public set flexShrink(value: any);
            public get flexWrap(): any;
            public set flexWrap(value: any);
            public get fontSize(): any;
            public set fontSize(value: any);
            public get height(): any;
            public set height(value: any);
            public get justifyContent(): any;
            public set justifyContent(value: any);
            public get left(): any;
            public set left(value: any);
            public get letterSpacing(): any;
            public set letterSpacing(value: any);
            public get margin(): any;
            public set margin(value: any);
            public get marginTop(): any;
            public set marginTop(value: any);
            public get marginRight(): any;
            public set marginRight(value: any);
            public get marginBottom(): any;
            public set marginBottom(value: any);
            public get marginLeft(): any;
            public set marginLeft(value: any);
            public get maxHeight(): any;
            public set maxHeight(value: any);
            public get maxWidth(): any;
            public set maxWidth(value: any);
            public get minHeight(): any;
            public set minHeight(value: any);
            public get minWidth(): any;
            public set minWidth(value: any);
            public get opacity(): any;
            public set opacity(value: any);
            public get overflow(): any;
            public set overflow(value: any);
            public get padding(): any;
            public set padding(value: any);
            public get paddingTop(): any;
            public set paddingTop(value: any);
            public get paddingRight(): any;
            public set paddingRight(value: any);
            public get paddingBottom(): any;
            public set paddingBottom(value: any);
            public get paddingLeft(): any;
            public set paddingLeft(value: any);
            public get position(): any;
            public set position(value: any);
            public get right(): any;
            public set right(value: any);
            public get rotate(): any;
            public set rotate(value: any);
            public get scale(): any;
            public set scale(value: any);
            public get textOverflow(): any;
            public set textOverflow(value: any);
            public get textShadow(): any;
            public set textShadow(value: any);
            public get top(): any;
            public set top(value: any);
            public get transformOrigin(): any;
            public set transformOrigin(value: any);
            public get transitionDelay(): any;
            public set transitionDelay(value: any);
            public get transitionDuration(): any;
            public set transitionDuration(value: any);
            public get transitionProperty(): any;
            public set transitionProperty(value: any);
            public get transitionTimingFunction(): any;
            public set transitionTimingFunction(value: any);
            public get translate(): any;
            public set translate(value: any);
            public get unityBackgroundImageTintColor(): any;
            public set unityBackgroundImageTintColor(value: any);
            public get unityBackgroundScaleMode(): any;
            public set unityBackgroundScaleMode(value: any);
            public get unityFont(): any;
            public set unityFont(value: any);
            public get unityFontDefinition(): any;
            public set unityFontDefinition(value: any);
            public get unityFontStyleAndWeight(): any;
            public set unityFontStyleAndWeight(value: any);
            public get unityOverflowClipBox(): any;
            public set unityOverflowClipBox(value: any);
            public get unityParagraphSpacing(): any;
            public set unityParagraphSpacing(value: any);
            public get unitySliceBottom(): any;
            public set unitySliceBottom(value: any);
            public get unitySliceLeft(): any;
            public set unitySliceLeft(value: any);
            public get unitySliceRight(): any;
            public set unitySliceRight(value: any);
            public get unitySliceTop(): any;
            public set unitySliceTop(value: any);
            public get unitySliceScale(): any;
            public set unitySliceScale(value: any);
            public get unityTextAlign(): any;
            public set unityTextAlign(value: any);
            public get unityTextOutlineColor(): any;
            public set unityTextOutlineColor(value: any);
            public get unityTextOutlineWidth(): any;
            public set unityTextOutlineWidth(value: any);
            public get unityTextOverflowPosition(): any;
            public set unityTextOverflowPosition(value: any);
            public get visibility(): any;
            public set visibility(value: any);
            public get whiteSpace(): any;
            public set whiteSpace(value: any);
            public get width(): any;
            public set width(value: any);
            public get wordSpacing(): any;
            public set wordSpacing(value: any);
            public SetProperty ($key: string, $value: any) : void
            public constructor ($dom: Preactor.Dom)
        }
        class InteropHost extends System.Object
        {
            protected [__keep_incompatibility]: never;
            public AddOrUpdateSubscribe ($store: any, $key: any, $handler: Function) : System.Action
            public Subscribe ($eventSource: any, $eventName: string, $handler: Function) : System.Action
            public Subscribe ($eventName: string, $handler: Function) : System.Action
            public AwaitTask ($task: System.Threading.Tasks.Task, $onCompleted: Function) : void
            public add_OnDestroy ($value: System.Action) : void
            public remove_OnDestroy ($value: System.Action) : void
            public constructor ($scriptEngine: Preactor.IScriptEngine)
        }
    }
}
