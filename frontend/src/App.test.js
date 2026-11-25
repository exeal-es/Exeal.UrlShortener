import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { User, UserManager } from 'oidc-client-ts';
import App from './App';

const now = () => Math.floor(Date.now() / 1000);

const user = (expiresAt, state) =>
  new User({
    access_token: 'the-access-token',
    token_type: 'Bearer',
    profile: { sub: '1', name: 'Ada Lovelace', email: 'ada@exeal.com' },
    expires_at: expiresAt,
    userState: state,
  });

const storeUser = (u) =>
  localStorage.setItem('oidc.user:https://auth.test:test-client', u.toStorageString());

beforeEach(() => {
  localStorage.clear();
  window.history.replaceState({}, '', '/');
  global.fetch = jest.fn().mockResolvedValue({ ok: true, json: async () => [] });
  jest.spyOn(UserManager.prototype, 'signinRedirect').mockResolvedValue();
});

afterEach(() => jest.restoreAllMocks());

test('signed out: offers to log in and calls no API', async () => {
  render(<App />);

  expect(await screen.findByText(/inicia sesión para gestionar/i)).toBeInTheDocument();
  expect(screen.queryByRole('button', { name: /nuevo enlace/i })).not.toBeInTheDocument();
  expect(global.fetch).not.toHaveBeenCalled();
});

test('logging in remembers the page', async () => {
  window.history.replaceState({}, '', '/links/abc/edit');
  render(<App />);
  await screen.findByText(/inicia sesión para gestionar/i);

  userEvent.click(screen.getByRole('button', { name: /iniciar sesión/i }));

  await waitFor(() =>
    expect(UserManager.prototype.signinRedirect).toHaveBeenCalledWith({ state: { returnTo: '/links/abc/edit' } })
  );
});

test('signed in: shows the user and calls the API with the access token', async () => {
  storeUser(user(now() + 3600));
  render(<App />);

  expect(await screen.findByText('ada@exeal.com')).toBeInTheDocument();
  await screen.findByText(/todavía no hay enlaces/i);
  expect(global.fetch.mock.calls[0][1].headers.Authorization).toBe('Bearer the-access-token');
});

test('after signing in, the callback goes back to the page', async () => {
  window.history.replaceState({}, '', '/callback?code=c&state=s');
  const signedIn = user(now() + 3600, { returnTo: '/links/abc/edit' });
  jest.spyOn(UserManager.prototype, 'signinCallback').mockResolvedValue(signedIn);
  render(<App />);

  await waitFor(() => expect(window.location.pathname).toBe('/links/abc/edit'));
  expect(window.location.search).toBe('');
});

test('a failed callback shows the error and offers to log in', async () => {
  window.history.replaceState({}, '', '/callback?error=access_denied&state=s');
  jest.spyOn(UserManager.prototype, 'signinCallback').mockRejectedValue(new Error('access_denied'));
  render(<App />);

  expect(await screen.findByText(/access_denied/)).toBeInTheDocument();
  userEvent.click(screen.getByRole('button', { name: /iniciar sesión/i }));
  await waitFor(() =>
    expect(UserManager.prototype.signinRedirect).toHaveBeenCalledWith({ state: { returnTo: '/' } })
  );
});

test('an expired session goes to sign in by itself', async () => {
  storeUser(user(now() - 10));
  window.history.replaceState({}, '', '/links/abc/details');
  render(<App />);

  await waitFor(
    () =>
      expect(UserManager.prototype.signinRedirect).toHaveBeenCalledWith({ state: { returnTo: '/links/abc/details' } }),
    { timeout: 4000 }
  );
});

test('creating a link sends the form and closes the dialog', async () => {
  // jsdom lacks modal dialogs.
  HTMLDialogElement.prototype.showModal = function () { this.setAttribute('open', ''); };
  storeUser(user(now() + 3600));
  render(<App />);

  userEvent.click(await screen.findByRole('button', { name: /nuevo enlace/i }));
  userEvent.type(screen.getByLabelText(/url de destino/i), 'https://exeal.com');
  userEvent.click(screen.getByRole('button', { name: /crear enlace/i }));

  await waitFor(() => expect(screen.queryByLabelText(/url de destino/i)).not.toBeInTheDocument());
  const [, request] = global.fetch.mock.calls.find(([, options]) => options.method === 'POST');
  expect(JSON.parse(request.body)).toEqual({ destinationUrl: 'https://exeal.com' });
});

test('editing the title saves it and shows it', async () => {
  const link = { slug: 'abc', title: 'Antes', fullUrl: 'https://s.test/abc', destinationUrl: 'https://exeal.com' };
  global.fetch.mockResolvedValue({ ok: true, json: async () => [link] });
  storeUser(user(now() + 3600));
  window.history.replaceState({}, '', '/links/abc/edit');
  render(<App />);

  userEvent.click(await screen.findByRole('button', { name: /editar título/i }));
  userEvent.clear(screen.getByLabelText('Título'));
  userEvent.type(screen.getByLabelText('Título'), 'Después');
  userEvent.click(screen.getByRole('button', { name: /guardar/i }));

  expect(await screen.findByRole('heading', { name: 'Después' })).toBeInTheDocument();
  const [, request] = global.fetch.mock.calls.find(([, options]) => options.method === 'PATCH');
  expect(JSON.parse(request.body)).toEqual({ title: 'Después' });
});
