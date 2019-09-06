import React, { useEffect, useState, Children, cloneElement } from 'react';
import NavMenu from './NavMenu';
import PatchSelector from './PatchSelector';
import { makeStyles, createStyles, Theme } from '@material-ui/core/styles';
import { Patch } from '../models/Types';

const useStyles = makeStyles(({ breakpoints, spacing }: Theme) =>
    createStyles({
        layout: {
            width: 'auto',
            marginLeft: spacing(3),
            marginRight: spacing(3),
            [breakpoints.up(1100 + spacing(6))]: {
                width: 1100,
                marginLeft: 'auto',
                marginRight: 'auto'
            },
        },
    })
);

const Layout = ({ children }: { children: any }) => {
    const classes = useStyles();
    const [patches, setPatches] = useState<Patch[]>([]);
    const [selectedPatch, setPatch] = useState<Patch | null>(null);

    useEffect(() => {
        fetch('/api/v1/patch')
            .then(res => res.json())
            .then((response: Patch[]) => {
                setPatches(response);
                if (response.length > 0)
                    setPatch(response[0]);
            });
    }, []);

    const propsChildren = Children.map(children, child => 
        cloneElement(child, { selectedPatch, patches, setPatch }));

    return (
        <div>
            <NavMenu />
            <div className={classes.layout}>
                <br />
                <PatchSelector selectedPatch={selectedPatch} patches={patches} setPatch={setPatch} />
                { propsChildren }
            </div>
        </div>
    );
};

export default Layout;
