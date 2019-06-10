using System;
using System.Threading.Tasks;
using Microsoft.Azure.Commands.Kusto.Utilities;
using Microsoft.Azure.Management.Kusto.Models;

namespace Microsoft.Azure.Commands.Kusto.Models
{
    public class PSKustoDataConnection
    {
        public DataConnection dataConnection { get; set; }

        public string Name
        {
            get
            {
                return dataConnection.Name;
            }
        }

        public string Id
        {
            get
            {
                return dataConnection.Id;
            }
        }

        public string Location
        {
            get
            {
                return dataConnection.Location;
            }
        }

        public string Type
        {
            get
            {
                return dataConnection.Type;
            }
        }

        public PSKustoDataConnection(DataConnection dataConnection)
        {
            this.dataConnection = dataConnection;
        }
    }
}
