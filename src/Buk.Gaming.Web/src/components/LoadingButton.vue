<template>
    <base-button :type="type" :loading="loading" :disabled="disabled" @click="runFunction()">
        {{ !loading ? title : '' }}
    </base-button>
</template>
<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
// import BaseButton from './BaseButton.vue';

@Component({
    props: {
        func: {
            default: (() => null),
            type: Function,
        },
        type: {
            default: '',
            type: String,
        },
        title: {
            default: '',
            type: String,
        },
        disabled: {
            default: false,
            type: Boolean,
        },
    },
})

export default class LoadingButton extends Vue {
    public func: (() => Promise<any>);
    public loading: boolean = false;
    public disabled: boolean;
    public type: string;

    public async runFunction() {
        this.loading = true;
        await this.func();
        this.loading = false;
    }
}


</script>