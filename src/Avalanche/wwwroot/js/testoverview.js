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
            let testId = e.target.dataset.testid;

            this.stopTest(name, testId);
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

            //json.tests.forEach(t => {
            //    let testname = t.name.replace(/ /g, '_');
            //    this.initChart(t, testname);
            //});
            location.reload();
        } catch (error) {
            console.error(error.message);
        }
    }

    async stopTest(name, testId) {
        const url = `api/test/${name}/stop/${testId}`;
        try {
            const response = await fetch(url, {
                method: "POST",
            });
            if (!response.ok) {
                throw new Error(`Response status: ${response.status}`);
            }

            const json = await response.json();

            location.reload();
        } catch (error) {
            console.error(error.message);
        }
    }




    async showSummaryData(scenario, testname, testid) {
        const url = `api/testdata/${scenario}/${testname}/summary/${testid}`;
        try {
            const response = await fetch(url);
            if (!response.ok) {
                throw new Error(`Response status: ${response.status}`);
            }

            const data = await response.json();

            if (data.data.testSummary !== null) {
                let summary = data.data.testSummary;

                let ts = document.querySelector(`#testsummary-${testid}`);
                ts.style.display = 'block';

                summary.forEach(s => {
                    let row = ts.querySelector(`#${s.testCase.replaceAll(' ', '_')}`);
                    row.querySelector('.TestCase').innerHTML = s.testCase;
                    row.querySelector('.TotalTime').innerHTML = s.totalTime;
                    row.querySelector('.AverageTicks').innerHTML = s.averageTicks;
                    row.querySelector('.Iterations').innerHTML = s.iterations;
                });
                
            }

            data.threadSummary.forEach(ts => {

            });

        } catch (error) {
            console.error(error.message);
        }
    }




    initChart(testsetting, testname) {
        let datasets = [];
        for (let i = 0; i < testsetting.threads; i++) {
            datasets.push({
                data: [],
                fill: false,
                label: i,
                lineTension: 0.1,
                radius: 0
            });
        }

        this.showChart(testname, datasets);
    }

    // gets called to start polling for chart data
    displayChart(scenario, testname, testId) {
        this.showChartData(scenario, testname, testId);
        this.showRampupData(scenario, testname, testId);

        this.poller = setInterval(() => {
            this.showChartData(scenario, testname, testId);
            this.showRampupData(scenario, testname, testId);
        }, 10000);
    }

    async showRampupData(scenario, testname, testId) {
        const url = `api/testdata/${scenario}/${testname}/rampupdata/${testId}`;
        try {
            const response = await fetch(url);
            if (!response.ok) {
                throw new Error(`Response status: ${response.status}`);
            }

            const data = await response.json();

            let datasets = [];

            let tmp = [];
            data.data.forEach(d => {
                tmp.push({ x: d.time, y: d.value });
            })
            datasets.push({
                data: tmp,
                fill: false,
                label: 'Rampup',
                //lineTension: 0.1,
                //radius: 0
            });

            if (tmp.length > 0) {
                this.showChart(`${testname}-rampup`, datasets);

                if (data.status == `Done` && this.poller) {
                    clearInterval(this.poller);
                }
            }

        } catch (error) {
            console.error(error.message);
        }
    }

    async showChartData(scenario, testname, testId) {
        const url = `api/testdata/${scenario}/${testname}/chartdata/${testId}`;
        try {
            const response = await fetch(url);
            if (!response.ok) {
                throw new Error(`Response status: ${response.status}`);
            }

            const data = await response.json();

            let datasets = [];

            data.chartData.forEach(function (a) {
                let tmpCd = [];
                a.data.forEach(d => {
                    tmpCd.push({ x: d.time, y: d.value });
                })
                datasets.push({
                    data: tmpCd,
                    fill: false,
                    label: a.name,
                    lineTension: 0.1,
                    radius: 0
                });
            }, Object.create(null));

            if (datasets.length > 0) {
                this.showChart(testname, datasets);



                //TODO: stop poller when state is done
                //TODO: show resultdata when state is done
            }

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
                            beginAtZero: true
                            //time: {
                            //    unit: 'second'
                            //}
                        },
                        y: {
                            stacked: true
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