using Kontakty.DTOs;
using Kontakty.Interfaces;
using Kontakty.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Kontakty.Service;
using Microsoft.EntityFrameworkCore;

namespace Kontakty.Controllers;


[Route("api/account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountController(UserManager<AppUser> userManager, ITokenService tokenService,
        SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _signInManager = signInManager;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        // Model validation
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Find user by username
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username);

        // Return unauthorized if user not found
        if (user == null) return Unauthorized(new { message = "Invalid username!" });

        // Verify password for user
        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

        // Return unauthorized if password check failed
        if (!result.Succeeded)
            return Unauthorized(new { message = "User not found or/and invalid password!" });

        // Return success with user details and JWT token
        return Ok(
            new NewUserDto
            {
                Username = user.UserName,
                Email = user.Email,
                Token = _tokenService.CreateToken(user)
            });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            // Validate the model state
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Create new user object with provided details
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);

            if (existingUser != null)
            {
                return BadRequest(new { message = "There already exists a user with the same email address." });
            }

            var appUser = new AppUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email
            };

            // Attempt to create the user in the database
            var createUser = await _userManager.CreateAsync(appUser, registerDto.Password);
            if (createUser.Succeeded)
            {
                // Assign the "User" role to the newly created user
                var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                if (roleResult.Succeeded)
                {
                    // Return success response with user details and JWT token
                    return Ok(
                        new NewUserDto
                        {
                            Username = appUser.UserName,
                            Email = appUser.Email,
                            Token = _tokenService.CreateToken(appUser)
                        }
                    );
                }
                else
                {
                    // Return error if role assignment fails
                    return StatusCode(500, roleResult.Errors);
                }
            }
            else
            {
                // Return error if user creation fails
                return StatusCode(500, createUser.Errors);
            }
        }
        catch (Exception e)
        {
            // Return error for any unexpected exceptions
            return StatusCode(500, e.Message);
        }
    }
}