import VueRouter from 'vue-router';
import routes from './routes';
import { ensureLanguageAsync } from '@/i18n/index';
import { tournaments, session } from '@/store/index';

// configure router
const router = new VueRouter({
    mode: 'history',
    routes, // short for routes: routes
    linkExactActiveClass: 'active',
    scrollBehavior(to) {
        if (to.hash) {
            return { selector: to.hash };
        } else {
            return { x: 0, y: 0 };
        }
    },
});

router.beforeEach((to, from, next) => {
    session.startSession().then(() => {
        if (to.path !== '/callback') {
            ensureLanguageAsync().then(() => {
                if (to.params.tournamentId) {
                    tournaments.setCurrent(to.params.tournamentId);
                }
            });
            next();
        }
    });
});

export default router;
