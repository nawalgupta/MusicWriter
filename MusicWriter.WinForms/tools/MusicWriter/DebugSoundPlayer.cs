using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter.WinForms
{
    public sealed class DebugSoundPlayer
    {
        readonly DebugSound sound;
        readonly SoundPlayer player = new SoundPlayer();

        public DebugSound Sound {
            get { return sound; }
        }

        public bool IsPlaying { get; private set; }

        public event Action PlayingStarted;
        public event Action PlayingFinished;
        public event Action Stopped;

        public DebugSoundPlayer(DebugSound sound) {
            this.sound = sound;
        }
        
        public async void Play() {
            sound.StartRender();

            player.Stream = sound.RenderedWave.Value.OpenRead();
            player.Load();
            IsPlaying = true;
            PlayingStarted?.Invoke();
            await Task.Run(() => player.PlaySync());
            PlayingFinished?.Invoke();
        }

        public void Stop() {
            sound.StopRender();

            player.Stop();
            player.Stream.Close();
            player.Stream.Dispose();
            Stopped?.Invoke();
        }
    }
}
