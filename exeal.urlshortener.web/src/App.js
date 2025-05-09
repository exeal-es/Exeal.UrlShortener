import React, { useRef } from 'react';
import { Auth0Provider, useAuth0 } from '@auth0/auth0-react';
// import './App.css';
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

function Profile() {
  const { user, isAuthenticated, isLoading } = useAuth0();
  const urlListRef = useRef();

  if (isLoading) {
    return <div>Loading...</div>;
  }

  return (
    isAuthenticated && (
      <div className="mt-8 p-4">
        <h2 className="text-lg font-semibold mb-4 text-gray-900">Welcome {user.name}!</h2>
        <p className="mb-2 text-gray-700">Email: {user.email}</p>
        <LogoutButton />
        <UrlShortenerForm onUrlCreated={() => urlListRef.current?.fetchUrls()} />
        <UrlList ref={urlListRef} />
      </div>
    )
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
        <header className="border-b border-gray-200 px-8 pt-8 pb-0 text-left">
          <h1 className="text-3xl font-bold mb-6">URL Shortener</h1>
          <LoginButton />
          <Profile />
        </header>
      </div>
    </Auth0Provider>
  );
}

export default App;
