using Microsoft.Azure.Commands.Blueprint.Common;
using Microsoft.Azure.Commands.Common.Authentication;
using Microsoft.Azure.Commands.Common.Authentication.Abstractions;
using Microsoft.Azure.Commands.ResourceManager.Common;
using Microsoft.Azure.Management.ManagementGroups;
using System;
using Microsoft.Azure.Graph.RBAC;
using Microsoft.Azure.Management.Authorization;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Rest;

namespace Microsoft.Azure.Commands.Blueprint.Cmdlets
{
    public class BlueprintCmdletBase : AzureRMCmdlet
    {
        #region Properties
        /// <summary>
        /// The blueprint client.
        /// </summary>
        private IBlueprintClient blueprintClient;

        public IBlueprintClient BlueprintClient
        {
            get
            {
                return blueprintClient = blueprintClient ?? new BlueprintClient(DefaultProfile.DefaultContext);
            }
            set => blueprintClient = value;
        }

        private IManagementGroupsAPIClient managementGroupsApiClient;
        public IManagementGroupsAPIClient ManagementGroupsClient
        {
            get
            {
                return managementGroupsApiClient = managementGroupsApiClient ??
                            AzureSession.Instance.ClientFactory.CreateArmClient<ManagementGroupsAPIClient>(DefaultProfile.DefaultContext,
                                                                                                           AzureEnvironment.Endpoint.ResourceManager);
            }
            set => managementGroupsApiClient = value;
        }

        private IGraphRbacManagementClient graphRbacManagementClient;

        public IGraphRbacManagementClient GraphRbacManagementClient
        {
            get
            {

                graphRbacManagementClient = graphRbacManagementClient ?? AzureSession.Instance.ClientFactory.CreateArmClient<GraphRbacManagementClient>(DefaultProfile.DefaultContext, AzureEnvironment.Endpoint.Graph);

                graphRbacManagementClient.TenantID = DefaultProfile.DefaultContext.Tenant.Id.ToString();

                return graphRbacManagementClient;
            }
            set => graphRbacManagementClient = value;
        }

        private IAuthorizationManagementClient authorizationManagementClient;

        public IAuthorizationManagementClient AuthorizationManagementClient
        {
            get
            {
                return authorizationManagementClient = authorizationManagementClient ?? AzureSession.Instance.ClientFactory.CreateArmClient<AuthorizationManagementClient>(DefaultProfile.DefaultContext, AzureEnvironment.Endpoint.ResourceManager);
 
            }
            set => authorizationManagementClient = value;
        }

        private ServiceClientCredentials clientCredentials;
        public ServiceClientCredentials ClientCredentials
        {
            get
            {
                return clientCredentials = clientCredentials ?? AzureSession.Instance.AuthenticationFactory.GetServiceClientCredentials(DefaultProfile.DefaultContext,
                    AzureEnvironment.Endpoint.ResourceManager);

            }
        }

        private IResourceManagementClient resourceManagerClient;
        public IResourceManagementClient ResourceManagerClient
        { 
            get
            {
                return resourceManagerClient = resourceManagerClient ?? new ResourceManagementClient(
                                                   DefaultProfile.DefaultContext.Environment.GetEndpointAsUri(AzureEnvironment.Endpoint.ResourceManager),
                                                   ClientCredentials);
            }
            set => resourceManagerClient = value;
        }

        #endregion Properties

        #region Cmdlet Overrides
        protected override void WriteExceptionError(Exception ex)
        {
            var aggEx = ex as AggregateException;

            if (aggEx != null && aggEx.InnerExceptions != null)
            {
                foreach (var e in aggEx.Flatten().InnerExceptions)
                {
                    WriteExceptionError(e);
                }

                return;
            }

            throw ex;
        }
        #endregion Cmdlet Overrides

    }
}
