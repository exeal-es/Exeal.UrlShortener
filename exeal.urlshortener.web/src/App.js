import React from 'react';
import { Auth0Provider } from '@auth0/auth0-react';
import './App.css';
import Navbar from './components/Navbar';
import Profile from './components/Profile';

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
      <div className="min-h-screen bg-gray-50 text-gray-900 font-sans">
        <Navbar />
        <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <Profile />
        </main>
      </div>
    </Auth0Provider>
  );
}

export default App;
