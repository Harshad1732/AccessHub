import {
  AppBar,
  Box,
  Button,
  Card,
  CardContent,
  Chip,
  Container,
  Link as MuiLink,
  Stack,
  Toolbar,
  Typography,
} from '@mui/material';
import { Link as RouterLink } from 'react-router-dom';

const features = [
  {
    title: 'Multi-tenant organizations',
    description: 'Each tenant gets an isolated organization with its own users, roles, and data.',
  },
  {
    title: 'Role-based access control',
    description: 'Compose roles from permissions and assign them to users per organization.',
  },
  {
    title: 'Fine-grained permissions',
    description: 'Granular permission codes gate every sensitive action across the platform.',
  },
  {
    title: 'JWT auth with API enforcement',
    description: 'Stateless JWT bearer tokens, validated and authorized on every API request.',
  },
  {
    title: 'Audit logging',
    description: 'Security-relevant actions are recorded as an immutable audit trail.',
  },
  {
    title: 'Sample protected Invoices API',
    description: 'A demo resource showing permission-enforced reads and writes end to end.',
  },
];

const techStack = [
  '.NET 8',
  'ASP.NET Core',
  'EF Core',
  'SQL Server',
  'JWT',
  'React',
  'TypeScript',
  'MUI',
];

export default function LandingPage() {
  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
      <AppBar position="static" elevation={0}>
        <Toolbar>
          <Typography variant="h6" sx={{ flexGrow: 1, fontWeight: 700 }}>
            AccessHub
          </Typography>
          <Stack direction="row" spacing={1}>
            <Button color="inherit" component={RouterLink} to="/login">
              Login
            </Button>
            <Button
              variant="contained"
              color="secondary"
              component={RouterLink}
              to="/register"
              sx={{ bgcolor: 'rgba(255,255,255,0.16)', '&:hover': { bgcolor: 'rgba(255,255,255,0.28)' } }}
            >
              Register
            </Button>
          </Stack>
        </Toolbar>
      </AppBar>

      <Box
        sx={{
          background: 'linear-gradient(135deg, #1565c0 0%, #1e88e5 100%)',
          color: 'common.white',
          py: { xs: 8, md: 12 },
        }}
      >
        <Container maxWidth="md">
          <Stack spacing={3} sx={{ alignItems: 'center', textAlign: 'center' }}>
            <Typography variant="h2" sx={{ fontWeight: 800, fontSize: { xs: '2.25rem', md: '3.5rem' } }}>
              AccessHub
            </Typography>
            <Typography variant="h5" sx={{ maxWidth: 720, opacity: 0.95 }}>
              Multi-tenant access control: organizations, roles, fine-grained permissions, and audit logs.
            </Typography>
            <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} sx={{ pt: 2 }}>
              <Button
                size="large"
                variant="contained"
                color="secondary"
                component={RouterLink}
                to="/register"
                sx={{ px: 4, bgcolor: 'common.white', color: 'primary.main', '&:hover': { bgcolor: 'grey.100' } }}
              >
                Get started — Register
              </Button>
              <Button
                size="large"
                variant="outlined"
                component={RouterLink}
                to="/login"
                sx={{ px: 4, color: 'common.white', borderColor: 'rgba(255,255,255,0.7)', '&:hover': { borderColor: 'common.white' } }}
              >
                Login
              </Button>
            </Stack>
          </Stack>
        </Container>
      </Box>

      <Container maxWidth="lg" sx={{ py: { xs: 6, md: 8 } }}>
        <Stack spacing={2} sx={{ mb: 6 }}>
          <Typography variant="h4" sx={{ fontWeight: 700 }}>
            About this project
          </Typography>
          <Typography variant="body1" color="text.secondary" sx={{ maxWidth: 820 }}>
            AccessHub is a portfolio Identity &amp; Access Management (IAM) / role-based access
            control (RBAC) system. It demonstrates multi-tenant organizations, composable roles,
            fine-grained permissions, JWT authentication enforced at the API, and audit logging.
            The backend is built with ASP.NET Core 8, Entity Framework Core, and SQL Server with
            JWT-based auth; the frontend is a React + TypeScript single-page app using MUI.
          </Typography>
        </Stack>

        <Typography variant="h4" sx={{ fontWeight: 700, mb: 3 }}>
          Features
        </Typography>
        <Box
          sx={{
            display: 'grid',
            gap: 3,
            gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr', md: '1fr 1fr 1fr' },
            mb: 6,
          }}
        >
          {features.map((feature) => (
            <Card key={feature.title} variant="outlined" sx={{ height: '100%' }}>
              <CardContent>
                <Typography variant="h6" sx={{ fontWeight: 600, mb: 1 }}>
                  {feature.title}
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  {feature.description}
                </Typography>
              </CardContent>
            </Card>
          ))}
        </Box>

        <Typography variant="h4" sx={{ fontWeight: 700, mb: 3 }}>
          Built with
        </Typography>
        <Stack direction="row" spacing={1} sx={{ flexWrap: 'wrap', gap: 1 }}>
          {techStack.map((tech) => (
            <Chip key={tech} label={tech} color="primary" variant="outlined" />
          ))}
        </Stack>
      </Container>

      <Box component="footer" sx={{ mt: 'auto', py: 3, bgcolor: 'grey.100' }}>
        <Container maxWidth="lg">
          <Stack
            direction={{ xs: 'column', sm: 'row' }}
            spacing={1}
            sx={{ justifyContent: 'space-between', alignItems: 'center' }}
          >
            <Typography variant="body2" color="text.secondary">
              AccessHub — a portfolio IAM/RBAC project.
            </Typography>
            <MuiLink
              href="https://github.com/Harshad1732/AccessHub"
              target="_blank"
              rel="noopener"
              variant="body2"
            >
              View source on GitHub
            </MuiLink>
          </Stack>
        </Container>
      </Box>
    </Box>
  );
}
