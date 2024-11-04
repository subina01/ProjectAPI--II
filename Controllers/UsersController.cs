using Carrental.WebAPI.Data;
using Carrental.WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Carrental.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    { 
        private readonly ApplicationDbContext _context;
        public UsersController(ApplicationDbContext _context)
        {
            this._context = _context;
            
        }

        [HttpPost]
        [Route("Registration")]
        public IActionResult Registration(RegistrationDTO registrationDTO)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var objuser = _context.Users.FirstOrDefault(x => x.UserName == registrationDTO.UserName);
            if (objuser == null)
            {


                _context.Users.Add(new User
                {
                    UserName = registrationDTO.UserName,
                    FirstName = registrationDTO.FirstName,
                    LastName = registrationDTO.LastName,
                    Password = registrationDTO.Password,
                    EmailId = registrationDTO.EmailId,
                    ContactNo = registrationDTO.ContactNo,
                    Address = registrationDTO.Address,
                    AltEmail = registrationDTO.AltEmail,
                    AltContact = registrationDTO.AltContact,
                    DriverLicInfo = registrationDTO.DriverLicInfo,
                    UserType = registrationDTO.UserType,
                });
                _context.SaveChanges();
                return Ok("User Registered Sucessfully");
            }
            else
            {
                return BadRequest("User already exists with the same Username");
            }
        }


        [HttpPost]
        [Route("Login")]
        public IActionResult Login(LoginDTO loginDTO)
        {

            var user = _context.Users.FirstOrDefault(x => x.UserName == loginDTO.UserName);
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
                _context.SaveChanges();
            }

          
            if (user.Password == loginDTO.Password)
            {
               
                user.FailedLoginAttempts = 0;
                _context.SaveChanges();
                return Ok(user);
            }
            else
            {
               
                user.FailedLoginAttempts++;
                if (user.FailedLoginAttempts >= 3)
                {
                    user.IsBlocked = true;
                    user.BlockedUntil = DateTime.UtcNow.AddMinutes(2);
                }
                _context.SaveChanges();
                return BadRequest("Invalid username or password");
            }
        }

        [HttpGet]
        [Route("GetUsers")]
        public IActionResult GetUsers() { 
            return Ok(_context.Users.ToList());
        }

        [HttpGet]
        [Route("GetUser")]
        public IActionResult GetUser(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            if (user != null) { 
            return Ok();
            }
            else
            {
                return NoContent();
            }

        }
    }
}
