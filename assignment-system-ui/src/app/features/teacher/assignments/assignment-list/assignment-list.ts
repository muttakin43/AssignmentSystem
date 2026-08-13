import { Component , inject, signal, OnInit} from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AssignmentService } from '../../../../core/services/assignment.service';
import { AssignmentDto, AssignmentStatus } from '../../../../core/models';
@Component({
  selector: 'app-assignment-list',
  imports: [
    CommonModule, RouterLink, MatTableModule, MatButtonModule,
    MatIconModule,MatCardModule, MatChipsModule, MatProgressSpinnerModule
  ],
  templateUrl: './assignment-list.html',
  styleUrl: './assignment-list.scss',
})
export class AssignmentList implements OnInit {
  private assignmentService = inject(AssignmentService);
  private snackBar = inject(MatSnackBar);

  assignments = signal<AssignmentDto[]>([]);
  loading = signal(false);
  displayedColumns = ['title', 'className', 'subjectName', 'deadline', 'status', 'submissionCount', 'actions'];
  AssignmentStatus = AssignmentStatus;

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

  publish(id: string): void {
    this.assignmentService.publish(id).subscribe({
      next: () => {
        this.snackBar.open('Assignment published.', 'Dismiss', { duration: 3000 });
        this.loadAssignments();
      },
      error: (err) => this.showError(err, 'Failed to publish.')
    });
  }

  close(id: string): void {
    if (!confirm('Close this assignment? Students will no longer be able to submit.')) return;
    this.assignmentService.close(id).subscribe({
      next: () => {
        this.snackBar.open('Assignment closed.', 'Dismiss', { duration: 3000 });
        this.loadAssignments();
      },
      error: (err) => this.showError(err, 'Failed to close.')
    });
  }

  deleteAssignment(id: string, title: string): void {
    if (!confirm(`Delete "${title}"? This cannot be undone.`)) return;
    this.assignmentService.delete(id).subscribe({
      next: () => {
        this.snackBar.open('Assignment deleted.', 'Dismiss', { duration: 3000 });
        this.loadAssignments();
      },
      error: (err) => this.showError(err, 'Failed to delete.')
    });
  }

  private showError(err: any, fallback: string): void {
    const message = err?.error?.message || fallback;
    this.snackBar.open(message, 'Dismiss', { duration: 4000 });
  }

  statusColor(status: AssignmentStatus): string {
    switch (status) {
      case AssignmentStatus.Draft: return '';
      case AssignmentStatus.Published: return 'primary';
      case AssignmentStatus.Closed: return 'warn';
      default: return '';
    }
  }
}
