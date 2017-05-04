﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class SquareFunction : IFunction
    {
        public IFunctionFactory Factory {
            get { return FactoryClass.Instance; }
        }

        public sealed class FactoryClass : IFunctionFactory
        {
            public string FriendlyName {
                get { return "Square Wave"; }
            }

            public string CodeName {
                get { return "square"; }
            }

            public bool AcceptsParameters {
                get { return false; }
            }

            public bool StoresBinaryData {
                get { return false; }
            }

            public IFunction Create(
                    IFunction context = null,
                    IFunction[] args = null,
                    EditorFile file = null,
                    string key = null,
                    params double[] numbers
                ) =>
                new SquareFunction();

            private FactoryClass() { }

            public static readonly IFunctionFactory Instance = new FactoryClass();
        }

        public double GetValue(FunctionCall arg) =>
            arg.WaveTime % 1f >= 0.5 ? +1 : -1;
    }
}
