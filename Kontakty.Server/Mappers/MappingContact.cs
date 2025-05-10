using Kontakty.Models;
using Kontakty.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Kontakty.Mappings;

/// <summary>
/// Provides extension methods for mapping between various data transfer objects (DTOs) and the ContactModel.
/// </summary>
public static class MappingContact
{
    /// <summary>
    /// Maps a ContactCreateAndUpdateDto instance to a ContactModel instance.
    /// </summary>
    /// <param name="contactCreateAndUpdateDto">The DTO object containing data for creating or updating a contact.</param>
    /// <returns>A ContactModel object populated with the data from the provided DTO.</returns>
    public static ContactModel ToContactFromCreateDto(this ContactCreateAndUpdateDto contactCreateAndUpdateDto)
    {
        return new ContactModel
        {
            Name = contactCreateAndUpdateDto.Name,
            LastName = contactCreateAndUpdateDto.LastName,
            Email = contactCreateAndUpdateDto.Email,
            Password = contactCreateAndUpdateDto.Password,
            CategoryId = contactCreateAndUpdateDto.CategoryId,
            CategoryName = contactCreateAndUpdateDto.CategoryName,
            SubCategoryId = contactCreateAndUpdateDto.SubCategoryId,
            CustomSubCategory = contactCreateAndUpdateDto.CustomSubCategory,
            PhoneNumber = contactCreateAndUpdateDto.PhoneNumber,
            DateOfBirth = contactCreateAndUpdateDto.DateOfBirth
        };
    }

    /// <summary>
    /// Maps a <see cref="ContactModel"/> instance to a <see cref="ContactDetailDto"/> instance.
    /// </summary>
    /// <param name="contactModel">The <see cref="ContactModel"/> object containing contact details to map.</param>
    /// <returns>A <see cref="ContactDetailDto"/> object with mapped values from the <paramref name="contactModel"/>.</returns>
    public static ContactDetailDto ToContactDetailDto(this ContactModel contactModel)
    {
        return new ContactDetailDto
        {
            Id = contactModel.Id,
            Name = contactModel.Name,
            LastName = contactModel.LastName,
            Email = contactModel.Email,
            CategoryId = contactModel.CategoryId,
            CategoryName = contactModel.CategoryName,
            SubCategoryId = contactModel.SubCategoryId,
            SubCategoryName = contactModel.SubCategoryName,
            PhoneNumber = contactModel.PhoneNumber,
            DateOfBirth = contactModel.DateOfBirth,
            Password = contactModel.Password
        };
    }

    /// <summary>
    /// Maps a <see cref="ContactModel"/> instance to a <see cref="ContactListDto"/> instance.
    /// </summary>
    /// <param name="contactModel">The <see cref="ContactModel"/> instance to be mapped.</param>
    /// <returns>A <see cref="ContactListDto"/> instance containing basic information about the contact.</returns>
    public static ContactListDto ToContactListDto(this ContactModel contactModel)
    {
        return new ContactListDto
        {
            Id = contactModel.Id,
            Name = contactModel.Name,
            LastName = contactModel.LastName,
            Email = contactModel.Email,
            PhoneNumber = contactModel.PhoneNumber
        };
    }

}