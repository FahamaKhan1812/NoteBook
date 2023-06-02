using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoteBook.DataService.Data;
using NoteBook.DataService.Dtos.Incomming;
using NoteBook.DataService.IConfiguration;
using NoteBook.Entities.DbSet;

namespace NoteBooks.Api.Controllers.v1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : BaseController
    {
        public UsersController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        [HttpGet("testrun")]
        public IActionResult TestRun()
        {
            return Ok("Hell yess!!!");
        }

        // Get all
        [HttpGet("getallusers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            return Ok(users);
        }

        // Get user by Id
        [HttpGet("getuser", Name = "getuser")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);

            return Ok(user);
        }

        // Add new user
        [HttpPost("adduser")]
        public async Task<IActionResult> AddNewUser(UserDto user)
        {
            var _user = new User();
            _user.FirstName = user.FirstName;
            _user.LastName = user.LastName;
            _user.Email = user.Email;
            _user.DateOfBirth = user.DateOfBirth;
            _user.Phone = user.Phone;
            _user.Country = user.Country;
            _user.Status = 1;

            await _unitOfWork.Users.Add(_user);
            await _unitOfWork.CompleteAsync();
            return CreatedAtRoute("getuser", new { id = _user.Id }, user);
        }
    }
}
