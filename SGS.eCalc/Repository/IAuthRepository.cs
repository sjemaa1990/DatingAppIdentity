using System.Threading.Tasks;
using SGS.eCalc.Models;

namespace SGS.eCalc.Repository
{
    public interface IAuthRepository
    {
        Task<bool> UserExists(string userName);

        Task<User> Register(User user ,string password);
        Task<User> Login(string user ,string password);

    }
}