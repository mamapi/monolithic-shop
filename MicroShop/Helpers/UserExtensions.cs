﻿using System.Linq;
using System.Security.Claims;

namespace MicroShop.Helpers
{
    public static class UserExtensions
    {
        public static string GetEmail(this ClaimsPrincipal user)
        {
            // Yes yes i know :D this code sux
            return user.Claims.SingleOrDefault(x => x.Value.Contains("@")).Value;
        }
    }
}