import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from 'react-oidc-context';
import Message from '../components/Message';
import Welcome from '../components/Welcome';

// The AuthProvider exchanges the code; this page only waits and leaves.
function AuthCallback() {
  const { isLoading, error, user } = useAuth();

  if (error) return <Welcome error={error} />;
  if (isLoading) return <Message>Cargando…</Message>;

  return <Navigate to={user?.state?.returnTo ?? '/'} replace />;
}

export default AuthCallback;
