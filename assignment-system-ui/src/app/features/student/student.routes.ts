import { Routes } from '@angular/router';

export const STUDENT_ROUTES: Routes = [
  {
    path: '',
    redirectTo: 'assignments',
    pathMatch: 'full'
  },
  {
    path: 'assignments',
    loadComponent: () => import('./assignments/assignment-list/assignment-list').then(m => m.AssignmentList)
  },
  {
    path: 'assignments/:id',
    loadComponent: () => import('./assignments/assignment-detail/assignment-detail').then(m => m.AssignmentDetail)
  },
  {
    path: 'submissions',
    loadComponent: () => import('./assignments/my-submissions/my-submissions').then(m => m.MySubmissions)
  }
];