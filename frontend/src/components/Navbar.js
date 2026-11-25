import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import { useAuth } from 'react-oidc-context';
import logo from '@exeal/design/dist/img/logo-exeal-header.svg';
import LogoutButton from './LogoutButton';

function Navbar() {
  const { user, isAuthenticated } = useAuth();
  const { pathname } = useLocation();

  return (
    <nav className="ex-navbar ex-theme-dark" aria-label="Principal">
      <Link to="/"><img className="ex-navbar__logo" src={logo} alt="Exeal" /></Link>
      {isAuthenticated && (
        <>
          <button className="ex-navbar__toggle" type="button" popoverTarget="menu">Menú</button>
          <ul className="ex-navbar__menu" id="menu" popover="auto">
            <li>
              <Link className="ex-navbar__link" to="/" aria-current={pathname === '/' ? 'page' : undefined}>
                Enlaces
              </Link>
            </li>
            <li className="app-user">
              <div>{user.profile.name}</div>
              <div>{user.profile.email}</div>
            </li>
            <li><LogoutButton /></li>
          </ul>
        </>
      )}
    </nav>
  );
}

export default Navbar;
