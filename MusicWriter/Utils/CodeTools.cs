using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public static class CodeTools
    {
        public static Duration ReadDuration(string value) {
            if (string.IsNullOrWhiteSpace(value))
                return Duration.Empty;

            var split = value.Split('+');

            return new Duration {
                Start = ReadTime(split[0]),
                Length = ReadTime(split[1])
            };
        }

        public static string WriteDuration(Duration duration) =>
            $"{WriteTime(duration.Start)}+{WriteTime(duration.Length)}";

        public static Time ReadTime(string value) =>
            !string.IsNullOrWhiteSpace(value) ?
                Time.FromTicks(int.Parse(value)) :
                Time.Zero;

        public static string WriteTime(Time time) =>
            time.Ticks.ToString();

        public static IEnumerable<KeyValuePair<string, string>> ReadKVPs(Stream stream) {
            using (var tr = new StreamReader(stream, Encoding.Unicode, false, 1024, false)) {
                string line;

                while ((line = tr.ReadLine()) != null) {
                    var name = line;
                    var type = tr.ReadLine();

                    if (type == null)
                        break;

                    yield return new KeyValuePair<string, string>(name, type);
                }
            }
        }
        
        public static void WriteKVPs(Stream stream, IEnumerable<KeyValuePair<string, string>> kvps) {
            using (var tw = new StreamWriter(stream, Encoding.Unicode, 1024, false)) {
                foreach (var kvp in kvps) {
                    tw.WriteLine(kvp.Key);
                    tw.WriteLine(kvp.Value);
                }
            }
        }
    }
}
