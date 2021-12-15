using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AaaS.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetId(this ClaimsPrincipal user)
        {
            return int.Parse(user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        }
    }
}
