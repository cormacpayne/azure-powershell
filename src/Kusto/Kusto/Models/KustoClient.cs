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

using Microsoft.Azure.Commands.Common.Authentication;
using Microsoft.Azure.Commands.Common.Authentication.Abstractions;
using Microsoft.Azure.Commands.ResourceManager.Common.Tags;
using Microsoft.Azure.Management.Kusto;
using Microsoft.Rest.Azure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Azure.Management.Kusto.Models;
using Microsoft.Azure.Commands.Kusto.Properties;
using Microsoft.Azure.Graph.RBAC.Version1_6.ActiveDirectory;
using Microsoft.WindowsAzure.Commands.Utilities.Common;

namespace Microsoft.Azure.Commands.Kusto.Models
{
    public class KustoClient
    {
        private const string clusterType = "Microsoft.Kusto/Clusters";
        private readonly KustoManagementClient _client;
        private readonly Guid _subscriptionId;
        private readonly string _currentUser;

        public KustoClient(IAzureContext context)
        {
            if (context == null)
            {
                throw new ApplicationException(Resources.InvalidDefaultSubscription);
            }

            _subscriptionId = context.Subscription.GetId();
            _client = AzureSession.Instance.ClientFactory.CreateArmClient<KustoManagementClient>(
                context,
                AzureEnvironment.Endpoint.ResourceManager);
            _currentUser = context.Account.Id;
        }

        public KustoClient()
        {
        }

        #region Cluster Related Operations

        public CheckNameResult CheckClusterNameAvailability(string clusterName, string location)
        {
            return _client.Clusters.CheckNameAvailability(location, new ClusterCheckNameRequest(clusterName));
        }
        public PSKustoCluster CreateOrUpdateCluster(string resourceGroupName,
            string clusterName,
            string location,
            string skuName = null,
            int? capacity = null,
            Hashtable customTags = null,
            bool clusterExists = false)
        {
            if (string.IsNullOrEmpty(resourceGroupName))
            {
                resourceGroupName = GetResourceGroupByCluster(clusterName);
            }

            var tags = (customTags != null)
                ? TagsConversionHelper.CreateTagDictionary(customTags, true)
                : null;

            Cluster newOrUpdatedCluster;
            if (clusterExists)
            {
                var updateParameters = new ClusterUpdate(sku: new AzureSku(skuName, capacity));
                newOrUpdatedCluster = _client.Clusters.Update(resourceGroupName, clusterName, updateParameters);
            }
            else
            {
                newOrUpdatedCluster = _client.Clusters.CreateOrUpdate(
                    resourceGroupName,
                    clusterName,
                    new Cluster()
                    {
                        Location = location,
                        Tags = tags,
                        Sku = new AzureSku(skuName, capacity),
                    });
            }

            return new PSKustoCluster(newOrUpdatedCluster);
        }

        public void DeleteCluster(string resourceGroupName, string clusterName)
        {
            if (string.IsNullOrEmpty(resourceGroupName))
            {
                resourceGroupName = GetResourceGroupByCluster(clusterName);
            }

            _client.Clusters.Delete(resourceGroupName, clusterName);
        }

        public void SuspendKustoCluster(string resourceGroupName, string clusterName)
        {
            if (string.IsNullOrEmpty(resourceGroupName))
            {
                resourceGroupName = GetResourceGroupByCluster(clusterName);
            }

            _client.Clusters.Stop(resourceGroupName, clusterName);
        }

        public void ResumeKustoCluster(string resourceGroupName, string clusterName)
        {
            if (string.IsNullOrEmpty(resourceGroupName))
            {
                resourceGroupName = GetResourceGroupByCluster(clusterName);
            }

            _client.Clusters.Start(resourceGroupName, clusterName);
        }

        public bool CheckIfClusterExists(string resourceGroupName, string clusterName, out PSKustoCluster cluster)
        {
            try
            {
                cluster = GetCluster(resourceGroupName, clusterName);
                return true;
            }
            catch (CloudException ex)
            {
                if ((ex.Response != null && ex.Response.StatusCode == HttpStatusCode.NotFound) || ex.Message.Contains(string.Format(Properties.Resources.FailedToDiscoverResourceGroup, clusterName,
                    _subscriptionId)))
                {
                    cluster = null;
                    return false;
                }

                throw;
            }
        }

        public PSKustoCluster GetCluster(string resourceGroupName, string clusterName)
        {
            if (string.IsNullOrEmpty(resourceGroupName))
            {
                resourceGroupName = GetResourceGroupByCluster(clusterName);
            }

            return new PSKustoCluster(_client.Clusters.Get(resourceGroupName, clusterName));
        }

        public List<PSKustoCluster> ListClusters(string resourceGroupName)
        {
            var clusters = new List<PSKustoCluster>();
            var response = string.IsNullOrEmpty(resourceGroupName) ?
                _client.Clusters.List() :
                _client.Clusters.ListByResourceGroup(resourceGroupName);

            response.ToList().ForEach(capacity => clusters.Add(new PSKustoCluster(capacity)));

            return clusters;
        }

        private string GetResourceGroupByCluster(string clusterName)
        {
            try
            {
                var acctId =
                    ListClusters(null)
                        .Find(x => x.Name.Equals(clusterName, StringComparison.InvariantCultureIgnoreCase))
                        .Id;
                var rgStart = acctId.IndexOf("resourceGroups/", StringComparison.InvariantCultureIgnoreCase) + ("resourceGroups/".Length);
                var rgLength = (acctId.IndexOf("/providers/", StringComparison.InvariantCultureIgnoreCase)) - rgStart;
                return acctId.Substring(rgStart, rgLength);
            }
            catch
            {
                throw new CloudException(string.Format(Resources.FailedToDiscoverResourceGroup, clusterName, _subscriptionId));
            }
        }
        #endregion

        #region Database Related Operations
        public PSKustoDatabase GetDatabase(string resourceGroupName, string clusterName, string databaseName)
        {
            if (string.IsNullOrEmpty(resourceGroupName))
            {
                resourceGroupName = GetResourceGroupByCluster(clusterName);
            }

            return new PSKustoDatabase(_client.Databases.Get(resourceGroupName, clusterName, databaseName));
        }

        public PSKustoDatabase CreateOrUpdateDatabase(string resourceGroupName,
            string clusterName,
            string databaseName,
            TimeSpan? hotCachePeriod,
            TimeSpan? softDeletePeriod,
            string location,
            bool databaseExists = false)
        {
            if (string.IsNullOrEmpty(resourceGroupName))
            {
                resourceGroupName = GetResourceGroupByCluster(clusterName);
            }

            Database newOrUpdatedDatabase;
            if (databaseExists)
            {
                var updateParameters = new DatabaseUpdate() { HotCachePeriod = softDeletePeriod };
                newOrUpdatedDatabase = _client.Databases.Update(resourceGroupName, clusterName, databaseName, updateParameters);
            }
            else
            {
                newOrUpdatedDatabase = _client.Databases.CreateOrUpdate(
                    resourceGroupName,
                    clusterName,
                    databaseName,
                    new Database()
                    {
                        HotCachePeriod = hotCachePeriod,
                        SoftDeletePeriod = softDeletePeriod,
                        Location = location
                    });
            }

            return new PSKustoDatabase(newOrUpdatedDatabase);
        }

        public void DeleteDatabase(string resourceGroupName, string clusterName, string databaseName)
        {
            if (string.IsNullOrEmpty(resourceGroupName))
            {
                resourceGroupName = GetResourceGroupByCluster(clusterName);
            }

            _client.Databases.Delete(resourceGroupName, clusterName, databaseName);
        }

        public List<PSKustoDatabase> ListDatabases(string resourceGroupName, string clusterName)
        {
            var databases = new List<PSKustoDatabase>();
            var response = _client.Databases.ListByCluster(resourceGroupName, clusterName);

            response.ToList().ForEach(capacity => databases.Add(new PSKustoDatabase(capacity)));

            return databases;
        }



        public bool CheckIfDatabaseExists(string resourceGroupName, string clusterName, string databaseName, out PSKustoDatabase database)
        {
            try
            {
                database = GetDatabase(resourceGroupName, clusterName, databaseName);
                return true;
            }
            catch (CloudException ex)
            {
                if ((ex.Response != null && ex.Response.StatusCode == HttpStatusCode.NotFound) || ex.Message.Contains(string.Format(Properties.Resources.FailedToDiscoverResourceGroup, clusterName,
                        _subscriptionId)))
                {
                    database = null;
                    return false;
                }

                throw;
            }
        }

        #endregion

        #region Data Connection Related Operations

        public List<PSKustoDataConnection> ListDataConnections(string resourceGroupName, string clusterName, string databaseName)
        {
            var dataConnections = new List<PSKustoDataConnection>();
            var response = _client.DataConnections.ListByDatabase(resourceGroupName, clusterName, databaseName);
            var list = response.ToList();
            list.Where(capacity => capacity is EventGridDataConnection).ForEach(capacity => dataConnections.Add(new PSKustoEventGridDataConnection(capacity as EventGridDataConnection)));
            list.Where(capacity => capacity is EventHubDataConnection).ForEach(capacity => dataConnections.Add(new PSKustoEventHubDataConnection(capacity as EventHubDataConnection)));

            return dataConnections;
        }

        public PSKustoDataConnection GetDataConnection(string resourceGroupName, string clusterName, string databaseName, string dataConnectionName)
        {
            if (string.IsNullOrEmpty(resourceGroupName))
            {
                resourceGroupName = GetResourceGroupByCluster(clusterName);
            }

            try
            {
                return new PSKustoDataConnection(_client.DataConnections.Get(resourceGroupName, clusterName,
                    databaseName, dataConnectionName));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public PSKustoDataConnection CreateOrUpdateDataConnection(string resourceGroupName,
            string clusterName,
            string databaseName,
            string dataConnectionName,
            eDataConnectionType dataConnectionType,
            string location = null,
            string eventHubResourceId = null,
            string consumerGroup = null,
            string tableName = null,
            string mappingRuleName = null,
            string dataFormat = null,
            string storageAccountResourceId = null,
            PSKustoDataConnection dataConnection = null)
        {
            if (string.IsNullOrEmpty(resourceGroupName))
            {
                resourceGroupName = GetResourceGroupByCluster(clusterName);
            }

            DataConnection dataConnectionParameters = null;
            DataConnection returnValue = null;

            switch (dataConnectionType)
            {
                case eDataConnectionType.Eventhub:
                    dataConnectionParameters = new EventHubDataConnection(eventHubResourceId: eventHubResourceId, consumerGroup: consumerGroup, location: location, tableName: tableName, mappingRuleName: mappingRuleName, dataFormat: dataFormat);
                    break;
                case eDataConnectionType.EventGrid:
                    dataConnectionParameters = new EventGridDataConnection(storageAccountResourceId: storageAccountResourceId, eventHubResourceId: eventHubResourceId, consumerGroup: consumerGroup, location: location, tableName: tableName, mappingRuleName: mappingRuleName, dataFormat: dataFormat);
                    break;
            }

            if (dataConnection != null)
            {
                dataConnectionParameters = dataConnection.dataConnection;
            }

            returnValue = _client.DataConnections.CreateOrUpdate(resourceGroupName, clusterName, databaseName, dataConnectionName, dataConnectionParameters);


            return new PSKustoDataConnection(returnValue);
        }

        public void DeleteDataConnection(string resourceGroupName, string clusterName, string databaseName, string dataConnectionName)
        {
            if (string.IsNullOrEmpty(resourceGroupName))
            {
                resourceGroupName = GetResourceGroupByCluster(clusterName);
            }

            _client.DataConnections.Delete(resourceGroupName, clusterName, databaseName, dataConnectionName);
        }

        #endregion


        #region Database Principals Related Operations

        public List<PSKustoDatabasePrincipal> ListDatabasePrincipals(string resourceGroupName, string clusterName, string databaseName)
        {
            var dataBasePrincipals = new List<PSKustoDatabasePrincipal>();
            var response = _client.Databases.ListPrincipals(resourceGroupName, clusterName, databaseName);

            response.ToList().ForEach(capacity => dataBasePrincipals.Add(new PSKustoDatabasePrincipal(capacity)));

            return dataBasePrincipals;
        }

        public List<PSKustoDatabasePrincipal> DatabasePrincipalsOperation(string resourceGroupName,
            string clusterName,
            string databaseName,
            List<PSKustoDatabasePrincipal> databasePrincipal,
            bool isAddCommand)
        {
            if (string.IsNullOrEmpty(resourceGroupName))
            {
                resourceGroupName = GetResourceGroupByCluster(clusterName);
            }

            List<PSKustoDatabasePrincipal> returnValue = new List<PSKustoDatabasePrincipal>();

            var dbPrincipals = databasePrincipal.Select(psKustoDatabase =>
                new DatabasePrincipal(psKustoDatabase.Role, psKustoDatabase.Name, psKustoDatabase.Type, psKustoDatabase.Fqn, psKustoDatabase.Email, psKustoDatabase.AppId)).ToList();

            DatabasePrincipalListResult response = null;
            if (isAddCommand)
            {
                response = _client.Databases.AddPrincipals(resourceGroupName, clusterName, databaseName, new DatabasePrincipalListRequest(dbPrincipals));
            }
            else
            {
                response = _client.Databases.RemovePrincipals(resourceGroupName, clusterName, databaseName, new DatabasePrincipalListRequest(dbPrincipals));
            }

            response.Value.ToList().ForEach(capacity => returnValue.Add(new PSKustoDatabasePrincipal(capacity)));

            return returnValue;
        }

        public eDataConnectionType GetDataConnectionTypeFromDataConnection(DataConnection dataConnection)
        {
            var eventhubDataConnection = dataConnection as EventHubDataConnection;
            if (eventhubDataConnection != null)
            {
                return eDataConnectionType.Eventhub;
            }

            var eventGridDataConnection = dataConnection as EventGridDataConnection;
            if (eventGridDataConnection != null)
            {
                return eDataConnectionType.EventGrid;
            }

            throw new ArgumentException("DataConnectionType must be eventhub or eventgrid");
        }

        #endregion

        public enum eDataConnectionType
        {
            Eventhub,
            EventGrid
        }
    }
}