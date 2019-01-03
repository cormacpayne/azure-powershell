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

using Microsoft.Azure.Management.Blueprint.Models;
using Microsoft.Azure.Commands.Blueprint.Models;
using Microsoft.Azure.Commands.Common.Authentication;
using Microsoft.Azure.Commands.Common.Authentication.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Blueprint;
using Microsoft.Rest.Azure;
using Microsoft.WindowsAzure.Commands.Utilities.Common;
using BlueprintManagement = Microsoft.Azure.Management.Blueprint;

namespace Microsoft.Azure.Commands.Blueprint.Common
{
    public partial class BlueprintClient : IBlueprintClient
    {
        private readonly BlueprintManagement.IBlueprintManagementClient blueprintManagementClient;

        public IAzureSubscription Subscription { get; private set; }

        // Injection point for unit tests
        public BlueprintClient()
        {
        }

        /// <summary>
        /// Construct a BlueprintClient from an IAzureContext
        /// </summary>
        /// <param name="context"></param>
        public BlueprintClient(IAzureContext context)
            : this(context.Subscription,
                   AzureSession.Instance.ClientFactory.CreateArmClient<BlueprintManagement.BlueprintManagementClient>(context, AzureEnvironment.Endpoint.ResourceManager))
        {
        }

        /// <summary>
        /// Construct a BlueprintClient from a subscription and a BlueprintManagementClient.
        /// </summary>
        /// <param name="subscription"></param>
        /// <param name="blueprintManagementClient"></param>
        public BlueprintClient(IAzureSubscription subscription,
                               BlueprintManagement.BlueprintManagementClient blueprintManagementClient)
        {
            // Requires.Argument("blueprintManagementClient", blueprintManagementClient).NotNull();

            this.Subscription = subscription;
            this.blueprintManagementClient = blueprintManagementClient;
        }

        public PSBlueprint GetBlueprint(string mgName, string blueprintName)
        {
            var result = blueprintManagementClient.Blueprints.GetWithHttpMessagesAsync(mgName, blueprintName)
                .GetAwaiter().GetResult();

            return PSBlueprint.FromBlueprintModel(result.Body, mgName);
        }

        public PSBlueprintAssignment GetBlueprintAssignment(string subscriptionId, string blueprintAssignmentName)
        {
            var result = blueprintManagementClient.Assignments.GetWithHttpMessagesAsync(subscriptionId, blueprintAssignmentName).GetAwaiter().GetResult();

            return PSBlueprintAssignment.FromAssignment(result.Body, subscriptionId);
        }

        public PSPublishedBlueprint GetPublishedBlueprint(string mgName, string blueprintName, string version)
        {
            var result = blueprintManagementClient.PublishedBlueprints.GetWithHttpMessagesAsync(mgName, blueprintName, version).GetAwaiter().GetResult();

            return PSPublishedBlueprint.FromPublishedBlueprintModel(result.Body, mgName);
        }

        public PSPublishedBlueprint GetLatestPublishedBlueprint(string mgName, string blueprintName)
        {
            PSPublishedBlueprint latest = null;
            var publishedBlueprints = ListPublishedBlueprints(mgName, blueprintName);

            foreach (var blueprint in publishedBlueprints)
            {
                if (latest == null)
                    latest = blueprint;
                else if (CompareDates(blueprint.Status.LastModified, latest.Status.LastModified) > 0)
                    latest = blueprint;
            }

            return latest;
        }

        public IEnumerable<PSBlueprintAssignment> ListBlueprintAssignments(string subscriptionId)
        {
            var assignments = blueprintManagementClient.Assignments.List(subscriptionId);

            if (!assignments.Any()) throw new Exception(string.Format(
                "Could not find any Blueprints assignments within '{0}'. Please check the Subscription Id and try again.",
                subscriptionId));

            foreach (var assignment in assignments.Select(assignment => PSBlueprintAssignment.FromAssignment(assignment, subscriptionId)))
            {
                yield return assignment;
            }            

            while (!string.IsNullOrEmpty(assignments.NextPageLink))
            {
                assignments = blueprintManagementClient.Assignments.ListNext(assignments.NextPageLink);
                foreach (var assignment in assignments.Select(assignment => PSBlueprintAssignment.FromAssignment(assignment, subscriptionId)))
                {
                    yield return assignment;
                }
            }
        }

        public IEnumerable<PSBlueprint> ListBlueprints(IEnumerable<string> mgList)
        {
            foreach (var mgName in mgList)
            {
                var blueprints = blueprintManagementClient.Blueprints.List(mgName);

                if (!blueprints.Any()) throw new Exception(string.Format(
                    "Could not find any Blueprints within '{0}'. Please check the Management Group Id and try again.",
                    mgName));

                foreach (var bp in blueprints.Select(bp => PSBlueprint.FromBlueprintModel(bp, mgName)))
                {
                    yield return bp;
                }

                while (!string.IsNullOrEmpty(blueprints.NextPageLink))
                {
                    blueprints = blueprintManagementClient.Blueprints.ListNext(blueprints.NextPageLink);
                    foreach (var bp in blueprints.Select(bp => PSBlueprint.FromBlueprintModel(bp, mgName)))
                    {
                        yield return bp;
                    }
                }
            }
        }

        private async Task<IEnumerable<PSPublishedBlueprint>> ListPublishedBlueprintsAsync(string mgName, string blueprintName)
        {
            var list = new List<PSPublishedBlueprint>();
            var response = await blueprintManagementClient.PublishedBlueprints.ListWithHttpMessagesAsync(mgName, blueprintName);

            list.AddRange(response.Body.Select(bp => PSPublishedBlueprint.FromPublishedBlueprintModel(bp, mgName)));

            while (response.Body.NextPageLink != null)
            {
                response = await blueprintManagementClient.PublishedBlueprints.ListNextWithHttpMessagesAsync(response.Body.NextPageLink);
                list.AddRange(response.Body.Select(bp => PSPublishedBlueprint.FromPublishedBlueprintModel(bp, mgName)));
            }

            return list;
        }

        private IEnumerable<PSPublishedBlueprint> ListPublishedBlueprints(string mgName, string blueprintName)
        {
            var list = new List<PSPublishedBlueprint>();

            var publishedBlueprints = blueprintManagementClient.PublishedBlueprints.List(mgName, blueprintName);

            if (!publishedBlueprints.Any()) throw new Exception(string.Format(
                "Could not find a published Blueprint with the name '{0}'. Please check the name and try again.",
                blueprintName));

            list.AddRange(publishedBlueprints.Select(bp => PSPublishedBlueprint.FromPublishedBlueprintModel(bp, mgName)));

            while (publishedBlueprints.NextPageLink != null)
            {
                publishedBlueprints = blueprintManagementClient.PublishedBlueprints.ListNext(publishedBlueprints.NextPageLink);
                list.AddRange(publishedBlueprints.Select(bp => PSPublishedBlueprint.FromPublishedBlueprintModel(bp, mgName)));
            }

            return list;
        }

        public PSBlueprintAssignment DeleteBlueprintAssignment(string subscriptionId, string blueprintAssignmentName)
        {
            var result = blueprintManagementClient.Assignments.DeleteWithHttpMessagesAsync(subscriptionId, blueprintAssignmentName).GetAwaiter().GetResult();

            if (result.Body != null)
            {
                return PSBlueprintAssignment.FromAssignment(result.Body, subscriptionId);
            }

            return null;
        }

        public PSBlueprintAssignment CreateOrUpdateBlueprintAssignment(string subscriptionId, string assignmentName, Assignment assignment)
        {
            var result = blueprintManagementClient.Assignments.CreateOrUpdateWithHttpMessagesAsync(subscriptionId, assignmentName, assignment).GetAwaiter().GetResult();

            if (result.Body != null)
            {
                return PSBlueprintAssignment.FromAssignment(result.Body, subscriptionId);
            }

            return null;
        }

        /// <summary>
        /// Compare to nullable DateTime objects
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns>
        /// An integer value that is less than zero if first is earlier than second, greater than zero if first is later than second,
        /// or equal to zero if first is the same as second.
        /// </returns>
        private static int CompareDates(DateTime? first, DateTime? second)
        {
            if (first == null && second == null)
                return 0;
            else if (first == null)
                return -1;
            else if (second == null)
                return 1;

            return DateTime.Compare(first.Value, second.Value);
        }
    }
}
