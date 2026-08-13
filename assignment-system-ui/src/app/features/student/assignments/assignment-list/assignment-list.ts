import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AssignmentService } from '../../../../core/services/assignment.service';
import { AssignmentDto } from '../../../../core/models';

@Component({
  selector: 'app-student-assignment-list',
  standalone: true,
  imports: [CommonModule,MatCardModule, RouterLink, MatTableModule, MatButtonModule, MatChipsModule, MatProgressSpinnerModule],
  templateUrl: './assignment-list.html',
  styleUrl: './assignment-list.scss'
})
export class AssignmentList implements OnInit {
  private assignmentService = inject(AssignmentService);
  private snackBar = inject(MatSnackBar);

  assignments = signal<AssignmentDto[]>([]);
  loading = signal(false);
  displayedColumns = ['title', 'subjectName', 'deadline', 'maxMarks', 'actions'];

  ngOnInit(): void {
    this.loadAssignments();
  }

  loadAssignments(): void {
    this.loading.set(true);
    this.assignmentService.getPaged({ page: 1, pageSize: 50 }).subscribe({
      next: (result) => {
        this.assignments.set(result.items);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Failed to load assignments.', 'Dismiss', { duration: 3000 });
      }
    });
  }

  isOverdue(deadline: string): boolean {
    return new Date(deadline) < new Date();
  }
}