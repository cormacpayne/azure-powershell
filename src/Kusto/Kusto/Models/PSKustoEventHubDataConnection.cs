

using System;
using System.Threading.Tasks;
using Microsoft.Azure.Commands.Kusto.Utilities;
using Microsoft.Azure.Management.Kusto.Models;

namespace Microsoft.Azure.Commands.Kusto.Models
{
    public class PSKustoEventHubDataConnection : PSKustoDataConnection
    {

        public string MappingRuleName
        {
            get
            {
                return ((EventHubDataConnection)dataConnection).MappingRuleName;
            }
        }

        public string ConsumerGroup
        {
            get
            {
                return ((EventHubDataConnection)dataConnection).ConsumerGroup;
            }
        }

        public string DataFormat
        {
            get
            {
                return ((EventHubDataConnection)dataConnection).DataFormat;
            }
        }

        public string EventHubResourceId
        {
            get
            {
                return ((EventHubDataConnection)dataConnection).EventHubResourceId;
            }
        }

        public string TableName
        {
            get
            {
                return ((EventHubDataConnection)dataConnection).TableName;
            }
        }

        public PSKustoEventHubDataConnection(EventHubDataConnection dataConnection) :
            base(dataConnection)
        {
        }
    }
}
