using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PABC.Server.Auth;
using ApiKeyOptions = PABC.Server.Auth.ApiKeyAuthentication.ApiKeyOptions; 
using Handler = PABC.Server.Auth.ApiKeyAuthentication.Handler;

namespace PABC.Server.Test.Auth
{ 
    public class FakeOptionsMonitor<T> : IOptionsMonitor<T> where T : class, new()
    {
        public T CurrentValue { get; }

        public FakeOptionsMonitor(T currentValue)
        {
            CurrentValue = currentValue;
        }

        public IDisposable OnChange(Action<T, string> listener)
        { 
            return new Mock<IDisposable>().Object;
        }

        public T Get(string? name)
        {
            return CurrentValue;
        }
    }


    public class ApiKeyAuthenticationHandlerTests
    {
        private readonly Mock<ILoggerFactory> _mockLoggerFactory;
        private readonly Mock<ILogger> _mockLogger;
        private readonly UrlEncoder _urlEncoder;
        private static readonly string[] API_KEYS = ["valid-key-123", "another-valid-key-456"];
        
        public ApiKeyAuthenticationHandlerTests()
        {
            _mockLoggerFactory = new Mock<ILoggerFactory>();
            _mockLogger = new Mock<ILogger>();
            _urlEncoder = UrlEncoder.Default;
            
            _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
                              .Returns(_mockLogger.Object);
        }

        private async Task<Handler> CreateHandlerAsync(string providedApiKey)
        { 
            var options = new ApiKeyOptions
            {
                ApiKeys = API_KEYS
            };
             
            var fakeOptionsMonitor = new FakeOptionsMonitor<ApiKeyOptions>(options);
             
            var handler = new Handler(fakeOptionsMonitor, _mockLoggerFactory.Object, _urlEncoder);
             
            var context = new DefaultHttpContext();
            const string schemeName = ApiKeyAuthentication.Scheme;
            if ( string.IsNullOrEmpty(providedApiKey) == false)
            {
                context.Request.Headers[ApiKeyAuthentication.API_KEY_HEADER_NAME] = providedApiKey;
            }

            var scheme = new AuthenticationScheme(schemeName, "ApiKey Test", typeof(Handler));
            await handler.InitializeAsync(scheme, context);

            return handler;
        }


        [Fact]
        public async Task HandleAuthenticateAsync_WithValidApiKey_ShouldSucceed()
        {
            var handler = await CreateHandlerAsync("valid-key-123");

            var result = await handler.AuthenticateAsync();

            Assert.True(result.Succeeded);
            Assert.NotNull(result.Ticket);
            VerifyWarningLogged(Times.Never());
        }

        [Fact]
        public async Task HandleAuthenticateAsync_WithANotherValidApiKey_ShouldSucceed()
        {
            var handler = await CreateHandlerAsync("another-valid-key-456");

            var result = await handler.AuthenticateAsync();

            Assert.True(result.Succeeded);
            Assert.NotNull(result.Ticket);
            VerifyWarningLogged(Times.Never());
        }

        [Fact]
        public async Task HandleAuthenticateAsync_WithInvalidApiKey_ShouldReturnNoResultAndLogWarning()
        {
            var invalidKey = "this-is-a-wrong-key";
            var handler = await CreateHandlerAsync(invalidKey);

            var result = await handler.AuthenticateAsync();

            Assert.False(result.Succeeded);
            Assert.Null(result.Failure);
            VerifyWarningLogged(Times.Once(), $"Authentication attempt with invalid API key: {invalidKey}");
        }

        [Fact]
        public async Task HandleAuthenticateAsync_WithMissingApiKeyHeader_ShouldReturnNoResultAndLogWarning()
        {
            var handler = await CreateHandlerAsync(string.Empty);
            var result = await handler.AuthenticateAsync();

            Assert.False(result.Succeeded);
            Assert.Null(result.Failure);
            VerifyWarningLogged(Times.Once(), "Authentication attempt with missing API key");
        }

        private void VerifyWarningLogged(Times times, string expectedMessage = "")
        {
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, type) =>  !string.IsNullOrEmpty(expectedMessage) || state.ToString().Contains(expectedMessage)),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                times);
        }
    }
}