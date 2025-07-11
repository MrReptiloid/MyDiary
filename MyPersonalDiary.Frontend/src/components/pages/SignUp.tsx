import Avatar from '@mui/material/Avatar';
import Button from '@mui/material/Button';
import CssBaseline from '@mui/material/CssBaseline';
import TextField from '@mui/material/TextField';
import Grid from '@mui/material/Grid';
import Box from '@mui/material/Box';
import LockOutlinedIcon from '@mui/icons-material/LockOutlined';
import Typography from '@mui/material/Typography';
import Container from '@mui/material/Container';
import { useNavigate, Link } from "react-router-dom";
import UserService from "../../services/UserSrvice.ts"
import {Alert, FormControl, ImageList, ImageListItem, InputLabel, MenuItem, Paper, Select} from "@mui/material";
import {useEffect, useState} from "react";
import CaptchaService from "../../services/CaptchaService.ts";

function Copyright(props: any) {
  return (
    <Typography variant="body2" color="text.secondary" align="center" {...props}>
      {'Copyright © '}
      <Link to="/">
        Your Website
      </Link>{' '}
      {new Date().getFullYear()}
      {'.'}
    </Typography>
  );
}

export const SignUp = () => {
  const navigate = useNavigate()
  const queryParams = new URLSearchParams(location.search);
  const [role, setRole] = useState('user');
  const [captchaVerified, setCaptchaVerified] = useState(false);
  const [selectedImage, setSelectedImage] = useState<string | null>(null);
  const [captchaImages, setCaptchaImages] = useState([] as string[]);
  const [captchaFolder, setCaptchaFolder] = useState<string | null>(null);

  const [verifiedCaptcha, setVerifiedCaptcha] = useState("");

  useEffect(() => {
    CaptchaService.GetImages().then(response => {
      setCaptchaImages(response.data.links)
      setCaptchaFolder(response.data.folder)
    });
  }, [])

  const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
    const inviteCode = queryParams.get('inviteCode') || '';

    event.preventDefault();
    const data = new FormData(event.currentTarget);

    UserService.Register(
      String(data.get('userName')),
      String(data.get('password')),
      String(data.get('email')),
      Number(role),
      inviteCode,
      verifiedCaptcha
    ).then(response => {
      if (response.status == 200)
        navigate("../signin")
    })
  };

  const handleRoleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    console.log(event.target.value);
    setRole(event.target.value);
  };

  const handleCaptchaImageSelect = (index: string) => {
    setSelectedImage(index);
  };

  const verifyCaptcha = () => {
    if (selectedImage === null) {
      return;
    }

    try {
      CaptchaService.VerifyCaptcha(selectedImage)
        .then((response) => setVerifiedCaptcha(response.data.captcha));
      setCaptchaVerified(true);
    } catch (e: any){
      return;
    }
  };

  return (
    <Container component="main" maxWidth="xs">
      <CssBaseline />
      <Box
        sx={{
          marginTop: 8,
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
        }}
      >
        <Avatar sx={{ m: 1, bgcolor: 'secondary.main' }}>
          <LockOutlinedIcon />
        </Avatar>
        <Typography component="h1" variant="h5">
          Sign Up
        </Typography>
        <Box component="form" noValidate onSubmit={handleSubmit} sx={{ mt: 3 }}>
          <Grid container spacing={2}>
            <Grid item xs={12} >
              <TextField
                autoComplete="given-name"
                name="userName"
                required
                fullWidth
                id="userName"
                label="User Name"
                autoFocus
              />
            </Grid>
            <Grid item xs={12} >
              <TextField
                autoComplete="given-name"
                name="email"
                required
                fullWidth
                id="email"
                label="Email"
                autoFocus
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                required
                fullWidth
                name="password"
                label="Password"
                type="password"
                id="password"
                autoComplete="new-password"
              />
            </Grid>
            <Grid item xs={12}>
              <FormControl fullWidth>
                <InputLabel id="role-select-label">Role</InputLabel>
                <Select
                  labelId="role-select-label"
                  id="role-select"
                  value={role}
                  label="Role"
                  onChange={handleRoleChange}
                >
                  <MenuItem value="0">User</MenuItem>
                  <MenuItem value="1">Admin</MenuItem>
                </Select>
              </FormControl>
            </Grid>
          </Grid>
          <Button
            type="submit"
            fullWidth
            variant="contained"
            sx={{ mt: 3, mb: 2 }}
          >
            Sign Up
          </Button>
          <>
            {/* Add this right before the submit button */}
            <Grid item xs={12} sx={{ mt: 2 }}>
              <Typography variant="subtitle1" gutterBottom>
                Security Verification
              </Typography>

              {!captchaVerified ? (
                <>
                  <Typography variant="body2" sx={{ mb: 1 }}>
                    Please select the image of a {/* Insert challenge text here */} car
                  </Typography>

                  <Paper elevation={1} sx={{ p: 2, mb: 2 }}>
                    <ImageList cols={2} rowHeight={120} sx={{ mb: 1 }}>
                      {captchaImages?.map((img, index) => (
                        <ImageListItem
                          key={index}
                          onClick={() => handleCaptchaImageSelect(img)}
                          sx={{
                            cursor: 'pointer',
                            border: selectedImage === img ? '2px solid #1976d2' : '2px solid transparent',
                            borderRadius: 1,
                            transition: 'all 0.2s',
                            '&:hover': { opacity: 0.8 }
                          }}
                        >
                          <img
                            src={`https://${import.meta.env.VITE_REACT_APP_SERVERDOMAIN || ''}/CaptchaTemp/${captchaFolder}/${img}`}
                            alt={`Captcha image ${index + 1}`}
                            loading="lazy"
                            style={{ height: '100%', objectFit: 'cover' }}
                          />
                        </ImageListItem>
                      ))}
                    </ImageList>

                    <Button
                      fullWidth
                      variant="outlined"
                      onClick={verifyCaptcha}
                      disabled={selectedImage === null}
                    >
                      Verify
                    </Button>
                  </Paper>
                </>
              ) : (
                <Alert severity="success" sx={{ mb: 2 }}>
                  CAPTCHA verification completed
                </Alert>
              )}
            </Grid>
          </>
          <Grid container justifyContent="flex-end">
            <Grid item>
              <Link to="../signin">
                Already have an account? Sign in
              </Link>
            </Grid>
          </Grid>
        </Box>
      </Box>
      <Copyright sx={{ mt: 5 }} />
    </Container>
  );
}