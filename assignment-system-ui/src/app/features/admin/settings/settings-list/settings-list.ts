import { Component,inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SettingsService } from '../../../../core/services/settings.service';
import { AppSettingDto } from '../../../../core/models';

@Component({
  selector: 'app-settings-list',
  imports: [
     CommonModule, ReactiveFormsModule,
    MatTableModule, MatButtonModule, MatIconModule,
    MatFormFieldModule, MatInputModule, MatCardModule, MatProgressSpinnerModule
  ],
  templateUrl: './settings-list.html',
  styleUrl: './settings-list.scss',
})
export class SettingsList implements OnInit{
  private fb = inject(FormBuilder);
  private settingService = inject(SettingsService);
  private snackBar = inject(MatSnackBar);

  settings = signal<AppSettingDto[]>([]);
  loading = signal(false);
  submitting = signal(false);
  editingKey = signal<string | null>(null);

  displayedColumns = ['key', 'value', 'description', 'actions'];

  createForm = this.fb.group({
    key: ['', Validators.required],
    value: ['', Validators.required],
    description: ['']
  });

  editForm = this.fb.group({
    value: ['', Validators.required],
    description: ['']
  });

  ngOnInit(): void {
    this.loadSettings();
  }

  loadSettings(): void {
    this.loading.set(true);
    this.settingService.getAll().subscribe({
      next: (data) => {
        this.settings.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Failed to load settings.', 'Dismiss', { duration: 3000 });
      }
    });
  }

  createSetting(): void {
    if (this.createForm.invalid) {
      this.createForm.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    const { key, value, description } = this.createForm.getRawValue();

    this.settingService.create({ key: key!, value: value!, description }).subscribe({
      next: () => {
        this.submitting.set(false);
        this.snackBar.open('Setting created.', 'Dismiss', { duration: 3000 });
        this.createForm.reset();
        this.loadSettings();
      },
      error: (err) => {
        this.submitting.set(false);
        const message = err?.error?.message || 'Failed to create setting.';
        this.snackBar.open(message, 'Dismiss', { duration: 4000 });
      }
    });
  }

  startEdit(setting: AppSettingDto): void {
    this.editingKey.set(setting.key);
    this.editForm.patchValue({ value: setting.value, description: setting.description });
  }

  cancelEdit(): void {
    this.editingKey.set(null);
  }

  saveEdit(key: string): void {
    if (this.editForm.invalid) return;

    const { value, description } = this.editForm.getRawValue();
    this.settingService.update(key, { value: value!, description }).subscribe({
      next: () => {
        this.snackBar.open('Setting updated.', 'Dismiss', { duration: 3000 });
        this.editingKey.set(null);
        this.loadSettings();
      },
      error: () => {
        this.snackBar.open('Failed to update setting.', 'Dismiss', { duration: 3000 });
      }
    });
  }

  deleteSetting(key: string): void {
    if (!confirm(`Delete setting "${key}"?`)) return;

    this.settingService.delete(key).subscribe({
      next: () => {
        this.snackBar.open('Setting deleted.', 'Dismiss', { duration: 3000 });
        this.loadSettings();
      },
      error: () => {
        this.snackBar.open('Failed to delete setting.', 'Dismiss', { duration: 3000 });
      }
    });
  }

}
