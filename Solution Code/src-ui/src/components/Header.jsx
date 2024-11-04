import { FaSliders } from "react-icons/fa6";


const Header = () => {
    return (
      <header className="bg-blue-50 text-blue-800 rounded-t-xl p-4">
        <div className="flex items-center space-x-2">
          <FaSliders />
          <h1 className="text-xl font-semibold text-primary">Config Manager</h1>
        </div>
        {/* <div className="flex space-x-4">
          <button className="bg-primary text-white py-2 px-4 rounded-md">Configure</button>
          <button className="bg-gray-100 text-gray-700 py-2 px-4 rounded-md">Change Log</button>
          <button className="bg-gray-100 text-gray-700 py-2 px-4 rounded-md">Information</button>
        </div> */}
      </header>
    );
  };
  
export default Header;
