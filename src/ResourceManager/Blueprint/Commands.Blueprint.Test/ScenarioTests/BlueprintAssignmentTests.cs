﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Commands.ScenarioTest;
using Microsoft.WindowsAzure.Commands.ScenarioTest;
using Xunit;

namespace Microsoft.Azure.Commands.Blueprint.Test.ScenarioTests
{
    class BlueprintAssignmentTests
    {
        private ServiceManagemenet.Common.Models.XunitTracingInterceptor _logger;

        public BlueprintAssignmentTests(Xunit.Abstractions.ITestOutputHelper output)
        {
            _logger = new ServiceManagemenet.Common.Models.XunitTracingInterceptor(output);
            ServiceManagemenet.Common.Models.XunitTracingInterceptor.AddToContext(_logger);
            TestExecutionHelpers.SetUpSessionAndProfile();
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestBlueprintAssignment()
        {
            TestController.NewInstance.RunPowerShellTest(_logger, "Test-BlueprintAssignment");
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void TestBlueprintByDefaultSetWithMgName()
        {
            TestController.NewInstance.RunPowerShellTest(_logger, "Test-BlueprintWithPiping");
        }
    }
}
