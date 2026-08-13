import { SubmissionStatus } from './enums';

export interface SubmissionDto {
  id: string;
  assignmentId: string;
  assignmentTitle: string;
  studentId: string;
  studentName: string;
  textAnswer: string | null;
  fileName: string | null;
  status: SubmissionStatus;
  marksObtained: number | null;
  assignmentMaxMarks: number;
  feedback: string | null;
  submittedAtUtc: string;
  updatedAtUtc: string;
  gradedAtUtc: string | null;
  gradedByTeacherName: string | null;
}

export interface GradeSubmissionRequest {
  marksObtained: number;
  feedback: string | null;
}

export interface ChangeSubmissionStatusRequest {
  status: SubmissionStatus;
}