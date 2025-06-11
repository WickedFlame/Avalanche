export class Statistics {
    constructor() {
        this.charts = new Map();
    }

    initChart(testname, data) {

        let tmp = [];
        data.forEach(d => {
            tmp.push({ x: d.time, y: d.value });
        })


        let datasets = [];
        datasets.push({
            data: tmp,
            fill: false,
            label: testname,
            //lineTension: 0.1,
            //radius: 0
        });

        this.showChart(testname, datasets);
    }

    async showChart(testname, data) {
        let id = testname.replace(/ /g, '_');

        if (this.charts[id] === null || this.charts[id] === undefined) {
            
            let ctx = document.querySelector(`#chart-${id}`).getContext("2d");
            this.charts[id] = new Chart(ctx, {
                type: "line",
                data: {
                    datasets: data
                },
                options: {
                    scales: {
                        x: {
                            type: 'time',
                            distribution: 'linear',
                            beginAtZero: true
                        },
                        y: {
                            stacked: true,
                            beginAtZero: true
                        }
                    },
                    //plugins: {
                    //    legend: {
                    //        display: true,
                    //        labels: {
                    //            color: 'rgb(255, 99, 132)'
                    //        }
                    //    }
                    //}
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