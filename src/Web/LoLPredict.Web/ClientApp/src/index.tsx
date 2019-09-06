import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';
import { App } from './App';
import './index.css';
import registerServiceWorker from './registerServiceWorker';
import { DragDropContextProvider } from 'react-dnd';
import HTML5Backend from 'react-dnd-html5-backend';

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href') || undefined;
const rootElement = document.getElementById('root');

ReactDOM.render(
    <BrowserRouter basename={baseUrl}>
        <DragDropContextProvider backend={HTML5Backend}>
            <App />
        </DragDropContextProvider>
    </BrowserRouter>,
    rootElement);

registerServiceWorker();
