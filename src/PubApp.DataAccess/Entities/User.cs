﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PubApp.DataAccess.Entities
{
    public class User :
        IdentityUser<int, AppUserLogin, AppUserRole, AppUserClaim>,
        IUser<int>
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(
            UserManager<User, int> manager, string authenticationType)
        {
            var userIdentity = await manager.CreateIdentityAsync(
                this, authenticationType);
            return userIdentity;
        }
    }
}