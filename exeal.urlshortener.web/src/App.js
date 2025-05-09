import React, { useRef } from 'react';
import { Auth0Provider, useAuth0 } from '@auth0/auth0-react';
import './App.css';
import UrlList from './UrlList';
import UrlShortenerForm from './UrlShortenerForm';
import Navbar from './components/Navbar';

function Profile() {
  const { isAuthenticated, isLoading } = useAuth0();
  const urlListRef = useRef();

  if (!isAuthenticated) {
    return null;
  }

  if (isLoading) {
    return <div>Loading...</div>;
  }

  return (
    <div className="mt-8 p-4">
      <UrlShortenerForm onUrlCreated={() => urlListRef.current?.fetchUrls()} />
      <UrlList ref={urlListRef} />
    </div>
  );
}

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
