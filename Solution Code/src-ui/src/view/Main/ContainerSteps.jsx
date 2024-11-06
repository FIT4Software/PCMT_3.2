import React, { useState } from "react";
import { Button, Steps, Table } from 'antd';
import UnitSelector from './UnitSelector';
import { VscSymbolVariable } from "react-icons/vsc";
import { FaDisplay } from "react-icons/fa6";
import { BsBoxSeam } from "react-icons/bs";
import { LuClipboardEdit } from "react-icons/lu";
import { modules,rowData,stepsConfig } from "./constants";


const ContainerSteps = ({ moduleUsed }) => {
  const [current, setCurrent] = useState(0);
  const [productionGroups, setProductionGroups] = useState([]);
  const [variablesRows, setVariablesRows] = useState([]) 
  const stepsToShow = stepsConfig[moduleUsed] || [];
  const [count, setCount] = useState(13);
  const [rowsdatita, setRowsdatita] = useState(rowData);
  

  const renderButton = (module, Icon, label) => (
    <Button
      type={moduleUsed === module ? "primary" : "default"}
      icon={<Icon className={moduleUsed === module ? "text-white" : "text-blue-800"} />}
      className="w-full p-8"
    >
      <h2 className={`font-semibold ${moduleUsed === module ? "text-white" : "text-primary"}`}>{label}</h2>
    </Button>
  );


  const columns = [
    {
      title: 'ID',
      dataIndex: 'id',
      key: 'id',
    },
    {
      title: 'Variable Description',
      dataIndex: 'name',
      key: 'name',
    },
  ];

  const handleAdd = () => {
    const newVariable = {
      id: count,
      name: `Variable 00${count}`,
    };
    console.log("newVariable",newVariable)
    console.log("rowsdatita",rowsdatita)

    setVariablesRows([newVariable, ...variablesRows]);
    setCount(count + 1);
    
  };

  

  return (
    <div className="p-4 h-full">
      {/* Contenedor de Botones */}
      <div className="grid grid-cols-4 gap-4">
        {renderButton(modules.variables, VscSymbolVariable, "Variables")}
        {renderButton(modules.displayTemplate, FaDisplay, "Display/Template")}
        {renderButton(modules.products, BsBoxSeam, "Products")}
        {renderButton(modules.propertyUDP, LuClipboardEdit, "Property/UDP")}
      </div>

      {/* Contenedor de Steps */}
      <div className="flex flex-col justify-center p-4">
        <Steps
          current={current}
          onChange={(value) => setCurrent(value)}
          items={stepsToShow.map(step => ({ title: step.title }))}
        />
      </div>

      {/* Selectores de Unidades, Grupos y Variables */}
      <div className="flex flex-col justify-center p-4 space-y-4">
        <UnitSelector 
          data={rowsdatita}
          productionGroups={productionGroups}
          onSetProductionGroups={setProductionGroups}
          onSetVariablesRows={setVariablesRows}/>
        <Button onClick={handleAdd} type="primary" style={{ marginBottom: 16 }}>
          Add a variable
        </Button>
        <Table dataSource={variablesRows} columns={columns} />
      </div>
    </div>
  );
};

export default ContainerSteps;
