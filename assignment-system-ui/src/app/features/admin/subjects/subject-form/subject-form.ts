import { Component,inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SubjectService } from '../../../../core/services/subject.service';
@Component({
  selector: 'app-subject-form',
  imports: [
     CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatCheckboxModule
  ],
  templateUrl: './subject-form.html',
  styleUrl: './subject-form.scss',
})
export class SubjectForm implements OnInit{
  private fb = inject(FormBuilder);
  private subjectService = inject(SubjectService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  isEditMode = signal(false);
  loading = signal(false);
  subjectId: string | null = null;

  form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    code: ['', [Validators.maxLength(20)]],
    isActive: [true]
  });

  ngOnInit(): void {
    this.subjectId = this.route.snapshot.paramMap.get('id');

    if (this.subjectId) {
      this.isEditMode.set(true);
      this.loadSubject(this.subjectId);
    }
  }

  loadSubject(id: string): void {
    this.loading.set(true);
    this.subjectService.getById(id).subscribe({
      next: (subj) => {
        this.form.patchValue({
          name: subj.name,
          code: subj.code,
          isActive: subj.isActive
        });
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Failed to load subject.', 'Dismiss', { duration: 3000 });
        this.router.navigate(['/admin/subjects']);
      }
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    const { name, code, isActive } = this.form.getRawValue();

    if (this.isEditMode() && this.subjectId) {
      this.subjectService.update(this.subjectId, { name: name!, code, isActive: isActive! }).subscribe({
        next: () => {
          this.loading.set(false);
          this.snackBar.open('Subject updated successfully.', 'Dismiss', { duration: 3000 });
          this.router.navigate(['/admin/subjects']);
        },
        error: (err) => this.handleError(err)
      });
    } else {
      this.subjectService.create({ name: name!, code }).subscribe({
        next: () => {
          this.loading.set(false);
          this.snackBar.open('Subject created successfully.', 'Dismiss', { duration: 3000 });
          this.router.navigate(['/admin/subjects']);
        },
        error: (err) => this.handleError(err)
      });
    }
  }

  private handleError(err: any): void {
    this.loading.set(false);
    const message = err?.error?.message || 'Something went wrong. Please try again.';
    this.snackBar.open(message, 'Dismiss', { duration: 4000 });
  }

  cancel(): void {
    this.router.navigate(['/admin/subjects']);
  }
}
