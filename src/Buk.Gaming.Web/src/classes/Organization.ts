import api from '@/services/api';

function roleStength(role: string) {
    if (role === 'owner') {
        return 4;
    } else if (role === 'officer') {
        return 3;
    } else if (role === 'captain') {
        return 2;
    } else if (role === 'member') {
        return 1;
    } else {
        return 0;
    }
}

export class Organization {
    private Id: string;
    private Members: Member[];
    private Name: string;
    private IsPublic: boolean;
    private Image: string;
    private Pending: PendingMember[];

    constructor(organization?: OrganizationModel) {
        this.Id = organization?._id || '';
        this.Name = organization?.name || '';
        this.Members = organization?.members || [];
        this.IsPublic = organization?.isPublic || false;
        this.Image = organization?.image || '';
        this.Pending = organization?.pending || [];
    }

    public toModel(): OrganizationModel {
        return {
            _id: this.Id,
            name: this.Name,
            members: this.Members,
            isPublic: this.IsPublic,
            image: this.Image,
            pending: this.Pending,
        };
    }

    public save() {
        return api.organizations.saveOrganization(this);
    }

    public create() {
        return api.organizations.createOrganization(this);
    }

    public addMember(member: Member) {
        if (this.Members.find((m) => m.player._id === member.player._id)) {
            return;
        } else {
            this.Members.push(member);
        }
    }

    public updateMember(member: Member) {
        const player = this.Members.find(
            (m) => m.player._id === member.player._id,
        );
        if (player) {
            player.role = member.role;
        }
    }

    public removeMember(playerId: string) {
        this.Members = this.Members.filter((m) => m.player._id !== playerId);
    }

    public addPendingMember(player: Player, type: string) {
        if (this.Pending.find((p) => p.player._id === player._id)) {
            return;
        } else {
            this.Pending.push({
                _key: player._id,
                player,
                type,
            });
        }
    }

    public removePendingMember(playerId: string) {
        this.Pending = this.Pending.filter((p) => p.player._id !== playerId);
    }

    public setImage(value: string) {
        this.Image = value;
    }

    public get id() {
        return this.Id;
    }

    public get members() {
        const members = [...this.Members];
        return members.sort((a, b) => roleStength(b.role) - roleStength(a.role));
    }

    public get pending() {
        return this.Pending;
    }

    public get name() {
        return this.Name;
    }

    public set name(value: string) {
        this.Name = value;
    }

    public get isPublic() {
        return this.IsPublic;
    }

    public get image() {
        return this.Image;
    }
}
