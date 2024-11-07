import './App.css';
import React from 'react';
import { HashRouter as Router, Routes, Route } from "react-router-dom";
import PrivateRoute from "./view/Login/PrivateRoute.js";
import Index from './view/Main/Index';
import Login from './view/Login/Index';

function App() {
  return (
    <div className="bg-white min-h-screen flex flex-col">
      <div className="border p-4 border-aliceblue bg-[#f3f8fb] flex flex-col flex-grow">
        <Router>
          <Routes>
            <Route path="/login" element={<Login />} />
            <Route path="/" element={<PrivateRoute><Index /></PrivateRoute>} />
          </Routes>
        </Router>
      </div>
    </div>
  );
}

export default App;
