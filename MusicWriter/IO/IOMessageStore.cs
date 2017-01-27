using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class IOMessageStore {
        readonly IObservableList<IOMessage> messages;
        readonly IObservableList<IOListener> listeners;
        readonly List<int> readylisteners = new List<int>();

        public IObservableList<IOMessage> Messages {
            get { return messages; }
        }

        public IObservableList<IOListener> Listeners {
            get { return listeners; }
        }

        public IOMessageStore(
                IObservableList<IOMessage> messages,
                IObservableList<IOListener> listeners
            ) {
            this.messages = messages;
            this.listeners = listeners;

            messages.ItemInserted += Messages_ItemInserted;
            messages.ItemRemoved += Messages_ItemRemoved;

            listeners.ItemInserted += Listeners_ItemInserted;
            listeners.ItemWithdrawn += Listeners_ItemWithdrawn;
        }

        private void Messages_ItemInserted(IOMessage msg, int i) {
            for (int j = 0; j < listeners.Count; j++) {
                var listener = listeners[j];

                while (readylisteners.Count <= j)
                    Thread.Sleep(20);

                var readystate = readylisteners[j];
                if (readystate < i)
                    listener.Process(msg);
            }
        }

        private void Messages_ItemRemoved(IOMessage msg) {
            throw new InvalidOperationException();
        }

        private void Listeners_ItemInserted(IOListener listener, int i) {
            var j = messages.Count;

            readylisteners.Insert(i, j);
            for (; j >= 0; j--)
                listener.Process(messages[j]);
        }

        private void Listeners_ItemWithdrawn(IOListener listener, int i) {
            //this is unsafe
            readylisteners.RemoveAt(i);
        }
    }
}
