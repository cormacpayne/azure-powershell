// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using Microsoft.Azure.Commands.Blueprint.Models;
using Microsoft.Azure.Commands.Common.Authentication;
using Microsoft.Azure.Commands.Common.Authentication.Abstractions;
using Microsoft.Azure.Management.Blueprint;
using Microsoft.Azure.Management.ManagementGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Commands.Blueprint.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "Blueprint", DefaultParameterSetName = BlueprintByDefaultSet)]
    public class GetAzureRmBlueprint : BlueprintCmdletBase
    {
        #region Class Constants

        // Parameter Set names
        private const string BlueprintByDefaultSet = "BlueprintByDefaultSet";
        private const string BlueprintByLatestPublished = "BlueprintByLatestPublished";
        private const string BlueprintByVersion = "BlueprintByVersion";

        #endregion Class Constants

        #region Parameters

        [Parameter(ParameterSetName = BlueprintByVersion, Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Management Group Id where Blueprint is located.")]
        [Parameter(ParameterSetName = BlueprintByDefaultSet, Position = 0, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Management Group Id where Blueprint is located.")]
        [Parameter(ParameterSetName = BlueprintByLatestPublished, Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Management Group Id where Blueprint is located.")]
        [ValidateNotNullOrEmpty]
        public string ManagementGroupId { get; set; }

        [Parameter(ParameterSetName = BlueprintByVersion, Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint definition name.")]
        [Parameter(ParameterSetName = BlueprintByDefaultSet, Position = 0, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint definition name.")]
        [Parameter(ParameterSetName = BlueprintByLatestPublished, Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint definition name.")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(ParameterSetName = BlueprintByVersion, Position = 2, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint definition version.")]
        [ValidateNotNullOrEmpty]
        public string Version { get; set; }

        [Parameter(ParameterSetName = BlueprintByLatestPublished, Position = 2, Mandatory = false, HelpMessage = "The latest published Blueprint flag. When set, execution returns the latest published version of Blueprint. Defaults to false. ")]
        public SwitchParameter LatestPublished { get; set; }

        #endregion Parameters

        #region Cmdlet Overrides

        public override void ExecuteCmdlet()
        {
            try
            {
                switch (ParameterSetName)
                {
                    case BlueprintByDefaultSet:
                        if (Name == null)
                        {
                            IEnumerable<string> mgList = string.IsNullOrEmpty(ManagementGroupId)
                                ? GetManagementGroupsForCurrentUser()
                                : new string[] { ManagementGroupId };

                            if (mgList?.Count() == 0)
                            {
                                WriteObject("We can't find any Management Groups that you have access to.");
                                return;
                            }

                            foreach (var bp in BlueprintClient.ListBlueprints(mgList))
                                WriteObject(bp);

                        }
                        else
                        {
                            WriteObject(BlueprintClient.GetBlueprint(ManagementGroupId, Name));
                        }
                        break;
                    case BlueprintByVersion:
                        WriteObject(BlueprintClient.GetPublishedBlueprint(ManagementGroupId, Name, Version));
                        break;
                    case BlueprintByLatestPublished:
                        WriteObject(BlueprintClient.GetLatestPublishedBlueprint(ManagementGroupId, Name));
                        break;
                    default:
                        throw new PSInvalidOperationException();
                }
            }
            catch (Exception ex)
            {
                WriteExceptionError(ex);
            }
        }

        #endregion Cmdlet Overrides
        private IEnumerable<string> GetManagementGroupsForCurrentUser()
        {
            var responseList = ManagementGroupsClient.ManagementGroups.List();
            return responseList.Select(managementGroup => managementGroup.Name).ToList();
        }

    }
}
