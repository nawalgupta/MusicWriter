using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicWriter
{
    public sealed class PolynomialFunction : IFunction, IDirectlyIntegratableFunction
    {
        readonly float[] coefficients;

        public float[] Coefficients {
            get { return coefficients; }
        }

        public int Degree {
            get { return Coefficients.Length; }
        }

        public PolynomialFunction(float[] coefficients) {
            this.coefficients = coefficients;
        }

        public float GetValue(FunctionCall arg) {
            var local_var = 1f;
            var local_actual = arg.LocalTime.Notes;

            var accumulator = 0f;

            for (int i = 0; i < Degree; i++) {
                accumulator += local_var * Coefficients[i];

                local_var *= local_actual;
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
