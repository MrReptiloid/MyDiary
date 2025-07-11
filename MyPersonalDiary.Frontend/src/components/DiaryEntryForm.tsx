import { Box, Button, IconButton, TextField } from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";
import ImageIcon from "@mui/icons-material/Image";
import { useEffect, useState } from "react";

interface DiaryEntryFormProps {
  initialContent?: string;
  initialImage?: string | null;
  onSubmit: (content: string, imageFile: File | null, removeExistingImage: boolean) => Promise<void>;
  onCancel: () => void;
  submitLabel: string;
}

export const DiaryEntryForm = ({
                                 initialContent = '',
                                 initialImage = null,
                                 onSubmit,
                                 onCancel,
                                 submitLabel
                               }: DiaryEntryFormProps) => {
  const [content, setContent] = useState(initialContent);
  const [imageFile, setImageFile] = useState<File | null>(null);
  const [imageUrl, setImageUrl] = useState<string | null>(initialImage);
  const [removeImage, setRemoveImage] = useState(false);

  useEffect(() => {
    setContent(initialContent);
    setImageUrl(initialImage);
    setImageFile(null);
    setRemoveImage(false);
  }, [initialContent, initialImage]);

  const handleSubmit = async () => {
    await onSubmit(
      content,
      imageFile,
      removeImage || (initialImage !== null && imageUrl === null)
    );
  };

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files[0]) {
      const file = e.target.files[0];
      setImageFile(file);
      setImageUrl(URL.createObjectURL(file));
      setRemoveImage(false);
    }
  };

  const handleRemoveImage = () => {
    setImageUrl(null);
    setImageFile(null);
    setRemoveImage(true);
  };

  return (
    <>
      <TextField
        fullWidth
        label="Content"
        multiline
        rows={6}
        margin="normal"
        value={content}
        onChange={(e) => setContent(e.target.value)}
      />

      <Box sx={{ mt: 2 }}>
        <Button
          variant="outlined"
          startIcon={<ImageIcon />}
          component="label"
          sx={{ mb: 2 }}
        >
          {imageUrl ? "Change Image" : "Upload Image"}
          <input
            type="file"
            hidden
            accept="image/*"
            onChange={handleImageChange}
          />
        </Button>

        {imageUrl && (
          <Box sx={{ mt: 2, position: "relative" }}>
            <img
              src={imageUrl.startsWith('blob:') ? imageUrl : `${import.meta.env.VITE_API_URL || ''}/${imageUrl}`}
              alt="Preview"
              style={{ maxWidth: "100%", maxHeight: "200px" }}
            />
            <IconButton
              size="small"
              sx={{ position: "absolute", top: 8, right: 8, bgcolor: "rgba(255,255,255,0.7)" }}
              onClick={handleRemoveImage}
            >
              <CloseIcon />
            </IconButton>
          </Box>
        )}
      </Box>

      <Box sx={{ mt: 3, display: "flex", justifyContent: "flex-end" }}>
        <Button onClick={onCancel} sx={{ mr: 1 }}>
          Cancel
        </Button>
        <Button
          variant="contained"
          onClick={handleSubmit}
          disabled={!content}
        >
          {submitLabel}
        </Button>
      </Box>
    </>
  );
};