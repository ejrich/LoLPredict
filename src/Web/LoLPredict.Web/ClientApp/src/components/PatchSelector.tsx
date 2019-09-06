import React, { CSSProperties, HTMLAttributes, useCallback, useMemo } from 'react';
import Select from 'react-select';
import { ControlProps } from 'react-select/src/components/Control';
import { OptionProps } from 'react-select/src/components/Option';
import { makeStyles, useTheme } from '@material-ui/core/styles';
import TextField, { BaseTextFieldProps } from '@material-ui/core/TextField';
import MenuItem from '@material-ui/core/MenuItem';
import { Patch } from '../models/Types';

const useStyles = makeStyles({
    input: {
        display: 'flex',
        padding: 0,
        height: 'auto',
    }
});

interface Props {
    selectedPatch: Patch | null,
    patches: Patch[],
    setPatch: (patch: Patch) => void
}

interface PatchLabel {
    value: number,
    label: string
}

const patchDisplayName = (patch: Patch | null): string =>
    patch ? `${patch.major}.${patch.minor}` : '';

type InputComponentProps = Pick<BaseTextFieldProps, 'inputRef'> & HTMLAttributes<HTMLDivElement>;

const inputComponent = ({ inputRef, ...props }: InputComponentProps) => {
    return <div ref={inputRef} {...props} />;
};

const Control = ({ children, innerProps, innerRef, selectProps: { classes, TextFieldProps }}: ControlProps<PatchLabel>) => {
    return (
        <TextField
            fullWidth
            InputProps={{
                inputComponent,
                inputProps: {
                    className: classes.input,
                    ref: innerRef,
                    children,
                    ...innerProps,
                },
            }}
            {...TextFieldProps}
        />
    );
};

const Option = ({ innerRef, isFocused, isSelected, innerProps, children }: OptionProps<PatchLabel>) => {
    return (
        <MenuItem
            ref={innerRef}
            selected={isFocused}
            component="div"
            style={{
                fontWeight: isSelected ? 500 : 400,
            }}
            {...innerProps}>
            {children}
        </MenuItem>
    );
};

const components = {
    Control,
    Option,
};

const PatchSelector = ({ selectedPatch, patches, setPatch }: Props) => {
    const classes = useStyles();
    const theme = useTheme();

    const selectStyles = {
        input: (base: CSSProperties) => ({
            ...base,
            color: theme.palette.text.primary,
            '& input': {
                font: 'inherit',
            },
        }),
    };

    const patchName = { label: patchDisplayName(selectedPatch) } as PatchLabel;
    const patchList = useMemo(() => patches.map((patch, index): PatchLabel =>
        ({ label: patchDisplayName(patch), value: index })), [patches]);

    const changePatch = useCallback((value: any, actionMeta: any) => {
        if (!value) return;
        setPatch(patches[value.value]);
    }, [setPatch, patches]);

    return (
        <div style={{ width: '10%' }}>
            <Select
                classes={classes}
                styles={selectStyles}
                inputId='patch'
                TextFieldProps={{
                    label: 'Patch',
                    InputLabelProps: {
                        htmlFor: 'patch',
                        shrink: true,
                    },
                }}
                options={patchList}
                value={patchName}
                components={components}
                onChange={changePatch} />
        </div>
    );
};

export default PatchSelector;
