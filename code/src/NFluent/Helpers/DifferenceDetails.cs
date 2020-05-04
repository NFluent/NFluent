﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="DifferenceDetails.cs" company="NFluent">
//   Copyright 2019 Cyrille DUPUYDAUBY
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

namespace NFluent.Helpers
{
    using NFluent.Extensions;
    internal class DifferenceDetails
    {
        private DifferenceMode Mode {get;}

        public bool StillNeededForEquivalence => this.Mode != DifferenceMode.Moved && this.Mode !=DifferenceMode.Value;
        public bool StillNeededForEquality => this.Mode != DifferenceMode.FoundInsteadOf;

        private DifferenceDetails(string firstName, object firstValue, object secondValue, int index, DifferenceMode mode)
        {
            this.Mode = mode;
            this.FirstName = firstName;
            this.FirstValue = firstValue;
            this.SecondValue = secondValue;
            this.Index = index;
            this.ActualIndex = index;
        }

        public static DifferenceDetails WasNotExpected(string checkedName, object value, int index)
        {
            return new DifferenceDetails(checkedName, value, null, index, DifferenceMode.Extra);
        }

        public static DifferenceDetails DoesNotHaveExpectedValue(string checkedName, object value, object expected, int index)
        {
            return new DifferenceDetails(checkedName, value, expected, index, DifferenceMode.Value);
        }
        public static DifferenceDetails DoesNotHaveExpectedAttribute(string checkedName, object value, object expected, int index)
        {
            return new DifferenceDetails(checkedName, value, expected, index, DifferenceMode.Attribute);
        }

        public static DifferenceDetails WasNotFound(string checkedName, object expected, int index)
        {
            return new DifferenceDetails(checkedName, null, expected, index, DifferenceMode.Missing);
        }

        public static DifferenceDetails WasFoundElseWhere(string checkedName, object value, int expectedIndex, int actualIndex)
        {
            return new DifferenceDetails(checkedName, value, null, expectedIndex, DifferenceMode.Moved) { ActualIndex = actualIndex };
        }

        public static DifferenceDetails WasFoundInsteadOf(string checkedName, object checkedValue, object expectedValue)
        {
            return new DifferenceDetails(checkedName, checkedValue, expectedValue, 0, DifferenceMode.FoundInsteadOf);
        }

        public static DifferenceDetails FromMatch(MemberMatch match)
        {
            if (!match.ActualFieldFound)
            {
                return WasNotFound(match.Expected.MemberLabel, match.Expected, 0);
            }

            if (!match.ExpectedFieldFound)
            {
                return WasNotExpected(match.Expected.MemberLabel, match.Expected, 0);
            }

            return DoesNotHaveExpectedValue(match.Expected.MemberLabel, match.Actual.Value, match.Expected.Value, 0);
        }
        
        public string FirstName { get; internal set; }
        public object FirstValue { get; internal set; }
        public object SecondValue { get; internal set; }
        public int Index { get; }
        public int ActualIndex { get; internal set; }

        public string GetMessage(bool forEquivalence)
        {
            if (this.Mode == DifferenceMode.Extra)
            {
                return forEquivalence
                    ? $"{this.FirstName} value should not exist (value {this.FirstValue.ToStringProperlyFormatted()})"
                    : $"{this.FirstName} should not exist (value {this.FirstValue.ToStringProperlyFormatted()}).";
            }
            else if (this.Mode == DifferenceMode.Missing)
            {
                return forEquivalence
                    ? $"{this.SecondValue.ToStringProperlyFormatted()} should be present but was not found."
                    : $"{this.FirstName} does not exist. Expected {this.SecondValue.ToStringProperlyFormatted()}.";
            }
            else if (this.Mode == DifferenceMode.Moved)
            {
                return
                    $"{this.FirstName} value ('{this.FirstValue}') was found at index {this.ActualIndex} instead of {this.Index}.";
            }
            else if (this.Mode == DifferenceMode.Attribute)
            {
                return
                    $"{this.FirstName} = {this.FirstValue.ToStringProperlyFormatted()} instead of {this.SecondValue.ToStringProperlyFormatted()}.";
            }
            else if (this.Mode == DifferenceMode.FoundInsteadOf)
            {
                return
                    $"{this.FirstValue.ToStringProperlyFormatted()} should not exist (found in {this.FirstName}); {this.SecondValue.ToStringProperlyFormatted()} should be found instead.";
            }
            else
            {
                return
                    $"{this.FirstName} = {this.FirstValue.ToStringProperlyFormatted()} instead of {this.SecondValue.ToStringProperlyFormatted()}.";
            }
        }

        public enum DifferenceMode
        {
            Attribute,
            Value,
            Missing,
            Extra,
            Moved,
            FoundInsteadOf
        };

    }
}
