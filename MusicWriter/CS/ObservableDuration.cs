using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class ObservableDuration : Duration
    {
        public ObservableProperty<Time> OffsetProperty { get; } =
            new ObservableProperty<Time>();

        public ObservableProperty<Time> LengthProperty { get; } =
            new ObservableProperty<Time>();

        public override Time Length {
            get { return LengthProperty.Value; }
            set { LengthProperty.Value = value; }
        }

        public override Time Offset {
            get { return OffsetProperty.Value; }
            set { OffsetProperty.Value = value; }
        }
    }
}
