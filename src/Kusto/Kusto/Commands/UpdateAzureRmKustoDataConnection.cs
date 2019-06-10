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
using Microsoft.Rest.Azure;
using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;
using Microsoft.Azure.Management.Kusto.Models;
using Microsoft.WindowsAzure.Commands.Utilities.Common;

namespace Microsoft.Azure.Commands.Kusto.Commands
{
    [Cmdlet("Update", ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "KustoDataConnection", DefaultParameterSetName = CmdletParametersSet,
         SupportsShouldProcess = true),
     OutputType(typeof(PSKustoDatabase))]
    public class UpdateAzureRmKustoDataConnection : KustoCmdletBase
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
            string dataConnectionName = Name;
            string databaseName = DatabaseName;
            string clusterName = ClusterName;
            string resourceGroupName = ResourceGroupName;
            KustoClient.eDataConnectionType dataConnectionType;
            PSKustoDataConnection dataConnection = null;

            if (!string.IsNullOrEmpty(ResourceId))
            {
                KustoUtils.GetResourceGroupNameClusterNameDatabaseNameAndDataConnectionNameFromDataConnectionId(ResourceId, out resourceGroupName, out clusterName, out databaseName, out dataConnectionName);
            }
            else if (InputObject != null)
            {
                KustoUtils.GetResourceGroupNameClusterNameDatabaseNameAndDataConnectionNameFromDataConnectionId(InputObject.Id, out resourceGroupName, out clusterName, out databaseName, out dataConnectionName);
            }

            EnsureDatabaseClusterResourceGroupSpecified(resourceGroupName, clusterName, databaseName);

            if (ShouldProcess(databaseName, Resources.UpdatingKustoDatabase))
            {
                try
                {
                    dataConnection = KustoClient.GetDataConnection(resourceGroupName, clusterName, databaseName, dataConnectionName);
                    if (dataConnection == null)
                    {
                        throw new CloudException(string.Format(Resources.KustoDataConnectionNotExist, dataConnectionName));
                    }

                    dataConnectionType = KustoClient.GetDataConnectionTypeFromDataConnection(dataConnection.dataConnection);
                }
                catch (CloudException ex)
                {
                    if (ex.Body != null && !string.IsNullOrEmpty(ex.Body.Code) && ex.Body.Code == "ResourceNotFound" ||
                        ex.Message.Contains("ResourceNotFound"))
                    {
                        throw new CloudException(string.Format(Resources.KustoDatabaseNotExist, databaseName));
                    }
                    else if (ex.Body != null && !string.IsNullOrEmpty(ex.Body.Code) &&
                             ex.Body.Code == "ResourceGroupNotFound" || ex.Message.Contains("ResourceGroupNotFound"))
                    {
                        throw new CloudException(string.Format(Resources.ResourceGroupNotExist, resourceGroupName));
                    }
                    else
                    {
                        // all other exceptions should be thrown
                        throw;
                    }
                }

                if (this.IsParameterBound(c => c.TableName))
                {
                    if (dataConnectionType == KustoClient.eDataConnectionType.EventGrid)
                    {
                        ((EventGridDataConnection)dataConnection.dataConnection).TableName = TableName;
                    }
                    else if (dataConnectionType == KustoClient.eDataConnectionType.Eventhub)
                    {
                        ((EventHubDataConnection)dataConnection.dataConnection).TableName = TableName;
                    }
                }
                if (this.IsParameterBound(c => c.DataFormat))
                {
                    if (dataConnectionType == KustoClient.eDataConnectionType.EventGrid)
                    {
                        ((EventGridDataConnection)dataConnection.dataConnection).DataFormat = DataFormat;
                    }
                    else if (dataConnectionType == KustoClient.eDataConnectionType.Eventhub)
                    {
                        ((EventHubDataConnection)dataConnection.dataConnection).DataFormat = DataFormat;
                    }
                }
                if (this.IsParameterBound(c => c.MappingRuleName))
                {
                    if (dataConnectionType == KustoClient.eDataConnectionType.EventGrid)
                    {
                        ((EventGridDataConnection)dataConnection.dataConnection).MappingRuleName = MappingRuleName;
                    }
                    else if (dataConnectionType == KustoClient.eDataConnectionType.Eventhub)
                    {
                        ((EventHubDataConnection)dataConnection.dataConnection).MappingRuleName = MappingRuleName;
                    }
                }

                var updatedDataConnection = KustoClient.CreateOrUpdateDataConnection(resourceGroupName, clusterName, databaseName, dataConnectionName, dataConnectionType, dataConnection: dataConnection);
                WriteObject(updatedDataConnection);
            }
        }
    }
}
