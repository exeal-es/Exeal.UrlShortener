import React, { useEffect } from 'react';
import { Outlet } from 'react-router-dom';
import { useAuth } from 'react-oidc-context';
import { useSignIn } from '../auth';

function RequireAuth() {
  const { isAuthenticated, isLoading, error, events } = useAuth();
  const signIn = useSignIn();

  // Fires when the session expires, also if it was already expired on load.
  useEffect(() => events.addAccessTokenExpired(signIn), [events, signIn]);

  if (isAuthenticated) return <Outlet />;
  if (isLoading) return <div className="mt-8 p-4 text-gray-600">Loading...</div>;
  if (error) return <div className="mt-8 p-4 text-red-600">Error: {error.message}</div>;
  return <div className="mt-8 p-4 text-gray-600">Log in to manage your short URLs.</div>;
}

export default RequireAuth;
