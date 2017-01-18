using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class ObservableProperty<T> {
        T value = default(T);

        public delegate void PropertyChangeHandler(T old, T @new);

        public event PropertyChangeHandler BeforeChange;
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

                if (ReferenceEquals(old, null) ||
                    ReferenceEquals(value, null) ||
                    !old.Equals(value)) {
                    ++changedstate;
                    BeforeChange?.Invoke(old, value);

                    this.value = value;

                    if (changedstate == 1)
                        AfterChange?.Invoke(old, value);

                    changedstate--;
                }
            }
        }

        public override string ToString() =>
            value.ToString();
    }
}
