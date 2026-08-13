import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AssignmentService } from '../../../../core/services/assignment.service';
import { SubmissionService } from '../../../../core/services/submission.service';
import { AssignmentDto, SubmissionDto, SubmissionStatus } from '../../../../core/models';

@Component({
  selector: 'app-assignment-detail',
  standalone: true,
  imports: [
    CommonModule, RouterLink, ReactiveFormsModule,
    MatCardModule,MatIconModule, MatFormFieldModule, MatInputModule, MatButtonModule,
    MatChipsModule, MatProgressSpinnerModule
  ],
  templateUrl: './assignment-detail.html',
  styleUrl: './assignment-detail.scss'
})
export class AssignmentDetail implements OnInit {
  private fb = inject(FormBuilder);
  private assignmentService = inject(AssignmentService);
  private submissionService = inject(SubmissionService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  assignmentId = '';
  assignment = signal<AssignmentDto | null>(null);
  mySubmission = signal<SubmissionDto | null>(null);
  loading = signal(false);
  submitting = signal(false);
  selectedFile: File | null = null;
  SubmissionStatus = SubmissionStatus;

  form = this.fb.group({
    textAnswer: ['']
  });

  get isDeadlinePassed(): boolean {
    const a = this.assignment();
    return a ? new Date(a.deadline) < new Date() : false;
  }

  get canEdit(): boolean {
    const sub = this.mySubmission();
    const a = this.assignment();
    if (!a) return false;
    if (!sub) return !this.isDeadlinePassed;
    if (sub.status === SubmissionStatus.Graded) return false;
    return a.allowUpdateAfterSubmit && !this.isDeadlinePassed;
  }

  ngOnInit(): void {
    this.assignmentId = this.route.snapshot.paramMap.get('id')!;
    this.loadData();
  }

  loadData(): void {
    this.loading.set(true);
    this.assignmentService.getById(this.assignmentId).subscribe({
      next: (a) => {
        this.assignment.set(a);
        this.checkExistingSubmission();
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Assignment not found.', 'Dismiss', { duration: 3000 });
        this.router.navigate(['/student/assignments']);
      }
    });
  }

  checkExistingSubmission(): void {
    this.submissionService.getMine().subscribe({
      next: (submissions) => {
        const existing = submissions.find(s => s.assignmentId === this.assignmentId);
        if (existing) {
          this.mySubmission.set(existing);
          this.form.patchValue({ textAnswer: existing.textAnswer });
        }
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedFile = input.files?.[0] ?? null;
  }

  submit(): void {
    const textAnswer = this.form.value.textAnswer ?? null;

    if (!textAnswer && !this.selectedFile && !this.mySubmission()?.fileName) {
      this.snackBar.open('Please provide a text answer, a file, or both.', 'Dismiss', { duration: 3000 });
      return;
    }

    this.submitting.set(true);
    const existing = this.mySubmission();

    if (existing) {
      this.submissionService.update(existing.id, textAnswer, this.selectedFile).subscribe({
        next: () => {
          this.submitting.set(false);
          this.snackBar.open('Submission updated.', 'Dismiss', { duration: 3000 });
          this.checkExistingSubmission();
        },
        error: (err) => this.handleError(err)
      });
    } else {
      this.submissionService.create(this.assignmentId, textAnswer, this.selectedFile).subscribe({
        next: () => {
          this.submitting.set(false);
          this.snackBar.open('Submitted successfully.', 'Dismiss', { duration: 3000 });
          this.checkExistingSubmission();
        },
        error: (err) => this.handleError(err)
      });
    }
  }

  viewFile(): void {
  const sub = this.mySubmission();

  if (!sub) return;

  this.submissionService.downloadFile(sub.id).subscribe({
    next: (blob) => {
      const url = window.URL.createObjectURL(blob);

      const link = document.createElement('a');
      link.href = url;
      link.download = sub.fileName ?? 'submission-file';

      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);

      window.URL.revokeObjectURL(url);
    },
    error: () => {
      this.snackBar.open(
        'Failed to download file.',
        'Dismiss',
        { duration: 3000 }
      );
    }
  });
}

  private handleError(err: any): void {
    this.submitting.set(false);
    const message = err?.error?.message || 'Failed to submit.';
    this.snackBar.open(message, 'Dismiss', { duration: 4000 });
  }
}