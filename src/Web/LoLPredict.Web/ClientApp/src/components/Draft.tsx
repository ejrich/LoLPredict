import React, { Fragment, useState, useMemo, useEffect } from 'react';
import Grid from '@material-ui/core/Grid';
import ChampionSelector from './ChampionSelector';
import { TeamChampions } from './TeamChampions';
import { PredictionBar } from './PredictionBar';
import { Champion, Picks, Prediction, Team, roles, Patch } from '../models/Types';

const BlueSide = false;
const RedSide = true;

const combinePicks = (blue: Team, red: Team): Picks => {
    const picks: Picks = {};
    Object.values(blue).forEach(champion => {
        if (champion)
            picks[champion.name] = true;
    });
    Object.values(red).forEach(champion => {
        if (champion)
            picks[champion.name] = true;
    });

    return picks;
};

const getChampion = (team: Team, role: string) => team[role] ? team[role].id : 0;;

const makePrediction = (patch: Patch, blue: Team, red: Team, setPrediction: (_: Prediction) => void): void => {
    if (!patch) return;
    const draft = {
        blueTop: getChampion(blue, 'top'),
        blueJungle: getChampion(blue, 'jungle'),
        blueMid: getChampion(blue, 'mid'),
        blueBottom: getChampion(blue, 'bottom'),
        blueSupport: getChampion(blue, 'support'),
        redTop: getChampion(red, 'top'),
        redJungle: getChampion(red, 'jungle'),
        redMid: getChampion(red, 'mid'),
        redBottom: getChampion(red, 'bottom'),
        redSupport: getChampion(red, 'support')
    };

    const predictionRequest = {
        patch: `${patch.major}.${patch.minor}.${patch.version}`,
        draft
    };

    fetch('api/v1/prediction', {
        method: 'POST',
        body: JSON.stringify(predictionRequest),
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(res => {
            if (res.ok) return res.json();
        })
        .then(prediction => {
            if (prediction)
                setPrediction(prediction);
        });
};

interface Props {
    selectedPatch: Patch
}

const Draft = ({ selectedPatch }: Props) => {
    const [currentPick, setCurrentPick] = useState<number | null>(null);
    const [blueChampions, setBlueChampions] = useState<Team>({});
    const [redChampions, setRedChampions] = useState<Team>({});
    const [prediction, setPrediction] = useState<Prediction | null>(null);

    const chooseChampion = (currentPick: number, champion: Champion) => {
        if (currentPick > 5) {
            setRedChampions({
                ...redChampions,
                [roles[currentPick - 6]]: champion
            });
        }
        else {
            setBlueChampions({
                ...blueChampions,
                [roles[currentPick - 1]]: champion
            });
        }
    };

    useEffect(() => makePrediction(selectedPatch, blueChampions, redChampions, setPrediction), [selectedPatch, blueChampions, redChampions]);

    const picks = useMemo(() => combinePicks(blueChampions, redChampions), [blueChampions, redChampions]);

    return (
        <Fragment>
            <PredictionBar prediction={prediction} />
            <Grid container justify='center' alignItems='center'>
                <Grid item xs={2}>
                    <TeamChampions team={blueChampions} setChampions={setBlueChampions} side={BlueSide} setCurrentPick={setCurrentPick} />
                </Grid>
                <Grid item xs={8}>
                    <ChampionSelector picks={picks} currentPick={currentPick} chooseChampion={chooseChampion} patch={selectedPatch} />
                </Grid>
                <Grid item xs={2}>
                    <TeamChampions team={redChampions} setChampions={setRedChampions} side={RedSide} setCurrentPick={setCurrentPick} />
                </Grid>
            </Grid>
        </Fragment>
    );
};

export default Draft;
