using System;
using System.Collections.Generic;
using System.Text;
using Anlab.Core.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Test.Helpers
{
    /// <summary>
    /// Mockable UserManager
    /// </summary>
    public class FakeUserManager : UserManager<User>
    {
        public FakeUserManager()
            : base(new Mock<IUserStore<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<User>>().Object,
                new IUserValidator<User>[0],
                new IPasswordValidator<User>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<User>>>().Object)
        { }

    }

    /// <summary>
    /// Mockable SignInManager (May not be needed)
    /// </summary>
    public class FakeSignInManager : SignInManager<User>
    {
        public FakeSignInManager()
            : base(new Mock<FakeUserManager>().Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<ILogger<SignInManager<User>>>().Object,
                new Mock<IAuthenticationSchemeProvider>().Object,
                new Mock<IUserConfirmation<User>>().Object)
        { }
    }

    public class FakeRoleManager : RoleManager<IdentityRole>
    {
        //IRoleStore<IdentityRole> store,
        //IEnumerable<IRoleValidator<IdentityRole>> roleValidators,
        //ILookupNormalizer keyNormalizer,
        //IdentityErrorDescriber errors,
        //ILogger<RoleManager<IdentityRole>> logger) : base(store, roleValidators, keyNormalizer, errors, logger)
        public FakeRoleManager() : base(new Mock<IRoleStore<IdentityRole>>().Object,
            new IRoleValidator<IdentityRole>[0],
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<ILogger<RoleManager<IdentityRole>>>().Object)

        {
        }
    }

    /// <summary>
    /// Mockable SessionStateTempDataProvider
    /// </summary>
    public class FakeSessionStateTempDataProvider : SessionStateTempDataProvider
    {
        public FakeSessionStateTempDataProvider() : base(new Mock<TempDataSerializer>().Object)
        {

        }
    }
}
