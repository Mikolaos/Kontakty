import { useEffect, useState } from 'react';
import axios from 'axios';
import { ContactCreateAndUpdateDto } from '../types';

interface Props {
  id: number;
  onSaved: () => void;
}

export default function EditContactForm({ id, onSaved }: Props) {
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

  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    axios.get(`http://localhost:5274/api/contact/${id}`)
      .then(res => setFormData({ ...res.data, password: '' }))
      .catch(console.error);
  }, [id]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: name === 'categoryId' || name === 'subCategoryId' ? Number(value) : value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setErrors({}); // Reset errors before submission
    try {
      await axios.put(`http://localhost:5274/api/contact/${id}`, formData, {
        headers: {
          Authorization: `Bearer ${localStorage.getItem('token')}`,
        },
      });
      onSaved();
    } catch (error: any) {
      if (error.response && error.response.data && error.response.data.errors) {
        // Handle validation errors from the backend
        const backendErrors: Record<string, string> = {};
        for (const key in error.response.data.errors) {
          backendErrors[key] = error.response.data.errors[key].join(', ');
        }
        setErrors(backendErrors);
      } else {
        alert('An error occurred while saving changes.');
      }
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <h3>Edit Contact</h3>
      <input
        name="name"
        value={formData.name}
        onChange={handleChange}
        placeholder="First Name"
        required
      />
      {errors.name && <p style={{ color: 'red' }}>{errors.name}</p>}
      <br />
      <input
        name="lastName"
        value={formData.lastName}
        onChange={handleChange}
        placeholder="Last Name"
        required
      />
      {errors.lastName && <p style={{ color: 'red' }}>{errors.lastName}</p>}
      <br />
      <input
        name="email"
        value={formData.email}
        onChange={handleChange}
        placeholder="Email"
        required
      />
      {errors.email && <p style={{ color: 'red' }}>{errors.email}</p>}
      <br />
      <input
        name="password"
        value={formData.password}
        onChange={handleChange}
        placeholder="Password"
        required
      />
      {errors.password && <p style={{ color: 'red' }}>{errors.password}</p>}
      <br />
      <select
        name="categoryId"
        value={formData.categoryId}
        onChange={handleChange}
        required
      >
        <option value={0}>Select Category</option>
        <option value={1}>Business</option>
        <option value={2}>Personal</option>
      </select>
      {errors.categoryId && <p style={{ color: 'red' }}>{errors.categoryId}</p>}
      <br />
      {formData.categoryId === 1 && (
        <select
          name="subCategoryId"
          value={formData.subCategoryId || ''}
          onChange={handleChange}
        >
          <option value="">Select Subcategory</option>
          <option value={1}>Boss</option>
          <option value={2}>Client</option>
        </select>
      )}
      {formData.categoryId === 2 && (
        <input
          name="customSubCategory"
          value={formData.customSubCategory}
          onChange={handleChange}
          placeholder="Custom Subcategory"
        />
      )}
      <input
        name="phoneNumber"
        value={formData.phoneNumber}
        onChange={handleChange}
        placeholder="Phone Number"
      />
      {errors.phoneNumber && <p style={{ color: 'red' }}>{errors.phoneNumber}</p>}
      <br />
      <input
        type="date"
        name="dateOfBirth"
        value={formData.dateOfBirth}
        onChange={handleChange}
        required
      />
      {errors.dateOfBirth && <p style={{ color: 'red' }}>{errors.dateOfBirth}</p>}
      <br />
      <button type="submit">Save</button>
    </form>
  );
}