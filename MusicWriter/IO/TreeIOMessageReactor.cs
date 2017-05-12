using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MusicWriter
{
    public sealed class TreeIOMessageReactor : IIOMessageReactor
    {
        // [indicies for listeners ; indicies for messages]
        readonly IObservableList<IOListener> listeners;
        readonly IObservableList<IOMessage> messages;

        readonly Dictionary<StorageObjectID, KeyValuePair<ShiftableBitArray, ShiftableBitArray>> lookup_subject =
            new Dictionary<StorageObjectID, KeyValuePair<ShiftableBitArray, ShiftableBitArray>>();

        readonly Dictionary<IOEvent, KeyValuePair<ShiftableBitArray, ShiftableBitArray>> lookup_verb =
            new Dictionary<IOEvent, KeyValuePair<ShiftableBitArray, ShiftableBitArray>>();

        readonly object locker = new object();

        public IObservableList<IOListener> Listeners {
            get { return listeners; }
        }

        public IObservableList<IOMessage> Messages {
            get { return messages; }
        }

        public TreeIOMessageReactor(
                IObservableList<IOMessage> messages,
                IObservableList<IOListener> listeners
            ) {
            this.messages = messages;
            this.listeners = listeners;

            messages.ItemInserted += Messages_ItemInserted;
            messages.ItemWithdrawn += Messages_ItemWithdrawn;

            listeners.ItemInserted += Listeners_ItemInserted;
            listeners.ItemWithdrawn += Listeners_ItemWithdrawn;
        }

        private void Messages_ItemInserted(IOMessage msg, int i) {
            int[] indicies_listeners;

            lock (locker) {
                var lookup_subject_this =
                    lookup_subject.Lookup(
                            msg.Subject,
                            () =>
                                new KeyValuePair<ShiftableBitArray, ShiftableBitArray>(
                                        new ShiftableBitArray(),
                                        new ShiftableBitArray()
                                    )
                        );

                var lookup_verb_this =
                    lookup_verb.Lookup(
                            msg.Verb,
                            () =>
                                new KeyValuePair<ShiftableBitArray, ShiftableBitArray>(
                                        new ShiftableBitArray(),
                                        new ShiftableBitArray()
                                    )
                        );

                //TODO: when it crashes, I wonder how the indicies_listeners werer computed.
                indicies_listeners = lookup_subject_this.Key.AllOnes(lookup_verb_this.Key).ToArray();
                var listeners = indicies_listeners.Select(index => Listeners[index]).ToArray();

                if (indicies_listeners.Contains(77)) {
                    Console.WriteLine();
                }

                foreach (var subject in lookup_subject)
                    subject.Value.Value.Insert(i, subject.Key == msg.Subject);

                foreach (var verb in lookup_verb)
                    verb.Value.Value.Insert(i, verb.Key == msg.Verb);

                for (int j = 0; j < listeners.Length; j++) {
                    var listener =
                        listeners[j];

                    if (listener.Filter.Relation != msg.Relation && listener.Filter.Relation != null)
                        continue;

                    if (listener.Filter.NewRelation != msg.NewRelation && listener.Filter.NewRelation != null)
                        continue;

                    if (listener.Filter.Object != msg.Object && listener.Filter.Object != default(StorageObjectID))
                        continue;

                    listener.Responder(msg);
                }
            }
        }

        private void Messages_ItemWithdrawn(IOMessage msg, int i) {
            lock (locker) {
                foreach (var subject in lookup_subject)
                    subject.Value.Value.Withdraw(i);

                foreach (var verb in lookup_verb)
                    verb.Value.Value.Withdraw(i);
            }
        }

        private void Listeners_ItemInserted(IOListener listener, int i) {
            int[] indicies_msg;

            lock (locker) {
                var lookup_subject_this =
                    lookup_subject.Lookup(
                            listener.Filter.Subject,
                            () =>
                                new KeyValuePair<ShiftableBitArray, ShiftableBitArray>(
                                        new ShiftableBitArray(),
                                        new ShiftableBitArray()
                                    )
                        );

                var lookup_verb_this =
                    lookup_verb.Lookup(
                            listener.Filter.Verb,
                            () =>
                                new KeyValuePair<ShiftableBitArray, ShiftableBitArray>(
                                        new ShiftableBitArray(),
                                        new ShiftableBitArray()
                                    )
                        );

                foreach (var subject in lookup_subject)
                    subject.Value.Key.Insert(i, subject.Key == listener.Filter.Subject);

                foreach (var verb in lookup_verb)
                    verb.Value.Key.Insert(i, verb.Key == listener.Filter.Verb);

                indicies_msg = lookup_subject_this.Value.AllOnes(lookup_verb_this.Value).ToArray();

                for (int j = 0; j < indicies_msg.Length; j++) {
                    var msg = Messages[indicies_msg[j]];

                    if (listener.Filter.Relation != msg.Relation && listener.Filter.Relation != null)
                        continue;

                    if (listener.Filter.NewRelation != msg.NewRelation && listener.Filter.NewRelation != null)
                        continue;

                    if (listener.Filter.Object != msg.Object && listener.Filter.Object != default(StorageObjectID))
                        continue;

                    listener.Responder(msg);
                }
            }
        }

        private void Listeners_ItemWithdrawn(IOListener listener, int i) {
            lock (locker) {
                var lookup_subject_this =
                    lookup_subject[listener.Filter.Subject];

                var lookup_verb_this =
                    lookup_verb[listener.Filter.Verb];

                foreach (var subject in lookup_subject)
                    subject.Value.Key.Withdraw(i);

                foreach (var verb in lookup_verb)
                    verb.Value.Key.Withdraw(i);
            }
        }
    }
}