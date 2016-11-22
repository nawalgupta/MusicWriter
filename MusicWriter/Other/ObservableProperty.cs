﻿using System;
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

        public T Value {
            get { return value; }
            set {
                var old = this.value;

                BeforeChange?.Invoke(old, value);

                this.value = value;

                AfterChange?.Invoke(old, value);
            }
        }

        public override string ToString() =>
            value.ToString();
    }
}