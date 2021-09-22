using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using Microsoft.Extensions.Options;
using PersonProfile_DF.Api;
using PersonProfile_DF.Api.Models;
using PersonProfile_DF.Business.Contracts;
using System.Threading.Tasks;

namespace PersonProfile_DF.Website.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AccountController : ControllerBase
	{
		private readonly IAuthenticateProcessor _authenticateProcessor;
		private readonly ApiProjectConfig _apiProjectConfig;
		private readonly IMapper _mapper;

		public AccountController(IMapper mapper, IOptions<ApiProjectConfig> options, IAuthenticateProcessor authenticateProcessor)
		{
			_authenticateProcessor = authenticateProcessor;
			_mapper = mapper;
			_apiProjectConfig = options.Value;
		}

		[HttpPost("LogIn", Name = nameof(LogIn))]
		public async Task<IActionResult> LogIn(LoginRequest model)
		{
			LoginResponse loginResponse = null;

			var serviceResponse = await _authenticateProcessor.SignInAsync(new Business.Data.Models.LoginRequest { Username = model.Email, Password = model.Password });

			if(serviceResponse.IsAuthenticationSuccess)
			{
				var currentDT = DateTime.UtcNow;
				var expiryDT = currentDT.AddSeconds(_apiProjectConfig.JwtTokenExpirationTimeInSeconds);

				var tokenHandler = new JwtSecurityTokenHandler();
				var key = Encoding.ASCII.GetBytes(_apiProjectConfig.JwtSecret);
				var tokenDescriptor = new SecurityTokenDescriptor
				{
					Subject = new ClaimsIdentity(new[] {
						new Claim(ClaimTypes.NameIdentifier, serviceResponse.Email),
						new Claim(ClaimTypes.Name, serviceResponse.Name),
						new Claim(ClaimTypes.UserData, serviceResponse.AuthenticatedToken)
					}),
					Expires = expiryDT,
					Issuer = _apiProjectConfig.JwtIssuer,
					Audience = _apiProjectConfig.JwtAudience,
					SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
				};
				var token = tokenHandler.CreateToken(tokenDescriptor);

				loginResponse = new LoginResponse();
				loginResponse.AccessToken = tokenHandler.WriteToken(token);
				loginResponse.Email = model.Email;
				loginResponse.ExpireAt = expiryDT;

				return Ok(loginResponse);
			}
			else
			{
				return Unauthorized("Authentication failed.");
			}
		}

		[HttpPost("LogOut", Name = nameof(LogOut))]
		public async Task<IActionResult> LogOut()
		{
			// JWT tokens (on client side) cannot be invalidated immediately; either wait for the token to expire after its expiry-time.
			// To immediately invalidate a user, we need to maintain a list of invalidated tokens on the server cache. Then on subsequent requests, you can check the cache to see if this token was already invalidated.
			// In this case, we're logging out the user from Business Layer which will immediately log the user out.
			await _authenticateProcessor.SignOutAsync();
			return Ok();
		}
	}
}

