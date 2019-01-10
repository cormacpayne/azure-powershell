using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Commands.Blueprint.Common
{
    public static class PSConstants
    {
        public const string AzureBlueprintAppId = "f71766dc-90d9-4b7d-bd9d-4499c4331c3f";

        public const string OwnerRoleDefinitionId = "/providers/Microsoft.Authorization/roleDefinitions/8e3af657-a8ff-443c-a75c-2fe8c4bcb635";
        public const string SubscriptionScope = "/subscriptions/";
        public const string BlueprintProviderNamespace = "Microsoft.Blueprint";

        public static class ParameterSetNames
        {
            // Parameter Set names
            public const string ManagementGroupScope = "ManagementGroupScope";
            public const string BlueprintByLatestPublished = "BlueprintByLatestPublished";
            public const string BlueprintByVersion = "BlueprintByVersion";
            public const string BlueprintByName = "BlueprintByName";

            public const string ListBlueprintAssignmentBySubscription = "ListBlueprintAssignmentBySubscription";
            public const string BlueprintAssignmentByName = "BlueprintAssignmentByName";

            public const string CreateBlueprintAssignment = "BlueprintAssignment";

            public const string DeleteBlueprintAssignmentByName = "DeleteBlueprintAssignmentByName";
            public const string DeleteBlueprintAssignmentByObject = "DeleteBlueprintAssignmentByObject";
        }

        public static class ParameterHelpMessages
        {
            public const string SubscriptionId = "Subscription Id.";
            public const string BlueprintAssignmentName = "Blueprint assignment name.";
            public const string BlueprintAssignmentObject = "Blueprint assignment object.";
            public const string BlueprintObject = "Blueprint object.";

            public const string ManagementGroupName = "Management Group Id where Blueprint is located.";
            public const string BlueprintDefinitionName = "Blueprint definition name.";
            public const string BlueprintDefinitionVersion = "Blueprint definition version.";
            public const string LatestPublishedFlag =
                "The latest published Blueprint flag. When set, execution returns the latest published version of Blueprint. Defaults to false.";

            public const string SubscriptionIdToAssign =
                "SubscriptionId to assign the Blueprint. Can be a comma delimited list of subscriptionId strings.";

            public const string Location =
                "Region for managed identity to be created in. Learn more at aka.ms/blueprintmsi";
            public const string Parameters = "Artifact parameters.";
            public const string LockFlag = "Lock resources. Learn more at aka.ms/blueprintlocks";
        }
    }
}
