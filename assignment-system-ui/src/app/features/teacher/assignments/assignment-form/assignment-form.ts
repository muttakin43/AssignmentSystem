import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AssignmentService } from '../../../../core/services/assignment.service';
import { TeacherAssignmentService } from '../../../../core/services/teacher-assignment.service';
import { AuthService } from '../../../../core/services/auth.service';

interface ClassSubjectOption {
  key: string;
  classId: string;
  className: string;
  subjectId: string;
  subjectName: string;
}

@Component({
  selector: 'app-assignment-form',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule,
    MatFormFieldModule, MatInputModule, MatButtonModule, MatCardModule,
    MatSelectModule, MatCheckboxModule, MatDatepickerModule, MatNativeDateModule
  ],
  templateUrl: './assignment-form.html',
  styleUrl: './assignment-form.scss'
})
export class AssignmentForm implements OnInit {
  private fb = inject(FormBuilder);
  private assignmentService = inject(AssignmentService);
  private teacherAssignmentService = inject(TeacherAssignmentService);
  private authService = inject(AuthService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  isEditMode = signal(false);
  loading = signal(false);
  assignmentId: string | null = null;
  classSubjectOptions = signal<ClassSubjectOption[]>([]);

  form = this.fb.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    description: [''],
    classSubjectKey: ['', Validators.required],
    deadline: [null as Date | null, Validators.required],
    maxMarks: [100, [Validators.required, Validators.min(1)]],
    allowUpdateAfterSubmit: [true]
  });

  ngOnInit(): void {
    this.loadMyClassSubjects();

    this.assignmentId = this.route.snapshot.paramMap.get('id');
    if (this.assignmentId) {
      this.isEditMode.set(true);
      this.form.get('classSubjectKey')?.disable();
      this.loadAssignment(this.assignmentId);
    }
  }

  loadMyClassSubjects(): void {
    this.teacherAssignmentService.getAll().subscribe(data => {
      const currentTeacherId = this.authService.currentUser()?.userId;
      const mine = data.filter(ta => ta.teacherId === currentTeacherId && ta.isActive);
      this.classSubjectOptions.set(
        mine.map(ta => ({
          key: `${ta.classId}|${ta.subjectId}`,
          classId: ta.classId,
          className: ta.className,
          subjectId: ta.subjectId,
          subjectName: ta.subjectName
        }))
      );
    });
  }
  selectAll(event: FocusEvent) {
  (event.target as HTMLInputElement).select();
}

  loadAssignment(id: string): void {
    this.loading.set(true);
    this.assignmentService.getById(id).subscribe({
      next: (a) => {
        this.form.patchValue({
          title: a.title,
          description: a.description,
          classSubjectKey: `${a.classId}|${a.subjectId}`,
          deadline: new Date(a.deadline),               // 👈 lowercase
          maxMarks: a.maxMarks,
          allowUpdateAfterSubmit: a.allowUpdateAfterSubmit   // 👈 lowercase
        });
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Failed to load assignment.', 'Dismiss', { duration: 3000 });
        this.router.navigate(['/teacher/assignments']);
      }
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    const raw = this.form.getRawValue();
    const deadlineIso = raw.deadline ? new Date(raw.deadline).toISOString() : '';
    const [classId, subjectId] = (raw.classSubjectKey ?? '').split('|');

    if (this.isEditMode() && this.assignmentId) {
      this.assignmentService.update(this.assignmentId, {
        title: raw.title!,
        description: raw.description ?? '',
        deadline: deadlineIso,                           // 👈 lowercase
        maxMarks: raw.maxMarks!,
        allowUpdateAfterSubmit: raw.allowUpdateAfterSubmit!   // 👈 lowercase
      }).subscribe({
        next: () => {
          this.loading.set(false);
          this.snackBar.open('Assignment updated.', 'Dismiss', { duration: 3000 });
          this.router.navigate(['/teacher/assignments']);
        },
        error: (err) => this.handleError(err)
      });
    } else {
      this.assignmentService.create({
        title: raw.title!,
        description: raw.description ?? '',
        classId: classId,
        subjectId: subjectId,
        deadline: deadlineIso,                           // 👈 lowercase
        maxMarks: raw.maxMarks!,
        allowUpdateAfterSubmit: raw.allowUpdateAfterSubmit!   // 👈 lowercase
      }).subscribe({
        next: () => {
          this.loading.set(false);
          this.snackBar.open('Assignment created as draft.', 'Dismiss', { duration: 3000 });
          this.router.navigate(['/teacher/assignments']);
        },
        error: (err) => this.handleError(err)
      });
    }
  }

  private handleError(err: any): void {
    this.loading.set(false);
    const message = err?.error?.message || 'Something went wrong.';
    this.snackBar.open(message, 'Dismiss', { duration: 4000 });
  }

  cancel(): void {
    this.router.navigate(['/teacher/assignments']);
  }
}