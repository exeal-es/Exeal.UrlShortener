import React, { useState } from 'react';

function EditableField({ id, label, value, display, onSave }) {
  const [draft, setDraft] = useState(value);
  const [editing, setEditing] = useState(false);
  const [saved, setSaved] = useState(false);

  const startEditing = () => {
    setDraft(value);
    setEditing(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!(await onSave(draft))) return;
    setEditing(false);
    setSaved(true);
    setTimeout(() => setSaved(false), 2000);
  };

  if (editing) {
    return (
      <form className="app-row" onSubmit={handleSubmit}>
        <div className="ex-field">
          <label className="ex-field__label" htmlFor={id}>{label}</label>
          <input
            id={id}
            className="ex-input"
            value={draft}
            onChange={(e) => setDraft(e.target.value)}
            onKeyDown={(e) => {
              if (e.key === 'Escape') setEditing(false);
            }}
            autoFocus
          />
        </div>
        <button type="submit" className="ex-button ex-button--accent ex-button--nav">Guardar</button>
        <button type="button" className="ex-button ex-button--hollow ex-button--nav" onClick={() => setEditing(false)}>
          Cancelar
        </button>
      </form>
    );
  }

  return (
    <div className="app-row">
      <div className="ex-field">
        <span className="ex-field__label">{label}</span>
        <span>{display}</span>
      </div>
      <span role="status">{saved ? 'Guardado' : ''}</span>
      <button
        type="button"
        className="ex-button ex-button--hollow ex-button--nav"
        aria-label={`Editar ${label.toLowerCase()}`}
        onClick={startEditing}
      >
        Editar
      </button>
    </div>
  );
}

export default EditableField;
