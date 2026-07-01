import React from 'react';
import { useAuth0 } from '@auth0/auth0-react';
import LoginButton from './LoginButton';
import LogoutButton from './LogoutButton';

function UserProfile() {
  const { user, isAuthenticated } = useAuth0();

  if (!isAuthenticated) return null;

  return (
    <div className="flex items-center gap-4">
      <div className="text-sm">
        <p className="font-semibold text-gray-900">{user.name}</p>
        <p className="text-gray-600">{user.email}</p>
      </div>
    </div>
  );
}

function Navbar() {
  const { isAuthenticated } = useAuth0();
  
  return (
    <nav className="bg-white shadow-sm">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between items-center h-16">
          <div className="flex-shrink-0">
            <h1 className="text-2xl font-bold text-gray-900">URL Shortener</h1>
          </div>
          <div className="flex items-center gap-4">
            <UserProfile />
            <div className="flex items-center">
              {!isAuthenticated ? <LoginButton /> : <LogoutButton />}
            </div>
          </div>
        </div>
      </div>
    </nav>
  );
}

export default Navbar; 