using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Models;
using Microsoft.AspNetCore.Http;
using System.Buffers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Api;

public class CustomBearerTokenAuthSchemeOptions : AuthenticationSchemeOptions
{
    public Model Model { get; set; }
}

public class CustomBearerTokenAuthSchemeHandler(
    IOptionsMonitor<CustomBearerTokenAuthSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<CustomBearerTokenAuthSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorizationHeader = Request.Headers.Authorization;
        if (authorizationHeader.Count == 0)
        {
            return Fail();
        }

        var headerParts = authorizationHeader.ToString().Split(' ');
        if (headerParts.Length != 2 || headerParts[0] != "Bearer")
        {
            return Fail();
        }

        var token = headerParts[1];

        var user = Options.Model.Users.GetForToken(token);
        if (user == null)
        {
            return Fail();
        }

        var claims = new[] { new Claim(ClaimTypes.Name, "Test") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Tokens"));
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private static Task<AuthenticateResult> Fail()
    {
        return Task.FromResult(AuthenticateResult.Fail("Authentication failed"));
    }
}
