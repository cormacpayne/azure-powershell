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

using System.Management.Automation;
using Microsoft.Azure.Commands.Kusto.Models;
using Microsoft.Azure.Commands.Kusto.Utilities;
using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;

namespace Microsoft.Azure.Commands.Kusto.Commands
{
    [Cmdlet(VerbsCommon.Get, ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "KustoDatabasePrincipals",
         DefaultParameterSetName = ParameterSet),
     OutputType(typeof(PSKustoDatabasePrincipal))]
    public class GetAzureRmKustoDatabasePrincipals : KustoCmdletBase
    {
        protected const string ParameterSet = "ByNameAndResourceGroup";
        protected const string ResourceIdParameterSet = "ByResourceId";
        protected const string ObjectParameterSet = "ByInputObject";

        [Parameter(
            ParameterSetName = ParameterSet,
            Mandatory = true,
            HelpMessage = "Name of resource group under which the the cluster exists.")]
        [ResourceGroupCompleter]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(
            ParameterSetName = ParameterSet,
            Mandatory = true,
            HelpMessage = "Name of cluster under which the database exists.")]
        [ValidateNotNullOrEmpty]
        public string ClusterName { get; set; }

        [Parameter(
            ParameterSetName = ParameterSet,
            Mandatory = true,
            HelpMessage = "the name of the database, under which under which the user wants to retrieve the principals.")]
        public string DatabaseName { get; set; }

        [Parameter(
            ParameterSetName = ResourceIdParameterSet,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Kusto database Resource ID.")]
        public string ResourceId { get; set; }

        [Parameter(
            ParameterSetName = ObjectParameterSet,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "Kusto database object.")]
        [ValidateNotNullOrEmpty]
        public PSKustoDatabase InputObject { get; set; }

        public override void ExecuteCmdlet()
        {
            string resourceGroupName = ResourceGroupName;
            string clusterName = ClusterName;
            string databaseName = DatabaseName;

            if (!string.IsNullOrEmpty(ResourceId))
            {
                KustoUtils.GetResourceGroupNameClusterNameAndDatabaseNameFromDatabaseId(ResourceId, out resourceGroupName, out clusterName, out databaseName);
            }

            if (InputObject != null)
            {
                KustoUtils.GetResourceGroupNameClusterNameAndDatabaseNameFromDatabaseId(InputObject.Id, out resourceGroupName, out clusterName, out databaseName);
            }

            // List all capacities in given resource group if available otherwise all capacities in the subscription
            var list = KustoClient.ListDatabasePrincipals(resourceGroupName, clusterName, databaseName).ToArray();
            WriteObject(list, true);
        }
    }
}