<template>
    <nav
        class="navbar navbar-expand-lg navbar-absolute"
        :class="{ 'bg-white': showMenu, 'navbar-transparent': !showMenu }"
    >
        <div class="container-fluid">
            <div class="navbar-wrapper">
                <div
                    class="navbar-toggle d-inline"
                    :class="{ toggled: $sidebar.showSidebar }"
                >
                    <button
                        type="button"
                        class="navbar-toggler"
                        aria-label="Navbar toggle button"
                        @click="toggleSidebar"
                    >
                        <span class="navbar-toggler-bar bar1"></span>
                        <span class="navbar-toggler-bar bar2"></span>
                        <span class="navbar-toggler-bar bar3"></span>
                    </button>
                </div>
                <!-- <h2 class="navbar-brand">{{$t(`module.${routeName.toLowerCase()}`)}}</h2> -->
            </div>

            <div class="float-right">
                <a href="#" @click="setLocale('en')"
                    ><img src="/img/flag-en-16.png" /></a
                >&nbsp;&nbsp;&nbsp;
                <a href="#" @click="setLocale('no')"
                    ><img src="/img/flag-no-16.png"
                /></a>
            </div>
        </div>
    </nav>
</template>
<script>
import { CollapseTransition } from 'vue2-transitions';
import Modal from '@/components/Modal';
import { ensureLanguageAsync, setI18nLanguageAsync } from '@/i18n/index';

export default {
    components: {
        CollapseTransition,
        Modal,
    },
    computed: {
        routeName() {
            const { name } = this.$route;
            return this.capitalizeFirstLetter(name);
        },
        // isRTL() {
        //   return this.$rtl.isRTL;
        // }
    },
    data() {
        return {
            activeNotifications: false,
            showMenu: false,
            searchModalVisible: false,
            searchQuery: '',
        };
    },
    methods: {
        capitalizeFirstLetter(str) {
            return str.charAt(0).toUpperCase() + str.slice(1);
        },
        toggleNotificationDropDown() {
            this.activeNotifications = !this.activeNotifications;
        },
        closeDropDown() {
            this.activeNotifications = false;
        },
        toggleSidebar() {
            this.$sidebar.displaySidebar(!this.$sidebar.showSidebar);
        },
        hideSidebar() {
            this.$sidebar.displaySidebar(false);
        },
        toggleMenu() {
            this.showMenu = !this.showMenu;
        },
        async setLocale(lang) {
            await setI18nLanguageAsync(lang);
            await localStorage.setItem('lang', lang);
            await this.$tournaments.loadAll();
        },
        // navbarBrand(){
        //   let currentRoute = this.$router.currentRoute;

        // }
    },
};
</script>
<style>
/* h2.navbar-brand {
    padding-top: 20px;
} */
</style>
