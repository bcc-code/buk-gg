<template>
    <div class="sidebar" :data="backgroundColor">
        <!--
            Tip 1: you can change the color of the sidebar's background using: data-background-color="white | black | darkblue"
            Tip 2: you can change the color of the active button using the data-active-color="primary | info | success | warning | danger"
        -->
        <!-- -->
        <div class="sidebar-wrapper" id="style-3">
            <div
                class="logo main-logo"
                @click="pushToDashboard()"
                style="cursor: pointer; height: auto;"
            >
                <div style="height:50px"></div>
                <a
                    href="/dashboard"
                    aria-label="sidebar mini logo"
                    class="simple-text logo-mini"
                >
                    <div class="logo-img" :class="{ 'logo-img-rtl': false }">
                        <img src="/img/icons/ms-icon-310x310.png" alt="" />
                    </div>
                </a>
                <div class="simple-text logo-normal" style="font-weight: 800; font-size: 22px;">
                    {{ title }}
                </div>
            </div>
            <slot> </slot>
            <ul class="nav">
                <!--By default vue-router adds an active class to each route link. This way the links are colored when clicked-->
                <slot name="links">
                    <sidebar-link
                        v-for="(link, index) in sidebarLinks"
                        :key="index"
                        :to="link.path"
                        :name="link.name"
                        :icon="link.icon"
                    >
                    </sidebar-link>
                </slot>
            </ul>

            <ul class="nav nav-bottom" style="width: 100%">
                <li class="nav-item">
                    <a style="cursor: pointer;" @click="setLocale('no')"
                        ><p><img src="/img/flag-no-16.png" /> NORSK</p></a
                    >
                </li>
                <li class="nav-item">
                    <a style="cursor: pointer;" @click="setLocale('en')"
                        ><p><img src="/img/flag-en-16.png" /> ENGLISH</p></a
                    >
                </li>
            </ul>
        </div>
    </div>
</template>
<style lang="scss">
.nav-bottom {
    position: absolute;
    bottom: 20px;
    img {
        margin-right: 15px;
        margin-left: 5px;
        width: 22px;
    }
    a {
        margin-top: 0 !important;
        padding-top: 0 !important;
    }
}
</style>
<script lang="ts">
import SidebarLink from './SidebarLink.vue';
import { ensureLanguageAsync, setI18nLanguageAsync } from '../../i18n/index';

export default {
    props: {
        title: {
            type: String,
            default: 'BUK GAMING',
        },
        backgroundColor: {
            type: String,
            default: 'primary',
        },
        activeColor: {
            type: String,
            default: 'success',
            validator: (value) => {
                const acceptedValues = [
                    'primary',
                    'info',
                    'success',
                    'warning',
                    'danger',
                ];
                return acceptedValues.indexOf(value) !== -1;
            },
        },
        sidebarLinks: {
            type: Array,
            default: () => [],
        },
        autoClose: {
            type: Boolean,
            default: true,
        },
    },
    provide() {
        return {
            autoClose: this.autoClose,
            addLink: this.addLink,
            removeLink: this.removeLink,
        };
    },
    components: {
        SidebarLink,
    },
    computed: {
        /**
         * Styles to animate the arrow near the current active sidebar link
         * @returns {{transform: string}}
         */
        arrowMovePx() {
            return this.linkHeight * this.activeLinkIndex;
        },
        shortTitle() {
            return this.title
                .split(' ')
                .map((word) => word.charAt(0))
                .join('')
                .toUpperCase();
        },
    },
    data() {
        return {
            linkHeight: 65,
            activeLinkIndex: 0,
            windowWidth: 0,
            isWindows: false,
            hasAutoHeight: false,
            links: [],
        };
    },
    methods: {
        findActiveLink() {
            this.links.forEach((link, index) => {
                if (link.isActive()) {
                    this.activeLinkIndex = index;
                }
            });
        },
        addLink(link) {
            const index = this.$slots.links.indexOf(link.$vnode);
            this.links.splice(index, 0, link);
        },
        removeLink(link) {
            const index = this.links.indexOf(link);
            if (index > -1) {
                this.links.splice(index, 1);
            }
        },
        pushToDashboard() {
            if (this.$route.name !== 'dashboard') { this.$router.push('/dashboard'); }
        },
        async setLocale(lang) {
            await setI18nLanguageAsync(lang);
            await localStorage.setItem('lang', lang);
            await this.$tournaments.loadAll();
        },
    },
    mounted() {
        this.$watch('$route', this.findActiveLink, {
            immediate: true,
        });
    },
};
</script>
