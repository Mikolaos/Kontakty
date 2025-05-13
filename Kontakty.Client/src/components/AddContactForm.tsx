import { useState } from 'react';
import { ContactCreateAndUpdateDto } from '../types';
import { getToken } from '../auth';

interface Props {
  onAdded: () => void;
}

export default function AddContactForm({ onAdded }: Props) {
  const [formData, setFormData] = useState<ContactCreateAndUpdateDto>({
    name: '',
    lastName: '',
    email: '',
    password: '',
    categoryId: 0,
    subCategoryId: undefined,
    customSubCategory: '',
    phoneNumber: '',
    dateOfBirth: '',
  });

  const [errors, setErrors] = useState<Record<string, string[]>>({});

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: name === 'categoryId' || name === 'subCategoryId' ? Number(value) : value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setErrors({}); // Clear old errors

    try {
      const response = await fetch('http://localhost:5274/api/contact', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${getToken()}`,
        },
        body: JSON.stringify(formData),
      });

      if (response.ok) {
        onAdded();
        setFormData({
          name: '',
          lastName: '',
          email: '',
          password: '',
          categoryId: 0,
          subCategoryId: undefined,
          customSubCategory: '',
          phoneNumber: '',
          dateOfBirth: '',
        });
      } else if (response.status === 400) {
        const data = await response.json();
        if (data.errors) {
          setErrors(data.errors); // Set errors returned by the backend
        } else {
          alert('Invalid data.');
        }
      } else if (response.status === 409) {
        const data = await response.json();
        alert(data.message || 'Email address already exists.');
      } else {
        alert('Error adding contact');
      }
    } catch (err) {
      console.error(err);
      alert('Error adding contact');
    }
  };

  const renderError = (field: string) => {
    return errors[field] && <p style={{ color: 'red' }}>{errors[field].join(', ')}</p>;
  };

  return (
    <form onSubmit={handleSubmit}>
      <h2>Add Contact</h2>
      <input name="name" value={formData.name} onChange={handleChange} placeholder="First Name" />
      {renderError('Name')}

      <input name="lastName" value={formData.lastName} onChange={handleChange} placeholder="Last Name" />
      {renderError('LastName')}

      <input name="email" value={formData.email} onChange={handleChange} placeholder="Email" />
      {renderError('Email')}

      <input name="password" type="password" value={formData.password} onChange={handleChange} placeholder="Password" />
      {renderError('Password')}

      <select name="categoryId" value={formData.categoryId} onChange={handleChange}>
        <option value={0}>Select Category</option>
        <option value={1}>Business</option>
        <option value={2}>Personal</option>
        <option value={3}>Other</option>
      </select>
      {renderError('CategoryId')}

      {formData.categoryId === 1 && (
        <>
          <select name="subCategoryId" value={formData.subCategoryId || ''} onChange={handleChange}>
            <option value="">Select Subcategory</option>
            <option value={1}>Boss</option>
            <option value={2}>Client</option>
          </select>
          {renderError('SubCategoryId')}
        </>
      )}

      {formData.categoryId === 2 && (
        <>
          <input name="customSubCategory" value={formData.customSubCategory} onChange={handleChange} placeholder="Custom Subcategory" />
          {renderError('CustomSubCategory')}
        </>
      )}

      <input name="phoneNumber" value={formData.phoneNumber} onChange={handleChange} placeholder="Phone Number" />
      {renderError('PhoneNumber')}

      <input type="date" name="dateOfBirth" value={formData.dateOfBirth} onChange={handleChange} />
      {renderError('DateOfBirth')}

      <button type="submit">Add Contact</button>
    </form>
  );
}
