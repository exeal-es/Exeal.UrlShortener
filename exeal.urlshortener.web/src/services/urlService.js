export async function createShortUrl(token, destinationUrl, customSlug) {
  const response = await fetch(
    `${process.env.REACT_APP_API_BASE_URL}/api/shorturl`,
    {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({
        destinationUrl,
        customSlug: customSlug || undefined,
      }),
    }
  );

  if (!response.ok) {
    throw new Error('Failed to create shortened URL');
  }

  return response.json();
}

export async function fetchUrls(token) {
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

  return response.json();
} 