import { Component,inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UserService } from '../../../../core/services/user.service';
import { ClassService } from '../../../../core/services/class.service';
import { UserRole, ClassRoom } from '../../../../core/models';
@Component({
  selector: 'app-user-form',
  imports: [
     CommonModule, ReactiveFormsModule,
    MatFormFieldModule,MatIconModule, MatInputModule, MatButtonModule, MatCardModule, MatSelectModule
  ],
  templateUrl: './user-form.html',
  styleUrl: './user-form.scss',
})
export class UserForm implements OnInit {
  private fb = inject(FormBuilder);
  private userService = inject(UserService);
  private classService = inject(ClassService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  isEditMode = signal(false);
  loading = signal(false);
  userId: string | null = null;
  classes = signal<ClassRoom[]>([]);
  roles = Object.values(UserRole);

  form = this.fb.group({
    fullName: ['', [Validators.required, Validators.maxLength(150)]],
    email: ['', [Validators.required, Validators.email]],
    password: [''],
    role: [UserRole.Student, [Validators.required]],
    classId: ['' as string | null]
  });

  get isStudent(): boolean {
    return this.form.get('role')?.value === UserRole.Student;
  }

  ngOnInit(): void {
    this.loadClasses();

    this.userId = this.route.snapshot.paramMap.get('id');
    if (this.userId) {
      this.isEditMode.set(true);
      this.form.get('password')?.clearValidators();
      this.loadUser(this.userId);
    } else {
      this.form.get('password')?.setValidators([Validators.required, Validators.minLength(6)]);
    }
    this.form.get('password')?.updateValueAndValidity();
  }

  loadClasses(): void {
    this.classService.getAll().subscribe(data => this.classes.set(data));
  }

  loadUser(id: string): void {
    this.loading.set(true);
    this.userService.getById(id).subscribe({
      next: (user) => {
        this.form.patchValue({
          fullName: user.fullName,
          email: user.email,
          role: user.role,
          classId: user.classId
        });
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Failed to load user.', 'Dismiss', { duration: 3000 });
        this.router.navigate(['/admin/users']);
      }
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    const { fullName, email, password, role, classId } = this.form.getRawValue();
    const finalClassId = role === UserRole.Student ? classId : null;

    if (this.isEditMode() && this.userId) {
      this.userService.update(this.userId, {
        fullName: fullName!, email: email!, role: role!, classId: finalClassId
      }).subscribe({
        next: () => {
          this.loading.set(false);
          this.snackBar.open('User updated successfully.', 'Dismiss', { duration: 3000 });
          this.router.navigate(['/admin/users']);
        },
        error: (err) => this.handleError(err)
      });
    } else {
      this.userService.create({
        fullName: fullName!, email: email!, password: password!, role: role!, classId: finalClassId
      }).subscribe({
        next: () => {
          this.loading.set(false);
          this.snackBar.open('User created successfully.', 'Dismiss', { duration: 3000 });
          this.router.navigate(['/admin/users']);
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
    this.router.navigate(['/admin/users']);
  }
}
