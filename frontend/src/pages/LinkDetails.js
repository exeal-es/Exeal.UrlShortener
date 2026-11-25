import React, { useEffect, useState, useCallback, useRef } from 'react';
import { useParams } from 'react-router-dom';
import { useAccessToken } from '../auth';
import { QRCodeSVG } from 'qrcode.react';
import { fetchUrlStats } from '../services/urlService';
import Message from '../components/Message';
import PageHeader from '../components/PageHeader';
import ShortLink from '../components/ShortLink';

function LinkDetails() {
  const { slug } = useParams();
  const token = useAccessToken();
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const qrRef = useRef(null);

  const loadStats = useCallback(async () => {
    try {
      const data = await fetchUrlStats(token, slug);
      setStats(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }, [token, slug]);

  useEffect(() => {
    loadStats();
  }, [loadStats]);

  const downloadSVG = () => {
    const svg = qrRef.current.querySelector('svg');
    const svgData = new XMLSerializer().serializeToString(svg);
    const blob = new Blob([svgData], { type: 'image/svg+xml' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `${slug}-qr.svg`;
    a.click();
    URL.revokeObjectURL(url);
  };

  const downloadPNG = () => {
    const svg = qrRef.current.querySelector('svg');
    const svgData = new XMLSerializer().serializeToString(svg);
    const blob = new Blob([svgData], { type: 'image/svg+xml' });
    const url = URL.createObjectURL(blob);
    const canvas = document.createElement('canvas');
    canvas.width = 160;
    canvas.height = 160;
    const ctx = canvas.getContext('2d');
    const img = new Image();
    img.onload = () => {
      ctx.drawImage(img, 0, 0);
      URL.revokeObjectURL(url);
      const a = document.createElement('a');
      a.href = canvas.toDataURL('image/png');
      a.download = `${slug}-qr.png`;
      a.click();
    };
    img.src = url;
  };

  if (loading) return <Message>Cargando…</Message>;
  if (error) return <Message error>Error: {error}</Message>;
  if (!stats) return <Message>No se ha encontrado el enlace.</Message>;

  return (
    <>
      <PageHeader title={stats.title || stats.slug} back />
      <div className="app-container ex-section app-stack">
        <section className="ex-card" aria-label="Enlace">
          <div className="ex-card__body">
            <ShortLink url={stats.fullUrl} />
            <p className="app-destination">Lleva a {stats.destinationUrl}</p>
          </div>
        </section>

        <section className="ex-card">
          <div className="ex-card__body">
            <h2 className="ex-arrow-title">Estadísticas</h2>
            <div className="ex-stats">
              <p className="ex-stat"><b>{stats.clickCount}</b><span>Clics totales</span></p>
              <p className="ex-stat"><b>{stats.uniqueVisitorCount}</b><span>Visitantes únicos</span></p>
            </div>
          </div>
        </section>

        <section className="ex-card">
          <div className="ex-card__body">
            <h2 className="ex-arrow-title">Código QR</h2>
            <div ref={qrRef} className="app-qr">
              <QRCodeSVG value={stats.fullUrl} size={160} fgColor="#182935" />
            </div>
            <div className="app-row">
              <button type="button" onClick={downloadSVG} className="ex-button ex-button--hollow ex-button--nav">
                Descargar SVG
              </button>
              <button type="button" onClick={downloadPNG} className="ex-button ex-button--hollow ex-button--nav">
                Descargar PNG
              </button>
            </div>
          </div>
        </section>
      </div>
    </>
  );
}

export default LinkDetails;
