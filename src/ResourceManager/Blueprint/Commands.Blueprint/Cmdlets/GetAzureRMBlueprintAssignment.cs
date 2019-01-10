using System;
using System.Management.Automation;
using ParameterSetNames = Microsoft.Azure.Commands.Blueprint.Common.PSConstants.ParameterSetNames;
using ParameterHelpMessages = Microsoft.Azure.Commands.Blueprint.Common.PSConstants.ParameterHelpMessages;

namespace Microsoft.Azure.Commands.Blueprint.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "BlueprintAssignment")]
    public class GetAzureRmBlueprintAssignment : BlueprintCmdletBase
    {
        [Parameter(ParameterSetName = ParameterSetNames.BlueprintAssignmentByName, Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = ParameterHelpMessages.SubscriptionId)]
        [Parameter(ParameterSetName = ParameterSetNames.ListBlueprintAssignmentBySubscription, Position = 0, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = ParameterHelpMessages.SubscriptionId)]
        [ValidateNotNullOrEmpty]
        public string SubscriptionId { get; set; }

        #region Parameters
        [Parameter(ParameterSetName = ParameterSetNames.BlueprintAssignmentByName, Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = ParameterHelpMessages.BlueprintAssignmentName)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }
        #endregion Parameters

        #region Cmdlet Overrides
        public override void ExecuteCmdlet()
        {
            try
            {
                switch (ParameterSetName) {
                    case ParameterSetNames.ListBlueprintAssignmentBySubscription:
                        foreach (var assignment in BlueprintClient.ListBlueprintAssignments(SubscriptionId ?? DefaultContext.Subscription.Id))
                            WriteObject(assignment);
                        break;
                    case ParameterSetNames.BlueprintAssignmentByName:
                        WriteObject(BlueprintClient.GetBlueprintAssignment(SubscriptionId ?? DefaultContext.Subscription.Id, Name));
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
