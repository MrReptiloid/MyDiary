export interface DiaryEntry {
  id: string;
  content: string;
  imagePath?: string;
  createdAt: string;
  imageUrl?: string | null;
}

export interface PaginatedData {
  items: DiaryEntry[];
  pageNumber: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}