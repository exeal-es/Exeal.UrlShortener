import React, { useEffect } from 'react';
import { Outlet } from 'react-router-dom';
import { useAuth } from 'react-oidc-context';
import { useSignIn } from '../auth';
import Welcome from './Welcome';
import Message from './Message';

function RequireAuth() {
  const { isAuthenticated, isLoading, error, events } = useAuth();
  const signIn = useSignIn();

  // Fires when the session expires, also if it was already expired on load.
  useEffect(() => events.addAccessTokenExpired(signIn), [events, signIn]);

  if (isAuthenticated) return <Outlet />;
  if (isLoading) return <Message>Cargando…</Message>;
  return <Welcome error={error} />;
}

export default RequireAuth;
