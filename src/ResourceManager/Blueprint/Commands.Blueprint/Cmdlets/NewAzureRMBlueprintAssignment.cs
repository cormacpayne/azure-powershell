using Microsoft.Azure.Commands.Blueprint.Models;
using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;
using Microsoft.Azure.Management.Blueprint.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Azure.Graph.RBAC;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Graph.RBAC.Models;
using Microsoft.Azure.Management.Authorization;
using Microsoft.Azure.Management.Authorization.Models;
using Microsoft.Azure.Commands.Blueprint.Common;
using Microsoft.Rest.Azure;
using ParameterSetNames = Microsoft.Azure.Commands.Blueprint.Common.PSConstants.ParameterSetNames;

namespace Microsoft.Azure.Commands.Blueprint.Cmdlets
{
    [Cmdlet(VerbsCommon.New, ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "BlueprintAssignment", SupportsShouldProcess = true)]
    public class NewAzureRmBlueprintAssignment : BlueprintCmdletBase
    {
        #region Parameters
        [Parameter(ParameterSetName = ParameterSetNames.CreateBlueprintAssignment, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint assignment name.")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(ParameterSetName = ParameterSetNames.CreateBlueprintAssignment, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Blueprint object.")]
        [ValidateNotNull]
        public PSBlueprintBase Blueprint { get; set; }

        [Parameter(ParameterSetName = ParameterSetNames.CreateBlueprintAssignment, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "SubscriptionId to assign the Blueprint. Can be a comma delimited list of subscriptionId strings.")]
        [ValidateNotNullOrEmpty]
        public string[] SubscriptionId { get; set; }

        [Parameter(ParameterSetName = ParameterSetNames.CreateBlueprintAssignment, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Region for managed identity to be created in. Learn more at aka.ms/blueprintmsi")]
        [ValidateNotNullOrEmpty]
        [LocationCompleter("Microsoft.Batch/operations")]
        public string Location { get; set; }

        [Parameter(ParameterSetName = ParameterSetNames.CreateBlueprintAssignment, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Artifact parameters.")]
        [ValidateNotNull]
        public Hashtable Parameters { get; set; }

        [Parameter(ParameterSetName = ParameterSetNames.CreateBlueprintAssignment, Mandatory = false, HelpMessage = "Lock resources. Learn more at aka.ms/blueprintlocks")]
        public PSLockMode? Lock { get; set; }
        #endregion Parameters

        #region Cmdlet Overrides
        public override void ExecuteCmdlet()
        {
            try
            {
                CheckIfAssignmentExist();

                var assignment = CreateAssignmentObject();
                var subscriptionsList =  SubscriptionId ?? new[] { DefaultContext.Subscription.Id };

                foreach (var subscription in subscriptionsList)
                {
                    // First Register Blueprint RP and grant owner permission to BP service principal
                    RegisterBlueprintRp(subscription);
                    var servicePrincipal = GetBlueprintSpn();
                    AssignOwnerPermission(subscription, servicePrincipal);

                    if (ShouldProcess(Name, string.Format(PSConstants.CreateAssignmentShouldProcessString, Name, subscription)))
                    {
                         WriteObject(BlueprintClient.CreateOrUpdateBlueprintAssignment(subscription, Name, assignment));
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

        private void CheckIfAssignmentExist()
        {
            var subscriptionsList = SubscriptionId ?? new[] { DefaultContext.Subscription.Id };
            PSBlueprintAssignment existingAssignment = null;

            foreach (var subscription in subscriptionsList)
            {
                existingAssignment = BlueprintClient.GetBlueprintAssignment(subscription, Name);

                if (existingAssignment != null)
                {
                    throw new Exception(string.Format(
                        "An assignment with name '{0}' in Subscription '{1}' already exists. Please use Set-AzBlueprintAssignment to update an existing assignment.",
                        this.Name, this.SubscriptionId));
                }
            }
        }
        private Assignment CreateAssignmentObject()
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

            if (Parameters != null)
            {
                foreach (var key in Parameters.Keys)
                {
                    var value = new ParameterValue(Parameters[key], null);
                    localAssignment.Parameters.Add(key.ToString(), value);
                }
            }

            return localAssignment;
        }

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
