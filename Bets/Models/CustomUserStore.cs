using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Bets
{
    public class CustomUserStore<T> : IUserStore<T> where T : ApplicationUser
    {
        Task IUserStore<T, string>.CreateAsync(T user)
        {
            throw new NotImplementedException();
        }

        Task IUserStore<T, string>.UpdateAsync(T user)
        {
            throw new NotImplementedException();
        }

        Task IUserStore<T, string>.DeleteAsync(T user)
        {
            throw new NotImplementedException();
        }

        Task<T> IUserStore<T, string>.FindByIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        Task<T> IUserStore<T, string>.FindByNameAsync(string userName)
        {
            throw new NotImplementedException();
        }

        void IDisposable.Dispose()
        {
        }
    }
}
