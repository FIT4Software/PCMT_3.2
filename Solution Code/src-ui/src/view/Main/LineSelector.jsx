import React, { useState } from "react";
import { Popconfirm, Button, Radio, Input } from 'antd';
import { LuListFilter } from "react-icons/lu";

const LineSelector = ({ lineselected, onSelectLine, onSetModuleUsed }) => {
  const [searchTerm, setSearchTerm] = useState("");

  const lines = [
    "CB1021", "CB1022", "CB1023", "CB1024", "CB1025",
    "CABTOYO101", "CABTOYO102", "CABTOYO103", "CABTOYO104",
    "CABTOYO105", "CABTOYO106", "CABTOYO107", "CABTOYO108",
    "CABTOYO109", "CABTOYO110", "CABTOYO111", "CABTOYO112", "CABTOYO113",
  ];

  const handleSelectLine = (line) => {
    onSelectLine(line);
  };

  const handleDeselect = () => {
    onSelectLine(null);
    onSetModuleUsed(0);
  };

  const filteredLines = lines.filter((line) =>
    line.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="bg-blue-50 rounded-lg p-4 h-full flex flex-col">
      <button className="border border-blue-700 text-blue-700 py-2 rounded-md text-lg md:text-sm">
        Line Selected
      </button>
      <div className="p-2">
        {lineselected ? (
          <div className="bg-blue-900 text-white rounded-md p-2 mb-1 flex items-center text-lg md:text-sm">
            <Radio checked={true} onChange={() => handleDeselect()} className="mr-2" />
            {lineselected}
          </div>
        ) : null}
      </div>

      <h3 className="text-sm font-semibold mb-2 md:text-xs">SELECT THE LINE</h3>
      <div className="mb-2">
        <Input
          prefix={<LuListFilter />}
          placeholder="Filter"
          className="w-full p-2 border border-gray-300 rounded-md"
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
        />
      </div>

      <div className="space-y-2 flex-grow overflow-y-auto">
        <Radio.Group value={lineselected} onChange={(e) => handleSelectLine(e.target.value)}>
          {filteredLines.map((line) => (
            <div key={line} className="flex items-center">
              <Radio value={line} className="mr-2 text-lg md:text-sm">{line}</Radio>
            </div>
          ))}
        </Radio.Group>
      </div>

      <div className="mt-4 flex flex-col space-y-2">
        <Button
          onClick={handleDeselect}
          className="bg-blue-900 text-white py-1 md:py-2 rounded-md text-xs md:text-sm lg:text-base max-w-full"
        >
          Save the changes
        </Button>
        <Popconfirm
          placement="rightBottom"
          title="Are you sure to discard all the changes?"
          description="Discard all the changes"
          okText="Yes"
          cancelText="No"
          onConfirm={handleDeselect}
        >
          <Button
            className="border border-blue-700 text-blue-700 py-1 md:py-2 rounded-md text-xs md:text-sm lg:text-base max-w-full"
          >
            Discard all the changes
          </Button>
        </Popconfirm>
      </div>
    </div>
  );
};

export default LineSelector;
