import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from 'react-oidc-context';

// The AuthProvider exchanges the code; this page only waits and leaves.
function AuthCallback() {
  const { isLoading, error, user } = useAuth();

  if (error) return <div className="mt-8 p-4 text-red-600">Error: {error.message}</div>;
  if (isLoading) return <div className="mt-8 p-4 text-gray-600">Loading...</div>;

  return <Navigate to={user?.state?.returnTo ?? '/'} replace />;
}

export default AuthCallback;
