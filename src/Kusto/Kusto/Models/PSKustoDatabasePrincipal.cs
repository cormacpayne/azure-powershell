using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Management.Kusto.Models;

namespace Microsoft.Azure.Commands.Kusto.Models
{
    public class PSKustoDatabasePrincipal
    {
        private DatabasePrincipal _databasePrincipal { get; set; }

        public string Role
        {
            get
            {
                return _databasePrincipal.Role;
            }

        }

        public string Name
        {
            get
            {
                return _databasePrincipal.Name;
            }
        }

        public string Type
        {
            get
            {
                return _databasePrincipal.Type;
            }
        }

        public string Fqn
        {
            get
            {
                return _databasePrincipal.Fqn;
            }
        }
       
        public string Email {
            get
            {
                return _databasePrincipal.Email;
            }
        }
     
        public string AppId {
            get
            {
                return _databasePrincipal.AppId;
            }
        }

        public PSKustoDatabasePrincipal(DatabasePrincipal databasePrincipal)
        {
            _databasePrincipal = databasePrincipal;
        }

        public PSKustoDatabasePrincipal(string role, string name, string type, string fqn = null, string email = null, string appId = null)
        {
            _databasePrincipal = new DatabasePrincipal(role,name, type, fqn , email , appId);
        }
    }
}
