﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="CustomEqualTests.cs" company="NFluent">
//   Copyright 2018 Thomas PIERRAIN & Cyrille DUPUYDAUBY
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace NFluent.Tests
{
    using System.Collections.Generic;
    using NFluent.Helpers;
    using NUnit.Framework;

    public class CustomEqualTests
    {
        private readonly List<int> a = new List<int> {1, 2};
        private readonly List<int> b = new List<int> {3, 4};

        private readonly List<List<int>> list = new List<List<int>>
        {
            new List<int> {1, 2}, // new instance, same as a
            new List<int> {3, 4} // new instance, same as b
        };

        [Test]
        public void ValueDifferenceReturnsEmptyIfEqual()
        {
            var result = DifferenceFinders.ValueDifference(this.a, "a", this.a);
            Check.That(result.GetCount()).IsEqualTo(0);
        }

        [Test]
        public void ValueDifferenceGivesDetailsOnFailure()
        {
            // List contains new instances of lists same as a and b

            var result = DifferenceFinders.ValueDifference(this.a, "a", this.b);
            Check.That(result.GetCount()).IsEqualTo(2);
            Check.That(result[0].FirstName).IsEqualTo("a[0]");
            Check.That(result[0].FirstValue).IsEqualTo(1);
            Check.That(result[0].SecondValue).IsEqualTo(3);

            result = DifferenceFinders.ValueDifference(this.a, "a", this.a);
            Check.That(result.GetCount()).IsEqualTo(0);

            result = DifferenceFinders.ValueDifference(this.list, "a", new List<List<int>> {this.a, this.a});
            Check.That(result[0].FirstName).IsEqualTo("a[1][0]");
        }

        [Test]
        public void ValueDifferenceGivesDetailsOnFailureWithNull()
        {
            var result = DifferenceFinders.ValueDifference(new List<List<int>> {this.a, null}, "a", this.list);
            Check.That(result[0].FirstName).IsEqualTo("a[1]");
            result = DifferenceFinders.ValueDifference(this.list, "a", new List<List<int>> {this.a, null});
            Check.That(result[0].FirstName).IsEqualTo("a[1]");
        }

        [Test]
        public void ValueDifferenceGivesDetailsOnFailureWithShorterEnums()
        {
            var result = DifferenceFinders.ValueDifference(new List<List<int>> {this.a}, "a", this.list);
            Check.That(result[0].FirstName).IsEqualTo("a[1]");
            result = DifferenceFinders.ValueDifference(this.list, "a", new List<List<int>> {this.a});
            Check.That(result[0].FirstName).IsEqualTo("a[1]");
        }
    }
}