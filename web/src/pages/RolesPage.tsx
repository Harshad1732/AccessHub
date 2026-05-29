import {
  Alert,
  Box,
  Button,
  Checkbox,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControlLabel,
  FormGroup,
  TextField,
} from '@mui/material';
import { DataGrid, type GridColDef } from '@mui/x-data-grid';
import { useEffect, useState } from 'react';
import api from '../api/client';
import { useAuth } from '../context/AuthContext';
import type { Permission, Role } from '../types';

export default function RolesPage() {
  const { user, hasPermission } = useAuth();
  const orgId = user?.organizationId;
  const [roles, setRoles] = useState<Role[]>([]);
  const [permissions, setPermissions] = useState<Permission[]>([]);
  const [error, setError] = useState('');
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState({ name: '', description: '', permissionCodes: [] as string[] });

  const load = async () => {
    if (!orgId) return;
    try {
      const [rolesRes, permsRes] = await Promise.all([
        api.get<Role[]>('/api/roles', { params: { organizationId: orgId } }),
        api.get<Permission[]>('/api/permissions'),
      ]);
      setRoles(rolesRes.data);
      setPermissions(permsRes.data);
    } catch {
      setError('Failed to load roles.');
    }
  };

  useEffect(() => {
    load();
  }, [orgId]);

  const columns: GridColDef[] = [
    { field: 'name', headerName: 'Role', flex: 1 },
    { field: 'description', headerName: 'Description', flex: 1 },
    {
      field: 'permissionCodes',
      headerName: 'Permissions',
      flex: 2,
      valueGetter: (_, row) => (row as Role).permissionCodes.join(', '),
    },
  ];

  const handleCreate = async () => {
    if (!orgId) return;
    try {
      await api.post('/api/roles', form, { params: { organizationId: orgId } });
      setOpen(false);
      setForm({ name: '', description: '', permissionCodes: [] });
      await load();
    } catch {
      setError('Failed to create role.');
    }
  };

  const togglePermission = (code: string) => {
    setForm((f) => ({
      ...f,
      permissionCodes: f.permissionCodes.includes(code)
        ? f.permissionCodes.filter((c) => c !== code)
        : [...f.permissionCodes, code],
    }));
  };

  if (!orgId) return <Alert severity="warning">Organization context required.</Alert>;

  return (
    <>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
        <h2>Roles</h2>
        {hasPermission('roles.write') && (
          <Button variant="contained" onClick={() => setOpen(true)}>
            Add Role
          </Button>
        )}
      </Box>
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      <DataGrid rows={roles} columns={columns} getRowId={(r) => r.id} autoHeight pageSizeOptions={[10]} initialState={{ pagination: { paginationModel: { pageSize: 10 } } }} />

      <Dialog open={open} onClose={() => setOpen(false)} fullWidth maxWidth="sm">
        <DialogTitle>Create Role</DialogTitle>
        <DialogContent>
          <TextField fullWidth label="Name" margin="dense" value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} />
          <TextField fullWidth label="Description" margin="dense" value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} />
          <FormGroup sx={{ mt: 1 }}>
            {permissions.map((p) => (
              <FormControlLabel
                key={p.id}
                control={
                  <Checkbox
                    checked={form.permissionCodes.includes(p.code)}
                    onChange={() => togglePermission(p.code)}
                  />
                }
                label={`${p.displayName} (${p.code})`}
              />
            ))}
          </FormGroup>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleCreate}>Create</Button>
        </DialogActions>
      </Dialog>
    </>
  );
}
