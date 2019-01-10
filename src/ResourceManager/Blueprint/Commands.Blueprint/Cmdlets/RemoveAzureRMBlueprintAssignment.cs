﻿using Microsoft.Azure.Commands.Blueprint.Models;
using System;
using System.Management.Automation;
using Microsoft.Azure.Commands.Blueprint.Common;
using Microsoft.Azure.Commands.Blueprint.Properties;
using ParameterSetNames = Microsoft.Azure.Commands.Blueprint.Common.PSConstants.ParameterSetNames;
using ParameterHelpMessages = Microsoft.Azure.Commands.Blueprint.Common.PSConstants.ParameterHelpMessages;

namespace Microsoft.Azure.Commands.Blueprint.Cmdlets
{
    [Cmdlet(VerbsCommon.Remove, ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "BlueprintAssignment", SupportsShouldProcess = true)]
    public class RemoveAzureRmBlueprintAssignment : BlueprintCmdletBase
    {
        #region Parameters
        [Parameter(ParameterSetName = ParameterSetNames.DeleteBlueprintAssignmentByName, Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = ParameterHelpMessages.SubscriptionId)]
        [ValidateNotNullOrEmpty]
        public string SubscriptionId { get; set; }

        [Parameter(ParameterSetName = ParameterSetNames.DeleteBlueprintAssignmentByName, Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = ParameterHelpMessages.BlueprintAssignmentName)]
        [ValidateNotNull]
        public string Name { get; set; }

        [Parameter(ParameterSetName = ParameterSetNames.DeleteBlueprintAssignmentByObject, Position = 0, Mandatory = true, ValueFromPipeline = true, HelpMessage = ParameterHelpMessages.BlueprintAssignmentObject)]
        public PSBlueprintAssignment BlueprintAssignmentObject { get; set; }
        #endregion Parameters


        #region Cmdlet Overrides
        public override void ExecuteCmdlet()
        {
            try
            {
                switch (ParameterSetName)
                {
                    case ParameterSetNames.DeleteBlueprintAssignmentByName:
                        if (ShouldProcess(Name, string.Format(Resources.DeleteAssignmentShouldProcessString, Name, SubscriptionId)))
                        {
                            WriteObject(BlueprintClient.DeleteBlueprintAssignment(SubscriptionId, Name));
                        }

                        break;
                    case ParameterSetNames.DeleteBlueprintAssignmentByObject:
                        if (ShouldProcess(BlueprintAssignmentObject.Name, string.Format(Resources.DeleteAssignmentShouldProcessString, BlueprintAssignmentObject.Name,
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
