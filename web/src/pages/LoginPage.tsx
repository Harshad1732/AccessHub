import { Alert, Box, Button, Link as MuiLink, Paper, Stack, TextField, Typography } from '@mui/material';
import { useState } from 'react';
import { Link as RouterLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [email, setEmail] = useState('admin@acme.local');
  const [password, setPassword] = useState('Admin123!');
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    try {
      await login(email, password);
      navigate('/app');
    } catch {
      setError('Invalid email or password.');
    }
  };

  return (
    <Box sx={{ display: 'flex', justifyContent: 'center', mt: 10 }}>
      <Paper sx={{ p: 4, width: 400 }}>
        <Typography variant="h5" gutterBottom>
          AccessHub Sign In
        </Typography>
        {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
        <form onSubmit={handleSubmit}>
          <TextField
            fullWidth
            label="Email"
            margin="normal"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
          <TextField
            fullWidth
            label="Password"
            type="password"
            margin="normal"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
          <Button type="submit" variant="contained" fullWidth sx={{ mt: 2 }}>
            Sign In
          </Button>
        </form>
        <Typography variant="caption" sx={{ mt: 2, display: 'block' }}>
          Try: admin@acme.local / Admin123! or viewer@acme.local / Viewer123!
        </Typography>
        <Stack direction="row" spacing={0.5} sx={{ mt: 2 }}>
          <Typography variant="body2">Don&apos;t have an account?</Typography>
          <MuiLink component={RouterLink} to="/register" variant="body2">
            Register
          </MuiLink>
        </Stack>
      </Paper>
    </Box>
  );
}
