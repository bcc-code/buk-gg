export default {
    api: {
        basePath: 'https://buk.gg',
    },
    useRealRoutes: false,
    colors: {
        default: '#344675',
        primary: '#42b883',
        info: '#1d8cf8',
        danger: '#fd5d93',
        teal: '#00d6b4',
        primaryGradient: [
            'rgba(76, 211, 150, 0.1)',
            'rgba(53, 183, 125, 0)',
            'rgba(119,52,169,0)',
        ],
    },
    authentication: {
        domain: 'login.bcc.no',
        audience: 'https://buk.gg/api',
        clientID: 'F17tny0a4z55HysZmdppNF0RCLTMiyet',
        redirectUri: '/callback',
    },
};
