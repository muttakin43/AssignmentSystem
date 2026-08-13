import { Component,inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ClassService } from '../../../../core/services/class.service';
import { ClassRoom } from '../../../../core/models';
@Component({
  selector: 'app-class-list',
  imports: [
     CommonModule,
    RouterLink,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatChipsModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './class-list.html',
  styleUrl: './class-list.scss',
})
export class ClassList {
   private classService = inject(ClassService);
  private snackBar = inject(MatSnackBar);

  classes = signal<ClassRoom[]>([]);
  loading = signal(false);
  displayedColumns = ['name', 'description', 'studentCount', 'isActive', 'actions'];

  ngOnInit(): void {
    this.loadClasses();
  }

  loadClasses(): void {
    this.loading.set(true);
    this.classService.getAll().subscribe({
      next: (data) => {
        this.classes.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Failed to load classes.', 'Dismiss', { duration: 3000 });
      }
    });
  }

  deactivate(id: string, name: string): void {
    if (!confirm(`Deactivate class "${name}"?`)) return;

    this.classService.deactivate(id).subscribe({
      next: () => {
        this.snackBar.open('Class deactivated.', 'Dismiss', { duration: 3000 });
        this.loadClasses();
      },
      error: () => {
        this.snackBar.open('Failed to deactivate class.', 'Dismiss', { duration: 3000 });
      }
    });
  }
}
