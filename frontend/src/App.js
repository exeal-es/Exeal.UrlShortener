import React from 'react';
import { AuthProvider } from 'react-oidc-context';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
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
        <div className="app ex-type ex-theme-gray">
          <Navbar />
          <main>
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
