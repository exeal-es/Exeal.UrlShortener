const API_BASE_URL = process.env.REACT_APP_API_BASE_URL;

export async function createShortUrl(token, destinationUrl, customSlug, title) {
  const response = await fetch(
    `${API_BASE_URL}/api/shorturl`,
    {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({
        destinationUrl,
        customSlug: customSlug || undefined,
        title: title || undefined,
      }),
    }
  );

  if (!response.ok) {
    throw new Error('Failed to create shortened URL');
  }

  return response.json();
}

export async function fetchUrlStats(token, slug) {
  const response = await fetch(
    `${API_BASE_URL}/api/shorturl/${slug}/stats`,
    {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    }
  );

  if (!response.ok) {
    throw new Error('Failed to fetch URL stats');
  }

  return response.json();
}

export async function updateShortUrl(token, slug, fields) {
  const response = await fetch(
    `${API_BASE_URL}/api/shorturl/${slug}`,
    {
      method: 'PATCH',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(fields),
    }
  );

  if (!response.ok) {
    throw new Error('Failed to update URL');
  }
}

export async function fetchUrls(token) {
  const response = await fetch(
    `${API_BASE_URL}/api/shorturl?skip=0&take=10`,
    {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    }
  );

  if (!response.ok) {
    throw new Error('Failed to fetch URLs');
  }

  return response.json();
} 