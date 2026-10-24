using BlazorMonaco.Editor;
using JEO3.Core;
using Microsoft.AspNetCore.Components;

namespace JEO3.Site.Dashboard
{
    public abstract class EntityViewerBase : ComponentBase
    {
        #region Properties
        [Inject] protected WSWorkspace W { get; set; }
        protected StandaloneCodeEditor queryEditor;
        #endregion

        #region Initialization
        protected virtual StandaloneEditorConstructionOptions EditorOptions(StandaloneCodeEditor editor)
        {
            var br = "\r\n";
            var defaultText = br + br + br + br + br + "\t// Click on a procedure to get started :) ...  " + br + br + br + br + br
                + string.Join(br, A5C11.GetAsciiArt("JEO3.com", false).Split('\n').Select(v => " " + v.Replace("\r", "").Replace(" ", " ")));
            return new StandaloneEditorConstructionOptions
            {
                AutomaticLayout = true,
                Value = defaultText,
                Minimap = new EditorMinimapOptions { Enabled = false },
                FontSize = 14,
                FontFamily = "Consolas, Cascadia Mono, Cascadia Code, Courier New, monospace",
                Language = "sql",
                Theme = "vs",
                //LineNumbers = "off",
                //RenderLineHighlight = "none",
                Guides = new GuidesOptions() { Indentation = false },
                MouseWheelZoom = true,
                CodeLens = false,
                SnippetSuggestions = "none",
                QuickSuggestions = new QuickSuggestionsOptions { Other = "false", Comments = "false", Strings = "false" }
            };
        }


        #endregion
        #region Functions
        protected abstract Task UpdateQuery((object? a, object? b) x);
        #endregion
    }
}
