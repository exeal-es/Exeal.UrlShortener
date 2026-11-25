import React from 'react';
import { useSignIn } from '../auth';

function LoginButton() {
  const signIn = useSignIn();

  return (
    <button onClick={signIn} className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 font-semibold transition">
      Log In
    </button>
  );
}

export default LoginButton; 