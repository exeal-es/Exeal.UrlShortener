import React, { useEffect, useState, useCallback, forwardRef, useImperativeHandle } from 'react';
import { Link } from 'react-router-dom';
import { useAuth0 } from '@auth0/auth0-react';
import { fetchUrls } from '../services/urlService';
import { BarChartIcon, EditIcon, CopyIcon, CheckIcon, RedirectIcon } from './icons';

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

  const [copiedSlug, setCopiedSlug] = useState(null);

  const handleCopy = (text, slug) => {
    navigator.clipboard.writeText(text);
    setCopiedSlug(slug);
    setTimeout(() => setCopiedSlug(null), 3000);
  };

  if (loading) return <div>Loading URLs...</div>;
  if (error) return <div>Error: {error}</div>;

  return (
    <div className="max-w-2xl mx-auto mt-0 p-0">
      {urls.length === 0 ? (
        <p className="text-gray-600">No URLs found</p>
      ) : (
        <ul className="list-none p-0 m-0">
          {urls.map((url) => (
            <li key={url.slug} className="bg-white my-4 p-8 pt-6 rounded-xl shadow border border-gray-200 flex flex-col gap-2">
              <div className="flex items-center justify-between mb-1">
                <Link
                  to={`/links/${url.slug}/details`}
                  className="font-semibold text-base text-gray-900 hover:underline"
                >
                  {url.title || url.slug}
                </Link>
                <div className="flex items-center gap-2 ml-2 shrink-0">
                  <Link
                    to={`/links/${url.slug}/details`}
                    title="Details"
                    className="text-gray-400 hover:text-blue-600 transition"
                  >
                    <BarChartIcon />
                  </Link>
                  <Link
                    to={`/links/${url.slug}/edit`}
                    title="Edit"
                    className="text-gray-400 hover:text-blue-600 transition"
                  >
                    <EditIcon />
                  </Link>
                </div>
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
                <button
                  title="Copy short URL"
                  onClick={() => handleCopy(url.fullUrl, url.slug)}
                  className={`shrink-0 transition ${copiedSlug === url.slug ? 'text-green-500' : 'text-gray-400 hover:text-blue-600'}`}
                >
                  {copiedSlug === url.slug ? <CheckIcon /> : <CopyIcon />}
                </button>
              </div>
              <div className="flex items-center gap-1.5 text-gray-500 text-sm mt-1">
                <RedirectIcon />
                <span className="text-gray-800 break-all">{url.destinationUrl}</span>
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
});

export default UrlList;
