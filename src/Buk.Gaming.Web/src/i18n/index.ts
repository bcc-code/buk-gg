import VueI18n from 'vue-i18n';
import en from '@/i18n/en';
import no from '@/i18n/no';
import api from '@/services/api';
import Vue from 'vue';
import moment from 'moment';
import auth from '@/services/auth';

Vue.use(VueI18n);

const i18n = new VueI18n({
    locale: 'no',
    fallbackLocale: 'no',
    messages: {
        en,
        no,
    },
});

const loadedLanguages: string[] = []; // no languages loaded default

export async function ensureLanguageAsync() {
    const lang = localStorage.getItem('lang') || 'no';
    if (i18n.locale !== lang || !loadedLanguages.includes(lang)) {
        if (!loadedLanguages.includes(lang)) {
            while (!auth.isAuthenticated()) {
                await new Promise((resolve) => setTimeout(resolve, 20));
            }
            const locale = await api.localization.getLang(lang);
            i18n.setLocaleMessage(lang, locale);
            loadedLanguages.push(lang);
        }
        i18n.locale = lang;
    }

    return lang;
}

export async function setI18nLanguageAsync(lang: string) {
    localStorage.setItem('lang', lang);
    if (lang === 'no') {
        lang = 'nb';
    }
    moment.locale(lang);
    // Set local storage lang
    await ensureLanguageAsync();
}

export default i18n;
