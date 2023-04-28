﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Dnmh.Security.ApiKeyAuthentication.Test;
internal static class AuthenticationHandlerExtensions
{
    public static async Task InitializeWithSchemeNameAsync<T>(this T handler, HttpContext httpContext, string schemeName = "ApiKey") where T : IAuthenticationHandler =>
        await handler.InitializeAsync(new AuthenticationScheme(schemeName, null, typeof(T)), httpContext);
}
