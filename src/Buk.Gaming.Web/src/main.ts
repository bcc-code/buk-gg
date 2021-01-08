/*
 =========================================================
 * Vue Black Dashboard - v1.1.0
 =========================================================

 * Product Page: https://www.creative-tim.com/product/black-dashboard
 * Copyright 2018 Creative Tim (http://www.creative-tim.com)

 =========================================================

 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

 */

import Vue from 'vue';
import VueRouter from 'vue-router';
import RouterPrefetch from 'vue-router-prefetch';
import router from '@/router';
import store from '@/store';
import Api from '@/services/api';
import EventBus from '@/services/eventBus';

import BootstrapVue from 'bootstrap-vue';

import BlackDashboard from './plugins/blackDashboard';
import '@fortawesome/fontawesome-free/scss/fontawesome.scss';
import '@fortawesome/fontawesome-free/scss/brands.scss';
import '@fortawesome/fontawesome-free/scss/solid.scss';
import i18n from '@/i18n/index';
import App from './App.vue';
import '@/style/custom.scss';

Vue.use(BlackDashboard);
Vue.use(VueRouter);
Vue.use(RouterPrefetch);
Vue.use(BootstrapVue);
Vue.config.productionTip = false;
Vue.use(Api);
Vue.use(EventBus);

import 'moment/locale/nb';

/* eslint-disable no-new */
new Vue({
    router,
    i18n,
    store,
    render: (h) => h(App),
}).$mount('#app');
