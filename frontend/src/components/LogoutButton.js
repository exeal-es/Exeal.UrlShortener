import React from 'react';
import { useAuth } from 'react-oidc-context';

function LogoutButton() {
  const { signoutRedirect } = useAuth();

  return (
    <button type="button" onClick={() => signoutRedirect()} className="ex-button ex-button--hollow ex-button--nav">
      Cerrar sesión
    </button>
  );
}

export default LogoutButton;
