import * as ko from 'knockout';
import 'isomorphic-fetch';

interface HomeEntity {
    id: number;
    address: string;
}

class FetchDataViewModel {
    public addressMax = ko.observable();
    public idMax = ko.observable();

    public addressMin = ko.observable();
    public idMin = ko.observable();

    public fullMin = ko.computed(() => {
        return 'id:' + this.idMin() + ' | adress:' + this.addressMin();
    });

    public fullMax = ko.computed(() => {
        return 'id:' + this.idMax() + ' | adress:' + this.addressMax();
    });

    constructor() {
        fetch('api/HomeEntities/max')
            .then(response => {
                if (response.ok) {
                    return (response.json() as Promise<HomeEntity>);
                }
                else {
                    if (response.status == 400) {
                        return <HomeEntity>{};
                    } else {
                        throw new Error('Error getting data');
                    }
                }
            })
            .then(homeEntity => {
                this.addressMax(homeEntity.address);
                this.idMax(homeEntity.id);
            })
            .catch(error => {
                alert(error)
            });

        fetch('api/HomeEntities/min')
            .then(response => {
                if (response.ok) {
                    return (response.json() as Promise<HomeEntity>);
                }
                else {
                    if (response.status == 400) {
                        return <HomeEntity>{};
                    } else {
                        throw new Error('Error getting data');
                    }
                }
            })
            .then(homeEntity => {
                this.addressMin(homeEntity.address);
                this.idMin(homeEntity.id);
            })
            .catch(error => {
                alert(error)
            });
    }
}

export default { viewModel: FetchDataViewModel, template: require('./fetch-data.html') };
