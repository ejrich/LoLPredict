import React, { Fragment, useEffect, useMemo, useState } from 'react';
import { makeStyles, createStyles, Theme } from '@material-ui/core/styles';
import GridList from '@material-ui/core/GridList';
import GridListTile from '@material-ui/core/GridListTile';
import TextField from '@material-ui/core/TextField';
import { Champion, Picks, Patch } from '../models/Types';

const useStyles = makeStyles(({ palette, spacing }: Theme) =>
    createStyles({
        root: {
            display: 'flex',
            flexWrap: 'wrap',
            justifyContent: 'space-around',
            overflow: 'hidden',
            backgroundColor: palette.background.paper,
        },
        gridList: {
            height: '500px',
            width: '100%'
        },
        icon: {
            width: '75px',
            height: '75px',
        },
        grey: {
            width: '75px',
            height: '75px',
            filter: 'grayscale(1)'
        },
        textField: {
            marginLeft: spacing(1),
            marginRight: spacing(1),
            width: 200
        }
    })
);

interface Props {
    currentPick: number | null,
    chooseChampion: (currentPick: number, champion: Champion) => void,
    picks: Picks,
    patch: string | null
}

const ChampionSelector = ({ currentPick, chooseChampion, picks, patch }: Props) => {
    const classes = useStyles();
    const [champions, setChampions] = useState<Champion[]>([]);
    const [search, setSearch] = useState('');

    useEffect(() => {
        if (patch) {
            fetch(`/api/v1/champion/${patch}`)
                .then(res => res.json())
                .then(setChampions);
        }
    }, [patch]);

    const filteredChampions = useMemo(() => champions.filter(_ => _.name.toLowerCase().includes(search.toLowerCase())), [champions, search]);

    return (
        <Fragment>
            <TextField
                label="Search"
                type="search"
                className={classes.textField}
                margin='normal'
                value={search}
                onChange={event => setSearch(event.target.value)}
            />
            <br />
            <div className={classes.root}>
                <GridList cellHeight={110} cols={6} className={classes.gridList}>
                    { filteredChampions.map(champion => {
                        const url = `https://ddragon.leagueoflegends.com/cdn/${patch}/img/champion/${champion.image}.png`;

                        if (picks[champion.name]) {
                            return (
                                <GridListTile key={champion.name} cols={1}>
                                    <img src={url} alt={url} className={classes.grey} />
                                </GridListTile>
                            );
                        }

                        return (
                            <GridListTile key={champion.name} cols={1} onClick={() => currentPick && chooseChampion(currentPick, champion)}>
                                <img src={url} alt={url} className={classes.icon} />
                            </GridListTile>
                        );
                    })}
                </GridList>
            </div>
        </Fragment>
    );
};

export default ChampionSelector;
