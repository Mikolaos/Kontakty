using Kontakty.Models;
using Kontakty.DTOs;
using Kontakty.Helpers;

namespace Kontakty.Interfaces;

public interface IContactRepository
{
    Task<List<ContactModel>> GetAllAsync(QueryObject query);

    Task<List<ContactModel>> GetAllAsync();

    Task<ContactModel?> GetByIdAsync(int id);

    Task<ContactModel> CreateAsync(ContactModel contactModel);

    Task<ContactModel?> UpdateAsync(int id, ContactCreateAndUpdateDto contactCreateAndUpdateDto);

    Task<ContactModel?> DeleteAsync(int id);
    Task<bool> ExistsByEmail(string email);
}