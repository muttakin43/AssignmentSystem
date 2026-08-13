import { Component,inject, signal, OnInit  } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UserService } from '../../../../core/services/user.service';
import { UserDTO, UserRole } from '../../../../core/models';
@Component({
  selector: 'app-user-list',
  imports: [
     CommonModule, RouterLink, FormsModule,
    MatTableModule, MatButtonModule, MatIconModule, MatChipsModule,
    MatPaginatorModule, MatCardModule,MatFormFieldModule, MatInputModule, MatSelectModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './user-list.html',
  styleUrl: './user-list.scss',
})
export class UserList implements OnInit{
   private userService = inject(UserService);
  private snackBar = inject(MatSnackBar);

  users = signal<UserDTO[]>([]);
  totalCount = signal(0);
  loading = signal(false);

  page = 1;
  pageSize = 10;
  roleFilter: UserRole | '' = '';
  search = '';

  roles = Object.values(UserRole);
  displayedColumns = ['fullName', 'email', 'role', 'className', 'isActive', 'actions'];

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.loading.set(true);
    this.userService.getPaged({
      page: this.page,
      pageSize: this.pageSize,
      role: this.roleFilter || undefined,
      search: this.search || undefined
    }).subscribe({
      next: (result) => {
        this.users.set(result.items);
        this.totalCount.set(result.totalCount);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Failed to load users.', 'Dismiss', { duration: 3000 });
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.page = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadUsers();
  }

  onFilterChange(): void {
    this.page = 1;
    this.loadUsers();
  }

  toggleActive(user: UserDTO): void {
    const action = user.isActive ? 'deactivate' : 'activate';
    if (!confirm(`Are you sure you want to ${action} ${user.fullName}?`)) return;

    this.userService.setActive(user.id, !user.isActive).subscribe({
      next: () => {
        this.snackBar.open(`User ${action}d.`, 'Dismiss', { duration: 3000 });
        this.loadUsers();
      },
      error: () => {
        this.snackBar.open(`Failed to ${action} user.`, 'Dismiss', { duration: 3000 });
      }
    });
}
}
