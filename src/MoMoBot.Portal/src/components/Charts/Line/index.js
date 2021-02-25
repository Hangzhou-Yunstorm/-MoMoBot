import autoHeight from '../autoHeight';
import React from 'react';
import Debounce from 'lodash-decorators/debounce';
import Bind from 'lodash-decorators/bind';
import { Chart, Axis, Tooltip, Geom } from 'bizcharts';
import styles from '../index.less';

@autoHeight()
class Line extends React.Component {
    state = {
        autoHideXLabels: false,
    };
    componentDidMount() {
        window.addEventListener('resize', this.resize, { passive: true });
    }

    componentWillUnmount() {
        window.removeEventListener('resize', this.resize);
    }

    @Bind()
    @Debounce(400)
    resize() {
        if (!this.node) {
            return;
        }
        const canvasWidth = this.node.parentNode.clientWidth;
        const { data = [], autoLabel = true } = this.props;
        if (!autoLabel) {
            return;
        }
        const minWidth = data.length * 30;
        const { autoHideXLabels } = this.state;

        if (canvasWidth <= minWidth) {
            if (!autoHideXLabels) {
                this.setState({
                    autoHideXLabels: true,
                });
            }
        } else if (autoHideXLabels) {
            this.setState({
                autoHideXLabels: false,
            });
        }
    }

    render() {
        const {
            height,
            title,
            forceFit = true,
            data,
            color = 'rgba(24, 144, 255, 0.85)',
            padding,
            shape
        } = this.props;

        const { autoHideXLabels } = this.state;

        const scale = {
            x: {
                type: 'cat',
            },
            y: {
                min: 0,
            },
        };

        const tooltip = [
            'x*y',
            (x, y) => ({
                name: x,
                value: y,
            }),
        ];


        return (
            <div className={styles.chart} style={{ height }} ref={this.handleRoot}>
                <div ref={this.handleRef}>
                    {title && <h4 style={{ marginBottom: 20 }}>{title}</h4>}
                    <Chart height={height}
                        data={data}
                        scale={scale}
                        forceFit={forceFit}
                        padding={padding || 'auto'}>
                        <Axis name="x" />
                        <Axis name="y" />
                        <Tooltip showTitle={false} crosshairs={false} />
                        <Geom type="line"
                            position="x*y"
                            size={2}
                            shape={shape}
                            color={color}
                            tooltip={tooltip} />
                        <Geom
                            type="point"
                            position="x*y"
                            tooltip={tooltip}
                            size={4}
                            shape={"circle"}
                            style={{
                                stroke: "#fff",
                                lineWidth: 1
                            }}
                        />
                    </Chart>
                </div>
            </div>
        )
    }
}

export default Line;