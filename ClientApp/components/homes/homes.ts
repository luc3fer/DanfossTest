import * as ko from 'knockout';
import 'isomorphic-fetch';

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

class HomeViewModel {
    public address = ko.observable('');

    public editableHome = ko.observable();
    public regWaterMeter = ko.observable();
    public sbmtWaterMeter = ko.observable();

    public homes = ko.observableArray<HomeEntity>();

    public addressEditableHome = ko.computed({
        read: () => {
            return this.editableHome() == undefined ? 'undefined' : (this.editableHome() as HomeEntity).address;
        },
        write: (value) => {
            if (this.editableHome() !== undefined) {
                (this.editableHome() as HomeEntity).address = value;
            }
        },
        owner: this
    });

    public serialNumberRegWaterMeter = ko.computed({
        read: () => {
            return this.regWaterMeter() == undefined ? 'undefined' : (this.regWaterMeter() as WaterMeter).serialNumber;;
        },
        write: (value) => {
            if (this.regWaterMeter() !== undefined) {
                (this.regWaterMeter() as WaterMeter).serialNumber = value;
            }
        },
        owner: this
    });

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
        fetch('api/HomeEntities')
            .then(response => {
                if (response.ok) {
                    return (response.json() as Promise<HomeEntity[]>);
                }
                else {
                    throw new Error('Error requesting data');
                }
            })
            .then(data => this.homes(data))
            .catch(error => {
                alert(error)
            });

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

    public addHome() {
        fetch('api/HomeEntities',
            {
                method: "POST",
                body: JSON.stringify(<HomeEntity>{ address: this.address(), id: 0 }),
                headers: { 'Content-Type': 'application/json' }
            })
            .then(response => {
                if (response.ok) {
                    return (response.json() as Promise<HomeEntity>);
                }
                else {
                    if (response.status == 409) {
                        throw new Error('Duplicate address');
                    }
                    else {
                        throw new Error('Error requesting data');
                    }
                }
            })
            .then(home => {
                this.homes.push(home);
                this.address('');
            })
            .catch(error => {
                alert(error)
            });
    }

    removeHome = (home: HomeEntity) => {
        var url = 'api/HomeEntities/' + home.id;

        fetch(url,
            {
                method: "DELETE",
                headers: { 'Content-Type': 'application/json' }
            })
            .then(response => {
                if (response.ok) {
                    return (response.json() as Promise<HomeEntity>);
                }
                else {
                    throw new Error('Error deleting data');
                }
            })
            .then(data => {
                this.homes.remove(function (deletedHome) {
                    return deletedHome.id == data.id;
                });
            })
            .catch(error => {
                alert(error)
            });
    }

    public saveHome() {
        var url = 'api/HomeEntities/' + (this.editableHome() as HomeEntity).id;

        fetch(url,
            {
                method: "PUT",
                body: JSON.stringify(this.editableHome()),
                headers: { 'Content-Type': 'application/json' }
            })
            .then(response => {
                if (response.ok) {
                    this.updateHomesArray(this.editableHome() as HomeEntity);
                    this.editableHome(null);
                } else {
                    if (response.status == 409) {
                        throw new Error('Duplicate address');
                    }
                    else {
                        throw new Error('Error updating data');
                    }
                }
            })
            .catch(error => {
                alert(error)
            });
    }

    public addWaterMeter() {
        fetch('api/WaterMeters/reg',
            {
                method: "POST",
                body: JSON.stringify(<WaterMeter>{
                    serialNumber: (this.regWaterMeter() as WaterMeter).serialNumber,
                    homeEntityId: (this.regWaterMeter() as WaterMeter).homeEntityId,
                    id: (this.regWaterMeter() as WaterMeter).id
                }),
                headers: { 'Content-Type': 'application/json' }
            })
            .then(response => {
                if (response.ok) {
                    return (response.json() as Promise<HomeEntity>);
                }
                else {
                    throw new Error('Error requesting data');
                }
            })
            .then(homeEntity => {
                this.updateHomesArray(homeEntity);
                this.regWaterMeter(null);
            })
            .catch(error => {
                alert(error)
            });
    }

    deleteWaterMeter = (home: HomeEntity) => {
        var url = 'api/WaterMeters/' + (home.waterMeter as WaterMeter).id;

        fetch(url,
            {
                method: "DELETE",
                headers: { 'Content-Type': 'application/json' }
            })
            .then(response => {
                if (response.ok) {
                    return (response.json() as Promise<HomeEntity>);
                }
                else {
                    throw new Error('Error deleting data');
                }
            })
            .then(homeEntity => {
                this.sbmtWaterMeter(null);
                this.updateHomesArray(homeEntity);
            })
            .catch(error => {
                alert(error)
            });
    }

    public updateMeter() {
        let params = new URLSearchParams();
        params.append('home_id', (this.sbmtWaterMeter() as WaterMeter).homeEntityId.toString());
        params.append('meter', (this.sbmtWaterMeter() as WaterMeter).meter.toString());

        var url = 'api/WaterMeters/SubmitMeter/by_home_id?' + params.toString();
        fetch(url,
            {
                method: "PUT",
                headers: { 'Content-Type': 'application/json' }
            })
            .then(response => {
                if (response.ok) {
                    return (response.json() as Promise<HomeEntity>);
                } else {
                    throw new Error('Error for submitting');
                }
            })
            .then(homeEntity => {
                this.updateHomesArray(homeEntity);
                this.sbmtWaterMeter(null);
            })
            .catch(error => {
                alert(error);
            });
    }

    editHome = (home: HomeEntity) => {
        this.resetAllEditableObject();
        this.editableHome(<HomeEntity>{ id: home.id, address: home.address, waterMeter: home.waterMeter });
    }

    registerWaterMeter = (home: HomeEntity) => {
        this.resetAllEditableObject();
        this.regWaterMeter(<WaterMeter>{
            id: ((home.waterMeter == undefined) ? 0 : home.waterMeter.id),
            homeEntityId: home.id,
            serialNumber: ((home.waterMeter == undefined) ? '' : home.waterMeter.serialNumber)
        });
    }

    submitWaterMeter = (home: HomeEntity) => {
        this.resetAllEditableObject();
        this.sbmtWaterMeter(<WaterMeter>{ homeEntityId: home.id, meter: ((home.waterMeter == undefined) ? 0 : home.waterMeter.meter) });
    }

    private updateHomesArray(newHome: HomeEntity) {
        this.homes().some(function (value) {
            if (value.id === newHome.id) {
                value.address = newHome.address;
                value.waterMeter = newHome.waterMeter;
                return true;
            }
            return false;
        });

        //¯\_(ツ)_/¯
        var data = this.homes().slice(0);
        this.homes([]);
        this.homes(data);
    }

    private resetAllEditableObject() {
        this.editableHome(null);
        this.regWaterMeter(null);
        this.sbmtWaterMeter(null);
    }
}

export default { viewModel: HomeViewModel, template: require('./homes.html') };
