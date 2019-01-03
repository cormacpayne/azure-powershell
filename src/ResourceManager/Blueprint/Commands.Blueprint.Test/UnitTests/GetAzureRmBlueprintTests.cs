using Microsoft.Azure.Commands.Blueprint.Cmdlets;
using Microsoft.Azure.Commands.Blueprint.Common;
using Microsoft.Azure.Commands.Blueprint.Models;
using Microsoft.Azure.Management.Blueprint;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Commands.Common.Test.Mocks;
using Microsoft.WindowsAzure.Commands.Test.Utilities.Common;
using Microsoft.WindowsAzure.Commands.Utilities.Common;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ParameterSetNames = Microsoft.Azure.Commands.Blueprint.Common.PSConstants.ParameterSetNames;

namespace Microsoft.Azure.Commands.Blueprint.Test.UnitTests
{
    [TestClass]
    public class GetAzureBlueprintTest : RMTestBase
    {
        private Mock<IBlueprintClient> _mockBlueprintClient;

        private MockCommandRuntime _mockCommandRuntime;

        private GetAzureRmBlueprint _cmdlet;

        [TestInitialize]
        public void SetupTest()
        {
            _mockBlueprintClient = new Mock<IBlueprintClient>();
            _mockCommandRuntime = new MockCommandRuntime();

            _cmdlet = new GetAzureRmBlueprint
            {
                BlueprintClient = _mockBlueprintClient.Object,
                CommandRuntime = _mockCommandRuntime
            };
        }

        [TestMethod] 
        public void GetBlueprints()
        {
            // Setup
            var mgList = new[] {"AzBlueprintPS"};

            _mockBlueprintClient.Setup(f => f.ListBlueprints(mgList)).Returns((IEnumerable<string> a) => new List<PSBlueprint>());

            // Test
            _cmdlet.ManagementGroupId = mgList[0];
            _cmdlet.SetParameterSet(ParameterSetNames.ListBlueprintByDefaultSet);
            _cmdlet.ExecuteCmdlet();

            // Assert
            _mockBlueprintClient.Verify(f => f.ListBlueprints(mgList), Times.Once());
        }

        [TestMethod]
        public void GetBlueprintByName()
        {
            // Setup
            var mgList = new[] { "AzBlueprintPS" };
            var name = "PowershellTestBlueprint";

            _mockBlueprintClient.Setup(f => f.GetBlueprint(mgList[0], name)).Returns((string a, string b) => new PSBlueprint());

            // Test
            _cmdlet.ManagementGroupId = mgList[0];
            _cmdlet.Name = name;
            _cmdlet.SetParameterSet(ParameterSetNames.ListBlueprintByDefaultSet);
            _cmdlet.ExecuteCmdlet();

            // Assert
            _mockBlueprintClient.Verify(f => f.GetBlueprint(mgList[0], name), Times.Once());
        }

        [TestMethod]
        public void GetBlueprintByLatestPublished()
        {
            // Setup
            var mgList = new[] { "AzBlueprintPS" };
            var name = "PowershellTestBlueprint";

            _mockBlueprintClient.Setup(f => f.GetLatestPublishedBlueprint(mgList[0], name)).Returns((string a, string b) => new PSPublishedBlueprint());

            // Test
            _cmdlet.ManagementGroupId = mgList[0];
            _cmdlet.Name = name;
            _cmdlet.LatestPublished = true;
            _cmdlet.SetParameterSet(ParameterSetNames.BlueprintByLatestPublished);
            _cmdlet.ExecuteCmdlet();

            // Assert
            _mockBlueprintClient.Verify(f => f.GetLatestPublishedBlueprint(mgList[0], name), Times.Once());
        }

        [TestMethod]
        public void GetBlueprintByVersion()
        {
            // Setup
            var mgList = new[] { "AzBlueprintPS" };
            var name = "PowershellTestBlueprint";
            var version = "1.0";

            _mockBlueprintClient.Setup(f => f.GetPublishedBlueprint(mgList[0], name, version)).Returns((string a, string b, string c) => new PSPublishedBlueprint());

            // Test
            _cmdlet.ManagementGroupId = mgList[0];
            _cmdlet.Name = name;
            _cmdlet.Version = version;
            _cmdlet.SetParameterSet(ParameterSetNames.BlueprintByVersion);
            _cmdlet.ExecuteCmdlet();

            // Assert
            _mockBlueprintClient.Verify(f => f.GetPublishedBlueprint(mgList[0], name, version), Times.Once());
        }
    }
}
