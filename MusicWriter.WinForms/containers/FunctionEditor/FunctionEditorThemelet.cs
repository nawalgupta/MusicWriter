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

        static FunctionEditorThemelet() {
            Path.Set += Path_Set;
            Theme.RegisterThemelet("function", path => Path.Value = path);

            CodePaint.Set += CodePaint_Set;
            CodeFont.Set += CodeFont_Set;
        }

        private static void Path_Set(string value) {
            var codefont = Theme.DeserializeFont(System.IO.Path.Combine(Path.Value, "code.font"));
            var codepaint = Theme.DeserializePaint(System.IO.Path.Combine(Path.Value, "code.paint"));

            if (codefont == null)
                CodeFont_Set(CodeFont.Value);
            else CodeFont.Value = codefont;

            if (codepaint == null)
                CodePaint_Set(CodePaint.Value);
            else CodePaint.Value = codepaint;
        }

        private static void CodePaint_Set(Theme.Paint value) {
            Theme.SerializePaint(System.IO.Path.Combine(Path.Value, "code.paint"), CodePaint.Value);
        }

        private static void CodeFont_Set(Font value) {
            Theme.SerializeFont(System.IO.Path.Combine(Path.Value, "code.font"), CodeFont.Value);
        }
    }
}
