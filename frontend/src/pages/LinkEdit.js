import React, { useEffect, useState, useCallback } from 'react';
import { useParams, Link } from 'react-router-dom';
import { useAuth0 } from '@auth0/auth0-react';
import { fetchUrls, updateShortUrl } from '../services/urlService';
import { RedirectIcon, EditIcon, SubmitIcon, CheckIcon } from '../components/icons';

function LinkEdit() {
  const { slug } = useParams();
  const { getAccessTokenSilently } = useAuth0();
  const [url, setUrl] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [titleValue, setTitleValue] = useState('');
  const [destUrlValue, setDestUrlValue] = useState('');
  const [editingTitle, setEditingTitle] = useState(false);
  const [editingDestUrl, setEditingDestUrl] = useState(false);
  const [titleSaved, setTitleSaved] = useState(false);
  const [destUrlSaved, setDestUrlSaved] = useState(false);

  const loadUrl = useCallback(async () => {
    try {
      const token = await getAccessTokenSilently();
      const data = await fetchUrls(token);
      const match = data.find((u) => u.slug === slug);
      if (match) {
        setUrl(match);
        setTitleValue(match.title || '');
        setDestUrlValue(match.destinationUrl || '');
      }
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }, [getAccessTokenSilently, slug]);

  useEffect(() => {
    loadUrl();
  }, [loadUrl]);

  const handleSaveTitle = async () => {
    try {
      const token = await getAccessTokenSilently();
      await updateShortUrl(token, slug, { title: titleValue });
      setUrl((prev) => ({ ...prev, title: titleValue }));
      setEditingTitle(false);
      setTitleSaved(true);
      setTimeout(() => setTitleSaved(false), 2000);
    } catch (err) {
      setError(err.message);
    }
  };

  const handleSaveDestUrl = async () => {
    try {
      const token = await getAccessTokenSilently();
      await updateShortUrl(token, slug, { destinationUrl: destUrlValue });
      setUrl((prev) => ({ ...prev, destinationUrl: destUrlValue }));
      setEditingDestUrl(false);
      setDestUrlSaved(true);
      setTimeout(() => setDestUrlSaved(false), 2000);
    } catch (err) {
      setError(err.message);
    }
  };

  if (loading) return <div className="mt-8 p-4 text-gray-600">Loading...</div>;
  if (error) return <div className="mt-8 p-4 text-red-600">Error: {error}</div>;
  if (!url) return <div className="mt-8 p-4 text-gray-600">Not found.</div>;

  return (
    <div className="mt-8 p-4">
      <div className="max-w-2xl mx-auto mb-6">
        <Link to="/" className="text-sm text-gray-500 hover:text-gray-700 hover:underline">
          ← Back
        </Link>
      </div>
      <div className="max-w-2xl mx-auto">
        <ul className="list-none p-0 m-0">
          <li className="bg-white my-4 p-8 pt-6 rounded-xl shadow border border-gray-200 flex flex-col gap-2">
            <div className="flex items-center gap-2 mb-1">
              {editingTitle ? (
                <>
                  <input
                    className="font-semibold text-base text-gray-900 border border-gray-300 rounded px-2 py-0.5 flex-1"
                    value={titleValue}
                    onChange={(e) => setTitleValue(e.target.value)}
                    onKeyDown={(e) => {
                      if (e.key === 'Enter') handleSaveTitle();
                      if (e.key === 'Escape') setEditingTitle(false);
                    }}
                    autoFocus
                  />
                  <button
                    className="text-gray-400 hover:text-blue-600 transition-colors"
                    onClick={handleSaveTitle}
                    title="Save"
                  >
                    <SubmitIcon />
                  </button>
                </>
              ) : (
                <>
                  <span className="font-semibold text-base text-gray-900">{url.title || url.slug}</span>
                  {titleSaved ? (
                    <CheckIcon className="text-green-600" />
                  ) : (
                    <button
                      className="text-gray-400 hover:text-blue-600 transition-colors"
                      onClick={() => { setTitleValue(url.title || ''); setEditingTitle(true); }}
                      title="Edit title"
                    >
                      <EditIcon />
                    </button>
                  )}
                </>
              )}
            </div>

            <div className="flex items-center gap-2">
              <a
                className="text-blue-600 text-base break-all hover:underline"
                href={url.fullUrl}
                target="_blank"
                rel="noopener noreferrer"
              >
                {url.fullUrl}
              </a>
            </div>

            <div className="flex items-center gap-1.5 text-gray-500 text-sm mt-1">
              <RedirectIcon />
              {editingDestUrl ? (
                <>
                  <input
                    className="text-gray-800 text-sm border border-gray-300 rounded px-2 py-0.5 flex-1"
                    value={destUrlValue}
                    onChange={(e) => setDestUrlValue(e.target.value)}
                    onKeyDown={(e) => {
                      if (e.key === 'Enter') handleSaveDestUrl();
                      if (e.key === 'Escape') setEditingDestUrl(false);
                    }}
                    autoFocus
                  />
                  <button
                    className="text-gray-400 hover:text-blue-600 transition-colors"
                    onClick={handleSaveDestUrl}
                    title="Save"
                  >
                    <SubmitIcon />
                  </button>
                </>
              ) : (
                <>
                  <span className="text-gray-800 break-all">{url.destinationUrl}</span>
                  {destUrlSaved ? (
                    <CheckIcon className="text-green-600" />
                  ) : (
                    <button
                      className="text-gray-400 hover:text-blue-600 transition-colors"
                      onClick={() => { setDestUrlValue(url.destinationUrl || ''); setEditingDestUrl(true); }}
                      title="Edit destination URL"
                    >
                      <EditIcon />
                    </button>
                  )}
                </>
              )}
            </div>
          </li>
        </ul>
      </div>
    </div>
  );
}

export default LinkEdit;
