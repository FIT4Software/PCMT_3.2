import React, { useState } from "react";
import { Button  } from 'antd';
import { FaSliders, FaUser  } from "react-icons/fa6";
import { IoLogOutOutline } from "react-icons/io5";
import ContainerSteps from './ContainerSteps';
import MainActions from './MainActions';
import LineSelector from './LineSelector';



const Index = () => {
    const [lineselected, setLineselected] = useState(null);
    const [moduleUsed, setModuleUsed] = useState(0);

    return (
      <div className="flex flex-col h-full">
        <header className="bg-blue-50 text-blue-800 rounded-t-xl p-4">
          <div className="flex items-center justify-between">
            <div className="flex items-center space-x-2">
              <FaSliders />
              <h1 className="text-xl font-semibold text-primary">Config Manager</h1>
            </div>
            <div className="flex items-center space-x-2">
              <FaUser />
              <h5 className="text-m font-semibold text-primary">Cami Olguin</h5>
              <Button type="primary" icon={<IoLogOutOutline />} />
            </div>
          </div>
        </header>
        
        <div className="flex-grow flex flex-row h-full">
          <div className="w-1/5 bg-gray-50 p-4 flex-shrink-0 flex flex-col">
            <LineSelector 
                lineselected={lineselected}
                onSelectLine={setLineselected}
                onSetModuleUsed={setModuleUsed}
                className="flex-grow" />
          </div>
          <main className="flex-grow bg-gray-50 flex flex-col">
            {console.log(lineselected, moduleUsed)}
            {lineselected && moduleUsed !== 0 
              ? (
                <ContainerSteps 
                  moduleUsed={moduleUsed} 
                  className="flex-grow" />
              ) : (
                <MainActions 
                  onSetModuleUsed={setModuleUsed}
                  lineselected={lineselected}
                  className="flex-grow" />
              )}
          </main>
        </div>

        <footer className="bg-blue-50 text-blue-800 rounded-t-xl p-4">
            <div className="flex items-center justify-end">
                <div className="flex items-center space-x-2">
                    <h5 className="font-semibold text-primary">Connected to <b>DEV-LAB8-DATA.na.pg.com</b></h5>
                </div>
                <div className="w-px h-8 bg-gray-400 mx-2"></div>
                <div className="flex items-center space-x-2">
                    <h1 className="font-semibold text-primary">Session timeout: 24 min</h1>
                </div>
            </div>
        </footer>

      </div>
    );
  };
  
  export default Index;
  
