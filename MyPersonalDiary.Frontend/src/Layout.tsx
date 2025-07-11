import { Header } from "./components";
import { Container } from "@mui/material";
import { Outlet } from "react-router-dom"
import { Footer } from "./components/LayoutComponents/Footer.tsx";

const Layout = () => {
  return (
    <>
      <Header/>
      <Container
        sx={{ mt: '8px', pb: '8px' }}
      >
        <Outlet/>
      </Container>
      <Footer/>
    </>
  )
}

export default Layout
