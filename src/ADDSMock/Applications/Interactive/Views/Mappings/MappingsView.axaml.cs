using Avalonia.Controls;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;

namespace ADDSMock.Applications.Interactive.Views.Mappings
{
    internal partial class MappingsView : UserControl
    {
        private TextMate.Installation _outputTextMate;
        private RegistryOptions _outputRegistry;

        public MappingsView()
        {
            InitializeComponent();

            Loaded += (sender, e) =>
            {
                var registry = new RegistryOptions(ThemeName.DarkPlus);
                _outputRegistry = new RegistryOptions(ThemeName.DarkPlus);

                var editorTextMate = _editor.InstallTextMate(registry);
                _outputTextMate = _outputEditor.InstallTextMate(_outputRegistry);

                editorTextMate.SetGrammar(registry.GetScopeByLanguageId(registry.GetLanguageByExtension(".cs").Id));

                ((MappingsModel)DataContext!).SetEditorActions(SetOutputToJson, SetOutputToText);
            };
        }

        private void SetOutputToJson()
        {
            _outputTextMate.SetGrammar(_outputRegistry.GetScopeByLanguageId(_outputRegistry.GetLanguageByExtension(".json").Id));
        }

        private void SetOutputToText()
        {
            _outputTextMate.SetGrammar(_outputRegistry.GetScopeByLanguageId(_outputRegistry.GetLanguageByExtension(".adoc").Id));
        }
    }
}
