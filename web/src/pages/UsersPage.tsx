import {
  Alert,
  Box,
  Button,
  Chip,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControl,
  InputLabel,
  MenuItem,
  Select,
  TextField,
} from '@mui/material';
import { DataGrid, type GridColDef } from '@mui/x-data-grid';
import { useEffect, useState } from 'react';
import api from '../api/client';
import { useAuth } from '../context/AuthContext';
import type { Role, User } from '../types';

export default function UsersPage() {
  const { user, hasPermission } = useAuth();
  const [users, setUsers] = useState<User[]>([]);
  const [roles, setRoles] = useState<Role[]>([]);
  const [error, setError] = useState('');
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState({ email: '', password: '', fullName: '', roleIds: [] as string[] });

  const orgId = user?.organizationId;

  const load = async () => {
    if (!orgId) return;
    try {
      const [usersRes, rolesRes] = await Promise.all([
        api.get<User[]>('/api/users', { params: { organizationId: orgId } }),
        api.get<Role[]>('/api/roles', { params: { organizationId: orgId } }),
      ]);
      setUsers(usersRes.data);
      setRoles(rolesRes.data);
    } catch {
      setError('Failed to load users.');
    }
  };

  useEffect(() => {
    load();
  }, [orgId]);

  const columns: GridColDef[] = [
    { field: 'email', headerName: 'Email', flex: 1 },
    { field: 'fullName', headerName: 'Name', flex: 1 },
    {
      field: 'isActive',
      headerName: 'Status',
      width: 120,
      renderCell: (p) => (
        <Chip label={p.value ? 'Active' : 'Inactive'} color={p.value ? 'success' : 'default'} size="small" />
      ),
    },
  ];

  const handleCreate = async () => {
    if (!orgId) return;
    try {
      await api.post('/api/users', { ...form, organizationId: orgId });
      setOpen(false);
      setForm({ email: '', password: '', fullName: '', roleIds: [] });
      await load();
    } catch {
      setError('Failed to create user.');
    }
  };

  if (!orgId) return <Alert severity="warning">Select an organization context to manage users.</Alert>;

  return (
    <>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
        <h2>Users</h2>
        {hasPermission('users.write') && (
          <Button variant="contained" onClick={() => setOpen(true)}>
            Add User
          </Button>
        )}
      </Box>
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      <DataGrid rows={users} columns={columns} getRowId={(r) => r.id} autoHeight pageSizeOptions={[10]} initialState={{ pagination: { paginationModel: { pageSize: 10 } } }} />

      <Dialog open={open} onClose={() => setOpen(false)} fullWidth maxWidth="sm">
        <DialogTitle>Create User</DialogTitle>
        <DialogContent>
          <TextField fullWidth label="Email" margin="dense" value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} />
          <TextField fullWidth label="Password" type="password" margin="dense" value={form.password} onChange={(e) => setForm({ ...form, password: e.target.value })} />
          <TextField fullWidth label="Full Name" margin="dense" value={form.fullName} onChange={(e) => setForm({ ...form, fullName: e.target.value })} />
          <FormControl fullWidth margin="dense">
            <InputLabel>Roles</InputLabel>
            <Select
              multiple
              value={form.roleIds}
              label="Roles"
              onChange={(e) => setForm({ ...form, roleIds: e.target.value as string[] })}
            >
              {roles.map((r) => (
                <MenuItem key={r.id} value={r.id}>{r.name}</MenuItem>
              ))}
            </Select>
          </FormControl>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleCreate}>Create</Button>
        </DialogActions>
      </Dialog>
    </>
  );
}
