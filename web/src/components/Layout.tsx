import {
  AppBar,
  Box,
  Button,
  Container,
  Drawer,
  List,
  ListItemButton,
  ListItemText,
  Toolbar,
  Typography,
} from '@mui/material';
import { Link, Outlet, useLocation } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const navItems = [
  { path: '/', label: 'Dashboard', permission: null },
  { path: '/users', label: 'Users', permission: 'users.read' },
  { path: '/roles', label: 'Roles', permission: 'roles.read' },
  { path: '/invoices', label: 'Invoices', permission: 'invoices.read' },
  { path: '/audit', label: 'Audit Log', permission: 'audit.read' },
  { path: '/organizations', label: 'Organizations', permission: 'organizations.manage' },
];

export default function Layout() {
  const { user, logout, hasPermission } = useAuth();
  const location = useLocation();

  return (
    <Box sx={{ display: 'flex' }}>
      <AppBar position="fixed" sx={{ zIndex: (t) => t.zIndex.drawer + 1 }}>
        <Toolbar>
          <Typography variant="h6" sx={{ flexGrow: 1 }}>
            AccessHub
          </Typography>
          <Typography variant="body2" sx={{ mr: 2 }}>
            {user?.fullName} ({user?.organizationName ?? 'Super Admin'})
          </Typography>
          <Button color="inherit" onClick={logout}>
            Logout
          </Button>
        </Toolbar>
      </AppBar>
      <Drawer
        variant="permanent"
        sx={{
          width: 220,
          flexShrink: 0,
          [`& .MuiDrawer-paper`]: { width: 220, boxSizing: 'border-box', mt: 8 },
        }}
      >
        <List>
          {navItems
            .filter((item) => !item.permission || hasPermission(item.permission))
            .map((item) => (
              <ListItemButton
                key={item.path}
                component={Link}
                to={item.path}
                selected={location.pathname === item.path}
              >
                <ListItemText primary={item.label} />
              </ListItemButton>
            ))}
        </List>
      </Drawer>
      <Box component="main" sx={{ flexGrow: 1, p: 3, mt: 8, ml: '220px' }}>
        <Container maxWidth="lg">
          <Outlet />
        </Container>
      </Box>
    </Box>
  );
}
