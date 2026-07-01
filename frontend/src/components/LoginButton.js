import React from 'react';
import { useAuth0 } from '@auth0/auth0-react';

function LoginButton() {
  const { loginWithRedirect } = useAuth0();

  return (
    <button onClick={() => loginWithRedirect()} className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 font-semibold transition">
      Log In
    </button>
  );
}

export default LoginButton; 