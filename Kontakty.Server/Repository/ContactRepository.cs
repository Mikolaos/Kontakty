using Kontakty.Data;
using Kontakty.DTOs;
using Kontakty.Helpers;
using Kontakty.Interfaces;
using Kontakty.Models;
using Microsoft.EntityFrameworkCore;

namespace Kontakty.Repository;

/// <summary>
/// Repository handling CRUD operations for contacts
/// </summary>
public class ContactRepository: IContactRepository
{
    // Database context for accessing contacts data
    private readonly ApplicationDBContext _context;

    /// <summary>
    /// Constructor initializing database context
    /// </summary>
    public ContactRepository(ApplicationDBContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adds a new contact to the database
    /// </summary>
    /// <returns>Created contact with assigned ID</returns>
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

    /// <summary>
    /// Removes specified contact from the database
    /// </summary>
    /// <param name="id">ID of contact to delete</param>
    /// <returns>Deleted contact or null if not found</returns>
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

    /// <summary>
    /// Retrieves a single contact by ID
    /// </summary>
    /// <param name="id">ID of contact to find</param>
    /// <returns>Found contact or null if not exists</returns>
    public async Task<ContactModel?> GetByIdAsync(int id)
    {
        return await _context.Contacts.FindAsync(id);
    }

    /// <summary>
    /// Updates existing contact with new data
    /// </summary>
    /// <param name="id">ID of contact to update</param>
    /// <param name="contactCreateAndUpdateDto">New contact data</param>
    /// <returns>Updated contact or null if not found</returns>
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

    /// <summary>
    /// Gets filtered list of contacts
    /// </summary>
    /// <param name="query">Filter criteria</param>
    /// <returns>List of contacts matching criteria</returns>
    public async Task<List<ContactModel>> GetAllAsync(QueryObject query)
    {
        var contacts = _context.Contacts.AsQueryable();

        // Apply last name filter if provided
        if(!String.IsNullOrEmpty(query.LastName))
        {
            contacts = contacts.Where(x => x.LastName.Contains(query.LastName));
        }

        // Apply phone number filter if provided
        if(!String.IsNullOrEmpty(query.PhoneNumber))
        {
            contacts = contacts.Where(x => x.PhoneNumber.Contains(query.PhoneNumber));
        }
        return await contacts.ToListAsync();
    }

    /// <summary>
    /// Gets all contacts without filtering
    /// </summary>
    /// <returns>List of all contacts</returns>
    public async Task<List<ContactModel>> GetAllAsync()
    {
        return await _context.Contacts.ToListAsync();
    }
    
    /// <summary>
    /// Checks if contact with given email exists
    /// </summary>
    /// <param name="email">Email to check</param>
    /// <returns>True if email exists, false otherwise</returns>
    public async Task<bool> ExistsByEmail(string email)
    {
        return await _context.Contacts.AnyAsync(c => c.Email == email);
    }
}