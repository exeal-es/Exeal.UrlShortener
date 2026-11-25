import { useCallback } from 'react';
import { useAuth } from 'react-oidc-context';
import { WebStorageStateStore } from 'oidc-client-ts';

export const CALLBACK_PATH = '/callback';

const orgId = process.env.REACT_APP_OIDC_ORG_ID;

export const oidcConfig = {
  authority: process.env.REACT_APP_OIDC_AUTHORITY,
  client_id: process.env.REACT_APP_OIDC_CLIENT_ID,
  redirect_uri: `${window.location.origin}${CALLBACK_PATH}`,
  post_logout_redirect_uri: window.location.origin,
  // The org scope pins Exeal Staff: Zitadel then goes straight to its only IdP, Google.
  scope: `openid profile email urn:zitadel:iam:org:id:${orgId}`,
  userStore: new WebStorageStateStore({ store: window.localStorage }),
  automaticSilentRenew: false,
};

export function useSignIn() {
  const { signinRedirect } = useAuth();
  return useCallback(
    () => {
      const { pathname } = window.location;
      return signinRedirect({ state: { returnTo: pathname === CALLBACK_PATH ? '/' : pathname } });
    },
    [signinRedirect]
  );
}

export function useAccessToken() {
  return useAuth().user?.access_token;
}
