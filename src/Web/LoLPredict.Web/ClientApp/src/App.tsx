import React, { createElement } from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Draft from './components/Draft';

const renderMergedProps = (component: any, ...rest: any[]) => {
    const finalProps = Object.assign({}, ...rest);
    return createElement(component, finalProps);
};

export const PropsRoute = ({ component, ...rest }: any) => {
  return (
    <Route {...rest} render={routeProps => {
      return renderMergedProps(component, routeProps, rest);
    }}/>
  );
}

export const App = () => {
    return (
        <Layout>
            <PropsRoute exact path='/' component={Draft} />
            <PropsRoute exact path='/draft' component={Draft} />
        </Layout>
    );
}
