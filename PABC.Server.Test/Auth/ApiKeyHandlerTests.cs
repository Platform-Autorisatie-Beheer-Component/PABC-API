using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Moq;
using PABC.Server.Auth;
using System.Security.Claims;

namespace PABC.Server.Test.Auth
{
    public class ApiKeyAuthenticationHandlerTests
    {
        private readonly Mock<ILogger<ApiKeyRequirement.Handler>> _mockLogger;
        private readonly ApiKeyRequirement.Handler _handler;
        private static readonly ApiKeyRequirement _requirement = new(["valid-key-123", "another-valid-key-456"]);

        public ApiKeyAuthenticationHandlerTests()
        {
            _mockLogger = new Mock<ILogger<ApiKeyRequirement.Handler>>();
            _handler = new(_mockLogger.Object);
        }

        private static AuthorizationHandlerContext CreateContext(string apiKey)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity([new(ApiKeyAuthentication.ClaimType, apiKey)]));

            return new AuthorizationHandlerContext([_requirement], user, null);
        }

        [Fact]
        public async Task HandleAuthenticateAsync_WithValidApiKey_ShouldSucceed()
        {
            var context = CreateContext("valid-key-123");
            await _handler.HandleAsync(context);
            Assert.True(context.HasSucceeded);
            VerifyWarningLogged(Times.Never());
        }

        [Fact]
        public async Task HandleAuthenticateAsync_WithANotherValidApiKey_ShouldSucceed()
        {
            var context = CreateContext("another-valid-key-456");
            await _handler.HandleAsync(context);
            Assert.True(context.HasSucceeded);
            VerifyWarningLogged(Times.Never());
        }

        [Fact]
        public async Task HandleAuthenticateAsync_WithInvalidApiKey_ShouldReturnNoResultAndLogWarning()
        {
            var invalidKey = "this-is-a-wrong-key";
            var context = CreateContext(invalidKey);
            await _handler.HandleAsync(context);
            Assert.False(context.HasSucceeded);
            VerifyWarningLogged(Times.Once(), $"Authentication attempt with invalid API key: {invalidKey}");
        }

        [Fact]
        public async Task HandleAuthenticateAsync_WithMissingApiKeyHeader_ShouldReturnNoResultAndLogWarning()
        {
            var context = CreateContext(string.Empty);
            await _handler.HandleAsync(context);
            Assert.False(context.HasSucceeded);
            VerifyWarningLogged(Times.Once(), "Authentication attempt with missing API key");
        }

        private void VerifyWarningLogged(Times times, string expectedMessage = "")
        {
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, type) => !string.IsNullOrEmpty(expectedMessage) || state.ToString().Contains(expectedMessage)),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                times);
        }
    }
}