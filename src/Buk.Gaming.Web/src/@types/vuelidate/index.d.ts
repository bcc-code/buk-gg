declare module 'vuelidate' {
    import Vue, { PluginFunction, PluginObject } from 'vue';

    class VuelidatePlugin implements PluginObject<{}> {
        [key: string]: any;
        public install: PluginFunction<{}>;
    }

    const Vuelidate: VuelidatePlugin;
    export default Vuelidate;
}
