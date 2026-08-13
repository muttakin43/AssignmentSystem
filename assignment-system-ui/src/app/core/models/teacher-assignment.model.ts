export interface TeacherAssignment {
  id: string;
  teacherId: string;
  teacherName: string;
  classId: string;
  className: string;
  subjectId: string;
  subjectName: string;
  isActive: boolean;
}

export interface CreateTeacherAssignmentRequest {
  teacherId: string;
  classId: string;
  subjectId: string;
}