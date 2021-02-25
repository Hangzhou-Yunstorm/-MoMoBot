import * as React from 'react';

interface IErrorPredictionsProps {
    intents?: Array<{}>;
    activeIntent?: any;
    errorDetail?: any;
    onItemClick?: (id: string) => void;
}

export default class ErrorPredictions extends React.Component<IErrorPredictionsProps> { }