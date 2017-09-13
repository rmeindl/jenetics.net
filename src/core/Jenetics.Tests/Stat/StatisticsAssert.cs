// Java Genetic Algorithm Library.
// Copyright (c) 2017 Franz Wilhelmstötter
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Author:
//    Franz Wilhelmstötter (franz.wilhelmstoetter@gmx.at)

using System;
using System.Linq;
using MathNet.Numerics.Distributions;
using Xunit;
using static MathNet.Numerics.Distributions.ChiSquared;

namespace Jenetics.Stat
{
    public static class StatisticsAssert
    {
        public static void AssertDistribution<T>(Histogram<T> distribution, double[] expected, double alpha,
            double safety)
        {
            var exp = expected.Select(v => Math.Max(v, double.MinValue)).ToArray();

            var dist = distribution.GetHistogram();

            var χ2 = ChiSquare(exp, dist);
            var max_χ2 = Chi(1 - alpha, distribution.Length);
            var reject = χ2 > max_χ2 * safety;
            //var reject = ChiSquareTest(exp, dist, alpha);

            Assert.False(reject,
                $"The histogram doesn't follow the given distribution. χ2 must be smaller than {max_χ2} but was {χ2}");
        }

        private static double Chi(double p, int degreeOfFreedom)
        {
            return InvCDF(degreeOfFreedom, p);
        }

        private static double ChiSquare(double[] expected, double[] observed)
        {
            /*
            if (expected.length < 2) {
                throw new DimensionMismatchException(expected.length, 2);
            }
            if (expected.length != observed.length) {
                throw new DimensionMismatchException(expected.length, observed.length);
            }
            MathArrays.checkPositive(expected);
            MathArrays.checkNonNegative(observed);            
            */

            var sumExpected = 0d;
            var sumObserved = 0d;
            for (var i = 0; i < observed.Length; i++)
            {
                sumExpected += expected[i];
                sumObserved += observed[i];
            }
            var ratio = 1.0d;
            var rescale = false;
            if (Math.Abs(sumExpected - sumObserved) > 10E-6)
            {
                ratio = sumObserved / sumExpected;
                rescale = true;
            }
            var sumSq = 0.0d;
            for (var i = 0; i < observed.Length; i++)
                if (rescale)
                {
                    var dev = observed[i] - ratio * expected[i];
                    sumSq += dev * dev / (ratio * expected[i]);
                }
                else
                {
                    var dev = observed[i] - expected[i];
                    sumSq += dev * dev / expected[i];
                }
            return sumSq;
        }

        private static double ChiSquareTest(double[] expected, double[] observed)
        {
            var distribution = new ChiSquared(expected.Length - 1.0);
            return 1 - distribution.CumulativeDistribution(ChiSquare(expected, observed));
            //final ChiSquaredDistribution distribution = new ChiSquaredDistribution(null, expected.length - 1.0);
            //return 1 - distribution.cumulativeProbability(chiSquare(counts));
        }

        private static bool ChiSquareTest(double[] expected, double[] observed, double alpha)
        {
            if (alpha <= 0 || alpha > 0.5)
                throw new ArgumentOutOfRangeException(
                    $"out of bounds significance level {alpha}, must be between {0.0} and {0.5}");
            return ChiSquareTest(expected, observed) < alpha;
        }
    }
}