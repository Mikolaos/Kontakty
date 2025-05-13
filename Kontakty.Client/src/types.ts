export interface ContactListDto {
  id: number;
  name: string;
  lastName: string;
  email: string;
  phoneNumber: string;
}

export interface ContactDetailDto extends ContactListDto {
  categoryId: number;
  categoryName: string;
  subCategoryId?: number;
  subCategoryName?: string;
  customSubCategory?: string;
  dateOfBirth: string;
  password: string;
}

export interface ContactCreateAndUpdateDto {
  name: string;
  lastName: string;
  email: string;
  password: string;
  categoryId: number;
  subCategoryId?: number;
  customSubCategory?: string;
  phoneNumber: string;
  dateOfBirth: string;
}