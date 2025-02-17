export class TestOverview {
    constructor() {
        document.querySelector('#StartTests').addEventListener('click', e => {
            e.preventDefault();

            let name = e.target.dataset.name;
            this.startTest(name);
        });

        document.querySelector('#StopTests').addEventListener('click', e => {
            e.preventDefault();

            let name = e.target.dataset.name;
            this.stopTest(name);
        });

        this.charts = new Map();
    }

    async startTest(name) {
        const url = `api/test/${name}/start`;
        try {
            const response = await fetch(url, {
                method: "POST",
            });
            if (!response.ok) {
                throw new Error(`Response status: ${response.status}`);
            }

            const json = await response.json();
        } catch (error) {
            console.error(error.message);
        }
    }

    async stopTest(name) {
        const url = `api/test/${name}/stop`;
        try {
            const response = await fetch(url, {
                method: "POST",
            });
            if (!response.ok) {
                throw new Error(`Response status: ${response.status}`);
            }

            const json = await response.json();
        } catch (error) {
            console.error(error.message);
        }
    }

    displayChart(scenario, testname) {
        this.loadChartData(scenario, testname);

        setInterval(() => {
            this.loadChartData(scenario, testname);
        }, 10000);
    }

    async loadChartData(scenario, testname) {
        const url = `api/testdata/${scenario}/${testname}/chartdata`;
        try {
            const response = await fetch(url);
            if (!response.ok) {
                throw new Error(`Response status: ${response.status}`);
            }

            const data = await response.json();

            let datasets = [];

            data.chartData.forEach(function (a) {
                let tmp = [];
                a.data.forEach(d => {
                    tmp.push({ x: d.time, y: d.value });
                })
                datasets.push({
                    data: tmp,
                    fill: false,
                    label: a.name,
                    lineTension: 0.1,
                    radius: 0
                });
            }, Object.create(null));

            this.showChart(testname, datasets);



            //TODO: stop poller when state is done
            //TODO: show resultdata when state is done

        } catch (error) {
            console.error(error.message);
        }
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
                            //time: {
                            //    unit: 'second'
                            //}
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
                chart.data.datasets[i] = data[i];
            }
            chart.update();
        }
    }

}