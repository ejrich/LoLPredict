import React, { useRef } from 'react';
import { makeStyles } from '@material-ui/core/styles';
import Avatar from '@material-ui/core/Avatar';
import { useDrag, useDrop } from 'react-dnd'
import { Champion } from '../models/Types';

const useStyles = makeStyles({
    blank: {
        margin: 10,
        width: 100,
        height: 100,
        backgroundColor: '#1e282d'
    },
    champion: {
        margin: 10,
        width: 100,
        height: 100
    },
});

interface Props {
    index: number
    team: boolean,
    champion: Champion,
    onClick: () => void,
    moveChampion: (dragIndex: number, Index: number) => void
}

interface DragItem {
    index: number,
    team: boolean,
    type: string
}

const ChampionPick = ({ index, team, champion, onClick, moveChampion }: Props) => {
    const classes = useStyles();

    const ref = useRef<HTMLDivElement>(null);

    const [, drop] = useDrop({
        accept: 'CHAMPION',
        drop(item: DragItem) {
            if (!ref.current || item.team !== team) {
                return;
            }
            const dragIndex = item.index

            // Don't replace items with themselves
            if (dragIndex === index) {
                return;
            }

            moveChampion(dragIndex, index)
        
            item.index = index
        }
    });

    const [, drag] = useDrag({
        item: { type: 'CHAMPION', index, team }
    });

    drag(drop(ref));

    if (champion) {
        const url = `https://ddragon.leagueoflegends.com/cdn/${champion.patch}/img/champion/${champion.image}.png`;
        return <Avatar ref={ref} onClick={onClick} src={url} className={classes.champion} />;
    }

    return <Avatar ref={ref} onClick={onClick} className={classes.blank} />;
};

export default ChampionPick;
