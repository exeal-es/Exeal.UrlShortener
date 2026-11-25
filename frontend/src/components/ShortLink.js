import React, { useState } from 'react';

function ShortLink({ url }) {
  const [copied, setCopied] = useState(false);

  const handleCopy = () => {
    navigator.clipboard.writeText(url);
    setCopied(true);
    setTimeout(() => setCopied(false), 3000);
  };

  return (
    <p className="app-row">
      <a className="ex-link" href={url} target="_blank" rel="noopener noreferrer">{url}</a>
      <button
        type="button"
        className="ex-button ex-button--hollow ex-button--nav"
        aria-label={copied ? undefined : `Copiar ${url}`}
        onClick={handleCopy}
      >
        {copied ? 'Copiado' : 'Copiar'}
      </button>
    </p>
  );
}

export default ShortLink;
