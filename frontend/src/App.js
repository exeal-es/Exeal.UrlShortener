import React from 'react';
import { Auth0Provider } from '@auth0/auth0-react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import './App.css';
import Navbar from './components/Navbar';
import Profile from './components/Profile';
import LinkDetails from './pages/LinkDetails';
import LinkEdit from './pages/LinkEdit';

function App() {
  return (
    <Auth0Provider
      domain={process.env.REACT_APP_AUTH0_DOMAIN}
      clientId={process.env.REACT_APP_AUTH0_CLIENT_ID}
      authorizationParams={{
        audience: process.env.REACT_APP_AUTH0_AUDIENCE
      }}
      redirectUri={window.location.origin}
    >
      <BrowserRouter>
        <div className="min-h-screen bg-gray-50 text-gray-900 font-sans">
          <Navbar />
          <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
            <Routes>
              <Route path="/" element={<Profile />} />
              <Route path="/links/:slug/details" element={<LinkDetails />} />
              <Route path="/links/:slug/edit" element={<LinkEdit />} />
            </Routes>
          </main>
        </div>
      </BrowserRouter>
    </Auth0Provider>
  );
}

export default App;
