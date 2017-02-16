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
                if (!lookup_subject.ContainsKey(msg.Subject) ||
                    !lookup_verb.ContainsKey(msg.Verb))
                    return;

                var lookup_subject_this =
                    lookup_subject[msg.Subject];

                var lookup_verb_this =
                    lookup_verb[msg.Verb];

                lookup_subject_this.Value.Insert(i, true);
                lookup_verb_this.Value.Insert(i, true);

                indicies_listeners = lookup_subject_this.Key.AllOnes(lookup_verb_this.Key).ToArray();
            }

            for (int j = 0; j < indicies_listeners.Length; j++)
                Listeners[indicies_listeners[j]].Responder(msg);
        }

        private void Messages_ItemWithdrawn(IOMessage msg, int i) {
            lock (locker) {
                if (!lookup_subject.ContainsKey(msg.Subject) ||
                    !lookup_verb.ContainsKey(msg.Verb))
                    return;

                var lookup_subject_this =
                    lookup_subject[msg.Subject];

                var lookup_verb_this =
                    lookup_verb[msg.Verb];

                lookup_subject_this.Value.Withdraw(i);
                lookup_verb_this.Value.Withdraw(i);
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

                lookup_subject_this.Key.Insert(i, true);
                lookup_verb_this.Key.Insert(i, true);

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

                lookup_subject_this.Key.Withdraw(i);
                lookup_verb_this.Key.Withdraw(i);
            }
        }
    }
}