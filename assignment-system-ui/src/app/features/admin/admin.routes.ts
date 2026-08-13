import { Routes } from '@angular/router';

export const ADMIN_ROUTES: Routes = [
  {
    path: '',
    redirectTo: 'classes',
    pathMatch: 'full'
  },
  {
  path: 'classes',
  loadComponent: () => import('./classes/class-list/class-list').then(m => m.ClassList)
},
  {
    path: 'classes/new',
    loadComponent: () => import('./classes/class-form/class-form').then(m => m.ClassForm)
  },
  {
    path: 'classes/:id/edit',
    loadComponent: () => import('./classes/class-form/class-form').then(m => m.ClassForm)
  },
  {
    path: 'subjects',
    loadComponent: () => import('./subjects/subject-list/subject-list').then(m => m.SubjectList)
  },
  {
    path: 'subjects/new',
    loadComponent: () => import('./subjects/subject-form/subject-form').then(m => m.SubjectForm)
  },
  {
    path: 'subjects/:id/edit',
    loadComponent: () => import('./subjects/subject-form/subject-form').then(m => m.SubjectForm)
  },
  {
  path: 'users',
  loadComponent: () => import('./users/user-list/user-list').then(m => m.UserList)
},
{
  path: 'users/new',
  loadComponent: () => import('./users/user-form/user-form').then(m => m.UserForm)
},
{
  path: 'users/:id/edit',
  loadComponent: () => import('./users/user-form/user-form').then(m => m.UserForm)
},
  {
  path: 'teacher-assignments',
  loadComponent: () => import('./teacher-assignments/teacher-assignment-list/teacher-assignment-list').then(m => m.TeacherAssignmentList)
},
  {
  path: 'settings',
  loadComponent: () => import('./settings/settings-list/settings-list').then(m => m.SettingsList)
}
];