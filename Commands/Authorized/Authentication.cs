using DSharpPlus.CommandsNext;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Commands.Authorized
{
    public class Authentication
    {
        public static bool IsUserAuthorized(object target, string methodName ,string username)
        {
            var attributes = target.GetType().GetMethod(methodName).GetCustomAttribute<GroupsService>();
            return attributes.IsUserAuthorized(username);
        }
    }
}
