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
using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;
using Microsoft.Azure.Management.Blueprint;
using Microsoft.Azure.Management.Blueprint.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Graph.RBAC;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Graph.RBAC.Models;
using Microsoft.Azure.Management.Authorization;
using Microsoft.Azure.Management.Authorization.Models;
using Microsoft.Azure.Commands.Blueprint.Common;
using Microsoft.Rest.Azure;

namespace Microsoft.Azure.Commands.Blueprint.Cmdlets
{
    [Cmdlet(VerbsCommon.New, ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "BlueprintAssignment", SupportsShouldProcess = true)]
    public class NewAzureRmBlueprintAssignment : BlueprintCmdletBase
    {
        #region Class Constants
        // Parameter Set names
        const string CreateUpdateBlueprintAssignment = "BlueprintAssignment";
        #endregion

        #region Parameters
        [Parameter(ParameterSetName = CreateUpdateBlueprintAssignment, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint assignment name.")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(ParameterSetName = CreateUpdateBlueprintAssignment, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint definition object.")]
        [ValidateNotNull]
        public PSBlueprintBase Blueprint { get; set; }

        [Parameter(ParameterSetName = CreateUpdateBlueprintAssignment, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Subscription ID to assign Blueprint. Can be a comma delimited list of subscription ID strings.")]
        [ValidateNotNullOrEmpty]
        public string[] SubscriptionId { get; set; }

        [Parameter(ParameterSetName = CreateUpdateBlueprintAssignment, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Region for managed identity to be created in. Learn more at aka.ms/blueprintmsi")]
        [ValidateNotNullOrEmpty]
        public string Location { get; set; }

        [Parameter(ParameterSetName = CreateUpdateBlueprintAssignment, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Artifact parameters.")]
        [ValidateNotNull]
        public Hashtable Parameters { get; set; }

        [Parameter(ParameterSetName = CreateUpdateBlueprintAssignment, Mandatory = false, HelpMessage = "Lock resources. Learn more at aka.ms/blueprintlocks")]
        public SwitchParameter Lock { get; set; }
        #endregion Parameters

        #region Cmdlet Overrides
        public override void ExecuteCmdlet()
        {
            //TODO: Move below to another function block and call through CreateNewAssignment()
            try
            {
                // Create an assignment object
                AssignmentLockSettings lockSettings = new AssignmentLockSettings { Mode = PSLockMode.None.ToString() };
                if (Lock)
                    lockSettings.Mode = PSLockMode.AllResources.ToString();

                var localAssignment = new Assignment
                {
                    Identity = new ManagedServiceIdentity { Type = "SystemAssigned" },
                    Location = Location,
                    BlueprintId = Blueprint.Id,
                    Locks = lockSettings,
                    Parameters = new Dictionary<string, ParameterValueBase>(),
                    ResourceGroups = new Dictionary<string, ResourceGroupValue>()
                };

                if (Parameters != null) // made Parameters optional as they can be entered during the BP definition creation
                {
                    foreach (var key in Parameters.Keys)
                    {
                        var value = new ParameterValue(Parameters[key], null);
                        localAssignment.Parameters.Add(key.ToString(), value);
                    }
                }

                var subscriptionsList =  SubscriptionId ?? new[] { DefaultContext.Subscription.Id };

                // for each subscription, assign Blueprint
                foreach (var sId in subscriptionsList)
                {
                    // First Register Blueprint RP and grant owner permission to BP service principal
                    RegisterBlueprintRp(sId);
                    var servicePrincipal = GetBlueprintSpn();
                    AssignOwnerPermission(sId, servicePrincipal);

                    if (ShouldProcess(Name))
                    {
                        // Then, assign blueprint
                        var assignmentResult = BlueprintClient.CreateOrUpdateBlueprintAssignment(sId, Name, localAssignment);
                        if (assignmentResult != null)
                        {
                            WriteObject(assignmentResult);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteExceptionError(ex);
            }
        }
        #endregion Cmdlet Overrides

        #region Private Methods

        private void RegisterBlueprintRp(string subscriptionId)
        {
            var subscription = DefaultProfile.Subscriptions.Where(x => x.Id == subscriptionId).FirstOrDefault();

            // save current subscription to re-assign later
            var defaultContextSubscription = DefaultProfile.DefaultContext.Subscription;

            // Change DefaultContext's subscription to the one we want to register with since Resource Manager client uses it to register the RP
            DefaultProfile.DefaultContext.Subscription = subscription;
            var response = ResourceManagerSdkClient.Providers.Register("Microsoft.Blueprint");

            // re-assign
            DefaultProfile.DefaultContext.Subscription = defaultContextSubscription;
        }

        private ServicePrincipal GetBlueprintSpn()
        {
            var odataQuery = new Rest.Azure.OData.ODataQuery<ServicePrincipal>(s => s.ServicePrincipalNames.Contains(PSConstants.AzureBlueprintAppId));
            var servicePrincipal = GraphRbacManagementClient.ServicePrincipals.List(odataQuery.ToString())
                .FirstOrDefault();

            return servicePrincipal;
        }

        private void AssignOwnerPermission(string subscriptionId, ServicePrincipal servicePrincipal) { 
            string scope = PSConstants.SubscriptionScope + subscriptionId;

            var filter = new Rest.Azure.OData.ODataQuery<RoleAssignmentFilter>();
            filter.SetFilter(a => a.AssignedTo(servicePrincipal.ObjectId));

            var roleAssignmentList = AuthorizationManagementClient.RoleAssignments.ListForScopeAsync(scope, filter).GetAwaiter().GetResult();

            var roleAssignment = roleAssignmentList?
                .Where(ra => ra.RoleDefinitionId.EndsWith(PSConstants.OwnerRoleDefinitionId) && ra.Scope == scope)
                .FirstOrDefault();

            if (roleAssignment == null)
            {
                try
                {
                    AuthorizationManagementClient.RoleAssignments.CreateAsync(scope: scope, roleAssignmentName: Guid.NewGuid().ToString(), parameters: new RoleAssignmentCreateParameters(roleDefinitionId: PSConstants.OwnerRoleDefinitionId, principalId: servicePrincipal.ObjectId)).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    if (ex is CloudException cex && cex.Body.Message != "The role assignment already exists.")
                    {
                        throw;
                    }
                }
            }
        }
        #endregion Private Methods
    }
}
