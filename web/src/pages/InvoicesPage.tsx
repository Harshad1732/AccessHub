import {
  Alert,
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  TextField,
} from '@mui/material';
import { DataGrid, type GridColDef } from '@mui/x-data-grid';
import { useEffect, useState } from 'react';
import api from '../api/client';
import { useAuth } from '../context/AuthContext';
import type { Invoice } from '../types';

export default function InvoicesPage() {
  const { hasPermission } = useAuth();
  const [invoices, setInvoices] = useState<Invoice[]>([]);
  const [error, setError] = useState('');
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState({ number: '', customerName: '', amount: 0 });

  const load = async () => {
    try {
      const { data } = await api.get<Invoice[]>('/api/invoices');
      setInvoices(data);
    } catch (e: unknown) {
      const status = (e as { response?: { status?: number } })?.response?.status;
      setError(status === 403 ? 'You do not have permission to view invoices.' : 'Failed to load invoices.');
    }
  };

  useEffect(() => {
    load();
  }, []);

  const columns: GridColDef[] = [
    { field: 'number', headerName: 'Number', width: 140 },
    { field: 'customerName', headerName: 'Customer', flex: 1 },
    { field: 'amount', headerName: 'Amount', width: 120 },
    { field: 'createdAtUtc', headerName: 'Created', width: 200 },
  ];

  const handleCreate = async () => {
    try {
      await api.post('/api/invoices', form);
      setOpen(false);
      setForm({ number: '', customerName: '', amount: 0 });
      await load();
    } catch {
      setError('Failed to create invoice (check invoices.write permission).');
    }
  };

  return (
    <>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
        <h2>Invoices</h2>
        {hasPermission('invoices.write') && (
          <Button variant="contained" onClick={() => setOpen(true)}>
            New Invoice
          </Button>
        )}
      </Box>
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      <DataGrid rows={invoices} columns={columns} getRowId={(r) => r.id} autoHeight pageSizeOptions={[10]} initialState={{ pagination: { paginationModel: { pageSize: 10 } } }} />

      <Dialog open={open} onClose={() => setOpen(false)} fullWidth maxWidth="sm">
        <DialogTitle>New Invoice</DialogTitle>
        <DialogContent>
          <TextField fullWidth label="Number" margin="dense" value={form.number} onChange={(e) => setForm({ ...form, number: e.target.value })} />
          <TextField fullWidth label="Customer" margin="dense" value={form.customerName} onChange={(e) => setForm({ ...form, customerName: e.target.value })} />
          <TextField fullWidth label="Amount" type="number" margin="dense" value={form.amount} onChange={(e) => setForm({ ...form, amount: Number(e.target.value) })} />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleCreate}>Create</Button>
        </DialogActions>
      </Dialog>
    </>
  );
}
