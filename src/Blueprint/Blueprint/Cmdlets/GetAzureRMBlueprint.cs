using Microsoft.Azure.Management.ManagementGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Azure.Commands.Blueprint.Common;
using Microsoft.Azure.Commands.Blueprint.Properties;
using ParameterSetNames = Microsoft.Azure.Commands.Blueprint.Common.PSConstants.ParameterSetNames;
using ParameterHelpMessages = Microsoft.Azure.Commands.Blueprint.Common.PSConstants.ParameterHelpMessages;

namespace Microsoft.Azure.Commands.Blueprint.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "Blueprint", DefaultParameterSetName = ParameterSetNames.ManagementGroupScope)]
    public class GetAzureRmBlueprint : BlueprintCmdletBase
    {
        #region Parameters

        [Parameter(ParameterSetName = ParameterSetNames.BlueprintByVersion, Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = ParameterHelpMessages.ManagementGroupName)]
        [Parameter(ParameterSetName = ParameterSetNames.ManagementGroupScope, Position = 0, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = ParameterHelpMessages.ManagementGroupName)]
        [Parameter(ParameterSetName = ParameterSetNames.BlueprintByLatestPublished, Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = ParameterHelpMessages.ManagementGroupName)]
        [ValidateNotNullOrEmpty]
        public string ManagementGroupName { get; set; }

        [Parameter(ParameterSetName = ParameterSetNames.BlueprintByVersion, Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = ParameterHelpMessages.BlueprintDefinitionName)]
        [Parameter(ParameterSetName = ParameterSetNames.ManagementGroupScope, Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = ParameterHelpMessages.BlueprintDefinitionName)]
        [Parameter(ParameterSetName = ParameterSetNames.BlueprintByLatestPublished, Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = ParameterHelpMessages.BlueprintDefinitionName)]
        [Parameter(ParameterSetName = ParameterSetNames.BlueprintByName, Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = ParameterHelpMessages.BlueprintDefinitionName)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(ParameterSetName = ParameterSetNames.BlueprintByVersion, Position = 2, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = ParameterHelpMessages.BlueprintDefinitionVersion)]
        [ValidateNotNullOrEmpty]
        public string Version { get; set; }

        [Parameter(ParameterSetName = ParameterSetNames.BlueprintByLatestPublished, Position = 2, Mandatory = false, HelpMessage = ParameterHelpMessages.LatestPublishedFlag)]
        public SwitchParameter LatestPublished { get; set; }

        #endregion Parameters

        #region Cmdlet Overrides

        public override void ExecuteCmdlet()
        {
            try
            {
                switch (ParameterSetName)
                {
                    case ParameterSetNames.ManagementGroupScope:
                        if (Name == null)
                        {
                            IEnumerable<string> mgList = string.IsNullOrEmpty(ManagementGroupName)
                                ? GetManagementGroupsForCurrentUser()
                                : new[] { ManagementGroupName };

                            foreach (var bp in BlueprintClient.ListBlueprints(mgList))
                                WriteObject(bp);
                        }
                        else
                        {
                            WriteObject(BlueprintClient.GetBlueprint(ManagementGroupName, Name));
                        }
                        break;
                    case ParameterSetNames.BlueprintByVersion:
                        WriteObject((object) BlueprintClient.GetPublishedBlueprint(ManagementGroupName, Name, Version));
                        break;
                    case ParameterSetNames.BlueprintByLatestPublished:
                        WriteObject((object) BlueprintClient.GetLatestPublishedBlueprint(ManagementGroupName, Name));
                        break;
                    case ParameterSetNames.BlueprintByName:
                        WriteObject("Test");
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

            if (responseList == null || !responseList.Any())
            {
                throw new Exception(Resources.ManagementGroupNotFound);
            }

            return responseList.Select(managementGroup => managementGroup.Name).ToList();
        }
    }
}
