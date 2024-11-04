import './App.css';
import React from 'react';
import Index from './view/Main/Index';


function App() {
  return (
    <div className="bg-white min-h-screen flex flex-col">
      <div className="border p-4 border-aliceblue bg-[#f3f8fb] flex flex-col flex-grow ">
        <Index></Index>
      </div>
    </div>
  );
}

export default App;