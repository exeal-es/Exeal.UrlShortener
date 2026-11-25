import React from 'react';
import { Link } from 'react-router-dom';

function PageHeader({ title, back = false, children }) {
  return (
    <header className="app-header ex-theme-dark ex-motif ex-motif--waves">
      <div className="app-container">
        {back && (
          <p className="app-back">
            <Link to="/" className="ex-link">Volver a tus enlaces</Link>
          </p>
        )}
        <div className="app-header__row">
          <h1 className="ex-h1">{title}</h1>
          {children}
        </div>
      </div>
    </header>
  );
}

export default PageHeader;
