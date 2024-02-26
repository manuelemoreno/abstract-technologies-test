using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AwesomeFruits.Application.DTOs;
using AwesomeFruits.Application.Interfaces;
using AwesomeFruits.WebAPI.Users.Exceptions;
using AwesomeFruits.WebAPI.Users.Model;
using AwesomeFruits.WebAPI.Users.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AwesomeFruits.WebAPI.Users.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;

    public AuthController(IUserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<ActionResult<SaveUserDto>> Register(SaveUserDto saveUserDto)
    {
        ValidateSaveUserDtoRequest(saveUserDto);

        return Created("", await _userService.SaveUserAsync(saveUserDto));
    }

    [HttpPost("login")]
    public async Task<ActionResult<Token>> Login(LoginUserDto login)
    {
        var user = await _userService.GetValidLoggedInUser(login);

        var token = GenerateJwtToken(user);
        var accessToken = new Token { AccessToken = token };

        return Ok(accessToken);
    }

    private string GenerateJwtToken(UserDto user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static void ValidateSaveUserDtoRequest(SaveUserDto request)
    {
        var errors = new SaveUserDtoValidator().ValidateSaveUserDto(request);

        if (errors.Count > 0) throw new ValidationErrorsException(errors);
    }
}