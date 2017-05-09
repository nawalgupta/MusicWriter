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
        readonly bool allowrestarting;
        Task task;
        CancellationTokenSource tokensource;

        public Action Act {
            get { return act; }
        }

        public bool AllowRestarting {
            get { return allowrestarting; }
        }

        public BackgroundWorker(
                Action act,
                bool allowrestarting = false
            ) {
            this.act = act;
            this.allowrestarting = allowrestarting;

            Reset();
        }

        public void Start() {
            if (task.Status != TaskStatus.Created)
                throw new InvalidOperationException();

            task.Start();
        }

        public async Task WaitForFinishAsync() {
            try {
                await task;
            }
            catch (TaskCanceledException) {
            }
            catch {
                throw;
            }
        }

        public void WaitForFinish() =>
            WaitForFinishAsync().Wait();

        public void Stop() {
            tokensource.Cancel();

            if (allowrestarting)
                Reset();
        }

        void Reset() {
            tokensource = new CancellationTokenSource();
            task = new Task(act, tokensource.Token);
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
