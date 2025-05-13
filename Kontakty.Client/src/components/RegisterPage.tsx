import { useState } from 'react';
import axios from 'axios';

interface RegisterFormProps {
  onRegister: () => void;
}

export default function RegisterForm({ onRegister }: RegisterFormProps) {
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const res = await axios.post('http://localhost:5274/api/account/register', {
        username,
        email,
        password
      });
      localStorage.setItem('token', res.data.token);
      onRegister();
    } catch (err: any) {
      const message =
        err?.response?.data?.message || // if the backend returned a message
        (Array.isArray(err?.response?.data)
          ? err.response.data.map((e: any) => e.description || e).join(', ')
          : '') ||
        err?.message ||
        'Unknown registration error';

      alert('Registration failed: ' + message);
    }
  };

  return (
    <form onSubmit={handleRegister}>
      <h2>Register</h2>
      <input
        type="text"
        placeholder="Username"
        value={username}
        onChange={(e) => setUsername(e.target.value)}
        required
      /><br/>
      <input
        type="email"
        placeholder="Email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        required
      /><br/>
      <input
        type="password"
        placeholder="Password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        required
      /><br/>
      <button type="submit">Register</button>
    </form>
  );
}
