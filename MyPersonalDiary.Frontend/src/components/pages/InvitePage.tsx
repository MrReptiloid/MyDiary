import { useState } from "react";
import {
  Box,
  Button,
  Container,
  Typography,
  Paper,
  CircularProgress,
  Snackbar,
  Alert,
  TextField,
  IconButton
} from "@mui/material";
import ContentCopyIcon from "@mui/icons-material/ContentCopy";
import InviteService from "../../services/InviteService";

export const InvitePage = () => {
  const [inviteCode, setInviteCode] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [showAlert, setShowAlert] = useState(false);

  const handleGenerateCode = async () => {
    setLoading(true);
    setError(null);

    try {
      const response = await InviteService.CreateInviteCode();
      if (response.status === 200) {
        const code = response.data.inviteCode;
        setInviteCode(code);
        setShowAlert(true);
      } else {
        setError("Failed to generate invitation code");
      }
    } catch (err) {
      setError("An error occurred while generating the invite code");
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const copyToClipboard = () => {
    if (inviteCode) {
      navigator.clipboard.writeText(inviteCode);
      setShowAlert(true);
    }
  };

  const handleCloseAlert = () => {
    setShowAlert(false);
  };

  const getInviteLink = () => {
    if (!inviteCode) return "";
    return `${window.location.origin}/signup?inviteCode=${inviteCode}`;
  };

  return (
    <Container maxWidth="md">
      <Box sx={{ mt: 8, display: "flex", flexDirection: "column", alignItems: "center" }}>
        <Typography component="h1" variant="h4" gutterBottom>
          Invitation Code Generator
        </Typography>

        <Paper elevation={3} sx={{ p: 4, width: "100%", mb: 4 }}>
          <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
            <Typography variant="body1">
              Generate an invitation code to register new users on the platform.
            </Typography>

            <Button
              variant="contained"
              color="primary"
              disabled={loading}
              onClick={handleGenerateCode}
              sx={{ alignSelf: "flex-start" }}
            >
              {loading ? <CircularProgress size={24} /> : "Generate Invitation Code"}
            </Button>

            {inviteCode && (
              <>
                <Box sx={{ mt: 2 }}>
                  <Typography variant="subtitle1" gutterBottom>
                    Invitation Code:
                  </Typography>
                  <TextField
                    fullWidth
                    variant="outlined"
                    value={inviteCode}
                    InputProps={{
                      readOnly: true,
                      endAdornment: (
                        <IconButton onClick={copyToClipboard} edge="end">
                          <ContentCopyIcon />
                        </IconButton>
                      ),
                    }}
                  />
                </Box>

                <Box sx={{ mt: 2 }}>
                  <Typography variant="subtitle1" gutterBottom>
                    Invitation Link:
                  </Typography>
                  <TextField
                    fullWidth
                    variant="outlined"
                    value={getInviteLink()}
                    InputProps={{
                      readOnly: true,
                      endAdornment: (
                        <IconButton onClick={() => {
                          navigator.clipboard.writeText(getInviteLink());
                          setShowAlert(true);
                        }} edge="end">
                          <ContentCopyIcon />
                        </IconButton>
                      ),
                    }}
                  />
                </Box>
              </>
            )}

            {error && (
              <Alert severity="error" sx={{ mt: 2 }}>
                {error}
              </Alert>
            )}
          </Box>
        </Paper>
      </Box>

      <Snackbar
        open={showAlert}
        autoHideDuration={3000}
        onClose={handleCloseAlert}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
      >
        <Alert onClose={handleCloseAlert} severity="success" sx={{ width: '100%' }}>
          {inviteCode ? "Invitation code generated successfully!" : "Copied to clipboard!"}
        </Alert>
      </Snackbar>
    </Container>
  );
};