using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteBook.Authentication.Models.DTO.Incoming
{ 
    public class UserRegistrationRequestDto
    {
        public string FirstName { get; set; } = String.Empty;

        public string LastName { get; set; } = String.Empty;

        public string Email { get; set; } = String.Empty;
        
        public string Password { get; set; } = String.Empty;
    }
}
