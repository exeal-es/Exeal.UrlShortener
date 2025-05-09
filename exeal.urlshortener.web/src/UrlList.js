import React, { useEffect, useState, useCallback, forwardRef, useImperativeHandle } from 'react';
import { useAuth0 } from '@auth0/auth0-react';

const UrlList = forwardRef((props, ref) => {
  const [urls, setUrls] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const { getAccessTokenSilently } = useAuth0();

  const fetchUrls = useCallback(async () => {
    setLoading(true);
    try {
      const token = await getAccessTokenSilently();
      const response = await fetch(
        `${process.env.REACT_APP_API_BASE_URL}/api/shorturl?skip=0&take=10`,
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      if (!response.ok) {
        throw new Error('Failed to fetch URLs');
      }

      const data = await response.json();
      setUrls(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }, [getAccessTokenSilently]);

  useImperativeHandle(ref, () => ({
    fetchUrls
  }));

  useEffect(() => {
    fetchUrls();
  }, [fetchUrls]);

  const handleCopy = (text) => {
    navigator.clipboard.writeText(text);
  };

  if (loading) return <div>Loading URLs...</div>;
  if (error) return <div>Error: {error}</div>;

  return (
    <div className="url-list">
      <h2>Your Shortened URLs</h2>
      {urls.length === 0 ? (
        <p>No URLs found</p>
      ) : (
        <ul>
          {urls.map((url) => (
            <li key={url.slug}>
              <div className="url-title">{url.slug}</div>
              <a
                className="url-link"
                href={`${window.location.origin}/${url.slug}`}
                target="_blank"
                rel="noopener noreferrer"
              >
                {window.location.origin}/{url.slug}
              </a>
              <div className="url-meta">
                <span>
                  <svg width="16" height="16" fill="#6b7280" style={{marginRight: 2}} viewBox="0 0 16 16"><path d="M2 8a6 6 0 1112 0A6 6 0 012 8zm6-4.5a.75.75 0 01.75.75v2.25h1.5a.75.75 0 010 1.5h-2.25A.75.75 0 017 7.25V4.25A.75.75 0 018 3.5z"></path></svg>
                  {new Date(url.createdAt).toLocaleDateString()}
                </span>
                <span style={{color: '#2563eb'}}>
                  <svg width="16" height="16" fill="#2563eb" style={{marginRight: 2}} viewBox="0 0 16 16"><path d="M7.5 1a.5.5 0 01.5.5V3a.5.5 0 01-1 0V1.5A.5.5 0 017.5 1zm4.95 2.05a.5.5 0 01.7.7l-1.06 1.06a.5.5 0 11-.7-.7l1.06-1.06zM15 7.5a.5.5 0 01-.5.5H13a.5.5 0 010-1h1.5a.5.5 0 01.5.5zm-2.05 4.95a.5.5 0 01-.7.7l-1.06-1.06a.5.5 0 11.7-.7l1.06 1.06zM8.5 15a.5.5 0 01-.5-.5V13a.5.5 0 011 0v1.5a.5.5 0 01-.5.5zm-4.95-2.05a.5.5 0 01-.7-.7l1.06-1.06a.5.5 0 11.7.7l-1.06 1.06zM1 8.5a.5.5 0 01.5-.5H3a.5.5 0 010 1H1.5a.5.5 0 01-.5-.5zm2.05-4.95a.5.5 0 01.7-.7l1.06 1.06a.5.5 0 11-.7.7L3.05 3.55z"></path></svg>
                  {url.destinationUrl}
                </span>
              </div>
              <div className="url-actions">
                <button title="Copy short URL" onClick={() => handleCopy(`${window.location.origin}/${url.slug}`)} style={{background: '#f3f4f6', color: '#222', border: '1px solid #e5e7eb', borderRadius: 6, padding: '6px 12px', fontSize: 14, fontWeight: 500, cursor: 'pointer'}}>Copiar</button>
                <button title="Copy destination URL" onClick={() => handleCopy(url.destinationUrl)} style={{background: '#f3f4f6', color: '#222', border: '1px solid #e5e7eb', borderRadius: 6, padding: '6px 12px', fontSize: 14, fontWeight: 500, cursor: 'pointer'}}>Copiar destino</button>
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
});

export default UrlList; 