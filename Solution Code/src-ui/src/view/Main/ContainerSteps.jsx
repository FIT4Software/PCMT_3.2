import React, { useState } from "react";
import { Button, Steps, Divider, Input, Select, Space } from 'antd';
import { VscSymbolVariable } from "react-icons/vsc";
import { FaDisplay } from "react-icons/fa6";
import { BsBoxSeam } from "react-icons/bs";
import { LuClipboardEdit } from "react-icons/lu";
import { PlusOutlined } from '@ant-design/icons';

import { modules } from "./constants";

const { Option } = Select;

const ContainerSteps = ({ moduleUsed }) => {
  const [current, setCurrent] = useState(0);
  const [newVariable, setNewVariable] = useState("");
  const [selectedUnit, setSelectedUnit] = useState(null);
  const [selectedGroup, setSelectedGroup] = useState(null);
  const [data, setData] = useState([
    {
      unit: "Unidad1",
      groups: [
        { group: "Variable Group 1", variables: ["Var001", "Var002", "Var003"] },
        { group: "Variable Group 2", variables: ["Var004", "Var005"] }
      ]
    },
    {
      unit: "Unidad2",
      groups: [
        { group: "Variable Group 1", variables: ["Var006", "Var007"] }
      ]
    }
  ]);

  const stepsConfig = {
    [modules.variables]: [
      { title: 'Create Variable Details' },
      { title: 'Set UDPs & Extended Info' },
      { title: 'Attach to Display' },
      { title: 'Review' }
    ],
    [modules.displayTemplate]: [
      { title: 'Choose Template' },
      { title: 'Review Template' }
    ],
    [modules.products]: [
      { title: 'Add Product Details' },
      { title: 'Set Product Specifications' },
      { title: 'Review Products' }
    ],
    [modules.propertyUDP]: [
      { title: 'Define Properties' },
      { title: 'Assign UDPs' }
    ]
  };

  const stepsToShow = stepsConfig[moduleUsed] || [];

  const renderButton = (module, Icon, label) => (
    <Button
      type={moduleUsed === module ? "primary" : "default"}
      icon={<Icon className={moduleUsed === module ? "text-white" : "text-blue-800"} />}
      className="w-full p-8"
    >
      <h2 className={`font-semibold ${moduleUsed === module ? "text-white" : "text-primary"}`}>{label}</h2>
    </Button>
  );

  const handleAddVariable = () => {
    if (newVariable && selectedUnit && selectedGroup) {
      const updatedData = data.map((unit) => {
        if (unit.unit === selectedUnit) {
          return {
            ...unit,
            groups: unit.groups.map((group) => {
              if (group.group === selectedGroup) {
                return {
                  ...group,
                  variables: [...group.variables, newVariable]
                };
              }
              return group;
            })
          };
        }
        return unit;
      });
      setData(updatedData);
      setNewVariable("");
    }
  };

  const handleUnitChange = (value) => {
    setSelectedUnit(value);
    setSelectedGroup(null); // Reset group selection when unit changes
  };

  const handleGroupChange = (value) => {
    setSelectedGroup(value);
  };

  const selectedUnitData = data.find((unit) => unit.unit === selectedUnit);
  const selectedGroupData = selectedUnitData?.groups.find((group) => group.group === selectedGroup);

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
        {/* Select de Unidades y Grupos en una sola fila */}
        <div className="flex space-x-4 w-full">
          <div className="flex items-center flex-1">
            <label className="font-semibold mr-2" style={{ minWidth: '80px' }}>Unit:</label>
            <Select
              style={{ width: '100%' }}
              placeholder="Select Unit"
              onChange={handleUnitChange}
              value={selectedUnit}
            >
              {data.map((unit) => (
                <Option key={unit.unit} value={unit.unit}>
                  {unit.unit}
                </Option>
              ))}
            </Select>
          </div>
          <div className="flex items-center flex-1">
            <label className="font-semibold mr-2" style={{ minWidth: '120px' }}>Variable Group:</label>
            <Select
              style={{ width: '100%' }}
              placeholder="Select Group"
              onChange={handleGroupChange}
              value={selectedGroup}
              disabled={!selectedUnit}
            >
              {selectedUnitData?.groups.map((group) => (
                <Option key={group.group} value={group.group}>
                  {group.group}
                </Option>
              ))}
            </Select>
          </div>
        </div>

        {/* Select de Variables en toda la fila */}
        <div className="flex items-center w-full">
          <label className="font-semibold mr-2" style={{ minWidth: '120px' }}>Select Variable(s):</label>
          <Select
            mode="multiple"
            style={{ flexGrow: 1 }}
            placeholder="Select Variable(s)"
            disabled={!selectedGroup}
            dropdownRender={(menu) => (
              <>
                {menu}
                <Divider style={{ margin: '8px 0' }} />
                <Space style={{ padding: '0 8px 4px' }}>
                  <Input
                    placeholder="New Variable"
                    value={newVariable}
                    onChange={(e) => setNewVariable(e.target.value)}
                    onKeyDown={(e) => e.stopPropagation()}
                  />
                  <Button type="text" icon={<PlusOutlined />} onClick={handleAddVariable}>
                    Add Variable
                  </Button>
                </Space>
              </>
            )}
          >
            {selectedGroupData?.variables.map((variable) => (
              <Option key={variable} value={variable}>
                {variable}
              </Option>
            ))}
          </Select>
        </div>
      </div>
    </div>
  );
};

export default ContainerSteps;
