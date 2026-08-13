import { Component,inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../../../core/services/auth.service';
import { UserRole } from '../../../../core/models';
interface MenuItem {
  label: string;
  icon: string;
  route: string;
}
@Component({
  selector: 'app-sidebar',
  imports: [CommonModule, RouterLink, RouterLinkActive, MatListModule, MatIconModule],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss',
})
export class SidebarComponent {
   authService = inject(AuthService);

  menuItems = computed<MenuItem[]>(() => {
  const role = this.authService.getRole();

  if (role === UserRole.Admin) {
    return [
      { label: 'Classes', icon: 'school', route: '/admin/classes' },
      { label: 'Subjects', icon: 'book', route: '/admin/subjects' },
      { label: 'Users', icon: 'people', route: '/admin/users' },
      { label: 'Teacher Assignments', icon: 'assignment_ind', route: '/admin/teacher-assignments' },
      { label: 'App Settings', icon: 'settings', route: '/admin/settings' },
    ];
  }
  if (role === UserRole.Teacher) {
    return [
      { label: 'My Assignments', icon: 'assignment', route: '/teacher/assignments' },
    ];
  }
  if (role === UserRole.Student) {
    return [
      { label: 'Assignments', icon: 'assignment', route: '/student/assignments' },
      { label: 'My Submissions', icon: 'upload_file', route: '/student/submissions' },
    ];
  }
  return [];
});
}
