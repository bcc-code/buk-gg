import Vue, { PluginObject } from 'vue';

export enum Events {
    SOME_EVENT = 'SOME_EVENT',
}

export class EventBus implements PluginObject<any> {
    private bus: Vue;

    constructor() {
        this.bus = new Vue();
    }

    public raise(event: string, ...args: any[]) {
        this.bus.$emit(event, ...args);
    }

    public on(event: string | string[], callback: () => void) {
        this.bus.$on(event, callback);
    }

    public once(event: string, callback: () => void) {
        this.bus.$once(event, callback);
    }

    public install(vue: typeof Vue, options?: any) {
        // Register bus globally
        vue.prototype.$events = this;
    }
}

const eventBus = new EventBus();

export default eventBus;
