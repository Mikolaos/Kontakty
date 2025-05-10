using Kontakty.Models;
using Kontakty.DTOs;
using Kontakty.Helpers;

namespace Kontakty.Interfaces;

/// <summary>
/// Interface representing the repository for handling contact-related operations.
/// Provides methods to perform CRUD operations and additional utility functions
/// for managing and retrieving contact data.
/// </summary>
public interface IContactRepository
{
    /// <summary>
    /// Retrieves all contacts from the repository.
    /// </summary>
    /// <returns>A list of all contact models.</returns>
    Task<List<ContactModel>> GetAllAsync(QueryObject query);

    /// <summary>
    /// Retrieves all contacts from the data source.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of all contacts.</returns>
    Task<List<ContactModel>> GetAllAsync();

    /// <summary>
    /// Asynchronously retrieves a single contact by its ID.
    /// </summary>
    /// <param name="id">The ID of the contact to retrieve.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the found contact if it exists, otherwise null.</returns>
    Task<ContactModel?> GetByIdAsync(int id);

    /// <summary>
    /// Adds a new contact to the database.
    /// </summary>
    /// <param name="contactModel">The contact information to be added.</param>
    /// <returns>The created contact with the assigned ID.</returns>
    Task<ContactModel> CreateAsync(ContactModel contactModel);

    /// <summary>
    /// Asynchronously updates an existing contact based on the provided ID and update data.
    /// </summary>
    /// <param name="id">The unique identifier of the contact to update.</param>
    /// <param name="contactCreateAndUpdateDto">The data transfer object containing updated contact information.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated contact model, or null if the contact was not found.</returns>
    Task<ContactModel?> UpdateAsync(int id,ContactCreateAndUpdateDto contactCreateAndUpdateDto);

    /// <summary>
    /// Deletes a contact by its ID.
    /// </summary>
    /// <param name="id">The ID of the contact to be deleted.</param>
    /// <returns>The deleted contact if it exists; otherwise, null.</returns>
    Task<ContactModel?> DeleteAsync(int id);

    /// <summary>
    /// Checks if a contact with the specified email address exists in the repository.
    /// </summary>
    /// <param name="email">The email address to check for existence.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains true if a contact with the email exists, otherwise false.</returns>
    Task<bool> ExistsByEmail(string email);
}