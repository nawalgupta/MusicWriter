using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class FunctionTrack : ITrack
    {
        public ITrackFactory Factory {
            get { return FactoryClass.Instance; }
        }

        public ObservableProperty<Time> Length { get; } =
            new ObservableProperty<Time>();

        public ObservableProperty<string> Name { get; } =
            new ObservableProperty<string>();

        public ObservableProperty<IFunction> Function { get; } =
            new ObservableProperty<IFunction>();
       
        public FunctionTrack(IFunction function = null) {
            Function.AfterChange += Function_AfterChange;

            if (function != null)
                Function.Value = function;
        }

        private void Function_AfterChange(IFunction old, IFunction @new) {
            Dirtied?.Invoke();
        }
        
        public event Action Dirtied;

        public sealed class FactoryClass : ITrackFactory
        {
            readonly FunctionCodeTools codetools; 

            public string Name {
                get { return "Function Track"; }
            }

            public FunctionCodeTools CodeTools {
                get { return codetools; }
            }

            public FactoryClass(FunctionCodeTools codetools) {
                this.codetools = codetools;
            }

            public ITrack Create() =>
                new FunctionTrack();

            public ITrack Load(Stream stream, IStorageObject storage) {
                var errors = new List<KeyValuePair<Tuple<int, int>, string>>();

                using (var tr = new StreamReader(stream)) {
                    var contents = tr.ReadToEnd();
                    var function = codetools.Parse(ref contents, storage, errors);

                    return new FunctionTrack(function);
                }
            }

            public void Save(ITrack track, Stream stream, IStorageObject storage) {
                var function = (track as FunctionTrack).Function.Value;

                using (var tw = new StreamWriter(stream)) {
                    var contents = new StringBuilder();
                    codetools.Render(contents, function, storage);

                    tw.Write(contents);
                }
            }

            private FactoryClass() { }

            public static readonly ITrackFactory Instance = new FactoryClass();
        }

        public object Copy(Duration window) {
            throw new InvalidOperationException();
        }

        public void Delete(Duration window) {
            throw new InvalidOperationException();
        }

        public void Erase(Duration window) {
            throw new InvalidOperationException();
        }

        public void Paste(object data, Time insert) {
            throw new InvalidOperationException();
        }
    }
}
