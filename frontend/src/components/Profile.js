import React, { useRef, useState, useEffect } from 'react';
import PageHeader from './PageHeader';
import UrlList from './UrlList';
import UrlShortenerForm from './UrlShortenerForm';

function Profile() {
  const urlListRef = useRef();
  const dialogRef = useRef();
  const [isModalOpen, setIsModalOpen] = useState(false);

  const closeModal = () => setIsModalOpen(false);

  useEffect(() => {
    if (isModalOpen && !dialogRef.current.open) dialogRef.current.showModal();
  }, [isModalOpen]);

  const handleUrlCreated = () => {
    urlListRef.current?.fetchUrls();
    closeModal();
  };

  return (
    <>
      <PageHeader title={<>Tus enlaces <em>cortos</em></>}>
        <button type="button" onClick={() => setIsModalOpen(true)} className="ex-button">
          Nuevo enlace
        </button>
      </PageHeader>

      <UrlList ref={urlListRef} />

      {isModalOpen && (
        <dialog
          ref={dialogRef}
          className="app-dialog ex-theme-light"
          aria-labelledby="new-link-title"
          onCancel={closeModal}
          onMouseDown={(e) => {
            const { left, right, top, bottom } = e.currentTarget.getBoundingClientRect();
            const outside = e.clientX < left || e.clientX > right || e.clientY < top || e.clientY > bottom;
            if (e.target === e.currentTarget && outside) closeModal();
          }}
        >
          <UrlShortenerForm titleId="new-link-title" onUrlCreated={handleUrlCreated} onClose={closeModal} />
        </dialog>
      )}
    </>
  );
}

export default Profile;
