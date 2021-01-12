import api from '@/services/api';
import auth from '@/services/auth';
import router from '@/router';
import { RootStore } from '@/store';
import BaseStore from '@/store/modules/base/BaseStore';

export interface SessionState {
    currentSession?: Session;
    currentUser?: User;
    isAuthenticated: boolean;
    isImpersonating: boolean;
    userDataLoaded: boolean;
    userDataLoading: boolean;
}

export class SessionStore extends BaseStore<SessionState> {
    private userDataLoadingPromise: Promise<any> | undefined = undefined;

    constructor(rootStore: RootStore) {
        super('session', rootStore, {
            currentSession: undefined,
            currentUser: undefined,
            isAuthenticated: false,
            isImpersonating: false,
            userDataLoaded: false,
            userDataLoading: false,
        });

        // MUTATIONS //
        this.mutations = {
            ...this.mutations,
            setCurrentSession: (state, value: Session) => {
                state.currentSession = value;
                state.currentUser = value.currentUser;
                if (value) {
                    state.isAuthenticated = true;
                } else {
                    state.isAuthenticated = false;
                    state.isImpersonating = false;
                }
            },
            setCurrentUser: (state, value: User) => {
                state.currentUser = value;
            },
            clearCurrentSession: (state) => {
                state.currentSession = undefined;
                state.currentUser = undefined;
                state.isAuthenticated = false;
            },
            setIsImpersonating: (state, value: boolean) => {
                state.isImpersonating = value;
            },
            setUserDataLoading: (state, value: boolean) => {
                state.userDataLoading = value;
                if (value) {
                    state.userDataLoaded = false;
                } else {
                    state.userDataLoaded = true;
                }
            },
            setUserDataLoaded: (state, value: boolean) => {
                state.userDataLoaded = value;
            },
            removeDiscord: (state) => {
                state.currentUser.discordId = '';
                state.currentUser.discordIsConnected = false;
                state.currentUser.discordUser = '';
            },
            addDiscordUser: (state, discordUser: ExtraDiscordUser) => {
                if (!state.currentUser.moreDiscordUsers) {
                    state.currentUser.moreDiscordUsers = [];
                }
                state.currentUser.moreDiscordUsers.push(discordUser);
            },
            removeDiscordUser: (state, discordUser: ExtraDiscordUser) => {
                state.currentUser.moreDiscordUsers = state.currentUser.moreDiscordUsers?.filter((d) => d._key !== discordUser._key);
            },
            loginDiscord: (state, discordUser: any) => {
                state.currentUser.discordUser =
                    discordUser.username + '#' + discordUser.discriminator;
                state.currentUser.discordId = discordUser.id;
            },
            setDiscordConnected: (state, connected: boolean) => {
                state.currentUser.discordIsConnected = connected;
            }
        };

        // ACTIONS //
        this.actions = {
            ...this.actions,
            loadCurrentSession: async (store): Promise<Session> => {
                const session = await api.session.getCurrentSession();
                this.setCurrentSession(session);
                return session;
            },
            startSession: async (store) => {
                if (auth.isAuthenticated()) {
                    // If already authenticated - load user
                    await this.loadCurrentSession();
                    if (
                        store.state.isImpersonating !== auth.isImpersonating()
                    ) {
                        this.setIsImpersonating(auth.isImpersonating());
                    }
                    await this.loadUserData(true);
                } else {
                    // If not authenticated - attempt to complete login and then load user
                    if (
                        (await auth.completeLogin()) &&
                        auth.isAuthenticated()
                    ) {
                        await this.loadCurrentSession();
                        router.push('/');
                        await this.loadUserData(true);
                    } else {
                        // If not authenticated - redirect to login
                        await this.login();
                    }
                }
            },
            updateUser: async (store, update: Player) => {
                const user = await api.session.updateCurrentUser(update);
                this.setCurrentUser(user);
                return user;
            },
            removeDiscord: async (store) => {
                this.commit('removeDiscord');
                const user = await api.session.updateCurrentUser(this.state.currentUser);
                this.setCurrentUser(user);
                return user;
            },
            updateDiscord: async (store, result) => {
                this.commit('updateDiscord', result);
                const user = await api.session.updateCurrentUser(this.state.currentUser);
                this.setCurrentUser(user);
                return user;
            },
            loginDiscord: async (store, obj: {discordUser: any, invite: string}) => {
                api.discord.isConnected(obj.discordUser.id).then((result) => {
                    store.commit('setDiscordConnected', result);
                })
                this.commit('loginDiscord', obj.discordUser);
                const user = await api.session.updateCurrentUser(this.state.currentUser);
                this.setCurrentUser(user);
                window.location.replace(obj.invite);
                return user;
            },
            loadUserData: (store, reload: boolean) => {
                if (this.userDataLoadingPromise && !reload) {
                    return this.userDataLoadingPromise;
                }
                // Load / reload
                this.userDataLoadingPromise = Promise.all([]);
                return this.userDataLoadingPromise;
            },

            ensureUserData: (store) => {
                return this.loadUserData(false);
            },

            login: ({ commit }) => {
                this.clearCurrentSession();
                auth.startLogin();
            },

            logout: ({ commit }) => {
                auth.clearSession();
                this.clearCurrentSession();
                // router.push('/login');
            },

            impersonate: async (store, email: string) => {
                auth.impersonate(email);
                this.setIsImpersonating(true);
                router.push('/');
                await this.loadCurrentSession();
                await this.loadUserData(true);
            },
            endImpersonation: async (store) => {
                auth.endImpersonation();
                this.setIsImpersonating(false);
                this.setIsActing(false);
                router.push('/');
                await this.loadCurrentSession();
                await this.loadUserData(true);
            },

            addDiscordUser: async (store, discordUser: ExtraDiscordUser) => {
                this.commit('addDiscordUser', discordUser);
                await this.updateUser(this.state.currentUser);
            },

            removeDiscordUser: async (store, discordUser: ExtraDiscordUser) => {
                this.commit('removeDiscordUser', discordUser);
                await this.updateUser(this.state.currentUser);
            },
        };

        // GETTERS //
        this.getters = {
            ...this.getters,
            isLoggedIn: (state) =>
                state.currentUser !== null && state.currentUser !== undefined,
            currentUserEmail: (state) =>
                state.currentUser ? state.currentUser.email || '' : '',
            currentUserDisplayName: (state) =>
                state.currentUser ? state.currentUser.displayName || '' : '',
            authenticatedUserCanImpersonate: (state) => {
                return state.currentSession &&
                    (state.currentSession as Session).authenticatedUser
                    ? (state.currentSession as Session).authenticatedUser
                          .canImpersonate || false
                    : false;
            },
        };
    }

    // TYPED MUTATIONS //
    public setIsImpersonating = (value: boolean) =>
        this.commit('setIsImpersonating', value)
    public setIsActing = (value: boolean) => this.commit('setIsActing', value);
    public setUserDataLoading = (value: boolean) =>
        this.commit('setUserDataLoading', value)
    public setUserDataLoaded = (value: boolean) =>
        this.commit('setUserDataLoaded', value)
    public clearCurrentSession = () => this.commit('clearCurrentSession');
    public setCurrentSession = (session: Session) =>
        this.commit('setCurrentSession', session)
    public setCurrentUser = (user: User) => this.commit('setCurrentUser', user);

    // TYPED ACTIONS //
    public startSession = () => this.dispatch('startSession');
    public loadCurrentSession = () =>
        this.dispatch('loadCurrentSession') as Promise<Session>
    public loadUserData = (reload: boolean) =>
        this.dispatch('loadUserData', reload) as Promise<any>
    public ensureUserData = () =>
        this.dispatch('ensureUserData') as Promise<any>
    public updateUser = (update: Player) =>
        this.dispatch('updateUser', update) as Promise<User>
    public removeDiscord = () => {
        return this.dispatch('removeDiscord') as Promise<User>;
    }
    public updateDiscord = (result: any) => {
        return this.dispatch('updateDiscord', result);
    }
    public loginDiscord = (discordUser: any) => {
        return this.dispatch('loginDiscord', discordUser);
    }
    public addDiscordUser = (discordUser: ExtraDiscordUser) => this.dispatch('addDiscordUser', discordUser) as Promise<void>;
    public removeDiscordUser = (discordUser: ExtraDiscordUser) => this.dispatch('removeDiscordUser', discordUser) as Promise<void>;

    public impersonate = (email: string) => this.dispatch('impersonate', email);
    public endImpersonation = () => this.dispatch('endImpersonation');
    public login = () => this.dispatch('login');
    public logout = () => this.dispatch('logout');

    // TYPED GETTERS //
    public get isLoggedIn(): boolean {
        return this.read('isLoggedIn');
    }
    public get currentUserEmail(): string {
        return this.read('currentUserEmail');
    }
    public get currentUserDisplayName(): string {
        return this.read('currentUserDisplayName');
    }
    public get authenticatedUserCanImpersonate(): boolean {
        return this.read('authenticatedUserCanImpersonate');
    }
}
