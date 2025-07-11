import api from "../http";

export interface DiaryEntryData {
  id: string;
  content: string;
  imagePath?: string;
  createdAt: string;
  imageUrl?: string | null;
}
interface PaginationParams {
  pageNumber?: number;
  searchTerm?: string;
  startDate?: string;
  endDate?: string;
}

interface PaginatedResponse<T> {
  items: T[];
  pageNumber: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export default class DiaryEntryService {
  static async CreateEntry(formData: FormData){
    return api.post("/diaryEntry", formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      }
    );
  }

  static async GetEntries(params: PaginationParams = {}) {
    try {
      const { pageNumber = 1, searchTerm, startDate, endDate } = params;


      const queryParams = new URLSearchParams()
      queryParams.append("PageNumber", String(pageNumber));

      if(searchTerm) {
        queryParams.append("searchTerm", searchTerm);
      }
      if(startDate) {
        queryParams.append("StartDate", startDate);
      }
      if(endDate) {
        queryParams.append("EndDate", endDate);
      }

      return await api.get<PaginatedResponse<DiaryEntryData>>(`/diaryEntry?${queryParams.toString()}`)
    } catch(e: any) {
      console.log('Error fetching diary entries:', e);
      throw e;
    }
  }

  static async DeleteEntry(entryId: string) {
    return api.delete(`/diaryEntry/${entryId}`);
  }

  static async UpdateEntry(entryId: string, formData: FormData) {
    return api.put(`/diaryEntry/${entryId}`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
  }
}