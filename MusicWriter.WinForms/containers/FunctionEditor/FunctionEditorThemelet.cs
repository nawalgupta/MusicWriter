using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter.WinForms
{
    public static class FunctionEditorThemelet
    {
        public static ObservableProperty<string> Path { get; } =
            new ObservableProperty<string>();

        public static ObservableProperty<Theme.Paint> CodePaint { get; } = 
            new ObservableProperty<Theme.Paint>(new Theme.Paint(SystemColors.ControlText, SystemColors.Window));

        public static ObservableProperty<Font> CodeFont { get; } = 
            new ObservableProperty<Font>(SystemFonts.DefaultFont);

        public static ObservableProperty<float> SplitterFraction { get; } =
            new ObservableProperty<float>(0.5f);

        static FunctionEditorThemelet() {
            Path.Set += Path_Set;
            Theme.RegisterThemelet("function", path => Path.Value = path);

            CodePaint.Set += CodePaint_Set;
            CodeFont.Set += CodeFont_Set;
            SplitterFraction.Set += SplitterFraction_Set;
            SplitterFraction.BeforeChange += SplitterFraction_BeforeChange;
        }

        private static void SplitterFraction_BeforeChange(ObservableProperty<float>.PropertyChangingEventArgs args) {
            if (args.NewValue < 0) {
                args.NewValue = 0;
                args.Altered = true;
            }

            if (args.NewValue > 1) {
                args.NewValue = 1;
                args.Altered = true;
            }
        }

        private static void Path_Set(string value) {
            var codefont = Theme.DeserializeFont(System.IO.Path.Combine(Path.Value, "code.font"));
            var codepaint = Theme.DeserializePaint(System.IO.Path.Combine(Path.Value, "code.paint"));
            var splitterfraction = Theme.DeserializeFloat(System.IO.Path.Combine(Path.Value, "splitter.fraction"));
            
            if (codefont == null)
                CodeFont_Set(CodeFont.Value);
            else CodeFont.Value = codefont;

            if (codepaint == null)
                CodePaint_Set(CodePaint.Value);
            else CodePaint.Value = codepaint;

            if (splitterfraction == null)
                SplitterFraction_Set(SplitterFraction.Value);
            else SplitterFraction.Value = splitterfraction.Value;
        }

        private static void CodePaint_Set(Theme.Paint value) {
            Theme.SerializePaint(System.IO.Path.Combine(Path.Value, "code.paint"), CodePaint.Value);
        }

        private static void CodeFont_Set(Font value) {
            Theme.SerializeFont(System.IO.Path.Combine(Path.Value, "code.font"), CodeFont.Value);
        }

        private static void SplitterFraction_Set(float value) {
            Theme.SerializeFloat(System.IO.Path.Combine(Path.Value, "splitter.fraction"), SplitterFraction.Value);
        }
    }
}
