import React, { useRef } from 'react';
import { Auth0Provider, useAuth0 } from '@auth0/auth0-react';
import './App.css';
import UrlList from './UrlList';
import UrlShortenerForm from './UrlShortenerForm';

function LoginButton() {
  const { loginWithRedirect } = useAuth0();

  return (
    <button onClick={() => loginWithRedirect()} className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 font-semibold transition">
      Log In
    </button>
  );
}

function LogoutButton() {
  const { logout } = useAuth0();

  return (
    <button onClick={() => logout({ returnTo: window.location.origin })} className="px-4 py-2 bg-gray-200 text-gray-800 rounded hover:bg-gray-300 font-semibold transition">
      Log Out
    </button>
  );
}

function UserProfile() {
  const { user, isAuthenticated } = useAuth0();

  if (!isAuthenticated) return null;

  return (
    <div className="flex items-center gap-4">
      <div className="text-sm">
        <p className="font-semibold text-gray-900">{user.name}</p>
        <p className="text-gray-600">{user.email}</p>
      </div>
    </div>
  );
}

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

function Navbar() {
  const { isAuthenticated } = useAuth0();
  
  return (
    <nav className="bg-white shadow-sm">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between items-center h-16">
          <div className="flex-shrink-0">
            <h1 className="text-2xl font-bold text-gray-900">URL Shortener</h1>
          </div>
          <div className="flex items-center gap-4">
            <UserProfile />
            <div className="flex items-center">
              {!isAuthenticated ? <LoginButton /> : <LogoutButton />}
            </div>
          </div>
        </div>
      </div>
    </nav>
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
