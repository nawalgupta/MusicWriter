using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class FunctionCodeTools
    {
        public ObservableList<IFunctionFactory> Factories { get; } =
            new ObservableList<IFunctionFactory>();

        public IFunction Parse(
                ref string source,
                AssortedFilesManager assortedfiles,
                List<KeyValuePair<Tuple<int, int>, string>> errors
            ) {
            IFunction context = null;

            while (!source.StartsWith(";")) {
                var name_length = source.IndexOfAny(" \r\f\n\v\t({;".ToCharArray());
                var name = source.Substring(0, name_length);
                source = source.Substring(name.Length);
                source = source.TrimStart();

                var num_arguments = new List<float>();
                var func_arguments = new List<IFunction>();
                string binary_key = null;

                if (source.StartsWith("(")) {
                    var inner_args_length = source.IndexOfAny(")".ToCharArray());
                    var inner_args = source.Substring(0, inner_args_length);
                    source = source.Substring(inner_args.Length + 1).TrimStart(); // removes the trailing parenthesi too

                    inner_args.Trim();

                    if (inner_args.Length > 0 && inner_args[0] == '@')
                        binary_key = inner_args.Substring(1);
                    else {
                        var args = inner_args.Split(',');

                        foreach (var arg in args)
                            num_arguments.Add(float.Parse(arg.Trim()));
                    }
                }

                if (source.StartsWith("{")) {
                    source = source.Substring(1).TrimStart();

                    while (!source.StartsWith("}")) {
                        var arg =
                            Parse(
                                    ref source,
                                    assortedfiles,
                                    errors
                                );

                        func_arguments.Add(arg);

                        if (source.StartsWith(";"))
                            source = source.Substring(1).TrimStart();
                    }

                    source = source.Substring(1).TrimStart();
                }

                var factory =
                    Factories.FirstOrDefault(fac => fac.CodeName == name);

                if (factory == null)
                    errors.Add(new KeyValuePair<Tuple<int, int>, string>(null, $"Codename \"{name}\" not recognized."));

                var file =
                    binary_key != null ?
                        assortedfiles[binary_key] :
                        null;

                context =
                    factory.Create(
                            context,
                            func_arguments?.ToArray(),
                            file,
                            num_arguments?.ToArray()
                        );
            }

            return context;
        }

        public void Render(
                StringBuilder builder,
                IFunction function,
                AssortedFilesManager assortedfiles,
                IStorageObject storage
            ) {
            var function_contextual =
                function as IContextualFunction;

            var function_mixing =
                function as IMixingFunction;

            var function_parameterized =
                function as IParamaterizedFunction;

            var function_storeddata =
                function as IStoredDataFunction;

            if (function_contextual != null) {
                Render(builder, function_contextual.Context, assortedfiles, storage);
                builder.Append(" ");
            }

            builder.Append(function.Factory.CodeName);

            if (function_storeddata != null) {
                builder.Append("(@");
                builder.Append(assortedfiles.GetName(function_storeddata.StorageObjectID));
                builder.Append(") ");
            }
            else if (function_parameterized != null) {
                builder.Append("(");

                for (int i = 0; i < function_parameterized.Arguments.Length; i++) {
                    builder.Append(function_parameterized.Arguments[i]);

                    if (i + 1 != function_parameterized.Arguments.Length)
                        builder.Append(", ");
                }

                builder.Append(") ");
            }

            if (function_mixing != null) {
                builder.Append("{");

                for (int i = 0; i < function_mixing.Arguments.Length; i++) {
                    Render(builder, function_mixing.Arguments[i], assortedfiles, storage);

                    if (i + 1 != function_mixing.Arguments.Length)
                        builder.AppendLine(";");
                }
            }
        }
    }
}
