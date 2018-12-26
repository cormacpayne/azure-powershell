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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Graph.RBAC;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Graph.RBAC.Models;
using Microsoft.Azure.Management.Authorization;
using Microsoft.Azure.Management.Authorization.Models;
using Microsoft.Azure.Commands.Blueprint.Common;
using Microsoft.Azure.Commands.Common.Authentication.Abstractions;
using Microsoft.Azure.Common.Authentication;
using Microsoft.Azure.Management.Internal.Resources.Models;
using Microsoft.Rest.Azure;
using Microsoft.WindowsAzure.Commands.Utilities.Common;

namespace Microsoft.Azure.Commands.Blueprint.Cmdlets
{
    [Cmdlet(VerbsCommon.New, ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "BlueprintAssignment", SupportsShouldProcess = true)]
    public class NewAzureRmBlueprintAssignment : BlueprintCmdletBase
    {
        #region Class Constants
        // Parameter Set names
        const string CreateBlueprintAssignment = "BlueprintAssignment";
        #endregion

        #region Parameters
        [Parameter(ParameterSetName = CreateBlueprintAssignment, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint assignment name.")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(ParameterSetName = CreateBlueprintAssignment, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint object.")]
        [ValidateNotNull]
        public PSBlueprintBase Blueprint { get; set; }

        [Parameter(ParameterSetName = CreateBlueprintAssignment, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "SubscriptionId to assign the Blueprint. Can be a comma delimited list of subscriptionId strings.")]
        [ValidateNotNullOrEmpty]
        public string[] SubscriptionId { get; set; }

        [Parameter(ParameterSetName = CreateBlueprintAssignment, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Region for managed identity to be created in. Learn more at aka.ms/blueprintmsi")]
        [ValidateNotNullOrEmpty]
        [LocationCompleter("Microsoft.Batch/operations")]
        public string Location { get; set; }

        [Parameter(ParameterSetName = CreateBlueprintAssignment, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Artifact parameters.")]
        [ValidateNotNull]
        public Hashtable Parameters { get; set; }

        [Parameter(ParameterSetName = CreateBlueprintAssignment, Mandatory = false, HelpMessage = "Lock resources. Learn more at aka.ms/blueprintlocks")]
        public PSLockMode? Lock { get; set; }
        #endregion Parameters

        #region Cmdlet Overrides
        public override void ExecuteCmdlet()
        {
            //TODO: Move below to another function block and call through CreateNewAssignment()
            try
            {
                var localAssignment = new Assignment
                {
                    Identity = new ManagedServiceIdentity { Type = "SystemAssigned" },
                    Location = Location,
                    BlueprintId = Blueprint.Id,
                    Locks = new AssignmentLockSettings { Mode = Lock == null ? PSLockMode.None.ToString() : Lock.ToString() },
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
                foreach (var subscription in subscriptionsList)
                {
                    // First Register Blueprint RP and grant owner permission to BP service principal
                    RegisterBlueprintRp(subscription);
                    var servicePrincipal = GetBlueprintSpn();
                    AssignOwnerPermission(subscription, servicePrincipal);

                    //Name should be more verbose here
                    if (ShouldProcess(Name, string.Format(PSConstants.CreateAssignmentShouldProcessString, Name, subscription)))
                    {
                         WriteObject(BlueprintClient.CreateOrUpdateBlueprintAssignment(subscription, Name, localAssignment));
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

        /// <summary>
        /// Register Blueprint RP with subscription in context.
        /// This is a prerequisite for getting Blueprint SPN for this tenant.
        /// </summary>
        /// <param name="subscriptionId"> SubscriptionId passed from the cmdlet</param>
        private void RegisterBlueprintRp(string subscriptionId)
        {
            ResourceManagerClient.SubscriptionId = subscriptionId;

            try
            {
                ResourceManagerClient.Providers.Register(PSConstants.BlueprintProviderNamespace);
            }
            catch (Exception)
            {
               //TODO: check error to report on the status to user
            }
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
                .Where(ra => ra.RoleDefinitionId.EndsWith(PSConstants.OwnerRoleDefinitionId) && string.Equals(ra.Scope, scope, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            if (roleAssignment != null) return;

            try
            {
                AuthorizationManagementClient.RoleAssignments.CreateAsync(scope: scope, roleAssignmentName: Guid.NewGuid().ToString(), parameters: new RoleAssignmentCreateParameters(roleDefinitionId: PSConstants.OwnerRoleDefinitionId, principalId: servicePrincipal.ObjectId)).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                if (ex is CloudException cex && cex.Response.StatusCode != System.Net.HttpStatusCode.Conflict)
                {
                    throw new CloudException(cex.Message);
                }
            }
        }
        #endregion Private Methods
    }
}
