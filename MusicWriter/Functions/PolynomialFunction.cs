﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class PolynomialFunction : IFunction, IDirectlyIntegratableFunction, IParamaterizedFunction {
        readonly float[] coefficients;

        public float[] Coefficients {
            get { return coefficients; }
        }

        public float[] Arguments {
            get { return coefficients; }
        }

        public int Degree {
            get { return Coefficients.Length; }
        }

        public IFunctionFactory Factory {
            get { return FactoryClass.Instance; }
        }

        public sealed class FactoryClass : IFunctionFactory
        {
            public string FriendlyName {
                get { return "Polynomial Function"; }
            }

            public string CodeName {
                get { return "polynomial"; }
            }

            public bool AcceptsParameters {
                get { return true; }
            }

            public bool StoresBinaryData {
                get { return false; }
            }

            public IFunction Create(
                    IFunction context = null,
                    IFunction[] args = null,
                    EditorFile file = null,
                    string key = null,
                    params float[] numbers
                ) =>
                new PolynomialFunction(
                        numbers ?? new float[0]
                    );

            private FactoryClass() { }

            public static readonly IFunctionFactory Instance = new FactoryClass();
        }

        public PolynomialFunction(params float[] coefficients) {
            this.coefficients = coefficients;
        }

        public float GetValue(FunctionCall arg) {
            var local_var = 1f;

            var accumulator = 0f;

            for (int i = 0; i < Degree; i++) {
                accumulator += local_var * Coefficients[i];

                local_var *= arg.LocalTime;
            }

            return accumulator;
        }

        public IFunction Integrate() {
            var integrated_coefficients = new float[Degree + 1];

            for (int i = 1; i <= Degree; i++)
                integrated_coefficients[i] = coefficients[i - 1] / i;

            return new PolynomialFunction(integrated_coefficients);
        }
    }
}
