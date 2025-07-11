import { Box, IconButton, Modal, Typography } from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";
import { DiaryEntry } from "../types";
import { DiaryEntryForm } from "./DiaryEntryForm";

interface EditEntryModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (content: string, imageFile: File | null, removeImage: boolean) => Promise<void>;
  entry: DiaryEntry | null;
}

export const EditEntryModal = ({ isOpen, onClose, onSubmit, entry }: EditEntryModalProps) => {
  if (!entry) return null;

  return (
    <Modal
      open={isOpen}
      onClose={onClose}
      aria-labelledby="edit-diary-modal"
    >
      <Box
        sx={{
          position: "absolute",
          top: "50%",
          left: "50%",
          transform: "translate(-50%, -50%)",
          width: { xs: "90%", sm: "70%", md: "50%" },
          bgcolor: "background.paper",
          boxShadow: 24,
          p: 4,
          borderRadius: 2,
          maxHeight: "90vh",
          overflowY: "auto"
        }}
      >
        <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 2 }}>
          <Typography variant="h6" id="edit-diary-modal">
            Edit Diary Entry
          </Typography>
          <IconButton onClick={onClose}>
            <CloseIcon />
          </IconButton>
        </Box>

        <DiaryEntryForm
          initialContent={entry.content}
          initialImage={entry.imageUrl}
          onSubmit={onSubmit}
          onCancel={onClose}
          submitLabel="Update Entry"
        />

      </Box>
    </Modal>
  );
};