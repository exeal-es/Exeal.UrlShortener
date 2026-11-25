import React from 'react';

function Message({ error = false, children }) {
  return (
    <div className="app-container ex-section">
      {error ? (
        <p className="ex-field__error" role="alert">{children}</p>
      ) : (
        <p className="ex-text ex-text--muted" role="status">{children}</p>
      )}
    </div>
  );
}

export default Message;
