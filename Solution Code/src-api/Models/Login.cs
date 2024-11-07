using System.ComponentModel.DataAnnotations;

namespace src_api.Models
{
    public class LoginRequest
    {
        [Required]
        public string Base64Username { get; set; }

        [Required]
        public string Base64Password { get; set; }

        public int ServerId { get; set; }
    }
}
