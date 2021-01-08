import * as auth0 from 'auth0-js';
import config from '@/config';

export class AuthService {
    private auth: auth0.WebAuth;
    private loginStarted: boolean = false;
    private authExpiresAt: number | null = null;
    private authAccessToken: string | null = null;
    private authIdToken: string | null = null;

    constructor() {
        this.auth = new auth0.WebAuth({
            domain: config.authentication.domain,
            clientID: config.authentication.clientID,
            redirectUri:
                window.location.origin + config.authentication.redirectUri,
            audience: config.authentication.audience,
            responseType: 'token id_token',
            scope: 'openid email profile church',
        });
    }

    /**
     * Get access token from localStorage.
     *
     * @return { String }
     */
    get accessToken() {
        const expires =
            this.authExpiresAt ||
            parseInt(window.localStorage.getItem('auth_expires_at') || '0', 10);
        if (expires > Date.now()) {
            const accessToken =
                this.authAccessToken ||
                window.localStorage.getItem('auth_access_token');
            if (accessToken) {
                return accessToken;
            }
        }
        return null;
    }

    public startLogin() {
        if (!this.loginStarted) {
            this.loginStarted = true;
            this.clearSession();
            this.auth.authorize();
        }
    }

    public async completeLogin() {
        this.clearSession();
        return new Promise((resolve, reject) => {
            this.auth.parseHash((err, authResult) => {
                if (
                    authResult &&
                    authResult.accessToken &&
                    authResult.idToken
                ) {
                    this.setSession(authResult);
                    return resolve(true);
                } else {
                    this.clearSession();
                    return resolve(false);
                }
            });
        });
    }

    public setSession(authResult: auth0.Auth0DecodedHash) {
        // Set the time that the access token will expire at
        const expiresAt = JSON.stringify(
            (authResult.expiresIn || 0) * 1000 + new Date().getTime(),
        );
        this.authAccessToken = authResult.accessToken || '';
        this.authIdToken = authResult.idToken || '';
        this.authExpiresAt = parseInt(expiresAt, 10);
        window.localStorage.setItem(
            'auth_access_token',
            authResult.accessToken || '',
        );
        window.localStorage.setItem('auth_id_token', authResult.idToken || '');
        window.localStorage.setItem('auth_expires_at', expiresAt);
        // this.authNotifier.emit('authChange', { authenticated: true })
    }

    public clearSession() {
        // Clear access token and ID token from local storage
        this.authAccessToken = null;
        this.authIdToken = null;
        this.authExpiresAt = 0;

        window.localStorage.removeItem('access_token');
        window.localStorage.removeItem('id_token');
        window.localStorage.removeItem('expires_at');
        window.localStorage.removeItem('auth_access_token');
        window.localStorage.removeItem('auth_id_token');
        window.localStorage.removeItem('auth_expires_at');
    }

    public isAuthenticated() {
        if (!this.accessToken) {
            return false;
        }
        // Check whether the current time is past the
        // access token's expiry time
        const expiresAt =
            this.authExpiresAt ||
            JSON.parse(window.localStorage.getItem('auth_expires_at') || '');
        const authenticated = new Date().getTime() < expiresAt;
        return authenticated;
    }

    public impersonate(email: string) {
        window.localStorage.setItem('impersonation_email', email);
    }

    public endImpersonation() {
        window.localStorage.removeItem('impersonation_email');
    }

    public isImpersonating(): boolean {
        return localStorage.getItem('impersonation_email') !== null;
    }

    public get impersonationHeader(): string | null {
        return localStorage.getItem('impersonation_email');
    }
}

export const auth = new AuthService();
export default auth;
