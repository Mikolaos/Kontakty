import { useEffect, useState } from 'react';
import { api } from '../api';
import { ContactListDto } from '../types';
import { getToken } from '../auth'; // Zakładając, że masz metodę getToken

interface Props {
  onSelect: (id: number) => void;
  onEdit: (id: number) => void;
  onDelete: (id: number) => void;
}

export default function ContactList({ onSelect, onEdit, onDelete }: Props) {
  const [contacts, setContacts] = useState<ContactListDto[]>([]);
  const token = getToken(); // Pobierz token, sprawdzamy, czy użytkownik jest zalogowany

  useEffect(() => {
    api<ContactListDto[]>('http://localhost:5274/api/contact')
      .then(setContacts)
      .catch(console.error);
  }, []);

  return (
    <div>
      <ul style={{ listStyle: 'none', padding: 0 }}>
        {contacts.map((c) => (
          <li
            key={c.id}
            style={{
              marginBottom: '1rem',
              border: '1px solid #ccc',
              padding: '0.5rem',
              backgroundColor: '#808080',
              borderRadius: '8px',
            }}
          >
            <strong>{c.name} {c.lastName}</strong><br />
            📧 {c.email}<br />
            📞 {c.phoneNumber}<br />
            {/* Show buttons only for logged in user */}
            {token && (
              <div style={{ marginTop: '0.5rem' }}>
                <button
                  onClick={() => onSelect(c.id)}
                  style={{
                    backgroundColor: '#4CAF50',
                    color: 'white',
                    padding: '5px 10px',
                    border: 'none',
                    borderRadius: '4px',
                    cursor: 'pointer',
                    marginRight: '5px',
                  }}
                >
                  Info
                </button>
                <button
                  onClick={() => onEdit(c.id)}
                  style={{
                    backgroundColor: '#FFA500',
                    color: 'white',
                    padding: '5px 10px',
                    border: 'none',
                    borderRadius: '4px',
                    cursor: 'pointer',
                    marginRight: '5px',
                  }}
                >
                  Edit
                </button>
                <button
                  onClick={() => onDelete(c.id)}
                  style={{
                    backgroundColor: '#f44336',
                    color: 'white',
                    padding: '5px 10px',
                    border: 'none',
                    borderRadius: '4px',
                    cursor: 'pointer',
                  }}
                >
                  Delete
                </button>
              </div>
            )}
          </li>
        ))}
      </ul>
    </div>
  );
}
