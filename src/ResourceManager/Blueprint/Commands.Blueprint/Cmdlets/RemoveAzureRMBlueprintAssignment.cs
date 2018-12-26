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
using Microsoft.Azure.Management.Blueprint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Commands.Blueprint.Common;

namespace Microsoft.Azure.Commands.Blueprint.Cmdlets
{
    [Cmdlet(VerbsCommon.Remove, ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "BlueprintAssignment", SupportsShouldProcess = true)]
    public class RemoveAzureRmBlueprintAssignment : BlueprintCmdletBase
    {
        #region Class Constants
        // Parameter Set names
        private const string DeleteBlueprintAssignmentByName = "DeleteBlueprintAssignmentByName";
        private const string DeleteBlueprintAssignmentByObject = "DeleteBlueprintAssignmentByObject";
        #endregion Class Constants

        #region Parameters
        [Parameter(ParameterSetName = DeleteBlueprintAssignmentByName, Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Subscription Id.")]
        [ValidateNotNullOrEmpty]
        public string SubscriptionId { get; set; }

        [Parameter(ParameterSetName = DeleteBlueprintAssignmentByName, Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint assignment name.")]
        [ValidateNotNull]
        public string Name { get; set; }

        [Parameter(ParameterSetName = DeleteBlueprintAssignmentByObject, Position = 1, Mandatory = true, ValueFromPipeline = true, HelpMessage = "Blueprint assignment object.")]
        public PSBlueprintAssignment BlueprintAssignmentObject { get; set; }
        #endregion Parameters


        #region Cmdlet Overrides
        public override void ExecuteCmdlet()
        {
            try
            {
                switch (ParameterSetName)
                {
                    case DeleteBlueprintAssignmentByName:
                        if (ShouldProcess(Name, string.Format(PSConstants.DeleteAssignmentShouldProcessString, Name, SubscriptionId)))
                        {
                            WriteObject(BlueprintClient.DeleteBlueprintAssignment(SubscriptionId, Name));
                        }

                        break;
                    case DeleteBlueprintAssignmentByObject:
                        if (ShouldProcess(BlueprintAssignmentObject.Name, string.Format(PSConstants.DeleteAssignmentShouldProcessString, BlueprintAssignmentObject.Name,
                                BlueprintAssignmentObject.SubscriptionId)))
                        {
                            WriteObject(BlueprintClient.DeleteBlueprintAssignment(BlueprintAssignmentObject.SubscriptionId, BlueprintAssignmentObject.Name));
                        }
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
    }
}
