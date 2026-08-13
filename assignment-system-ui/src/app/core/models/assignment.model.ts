import { AssignmentStatus } from './enums';
import { PageQuery } from './paged-result.model';

export interface AssignmentDto {
  id: string;
  title: string;
  description: string;
  classId: string;
  className: string;
  subjectId: string;
  subjectName: string;
  teacherId: string;
  teacherName: string;
  maxMarks: number;
  deadline: string;
  allowUpdateAfterSubmit: boolean;
  status: AssignmentStatus;
  submissionCount: number;
  createdAtUtc: string;
}

export interface AssignmentQuery extends PageQuery {
  classId?: string;
  subjectId?: string;
  status?: AssignmentStatus;
  search?: string;
}

export interface CreateAssignmentRequest {
  title: string;
  description: string;
  classId: string;
  subjectId: string;
  maxMarks: number;
  deadline: string;
  allowUpdateAfterSubmit: boolean;
}

export interface UpdateAssignmentRequest {
  title: string;
  description: string;
  maxMarks: number;
  deadline: string;
  allowUpdateAfterSubmit: boolean;
}