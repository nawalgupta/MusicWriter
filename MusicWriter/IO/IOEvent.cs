using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    [Flags]
    public enum IOEvent : byte
    {
        None = 0,

        EventClass = 0xF0,
        InternalEvent = 0x10,
        ChildEvent = 0x20,

        EventVerb = 0x0F,
        ConstructiveEvent = 0x01,
        DestructiveEvent = 0x02,
        PerspectiveEvent = 0x04,
        ManipulationEvent = 0x08,

        ObjectCreated = InternalEvent | ConstructiveEvent,
        ObjectDeleted = InternalEvent | DestructiveEvent,
        // an object itself cannot be rekeyed
        ObjectContentsSet = InternalEvent | ManipulationEvent,

        ChildAdded = ChildEvent | ConstructiveEvent,
        ChildRemoved = ChildEvent | DestructiveEvent,
        ChildRekeyed = ChildEvent | PerspectiveEvent,
        ChildContentsSet = ChildEvent | ManipulationEvent,
    }
}
