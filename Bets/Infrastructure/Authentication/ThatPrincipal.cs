using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;

namespace ThatAuthentication
{
    public class ThatPrincipal : IPrincipal
    {
        public ThatPrincipal(IIdentity identity)
        {
            Identity = identity;
        }

        public bool IsInRole(string role)
        {
            if (String.Compare(role, "admin", true) == 0)
            {
                return (Identity.Name == "szilardd");
            }
            else
            {
                return false;
            }
        }

        public IIdentity Identity
        {
            get;
            private set;
        }
    }
}