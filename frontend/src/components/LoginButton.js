import React from 'react';
import { useSignIn } from '../auth';

function LoginButton() {
  const signIn = useSignIn();

  return (
    <button type="button" onClick={signIn} className="ex-button">
      Iniciar sesión
    </button>
  );
}

export default LoginButton;
