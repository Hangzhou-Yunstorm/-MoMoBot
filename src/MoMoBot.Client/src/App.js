import React from 'react';
import Home from './components/Home';
import './App.css';
import { Route } from 'react-router'
import ErrorBoundary from './components/ErrorBoundary';
// import Translate from './components/Translate';

export default () => {
    return (
        <ErrorBoundary>
            <div className="App">
                <Route path="/" exact={true} component={Home} />
                {/* <Route path="/translate" exact={true} component={Translate} /> */}
            </div>
        </ErrorBoundary>
    );
}