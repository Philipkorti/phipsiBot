using Enums.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbConnection.Entity;
using Services.DbServices;

namespace Services.Services
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public sealed class GroupsService : Attribute
    {
        private readonly Groups group;

        public GroupsService(Groups group)
        {
            this.group = group;
        }

        public bool IsUserAuthorized(string username)
        {
            User user = UserServices.GetUerByUsername(username);

            if(user.Groups <= group)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
