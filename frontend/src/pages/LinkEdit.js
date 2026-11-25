import React, { useEffect, useState, useCallback } from 'react';
import { useParams } from 'react-router-dom';
import { useAccessToken } from '../auth';
import { fetchUrls, updateShortUrl } from '../services/urlService';
import EditableField from '../components/EditableField';
import Message from '../components/Message';
import PageHeader from '../components/PageHeader';

function LinkEdit() {
  const { slug } = useParams();
  const token = useAccessToken();
  const [url, setUrl] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const loadUrl = useCallback(async () => {
    try {
      const data = await fetchUrls(token);
      const match = data.find((u) => u.slug === slug);
      if (match) setUrl(match);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }, [token, slug]);

  useEffect(() => {
    loadUrl();
  }, [loadUrl]);

  const save = async (fields) => {
    try {
      await updateShortUrl(token, slug, fields);
      setUrl((prev) => ({ ...prev, ...fields }));
      return true;
    } catch (err) {
      setError(err.message);
      return false;
    }
  };

  if (loading) return <Message>Cargando…</Message>;
  if (error) return <Message error>Error: {error}</Message>;
  if (!url) return <Message>No se ha encontrado el enlace.</Message>;

  return (
    <>
      <PageHeader title={url.title || url.slug} back />
      <div className="app-container ex-section">
        <div className="ex-card">
          <div className="ex-card__body">
            <p className="app-row">
              <a className="ex-link" href={url.fullUrl} target="_blank" rel="noopener noreferrer">{url.fullUrl}</a>
            </p>
            <EditableField
              id="title"
              label="Título"
              value={url.title || ''}
              display={url.title || url.slug}
              onSave={(title) => save({ title })}
            />
            <EditableField
              id="destinationUrl"
              label="URL de destino"
              value={url.destinationUrl || ''}
              display={url.destinationUrl}
              onSave={(destinationUrl) => save({ destinationUrl })}
            />
          </div>
        </div>
      </div>
    </>
  );
}

export default LinkEdit;
