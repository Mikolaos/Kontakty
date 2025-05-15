using Kontakty.DTOs;
using Kontakty.Interfaces;
using Kontakty.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Kontakty.Service;
using Microsoft.EntityFrameworkCore;

namespace Kontakty.Controllers;

/// <summary>
/// Controller responsible for handling user account-related operations such as login and registration
/// </summary>
[Route("api/account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly SignInManager<AppUser> _signInManager;

    /// <summary>
    /// Initializes a new instance of the AccountController
    /// </summary>
    /// <param name="userManager">The UserManager service for managing user accounts</param>
    /// <param name="tokenService">The service for creating authentication tokens</param>
    /// <param name="signInManager">The SignInManager service for handling user sign-in operations</param>
    public AccountController(UserManager<AppUser> userManager, ITokenService tokenService,
        SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _signInManager = signInManager;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token upon successful login
    /// </summary>
    /// <param name="loginDto">The login credentials (username and password)</param>
    /// <returns>
    /// 200 OK with user details and token if login successful
    /// 400 Bad Request if model validation fails
    /// 401 Unauthorized if credentials are invalid
    /// </returns>
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

    /// <summary>
    /// Registers a new user account with the provided details
    /// </summary>
    /// <param name="registerDto">The registration details including username, email, and password</param>
    /// <returns>
    /// 200 OK with user details and token if registration successful
    /// 400 Bad Request if model validation fails
    /// 500 Internal Server Error if user creation or role assignment fails
    /// </returns>
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
                return BadRequest(new { message = "Użytkownik z tym adresem email już istnieje." });
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