// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="StringCheckExtensions.cs" company="NFluent">
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

namespace NFluent
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Extensibility;
    using Extensions;
    using Helpers;
    using Kernel;
    using System.Linq;

    /// <summary>
    /// Provides check methods to be executed on a string instance.
    /// </summary>
    public static class StringCheckExtensions
    {
        /// <summary>
        ///     Checks that the checker value is equal to another expected value.
        /// </summary>
        /// <param name="check">The fluent check to be extended.</param>
        /// <param name="expected">The expected value.</param>
        /// <returns>
        ///     A check link.
        /// </returns>
        /// <exception cref="FluentCheckException">The checker value is not equal to the expected value.</exception>
        public static ICheckLink<ICheck<string>> IsEqualTo(this ICheck<string> check, string expected)
        {
            var test = ExtensibilityHelper.BeginCheck(check);
            PerformEqualCheck(expected, test);
            return ExtensibilityHelper.BuildCheckLink(check);
        }

        /// <summary>
        ///     Checks that the checker value is not equal to another expected value.
        /// </summary>
        /// <param name="check">The fluent check to be extended.</param>
        /// <param name="expected">The expected value.</param>
        /// <returns>
        ///     A check link.
        /// </returns>
        /// <exception cref="FluentCheckException">The checker value is equal to the expected value.</exception>
        public static ICheckLink<ICheck<string>> IsNotEqualTo(this ICheck<string> check, object expected)
        {
            return IsNotEqualTo(check, (string) expected);
        }


        /// <summary>
        ///     Checks that the checker value is not equal to another expected value.
        /// </summary>
        /// <param name="check">The fluent check to be extended.</param>
        /// <param name="expected">The expected value.</param>
        /// <returns>
        ///     A check link.
        /// </returns>
        /// <exception cref="FluentCheckException">The checker value is equal to the expected value.</exception>
        public static ICheckLink<ICheck<string>> IsNotEqualTo(this ICheck<string> check, string expected)
        {
            return check.Not.IsEqualTo(expected);
        }

        /// <summary>
        /// Checks that the checker value is equal to another expected value.
        /// </summary>
        /// <param name="check">The fluent check to be extended.</param>
        /// <param name="expected">The expected value.</param>
        /// <returns>
        ///  A check link.
        /// </returns>
        /// <exception cref="FluentCheckException">The checker value is not equal to the expected value.</exception>
        public static ICheckLink<ICheck<string>> IsEqualTo(this ICheck<string> check, object expected)
        {
            if (expected is string s)
            {
                return IsEqualTo(check, s);
            }
            else
            {
                return EqualityHelper.PerformEqualCheck(check, expected);
            }
        }

        /// <summary>
        /// Checks that the string is equals to another one, disregarding case.
        /// </summary>
        /// <param name="check">The fluent check to be extended.</param>
        /// <param name="comparand">The string to compare to.</param>
        /// <returns>
        /// A check link.
        /// </returns>
        /// <exception cref="FluentCheckException">The string is not equal to the comparand.</exception>
        public static ICheckLink<ICheck<string>> IsEqualIgnoringCase(this ICheck<string> check, string comparand)
        {
            var test = ExtensibilityHelper.BeginCheck(check);
            PerformEqualCheck(comparand, test, true);
            return ExtensibilityHelper.BuildCheckLink(check);
        }


        private static void PerformEqualCheck(object expected, ICheckLogic<string> test, bool ignoreCase = false)
        {
            test.DefineExpectedValue(expected)
                .OnNegate("The {0} is equal to the {1} whereas it must not.", MessageOption.NoCheckedBlock)
                .FailWhen((sut) => expected == null && sut != null, "The {0} is not null whereas it must.")
                .FailWhen((sut) => expected != null && sut == null, "The {0} is null whereas it must not.")
                .Analyze((sut, runner) =>
                {
                    var details = StringDifference.Analyze(sut, (string) expected, ignoreCase);
                    if (details != null && details.Count > 0)
                    {
                        runner.Fail(StringDifference.SummaryMessage(details));
                    }
                })
                .EndCheck();
        }

        /// <summary>
        ///     Checks that the checker value is one of these possible elements.
        /// </summary>
        /// <param name="check">The check.</param>
        /// <param name="possibleElements">The possible elements.</param>
        /// <returns>
        ///     A check link.
        /// </returns>
        /// <exception cref="FluentCheckException">The checker value is NOT one of the elements.</exception>
        public static ICheckLink<ICheck<string>> IsOneOfThese(this ICheck<string> check, params string[] possibleElements)
        {
            ExtensibilityHelper.BeginCheck(check)
                .FailWhen(sut => possibleElements == null && sut != null,
                    "The {0} must be null as there is no other possible value.", MessageOption.NoExpectedBlock)
                .FailWhen(sut => possibleElements != null && !possibleElements.Any(x => string.Equals(x, sut)),
                    "The {0} is not one of the possible elements.")
                .DefinePossibleValues(possibleElements, possibleElements== null ? 0 : possibleElements.Length)
                .OnNegate("The {0} is one of the possible elements whereas it must not.")
                .EndCheck();

            return ExtensibilityHelper.BuildCheckLink(check);
        }

        /// <summary>
        ///     Checks that the string contains the given expected values, in any order.
        /// </summary>
        /// <param name="check">The fluent check to be extended.</param>
        /// <param name="values">The expected values to be found.</param>
        /// <returns>
        ///     A check link.
        /// </returns>
        /// <exception cref="FluentCheckException">The string  contains all the given strings in any order.</exception>
        public static ExtendableCheckLink<ICheck<string>, string[]> Contains(this ICheck<string> check, params string[] values)
        {
            var block = ExtensibilityHelper.BeginCheck(check);
            ContainsLogic(values, block);
            return ExtensibilityHelper.BuildExtendableCheckLink(check, values);
        }

        /// <summary>
        ///     Checks that the string does not contain any of the given expected values.
        /// </summary>
        /// <param name="check">The fluent check to be extended.</param>
        /// <param name="values">The values not to be present.</param>
        /// <returns>
        ///     A check link.
        /// </returns>
        /// <exception cref="FluentCheckException">The string contains at least one of the given strings.</exception>
        public static ICheckLink<ICheck<string>> DoesNotContain(this ICheck<string> check, params string[] values)
        {
            return check.Not.Contains(values);
        }

        private static void ContainsLogic(ICollection<string> values, ICheckLogic<string> block)
        {
            block.
                FailIfNull().
                DefinePossibleValues(values, values.Count, "contains", "does not contain").
                Analyze((sut, test) =>
                    {
                        var missingItems = new List<string>();
                        var presentItems = new List<string>();

                        foreach (var value in values)
                        {
                            (sut.Contains(value) ? presentItems : missingItems).Add(value);
                        }

                        if (missingItems.Any())
                        {
                            test.Fail($"The {{0}} does not contain the expected value(s): " +
                                      $"{missingItems.ToStringProperlyFormatted().DoubleCurlyBraces()}");
                        }
                        test.OnNegate($"The {{0}} contains unauthorized value(s): {presentItems.ToStringProperlyFormatted().DoubleCurlyBraces()}");

                    }).
                //OnNegate("Can't be negated").
                EndCheck();
        }

        /// <summary>
        ///     Checks that the string starts with the given expected prefix.
        /// </summary>
        /// <param name="check">The fluent check to be extended.</param>
        /// <param name="expectedPrefix">The expected prefix.</param>
        /// <returns>
        ///     A check link.
        /// </returns>
        /// <exception cref="FluentCheckException">The string does not start with the expected prefix.</exception>
        public static ICheckLink<ICheck<string>> StartsWith(this ICheck<string> check, string expectedPrefix)
        {
            var checker = ExtensibilityHelper.ExtractChecker(check);

            checker.BeginCheck()
                .FailIfNull()
                .FailWhen(sut => !sut.StartsWith(expectedPrefix), "The {0}'s start is different from the {1}.")
                .DefineExpectedValue(expectedPrefix, "starts with", "does not start with")
                .OnNegate("The {0} starts with the {1}, whereas it must not.")
                .EndCheck();
            return checker.BuildChainingObject();
        }

        /// <summary>
        ///     Checks that the string ends with the given expected suffix.
        /// </summary>
        /// <param name="check">The fluent check to be extended.</param>
        /// <param name="expectedEnd">The expected suffix.</param>
        /// <returns>
        ///     A check link.
        /// </returns>
        /// <exception cref="FluentCheckException">The string does not end with the expected prefix.</exception>
        public static ICheckLink<ICheck<string>> EndsWith(this ICheck<string> check, string expectedEnd)
        {
            var checker = ExtensibilityHelper.ExtractChecker(check);
            checker.BeginCheck()
                .FailIfNull()
                .FailWhen(sut => !sut.EndsWith(expectedEnd), "The {0}'s end is different from the {1}.")
                .DefineExpectedValue(expectedEnd, "ends with", "does not end with")
                .OnNegate("The {0} ends with {1}, whereas it must not.")
                .EndCheck();
            return checker.BuildChainingObject();
        }

        /// <summary>
        /// Checks that the string matches a given regular expression.
        /// </summary>
        /// <param name="context">The fluent check to be extended.</param>
        /// <param name="regExp">The regular expression.</param>
        /// <returns>
        ///     A check link.
        /// </returns>
        /// <exception cref="FluentCheckException">The string does not end with the expected prefix.</exception>
        public static ICheckLink<ICheck<string>> Matches(this ICheck<string> context, string regExp)
        {
            ExtensibilityHelper.BeginCheck(context)
                .DefineExpectedValue(regExp, "matches", "does not match")
                .FailIfNull()
                .FailWhen(sut => new Regex(regExp).IsMatch(sut) == false, "The {0} does not match the {1}.")
                .OnNegate("The {0} matches the {1}, whereas it must not.")
                .EndCheck();

            return ExtensibilityHelper.BuildCheckLink(context);
        }

        /// <summary>
        /// Checks that the string matches a given expression with wildcard.
        /// </summary>
        /// <param name="context">The fluent check to be extended.</param>
        /// <param name="expression">The regular expression.</param>
        /// <returns>
        ///     A check link.
        /// </returns>
        /// <exception cref="FluentCheckException">The string does not end with the expected prefix.</exception>
        public static ICheckLink<ICheck<string>> MatchesWildcards(this ICheck<string> context, string expression)
        {
            var regExp = "^"+Regex.Escape(expression).Replace("\\*", ".*").Replace("\\?", ".")+"$";
            ExtensibilityHelper.BeginCheck(context)
                .DefineExpectedValue(expression, "matches", "does not match")
                .FailIfNull()
                .FailWhen(sut => new Regex(regExp).IsMatch(sut) == false, "The {0} does not match the {1}.")
                .OnNegate("The {0} matches the {1}, whereas it must not.")
                .EndCheck();

            return ExtensibilityHelper.BuildCheckLink(context);
        }

        /// <summary>
        ///     Checks that the string does not match a given regular expression.
        /// </summary>
        /// <param name="check">The fluent check to be extended.</param>
        /// <param name="regExp">The regular expression prefix.</param>
        /// <returns>
        ///     A check link.
        /// </returns>
        /// <exception cref="FluentCheckException">The string does not end with the expected prefix.</exception>
        public static ICheckLink<ICheck<string>> DoesNotMatch(this ICheck<string> check, string regExp)
        {
            return check.Not.Matches(regExp);
         }


        /// <summary>
        ///     Checks that the string is null, empty or only spaces.
        /// </summary>
        /// <param name="check">The fluent check.</param>
        /// <returns>
        ///     A check link.
        /// </returns>
        public static ICheckLink<ICheck<string>> IsNullOrWhiteSpace(this ICheck<string> check)
        {
            ExtensibilityHelper.BeginCheck(check)
                .FailWhen(sut => !PolyFill.IsNullOrWhiteSpace(sut), "The {0} contains non whitespace characters.")
                .OnNegateWhen(sut => sut == null, "The {0} is null, whereas it should not.")
                .OnNegateWhen(sut => sut == string.Empty, "The {0} is empty, whereas it should not.")
                .OnNegate("The {0} contains only whitespace characters, whereas it should not.")
                .EndCheck();

            return ExtensibilityHelper.BuildCheckLink(check);
        }

        /// <summary>
        ///     Checks that the string is empty.
        /// </summary>
        /// <param name="check">The fluent check.</param>
        /// <returns>
        ///     A check link.
        /// </returns>
        /// <exception cref="FluentCheckException">The string is not empty.</exception>
        public static ICheckLink<ICheck<string>> IsEmpty(this ICheck<string> check)
        {
            ExtensibilityHelper.BeginCheck(check)
                .OnNegate("The {0} is empty, whereas it must not.")
                .FailWhen(sut => sut == null, "The {0} is null instead of being empty.", MessageOption.NoCheckedBlock)
                .FailWhen(sut => sut != string.Empty, "The {0} is not empty.")
                .EndCheck();
            return ExtensibilityHelper.BuildCheckLink(check);
        }

        /// <summary>
        ///     Checks that the string is empty or null.
        /// </summary>
        /// <param name="check">The fluent check.</param>
        /// <returns>
        ///     A check link.
        /// </returns>
        /// <exception cref="FluentCheckException">The string is neither empty or null.</exception>
        public static ICheckLink<ICheck<string>> IsNullOrEmpty(this ICheck<string> check)
        {
            ExtensibilityHelper.BeginCheck(check)
                .OnNegate("The {0} is empty, whereas it must not.")
                .FailWhen(sut => !string.IsNullOrEmpty(sut), "The {0} is not empty or null.")
                .EndCheck();
            return ExtensibilityHelper.BuildCheckLink(check);
        }

        /// <summary>
        ///     Checks that the string is not empty.
        /// </summary>
        /// <param name="check">The fluent check to be extended.</param>
        /// <returns>
        ///     A check link.
        /// </returns>
        /// <exception cref="FluentCheckException">The string is empty.</exception>
        public static ICheckLink<ICheck<string>> IsNotEmpty(this ICheck<string> check)
        {
            ExtensibilityHelper.BeginCheck(check)
                .FailWhen(sut => sut == null, "The {0} is null whereas it must have content.",
                    MessageOption.NoCheckedBlock)
                .FailWhen(string.IsNullOrEmpty, "The {0} is empty, whereas it must not.", MessageOption.NoCheckedBlock)
                .OnNegateWhen(sut => sut == null, "The {0} is null instead of being empty.")
                .OnNegate("The {0} is not empty or null.")
                .EndCheck();
            return ExtensibilityHelper.BuildCheckLink(check);
        }

        /// <summary>
        ///     Checks that the string has content.
        /// </summary>
        /// <param name="check">The fluent check to be extended.</param>
        /// <returns>
        ///     A check link.
        /// </returns>
        /// <exception cref="FluentCheckException">The string is empty or null.</exception>
        public static ICheckLink<ICheck<string>> HasContent(this ICheck<string> check)
        {
            return IsNotEmpty(check);
        }

        /// <summary>
        ///     Convert a string to an array of lines.
        /// </summary>
        /// <param name="check">The fluent check to be processed.</param>
        /// <returns>A checker.</returns>
        public static ICheck<IEnumerable<string>> AsLines(this ICheck<string> check)
        {
            var checker = ExtensibilityHelper.ExtractChecker(check);
            var checkedString = checker.Value;
            IEnumerable<string> next = checkedString.SplitAsLines();

            return new FluentCheck<IEnumerable<string>>(next);
        }
    }
}