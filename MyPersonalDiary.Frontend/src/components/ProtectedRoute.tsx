import {useEffect, useState} from "react"
import UserService from "../services/UserSrvice"
import {Outlet, Navigate} from "react-router-dom"
import {jwtDecode} from "jwt-decode";

interface DecodedToken {
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier": string;
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name": string;
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress": string;
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": string;
  exp: number;
}

export const ProtectedRoute = ({ requiredRole }: { requiredRole?: string }) => {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean | null>(null);
  const [hasRequiredRole, setHasRequiredRole] = useState<boolean>(true);

  useEffect(() => {
    UserService.Verify()
      .then(response => {
        setIsAuthenticated(response.status === 200);

        const getCookie = (name: string) => {
          const value = `; ${document.cookie}`;
          const parts = value.split(`; ${name}=`);
          if (parts.length === 2) return parts.pop()?.split(';').shift();
          return null;
        };

        const token = getCookie('tasty-cookies');

        if (token) {
          try {
            const decoded = jwtDecode<DecodedToken>(token);
            const userRole = decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

            // If requiredRole is specified, check if user has it
            if (requiredRole) {
              setHasRequiredRole(userRole === requiredRole);
            }

            console.log("User role:", userRole);
          } catch (error) {
            console.error("Error decoding token:", error);
            setIsAuthenticated(false);
          }
        } else {
          setIsAuthenticated(false);
        }
      })
      .catch(() => {
        setIsAuthenticated(false);
      });
  }, [requiredRole]);

  // Show loading state while checking authentication
  if (isAuthenticated === null) {
    return <div>Loading...</div>;
  }

  // Redirect if not authenticated or doesn't have required role
  if (!isAuthenticated) {
    return <Navigate to="/signin" />;
  }

  if (!hasRequiredRole) {
    return <Navigate to="/unauthorized" />;
  }

  // User is authenticated and has required role
  return <Outlet />;
}