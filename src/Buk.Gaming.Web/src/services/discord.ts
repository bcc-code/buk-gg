import fetch from 'isomorphic-fetch';
import api from './api';

class Discord {
    public startDiscordLogin() {
        const redirectUrl = `${window.location.origin}/dashboard`;
        const clientId = `551349691959345177`;
        window.location.href = `https://discordapp.com/api/oauth2/authorize?response_type=token&client_id=${clientId}&redirect_uri=${encodeURIComponent(
            redirectUrl,
        )}&scope=identify`;
    }

    public get discordUser() {
        const currentUser = window.localStorage.getItem('DISCORD_USER');
        if (currentUser) {
            return JSON.parse(currentUser);
        }
        return null;
    }

    public disconnectDiscordLogin() {
        window.localStorage.removeItem('DISCORD_USER');
    }

    public async completeDiscordLogin() {
        const hasAccessToken =
            window.location.hash &&
            window.location.hash.indexOf('access_token') !== -1;
        if (hasAccessToken) {
            const hashParts = window.location.hash.replace('#', '').split('&');
            const hash: any = {};
            for (const i of hashParts) {
                const kv = i.split('=');
                hash[kv[0]] = kv[1];
            }
            // for (let i=0; i<hashParts.length; i++) {
            //     const kv = hashParts[i].split('=');
            //     hash[kv[0]] = kv[1];
            // }
            const options: RequestInit = {
                headers: {
                    Authorization: `Bearer ${hash.access_token}`,
                },
            };
            const path = 'https://discordapp.com/api/users/@me';
            const response = await fetch(path, options);
            if (response.body != null) {
                const discordUser = await response.json();
                window.localStorage.setItem(
                    'DISCORD_USER',
                    JSON.stringify(discordUser),
                );
                // window.location.href = window.location.href.split('#')[0];
                return true;
            }
        }
        return false;
    }
    
    public async searchForMember(data: any) {
        const result = await api.discord.searchForMember(data);

        return result;
    }
}

const discord = new Discord();

export default discord;
