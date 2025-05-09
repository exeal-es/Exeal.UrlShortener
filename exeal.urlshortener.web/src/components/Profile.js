import React, { useRef } from 'react';
import { useAuth0 } from '@auth0/auth0-react';
import UrlList from './UrlList';
import UrlShortenerForm from './UrlShortenerForm';

function Profile() {
  const { isAuthenticated, isLoading } = useAuth0();
  const urlListRef = useRef();

  if (!isAuthenticated) {
    return null;
  }

  if (isLoading) {
    return <div>Loading...</div>;
  }

  return (
    <div className="mt-8 p-4">
      <UrlShortenerForm onUrlCreated={() => urlListRef.current?.fetchUrls()} />
      <UrlList ref={urlListRef} />
    </div>
  );
}

export default Profile; 