import React from 'react';

export default class ErrorBoundary extends React.Component{

    static getDerivedStateFromError(error) {
        // alert(JSON.stringify(error));
    }

    componentDidCatch(error, info){
        console.error(error);
        console.error(info);
    }

    render(){
        return this.props.children;
    }
}