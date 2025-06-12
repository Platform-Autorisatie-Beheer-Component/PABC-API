 
using Xunit;
using Microsoft.AspNetCore.Authorization;
using PABC.Server.Auth;
using PABC.Server.Features.GetApplicationRolesPerEntityType; 
using System.Linq;
using System;

namespace PABC.Server.Test.Auth
{
    public class ControllerAuthenticationPolicyTests
    {
        [Theory] 
        [InlineData(typeof(GetApplicationRolesPerEntityTypeController))]
        public void Controller_ShouldHave_ApiKeyPolicy_Authorization(Type controller)
        { 
            var authorizeAttributes = controller
                .GetCustomAttributes(typeof(AuthorizeAttribute), true)
                .OfType<AuthorizeAttribute>()
                .ToList();
             
            var hasCorrectPolicy = authorizeAttributes
                .Any(attr => attr.Policy == ApiKeyAuthentication.Policy); 

            Assert.True(hasCorrectPolicy, $"Controller '{controller.Name}' is missing the required authorization policy: '{ApiKeyAuthentication.Policy}'.");
        }
    }
}
