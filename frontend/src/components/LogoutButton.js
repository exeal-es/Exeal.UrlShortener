import React from 'react';
import { useAuth } from 'react-oidc-context';

function LogoutButton() {
  const { signoutRedirect } = useAuth();

  return (
    <button onClick={() => signoutRedirect()} className="px-4 py-2 bg-gray-200 text-gray-800 rounded hover:bg-gray-300 font-semibold transition">
      Log Out
    </button>
  );
}

export default LogoutButton; 