import React, { useRef, useState, useEffect } from 'react';
import { useAuth0 } from '@auth0/auth0-react';
import UrlList from './UrlList';
import UrlShortenerForm from './UrlShortenerForm';

function Profile() {
  const { isAuthenticated, isLoading } = useAuth0();
  const urlListRef = useRef();
  const [isModalOpen, setIsModalOpen] = useState(false);

  useEffect(() => {
    const handleKeyDown = (e) => {
      if (e.key === 'Escape') setIsModalOpen(false);
    };
    if (isModalOpen) document.addEventListener('keydown', handleKeyDown);
    return () => document.removeEventListener('keydown', handleKeyDown);
  }, [isModalOpen]);

  if (!isAuthenticated) return null;
  if (isLoading) return <div>Loading...</div>;

  const handleUrlCreated = () => {
    urlListRef.current?.fetchUrls();
    setIsModalOpen(false);
  };

  return (
    <div className="mt-8 p-4">
      <div className="max-w-2xl mx-auto flex justify-between items-center mb-6">
        <h2 className="text-xl font-semibold text-gray-900">Your Shortened URLs</h2>
        <button
          onClick={() => setIsModalOpen(true)}
          className="px-4 py-2 bg-blue-600 text-white rounded-md font-semibold transition hover:bg-blue-700"
        >
          + New Short URL
        </button>
      </div>

      <UrlList ref={urlListRef} />

      {isModalOpen && (
        <div
          className="fixed inset-0 bg-black bg-opacity-50 z-50 flex items-center justify-center p-4"
          onClick={() => setIsModalOpen(false)}
        >
          <div onClick={(e) => e.stopPropagation()}>
            <UrlShortenerForm
              onUrlCreated={handleUrlCreated}
              onClose={() => setIsModalOpen(false)}
            />
          </div>
        </div>
      )}
    </div>
  );
}

export default Profile; 