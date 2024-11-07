import React from "react";
import { Button } from 'antd';

const ModuleButton = ({ Icon, label, isSelected, onClick }) => (
  <Button
    type={isSelected ? "primary" : "default"}
    icon={<Icon className={isSelected ? "text-white" : "text-blue-800"} />}
    className="w-full p-8"
    onClick={onClick}
  >
    <h2 className={`font-semibold ${isSelected ? "text-white" : "text-primary"}`}>{label}</h2>
  </Button>
);

export default ModuleButton;
