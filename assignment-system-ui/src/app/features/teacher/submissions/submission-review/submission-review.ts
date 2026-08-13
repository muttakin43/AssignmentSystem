import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card';
import { SubmissionService } from '../../../../core/services/submission.service';
import { AssignmentService } from '../../../../core/services/assignment.service';
import { SubmissionDto, AssignmentDto, SubmissionStatus } from '../../../../core/models';
import { GradeDialog } from '../grade-dialog/grade-dialog';

@Component({
  selector: 'app-submission-review',
  standalone: true,
  imports: [
    CommonModule, RouterLink, MatTableModule, MatButtonModule, MatIconModule,
    MatChipsModule,MatCardModule, MatFormFieldModule, MatInputModule, MatDialogModule, MatProgressSpinnerModule
  ],
  templateUrl: './submission-review.html',
  styleUrl: './submission-review.scss'
})
export class SubmissionReview implements OnInit {
  private submissionService = inject(SubmissionService);
  private assignmentService = inject(AssignmentService);
  private route = inject(ActivatedRoute);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);

  assignmentId = '';
  assignment = signal<AssignmentDto | null>(null);
  submissions = signal<SubmissionDto[]>([]);
  loading = signal(false);
  SubmissionStatus = SubmissionStatus;

  displayedColumns = ['studentName', 'submittedAtUtc', 'status', 'marksObtained', 'actions'];

  ngOnInit(): void {
    this.assignmentId = this.route.snapshot.paramMap.get('id')!;
    this.loadAssignment();
    this.loadSubmissions();
  }

  loadAssignment(): void {
    this.assignmentService.getById(this.assignmentId).subscribe(a => this.assignment.set(a));
  }

  loadSubmissions(): void {
    this.loading.set(true);
    this.submissionService.getForAssignment(this.assignmentId).subscribe({
      next: (data) => {
        this.submissions.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Failed to load submissions.', 'Dismiss', { duration: 3000 });
      }
    });
  }
  

 viewFile(submissionId: string): void {
  this.submissionService.downloadFile(submissionId).subscribe({
    next: (blob) => {
      const url = window.URL.createObjectURL(blob);

      const link = document.createElement('a');
      link.href = url;
      link.download = 'submission-file';

      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);

      window.URL.revokeObjectURL(url);
    },
    error: (err) => {
      console.error('File download error:', err);

      this.snackBar.open(
        'Failed to download file.',
        'Dismiss',
        { duration: 3000 }
      );
    }
  });
}

  openGradeDialog(submission: SubmissionDto): void {
    const dialogRef = this.dialog.open(GradeDialog, {
      width: '450px',
      data: { submission, maxMarks: this.assignment()?.maxMarks ?? 100 }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.submissionService.grade(submission.id, result).subscribe({
          next: () => {
            this.snackBar.open('Submission graded.', 'Dismiss', { duration: 3000 });
            this.loadSubmissions();
          },
          error: (err) => {
            const message = err?.error?.message || 'Failed to grade submission.';
            this.snackBar.open(message, 'Dismiss', { duration: 4000 });
          }
        });
      }
    });
  }
}