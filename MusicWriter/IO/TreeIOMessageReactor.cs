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

                indicies_listeners = lookup_subject_this.Key.AllOnes(lookup_verb_this.Key).ToArray();
                
                foreach (var subject in lookup_subject)
                    subject.Value.Value.Insert(i, subject.Key == msg.Subject);

                foreach (var verb in lookup_verb)
                    verb.Value.Value.Insert(i, verb.Key == msg.Verb);
            }
            
            for (int j = 0; j < indicies_listeners.Length; j++)
                Listeners[indicies_listeners[j]].Responder(msg);
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
            }

            for (int j = 0; j < indicies_msg.Length; j++)
                listener.Responder(Messages[indicies_msg[j]]);
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