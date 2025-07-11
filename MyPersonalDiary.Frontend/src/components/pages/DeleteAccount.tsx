import { useState } from "react";
import {
  Box,
  Typography,
  Container,
  Button,
  TextField,
  Paper,
  Alert,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
} from "@mui/material";
import WarningIcon from "@mui/icons-material/Warning";
import { useNavigate } from "react-router-dom";
import UserService from "../../services/UserSrvice";

export const DeleteAccount = () => {
  const [password, setPassword] = useState("");
  const [confirmDialogOpen, setConfirmDialogOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();

  const handleDeleteRequest = () => {
    if (!password) {
      setError("Password is required");
      return;
    }
    setConfirmDialogOpen(true);
  };

  const handleDeleteAccount = async () => {
    try {
      setIsLoading(true);
      setError(null);

      await UserService.DeleteAccount(password);

      localStorage.removeItem("token");
      navigate("/signin", {
        state: { message: "Your account has been successfully deleted" }
      });
    } catch (error) {
      console.error("Error deleting account:", error);
      setError("Failed to delete account. Please ensure your password is correct.");
    } finally {
      setIsLoading(false);
      setConfirmDialogOpen(false);
    }
  };

  return (
    <Container maxWidth="md">
      <Box sx={{ my: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom align="center" color="error">
          Delete Account
        </Typography>

        <Paper
          elevation={3}
          sx={{
            p: 4,
            mt: 4,
            mb: 2,
            border: "1px solid #f44336",
            borderRadius: 2,
          }}
        >
          <Box sx={{ display: "flex", alignItems: "center", mb: 2 }}>
            <WarningIcon color="error" sx={{ fontSize: 40, mr: 2 }} />
            <Typography variant="h6" component="h2" color="error">
              Warning: This action cannot be undone
            </Typography>
          </Box>

          <Typography variant="body1" paragraph>
            Deleting your account will permanently remove:
          </Typography>

          <Box component="ul" sx={{ ml: 2 }}>
            <Typography component="li" variant="body1" paragraph>
              All your diary entries and associated images
            </Typography>
            <Typography component="li" variant="body1" paragraph>
              Your account information and settings
            </Typography>
            <Typography component="li" variant="body1" paragraph>
              All saved data and preferences
            </Typography>
          </Box>

          <Typography variant="body1" paragraph sx={{ fontWeight: "bold" }}>
            This action is permanent and cannot be reversed.
          </Typography>

          {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

          <TextField
            fullWidth
            label="Enter your password to confirm"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            margin="normal"
            required
            error={!!error}
          />

          <Box sx={{ mt: 3, display: "flex", justifyContent: "space-between" }}>
            <Button
              variant="outlined"
              onClick={() => navigate("/settings")}
              disabled={isLoading}
            >
              Cancel
            </Button>
            <Button
              variant="contained"
              color="error"
              onClick={handleDeleteRequest}
              disabled={!password || isLoading}
              startIcon={isLoading ? <CircularProgress size={20} color="inherit" /> : null}
            >
              {isLoading ? "Processing..." : "Delete My Account"}
            </Button>
          </Box>
        </Paper>
      </Box>

      <Dialog open={confirmDialogOpen} onClose={() => setConfirmDialogOpen(false)}>
        <DialogTitle>Confirm Account Deletion</DialogTitle>
        <DialogContent>
          Are you absolutely sure you want to delete your account? This action cannot be undone and all your data will be permanently lost.
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setConfirmDialogOpen(false)} disabled={isLoading}>
            Cancel
          </Button>
          <Button onClick={handleDeleteAccount} color="error" disabled={isLoading}>
            {isLoading ? "Deleting..." : "Yes, Delete My Account"}
          </Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
};

export default DeleteAccount;