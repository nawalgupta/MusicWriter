using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class FunctionCodeTools
    {
        public List<IFunctionFactory> Factories { get; } =
            new List<IFunctionFactory>();

        public IFunction Parse(
                ref string source,
                IStorageObject storage,
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
                        var arg = Parse(ref source, storage, errors);
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

                if (func_arguments.Count == 0 && num_arguments.Count == 0 && binary_key == null) {
                    if (context != null)
                        context = factory.Create(context);
                    else context = factory.Create();
                }
                else if (binary_key != null) {
                    var stream =
                        storage.Read(binary_key);

                    if (context == null) {
                        if (func_arguments.Count == 0)
                            context =
                                factory.Deserialize(stream);
                        else
                            context =
                                factory.Deserialize(
                                        stream,
                                        func_arguments.ToArray()
                                    );
                    }
                    else {
                        if (func_arguments.Count == 0)
                            context =
                                factory.Deserialize(
                                        stream,
                                        context
                                    );
                        else context =
                                factory.Deserialize(
                                        stream,
                                        context,
                                        func_arguments.ToArray()
                                    );
                    }
                }
                else if (func_arguments.Count != 0) {
                    if (context == null) {
                        if (num_arguments.Count == 0)
                            context =
                                factory.Create(
                                        func_arguments.ToArray()
                                    );
                        else context =
                                factory.Create(
                                        func_arguments.ToArray(),
                                        num_arguments.ToArray()
                                    );
                    }
                    else {
                        if (num_arguments.Count == 0)
                            context =
                                factory.Create(
                                        context,
                                        func_arguments.ToArray()
                                    );
                        else context =
                                factory.Create(
                                        context,
                                        func_arguments.ToArray(),
                                        num_arguments.ToArray()
                                    );
                    }
                }
                else {
#if DEBUG
                    if (num_arguments.Count == 0)
                        throw new Exception("This shouldn't happen");
#endif
                    if (context == null)
                        context = factory.Create(num_arguments.ToArray());
                    else context = factory.Create(context, num_arguments.ToArray());
                }
            }

            return context;
        }

        public void Render(
                StringBuilder builder,
                IFunction function,
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
                Render(builder, function_contextual.Context, storage);
                builder.Append(" ");
            }

            builder.Append(function.Factory.CodeName);
            
            if (function_storeddata != null) {
                builder.Append("(@");
                builder.Append(function_storeddata.Name);
                builder.Append(") ");
            }
            else if(function_parameterized != null) {
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
                    Render(builder, function_mixing.Arguments[i], storage);

                    if (i + 1 != function_mixing.Arguments.Length)
                        builder.AppendLine(";");
                }
            }
        }
    }
}
