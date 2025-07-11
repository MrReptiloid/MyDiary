import { Box, IconButton, Modal, Typography } from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";
import { DiaryEntryForm } from "./DiaryEntryForm";

interface CreateEntryModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (content: string, imageFile: File | null) => Promise<void>;
}

export const CreateEntryModal = ({ isOpen, onClose, onSubmit }: CreateEntryModalProps) => {
  const handleSubmit = async (content: string, imageFile: File | null) => {
    await onSubmit(content, imageFile);
  };

  return (
    <Modal
      open={isOpen}
      onClose={onClose}
      aria-labelledby="create-diary-modal"
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
          <Typography variant="h6" id="create-diary-modal">
            Create New Diary Entry
          </Typography>
          <IconButton onClick={onClose}>
            <CloseIcon />
          </IconButton>
        </Box>

        <DiaryEntryForm
          onSubmit={(content, imageFile) => handleSubmit(content, imageFile)}
          onCancel={onClose}
          submitLabel="Create Entry"
        />
      </Box>
    </Modal>
  );
};