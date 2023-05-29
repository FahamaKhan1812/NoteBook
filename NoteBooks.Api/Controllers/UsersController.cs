using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoteBook.DataService.Data;
using NoteBook.DataService.Dtos.Incomming;
using NoteBook.DataService.IConfiguration;
using NoteBook.Entities.DbSet;

namespace NoteBooks.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
    }
}
