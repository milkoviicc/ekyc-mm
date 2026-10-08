import { combineReducers } from '@reduxjs/toolkit';
import { A_Apl_Prmtr, A_ISRole, A_ISRole_Objct, A_Objct, A_Objct_Typs, A_Usr, A_Usr_ISRole } from '../../models';
import { createListFeature } from '../utils/createListFeature';

// Read-only admin endpoints under api/admin/* - one feature per A_ table. Locks have their own store (../adminLocks).
export const users = createListFeature<A_Usr>('admin/users', '/api/admin/users');
export const roles = createListFeature<A_ISRole>('admin/roles', '/api/admin/roles');
export const userRoles = createListFeature<A_Usr_ISRole>('admin/userRoles', '/api/admin/user-roles');
export const roleObjects = createListFeature<A_ISRole_Objct>('admin/roleObjects', '/api/admin/role-objects');
export const objects = createListFeature<A_Objct>('admin/objects', '/api/admin/objects');
export const objectTypes = createListFeature<A_Objct_Typs>('admin/objectTypes', '/api/admin/object-types');
export const parameters = createListFeature<A_Apl_Prmtr>('admin/parameters', '/api/admin/parameters');

export const getUsers = users.fetchAll;
export const getRoles = roles.fetchAll;
export const getUserRoles = userRoles.fetchAll;
export const getRoleObjects = roleObjects.fetchAll;
export const getObjects = objects.fetchAll;
export const getObjectTypes = objectTypes.fetchAll;
export const getParameters = parameters.fetchAll;

export const adminReducer = combineReducers({
  users: users.reducer,
  roles: roles.reducer,
  userRoles: userRoles.reducer,
  roleObjects: roleObjects.reducer,
  objects: objects.reducer,
  objectTypes: objectTypes.reducer,
  parameters: parameters.reducer,
});
