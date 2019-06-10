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
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Azure.Commands.Kusto.Models;
using Microsoft.Azure.Commands.Kusto.Properties;
using Microsoft.Azure.Commands.Kusto.Utilities;
using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;
using Microsoft.Rest.Azure;

namespace Microsoft.Azure.Commands.Kusto.Commands
{
    [Cmdlet(VerbsCommon.Remove, ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "KustoDatabasePrincipals", SupportsShouldProcess = true, DefaultParameterSetName = CmdletParametersSet), OutputType(typeof(bool))]
    public class RemoveAzureRmKustoDatabasePrincipals : KustoCmdletBase
    {
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
            Mandatory = true,
            HelpMessage = "List of principals to remove.")]
        public List<PSKustoDatabasePrincipal> DatabasePrincipal { get; set; }



        public override void ExecuteCmdlet()
        {
            string resourceGroupName = ResourceGroupName;
            string clusterName = ClusterName;
            string databaseName = DatabaseName;

            if (ShouldProcess(DatabaseName, Resources.AddKustoDatabasePrincipals))
            {
                try
                {
                    var cluser = KustoClient.GetCluster(resourceGroupName, clusterName);
                    if (cluser == null)
                    {
                        throw new CloudException(string.Format(Resources.KustoClusterNotExist, clusterName));
                    }


                    var database = KustoClient.GetDatabase(resourceGroupName, clusterName, databaseName);
                    if (database == null)
                    {
                        throw new CloudException(string.Format(Resources.KustoDatabaseNotExist, databaseName));
                    }
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
                        // resource group not found, let add throw error don't throw from here
                    }
                    else
                    {
                        // all other exceptions should be thrown
                        throw;
                    }
                }

                var databasePrincipals = KustoClient.DatabasePrincipalsOperation(resourceGroupName, clusterName, databaseName, DatabasePrincipal, false);
                WriteObject(databasePrincipals);
            }
        }
    }
}
