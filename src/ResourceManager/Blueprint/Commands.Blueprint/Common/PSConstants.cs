using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Commands.Blueprint.Common
{
    public static class PSConstants
    {
        public const string AzureBlueprintAppId = "f71766dc-90d9-4b7d-bd9d-4499c4331c3f";

        public const string OwnerRoleDefinitionId = "/providers/Microsoft.Authorization/roleDefinitions/8e3af657-a8ff-443c-a75c-2fe8c4bcb635";

        public const string SubscriptionScope = "/subscriptions/";

        public const string BlueprintProviderNamespace = "Microsoft.Blueprint1";

        public const string CreateAssignmentShouldProcessString = "Creating blueprint assignment '{0}' in Subscription '{1}'";
        public const string DeleteAssignmentShouldProcessString = "Deleting blueprint assignment '{0}' in Subscription '{1}'";

    }
}
