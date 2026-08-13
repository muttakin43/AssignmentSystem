import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { SubmissionDto } from '../../../../core/models';

@Component({
  selector: 'app-grade-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './grade-dialog.html',
  styleUrl: './grade-dialog.scss'
})
export class GradeDialog {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<GradeDialog>);
  data = inject<{ submission: SubmissionDto; maxMarks: number }>(MAT_DIALOG_DATA);

  form = this.fb.group({
    marksObtained: [
      this.data.submission.marksObtained ?? 0,
      [Validators.required, Validators.min(0), Validators.max(this.data.maxMarks)]
    ],
    feedback: [this.data.submission.feedback ?? '']
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.dialogRef.close(this.form.getRawValue());
  }

  cancel(): void {
    this.dialogRef.close();
  }
}