using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class AggregateDurationField<T> :
        IDurationField<T> {
        readonly IDurationField<T>[] fields;

        public IDurationField<T>[] Fields {
            get { return fields; }
        }

        public AggregateDurationField(params IDurationField<T>[] fields) {
            this.fields = fields;
        }

        public IEnumerable<IDuratedItem<T>> Intersecting(Duration duration) =>
            fields
                .SelectMany(
                        field => field.Intersecting(duration)
                    );

        public IEnumerable<IDuratedItem<T>> Intersecting(Time point) =>
            fields
                .SelectMany(
                        field => field.Intersecting(point)
                    );
        }
}
