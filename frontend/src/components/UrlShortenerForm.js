import React, { useState } from 'react';
import { useAuth0 } from '@auth0/auth0-react';
import { createShortUrl } from '../services/urlService';

function UrlShortenerForm({ onUrlCreated, onClose }) {
  const [destinationUrl, setDestinationUrl] = useState('');
  const [customSlug, setCustomSlug] = useState('');
  const [title, setTitle] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const { getAccessTokenSilently } = useAuth0();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      const token = await getAccessTokenSilently();
      await createShortUrl(token, destinationUrl, customSlug, title);
      setDestinationUrl('');
      setCustomSlug('');
      setTitle('');
      if (onUrlCreated) onUrlCreated();
      if (onClose) onClose();
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="w-full max-w-xl p-8 bg-white rounded-xl shadow-lg border border-gray-200">
      <div className="flex justify-between items-center mb-6">
        <h2 className="text-lg font-semibold text-gray-900">Create New Short URL</h2>
        {onClose && (
          <button
            type="button"
            onClick={onClose}
            className="text-gray-400 hover:text-gray-600 transition text-xl leading-none"
            aria-label="Close"
          >
            ✕
          </button>
        )}
      </div>
      <form onSubmit={handleSubmit} className="flex flex-col gap-5">
        <div>
          <label htmlFor="destinationUrl" className="block font-medium text-gray-700 mb-1">Destination URL:</label>
          <input
            type="url"
            id="destinationUrl"
            value={destinationUrl}
            onChange={(e) => setDestinationUrl(e.target.value)}
            placeholder="https://example.com"
            required
            className="w-full px-4 py-3 border border-gray-300 rounded-md bg-gray-50 text-gray-900 focus:border-blue-600 focus:bg-white outline-none transition"
          />
        </div>
        <div>
          <label htmlFor="title" className="block font-medium text-gray-700 mb-1">Title (optional):</label>
          <input
            type="text"
            id="title"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Leave empty to use the slug as display name"
            className="w-full px-4 py-3 border border-gray-300 rounded-md bg-gray-50 text-gray-900 focus:border-blue-600 focus:bg-white outline-none transition"
          />
        </div>
        <div>
          <label htmlFor="customSlug" className="block font-medium text-gray-700 mb-1">Custom Slug (optional):</label>
          <input
            type="text"
            id="customSlug"
            value={customSlug}
            onChange={(e) => setCustomSlug(e.target.value)}
            placeholder="Leave empty for auto-generated slug"
            className="w-full px-4 py-3 border border-gray-300 rounded-md bg-gray-50 text-gray-900 focus:border-blue-600 focus:bg-white outline-none transition"
          />
        </div>
        {error && <div className="text-red-600 mt-2">{error}</div>}
        <div className="flex gap-3 justify-end">
          {onClose && (
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-3 border border-gray-300 text-gray-700 rounded-md font-semibold transition hover:bg-gray-50"
            >
              Cancel
            </button>
          )}
          <button
            type="submit"
            disabled={loading}
            className="px-6 py-3 bg-blue-600 text-white rounded-md font-semibold transition hover:bg-blue-700 disabled:bg-indigo-200 disabled:cursor-not-allowed"
          >
            {loading ? 'Creating...' : 'Create Short URL'}
          </button>
        </div>
      </form>
    </div>
  );
}

export default UrlShortenerForm; 