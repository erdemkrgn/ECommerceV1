using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities;

namespace ECommerce.Core.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);  // Nullable dönen ID ile kullanıcı bulma
        Task AddAsync(User user);          // Yeni kullanıcı ekleme
        Task UpdateAsync(User user);       // Kullanıcı güncelleme
        Task DeleteAsync(int id);          // Kullanıcı silme

        Task<User?> LoginAsync(string email, string passwordHash);
        Task<User?> GetByEmailAsync(string email);

    }
}