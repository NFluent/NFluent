﻿// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="LambdaExceptionCheck.cs" company="">
// //   Copyright 2013 Rui CARVALHO, Thomas PIERRAIN
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
    using System.Linq.Expressions;
    using Extensibility;
    using Kernel;

#if NETSTANDARD1_3
    using System.Reflection;
#endif

    /// <summary>
    /// Implements specific Value check after lambda checks.
    /// </summary>
    /// <typeparam name="T">Code checker type./>.
    /// </typeparam>
    public class LambdaExceptionCheck<T> : FluentSut<T>, ILambdaExceptionCheck<T>, IForkableCheck
        where T : Exception
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="LambdaExceptionCheck{T}"/> class.
        /// This check can only be fluently called after a lambda check.
        /// </summary>
        /// <param name="value">The Value.</param>
        /// <param name="negated">Set to true to invert logic</param>
        public LambdaExceptionCheck(T value, bool negated) : base(value, Check.Reporter, negated)
        {
        }

        /// <inheritdoc />
        public ILambdaExceptionCheck<TE> DueTo<TE>()
            where TE : Exception
        {
            Exception resultException = null;
            ExtensibilityHelper.BeginCheck(this)
                .CantBeNegated("DueTo")
                .CheckSutAttributes(sut => sut.InnerException, "inner exception")
                .FailIfNull("There is no inner exception.")
                .Analyze((sut, test) =>
                {
                    resultException = sut;
                    while (resultException != null)
                    {
                        if (resultException.GetType() == typeof(TE))
                        {
                            break;
                        }

                        resultException = resultException.InnerException;
                    }
                    test.FailWhen(_ => resultException == null,
                        "The {0} is not of the expected type.");
                })
                .DefineExpectedType(typeof(TE))
                .EndCheck();
            return new LambdaExceptionCheck<TE>((TE)resultException, Negated);
        }

        /// <inheritdoc />
        public object ForkInstance()
        {
            return new LambdaExceptionCheck<T>(this.Value, false);
        }
    }
}