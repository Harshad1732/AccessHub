import { Alert, Box, Button, Dialog, DialogActions, DialogContent, DialogTitle, TextField } from '@mui/material';
import { DataGrid, type GridColDef } from '@mui/x-data-grid';
import { useEffect, useState } from 'react';
import api from '../api/client';
import type { Organization } from '../types';

export default function OrganizationsPage() {
  const [orgs, setOrgs] = useState<Organization[]>([]);
  const [error, setError] = useState('');
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState({ name: '', slug: '' });

  const load = async () => {
    try {
      const { data } = await api.get<Organization[]>('/api/organizations');
      setOrgs(data);
    } catch {
      setError('Failed to load organizations.');
    }
  };

  useEffect(() => {
    load();
  }, []);

  const columns: GridColDef[] = [
    { field: 'name', headerName: 'Name', flex: 1 },
    { field: 'slug', headerName: 'Slug', width: 160 },
    { field: 'isActive', headerName: 'Active', width: 100 },
  ];

  const handleCreate = async () => {
    try {
      await api.post('/api/organizations', form);
      setOpen(false);
      setForm({ name: '', slug: '' });
      await load();
    } catch {
      setError('Failed to create organization.');
    }
  };

  return (
    <>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
        <h2>Organizations</h2>
        <Button variant="contained" onClick={() => setOpen(true)}>
          Add Organization
        </Button>
      </Box>
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      <DataGrid rows={orgs} columns={columns} getRowId={(r) => r.id} autoHeight pageSizeOptions={[10]} initialState={{ pagination: { paginationModel: { pageSize: 10 } } }} />

      <Dialog open={open} onClose={() => setOpen(false)} fullWidth maxWidth="sm">
        <DialogTitle>Create Organization</DialogTitle>
        <DialogContent>
          <TextField fullWidth label="Name" margin="dense" value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} />
          <TextField fullWidth label="Slug" margin="dense" value={form.slug} onChange={(e) => setForm({ ...form, slug: e.target.value })} />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleCreate}>Create</Button>
        </DialogActions>
      </Dialog>
    </>
  );
}
