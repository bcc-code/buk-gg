import Vue, { PluginObject } from 'vue';
import Vuex from 'vuex';
import { SessionState, SessionStore } from '@/store/modules/SessionStore';
import { TournamentState, TournamentStore } from './modules/TournamentStore';
import { EventState, EventStore } from './modules/EventStore';
import {
    OrganizationState,
    OrganizationStore,
} from './modules/OrganizationStore';
import { TeamStore, TeamState } from './modules/TeamStore';

// How to add a new module
// -----------------------------------------------------------
// 1. Create new Store class which inherits from BaseStore, CrudStore or similar (along with corropsonding state)
//
// 2. Add state to RootState and initialize store in RootStore class below
//
// 3. Optionally export initialized store (so that it can be imported into other modules)
//
// 4. Optionally modify /@types/vue.d.ts so that the store is strongly typed in Vue instances

Vue.use(Vuex);

export interface RootState {
    session: SessionState;
    tournaments: TournamentState;
    ievents: EventState;
    organizations: OrganizationState;
    teams: TeamState;
}

export class RootStore extends Vuex.Store<RootState>
    implements PluginObject<any> {
    // Strongly typed modules
    public session = new SessionStore(this);
    public tournaments = new TournamentStore(this);
    public ievents = new EventStore(this);
    public organizations = new OrganizationStore(this);
    public teams = new TeamStore(this);

    constructor() {
        super({ strict: true });
    }
    public install(vue: typeof Vue, options?: any) {
        // Register state globally
        vue.prototype.$state = this.state;

        // Register strongly typed modules globally
        vue.use(this.session);
        vue.use(this.tournaments);
        vue.use(this.ievents);
        vue.use(this.organizations);
        vue.use(this.teams);
    }

    // GLOBAL ACTIONS
    public ensureUserData(): Promise<any> {
        return this.session.ensureUserData();
    }

    public loadUserData(reload: boolean): Promise<any> {
        return this.session.loadUserData(reload);
    }
}

const store = new RootStore();
Vue.use(store);

export const session = store.session;
export const tournaments = store.tournaments;
export const ievents = store.ievents;
export const organizations = store.organizations;
export const teams = store.teams;

export * from './modules/EventStore';
export * from './modules/OrganizationStore';
export * from './modules/SessionStore';
export * from './modules/TeamStore';
export * from './modules/TournamentStore';

// Export Store
export default store;
