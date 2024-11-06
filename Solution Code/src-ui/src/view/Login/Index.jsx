import React, { useState, useEffect } from "react";
import { FaUser, FaKey, FaSignInAlt, FaSpinner, FaServer, FaExclamation } from "react-icons/fa";
import { Select } from "antd";
import { useNavigate } from "react-router-dom"; 

const Login = () => {
  const [server, setServer] = useState("");
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [errorMsg, setErrorMsg] = useState("");
  const [loading, setLoading] = useState(false);
  const [showLogin, setShowLogin] = useState(true);
  const [serverSelector, setServerSelector] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    document.title = "Login";
  }, []);

  const handleInputChange = (event) => {
    const { name, value } = event.target;
    if (name === "username") setUsername(value);
    if (name === "password") setPassword(value);
    setErrorMsg("");
  };

  const handleSubmit = (event) => {
    event.preventDefault();
    console.log(username)
    console.log(password)
    console.log(password === "123")
    if (username === "camila" && password === "123" && auth.servers.length > 0 ) {
      setErrorMsg("You have access to more than one server. Please select one to continue");
      setServerSelector(true);
    }
    if (username === "camila" && password === "123" && server) {
      setLoading(true);
      setErrorMsg("");
      setTimeout(() => {
        setShowLogin(false)
        setLoading(true);
        localStorage.setItem("isAuthenticated", "true"); // localStorage
        navigate("/"); 
      }, 1500); // Delay
    } else {
      setErrorMsg("Incorrect Credentials");
      setLoading(false);
    }
  };

  const handleSubmitServers = (value) => {
    setServer(value);
  };

  const auth = {
    servers: [
      { serverId: 1, serverName: "Mehoopany" },
      { serverId: 2, serverName: "Dover" },
      { serverId: 3, serverName: "DevLab8" },
      { serverId: 4, serverName: "Villa Mercedes" },
    ],
  };

  return (
    <div className="bg-[#f3f8fb] flex items-center justify-center h-screen">
      <div className="bg-[#f3f8fb] flex justify-center items-center h-screen">
        <div className="bg-white shadow-md rounded-lg text-center p-8 min-h-[60vh]">
          <form onSubmit={handleSubmit} className="w-[450px] max-w-full mx-auto">
            <div>
              <img
                alt=""
                src={`${process.env.PUBLIC_URL}/images/logoPCMT.png`}
                className="w-[90%] my-4 mx-1"
              />
              <h4 className="text-lg font-bold">PCMT v3.2</h4>
            </div>
            <div className="p-8">
              <div className="relative mb-6">
                <input
                  type="text"
                  name="username"
                  placeholder="Username"
                  value={username}
                  onChange={handleInputChange}
                  className="w-[80%] p-2 border-b-2 border-gray-400 focus:border-gray-600 outline-none"
                />
                <FaUser className="absolute right-4 top-2 text-[#003da7]" />
              </div>
              <div className="relative mb-6">
                <input
                  type="password"
                  name="password"
                  placeholder="Password"
                  value={password}
                  onChange={handleInputChange}
                  className="w-[80%] p-2 border-b-2 border-gray-400 focus:border-gray-600 outline-none"
                />
                <FaKey className="absolute right-4 top-2 text-[#003da7]" />
              </div>
              {serverSelector && (
                <div className="relative mb-6">
                  <Select
                    placeholder="Select Server"
                    className="w-[80%] border-gray-400 focus:border-gray-600 outline-none"
                    value={server}
                    onChange={(value) => handleSubmitServers(value)}
                    options={auth.servers.map((srv) => ({
                      label: srv.serverName, 
                      value: srv.serverId,
                    }))}
                  />
                  <FaServer className="absolute right-4 top-2 text-[#003da7]" />
                </div>
              )}
              {showLogin && (<button
                type="submit"
                disabled={loading}
                className="bg-[#003da7] text-white font-semibold py-2 px-6 rounded mt-4 hover:bg-blue-700 transition duration-200"
              >
                <FaSignInAlt className="inline" />
                &nbsp;Log In
              </button>)}
              <div className="mt-4">
                {loading && (
                  <span className="flex items-center justify-center">
                    <FaSpinner className="animate-spin" />
                    &nbsp;{"Loading"}
                  </span>
                )}
                {errorMsg && (
                  <span className="flex items-center justify-center">
                    <FaExclamation/>
                    {errorMsg}
                  </span>
                )}
              </div>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default Login;
