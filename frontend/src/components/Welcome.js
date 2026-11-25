import React from 'react';
import LoginButton from './LoginButton';

function Welcome({ error }) {
  return (
    <section className="ex-hero ex-theme-dark ex-motif ex-motif--code">
      <div className="app-container">
        <h1 className="ex-h1">Enlaces cortos, <em>con datos</em></h1>
        <p className="ex-text">Inicia sesión para gestionar tus enlaces.</p>
        {error && <p className="ex-field__error ex-text" role="alert">Error: {error.message}</p>}
        <LoginButton />
      </div>
    </section>
  );
}

export default Welcome;
