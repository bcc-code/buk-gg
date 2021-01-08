import {
    Store,
    Module,
    MutationTree,
    ActionTree,
    GetterTree,
    ActionContext,
} from 'vuex';
import { RootState } from '@/store';
import BaseStore from '@/store/modules/base/BaseStore';
import Vue from 'vue';

export interface BaseEntity {
    [key: string]: any;
}

export interface CrudState<TEntity extends BaseEntity, TId> {
    all: TEntity[];
    currentId?: TId;
    item: object;
}

export default abstract class CrudStore<
    TEntity extends BaseEntity,
    TState extends CrudState<TEntity, TId>,
    TId
> extends BaseStore<TState> {
    constructor(name: string, store: Store<RootState>, initialState: TState) {
        super(name, store, initialState);

        // MUTATIONS //
        this.mutations = {
            ...this.mutations,
            setAll: (state, items?: TEntity[]) => {
                state.all = items || [];
            },
            setCurrent: (state, itemId: TId) => {
                state.currentId = itemId;
            },
            clearAll: (state) => {
                state.all = [];
                state.currentId = undefined;
            },
            updateItem: (state, item: TEntity) => {
                state.all = state.all || [];
                const itemId = this.getItemId(item);
                for (let i = 0; i < state.all.length; i++) {
                    if (this.getItemId(state.all[i]) === itemId) {
                        Vue.set(state.all, i, item);
                        return;
                    }
                }
                state.all.push(item);
            },
            patchItem: (state, patch: object) => {
                state.all = state.all || [];
                const itemId = this.getItemId(patch);
                for (let i = 0; i < state.all.length; i++) {
                    const item = state.all[i];
                    if (this.getItemId(item) === itemId) {
                        Vue.set(state.all, i, Object.assign(item, patch));
                        return;
                    }
                }
            },
            removeItem: (state, itemId: TId) => {
                state.all = state.all || [];
                for (let i = 0; i < state.all.length; i++) {
                    if (this.getItemId(state.all[i]) === itemId) {
                        state.all.splice(i, 1);
                        if (state.currentId === itemId) {
                            state.currentId = undefined;
                        }
                        return;
                    }
                }
            },
        };

        // ACTIONS //
        this.actions = {
            ...this.actions,
            loadAll: async (state) => {
                const items = await this.loadAllFromSource();
                this.setAll(items);
            },
        };

        // GETTERS //
        this.getters = {
            ...this.getters,
            current: (state) => {
                if (state.currentId) {
                    return state.all.find(
                        (x) => this.getItemId(x) === state.currentId,
                    );
                }
                return undefined;
            },
            currentId: (state) => state.currentId,
        };
    }

    // TYPED GETTERS //
    public get current(): TEntity | undefined {
        return this.read('current');
    }
    public get currentId(): TId | undefined {
        return this.read('currentId');
    }

    // TYPED MUTATIONS //
    public setAll = (items?: TEntity[]) => this.commit('setAll', items);
    public clearAll = () => this.commit('clearAll');
    public setCurrent = (itemId: TId) => this.commit('setCurrent', itemId);
    public updateItem = (item: TEntity) => this.commit('updateItem', item);
    public patchItem = (patch: object) => this.commit('patchItem', patch);
    public removeItem = (itemId: TId) => this.commit('removeItem', itemId);

    // TYPED ACTIONS //
    public loadAll = () => this.dispatch('loadAll') as Promise<TEntity[]>;

    // TYPED HELPERS //
    public getItem(itemId: TId): TEntity | undefined {
        if (this.state.all) {
            for (const item of this.state.all) {
                if (this.getItemId(item) === itemId) {
                    return item;
                }
            }
        }
        return undefined;
    }

    // ABSTRACT METHODS //
    protected abstract getItemId(item?: TEntity | object): any;
    protected abstract loadAllFromSource(): Promise<TEntity[]>;
}
