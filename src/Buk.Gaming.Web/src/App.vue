<template>
    <transition name="fade">
        <div
            v-if="$session.state.isAuthenticated"
            class="vld-parent"
            ref="formContainer"
        >
            <notifications></notifications>
            <!-- <router-view :key='$route.fullPath'></router-view> -->
            <dashboard-layout></dashboard-layout>
        </div>
    </transition>
</template>

<script lang="ts">
// import './@types/global.d.ts';
import { Component, Vue } from 'vue-property-decorator';
import DashboardLayout from '@/layout/DashboardLayout.vue';
import { ievents } from './store';
import { ensureLanguageAsync, setI18nLanguageAsync } from './i18n/index';
import auth from './services/auth';
import discord from './services/discord';
import config from './config';
import http from './services/http';

@Component({
    components: {
        DashboardLayout,
    },
})
export default class App extends Vue {
    public async mounted() {

        const version = await localStorage.getItem('version');
        const apiVersion = await http.get<string>('version');
        if (version !== apiVersion && apiVersion) {
            await localStorage.setItem('version', apiVersion);
            window.location.reload(true);
        }

        this.$watch('$sidebar.showSidebar', this.toggleNavOpen);
        // Source: https://dennisreimann.de/articles/delegating-html-links-to-vue-router.html
        window.addEventListener('click', ($event) => {
            // ensure we use the link, in case the click has been received by a subelement
            let { target } = $event as any;
            while (target && target.tagName !== 'A') {
                target = target.parentNode;
            }
            // handle only links that do not reference external resources
            if (
                target &&
                target.matches('a:not([href*="://"])') &&
                target.href
            ) {
                // some sanity checks taken from vue-router:
                // https://github.com/vuejs/vue-router/blob/dev/src/components/link.js#L106
                const {
                    altKey,
                    ctrlKey,
                    metaKey,
                    shiftKey,
                    button,
                    defaultPrevented,
                } = $event;
                // don't handle with control keys
                if (metaKey || altKey || ctrlKey || shiftKey) {
                    return;
                }
                // don't handle when preventDefault called
                if (defaultPrevented) {
                    return;
                }
                // don't handle right clicks
                if (button !== undefined && button !== 0) {
                    return;
                }
                // don't handle if `target='_blank'`
                if (target && target.getAttribute) {
                    const linkTarget = target.getAttribute('target');
                    if (/\b_blank\b/i.test(linkTarget)) {
                        return;
                    }
                }
                // don't handle same page links/anchors
                const url = new URL(target.href);
                const to = url.pathname;
                if (window.location.pathname !== to && $event.preventDefault) {
                    $event.preventDefault();
                    this.$router.push(to).catch((err) => {
                        return;
                    });
                }
            }
        });
        if (navigator && navigator.serviceWorker) {
            navigator.serviceWorker.getRegistrations().then((regs) => {
                regs.forEach((reg) => {
                    reg.unregister();
                });
            });
        }

        if (await localStorage.getItem('lang')) {
            setI18nLanguageAsync(await localStorage.getItem('lang'));
        } else if (navigator.language === 'no-NB') {
            setI18nLanguageAsync('no');
            await localStorage.setItem('lang', 'no');
        } else {
            setI18nLanguageAsync('en');
            await localStorage.setItem('lang', 'en');
        }

        while (!auth.isAuthenticated()) {
            await new Promise((resolve) => setTimeout(resolve, 10));
        }
        await this.$tournaments.loadAll();
        await this.$organizations.loadAll();
        await this.$teams.loadTeams();
    }

    public toggleNavOpen() {
        const root = document.getElementsByTagName('html')[0];
        root.classList.toggle('nav-open');
    }
}
</script>

<style lang="scss"></style>
