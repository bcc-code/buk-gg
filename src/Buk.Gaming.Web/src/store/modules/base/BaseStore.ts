import { Store, Module, MutationTree, ActionTree, GetterTree } from 'vuex';
import { RootState } from '@/store';
import _Vue, { PluginObject } from 'vue';

export default class BaseStore<TState>
    implements Module<TState, RootState>, PluginObject<any> {
    public namespaced = true;

    // Store Mutations
    public mutations: MutationTree<TState> = {};

    // Store Actions
    public actions: ActionTree<TState, RootState> = {};

    // Store Getters
    public getters: GetterTree<TState, RootState> = {};

    constructor(
        public name: string,
        protected rootStore: Store<RootState>,
        public state: TState,
    ) {}

    // Install
    public install(Vue: typeof _Vue, options?: any) {
        this.rootStore.registerModule(this.name, this);
        Vue.prototype['$' + this.name] = this;
    }

    // Typed Actions

    // Typed Getters

    // Internal Helpers //
    protected dispatch<T>(action: string, payload?: any): Promise<T> {
        return this.rootStore.dispatch(this.name + '/' + action, payload);
    }

    protected read<T>(name: string): T {
        return this.rootStore.getters[this.name + '/' + name];
    }

    protected commit(name: string, payload?: any) {
        return this.rootStore.commit(this.name + '/' + name, payload);
    }
}
