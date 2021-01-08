<template>
    <div class="content">
        <!-- your content here -->
        <transition name="fade" mode="out-in">
            <router-view></router-view>
        </transition>
    </div>
</template>
<style lang="scss">
.fade-enter-active,
.fade-leave-active {
    transition: opacity 0.2s;
}

.fade-enter,
.fade-leave-to {
    opacity: 0;
}
</style>
<script>
import { FadeTransition } from 'vue2-transitions';
export default {
    components: {
        FadeTransition,
    },
    methods: {
        key() {
            return this.$route.name;
        },
    },
};
</script>
