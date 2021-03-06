﻿#region License
// Copyright 2014 Colin Svingen

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

//    http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Management.Automation;
using Octopus.Client;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;

namespace Octopus_Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Remove a variable set from the Octopus Deploy server.</para>
    /// <para type="description">The Remove-OctoVariableSet cmdlet removes a variable set from the Octopus Deploy server.</para>
    /// </summary>
    /// <example>
    ///   <code>PS C:\>remove-octovariableset VariableSet</code>
    ///   <para>
    ///      Remove the variableset 'VariableSet'.
    ///   </para>
    /// </example>
    [Cmdlet(VerbsCommon.Remove, "VariableSet", DefaultParameterSetName = "ByName")]
    public class RemoveVariableSet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The name of the library variable set to remove.</para>
        /// </summary>
        [Parameter(
            ParameterSetName = "ByName",
            Position = 0,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = true)]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">The Id of the variable set to remove.</para>
        /// </summary>
        [Parameter(
            ParameterSetName = "ById",
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = true)]
        public string Id { get; set; }

        /// <summary>
        /// <para type="description">
        /// Specifies one or more library variableset objects. Enter a variable that contains the objects, 
        /// or type a command or expressionthat gets the objects.
        /// </para>
        /// </summary>
        [Parameter(
            ParameterSetName = "ByObject",
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = true)]
        public LibraryVariableSetResource InputObject { get; set; }

        private IOctopusRepository _octopus;

        /// <summary>
        /// BeginProcessing
        /// </summary>
        protected override void BeginProcessing()
        {
            _octopus = Session.RetrieveSession(this);
        }

        /// <summary>
        /// ProcessRecord
        /// </summary>
        protected override void ProcessRecord()
        {
            switch (ParameterSetName)
            {
                case "ByName":
                    ProcessByName();
                    break;
                case "ById":
                    ProcessById();
                    break;
                case "ByObject":
                    ProcessByObject();
                    break;
                default:
                    throw new Exception("Unknown ParameterSetName: " + ParameterSetName);
            }
        }

        private void ProcessByObject()
        {
            try
            {
                _octopus.LibraryVariableSets.Delete(InputObject);
                WriteVerbose("Deleting variableset: " + InputObject.Name);
            }
            catch (KeyNotFoundException)
            {
                WriteWarning(string.Format("The library variable set '{0}' does not exist.", InputObject.Name));
            }
        }

        private void ProcessById()
        {
            try
            {
                var set = _octopus.LibraryVariableSets.Get(Id);
                WriteVerbose("Deleting variableset: " + set.Name);
                _octopus.LibraryVariableSets.Delete(set);
            }
            catch (OctopusResourceNotFoundException)
            {
                WriteWarning(string.Format("The library variable set with id '{0}' does not exist.", Id));
            }
        }

        private void ProcessByName()
        {
            var set = _octopus.LibraryVariableSets.FindOne(vs => vs.Name.Equals(Name, StringComparison.InvariantCultureIgnoreCase));
            if (set != null)
            {
                WriteVerbose("Deleting variableset: " + set.Name);
                _octopus.LibraryVariableSets.Delete(set);
            }
            else
            {
                WriteWarning(string.Format("The library variable set '{0}' does not exist.", Name));
            }
        }
    }
}
