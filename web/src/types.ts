export interface LoginResponse {
  token: string;
  userId: string;
  email: string;
  fullName: string;
  organizationId: string | null;
  organizationName: string | null;
  isSuperAdmin: boolean;
  permissions: string[];
}

export interface Organization {
  id: string;
  name: string;
  slug: string;
  isActive: boolean;
  createdAtUtc: string;
}

export interface User {
  id: string;
  email: string;
  fullName: string;
  organizationId: string | null;
  isActive: boolean;
  isSuperAdmin: boolean;
  roleIds: string[];
}

export interface Role {
  id: string;
  organizationId: string;
  name: string;
  description: string | null;
  permissionCodes: string[];
}

export interface Permission {
  id: string;
  code: string;
  displayName: string;
  description: string | null;
}

export interface Invoice {
  id: string;
  number: string;
  customerName: string;
  amount: number;
  createdAtUtc: string;
}

export interface AuditEvent {
  id: string;
  organizationId: string | null;
  actorUserId: string;
  action: string;
  entityType: string;
  entityId: string | null;
  payloadJson: string | null;
  createdAtUtc: string;
}
