﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public static class Extensions {
        public static ChromaticPitchClass GetPitchClass(this DiatonicToneClass key) {
            switch (key) {
                case DiatonicToneClass.C:
                    return ChromaticPitchClass.C;

                case DiatonicToneClass.D:
                    return ChromaticPitchClass.D;

                case DiatonicToneClass.E:
                    return ChromaticPitchClass.E;

                case DiatonicToneClass.F:
                    return ChromaticPitchClass.F;

                case DiatonicToneClass.G:
                    return ChromaticPitchClass.G;

                case DiatonicToneClass.A:
                    return ChromaticPitchClass.A;

                case DiatonicToneClass.B:
                    return ChromaticPitchClass.B;

                default:
                    return (ChromaticPitchClass)key;
            }
        }

        public static DiatonicToneClass GetNaturalKeyClass_PreferSharps(this ChromaticPitchClass pitch) {
            switch (pitch) {
                case ChromaticPitchClass.C:
                case ChromaticPitchClass.Dflat:
                    return DiatonicToneClass.C;

                case ChromaticPitchClass.D:
                case ChromaticPitchClass.Eflat:
                    return DiatonicToneClass.D;

                case ChromaticPitchClass.E:
                    return DiatonicToneClass.E;

                case ChromaticPitchClass.F:
                case ChromaticPitchClass.Gflat:
                    return DiatonicToneClass.F;

                case ChromaticPitchClass.G:
                case ChromaticPitchClass.Aflat:
                    return DiatonicToneClass.G;

                case ChromaticPitchClass.A:
                case ChromaticPitchClass.Bflat:
                    return DiatonicToneClass.A;

                case ChromaticPitchClass.B:
                    return DiatonicToneClass.B;

                default:
                    return (DiatonicToneClass)pitch;
            }
        }

        public static DiatonicToneClass GetNaturalKeyClass_PreferFlats(this ChromaticPitchClass pitch) {
            switch (pitch) {
                case ChromaticPitchClass.C:
                    return DiatonicToneClass.C;

                case ChromaticPitchClass.Dflat:
                case ChromaticPitchClass.D:
                    return DiatonicToneClass.D;

                case ChromaticPitchClass.Eflat:
                case ChromaticPitchClass.E:
                    return DiatonicToneClass.E;

                case ChromaticPitchClass.F:
                    return DiatonicToneClass.F;

                case ChromaticPitchClass.Gflat:
                case ChromaticPitchClass.G:
                    return DiatonicToneClass.G;

                case ChromaticPitchClass.Aflat:
                case ChromaticPitchClass.A:
                    return DiatonicToneClass.A;

                case ChromaticPitchClass.Bflat:
                case ChromaticPitchClass.B:
                    return DiatonicToneClass.B;

                default:
                    return (DiatonicToneClass)pitch;
            }
        }

        public static DiatonicTone ToKey(this DiatonicToneClass keyclass, int octave) =>
            new DiatonicTone(
                keyclass,
                octave
            );

        public static ChromaticPitchClass Transform(
                this PitchTransform transform,
                ChromaticPitchClass pitch
            ) =>
            (ChromaticPitchClass)(((int)pitch + transform.Steps + 12) % 12);

        public static DiatonicTone Key(
                this KeySignature signature,
                SemiTone tone,
                out PitchTransform transform
            ) =>
            new DiatonicTone(
                    signature.KeyOfPitchClass(tone.PitchClass, out transform),
                    tone.Octave
                );

        public static DiatonicToneClass ToRight(this DiatonicToneClass key) =>
            (DiatonicToneClass)(((int)key + 1) % 7);

        public static DiatonicToneClass ToLeft(this DiatonicToneClass key) =>
            (DiatonicToneClass)(((int)key - 1) % 7);

        public static IEnumerable<IDuratedItem<T>> Intersecting_children<T>(
                this IDurationField<IDurationField<T>> field,
                Time point
            ) =>
            field
                .Intersecting(point)
                .SelectMany(
                        child =>
                            child
                                .Value
                                .Intersecting(point - child.Duration.Start)
                                .Select(
                                        item =>
                                            new DuratedItem<T> {
                                                Value = item.Value,
                                                Duration = item.Duration + child.Duration.Start
                                            }
                                    )
                    );

        public static IEnumerable<IDuratedItem<T>> Intersecting_children<T>(
                this IDurationField<IDurationField<T>> field,
                Duration duration
            ) =>
            field
                .Intersecting(duration)
                .SelectMany(
                        child =>
                            child
                                .Value
                                .Intersecting(duration - child.Duration.Start)
                                .Select(
                                        item =>
                                            new DuratedItem<T> {
                                                Value = item.Value,
                                                Duration = item.Duration + child.Duration.Start
                                            }
                                    )
                    );

        public static bool AnyItemIn<T>(
                this IDurationField<T> field,
                Duration duration
            ) =>
            field.Intersecting(duration).Any();

        public static void ScootAndOverwrite<T>(
                this DurationField<T> field,
                T item,
                Duration duration
            ) {
            foreach (var oldstaff in field.Intersecting(duration).ToArray()) {
                field.Remove(oldstaff);

                foreach (var cutduration in oldstaff.Duration.Subtract(duration))
                    field.Add(oldstaff.Value, cutduration);
            }

            field.Add(item, duration);
        }

        public static void OverwriteEverythingToRight<T>(
                this DurationField<T> field,
                T item,
                Time start
            ) =>
            ScootAndOverwrite(
                    field,
                    item,
                    new Duration {
                        Start = start,
                        End =
                            field
                                .Intersecting(start)
                                .Max(item2 => item2.Duration.End)
                    }
                );

        public static void DeleteTime<T>(
                this DurationField<T> field,
                Duration eraser
            ) {
            var throughandbeyond =
                new Duration {
                    Start = eraser.Start,
                    End = field.GeneralDuration.Value.End
                };

            foreach (var item in field.Intersecting(throughandbeyond).ToArray()) {
                var subtractedtime =
                    item.Duration.Subtract_Time(eraser);
                
                if (subtractedtime != null)
                    field.Move(item, subtractedtime);
                else field.Remove(item);
            }
        }

        public static IStorageObject GetOrMake(this IStorageObject parent, string child) {
            try {
                return parent.Graph[parent[child]];
            }
            catch (KeyNotFoundException) {
                var childID = parent.Graph.Create();

                parent.Add(child, childID);

                return parent.Graph[childID];
            }
        }

        public static IStorageObject Get(this IStorageObject parent, string child) =>
            parent.Graph[parent[child]];

        public static IStorageObject GetOrDefault(this IStorageObject parent, string child) {
            var sink =
                parent[child];

            if (sink == default(StorageObjectID))
                return null;

            return parent.Graph[sink];
        }

        public static IStorageObject CreateObject(this IStorageGraph graph) =>
            graph[graph.Create()];

        public static void CreateAllObjects<T>(this BoundList<T> boundlist)
            where T : IBoundObject<T> {
            foreach (var factory in boundlist.FactorySet.Factories)
                boundlist.Create(factory.Name);
        }
    }
}
