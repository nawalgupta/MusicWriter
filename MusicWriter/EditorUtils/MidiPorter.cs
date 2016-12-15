using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MusicWriter.TimeSignature;

namespace MusicWriter
{
	public sealed class MidiPorter : IPorter
	{
		public string Name {
			get { return "MIDI Files"; }
		}

		public string FileExtension {
			get { return "*.mid"; }
		}

		public void Export<View>(EditorFile<View> editor, string filename) {
			throw new NotImplementedException();
		}

		public void Import<View>(EditorFile<View> editor, string filename) {
			var midifile = new MidiFile(filename);

			for (int track_index = 0; track_index < midifile.Tracks; track_index++) {
				var events = midifile.Events.GetTrackEvents(track_index);

				var track =
					MusicTrack.Create();

				foreach (var e in events) {
					switch (e.CommandCode) {
						case MidiCommandCode.NoteOn: {
								var e1 = (NoteOnEvent)e;

								var start =
									ImportTime(e1.AbsoluteTime, midifile);

								var length =
									ImportTime(e1.NoteLength, midifile);

								var duration =
									new Duration {
										Start = start,
										Length = length
									};

								track.Melody.AddNote(new SemiTone(e1.NoteNumber - 12), duration);

								break;
							}

						case MidiCommandCode.NoteOff: {
								var e1 = (NoteEvent)e;

								break;
							}

						case MidiCommandCode.MetaEvent: {
								var meta = (MetaEvent)e;

								switch (meta.MetaEventType) {
									case MetaEventType.Copyright:

										break;

									case MetaEventType.CuePoint:

										break;

									case MetaEventType.DeviceName:

										break;

									case MetaEventType.EndTrack:

										break;

									case MetaEventType.KeySignature:

										break;

									case MetaEventType.Lyric:

										break;

									case MetaEventType.Marker:

										break;

									case MetaEventType.MidiChannel:

										break;

									case MetaEventType.MidiPort:

										break;

									case MetaEventType.ProgramName:

										break;

									case MetaEventType.SequencerSpecific:
										break;

									case MetaEventType.SequenceTrackName: {
											var text = (TextEvent)meta;

											track.Name.Value = text.Text;

											break;
										}

									case MetaEventType.SetTempo:

										break;

									case MetaEventType.SmpteOffset:

										break;

									case MetaEventType.TextEvent:
										break;

									case MetaEventType.TimeSignature:
										var timesigevent = (TimeSignatureEvent)meta;

										var timesig =
											new TimeSignature(
													new Simple(
															timesigevent.Numerator,
															timesigevent.Denominator
														)
												);

										track
											.Rhythm
											.TimeSignatures
											.ScootAndOverwrite(
													timesig,
													new Duration {
														Start = ImportTime(timesigevent.AbsoluteTime, midifile),
														Length = Time.Eternity
													}
												);

										break;

									case MetaEventType.TrackInstrumentName:

										break;

									case MetaEventType.TrackSequenceNumber:

										break;

									default:
										throw new InvalidOperationException();
								}

								break;
							}

						case MidiCommandCode.PatchChange: {
								

								break;
							}

						case MidiCommandCode.ControlChange: {

								break;
							}

						default:
							throw new InvalidOperationException();
					}
				}

				editor.Tracks.Add(track);
			}
		}

		Time ImportTime(long ticks, MidiFile file) =>
			Time.Fraction(ticks, file.DeltaTicksPerQuarterNote * 4);
	}
}
