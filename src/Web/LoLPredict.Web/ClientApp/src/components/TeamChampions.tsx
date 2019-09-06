import React, { Fragment, useCallback } from 'react';
import ChampionPick from './ChampionPick';
import { Team, roles } from '../models/Types';

interface Props {
    team: Team,
    side: boolean,
    setCurrentPick: (_: number) => void
    setChampions: (_: Team) => void
}

export const TeamChampions = ({ team, side, setCurrentPick, setChampions }: Props) => {
    const beginIndex = side ? 6 : 1;

    const moveChampion = useCallback(
        (dragIndex: number, dropIndex: number): void => {
            const champ = team[roles[dragIndex]];
            const otherChamp = team[roles[dropIndex]];
            setChampions({
                ...team,
                [roles[dragIndex]]: otherChamp,
                [roles[dropIndex]]: champ,
            });
        },
        [team, setChampions]
    );

    return (
        <Fragment>
            { roles.map((role, index) =>
                <ChampionPick key={index} index={index} team={side}
                    champion={team[role]} onClick={() => setCurrentPick(beginIndex + index)}
                    moveChampion={moveChampion} />
            ) }
        </Fragment>
    );
}
