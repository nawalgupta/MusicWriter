﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter {
    public sealed class AdornmentTrack : BoundObject<AdornmentTrack> {
        readonly DurationFieldBinder<Staff> binder_staffs;
        readonly DurationFieldBinder<KeySignature> binder_keysigs;
        readonly IStorageObject obj;

        public DurationField<KeySignature> KeySignatures { get; } =
            new DurationField<KeySignature>();

        public DurationField<Staff> Staffs { get; } =
            new DurationField<Staff>();
        
        public AdornmentTrack(
                StorageObjectID storageobjectID,
                EditorFile file
            ) :
            base(
                    storageobjectID,
                    file
                ) {
            obj = this.Object();

            binder_staffs =
                Staffs.Bind(
                        obj.GetOrMake("staffs").ID,
                        File
                    );

            binder_staffs.Deserializer = staff_obj => {
                using (var stream = staff_obj.OpenRead()) {
                    using (var br = new BinaryReader(stream)) {
                        var staff = new Staff();

                        staff.Lines = br.ReadInt32();
                        staff.MiddleHalfLine = br.ReadInt32();
                        staff.Shift = br.ReadInt32();
                        staff.Clef.BottomKey = new DiatonicTone(br.ReadInt32());
                        staff.Clef.Symbol = (ClefSymbol)br.ReadInt32();

                        return staff;
                    }
                }
            };

            binder_staffs.Serializer = (staff_obj, staff) => {
                using (var stream = staff_obj.OpenWrite()) {
                    using (var bw = new BinaryWriter(stream)) {
                        bw.Write(staff.Lines);
                        bw.Write(staff.MiddleHalfLine);
                        bw.Write(staff.Shift);
                        bw.Write(staff.Clef.BottomKey.Tones);
                        bw.Write((int)staff.Clef.Symbol);
                    }
                }
            };

            binder_keysigs =
                KeySignatures.Bind(
                        obj.GetOrMake("key-signatures").ID,
                        File
                    );

            binder_keysigs.Deserializer = keysig_obj => {
                using (var stream = keysig_obj.OpenRead()) {
                    using (var br = new BinaryReader(stream)) {
                        var transform_a = new PitchTransform(br.ReadInt32());
                        var transform_b = new PitchTransform(br.ReadInt32());
                        var transform_c = new PitchTransform(br.ReadInt32());
                        var transform_d = new PitchTransform(br.ReadInt32());
                        var transform_e = new PitchTransform(br.ReadInt32());
                        var transform_f = new PitchTransform(br.ReadInt32());
                        var transform_g = new PitchTransform(br.ReadInt32());

                        var keysig =
                            new KeySignature(
                                    transform_c,
                                    transform_d,
                                    transform_e,
                                    transform_f,
                                    transform_g,
                                    transform_a,
                                    transform_b
                                );

                        return keysig;
                    }
                }
            };

            binder_keysigs.Serializer = (keysig_obj, keysig) => {
                using (var stream = keysig_obj.OpenWrite()) {
                    using (var bw = new BinaryWriter(stream)) {
                        bw.Write(keysig[DiatonicToneClass.A].Steps);
                        bw.Write(keysig[DiatonicToneClass.B].Steps);
                        bw.Write(keysig[DiatonicToneClass.C].Steps);
                        bw.Write(keysig[DiatonicToneClass.D].Steps);
                        bw.Write(keysig[DiatonicToneClass.E].Steps);
                        bw.Write(keysig[DiatonicToneClass.F].Steps);
                        bw.Write(keysig[DiatonicToneClass.G].Steps);
                    }
                }
            };
        }

        public override void Bind() {
            binder_staffs.Bind();
            binder_keysigs.Bind();

            base.Bind();
        }

        public override void Unbind() {
            binder_staffs.Unbind();
            binder_keysigs.Unbind();

            base.Unbind();
        }
    }
}
