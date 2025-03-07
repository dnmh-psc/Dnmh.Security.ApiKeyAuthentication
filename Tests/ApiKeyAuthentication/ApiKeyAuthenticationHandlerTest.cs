﻿using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Shouldly;

namespace DNMH.Security.ApiKeyAuthentication.Tests;
public class ApiKeyAuthenticationHandlerTest
{
    [Fact]
    public async Task TestDefaultOptionsHeaderKeySuccessfully()
    {
        // Arrange
        var optionsMock = MockHelpers.CreateMockOptionsMonitor<ApiKeyAuthenticationOptions>();
        var handler = MockHelpers.CreateApiKeyAuthenticationHandler(optionsMock.Object);
        var mockHttpContext = MockHelpers.CreateMockHttpContextWithRequestHeaders(new Dictionary<string, StringValues> { { "X-API-KEY", "key" } });
        await handler.InitializeWithSchemeNameAsync(mockHttpContext.Object);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
    }

    [Fact]
    public async Task TestOptionsWithCustomHeaderKeySuccessfully()
    {
        // Arrange
        var optionsMock = MockHelpers.CreateMockOptionsMonitor<ApiKeyAuthenticationOptions>(options =>
        {
            options.AllowApiKeyInQuery = false;
            options.AllowApiKeyInRequestHeader = true;
            options.HeaderKeys.Add("ApiKey");
        });
        var handler = MockHelpers.CreateApiKeyAuthenticationHandler(optionsMock.Object);
        var mockHttpContext = MockHelpers.CreateMockHttpContextWithRequestHeaders(new Dictionary<string, StringValues> { { "ApiKey", "key" } });
        await handler.InitializeWithSchemeNameAsync(mockHttpContext.Object);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
    }

    [Fact]
    public async Task TestOptionsWithMultipleCustomHeaderKeysSuccessfully()
    {
        // Arrange
        var optionsMock = MockHelpers.CreateMockOptionsMonitor<ApiKeyAuthenticationOptions>(options =>
        {
            options.AllowApiKeyInQuery = false;
            options.AllowApiKeyInRequestHeader = true;
            options.HeaderKeys.Add("ApiKey");
            options.HeaderKeys.Add("XApiKey");
        });
        var handler = MockHelpers.CreateApiKeyAuthenticationHandler(optionsMock.Object);
        var mockHttpContext = MockHelpers.CreateMockHttpContextWithRequestHeaders(new Dictionary<string, StringValues> { { "XApiKey", "key" } });
        await handler.InitializeWithSchemeNameAsync(mockHttpContext.Object);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
    }

    [Fact]
    public async Task TestOptionsWithCaseInsensitiveCustomHeaderKeysSuccessfully()
    {
        // Arrange
        var optionsMock = MockHelpers.CreateMockOptionsMonitor<ApiKeyAuthenticationOptions>(options =>
        {
            options.AllowApiKeyInQuery = false;
            options.AllowApiKeyInRequestHeader = true;
            options.HeaderKeys.Add("X-ApiKey", false);
        });
        var handler = MockHelpers.CreateApiKeyAuthenticationHandler(optionsMock.Object);
        var mockHttpContext = MockHelpers.CreateMockHttpContextWithRequestHeaders(new Dictionary<string, StringValues> { { "x-apikey", "key" } });
        await handler.InitializeWithSchemeNameAsync(mockHttpContext.Object);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
    }

    [Fact]
    public async Task TestOptionsWithMultipleCustomHeaderKeysButNoneMatchShouldFail()
    {
        // Arrange
        var optionsMock = MockHelpers.CreateMockOptionsMonitor<ApiKeyAuthenticationOptions>(options =>
        {
            options.AllowApiKeyInQuery = false;
            options.AllowApiKeyInRequestHeader = true;
            options.HeaderKeys.Add("ApiKey");
            options.HeaderKeys.Add("XApiKey");
        });
        var handler = MockHelpers.CreateApiKeyAuthenticationHandler(optionsMock.Object);
        var mockHttpContext = MockHelpers.CreateMockHttpContextWithRequestHeaders(new Dictionary<string, StringValues> { { "mykey", "key" } });
        await handler.InitializeWithSchemeNameAsync(mockHttpContext.Object);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
    }

    [Fact]
    public async Task TestOptionsWithAuthorizationHeaderKeySuccessfully()
    {
        // Arrange
        var optionsMock = MockHelpers.CreateMockOptionsMonitor<ApiKeyAuthenticationOptions>(options =>
        {
            options.AllowApiKeyInQuery = false;
            options.AllowApiKeyInRequestHeader = true;
            options.UseAuthorizationHeaderKey = true;
        });
        var handler = MockHelpers.CreateApiKeyAuthenticationHandler(optionsMock.Object);
        var mockHttpContext = MockHelpers.CreateMockHttpContextWithRequestHeaders(new Dictionary<string, StringValues> { { HeaderNames.Authorization, "Bearer key" } });
        await handler.InitializeWithSchemeNameAsync(mockHttpContext.Object);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
    }

    [Fact]
    public async Task TestOptionsWithAuthorizationHeaderAndCustomSchemeKeySuccessfully()
    {
        // Arrange
        var optionsMock = MockHelpers.CreateMockOptionsMonitor<ApiKeyAuthenticationOptions>(options =>
        {
            options.AllowApiKeyInQuery = false;
            options.AllowApiKeyInRequestHeader = true;
            options.UseAuthorizationHeaderKey = true;
            options.AuthorizationSchemeInHeader = "ApiKey";
        });
        var handler = MockHelpers.CreateApiKeyAuthenticationHandler(optionsMock.Object);
        var mockHttpContext = MockHelpers.CreateMockHttpContextWithRequestHeaders(new Dictionary<string, StringValues> { { HeaderNames.Authorization, "ApiKey key" } });
        await handler.InitializeWithSchemeNameAsync(mockHttpContext.Object);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
    }

    [Fact]
    public async Task TestOptionsWithAuthorizationHeaderAndCustomSchemeKeyButWrongSchemeShouldFail()
    {
        // Arrange
        var optionsMock = MockHelpers.CreateMockOptionsMonitor<ApiKeyAuthenticationOptions>(options =>
        {
            options.AllowApiKeyInQuery = false;
            options.AllowApiKeyInRequestHeader = true;
            options.UseAuthorizationHeaderKey = true;
            options.AuthorizationSchemeInHeader = "ApiKey";
        });
        var handler = MockHelpers.CreateApiKeyAuthenticationHandler(optionsMock.Object);
        var mockHttpContext = MockHelpers.CreateMockHttpContextWithRequestHeaders(new Dictionary<string, StringValues> { { HeaderNames.Authorization, "Bearer key" } });
        await handler.InitializeWithSchemeNameAsync(mockHttpContext.Object);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
    }

    [Fact]
    public async Task TestOptionsWithAuthorizationHeaderKeyNoBearerShouldFail()
    {
        // Arrange
        var optionsMock = MockHelpers.CreateMockOptionsMonitor<ApiKeyAuthenticationOptions>(options =>
        {
            options.AllowApiKeyInQuery = false;
            options.AllowApiKeyInRequestHeader = true;
            options.UseAuthorizationHeaderKey = true;
        });
        var handler = MockHelpers.CreateApiKeyAuthenticationHandler(optionsMock.Object);
        var mockHttpContext = MockHelpers.CreateMockHttpContextWithRequestHeaders(new Dictionary<string, StringValues> { { HeaderNames.Authorization, "key" } }); // "Bearer" is missing
        await handler.InitializeWithSchemeNameAsync(mockHttpContext.Object);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
    }

    [Fact]
    public async Task TestDefaultOptionsQueryKeySuccessfully()
    {
        // Arrange
        var optionsMock = MockHelpers.CreateMockOptionsMonitor<ApiKeyAuthenticationOptions>();
        var handler = MockHelpers.CreateApiKeyAuthenticationHandler(optionsMock.Object);
        var mockHttpContext = MockHelpers.CreateMockHttpContextWithRequestQueryParams(new Dictionary<string, StringValues> { { "apikey", "key" } });
        await handler.InitializeWithSchemeNameAsync(mockHttpContext.Object);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
    }

    [Fact]
    public async Task TestDefaultOptionsWrongQueryKeyShouldFail()
    {
        // Arrange
        var optionsMock = MockHelpers.CreateMockOptionsMonitor<ApiKeyAuthenticationOptions>();
        var handler = MockHelpers.CreateApiKeyAuthenticationHandler(optionsMock.Object);
        var mockHttpContext = MockHelpers.CreateMockHttpContextWithRequestQueryParams(new Dictionary<string, StringValues> { { "jwt", "key" } });
        await handler.InitializeWithSchemeNameAsync(mockHttpContext.Object);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
    }

    [Fact]
    public async Task TestOptionsWithCustomQueryKeySuccessfully()
    {
        // Arrange
        var optionsMock = MockHelpers.CreateMockOptionsMonitor<ApiKeyAuthenticationOptions>(options =>
        {
            options.QueryKeys.Add("mykey");
        });
        var handler = MockHelpers.CreateApiKeyAuthenticationHandler(optionsMock.Object);
        var mockHttpContext = MockHelpers.CreateMockHttpContextWithRequestQueryParams(new Dictionary<string, StringValues> { { "mykey", "key" } });
        await handler.InitializeWithSchemeNameAsync(mockHttpContext.Object);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
    }

    [Fact]
    public async Task TestOptionsWithMultipleCustomQueryKeysSuccessfully()
    {
        // Arrange
        var optionsMock = MockHelpers.CreateMockOptionsMonitor<ApiKeyAuthenticationOptions>(options =>
        {
            options.QueryKeys.Add("mykey");
            options.QueryKeys.Add("mykey2");
        });
        var handler = MockHelpers.CreateApiKeyAuthenticationHandler(optionsMock.Object);
        var mockHttpContext = MockHelpers.CreateMockHttpContextWithRequestQueryParams(new Dictionary<string, StringValues> { { "mykey", "key" } });
        await handler.InitializeWithSchemeNameAsync(mockHttpContext.Object);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
    }

    [Fact]
    public async Task TestOptionsWithMultipleCustomQueryKeysButNoneMatchShouldFail()
    {
        // Arrange
        var optionsMock = MockHelpers.CreateMockOptionsMonitor<ApiKeyAuthenticationOptions>(options =>
        {
            options.QueryKeys.Add("mykey");
            options.QueryKeys.Add("mykey2");
        });
        var handler = MockHelpers.CreateApiKeyAuthenticationHandler(optionsMock.Object);
        var mockHttpContext = MockHelpers.CreateMockHttpContextWithRequestQueryParams(new Dictionary<string, StringValues> { { "someother", "key" } });
        await handler.InitializeWithSchemeNameAsync(mockHttpContext.Object);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
    }

    [Fact]
    public async Task TestOptionsWithCaseInsensitiveCustomQueryKeySuccessfully()
    {
        // Arrange
        var optionsMock = MockHelpers.CreateMockOptionsMonitor<ApiKeyAuthenticationOptions>(options =>
        {
            options.QueryKeys.Add("mykey", false);
        });
        var handler = MockHelpers.CreateApiKeyAuthenticationHandler(optionsMock.Object);
        var mockHttpContext = MockHelpers.CreateMockHttpContextWithRequestQueryParams(new Dictionary<string, StringValues> { { "MYKEY", "key" } });
        await handler.InitializeWithSchemeNameAsync(mockHttpContext.Object);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
    }

    [Fact]
    public async Task TestOptionsWithMultipleCaseInsensitiveCustomQueryKeySuccessfully()
    {
        // Arrange
        var optionsMock = MockHelpers.CreateMockOptionsMonitor<ApiKeyAuthenticationOptions>(options =>
        {
            options.QueryKeys.Add("mykey", false);
            options.QueryKeys.Add("otherkey", true);
        });
        var handler = MockHelpers.CreateApiKeyAuthenticationHandler(optionsMock.Object);
        var mockHttpContext = MockHelpers.CreateMockHttpContextWithRequestQueryParams(new Dictionary<string, StringValues> { { "MYKEY", "key" }, { "OTHERKEY", "key" } });
        await handler.InitializeWithSchemeNameAsync(mockHttpContext.Object);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
    }


}
