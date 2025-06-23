using Microsoft.AspNetCore.Identity;

namespace webjooneli.Models.Entities
{
    public class Users : IdentityUser
    {
        public string Role { get; set; } = default!;
    }


}
