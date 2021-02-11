﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RunnerHelper.cs" company="">
//   Copyright 2013 Cyrille DUPUYDAUBY
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//        http://www.apache.org/licenses/LICENSE-2.0
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NFluent.Tests.ForDocumentation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using Kernel;

    using NUnit.Framework;
#if NETCOREAPP1_0
    using Microsoft.Extensions.DependencyModel;
#endif

    /// <summary>
    /// Hosts several methods helping to execute unit tests in a controlled fashion.
    /// </summary>
    public static class RunnerHelper
    {
        private static bool inProcess;

        public static void RunThoseTests(IEnumerable<MethodInfo> tests, Type type, FullRunDescription report, bool log)
        {
            var specificTests = tests as IList<MethodInfo> ?? tests.ToList();
            if (specificTests.Count <= 0)
            {
                return;
            }

            Log($"TestFixture :{type.FullName}");
            var constructor = type.GetTypeInfo().GetConstructor(new Type[0]);
            // creates an instance
            var test = constructor?.Invoke(new object[0]);

            // run TestFixtureSetup
            RunAllMethodsWithASpecificAttribute(type, typeof(OneTimeSetUpAttribute), test);

            try
            {
                // run all tests
                foreach (var specificTest in specificTests.Where(
                    specificTest => !specificTest.GetCustomAttributes(typeof(ExplicitAttribute), false).Any() 
                                    && !specificTest.GetCustomAttributes(typeof(IgnoreAttribute), false).Any()))
                {
                    RunAllMethodsWithASpecificAttribute(type, typeof(SetUpAttribute), test);

                    try
                    {
                        // we have to capture exceptions
                        RunMethod(specificTest, test, report, log);
                    }
                    finally
                    {
                        RunAllMethodsWithASpecificAttribute(type, typeof(TearDownAttribute), test);
                    }
                }
            }
            finally
            {
                RunAllMethodsWithASpecificAttribute(type, typeof(OneTimeTearDownAttribute), test);
            }
        }

        public static void RunAction(Action action)
        {
            RunMethod(action.GetMethodInfo().GetBaseDefinition(), action.Target, null, false);
        }

        internal static Assembly[] GetLoadedAssemblies()
        {
#if NETCOREAPP1_0
            var assemblies = new List<Assembly>();
            var dependencies = DependencyContext.Default.RuntimeLibraries;
            foreach (var library in dependencies)
            {
                if (!IsCandidateCompilationLibrary(library)) continue;
                var assembly = Assembly.Load(new AssemblyName(library.Name));
                assemblies.Add(assembly);
            }
            return assemblies.ToArray();
        }

        private static bool IsCandidateCompilationLibrary(Library compilationLibrary)
        {
            return compilationLibrary.Name == ("Specify")
                || compilationLibrary.Dependencies.Any(d => d.Name.StartsWith("Specify"));
        }

#else
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        /// <summary>
        /// Polyfills for TypeInfo
        /// </summary>
        /// <param name="type"><see cref="Type"/> of interest.</param>
        /// <returns>Related type infos</returns>
        public static Type GetTypeInfo(this Type type)
        {
            return type;
        }

        /// <summary>
        /// Polyfill for GetMethodInfo
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo(this Action info)
        {
            return info.Method;
        }
#endif

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static void RunMethod(MethodBase specificTest, object test, FullRunDescription report, bool log)
        {

            IList<object[]> parameters = new List<object[]>();
            var testCases = specificTest.GetCustomAttributes(typeof(TestCaseAttribute));
            var attributes = testCases.ToList();
            if (attributes.Any())
            {
                foreach (var attribute in attributes)
                {
                    if (attribute is TestCaseAttribute testCase)
                    {
                        parameters.Add(testCase.Arguments);
                    }
                }
            }
            else
            {
                parameters.Add(new object[]{});
            }

            try
            {
                foreach (var pars in parameters)
                {
                    specificTest.Invoke(test, pars);
                }
            }
            catch (Exception e)
            {

                if (CheckContext.DefaultNegated == false)
                {
                    return;
                }
                if (e.InnerException is IgnoreException)
                {
                    return;
                }
                throw;
                //}

                //Type expectedType = ((ExpectedExceptionAttribute) specificTest.GetCustomAttributes(typeof(ExpectedExceptionAttribute), false)[0]).ExpectedException;
                //if (e.InnerException != null)
                //{
                //    if (e.InnerException is FluentCheckException)
                //    {
                //        var fluExc = e.InnerException as FluentCheckException;
                //        var desc = GetCheckAndType(fluExc);
                //        if (desc != null)
                //        {
                //            var method = desc.Check;
                //            var testedtype = desc.CheckedType;
                //            desc.ErrorSampleMessage = fluExc.Message;

                //            // are we building a report
                //            if (log)
                //            {
                //                Log(
                //                    string.Format(
                //                        "Check.That({1} sut).{0} failure message" + Environment.NewLine + "****" + Environment.NewLine + "{2}" + Environment.NewLine + "****",
                //                        method.Name,
                //                        testedtype.Name,
                //                        fluExc.Message));
                //            }

                //            if (report != null)
                //            {
                //                report.AddEntry(desc);
                //            }

                //            if (CheckContext.DefaultNegated == false)
                //            {
                //                Log(string.Format("(Forced) Negated test '{0}' should have succeeded, but it failed (method {1}).", specificTest.Name, desc.Signature));
                //            }
                //        }
                //        else
                //        {
                //            Log(string.Format("Failed to parse the method signature {0}", specificTest.Name));
                //        }

                //        return;
                //    }

                //    if (report != null)
                //    {
                //        Log(
                //            string.Format(
                //                "{0} did not generate a fluent check:" + Environment.NewLine + "{1}",
                //                specificTest.Name,
                //                e.InnerException.Message));
                //    }

                //    if (e.InnerException.GetType() != expectedType && expectedType != null)
                //    {
                //        throw;
                //    }
                //}
                //    else
                //    {
                //        if (report != null)
                //        {
                //            Log(string.Format("{0} failed to run:" + Environment.NewLine + "{1}", specificTest.Name, e));
                //        }

                //        throw;
                //    }
            }
            if (CheckContext.DefaultNegated == false)
            {
                throw new ApplicationException($"{specificTest} should have failed when negated.");
            }

        }

        public static void Log(string message)
        {
            Debug.WriteLine(message);
        }

        /// <summary>
        /// Runs all tests found in current assembly.
        /// </summary>
        /// <param name="log">if set to <c>true</c> log the activity (to Debug output).</param>
        public static void RunAllTests(bool log)
        {
            var assembly = typeof(RunnerHelper).GetTypeInfo().Assembly;
            // prevent recursion
            RunAllTests(assembly, log);
        }

        public static void RunAllTests(Assembly assembly, bool log)
        {
            if (inProcess)
            {
                return;
            }

            try
            {
                inProcess = true;

                // get all test fixtures
                foreach (var type in assembly.GetTypes())
                {
                    // enumerate testmethods with expectedexception attribute with an FluentException type
                    var tests =
                        type.GetMethods().Where(method => method.GetCustomAttributes(typeof(TestAttribute), false).Any() 
                                                          || method.GetCustomAttributes(typeof(TestCaseAttribute), false).Any());
                    RunThoseTests(tests, type, null, log);
                }
            }
            finally
            {
                inProcess = false;
            }
        }

        // ReSharper disable once UnusedMember.Local
        private static CheckDescription GetCheckAndType(Exception fluExc)
        {
            // identify failing test
            var trace = new StackTrace(fluExc, true);

            var stackFrames = trace.GetFrames();
            if (stackFrames == null)
            {
                return null;
            }
            // get fluententrypoint stackframe
            var frameIndex = stackFrames.Count() - 2;
            if (frameIndex < 0)
            {
                frameIndex = 0;
            }

            var frame = stackFrames[frameIndex];

            // get method
            var method = frame.GetMethod();

            return CheckDescription.AnalyzeSignature(method);
        }

        private static void RunAllMethodsWithASpecificAttribute(Type type, Type attributeTypeToScan, object test)
        {
            var startup =
                type.GetMethods().Where(method => method.GetCustomAttributes(attributeTypeToScan, false).Any());
            foreach (var methodInfo in startup)
            {
                try
                {
                    methodInfo.Invoke(test, new object[] { });
                }
                catch (Exception e)
                {
                    Log($"Error: {methodInfo.Name} failed, {e.Message}");
                }
            }
        }

    }
}