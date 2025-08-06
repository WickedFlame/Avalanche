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

        let btn = document.querySelector('#btn_del_scenario')
        if (btn) {
            btn.addEventListener('click', e => {
                document.querySelector('#delScenario').classList.toggle('is-active');
            });
        }

        document.querySelectorAll('.pt-cancel-btn').forEach(btn => btn.addEventListener('click', e => {
            document.querySelector('#delScenario').classList.toggle('is-active');
        }));
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

            await response.json();

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

            await response.json();

            location.reload();
        } catch (error) {
            console.error(error.message);
        }
    }


    async initSummaryDataPoller(scenario, testid) {
        this.showSummaryData(scenario, testid);

        this.detailpoller = setInterval(() => {
            this.showSummaryData(scenario, testid);
        }, 3000);
    }

    async showSummaryData(scenario, testid) {
        const url = `api/testdata/${scenario}/summary/${testid}`;
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
                    row.querySelector('.AverageMs').innerHTML = s.averageMilliseconds;
                    row.querySelector('.Iterations').innerHTML = s.iterations;
                    row.querySelector('.Throughput').innerHTML = s.throughput;
                    row.querySelector('.Fails').innerHTML = s.failed;
                    row.querySelector('.Slowest').innerHTML = s.slowest;
                    row.querySelector('.Fastest').innerHTML = s.fastest;
                });
            }

            if (data.status == `Done`) {
                clearInterval(this.detailpoller);

                document.querySelector('#scenario-status').innerHTML = `Done`;
                document.querySelector('#StartTests').removeAttribute('disabled');
                document.querySelector('#StopTests').setAttribute('disabled', 'true');
            }

        } catch (error) {
            console.error(error.message);
        }
    }




    initChart(testsetting, testcase) {
        let datasets = [];
        for (let i = 0; i < testsetting.users; i++) {
            datasets.push({
                data: [],
                fill: false,
                label: i,
                lineTension: 0.1,
                radius: 0
            });
        }

        this.showChart(testcase, datasets);
    }

    // gets called to start polling for chart data
    displayChart(scenario, testcase, testId) {
        this.showChartData(scenario, testcase, testId);
        this.showRampupData(scenario, testcase, testId);
    }

    async showRampupData(scenario, testcase, testId) {
        const url = `api/testdata/${scenario}/${testcase}/rampupdata/${testId}`;
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
                this.showChart(`${testcase}-rampup`, datasets);
            }

        } catch (error) {
            console.error(error.message);
        }
    }

    async showChartData(scenario, testcase, testId) {
        const url = `api/testdata/${scenario}/${testcase}/chartdata/${testId}`;
        try {
            const response = await fetch(url);
            if (!response.ok) {
                throw new Error(`Response status: ${response.status}`);
            }

            const data = await response.json();

            let datasets = [];
            let tpData = [];

            data.chartData.forEach(function (a) {
                let tmpCd = [];
                let throughput = [];
                a.data.forEach(d => {
                    tmpCd.push({ x: d.time, y: d.milliseconds });
                    throughput.push({ x: d.time, y: d.throughput });
                })

                datasets.push({
                    data: tmpCd,
                    fill: false,
                    label: a.name,
                    //lineTension: 0.1,
                    //radius: 0
                });
                tpData.push({
                    data: throughput,
                    fill: false,
                    label: a.name
                });
            }, Object.create(null));

            if (datasets.length > 0) {
                this.showChart(`${testcase}-average`, datasets);
                this.showChart(`${testcase}-throughput`, tpData);
            }

        } catch (error) {
            console.error(error.message);
        }
    }

    async showChart(testcase, data) {
        let id = testcase.replace(/ /g, '_');

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
                            //distribution: 'linear',
                            //beginAtZero: true
                            //time: {
                            //    unit: 'second'
                            //}
                            //time: {
                            //    unit: 'hour',
                            //    unitStepSize: 0.5,
                            //    displayFormats: {
                            //        'hour': 'HH:mm:ss'
                            //    },
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