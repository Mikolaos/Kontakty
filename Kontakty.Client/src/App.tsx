import { useState } from 'react';
import ContactList from './components/ContactList';
import ContactDetails from './components/ContactDetails';
import LoginForm from './components/LoginPage';
import RegisterForm from './components/RegisterPage';
import EditContactForm from './components/EditContactForm';
import AddContactForm from './components/AddContactForm';
import { getToken, removeToken } from './auth';

export default function App() {
  const [selectedId, setSelectedId] = useState<number | null>(null);
  const [token, setToken] = useState<string | null>(getToken());
  const [formToShow, setFormToShow] = useState<'login' | 'register' | null>(null);
  const [editId, setEditId] = useState<number | null>(null);
  const [addFormVisible, setAddFormVisible] = useState(false);
  const [refreshKey, setRefreshKey] = useState(0);

  // Handles successful login
  const handleLoginSuccess = () => {
    setToken(getToken());
    setFormToShow(null);
  };

  // Handles logout
  const handleLogout = () => {
    removeToken();
    setToken(null);
    setSelectedId(null);
    setEditId(null);
  };

  // Handles deleting a contact
  const handleDelete = async (id: number) => {
    if (!window.confirm('Are you sure you want to delete this contact?')) return;

    try {
      await fetch(`http://localhost:5274/api/contact/${id}`, {
        method: 'DELETE',
        headers: {
          Authorization: `Bearer ${getToken()}`
        }
      });
      setRefreshKey(prev => prev + 1);
      if (selectedId === id) setSelectedId(null);
    } catch (err) {
      alert('Error deleting contact');
    }
  };

  // Handles showing the add contact form
  const handleAddContact = () => {
    setAddFormVisible(true);
  };

  // Called after a contact is added
  const handleContactAdded = () => {
    setAddFormVisible(false);
    setRefreshKey(prev => prev + 1);
  };

  return (
    <div style={{
      maxWidth: '800px',
      margin: '0 auto',
      padding: '2rem',
      textAlign: 'center',
      fontFamily: 'sans-serif',
      alignItems: 'center'
    }}>
      {/* If not logged in */}
      {!token && (
        <div style={{ marginBottom: '1rem' }}>
          <button onClick={() => setFormToShow('login')}>Log in</button>
          <button onClick={() => setFormToShow('register')}>Register</button>
        </div>
      )}

      {/* If logged in */}
      {token && (
        <div style={{ marginBottom: '1rem' }}>
          <button onClick={handleLogout}>Log out</button>
          <button onClick={handleAddContact}>Add contact</button>
        </div>
      )}

      {/* Login and register forms */}
      {formToShow === 'login' && <LoginForm onLogin={handleLoginSuccess} />}
      {formToShow === 'register' && <RegisterForm onRegister={handleLoginSuccess} />}

      {/* Add contact form */}
      {addFormVisible && <AddContactForm onAdded={handleContactAdded} />}

      <h1>Contact list</h1>

      {/* Contact list */}
      <ContactList
        key={refreshKey}
        onSelect={(id) => {
          setSelectedId(id);
          setEditId(null);
        }}
        onEdit={(id) => {
          setEditId(id);
          setSelectedId(null);
        }}
        onDelete={handleDelete}
      />

      {/* Contact details */}
      {token && selectedId !== null && (
        <ContactDetails id={selectedId} />
      )}

      {/* Edit contact form */}
      {token && editId !== null && (
        <EditContactForm
          id={editId}
          onSaved={() => {
            setEditId(null);
            setRefreshKey(prev => prev + 1);
          }}
        />
      )}
    </div>
  );
}
