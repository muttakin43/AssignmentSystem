import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { SubmissionService } from '../../../../core/services/submission.service';
import { SubmissionDto, SubmissionStatus } from '../../../../core/models';

@Component({
  selector: 'app-my-submissions',
  standalone: true,
  imports: [CommonModule,MatCardModule, RouterLink, MatTableModule, MatChipsModule, MatButtonModule, MatProgressSpinnerModule],
  templateUrl: './my-submissions.html',
  styleUrl: './my-submissions.scss'
})
export class MySubmissions implements OnInit {
  private submissionService = inject(SubmissionService);

  submissions = signal<SubmissionDto[]>([]);
  loading = signal(false);
  displayedColumns = ['assignmentTitle', 'submittedAtUtc', 'status', 'marksObtained'];
  SubmissionStatus = SubmissionStatus;

  ngOnInit(): void {
    this.loading.set(true);
    this.submissionService.getMine().subscribe({
      next: (data) => {
        this.submissions.set(data);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }
}