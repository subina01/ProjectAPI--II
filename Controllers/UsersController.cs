using Carrental.WebAPI.Data;
using Carrental.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Carrental.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager; 


        
        public UsersController(ApplicationDbContext context, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;  
            _configuration = configuration;
        }
        [HttpPost]
        [Route("Registration")]
        public async Task<IActionResult> Registration(RegistrationDTO registrationDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _userManager.FindByNameAsync(registrationDTO.UserName);
            if (existingUser != null)
            {
                return BadRequest("User already exists.");
            }

            var user = new User
            {
                UserName = registrationDTO.UserName,
                Email = registrationDTO.EmailId,
                FirstName = registrationDTO.FirstName,
                LastName = registrationDTO.LastName,
                ContactNo = registrationDTO.ContactNo,
                EmailId = registrationDTO.EmailId,
                Address = registrationDTO.Address,
                DriverLicInfo = registrationDTO.DriverLicInfo,
                UserType = registrationDTO.UserType
            };

            var result = await _userManager.CreateAsync(user, registrationDTO.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            if (await _roleManager.RoleExistsAsync(registrationDTO.UserType))
            {
                var roleResult = await _userManager.AddToRoleAsync(user, registrationDTO.UserType);
                if (!roleResult.Succeeded)
                {
                    return BadRequest(roleResult.Errors);
                }
            }
            else
            {
                return BadRequest("Invalid role specified.");
            }

            return Ok("User registered successfully.");
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByNameAsync(loginDTO.UserName);
            if (user == null)
            {
                return BadRequest("User is not registered");
            }

            if (user.IsBlocked && user.BlockedUntil > DateTime.UtcNow)
            {
                return BadRequest("User is blocked. Please try again after 2 minutes.");
            }
            else if (user.IsBlocked && user.BlockedUntil <= DateTime.UtcNow)
            {
                user.IsBlocked = false;
                user.FailedLoginAttempts = 0;
                user.BlockedUntil = null;
                await _userManager.UpdateAsync(user);
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginDTO.Password, false, false);
            if (result.Succeeded)
            {
                user.FailedLoginAttempts = 0;
                await _userManager.UpdateAsync(user);

                var token = GenerateJwtToken(user);

                
                var response = new LoginResponseDTO
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    ContactNo = user.ContactNo,
                    Address = user.Address,
                    DriverLicInfo = user.DriverLicInfo,
                    UserType = user.UserType,
                    Token = token
                };

                return Ok(response);
            }
            else
            {
                user.FailedLoginAttempts++;
                if (user.FailedLoginAttempts >= 3)
                {
                    user.IsBlocked = true;
                    user.BlockedUntil = DateTime.UtcNow.AddMinutes(2);
                }
                await _userManager.UpdateAsync(user);
                return BadRequest("Invalid username or password");
            }
        }


        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, user.UserType) 
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet]
        [Route("GetUsers")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetUsers()
        {
            return Ok(_context.Users.ToList());
        }

        [HttpGet]
        [Route("GetUser/{id}")]
        [Authorize(Roles = "Admin,User,Dealer")]
        public IActionResult GetUser(string id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                return NoContent();
            }
        }

        [HttpPut("UpdateUserStatus/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserStatus(string id, [FromBody] UserStatusUpdateDTO statusUpdateDTO)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.IsBlocked = statusUpdateDTO.IsBlocked;
            user.BlockedUntil = statusUpdateDTO.BlockedUntil;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest("Failed to update user status.");
            }

            return Ok("User status updated successfully.");
        }
        [HttpGet("GetAllUsers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(user => new UserDetailsDTO
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmailId = user.EmailId,
                    ContactNo = user.ContactNo,
                    Address = user.Address,
                    DriverLicInfo = user.DriverLicInfo,
                    UserType = user.UserType,
                    IsBlocked = user.IsBlocked,
                    BlockedUntil = user.BlockedUntil
                })
                .ToListAsync();

            var response = new
            {
                TotalUsers = users.Count,
                Users = users
            };

            return Ok(response);
        }

        [HttpPut("UpdateUserDetails/{id}")]
    
        public async Task<IActionResult> UpdateUserDetails(string id, [FromBody] UserUpdateDTO userUpdateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            
            if (!string.IsNullOrEmpty(userUpdateDTO.UserName))
            {
                user.UserName = userUpdateDTO.UserName;
            }

            if (!string.IsNullOrEmpty(userUpdateDTO.FirstName))
            {
                user.FirstName = userUpdateDTO.FirstName;
            }

            if (!string.IsNullOrEmpty(userUpdateDTO.LastName))
            {
                user.LastName = userUpdateDTO.LastName;
            }

            if (!string.IsNullOrEmpty(userUpdateDTO.Email))
            {
                user.Email = userUpdateDTO.Email;
            }

            if (!string.IsNullOrEmpty(userUpdateDTO.ContactNo))
            {
                user.ContactNo = userUpdateDTO.ContactNo;
            }

            if (!string.IsNullOrEmpty(userUpdateDTO.Address))
            {
                user.Address = userUpdateDTO.Address;
            }

            if (!string.IsNullOrEmpty(userUpdateDTO.DriverLicInfo))
            {
                user.DriverLicInfo = userUpdateDTO.DriverLicInfo;
            }

            
            if (!string.IsNullOrEmpty(userUpdateDTO.CurrentPassword) && !string.IsNullOrEmpty(userUpdateDTO.NewPassword))
            {
                var passwordChangeResult = await _userManager.ChangePasswordAsync(user, userUpdateDTO.CurrentPassword, userUpdateDTO.NewPassword);
                if (!passwordChangeResult.Succeeded)
                {
                    return BadRequest(passwordChangeResult.Errors);
                }
            }

           
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("User details updated successfully.");
        }



    }
}
