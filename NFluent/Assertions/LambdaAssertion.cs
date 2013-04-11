﻿// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="LambdaAssertion.cs" company="">
// //   Copyright 2013 Cyrille DUPUYDAUBY
// //   Licensed under the Apache License, Version 2.0 (the "License");
// //   you may not use this file except in compliance with the License.
// //   You may obtain a copy of the License at
// //       http://www.apache.org/licenses/LICENSE-2.0
// //   Unless required by applicable law or agreed to in writing, software
// //   distributed under the License is distributed on an "AS IS" BASIS,
// //   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// //   See the License for the specific language governing permissions and
// //   limitations under the License.
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------

namespace NFluent
{
    using System;
    using System.Diagnostics;

    using NFluent.Helpers;

    /// <summary>
    /// Implements lambda/action specific assertion.
    /// </summary>
    public class LambdaAssertion : IFluentAssertion<Action>
    {
        private Exception exception;

        private double durationInNs;

        /// <summary>
        /// Initializes a new instance of the <see cref="LambdaAssertion"/> class. 
        /// </summary>
        /// <param name="action">
        /// Action to be assessed.
        /// </param>
        public LambdaAssertion(Action action)
        {
            this.Value = action;
            this.Execute();
        }

        /// <summary>
        /// Gets the value to be tested (provided for any extension method to be able to test it).
        /// </summary>
        /// <value>
        /// The value to be tested by any fluent assertion extension method.
        /// </value>
        public Action Value { get; private set; }

        /// <summary>
        /// Checks that the execution time is below a specified threshold.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="timeUnit">
        /// The time Unit.
        /// </param>
        /// <exception cref="FluentAssertionException">
        /// When execution is strictly above limit.
        /// </exception>
        /// <returns>
        /// A chainable assertion.
        /// </returns>
        public IChainableFluentAssertion<LambdaAssertion> LastsLessThan(double value, TimeUnit timeUnit)
        {
            double comparand = TimeHelper.GetInNanoSeconds(value, timeUnit);
            if (this.durationInNs > comparand)
            {
                throw new FluentAssertionException(string.Format("Code execution lasted more than {0} {1}.", value, timeUnit));
            }
            return new ChainableFluentAssertion<LambdaAssertion>(this);
        }

        private void Execute()
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                this.Value();
            }
            catch (Exception e)
            {
                this.exception = e;
            }
            finally
            {
                watch.Stop();
            }
            // ReSharper disable PossibleLossOfFraction
            this.durationInNs = watch.ElapsedTicks * 1000000000 / Stopwatch.Frequency;
        }

    }
}