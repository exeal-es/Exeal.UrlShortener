import React, { useState } from 'react';
import { useAuth0 } from '@auth0/auth0-react';

function UrlShortenerForm({ onUrlCreated }) {
  const [destinationUrl, setDestinationUrl] = useState('');
  const [customSlug, setCustomSlug] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const { getAccessTokenSilently } = useAuth0();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      const token = await getAccessTokenSilently();
      const response = await fetch(
        `${process.env.REACT_APP_API_BASE_URL}/api/shorturl`,
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({
            destinationUrl,
            customSlug: customSlug || undefined,
          }),
        }
      );

      if (!response.ok) {
        throw new Error('Failed to create shortened URL');
      }

      const data = await response.json();
      setDestinationUrl('');
      setCustomSlug('');
      if (onUrlCreated) {
        onUrlCreated();
      }
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-xl mx-auto mt-8 p-8 bg-white rounded-xl shadow border border-gray-200">
      <h2 className="text-lg font-semibold mb-6 text-gray-900">Create New Short URL</h2>
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
        <button
          type="submit"
          disabled={loading}
          className="px-4 py-3 bg-blue-600 text-white rounded-md font-semibold transition hover:bg-blue-700 disabled:bg-indigo-200 disabled:cursor-not-allowed"
        >
          {loading ? 'Creating...' : 'Create Short URL'}
        </button>
      </form>
    </div>
  );
}

export default UrlShortenerForm; 