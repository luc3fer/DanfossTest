import * as ko from 'knockout';

interface WaterMeter {
    id: number;
    serialNumber: string;
    meter: number;
    homeEntityId: number;
    homeEntity?: HomeEntity;
}

interface HomeEntity {
    id: number;
    address: string;
    waterMeter?: WaterMeter
}

class WaterMeterViewModel {
    public sbmtWaterMeter = ko.observable();

    public waterMeters = ko.observableArray<WaterMeter>();

    public readingsWaterMeter = ko.computed({
        read: () => {
            var test = this.sbmtWaterMeter() == undefined ? -1 : (this.sbmtWaterMeter() as WaterMeter).meter;
            return test;
        },
        write: (value) => {
            if (this.sbmtWaterMeter() !== undefined) {
                (this.sbmtWaterMeter() as WaterMeter).meter = value;
            }
        },
        owner: this
    });

    constructor() {
        fetch('api/WaterMeters')
            .then(response => response.json() as Promise<WaterMeter[]>)
            .then(data => this.waterMeters(data));

        ko.bindingHandlers.numeric = {
            init: function (element: HTMLElement, valueAccessor) {
                element.addEventListener("keydown", function (event: KeyboardEvent) {
                    // Allow: backspace, delete, tab, escape, and enter
                    if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 27 || event.keyCode == 13 ||
                        // Allow: Ctrl+A
                        (event.keyCode == 65 && event.ctrlKey === true) ||
                        // Allow: . ,
                        (event.keyCode == 188 || event.keyCode == 190 || event.keyCode == 110) ||
                        // Allow: home, end, left, right
                        (event.keyCode >= 35 && event.keyCode <= 39)) {
                        // let it happen, don't do anything
                        return;
                    }
                    else {
                        // Ensure that it is a number and stop the keypress
                        if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                            event.preventDefault();
                        }
                    }
                });
            }
        };
    }

    public updateMeter() {
        let params = new URLSearchParams();
        params.append('serial_number', (this.sbmtWaterMeter() as WaterMeter).serialNumber.toString());
        params.append('meter', (this.sbmtWaterMeter() as WaterMeter).meter.toString());

        var url = 'api/WaterMeters/SubmitMeter/by_serial_number?' + params.toString();
        fetch(url,
            {
                method: "PUT",
                headers: { 'Content-Type': 'application/json' }
            })
            .then(response => {
                if (response.ok) {
                    return (response.json() as Promise<WaterMeter>);
                } else {
                    throw new Error('Error for submitting');
                }
            })
            .then(waterMeter => {
                this.updateWaterMetersArray(waterMeter);
                this.sbmtWaterMeter(null);
            })
            .catch(error => {
                alert(error);
            });
    }

    submitWaterMeter = (waterMeter: WaterMeter) => {
        this.sbmtWaterMeter(null);
        this.sbmtWaterMeter(<WaterMeter>{ serialNumber: waterMeter.serialNumber, meter: waterMeter.meter });
    }

    private updateWaterMetersArray(newWaterMeter: WaterMeter) {
        this.waterMeters().some(function (value) {
            if (value.id === newWaterMeter.id) {
                value.meter = newWaterMeter.meter;
                return true;
            }
            return false;
        });

        //¯\_(ツ)_/¯ 
        var data = this.waterMeters().slice(0);
        this.waterMeters([]);
        this.waterMeters(data);
    }
}

export default { viewModel: WaterMeterViewModel, template: require('./water-meters.html') };
