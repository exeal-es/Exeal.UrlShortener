import React, { useEffect, useState, useCallback, forwardRef, useImperativeHandle } from 'react';
import { Link } from 'react-router-dom';
import { useAccessToken } from '../auth';
import { fetchUrls } from '../services/urlService';
import Message from './Message';
import ShortLink from './ShortLink';

const UrlList = forwardRef((props, ref) => {
  const [urls, setUrls] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const token = useAccessToken();

  const fetchUrlsData = useCallback(async () => {
    setLoading(true);
    try {
      const data = await fetchUrls(token);
      setUrls(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }, [token]);

  useImperativeHandle(ref, () => ({
    fetchUrls: fetchUrlsData
  }));

  useEffect(() => {
    fetchUrlsData();
  }, [fetchUrlsData]);

  if (loading) return <Message>Cargando enlaces…</Message>;
  if (error) return <Message error>Error: {error}</Message>;
  if (urls.length === 0) return <Message>Todavía no hay enlaces.</Message>;

  return (
    <ul className="app-container ex-section app-stack">
      {urls.map((url) => (
        <li key={url.slug} className="ex-card">
          <div className="ex-card__body">
            <h2 className="app-card-title">
              <Link to={`/links/${url.slug}/details`}>{url.title || url.slug}</Link>
            </h2>
            <ShortLink url={url.fullUrl} />
            <p className="app-destination">Lleva a {url.destinationUrl}</p>
            <p className="app-row">
              <Link to={`/links/${url.slug}/details`} className="ex-link">Estadísticas</Link>
              <Link to={`/links/${url.slug}/edit`} className="ex-link">Editar</Link>
            </p>
          </div>
        </li>
      ))}
    </ul>
  );
});

export default UrlList;
