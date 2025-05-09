import React, { useRef } from 'react';
import { Auth0Provider, useAuth0 } from '@auth0/auth0-react';
import './App.css';
import UrlList from './UrlList';
import UrlShortenerForm from './UrlShortenerForm';

function LoginButton() {
  const { loginWithRedirect } = useAuth0();

  return (
    <button onClick={() => loginWithRedirect()}>
      Log In
    </button>
  );
}

function LogoutButton() {
  const { logout } = useAuth0();

  return (
    <button onClick={() => logout({ returnTo: window.location.origin })}>
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
      <div>
        <h2>Welcome {user.name}!</h2>
        <p>Email: {user.email}</p>
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
      <div className="App">
        <header className="App-header">
          <h1>URL Shortener</h1>
          <LoginButton />
          <Profile />
        </header>
      </div>
    </Auth0Provider>
  );
}

export default App;
