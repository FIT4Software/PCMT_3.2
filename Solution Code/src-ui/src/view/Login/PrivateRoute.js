import React from 'react';
import { Navigate } from 'react-router-dom';
// import { loggedIn } from "../services/auth"; 

const PrivateRoute = ({ children }) => {
    const isAuthenticated = localStorage.getItem("isAuthenticated") === "true";
  
    return isAuthenticated ? children : <Navigate to="/login" />;
  };

export default PrivateRoute;
