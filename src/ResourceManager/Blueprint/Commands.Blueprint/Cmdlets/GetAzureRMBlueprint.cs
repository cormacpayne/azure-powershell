using Microsoft.Azure.Management.ManagementGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Commands.Blueprint.Common;
using ParameterSetNames = Microsoft.Azure.Commands.Blueprint.Common.PSConstants.ParameterSetNames;

namespace Microsoft.Azure.Commands.Blueprint.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "Blueprint", DefaultParameterSetName = ParameterSetNames.ListBlueprintByDefaultSet)]
    public class GetAzureRmBlueprint : BlueprintCmdletBase
    {
        #region Parameters

        [Parameter(ParameterSetName = ParameterSetNames.BlueprintByVersion, Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Management Group Id where Blueprint is located.")]
        [Parameter(ParameterSetName = ParameterSetNames.ListBlueprintByDefaultSet, Position = 0, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Management Group Id where Blueprint is located.")]
        [Parameter(ParameterSetName = ParameterSetNames.BlueprintByLatestPublished, Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Management Group Id where Blueprint is located.")]
        [ValidateNotNullOrEmpty]
        public string ManagementGroupId { get; set; }

        [Parameter(ParameterSetName = ParameterSetNames.BlueprintByVersion, Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint definition name.")]
        [Parameter(ParameterSetName = ParameterSetNames.ListBlueprintByDefaultSet, Position = 0, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint definition name.")]
        [Parameter(ParameterSetName = ParameterSetNames.BlueprintByLatestPublished, Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint definition name.")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(ParameterSetName = ParameterSetNames.BlueprintByVersion, Position = 2, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint definition version.")]
        [ValidateNotNullOrEmpty]
        public string Version { get; set; }

        [Parameter(ParameterSetName = ParameterSetNames.BlueprintByLatestPublished, Position = 2, Mandatory = false, HelpMessage = "The latest published Blueprint flag. When set, execution returns the latest published version of Blueprint. Defaults to false. ")]
        public SwitchParameter LatestPublished { get; set; }

        #endregion Parameters

        #region Cmdlet Overrides

        public override void ExecuteCmdlet()
        {
            try
            {
                switch (ParameterSetName)
                {
                    case ParameterSetNames.ListBlueprintByDefaultSet:
                        if (Name == null)
                        {
                            IEnumerable<string> mgList = string.IsNullOrEmpty(ManagementGroupId)
                                ? GetManagementGroupsForCurrentUser()
                                : new[] { ManagementGroupId };

                            foreach (var bp in BlueprintClient.ListBlueprints(mgList))
                                WriteObject(bp);

                        }
                        else
                        {
                            WriteObject(BlueprintClient.GetBlueprint(ManagementGroupId, Name));
                        }
                        break;
                    case ParameterSetNames.BlueprintByVersion:
                        WriteObject((object) BlueprintClient.GetPublishedBlueprint(ManagementGroupId, Name, Version) ?? "No Published Bluepritn found");
                        break;
                    case ParameterSetNames.BlueprintByLatestPublished:
                        WriteObject((object) BlueprintClient.GetLatestPublishedBlueprint(ManagementGroupId, Name) ?? "And error message saying no published bp found");
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
                throw new Exception("We can't find any Management Groups that you have access to");
            }

            return responseList.Select(managementGroup => managementGroup.Name).ToList();
        }
    }
}
