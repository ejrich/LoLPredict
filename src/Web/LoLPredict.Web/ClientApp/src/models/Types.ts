export interface Champion {
    id: number,
    name: string,
    image: string,
    patch: string
}

export interface Team {
    [ position: string ]: Champion;
};

export interface Picks {
    [ name: string ]: boolean
}

export interface Prediction {
    prediction: boolean,
    probability: number,
    score: number
}

export type Role = 'top' | 'jungle' | 'mid' | 'bottom' | 'support';

export const roles = ['top', 'jungle', 'mid', 'bottom', 'support'];

export interface Patch {
    major: number,
    minor: number,
    version: number,
    live: boolean,
    tournament: boolean
}
