using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteBook.Authentication.Models.DTO.Outgoing
{
    public class AuthResult
    {
        public string Token { get; set; }
        public bool Sccuess { get; set; }
        public List<string> Errors { get; set; }
    }
}
