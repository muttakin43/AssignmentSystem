export interface ClassRoom {
  id: string;
  name: string;
  description: string | null;
  isActive: boolean;
  studentCount: number;
}

export interface ClassSubject {
  subjectId: string;
  subjectName: string;
  subjectCode: string;
}

export interface ClassDetail extends ClassRoom {
  subjects: ClassSubject[];
}

export interface CreateClassRequest {
  name: string;
  description: string | null;
}

export interface UpdateClassRequest {
  name: string;
  description: string | null;
  isActive: boolean;
}