import api from '@/services/api';
import { RootStore } from '@/store';
import CrudStore, { CrudState } from './base/CrudStore';

export interface EventState extends CrudState<IEvent, string> {}

export class EventStore extends CrudStore<IEvent, EventState, string> {
    constructor(rootStore: RootStore) {
        super('ievents', rootStore, {
            all: [],
            item: {},
        });

        this.mutations = {
            ...this.mutations,
            setAll: (state: EventState, items: IEvent[]) => {
                state.all = items;
            },
        };

        this.actions = {
            ...this.actions,
            loadEvent: async (store, eventId: string): Promise<IEvent> => {
                const item = await api.events.getEvent(eventId);
                this.updateItem(item);
                this.setCurrent(item.id);
                return item;
            },
        };

        // GETTERS //
        this.getters = {
            ...this.getters,
        };
    }

    public loadEvent = (eventId: string) =>
        this.dispatch('loadEvent', eventId) as Promise<IEvent>

    protected getItemId(item?: object | Event | undefined) {
        if (item) {
            return (item as IEvent).id;
        }
    }
    protected loadAllFromSource(): Promise<IEvent[]> {
        return api.events.getAll();
    }
}
