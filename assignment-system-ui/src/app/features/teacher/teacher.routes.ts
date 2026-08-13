import { Routes } from '@angular/router';

export const TEACHER_ROUTES: Routes = [
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
    path: 'assignments/new',
    loadComponent: () => import('./assignments/assignment-form/assignment-form').then(m => m.AssignmentForm)
  },
  {
    path: 'assignments/:id/edit',
    loadComponent: () => import('./assignments/assignment-form/assignment-form').then(m => m.AssignmentForm)
  },
 {
  path: 'assignments/:id/submissions',
  loadComponent: () => import('./submissions/submission-review/submission-review').then(m => m.SubmissionReview)
}
];