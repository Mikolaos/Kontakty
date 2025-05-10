using Kontakty.Models;

namespace Kontakty.Interfaces;

public interface ITokenService
{

        string CreateToken(AppUser appUser);
   
}