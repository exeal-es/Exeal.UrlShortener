import React, { useEffect, useState, useCallback, forwardRef, useImperativeHandle } from 'react';
import { useAuth0 } from '@auth0/auth0-react';
import { fetchUrls } from '../services/urlService';

const UrlList = forwardRef((props, ref) => {
  const [urls, setUrls] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const { getAccessTokenSilently } = useAuth0();

  const fetchUrlsData = useCallback(async () => {
    setLoading(true);
    try {
      const token = await getAccessTokenSilently();
      const data = await fetchUrls(token);
      setUrls(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }, [getAccessTokenSilently]);

  useImperativeHandle(ref, () => ({
    fetchUrls: fetchUrlsData
  }));

  useEffect(() => {
    fetchUrlsData();
  }, [fetchUrlsData]);

  const handleCopy = (text) => {
    navigator.clipboard.writeText(text);
  };

  if (loading) return <div>Loading URLs...</div>;
  if (error) return <div>Error: {error}</div>;

  return (
    <div className="max-w-2xl mx-auto mt-8 p-0">
      <h2 className="text-lg font-semibold mb-4 text-gray-900">Your Shortened URLs</h2>
      {urls.length === 0 ? (
        <p className="text-gray-600">No URLs found</p>
      ) : (
        <ul className="list-none p-0 m-0">
          {urls.map((url) => (
            <li key={url.slug} className="bg-white my-4 p-8 pt-6 rounded-xl shadow border border-gray-200 flex flex-col gap-2 relative">
              <div className="font-semibold text-base text-gray-900 mb-1">{url.slug}</div>
              <a
                className="text-blue-600 text-base break-all hover:underline"
                href={`${window.location.origin}/${url.slug}`}
                target="_blank"
                rel="noopener noreferrer"
              >
                {window.location.origin}/{url.slug}
              </a>
              <div className="flex gap-5 items-center text-gray-500 text-sm mt-1">
                <span className="flex items-center gap-1">
                  <svg width="16" height="16" fill="#6b7280" className="mr-0.5" viewBox="0 0 16 16"><path d="M2 8a6 6 0 1112 0A6 6 0 012 8zm6-4.5a.75.75 0 01.75.75v2.25h1.5a.75.75 0 010 1.5h-2.25A.75.75 0 017 7.25V4.25A.75.75 0 018 3.5z"></path></svg>
                  {new Date(url.createdAt).toLocaleDateString()}
                </span>
                <span className="flex items-center gap-1 text-blue-600">
                  <svg width="16" height="16" fill="#2563eb" className="mr-0.5" viewBox="0 0 16 16"><path d="M7.5 1a.5.5 0 01.5.5V3a.5.5 0 01-1 0V1.5A.5.5 0 017.5 1zm4.95 2.05a.5.5 0 01.7.7l-1.06 1.06a.5.5 0 11-.7-.7l1.06-1.06zM15 7.5a.5.5 0 01-.5.5H13a.5.5 0 010-1h1.5a.5.5 0 01.5.5zm-2.05 4.95a.5.5 0 01-.7.7l-1.06-1.06a.5.5 0 11.7-.7l1.06 1.06zM8.5 15a.5.5 0 01-.5-.5V13a.5.5 0 011 0v1.5a.5.5 0 01-.5.5zm-4.95-2.05a.5.5 0 01-.7-.7l1.06-1.06a.5.5 0 11.7.7l-1.06 1.06zM1 8.5a.5.5 0 01.5-.5H3a.5.5 0 010 1H1.5a.5.5 0 01-.5-.5zm2.05-4.95a.5.5 0 01.7-.7l1.06 1.06a.5.5 0 11-.7.7L3.05 3.55z"></path></svg>
                  {url.destinationUrl}
                </span>
              </div>
              <div className="absolute top-6 right-8 flex gap-2">
                <button
                  title="Copy short URL"
                  onClick={() => handleCopy(`${window.location.origin}/${url.slug}`)}
                  className="bg-gray-100 text-gray-800 border border-gray-200 rounded px-3 py-1.5 text-sm font-medium cursor-pointer hover:bg-gray-200 transition"
                >
                  Copiar
                </button>
                <button
                  title="Copy destination URL"
                  onClick={() => handleCopy(url.destinationUrl)}
                  className="bg-gray-100 text-gray-800 border border-gray-200 rounded px-3 py-1.5 text-sm font-medium cursor-pointer hover:bg-gray-200 transition"
                >
                  Copiar destino
                </button>
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
});

export default UrlList; 