import { Box, Container, Grid, Link, Typography } from "@mui/material";

export const Footer = () => {
  return (
    <Box
      sx={{
        width: "100%",
        height: "auto",
        bgcolor: "#e0e0ff",
        paddingTop: "1rem",
        paddingBottom: "1rem",
      }}
    >
      <Container maxWidth="lg">
        <Grid container direction="column" alignItems="center">
          <Grid item xs={12}>
            <Typography color="black" variant="h5">
              React Starter App
            </Typography>
          </Grid>
          <Grid item xs={12}>
            <Typography color="textSecondary" variant="subtitle1">
              {`${new Date().getFullYear()} | React | Material UI | React Router`}
            </Typography>
          </Grid>
          <Grid item xs={12}>
            <Link
              href={""}
              sx={{mr:"4px"}}
            >
              GitHub Frontend
            </Link>
            |
            <Link
              href={""}
              sx={{ml:"4px"}}
            >
              Github Backend
            </Link>
          </Grid>
        </Grid>
      </Container>
    </Box>
  );
};
