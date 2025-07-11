import './App.css'
import {
  Route,
  RouterProvider,
  createBrowserRouter,
  createRoutesFromElements,
} from "react-router-dom"

import Layout from "./Layout"
import { ProtectedRoute } from "./components/ProtectedRoute"
import { Home } from "./components/pages/Home.tsx";
import { SignUp } from "./components/pages/SignUp.tsx"
import { SignIn } from "./components/pages/SignIn.tsx"
import { InvitePage } from "./components/pages/InvitePage.tsx";
import { DeleteAccount } from "./components/pages/DeleteAccount";
import { RestoreAccount } from "./components/pages/RestoreAccount";

const router = createBrowserRouter(
  createRoutesFromElements(
    <>
      <Route path="/" element={<Layout />}>
        <Route path="/" element={<Home/>} />
        <Route path="signup" element={<SignUp />} />
        <Route path="signin" element={<SignIn />} />
        <Route path="delete-account" element={<DeleteAccount />} />
        <Route path="restore-account" element={<RestoreAccount />} />
        <Route element={<ProtectedRoute requiredRole={"Admin"}/>}>
          <Route path="invite" element={<InvitePage/>} />
        </Route>
      </Route>
    </>,
  ),
)


function App() {
  return <RouterProvider router={router} />
}

export default App
