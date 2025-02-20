using Avalonia.Controls;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;

namespace ADDSMock.Applications.Interactive.Views.Traffic
{
    internal partial class TrafficView : UserControl
    {
        public TrafficView()
        {
            InitializeComponent();

            Loaded += (sender, e) =>
            {
                var requestRegistry = new RegistryOptions(ThemeName.DarkPlus);
                var responseRegistry = new RegistryOptions(ThemeName.DarkPlus);

                var requestTextMate = _requestEditor.InstallTextMate(requestRegistry);
                var responseTextMate = _responseEditor.InstallTextMate(responseRegistry);

                requestTextMate.SetGrammar(requestRegistry.GetScopeByLanguageId(requestRegistry.GetLanguageByExtension(".json").Id));
                responseTextMate.SetGrammar(responseRegistry.GetScopeByLanguageId(responseRegistry.GetLanguageByExtension(".json").Id));
            };
        }
    }
}
