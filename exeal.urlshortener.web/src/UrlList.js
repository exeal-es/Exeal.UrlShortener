import { useEffect, useState } from 'react';
import { useAuth0 } from '@auth0/auth0-react';

function UrlList() {
  const [urls, setUrls] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const { getAccessTokenSilently } = useAuth0();

  useEffect(() => {
    const fetchUrls = async () => {
      try {
        const token = await getAccessTokenSilently();
        const response = await fetch(
          `${process.env.REACT_APP_API_BASE_URL}/api/shorturl?skip=0&take=10`,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );

        if (!response.ok) {
          throw new Error('Failed to fetch URLs');
        }

        const data = await response.json();
        setUrls(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchUrls();
  }, [getAccessTokenSilently]);

  if (loading) return <div>Loading URLs...</div>;
  if (error) return <div>Error: {error}</div>;

  return (
    <div className="url-list">
      <h2>Your Shortened URLs</h2>
      {urls.length === 0 ? (
        <p>No URLs found</p>
      ) : (
        <ul>
          {urls.map((url) => (
            <li key={url.slug}>
              <div>
                <strong>Slug:</strong> {url.slug}
              </div>
              <div>
                <strong>Destination:</strong>{' '}
                <a href={url.destinationUrl} target="_blank" rel="noopener noreferrer">
                  {url.destinationUrl}
                </a>
              </div>
              <div>
                <strong>Created:</strong>{' '}
                {new Date(url.createdAt).toLocaleString()}
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}

export default UrlList; 