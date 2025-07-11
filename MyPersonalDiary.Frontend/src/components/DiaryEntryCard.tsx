import { Card, CardContent, CardMedia, Typography, IconButton, Dialog, DialogActions, DialogContent, DialogTitle, Button, Box } from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import { useState } from "react";
import { DiaryEntry } from "../types";

interface DiaryEntryCardProps {
  entry: DiaryEntry;
  onDelete: (entryId: string) => Promise<void>;
  onEdit: (entryId: string) => void;
}

export const DiaryEntryCard = ({ entry, onDelete, onEdit }: DiaryEntryCardProps) => {
  const [confirmOpen, setConfirmOpen] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  };

  const handleDelete = async () => {
    try {
      setIsDeleting(true);
      await onDelete(entry.id);
    } finally {
      setIsDeleting(false);
      setConfirmOpen(false);
    }
  };

  return (
    <>
      <Card sx={{ height: "100%", display: "flex", flexDirection: "column", position: "relative" }}>
        {entry.imageUrl && (
          <CardMedia
            component="img"
            height="180"
            image={`https://${import.meta.env.VITE_REACT_APP_SERVERDOMAIN || ''}${entry.imageUrl}`}
            alt="Diary entry image"
          />
        )}
        <CardContent sx={{ flexGrow: 1 }}>
          <Typography variant="body2" color="text.secondary" gutterBottom>
            {formatDate(entry.createdAt)}
          </Typography>
          <Typography variant="body1">
            {entry.content.length > 150
              ? `${entry.content.substring(0, 150)}...`
              : entry.content}
          </Typography>
        </CardContent>

        <Box sx={{ position: "absolute", top: 8, right: 8, display: "flex", gap: 1 }}>
          <IconButton
            aria-label="edit"
            onClick={() => onEdit(entry.id)}
            sx={{
              bgcolor: "rgba(255,255,255,0.7)",
              "&:hover": { bgcolor: "rgba(0,0,255,0.1)" }
            }}
          >
            <EditIcon />
          </IconButton>
          <IconButton
            aria-label="delete"
            onClick={() => setConfirmOpen(true)}
            sx={{
              bgcolor: "rgba(255,255,255,0.7)",
              "&:hover": { bgcolor: "rgba(255,0,0,0.1)" }
            }}
          >
            <DeleteIcon />
          </IconButton>
        </Box>
      </Card>

      <Dialog open={confirmOpen} onClose={() => setConfirmOpen(false)}>
        <DialogTitle>Delete Diary Entry</DialogTitle>
        <DialogContent>
          Are you sure you want to delete this diary entry? This action cannot be undone.
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setConfirmOpen(false)} disabled={isDeleting}>Cancel</Button>
          <Button onClick={handleDelete} color="error" disabled={isDeleting}>
            {isDeleting ? "Deleting..." : "Delete"}
          </Button>
        </DialogActions>
      </Dialog>
    </>
  );
};