using Microsoft.AspNetCore.Identity;
using MusicPlayer.Application.Users.Register;
using MusicPlayer.Infrastructure.Identity;

namespace MusicPlayer.Infrastructure.UnitTests.Identity;

public sealed class IdentityErrorMapperTests
{
    [Fact]
    public void MapReturnsEmailAlreadyExistsForDuplicateEmail()
    {
        var error = IdentityErrorMapper.Map([new IdentityError { Code = "DuplicateEmail", Description = "Email already exists." }]);

        Assert.Equal(RegistrationErrors.EmailAlreadyExistsCode, error.Code);
        Assert.Equal("Email already exists.", error.Message);
    }

    [Fact]
    public void MapReturnsInvalidPasswordForPasswordRules()
    {
        var error = IdentityErrorMapper.Map(
        [
            new IdentityError { Code = "PasswordTooShort", Description = "Passwords must be at least 6 characters." },
            new IdentityError { Code = "PasswordRequiresDigit", Description = "Passwords must have at least one digit." }
        ]);

        Assert.Equal(RegistrationErrors.InvalidPasswordCode, error.Code);
        Assert.Contains("6 characters", error.Message);
        Assert.Contains("digit", error.Message);
    }
}
