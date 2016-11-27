using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter.WinForms
{
    public sealed class Theme
    {
        public struct SheetMusicTheme
        {
            public struct CursorTheme
            {
                public Color CaretColor;
                public float CaretWidth;

                public Color ExtensionLineColor;
                public float ExtensionLineWidth;
            }

            public struct TrackTheme
            {
                public Color Background;
                public Color Foreground;
            }

            public CursorTheme CursorActive;
            public CursorTheme CursorPassive;
            public TrackTheme TrackActive;
            public TrackTheme TrackPassive;
        }

        public SheetMusicTheme SheetMusicActive;
        public SheetMusicTheme SheetMusicPassive;

        public static Theme Default = new Theme {
            SheetMusicActive = new SheetMusicTheme {
                CursorActive = new SheetMusicTheme.CursorTheme {
                    CaretColor = Color.LawnGreen.Alpha(0.65),
                    CaretWidth = 3.2f,
                    ExtensionLineColor = Color.Red.Alpha(0.6),
                    ExtensionLineWidth = 2.8f
                },
                CursorPassive = new SheetMusicTheme.CursorTheme {
                    CaretColor = Color.DarkSeaGreen.Alpha(0.5),
                    CaretWidth = 2.6f,
                    ExtensionLineColor = Color.IndianRed.Alpha(0.45),
                    ExtensionLineWidth = 2f
                },
                TrackActive = new SheetMusicTheme.TrackTheme {
                    Background = Color.Khaki,
                    Foreground = Color.Black
                },
                TrackPassive = new SheetMusicTheme.TrackTheme {
                    Background = Color.Beige,
                    Foreground = Color.Black
                }
            },

            SheetMusicPassive = new SheetMusicTheme {
                CursorActive = new SheetMusicTheme.CursorTheme {
                    CaretColor = Color.DarkSeaGreen.Alpha(0.5),
                    CaretWidth = 2.6f,
                    ExtensionLineColor = Color.IndianRed.Alpha(0.45),
                    ExtensionLineWidth = 2f
                },
                CursorPassive = new SheetMusicTheme.CursorTheme {
                    CaretColor = Color.DarkSeaGreen.Alpha(0.5),
                    CaretWidth = 2.6f,
                    ExtensionLineColor = Color.IndianRed.Alpha(0.45),
                    ExtensionLineWidth = 2f
                },
                TrackActive = new SheetMusicTheme.TrackTheme {
                    Background = Color.Beige,
                    Foreground = Color.Black
                },
                TrackPassive = new SheetMusicTheme.TrackTheme {
                    Background = Color.Beige,
                    Foreground = Color.Black
                }
            }
        };
    }
}
