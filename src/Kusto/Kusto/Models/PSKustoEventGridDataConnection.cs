

using System;
using System.Threading.Tasks;
using Microsoft.Azure.Commands.Kusto.Utilities;
using Microsoft.Azure.Management.Kusto.Models;

namespace Microsoft.Azure.Commands.Kusto.Models
{
    public class PSKustoEventGridDataConnection : PSKustoDataConnection
    {

        public string MappingRuleName
        {
            get
            {
                return ((EventGridDataConnection)dataConnection).MappingRuleName;
            }
        }

        public string ConsumerGroup
        {
            get
            {
                return ((EventGridDataConnection)dataConnection).ConsumerGroup;
            }
        }

        public string DataFormat
        {
            get
            {
                return ((EventGridDataConnection)dataConnection).DataFormat;
            }
        }

        public string EventHubResourceId
        {
            get
            {
                return ((EventGridDataConnection)dataConnection).EventHubResourceId;
            }
        }

        public string TableName
        {
            get
            {
                return ((EventGridDataConnection)dataConnection).TableName;
            }
        }

        public string StorageAccountResourceId
        {
            get
            {
                return ((EventGridDataConnection)dataConnection).StorageAccountResourceId;
            }
        }

        public PSKustoEventGridDataConnection(EventGridDataConnection dataConnection) :
            base(dataConnection)
        {
        }
    }
}
