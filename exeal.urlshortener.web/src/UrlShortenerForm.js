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
    <div className="url-shortener-form">
      <h2>Create New Short URL</h2>
      <form onSubmit={handleSubmit}>
        <div>
          <label htmlFor="destinationUrl">Destination URL:</label>
          <input
            type="url"
            id="destinationUrl"
            value={destinationUrl}
            onChange={(e) => setDestinationUrl(e.target.value)}
            placeholder="https://example.com"
            required
          />
        </div>
        <div>
          <label htmlFor="customSlug">Custom Slug (optional):</label>
          <input
            type="text"
            id="customSlug"
            value={customSlug}
            onChange={(e) => setCustomSlug(e.target.value)}
            placeholder="Leave empty for auto-generated slug"
          />
        </div>
        {error && <div className="error">{error}</div>}
        <button type="submit" disabled={loading}>
          {loading ? 'Creating...' : 'Create Short URL'}
        </button>
      </form>
    </div>
  );
}

export default UrlShortenerForm; 