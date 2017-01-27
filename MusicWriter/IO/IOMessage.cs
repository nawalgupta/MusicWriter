using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public struct IOMessage
    {
        public readonly StorageObjectID Subject;
        public readonly IOEvent Verb;
        public readonly string Relation;
        public readonly string NewRelation;
        public readonly StorageObjectID Object;

        public IOMessage(
                StorageObjectID subject,
                IOEvent verb
            )
            : this(
                  subject,
                  verb,
                  default(string),
                  default(string),
                  default(StorageObjectID)
                 ) {
        }

        public IOMessage(
                StorageObjectID subject,
                IOEvent verb,
                string relation,
                StorageObjectID @object
            )
            : this(
                    subject,
                    verb,
                    relation,
                    default(string),
                    @object
                 ) {
        }

        public IOMessage(
                StorageObjectID subject,
                IOEvent verb,
                string relation,
                string newrelation,
                StorageObjectID @object
            ) {
            Subject = subject;
            Verb = verb;
            Relation = relation;
            NewRelation = newrelation;
            Object = @object;
        }

        public bool Matches(IOMessage filter) =>
            Verb == filter.Verb &&
            (Subject == filter.Subject || filter.Subject == StorageObjectID.Any) &&
            ((Verb & IOEvent.ChildEvent) == IOEvent.ChildEvent ?
                ((Object == filter.Object || filter.Object == StorageObjectID.Any) &&
                (NewRelation == filter.NewRelation) || filter.NewRelation == default(string) &&
                (Relation == filter.Relation) || filter.Relation == default(string)) 
            : true);
    }
}
