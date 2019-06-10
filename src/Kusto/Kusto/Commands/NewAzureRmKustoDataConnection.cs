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

using System;
using System.Management.Automation;
using Microsoft.Azure.Commands.Kusto.Models;
using Microsoft.Azure.Commands.Kusto.Properties;
using Microsoft.Azure.Commands.Kusto.Utilities;
using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;
using Microsoft.Rest.Azure;

namespace Microsoft.Azure.Commands.Kusto.Commands
{
    [Cmdlet(VerbsCommon.New, ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "KustoDataConnection", DefaultParameterSetName = CmdletParametersSet, SupportsShouldProcess = true),
     OutputType(typeof(PSKustoEventHubDataConnection), typeof(PSKustoEventHubDataConnection))]
    public class NewAzureRmKustoDataConnection : KustoCmdletBase
    {
        protected const string ObjectParameterSet = "ByInputObject";
        protected const string ResourceIdParameterSet = "ByResourceId";
        protected const string CmdletParametersSet = "ByNameAndResourceGroup";
        [Parameter(
            ParameterSetName = CmdletParametersSet,
            Mandatory = true,
            HelpMessage = "Name of resource group under which the user wants to retrieve the cluster.")]
        [ResourceGroupCompleter]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(
            ParameterSetName = CmdletParametersSet,
            Mandatory = true,
            HelpMessage = "Name of cluster under which the database exists.")]
        [ValidateNotNullOrEmpty]
        public string ClusterName { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "the name of the database, under which the data connection exists.")]
        public string DatabaseName { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "the name of the DataConnection.")]
        public string Name { get; set; }

        [Parameter(
            ParameterSetName = ResourceIdParameterSet,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Kusto database ResourceID.")]
        public string ResourceId { get; set; }

        [Parameter(
            ParameterSetName = ObjectParameterSet,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "Kusto cluster object.")]
        [ValidateNotNullOrEmpty]
        public PSKustoDatabase InputObject { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The Type of the data connection.")]
        public KustoClient.eDataConnectionType DataConnectionType { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The Resource ID of the event hub.")]
        public string EventHubResourceId { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The Consumer group of the event hub.")]
        public string ConsumerGroup { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The name of the table.")]
        public string TableName { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The name of the mapping rule.")]
        public string MappingRuleName { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The format of the data.")]
        public string DataFormat { get; set; }

        [Parameter(
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The Resource ID of the storage account.")]
        public string StorageAccountResourceId { get; set; }

        public override void ExecuteCmdlet()
        {
            string resourceGroupName = ResourceGroupName;
            string clusterName = ClusterName;
            string databaseName = DatabaseName;
            string dataConnectionName = Name;
            string location = null;
            if (ShouldProcess(Name, Resources.CreateNewKustoDataConnection))
            {
                try
                {
                    if (!string.IsNullOrEmpty(ResourceId))
                    {
                        KustoUtils.GetResourceGroupNameClusterNameAndDatabaseNameFromDatabaseId(ResourceId, out resourceGroupName, out clusterName, out databaseName);
                    }

                    if (InputObject != null)
                    {
                        KustoUtils.GetResourceGroupNameClusterNameAndDatabaseNameFromDatabaseId(InputObject.Id, out resourceGroupName, out clusterName, out databaseName);
                    }

                    var cluser = KustoClient.GetCluster(resourceGroupName, clusterName);
                    if (cluser == null)
                    {
                        throw new CloudException(string.Format(Resources.KustoClusterNotExist, clusterName));
                    }

                    location = cluser.Location;

                    var database = KustoClient.GetDatabase(resourceGroupName, clusterName, databaseName);
                    if (database == null)
                    {
                        throw new CloudException(string.Format(Resources.KustoDatabaseNotExist, databaseName));
                    }

                    var dataConnection = KustoClient.GetDataConnection(resourceGroupName, clusterName, databaseName, dataConnectionName);
                    if (dataConnection != null)
                    {
                        throw new CloudException(string.Format(Resources.KustoDataConnectionExist, dataConnectionName));
                    }
                }
                catch (CloudException ex)
                {
                    if (ex.Body != null && !string.IsNullOrEmpty(ex.Body.Code) && ex.Body.Code == "ResourceNotFound" ||
                        ex.Message.Contains("ResourceNotFound"))
                    {
                        // there are 2 options:
                        // -database does not exists so go ahead and create one
                        // -cluster does not exist, so continue and let the command fail
                    }
                    else if (ex.Body != null && !string.IsNullOrEmpty(ex.Body.Code) &&
                             ex.Body.Code == "ResourceGroupNotFound" || ex.Message.Contains("ResourceGroupNotFound"))
                    {
                        // resource group not found, let create throw error don't throw from here
                    }
                    else
                    {
                        // all other exceptions should be thrown
                        throw;
                    }
                }

                var createdDataconnection = KustoClient.CreateOrUpdateDataConnection(resourceGroupName, clusterName, databaseName, dataConnectionName, DataConnectionType, location, EventHubResourceId, ConsumerGroup, TableName, MappingRuleName, DataFormat, StorageAccountResourceId);
                WriteObject(createdDataconnection);
            }
        }
    }
}
