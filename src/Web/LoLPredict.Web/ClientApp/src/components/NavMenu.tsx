import React from 'react';
import { Link } from 'react-router-dom';
import { makeStyles } from '@material-ui/core/styles';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import Typography from '@material-ui/core/Typography';
import Button from '@material-ui/core/Button';
import './NavMenu.css';

const useStyles = makeStyles({
    root: {
        flexGrow: 1
    },
    title: {
        flexGrow: 1,
        textDecoration: 'none',
        color: 'white'
    },
    menuButton: {
        marginLeft: -12,
        marginRight: 20
    },
});

export const NavMenu = () => {
    const classes = useStyles();

    return (
        <header className={classes.root}>
            <AppBar position="static">
                <Toolbar>
                    <Typography variant="h6" color="inherit" className={classes.title}>
                        <Link to='/' className={classes.title}>
                            LoL Predict
                        </Link>
                    </Typography>
                    <Button color="inherit" component={Link} to='/draft'>Draft</Button>
                </Toolbar>
            </AppBar>
        </header>
    );
}

export default NavMenu;
