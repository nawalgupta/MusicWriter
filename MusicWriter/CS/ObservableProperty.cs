using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class ObservableProperty<T>
    {
        T value = default(T);

        public sealed class PropertyChangingEventArgs : EventArgs
        {
            readonly T oldvalue;

            public T OldValue {
                get { return oldvalue; }
            }
            public T NewValue { get; set; } = default(T);

            public bool Altered { get; set; } = false;
            public bool Canceled { get; set; } = false;

            public PropertyChangingEventArgs(T oldvalue) {
                this.oldvalue = oldvalue;
            }

            public PropertyChangingEventArgs(T oldvalue, T newvalue)
                : this(oldvalue) {
                NewValue = newvalue;
            }
        }

        public delegate void PropertyChangeHandler(T old, T @new);
        public delegate void PropertyChangingHandler(PropertyChangingEventArgs args);

        public event PropertyChangingHandler BeforeChange;
        public event PropertyChangeHandler AfterChange;

        public ObservableProperty() {
        }

        public ObservableProperty(T value) {
            this.value = value;
        }

        volatile int changedstate = 0;
        public T Value {
            get { return value; }
            set {
                var old = this.value;

                if (old == null && value == null) {
                }
                else if (old == null ||
                    value == null ||
                    !value.Equals(old)) {
                    ++changedstate;
                    if (BeforeChange != null) {
                        var args =
                            new PropertyChangingEventArgs(old, value);

                        do {
                            args.Altered = false;

                            foreach (PropertyChangingHandler handler in BeforeChange.GetInvocationList()) {
                                handler(args);

                                if (args.Canceled)
                                    goto end;
                            }
                        } while (args.Altered);
                        
                        value = args.NewValue;
                    }

                    this.value = value;

                    if (changedstate == 1)
                        AfterChange?.Invoke(old, value);
                    else {
                        //throw new InvalidOperationException();
                        // the value was changed while it was changing
                    }
                    
                end:
                    changedstate--;
                }
            }
        }

        public override string ToString() =>
            value.ToString();
    }
}
