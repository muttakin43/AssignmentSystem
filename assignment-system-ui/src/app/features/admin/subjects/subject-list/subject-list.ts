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
import { SubjectService } from '../../../../core/services/subject.service';
import { Subject } from '../../../../core/models';

@Component({
  selector: 'app-subject-list',
  imports: [
      CommonModule,
    RouterLink,
    MatTableModule,
    MatButtonModule,
    MatIconModule,MatCardModule,
    MatChipsModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './subject-list.html',
  styleUrl: './subject-list.scss',
})
export class SubjectList implements OnInit {
   private subjectService = inject(SubjectService);
  private snackBar = inject(MatSnackBar);

  subjects = signal<Subject[]>([]);
  loading = signal(false);
  displayedColumns = ['name', 'code', 'isActive', 'actions'];

  ngOnInit(): void {
    this.loadSubjects();
  }

  loadSubjects(): void {
    this.loading.set(true);
    this.subjectService.getAll().subscribe({
      next: (data) => {
        this.subjects.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Failed to load subjects.', 'Dismiss', { duration: 3000 });
      }
    });
  }

  deactivate(id: string, name: string): void {
    if (!confirm(`Deactivate subject "${name}"?`)) return;

    this.subjectService.deactivate(id).subscribe({
      next: () => {
        this.snackBar.open('Subject deactivated.', 'Dismiss', { duration: 3000 });
        this.loadSubjects();
      },
      error: () => {
        this.snackBar.open('Failed to deactivate subject.', 'Dismiss', { duration: 3000 });
      }
    });
}
}
