﻿// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="NotRelatedTests.cs" company="">
// //   Copyright 2013 Thomas PIERRAIN
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

// ReSharper disable once CheckNamespace

namespace NFluent.Tests
{
    using ForDocumentation;
    using Kernel;
    using NUnit.Framework;

    [TestFixture]
    public class NotRelatedTests
    {
        [Test]
        [Explicit]
        public void CheckContextWorks()
        {
            if (!CheckContext.DefaultNegated)
            {
               return;
            }
            CheckContext.DefaultNegated = false;
            try
            {
               Assert.IsFalse(CheckContext.DefaultNegated);
            }
            finally
            {
                CheckContext.DefaultNegated = true;
            }
        }

        [Test]
        [Ignore("Test no longer works for tests which are expected to fail due to double negation. Need to be fixed.")]
        public void ForceNegationOnAllTest()
        {
            if (!CheckContext.DefaultNegated)
            {
                return;
            }

            CheckContext.DefaultNegated = false;
            try
            {
                var assembly = typeof(ObjectRelatedTest).GetTypeInfo().Assembly;
                RunnerHelper.RunAllTests(assembly, false);
            }
            finally
            {
                CheckContext.DefaultNegated = true;
            }
        }
    }
}
