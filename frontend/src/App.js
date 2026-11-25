import React from 'react';
import { AuthProvider } from 'react-oidc-context';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import './App.css';
import { CALLBACK_PATH, oidcConfig } from './auth';
import Navbar from './components/Navbar';
import Profile from './components/Profile';
import RequireAuth from './components/RequireAuth';
import AuthCallback from './pages/AuthCallback';
import LinkDetails from './pages/LinkDetails';
import LinkEdit from './pages/LinkEdit';

function App() {
  return (
    <AuthProvider {...oidcConfig}>
      <BrowserRouter>
        <div className="min-h-screen bg-gray-50 text-gray-900 font-sans">
          <Navbar />
          <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
            <Routes>
              <Route path={CALLBACK_PATH} element={<AuthCallback />} />
              <Route element={<RequireAuth />}>
                <Route path="/" element={<Profile />} />
                <Route path="/links/:slug/details" element={<LinkDetails />} />
                <Route path="/links/:slug/edit" element={<LinkEdit />} />
              </Route>
            </Routes>
          </main>
        </div>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
