using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NoteBook.Authentication.Configuration;
using NoteBook.Authentication.Models.DTO.Incoming;
using NoteBook.Authentication.Models.DTO.Outgoing;
using NoteBook.DataService.IConfiguration;
using NoteBook.Entities.DbSet;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NoteBooks.Api.Controllers.v1.Auth
{
    public class AccountsController : BaseController
    {
        // Class provided by the ASP .NET Core Identity
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;

        public AccountsController(
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager,
           IOptionsMonitor<JwtConfig> optionsMonitor
            ) : base(unitOfWork)
        {
            _userManager = userManager;
            _jwtConfig = optionsMonitor.CurrentValue;
        }

        // Register User
        [HttpPost("createuser")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationRequestDto registrationRequestDto)
        {
            // check form is dirty/invalid
            if (ModelState.IsValid)
            {
                // check if already exist
                var userExist = await _userManager.FindByEmailAsync(registrationRequestDto.Email);

                if(userExist != null)
                {
                    return BadRequest(new UserRegistrationResponseDto
                    {
                        Sccuess = false,
                        Errors = new List<string>()
                    {
                        "Email is already exist"
                    }
                    });
                }
                // Add the user
                var newUser = new IdentityUser()
                {
                    Email = registrationRequestDto.Email,
                    UserName = registrationRequestDto.Email,
                    EmailConfirmed = true
                };

                // adding the user to table
                var isCreated = await _userManager.CreateAsync(newUser, registrationRequestDto.Password);
                if (!isCreated.Succeeded)
                {
                    return BadRequest(new UserRegistrationResponseDto
                    {
                        Sccuess = false,
                        Errors = isCreated.Errors.Select(x => x.Description).ToList()
                    });
                }

                // Add user to the Users table 
                var _user = new User();
                _user.FirstName = registrationRequestDto.FirstName;
                _user.LastName = registrationRequestDto.LastName;
                _user.Email = registrationRequestDto.Email;
                _user.DateOfBirth = DateTime.UtcNow;
                _user.Phone = ""; 
                _user.Country = "";
                _user.Status = 1;

                await _unitOfWork.Users.Add(_user);
                await _unitOfWork.CompleteAsync();

                // create a jwt
                var token = GenerateJwtToken(newUser);

                // return back to the user
                return Ok(new UserRegistrationResponseDto()
                {
                    Sccuess = true,
                    Token = token
                });
            }
            else
            {
                return BadRequest(new UserRegistrationResponseDto
                { 
                    Sccuess = false,
                    Errors = new List<string>()
                    {
                        "Invalid payload"
                    }
                });
            }
        }


        // Login user
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody ] UserLoginRequestDto userLoginRequestDto)
        {
            if (ModelState.IsValid)
            {
                // 1- Check if email exists
                var user = await _userManager.FindByEmailAsync(userLoginRequestDto.Email);
                if(user == null)
                {
                    return BadRequest(new UserLoginResponseDto
                    {
                        Sccuess = false,
                        Errors = new List<string>
                        {
                            "Invalid authentication request"
                        }
                    }) ;
                }

                // 2- Check if the user hass a valid password
                var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, userLoginRequestDto.Password);
                if (isPasswordCorrect)
                {
                    // generate JWT
                    var jwtToken = GenerateJwtToken(user);
                    return Ok(new UserLoginResponseDto
                    {
                        Sccuess = true,
                        Token = jwtToken
                    });
                }
                else
                {
                    // through an error
                    return BadRequest(new UserLoginResponseDto
                    {
                        Sccuess = false,
                        Errors = new List<string>
                        {
                            "Invalid authentication request"
                        }
                    });
                }
            }
            else // Invalid request object
            {
                return BadRequest(new UserRegistrationResponseDto
                {
                    Sccuess = false,
                    Errors = new List<string>()
                    {
                        "Invalid payload"
                    }
                });
            }
        }
        private string GenerateJwtToken(IdentityUser user)
        {
            // creating a handler
            var jwtHandler = new JwtSecurityTokenHandler();

            // get the security key
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new []
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email), // sub --> unique id
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials
                (
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )       
            };

            // generate security token  
            var token = jwtHandler.CreateToken(tokenDescriptor);
            
            // convert secure token into a string
            var jwtToken = jwtHandler.WriteToken(token);

            return jwtToken;

        }
    }

}
