import React, { useMemo } from 'react';
import { makeStyles, createStyles, withStyles, Theme } from '@material-ui/core/styles';
import LinearProgress from '@material-ui/core/LinearProgress';
import { Prediction } from '../models/Types';

const PredictionIndicator = withStyles({
    root: {
        height: 20,
        backgroundColor: '#DE2F2F',
    },
    bar: {
        borderRadius: 20,
        backgroundColor: '#1580B6',
    },
})(LinearProgress);

const useStyles = makeStyles((theme: Theme) =>
    createStyles({
        root: {
            flexGrow: 1,
        },
        margin: {
            margin: theme.spacing(1),
        },
    }),
);

interface Props {
    prediction: Prediction | null
}

export const PredictionBar = ({ prediction }: Props) => {
    const classes = useStyles();

    const predictionValue = useMemo(() => prediction ? (1 - prediction.probability) * 100 : 50, [prediction]);

    return (
        <span className={classes.root}>
            <PredictionIndicator
                className={classes.margin}
                variant="determinate"
                color="secondary"
                value={predictionValue} />
        </span>
    );
}
