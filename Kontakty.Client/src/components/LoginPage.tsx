import { useState } from 'react';
import { setToken } from '../auth';

export default function LoginPage({ onLogin }: { onLogin: () => void }) {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);

  async function login() {
    setError(null); // Reset error
    try {
      const res = await fetch('/api/account/login', { 
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, password }),
      });

      const text = await res.text();
      const data = text ? JSON.parse(text) : {};

      if (!res.ok) {
        throw new Error(data.message || 'Login error');
      }

      if (!data.token) {
        throw new Error('No token in response.');
      }

      setToken(data.token);
      onLogin();
    } catch (err: any) {
      setError(err.message || 'Unknown login error');
    }
  }

  return (
    <div>
      <h2>Login</h2>
      <input
        value={username}
        onChange={(e) => setUsername(e.target.value)}
        placeholder="Username"
      /><br/>
      <input
        type="password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        placeholder="Password"
      /><br/>
      <button onClick={login}>Log in</button>
      {error && <p style={{ color: 'red' }}>{error}</p>}
    </div>
  );
}
