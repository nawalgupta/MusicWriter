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

		public void Export<View>(
                EditorFile<View> editor,
                string filename,
                PorterOptions options
            ) {
			throw new NotImplementedException();
		}

		public void Import<View>(
                EditorFile<View> editor,
                string filename,
                PorterOptions options
            ) {
			var midifile = new MidiFile(filename, false);

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

									case MetaEventType.KeySignature: {
                                            var keysigevent = (KeySignatureEvent)meta;

                                            var circle5index = (sbyte)keysigevent.SharpsFlats;

                                            Mode mode;

                                            switch (keysigevent.MajorMinor) {
                                                case 1:
                                                    mode = Mode.Major;
                                                    break;

                                                case 0:
                                                    mode = Mode.Minor;
                                                    break;

                                                default:
                                                    throw new InvalidOperationException();
                                            }

                                            PitchTransform transform;
                                            var key =
                                                CircleOfFifths.Index(circle5index, ChromaticPitchClass.C, out transform);

                                            var sig =
                                                KeySignature.Create(
                                                        key,
                                                        transform,
                                                        mode
                                                    );

                                            var start =
                                                ImportTime(keysigevent.AbsoluteTime, midifile);
                                            
                                            track
                                                .Adornment
                                                .KeySignatures
                                                .OverwriteEverythingToRight(
                                                        sig,
                                                        start
                                                    );

                                            break;
                                        }

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
                                        var tempoevent = meta as TempoEvent;
                                        // Can midi files have linear varying tempos?
                                        // if so, then this code doesn't handle all midis.

                                        if (options.PortTempo) {
                                            editor
                                                .Tempo
                                                .AddConstant(
                                                        ImportTime(tempoevent.AbsoluteTime, midifile).Notes,
                                                        60 / (float)tempoevent.Tempo
                                                    );
                                        }

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
											.OverwriteEverythingToRight(
													timesig,
                                                    ImportTime(timesigevent.AbsoluteTime, midifile)
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
