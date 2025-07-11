import { Box, CircularProgress, Grid, Pagination, Typography } from "@mui/material";
import { DiaryEntry, PaginatedData } from "../types";
import { DiaryEntryCard } from "./DiaryEntryCard";

interface DiaryEntryGridProps {
  isLoading: boolean;
  diaryEntries: DiaryEntry[];
  paginationData: Omit<PaginatedData, "items">;
  handlePageChange: (event: React.ChangeEvent<unknown>, page: number) => void;
  onDeleteEntry: (entryId: string) => Promise<void>;
  onEdit: (entryId: string) => void; // Add this prop
}

export const DiaryEntryGrid = ({
                                 isLoading,
                                 diaryEntries,
                                 paginationData,
                                 handlePageChange,
                                 onDeleteEntry,
                                 onEdit // Include this prop
                               }: DiaryEntryGridProps) => {
  if (isLoading) {
    return (
      <Box sx={{ display: "flex", justifyContent: "center", mt: 4 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (diaryEntries.length === 0) {
    return (
      <Box sx={{ textAlign: "center", mt: 4 }}>
        <Typography variant="h6" color="text.secondary">
          No entries found. {paginationData.totalCount === 0 ? "Create your first diary entry!" : "Try adjusting your filters."}
        </Typography>
      </Box>
    );
  }

  return (
    <>
      <Grid container spacing={3} sx={{ mt: 2 }}>
        {diaryEntries.map((entry) => (
          <Grid item xs={12} sm={6} md={4} key={entry.id}>
            <DiaryEntryCard
              entry={entry}
              onDelete={onDeleteEntry}
              onEdit={onEdit} // Pass the onEdit prop here
            />
          </Grid>
        ))}
      </Grid>

      {paginationData.totalPages > 1 && (
        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
          <Pagination
            count={paginationData.totalPages}
            page={paginationData.pageNumber}
            onChange={handlePageChange}
            color="primary"
          />
        </Box>
      )}
    </>
  );
};