
// SESSION
interface Session {
    currentUser: User;
    authenticatedUser: User;
}

interface Player {
    _id: string;
    email?: string;
    personKey: string;
    displayName: string;
    name?: string;
    noNbIsStandard?: boolean;
    isO18: boolean;
    nickname: string;
    location: string;
    discordUser: string;
    discordId: string;
    discordIsConnected: boolean;
    enableMoreDiscords: boolean;
    moreDiscordUsers: ExtraDiscordUser[];
    image?: string;
    dateRegistered: Date;
    dateLastActive: Date;
    isRegistered: boolean;
    agreeToPrivacyPolicy: boolean;
    phoneNumber: string;
}

interface ExtraDiscordUser {
    _key: string;
    name: string;
    discordId: string;
}

interface User extends Player {
    isAdministrator: boolean;
    canImpersonate: boolean;
}

// ORGANIZATION
interface OrganizationModel {
    _id: string;
    name: string;
    isPublic: boolean;
    image: any;
    members: Member[];
    pending: PendingMember[];
}

interface TeamModel {
    _id: string;
    name: string;
    captain: Player;
    players: Player[];
    organization: OrganizationModel;
    game: Game;
}

interface Member {
    _key: string;
    player?: Player;
    role: string;
}

interface PendingMember {
    _key: string;
    player: Player;
    type: string;
}

// TOURNAMENT
interface Stage {
    id: string;
    name: string;
    number: number;
    type: string;
    closed: boolean;
    settings: any;
}

interface TournamentInfo {
    id: string;
    slug: string;
    title: string;
    platform: string;
    introduction: string;
    categoryIds: string[];
    responsibleId: string;
    body: string;
    logo: string;
    mainImage: string;
    toornamentId: string;
    registrationForm: string;
    registrationOpen: boolean;
    telegramLink: string;
    liveStream: string;
    liveChat: boolean;
    stages: Stage[];
    game: Game;
    signupType: string;
    requiredInformation: string[];
    teamSize: {
        max: number;
        min: number;
    };
    teams: TeamModel[];
    playerIds: string[];
    winner: string;
    contacts: {
        _key: string;
        name: string;
        email: string;
        discord: string;
        phoneNumber: string;
    };
}

interface TournamentAdminInfo extends TournamentInfo {
    responsible: Player;
    soloPlayers: {
        information: string[];
        player: Player;
        toornamentId: string;
    };
    participantTeams: {
        information: string[];
        team: TeamModel;
        toornamentId: string;
    };
}

interface Tournament {
    id: string;
    name: string;
    full_name: string;
    discipline: string;
    scheduled_date_start: Date;
    scheduled_date_end: Date;
    logo: any;
    registration_enabled: boolean;
}

interface Game {
    _id: string;
    name: string;
    hasTeams: string;
    icon?: string;
    teamFields: Array<{
        title: string;
        key: string;
    }>;
    playerFields: Array<{
        title: string;
        key: string;
    }>;
}

// EVENT
interface IEvent {
    id: string;
    title: string;
    image: string;
    responsible: string;
    date: Date;
    description: string;
    categoryId: string;
}

// SANITY
interface SanityResult<T> {
    item: T;
    success: boolean;
    reason: string;
}
