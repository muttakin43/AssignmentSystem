import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';
import { ShellComponent } from './shared/components/shell/shell/shell';
export const routes: Routes =  [
    
     {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login').then((m) => m.Login)
  },
  {
    path: 'forbidden',
    loadComponent: () => import('./features/forbidden/forbidden').then((m) => m.Forbidden)
  },
  {
  path: 'admin',
  component: ShellComponent,
  canActivate: [authGuard, roleGuard],
  data: { roles: ['Admin'] },
  loadChildren: () =>
    import('./features/admin/admin.routes')
      .then(m => m.ADMIN_ROUTES)
},
  {
    path: 'teacher',
    component: ShellComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Teacher'] },
    loadChildren: () => import('./features/teacher/teacher.routes').then((m) => m.TEACHER_ROUTES)
  },
  {
    path: 'student',
    component: ShellComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Student'] },
    loadChildren: () => import('./features/student/student.routes').then((m) => m.STUDENT_ROUTES)
  },
  { path: '', pathMatch: 'full', redirectTo: 'login' },
  { path: '**', redirectTo: 'login' }
];
