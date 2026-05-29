import { Alert, Typography } from '@mui/material';
import { useAuth } from '../context/AuthContext';

export default function DashboardPage() {
  const { user } = useAuth();

  return (
    <>
      <Typography variant="h4" gutterBottom>
        Dashboard
      </Typography>
      <Alert severity="info">
        Signed in as <strong>{user?.email}</strong> with {user?.permissions.length ?? 0}{' '}
        permission(s). Organization: {user?.organizationName ?? 'All (Super Admin)'}.
      </Alert>
    </>
  );
}
