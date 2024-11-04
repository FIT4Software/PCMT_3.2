import React from "react";
import { Button, message } from 'antd';
import { VscSymbolVariable } from "react-icons/vsc";
import { FaDisplay } from "react-icons/fa6";
import { BsBoxSeam } from "react-icons/bs";
import { LuClipboardEdit } from "react-icons/lu";

import { modules } from "./constants";

const MainActions = ({onSetModuleUsed, lineselected}) => {
  const [messageApi, contextHolder] = message.useMessage();

  const handleButtonClick = (selectedModule) => {
    if (!lineselected) {
      messageApi.open({
        type: 'warning',
        content: 'Please select a line.',
      });
      onSetModuleUsed(0);
    } else {
      onSetModuleUsed(selectedModule);
    }
  };

  const renderButton = (module, Icon, label, textSize = "text-5xl") => (
    <Button
      className="bg-white p-4 shadow-md rounded-md flex flex-col items-center justify-center"
      icon={<Icon className={`text-blue-800 ${textSize}`} />}
      size="large"
      style={{ height: '100%' }}
      onClick={() => handleButtonClick(module)}
    >
      <h2 className="text-xl font-bold text-primary mt-2">{label}</h2>
    </Button>
  );

  return (
    <>
      {contextHolder}
      <div className="grid grid-cols-2 gap-4 p-4 box-border h-full">
        {renderButton(modules.variables, VscSymbolVariable, "Variables", "text-6xl")}
        {renderButton(modules.displayTemplate, FaDisplay, "Display/Template")}
        {renderButton(modules.products, BsBoxSeam, "Products")}
        {renderButton(modules.propertyUDP, LuClipboardEdit, "Property/UDP")}
      </div>
    </>
  );
};

export default MainActions;
