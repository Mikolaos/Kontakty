using Kontakty.Data;
using Kontakty.DTOs;
using Kontakty.Helpers;
using Kontakty.Interfaces;
using Kontakty.Models;
using Microsoft.EntityFrameworkCore;

namespace Kontakty.Repository;

public class ContactRepository : IContactRepository
{
    // Database context for accessing contacts data
    private readonly ApplicationDBContext _context;

    public ContactRepository(ApplicationDBContext context)
    {
        _context = context;
    }

    public async Task<ContactModel> CreateAsync(ContactModel contactModel)
    {
        try
        {
            await _context.Contacts.AddAsync(contactModel);
            await _context.SaveChangesAsync();
            return contactModel;
        }
        catch (Exception e)
        {
            throw new Exception("Failed to create contact", e);
        }
    }

    public async Task<ContactModel?> DeleteAsync(int id)
    {
        // Try to find contact with given ID
        var contact = await _context.Contacts.FirstOrDefaultAsync(x => x.Id == id);
        if (contact != null)
        {
            // Remove contact and save changes
            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
        }

        return contact;
    }

    public async Task<ContactModel?> GetByIdAsync(int id)
    {
        return await _context.Contacts.FindAsync(id);
    }
    public async Task<ContactModel?> UpdateAsync(int id, ContactCreateAndUpdateDto contactCreateAndUpdateDto)
    {
        // Find existing contact
        var existingContacts = await _context.Contacts.FirstOrDefaultAsync(x => x.Id == id);
        if (existingContacts == null)
        {
            return null;
        }

        // Update all properties
        existingContacts.DateOfBirth = contactCreateAndUpdateDto.DateOfBirth;
        existingContacts.Email = contactCreateAndUpdateDto.Email;
        existingContacts.Name = contactCreateAndUpdateDto.Name;
        existingContacts.CategoryId = contactCreateAndUpdateDto.CategoryId;
        existingContacts.LastName = contactCreateAndUpdateDto.LastName;
        existingContacts.SubCategoryId = contactCreateAndUpdateDto.SubCategoryId;
        existingContacts.PhoneNumber = contactCreateAndUpdateDto.PhoneNumber;
        existingContacts.Password = contactCreateAndUpdateDto.Password;

        await _context.SaveChangesAsync();
        return existingContacts;
    }

    public async Task<List<ContactModel>> GetAllAsync(QueryObject query)
    {
        var contacts = _context.Contacts.AsQueryable();

        // Apply last name filter if provided
        if (!String.IsNullOrEmpty(query.LastName))
        {
            contacts = contacts.Where(x => x.LastName.Contains(query.LastName));
        }

        // Apply phone number filter if provided
        if (!String.IsNullOrEmpty(query.PhoneNumber))
        {
            contacts = contacts.Where(x => x.PhoneNumber.Contains(query.PhoneNumber));
        }
        return await contacts.ToListAsync();
    }

    public async Task<List<ContactModel>> GetAllAsync()
    {
        return await _context.Contacts.ToListAsync();
    }

    public async Task<bool> ExistsByEmail(string email)
    {
        return await _context.Contacts.AnyAsync(c => c.Email == email);
    }
}