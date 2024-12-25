using Microsoft.AspNetCore.Mvc;

namespace Demo1.Models
{
    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        [HiddenInput]
        public string ReturnUrl { get; set; }
    }
}
