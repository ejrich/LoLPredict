import React, { useRef, useMemo } from 'react';
import { makeStyles } from '@material-ui/core/styles';
import { useDrag, useDrop } from 'react-dnd'
import { Champion } from '../models/Types';

const useStyles = makeStyles({
    icon: {
        width: 100,
        height: 100,
        backgroundColor: '#1e282d'
    },
    border: {
        marginLeft: 'auto',
        marginRight: '21%',
        padding: '5px',
        width: 'min-content'
    },
    blue: {
        backgroundColor: '#1580B6'
    },
    red: {
        backgroundColor: '#DE2F2F'
    }
});

interface Props {
    index: number;
    team: boolean
    patch: string | null;
    selecting: boolean;
    champion: Champion;
    onClick: () => void;
    moveChampion: (dragIndex: number, Index: number) => void;
}

interface DragItem {
    index: number,
    team: boolean,
    type: string
}

const ChampionPick = ({ index, team, patch, selecting, champion, onClick, moveChampion }: Props) => {
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

    const selectingClass = selecting ? team ? classes.red : classes.blue : '';

    const url = champion ? `https://ddragon.leagueoflegends.com/cdn/${patch}/img/champion/${champion.image}.png` : null;

    return (
        <div className={`${classes.border} ${selectingClass}`}>
            <div ref={ref} onClick={onClick} className={classes.icon}>
                { url && <img style={{ width: '100%' }} src={url} /> } 
            </div>
        </div>
    );
};

export default ChampionPick;
