export class Statistics {
    constructor() {
        this.charts = new Map();
    }

    initChart(name, data) {
        let datasets = [];

        data.forEach(t => {
            let tmp = [];
            t.values.forEach(d => {
                tmp.push({ x: d.time, y: d.value });
            })

            datasets.push({
                data: tmp,
                fill: false,
                label: t.testCase,
                //lineTension: 0.1,
                //radius: 0
            });
        });

        this.showChart(name, datasets);
    }

    async showChart(name, data) {

        if (this.charts[name] === null || this.charts[name] === undefined) {
            let ctx = document.querySelector(`#${name}`).getContext("2d");
            this.charts[name] = new Chart(ctx, {
                type: "line",
                data: {
                    datasets: data
                },
                options: {
                    scales: {
                        x: {
                            type: 'timeseries',
                            //type: 'time',
                            //distribution: 'linear',
                            //beginAtZero: true,
                            //display: false,
                            time: {
                                unit: 'hour',
                                unitStepSize: 0.5,
                                displayFormats: {
                                    'hour': 'HH:mm:ss'
                                },
                            }
                        },
                        y: {
                            stacked: true,
                            beginAtZero: true
                        }
                    }
                }
            });
        } else {
            let chart = this.charts[id];
            for (let i = 0; i < chart.data.datasets.length; i++) {
                chart.data.datasets[i] = data.length > i ? data[i] : [];
            }
            chart.update();
        }
    }

}