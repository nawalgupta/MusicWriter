using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class Property : IEquatable<Property>
    {
        readonly string name;
        readonly object @default;
        readonly int id;

        public string Name {
            get { return name; }
        }

        public object Default {
            get { return @default; }
        }

        public int ID {
            get { return id; }
        }

        public Property(
                string name,
                object @default,
                int id
            ) {
            this.name = name;
            this.@default = @default;
            this.id = id;
        }

        public override int GetHashCode() => id;

        public override bool Equals(object that) =>
            ReferenceEquals(this, that);

        public bool Equals(Property that) =>
            ReferenceEquals(this, that);

        public override string ToString() => name;

        public static bool operator ==(Property a, Property b) =>
            ReferenceEquals(a, b);

        public static bool operator !=(Property a, Property b) =>
            !ReferenceEquals(a, b);
    }
}
