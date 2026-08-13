import { Component,inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ClassService } from '../../../../core/services/class.service';

@Component({
  selector: 'app-class-form',
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
  templateUrl: './class-form.html',
  styleUrl: './class-form.scss',
})
export class ClassForm {
   private fb = inject(FormBuilder);
  private classService = inject(ClassService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  isEditMode = signal(false);
  loading = signal(false);
  classId: string | null = null;

  form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    description: [''],
    isActive: [true]
  });

  ngOnInit(): void {
    this.classId = this.route.snapshot.paramMap.get('id');

    if (this.classId) {
      this.isEditMode.set(true);
      this.loadClass(this.classId);
    }
  }

  loadClass(id: string): void {
    this.loading.set(true);
    this.classService.getById(id).subscribe({
      next: (cls) => {
        this.form.patchValue({
          name: cls.name,
          description: cls.description,
          isActive: cls.isActive
        });
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Failed to load class.', 'Dismiss', { duration: 3000 });
        this.router.navigate(['/admin/classes']);
      }
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    const { name, description, isActive } = this.form.getRawValue();

    if (this.isEditMode() && this.classId) {
      this.classService.update(this.classId, { name: name!, description, isActive: isActive! }).subscribe({
        next: () => {
          this.loading.set(false);
          this.snackBar.open('Class updated successfully.', 'Dismiss', { duration: 3000 });
          this.router.navigate(['/admin/classes']);
        },
        error: (err) => this.handleError(err)
      });
    } else {
      this.classService.create({ name: name!, description }).subscribe({
        next: () => {
          this.loading.set(false);
          this.snackBar.open('Class created successfully.', 'Dismiss', { duration: 3000 });
          this.router.navigate(['/admin/classes']);
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
    this.router.navigate(['/admin/classes']);
  }
}
