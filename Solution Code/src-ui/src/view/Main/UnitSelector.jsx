// UnitSelector.js
import React, { useState } from "react";
import { Select } from 'antd';

const { Option } = Select;

const UnitSelector = ({
  data,
  productionGroups,
  onSetProductionGroups,
  onSetVariablesRows
}) => {

  const [selectedUnit, setSelectedUnit] = useState(null);
  const [selectedGroup, setSelectedGroup] = useState(null);
  const [selectedSlave, setSelectedSlave] = useState(null);
  const [slaveUnits, setSlaveUnits] = useState([]);

  
  const handleUnitChange = (value) => {
    const unit = data.find((u) => u.unit === value);
    setSelectedUnit(value);
    setSelectedSlave(null); 
    setSelectedGroup(null); 
  
    if (unit.master === null) {
      setSlaveUnits(data.filter((u) => u.master === value)); 
    } else {
      setSlaveUnits([]);
    }
  
    onSetProductionGroups(unit.groups);
  };
  
  const handleSlaveChange = (value) => {
    setSelectedSlave(value);
    setSelectedGroup(null); 
    var global;
    if (value) {
      global =  data.find((unit) => unit.unit === value);
    } else {
      global = data.find((unit) => unit.unit === selectedUnit);
    }
    onSetProductionGroups(global ? global.groups : []);
  };
  
  const handleGroupChange = (value) => {
    setSelectedGroup(value);
    onSetVariablesRows(productionGroups.filter(group => group.group === value)[0].variables)

    console.log("variables",productionGroups.filter(group => group.group === value)[0].variables)
  };
  
  return (
  <div> 
    {/* Contenedor para Unit y Slave Unit en la misma línea */}
    <div className="flex space-x-4 w-full mb-4">
      <div className={`flex items-center ${slaveUnits.length > 0 ? 'w-1/2' : 'w-full'}`}>
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

      {/* Select de Unidades Esclavas (solo si la unidad seleccionada es Maestra y tiene esclavas) */}
      {slaveUnits.length > 0 && (
        <div className="flex items-center w-1/2">
          <label className="font-semibold mr-2" style={{ minWidth: '80px' }}>Slave Unit:</label>
          <Select
            style={{ width: '100%' }}
            placeholder="Select Slave Unit (optional)"
            onChange={handleSlaveChange}
            value={selectedSlave}
            allowClear
          >
            {slaveUnits.map((slave) => (
              <Option key={slave.unit} value={slave.unit}>
                {slave.unit}
              </Option>
            ))}
          </Select>
        </div>
      )}
    </div>
    
    {/* Select de Grupos de Variables */}
    <div className="flex items-center w-full mb-4">
      <label className="font-semibold mr-2" style={{ minWidth: '120px' }}>Variable Group:</label>
      <Select
        style={{ width: '100%' }}
        placeholder="Select Group"
        onChange={handleGroupChange}
        value={selectedGroup}
        disabled={!selectedUnit && !selectedSlave}
      >
        {productionGroups.map((group) => (
          <Option key={group.group} value={group.group}>
            {group.group}
          </Option>
        ))}
      </Select>
    </div>
  </div>
)};

export default UnitSelector;
