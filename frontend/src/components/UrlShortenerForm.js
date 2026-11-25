import React, { useState } from 'react';
import { useAccessToken } from '../auth';
import { createShortUrl } from '../services/urlService';

function UrlShortenerForm({ titleId, onUrlCreated, onClose }) {
  const [destinationUrl, setDestinationUrl] = useState('');
  const [customSlug, setCustomSlug] = useState('');
  const [title, setTitle] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const token = useAccessToken();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
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
    <form onSubmit={handleSubmit}>
      <h2 className="ex-h2" id={titleId}>Nuevo enlace corto</h2>
      <div className="ex-field">
        <label className="ex-field__label" htmlFor="destinationUrl">URL de destino</label>
        <input
          type="url"
          required
          id="destinationUrl"
          className="ex-input"
          value={destinationUrl}
          onChange={(e) => setDestinationUrl(e.target.value)}
          placeholder="https://ejemplo.com"
        />
      </div>
      <div className="ex-field">
        <label className="ex-field__label" htmlFor="title">Título (opcional)</label>
        <input
          type="text"
          id="title"
          className="ex-input"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          placeholder="Si lo dejas vacío, se muestra el slug"
        />
      </div>
      <div className="ex-field">
        <label className="ex-field__label" htmlFor="customSlug">Slug (opcional)</label>
        <input
          type="text"
          id="customSlug"
          className="ex-input"
          value={customSlug}
          onChange={(e) => setCustomSlug(e.target.value)}
          placeholder="Si lo dejas vacío, se genera solo"
        />
      </div>
      {error && <p className="ex-field__error ex-text" role="alert">{error}</p>}
      <div className="app-row app-row--end">
        {onClose && (
          <button type="button" onClick={onClose} className="ex-button ex-button--hollow ex-button--nav">
            Cancelar
          </button>
        )}
        <button type="submit" disabled={loading} className="ex-button ex-button--accent ex-button--nav">
          {loading ? 'Creando…' : 'Crear enlace'}
        </button>
      </div>
    </form>
  );
}

export default UrlShortenerForm; 