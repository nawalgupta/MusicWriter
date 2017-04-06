using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class BackgroundWorker
    {
        readonly Action act;
        readonly Task task;
        readonly CancellationTokenSource tokensource = new CancellationTokenSource();

        public Action Act {
            get { return act; }
        }

        public BackgroundWorker(Action act) {
            this.act = act;

            task = new Task(act, tokensource.Token);
        }

        public void Start() {
            if (task.Status != TaskStatus.Created)
                throw new InvalidOperationException();

            task.Start();
        }

        public Task WaitForFinishAsync() => task;

        public void WaitForFinish() =>
            task.Wait();

        public void Stop() {
            tokensource.Cancel();
        }

        public static BackgroundWorker MakeWaitHandle() =>
            new BackgroundWorker(
                    () => {
                        while (true)
                            Thread.Sleep(20);
                    }
                );
    }
}
