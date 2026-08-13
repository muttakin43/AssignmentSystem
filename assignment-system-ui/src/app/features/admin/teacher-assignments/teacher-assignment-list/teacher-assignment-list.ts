import { Component,inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { TeacherAssignmentService } from '../../../../core/services/teacher-assignment.service';
import { UserService } from '../../../../core/services/user.service';
import { ClassService } from '../../../../core/services/class.service';
import { SubjectService } from '../../../../core/services/subject.service';
import { TeacherAssignment, UserDTO, ClassRoom, Subject, UserRole } from '../../../../core/models';

@Component({
  selector: 'app-teacher-assignment-list',
  imports: [
    CommonModule, ReactiveFormsModule,
    MatTableModule, MatButtonModule, MatIconModule, MatChipsModule,
    MatFormFieldModule,MatTooltipModule, MatSelectModule, MatCardModule, MatProgressSpinnerModule
  ],
  templateUrl: './teacher-assignment-list.html',
  styleUrl: './teacher-assignment-list.scss',
})
export class TeacherAssignmentList implements OnInit {
  private fb = inject(FormBuilder);
  private taService = inject(TeacherAssignmentService);
  private userService = inject(UserService);
  private classService = inject(ClassService);
  private subjectService = inject(SubjectService);
  private snackBar = inject(MatSnackBar);

  assignments = signal<TeacherAssignment[]>([]);
  teachers = signal<UserDTO[]>([]);
  classes = signal<ClassRoom[]>([]);
  subjects = signal<Subject[]>([]);
  loading = signal(false);
  submitting = signal(false);

  displayedColumns = ['teacherName', 'className', 'subjectName', 'isActive', 'actions'];

  form = this.fb.group({
    teacherId: ['', Validators.required],
    classId: ['', Validators.required],
    subjectId: ['', Validators.required]
  });

  ngOnInit(): void {
    this.loadAll();
  }

  loadAll(): void {
    this.loading.set(true);
    this.taService.getAll().subscribe({
      next: (data) => {
        this.assignments.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Failed to load teacher assignments.', 'Dismiss', { duration: 3000 });
      }
    });

    this.userService.getPaged({ role: UserRole.Teacher, page: 1, pageSize: 100 }).subscribe(
      result => this.teachers.set(result.items)
    );
    this.classService.getAll().subscribe(data => this.classes.set(data));
    this.subjectService.getAll().subscribe(data => this.subjects.set(data));
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    const { teacherId, classId, subjectId } = this.form.getRawValue();

    this.taService.create({ teacherId: teacherId!, classId: classId!, subjectId: subjectId! }).subscribe({
      next: () => {
        this.submitting.set(false);
        this.snackBar.open('Teacher assigned successfully.', 'Dismiss', { duration: 3000 });
        this.form.reset();
        this.loadAll();
      },
      error: (err) => {
        this.submitting.set(false);
        const message = err?.error?.message || 'Failed to assign teacher.';
        this.snackBar.open(message, 'Dismiss', { duration: 4000 });
      }
    });
  }

  toggleActive(assignment: TeacherAssignment): void {
    const action = assignment.isActive ? 'deactivate' : 'activate';
    if (!confirm(`Are you sure you want to ${action} this assignment?`)) return;

    this.taService.setActive(assignment.id, !assignment.isActive).subscribe({
      next: () => {
        this.snackBar.open(`Assignment ${action}d.`, 'Dismiss', { duration: 3000 });
        this.loadAll();
      },
      error: (err) => {
        const message = err?.error?.message || `Failed to ${action} assignment.`;
        this.snackBar.open(message, 'Dismiss', { duration: 4000 });
      }
    });
  }
}
