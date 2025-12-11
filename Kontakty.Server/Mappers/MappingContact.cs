using Kontakty.Models;
using Kontakty.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Kontakty.Mappings;

public static class MappingContact
{

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