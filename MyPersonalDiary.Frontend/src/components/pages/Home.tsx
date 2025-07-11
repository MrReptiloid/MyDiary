// src/components/pages/Home/index.tsx
import { useEffect, useState } from "react";
import {
  Box,
  Typography,
  Container,
  Fab,
} from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import DiaryEntryService from "../../services/DiaryEntryService";
import { DiaryEntry, PaginatedData } from "../../types";
import { DiaryEntryGrid } from "../DiaryEntryGrid";
import { SearchFilterBar } from "../SearchFilterBar";
import { CreateEntryModal } from "../CreateEntryModal";
import { EditEntryModal } from "../EditEntryModal";

export const Home = () => {
  const [diaryEntries, setDiaryEntries] = useState<DiaryEntry[]>([]);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [editingEntry, setEditingEntry] = useState<DiaryEntry | null>(null);

  const [paginationData, setPaginationData] = useState<Omit<PaginatedData, "items">>({
    pageNumber: 1,
    totalCount: 0,
    totalPages: 0,
    hasPreviousPage: false,
    hasNextPage: false
  });
  const [searchTerm, setSearchTerm] = useState("");
  const [debouncedSearchTerm, setDebouncedSearchTerm] = useState("");
  const [startDate, setStartDate] = useState<Date | null>(null);
  const [endDate, setEndDate] = useState<Date | null>(null);

  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedSearchTerm(searchTerm);
    }, 500);
    return () => clearTimeout(timer);
  }, [searchTerm]);

  useEffect(() => {
    fetchEntries();
  }, [paginationData.pageNumber, debouncedSearchTerm, startDate, endDate]);

  const handleDeleteEntry = async (entryId: string) => {
    try {
      await DiaryEntryService.DeleteEntry(entryId);
      fetchEntries();
    } catch (error) {
      console.error("Error deleting diary entry:", error);
    }
  };

  const handleEditEntry = (entryId: string) => {
    const entry = diaryEntries.find(entry => entry.id === entryId);
    if (entry) {
      setEditingEntry(entry);
    }
  };

  const handleUpdateEntry = async (content: string, imageFile: File | null, removeImage: boolean) => {
    if (!editingEntry) return;

    try {
      const formData = new FormData();
      formData.append("content", content);

      if (imageFile) {
        formData.append("image", imageFile);
      }

      if (removeImage) {
        formData.append("removeImage", "true");
      }

      await DiaryEntryService.UpdateEntry(editingEntry.id, formData);
      fetchEntries();
      setEditingEntry(null);
    } catch (error) {
      console.error("Error updating diary entry:", error);
    }
  };

  const fetchEntries = async () => {
    try {
      setIsLoading(true);

      const params = {
        pageNumber: paginationData.pageNumber,
        searchTerm: debouncedSearchTerm || undefined,
        startDate: startDate ? startDate.toISOString() : undefined,
        endDate: endDate ? endDate.toISOString() : undefined
      };

      const response = await DiaryEntryService.GetEntries(params);

      if (response && response.data) {
        const entries = response.data.items.map(entry => ({
          ...entry,
          id: entry.id || `temp-${Date.now()}-${Math.random()}`,
          createdAt: entry.createdAt || new Date().toISOString(),
          imageUrl: entry.imagePath ? `${import.meta.env.VITE_API_URL || ''}/uploads/${entry.imagePath.split('/').pop()}` : null
        }));

        setDiaryEntries(entries);
        setPaginationData({
          pageNumber: response.data.pageNumber,
          totalCount: response.data.totalCount,
          totalPages: response.data.totalPages,
          hasPreviousPage: response.data.hasPreviousPage,
          hasNextPage: response.data.hasNextPage
        });
      }
    } catch (error) {
      console.error("Failed to fetch diary entries:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleCreateEntry = async (content: string, imageFile: File | null) => {
    try {
      const formData = new FormData();
      formData.append("content", content);
      if (imageFile) {
        formData.append("image", imageFile);
      }

      await DiaryEntryService.CreateEntry(formData);
      fetchEntries();
      setIsModalOpen(false);
    } catch (error) {
      console.error("Error creating diary entry:", error);
    }
  };

  const handlePageChange = (_: React.ChangeEvent<unknown>, page: number) => {
    setPaginationData(prev => ({
      ...prev,
      pageNumber: page
    }));
  };

  return (
    <Container maxWidth="lg">
      <Box sx={{ my: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom align="center" fontWeight="bold">
          My Personal Diary
        </Typography>
        <Typography variant="subtitle1" align="center" color="text.secondary" paragraph>
          Capture your thoughts, memories, and special moments
        </Typography>

        {/* Search and Filter Controls */}
        <Box sx={{ mb: 4 }}>
          <SearchFilterBar
            searchTerm={searchTerm}
            setSearchTerm={setSearchTerm}
            startDate={startDate}
            setStartDate={setStartDate}
            endDate={endDate}
            setEndDate={setEndDate}
          />
        </Box>

        <DiaryEntryGrid
          isLoading={isLoading}
          diaryEntries={diaryEntries}
          paginationData={paginationData}
          handlePageChange={handlePageChange}
          onDeleteEntry={handleDeleteEntry}
          onEdit={handleEditEntry}
        />

        <Fab
          color="primary"
          aria-label="add"
          sx={{ position: "fixed", bottom: 20, right: 20 }}
          onClick={() => setIsModalOpen(true)}
        >
          <AddIcon />
        </Fab>

        <EditEntryModal
          isOpen={editingEntry !== null}
          onClose={() => setEditingEntry(null)}
          onSubmit={handleUpdateEntry}
          entry={editingEntry}
        />

        <CreateEntryModal
          isOpen={isModalOpen}
          onClose={() => setIsModalOpen(false)}
          onSubmit={handleCreateEntry}
        />
      </Box>
    </Container>
  );
};