// Ref: https://github.com/github/fetch
//      https://github.com/matthew-andrews/isomorphic-fetch

import config from '@/config';
import fetch from 'isomorphic-fetch';
import { interpolate, getParameters } from '@/lib';
import auth from '@/services/auth';

class Http {
    public validateResponse(response: any) {
        return new Promise((resolve, reject) => {
            if (response.status >= 200 && response.status < 300) {
                resolve(response);
            } else if (response.status === 401) {
                setTimeout(() => {
                    if (!auth.isAuthenticated()) {
                        auth.startLogin();
                    }
                }, 10000);
                // auth.startLogin();
                // resolve(null);
                reject(new Error('Unauthorized'));
            } else {
                const error = new Error(response.statusText);
                // error.response = response;
                reject(error);
            }
        });
    }

    public validateRequest(path: string, options?: object) {
        return new Promise((resolve, reject) => {
            resolve();
            // if (Vue.$auth.isAuthenticated()) {
            //   resolve()
            // } else {
            //   // debugger
            //   Vue.$auth.login()
            //   reject(new Error('Not logged in'))
            // }
        });
    }

    public parseJson(response: any) {
        return response.json();
    }

    /** Returns path interpolated with values from content and / or query */
    public getPathAndQuery(path: string, query?: any, content?: any) {
        if (!query && !content) {
            return path;
        }
        // Find which paramters are used in path
        const pathParams = getParameters(path);
        const queryParams = Object.assign({}, query);
        // tslint:disable-next-line:prefer-for-of
        for (let i = 0; i < pathParams.length; i++) {
            delete queryParams[pathParams[i]];
        }
        // Interpolate path with paramters both from content and query
        path = interpolate(path, Object.assign({}, content || {}, query || {}));

        // Add additional query parameters
        if (Object.keys(queryParams).length > 0) {
            path += '?';
            for (const key of Object.keys(queryParams)) {
                path += `${key}=${encodeURIComponent(queryParams[key] || '')}&`;
            }
            // remove last &
            path = path.substr(0, path.length - 1);
        }
        return path;
    }

    /**
     * API GET request
     *
     * @param  {String} path
     * @param  {Object} query (optional)
     * @param  {Object} options (optional)
     * @return {Promise}
     */
    public get<T>(path: string, query?: any, options?: any): Promise<T> {
        query = Object.assign({ r: new Date().getTime() }, query || {});
        return this.apifetch(
            this.getPathAndQuery(path, query),
            Object.assign(
                {
                    method: 'GET',
                },
                options || {},
            ),
        );
    }

    /**
     * API POST request
     *
     * @param  {String} path
     * @param  {Object} content
     * @param  {Object} query (optional)
     * @param  {Object} options (optional)
     * @return {Promise}
     */
    public post<T>(
        path: string,
        content: any,
        query?: object,
        options?: object,
    ): Promise<T> {
        if (!options) {
            options = query;
        }
        return this.apifetch(
            this.getPathAndQuery(path, query, content),
            Object.assign(
                {
                    method: 'POST',
                    body: JSON.stringify(content),
                },
                options || {},
            ),
        );
    }

    /**
     * API DELETE request
     *
     * @param  {String} path
     * @param  {Object} query (optional)
     * @param  {Object} options (optional)
     * @return {Promise}
     */
    public ['delete']<T>(
        path: string,
        query?: object,
        options?: object,
    ): Promise<T> {
        return this.apifetch(
            this.getPathAndQuery(path, query),
            Object.assign(
                {
                    method: 'DELETE',
                },
                options || {},
            ),
        );
    }

    /**
     * API PUT request
     *
     * @param  {String} path
     * @param  {Object} content
     * @param  {Object} query (optional)
     * @param  {Object} options (optional)
     * @return {Promise}
     */
    public put<T>(
        path: string,
        content: any,
        query?: object,
        options?: object,
    ): Promise<T> {
        if (!options) {
            options = query;
        }
        return this.apifetch(
            this.getPathAndQuery(path, query, content),
            Object.assign(
                {
                    method: 'PUT',
                    body: JSON.stringify(content),
                },
                options || {},
            ),
        );
    }

    public apifetch(path: string, options: any) {
        return this.validateRequest(path, options)
            .then(() => {
                path = `${config.api.basePath}/api/${path}`;
                options = options || {};
                options.headers = options.headers || {};
                options.headers.Accept =
                    options.headers.Accept || 'application/json';
                options.headers['Content-Type'] =
                    options.headers['Content-Type'] || 'application/json';
                options.headers.Authorization = `Bearer ${auth.accessToken}`;
                options.headers['Accept-Language'] =
                    options.headers['Accept-Language'] ||
                    localStorage.getItem('lang') ||
                    'no';
                if (auth.impersonationHeader) {
                    options.headers['X-Impersonate-Email'] =
                        auth.impersonationHeader;
                }
                return fetch(path, options);
            })
            .then(this.validateResponse)
            .then(this.parseJson);
    }
}

const http = new Http();
export default http;
