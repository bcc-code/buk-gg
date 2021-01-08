declare module 'vue-multiselect' {
    import Vue from 'vue';

    class Multiselect extends Vue {}
    class multiselectMixin extends Vue {}
    class pointerMixin extends Vue {}

    export default Multiselect;
    export { Multiselect, multiselectMixin, pointerMixin };
}
