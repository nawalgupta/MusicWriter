using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class MusicTrackFunction : 
        IFunction,
        IContextualFunction
    {
        readonly MusicTrack track;
        readonly IFunction context;

        public MusicTrack Track {
            get { return track; }
        }

        public IFunction Context {
            get { return context; }
        }

        public IFunctionFactory Factory {
            get { return FactoryClass.Instance; }
        }

        public MusicTrackFunction(
                MusicTrack track,
                IFunction context
            ) {
            this.track = track;
            this.context = context;
        }

        public sealed class FactoryClass : IFunctionFactory
        {
            public bool AcceptsParameters {
                get { return false; }
            }

            public string CodeName {
                get { return "music"; }
            }

            public string FriendlyName {
                get { return "Music Track Function"; }
            }

            public bool StoresBinaryData {
                get { return true; }
            }

            private FactoryClass() {
            }

            public IFunction Create(
                    IFunction context = null,
                    IFunction[] args = null,
                    EditorFile file = null,
                    string key = null,
                    params double[] numbers
                ) =>
                new MusicTrackFunction(
                        file
                            [TrackControllerContainer.ItemName]
                            .As<IContainer, TrackControllerContainer>()
                            .Tracks
                            [key]
                            as MusicTrack,
                        context
                    );

            public static IFunctionFactory Instance { get; } =
                new FactoryClass();
        }

        public double GetValue(FunctionCall arg) {
            var time =
                track
                    .Tempo
                    .GetSongTime(arg.WaveTime);

            var sum = 0.0;

            foreach (var note in track.Melody.NotesInTime(time)) {
                var notestart =
                    track
                        .Tempo
                        .GetRealTime(note.Duration.Start);

                var noterealtime =
                    arg.WaveTime - notestart;

                var minicall = new FunctionCall(noterealtime, noterealtime, arg.RealTime);
                var local = context.GetValue(minicall);

                sum += local;
            }

            return sum;
        }
    }
}
