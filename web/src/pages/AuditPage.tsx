import { Alert } from '@mui/material';
import { DataGrid, type GridColDef } from '@mui/x-data-grid';
import { useEffect, useState } from 'react';
import api from '../api/client';
import { useAuth } from '../context/AuthContext';
import type { AuditEvent } from '../types';

export default function AuditPage() {
  const { user } = useAuth();
  const [events, setEvents] = useState<AuditEvent[]>([]);
  const [error, setError] = useState('');

  useEffect(() => {
    const load = async () => {
      try {
        const { data } = await api.get<AuditEvent[]>('/api/audit', {
          params: user?.organizationId ? { organizationId: user.organizationId } : {},
        });
        setEvents(data);
      } catch {
        setError('Failed to load audit log.');
      }
    };
    load();
  }, [user?.organizationId]);

  const columns: GridColDef[] = [
    { field: 'createdAtUtc', headerName: 'When', width: 200 },
    { field: 'action', headerName: 'Action', width: 120 },
    { field: 'entityType', headerName: 'Entity', width: 120 },
    { field: 'entityId', headerName: 'Entity Id', width: 280 },
    { field: 'payloadJson', headerName: 'Payload', flex: 1 },
  ];

  return (
    <>
      <h2>Audit Log</h2>
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      <DataGrid rows={events} columns={columns} getRowId={(r) => r.id} autoHeight pageSizeOptions={[25]} initialState={{ pagination: { paginationModel: { pageSize: 25 } } }} />
    </>
  );
}
