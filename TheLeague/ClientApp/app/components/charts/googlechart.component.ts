import { Component, OnInit } from '@angular/core';

declare var google: any;

@Component({
    selector: 'googlechart',
    template: '<ng-content></ng-content>'
})

export class GoogleChartComponent implements OnInit {
    private static googleLoaded: any;

    constructor() {}

    getGoogle() {
        return google;
    }

    ngOnInit(): void {
        if (!GoogleChartComponent.googleLoaded) {
            GoogleChartComponent.googleLoaded = true;
            google.charts.load('current', { 'packages': ['corechart'] });
        }

        google.charts.setOnLoadCallback(() => this.drawChart());
    }

    drawChart() { }

    createDataTable(array: any[]):any {
        return google.visualization.arrayToDataTable(array);
    }

    createLineChart(element: any):any {
        return new google.visualization.LineChart(element);
    }

    convertOptions(options: any):any {
        return google.charts.Line.convertOptions(options);
    }
}