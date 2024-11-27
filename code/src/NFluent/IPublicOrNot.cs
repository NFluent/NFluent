﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="IPublicOrNot.cs" company="NFluent">
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
    /// <summary>
    ///     Allow to precise public or non public as a scope
    /// </summary>
    public interface IPublicOrNot
    {
        /// <summary>
        ///     Allow to scope to public.
        /// </summary>
#pragma warning disable CA1716
        IFieldsOrProperties Public { get; }
#pragma warning restore CA1716

        /// <summary>
        ///     Allow to scope to non public member (private, protected, internal)
        /// </summary>
        IFieldsOrProperties NonPublic { get; }

        /// <summary>
        ///     Allow all members
        /// </summary>
        IFieldsOrProperties All { get; }
    }
}