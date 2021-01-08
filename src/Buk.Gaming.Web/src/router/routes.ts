// GeneralViews
import NotFound from '@/pages/NotFoundPage.vue';
import { RouterOptions } from 'vue-router';

import Events from '@/pages/Events/List.vue';
import EventDetails from '@/pages/Events/Details.vue';
import Dashboard from '@/pages/Dashboard.vue';
import Profile from '@/pages/Profile.vue';
import Tournaments from '@/pages/Tournaments/List.vue';
import TournamentAdminInfo from '@/pages/Tournaments/AdminInfo.vue';
import Camps from '@/pages/Camps/List.vue';
const TournamentDetails = () => import(/* webpackChunkName: 'tournamentDetails' */'@/pages/Tournaments/Details.vue');
const CampDetails = () => import(/* webpackChunkName: 'campDetails' */ '@/pages/Camps/Details.vue');
const Discord = () => import(/* webpackChunkName: 'discord' */ '@/pages/Discord.vue');
const PrivacyPolicy = () => import(/* webpackChunkName: 'privacyPolicy' */ '@/pages/PrivacyPolicy.vue');
const OrganizationModel = () => import(/* webpackChunkName: 'organizationModel' */ '../pages/Organization.vue');
const OrganizationList = () => import(/* webpackChunkName: 'organizationList' */ '../pages/Organizations/ListCards.vue');
const TeamList = () => import(/* webpackChunkName: 'teamList' */ '../pages/Organizations/TeamList.vue');

const routes: RouterOptions['routes'] = [
    {
        path: '/',
        redirect: '/dashboard',
    },
    {
        path: '/dashboard',
        name: 'dashboard',
        component: Dashboard,
    },
    {
        path: '/profile',
        name: 'profile',
        component: Profile,
    },
    {
        path: '/discord',
        name: 'discord',
        component: Discord,
    },
    {
        path: '/camps',
        name: 'camps',
        component: Camps,
    },
    {
        path: '/camps/:camp',
        name: 'camp-details',
        component: CampDetails,
    },
    {
        path: '/tournament/:tournamentId',
        name: 'tournament-details',
        component: TournamentDetails,
    },
    {
        path: '/tournament/:tournamentId/admin',
        name: 'tournament-admin-info',
        component: TournamentAdminInfo,
    },
    {
        path: '/tournaments',
        name: 'tournaments',
        component: Tournaments,
    },
    {
        path: '/privacy-policy',
        name: 'privacy-policy',
        component: PrivacyPolicy,
    },
    {
        path: '/events',
        name: 'events',
        component: Events,
    },
    {
        path: '/events/:eventId',
        name: 'event-details',
        component: EventDetails,
    },
    {
        path: '/teams',
        name: 'teams',
        component: TeamList,
    },
    {
        path: '/organizations',
        name: 'organizations',
        component: OrganizationList,
    },
    {
        path: '/organization/:id',
        name: 'organization',
        component: OrganizationModel,
    },
    { path: '*', component: NotFound },
];

export default routes;
