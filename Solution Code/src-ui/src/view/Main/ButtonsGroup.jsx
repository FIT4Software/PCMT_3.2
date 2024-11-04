import React from "react";
import {Button} from 'antd';
import { VscSymbolVariable } from "react-icons/vsc";
import { FaDisplay } from "react-icons/fa6";
import { BsBoxSeam } from "react-icons/bs";
import { LuClipboardEdit } from "react-icons/lu";

const ButtonsGroup = ({
    component
}) => {

  return (
    <div className="grid grid-cols-4 ">
        <Button
          color={component}
          variant="solid"
          icon={<VscSymbolVariable className="text-white" />}
          className="w-full p-8"
        >
          <h2 className="text-primary font-semibold">Variables</h2>
        </Button>
        <Button
          icon={<FaDisplay className="text-blue-800" />}
          className="w-full p-8"
        >
          <h2 className="text-primary font-semibold">Display/Template</h2>
        </Button>
        <Button
          icon={<BsBoxSeam className="text-blue-800" />}
          className="w-full p-8"
        >
          <h2 className="text-primary font-semibold">Products</h2>
        </Button>
        <Button
          icon={<LuClipboardEdit className="text-blue-800" />}
          className="w-full p-8"
        >
          <h2 className="text-primary font-semibold">Property/UDP</h2>
        </Button>
    </div>
  );
};

export default ButtonsGroup;
