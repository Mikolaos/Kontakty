import { useEffect, useState } from 'react';
import { api } from '../api';
import { ContactDetailDto } from '../types';

export default function ContactDetails({ id }: { id: number }) {
  const [contact, setContact] = useState<ContactDetailDto | null>(null);

  useEffect(() => {
    api<ContactDetailDto>(`http://localhost:5274/api/contact/${id}`)
      .then(setContact)
      .catch(console.error);
  }, [id]);

  if (!contact) return <p>Loading contact details...</p>;

  return (
    <div style={{ marginTop: '2rem', textAlign: 'left' }}>
      <h2>📌 Contact Details</h2>
      <p><strong>First Name:</strong> {contact.name}</p>
      <p><strong>Last Name:</strong> {contact.lastName}</p>
      <p><strong>Email:</strong> {contact.email}</p>
      <p><strong>Phone:</strong> {contact.phoneNumber}</p>
      <p><strong>Password:</strong> {contact.password}</p>
      <p><strong>Category:</strong> {contact.categoryName}</p>
      <p><strong>Subcategory:</strong> {contact.subCategoryName || contact.customSubCategory}</p>
      <p><strong>Date of Birth:</strong> {contact.dateOfBirth}</p>
    </div>
  );
}