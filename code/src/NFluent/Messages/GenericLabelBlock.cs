﻿#region File header
// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="GenericLabelBlock.cs" company="">
// //   Copyright 2014 Cyrille Dupuydauby, Thomas PIERRAIN
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
#endregion
using System.Globalization;

namespace NFluent.Messages
{
    internal class GenericLabelBlock
    {
        private readonly string adjective;
        private readonly string adjectiveForMessages;
        private const string Template = "The {0} {1}:";

        internal GenericLabelBlock(string adjective, string adjectiveMessage, EntityNamingLogic namer)
        {
            this.adjective = adjective;
            this.adjectiveForMessages = adjectiveMessage;
            this.EntityLogic = namer;
        }

        /// <summary>
        /// Gets or sets the entity logic.
        /// </summary>
        /// <value>
        /// The entity logic.
        /// </value>
        public EntityNamingLogic EntityLogic { get; set; }

        public static GenericLabelBlock BuildCheckedBlock(EntityNamingLogic namer)
        {
            return new GenericLabelBlock("checked", "checked", namer);
        }

        public static GenericLabelBlock BuildExpectedBlock(EntityNamingLogic namer)
        {
            return new GenericLabelBlock("expected", "expected", namer);
        }

        public static GenericLabelBlock BuildGivenBlock(EntityNamingLogic namer)
        {
            return new GenericLabelBlock("expected", "given", namer);
        }


        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{this.Adjective()} {this.EntityName()}";
        }

        /// <summary>
        /// Customs the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// A custom formatted message.
        /// </returns>
        public string CustomMessage(string message)
        {
            return string.Format(CultureInfo.InvariantCulture, message ?? Template, this.adjectiveForMessages, this.EntityName());
        }

        private string Adjective()
        {
            return this.adjective;
        }

        public string EntityName()
        {
            return this.EntityLogic.EntityName;
        }
    }
}