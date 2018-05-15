import * as ko from 'knockout';
import * as History from 'history';
import { Route, Router } from '../../router';
import navMenu from '../nav-menu/nav-menu';

// Declare the client-side routing configuration
const routes: Route[] = [
    { url: '', params: { page: 'homes' } },
    { url: 'water-meters', params: { page: 'water-meters' } },
    { url: 'fetch-data', params: { page: 'fetch-data' } }
];

class AppRootViewModel {
    public route: KnockoutObservable<Route>;
    public router: Router;

    constructor(params: { history: History.History, basename: string }) {
        // Activate the client-side router
        this.router = new Router(params.history, routes, params.basename);
        this.route = this.router.currentRoute;

        // Load and register all the KO components needed to handle the routes
        // The optional 'bundle-loader?lazy!' prefix is a Webpack feature that causes the referenced modules
        // to be split into separate files that are then loaded on demand.
        // For docs, see https://github.com/webpack/bundle-loader
        ko.components.register('nav-menu', navMenu);
        ko.components.register('homes', require('bundle-loader?lazy!../homes/homes'));
        ko.components.register('water-meters', require('bundle-loader?lazy!../water-meters/water-meters'));
        ko.components.register('fetch-data', require('bundle-loader?lazy!../fetch-data/fetch-data'));
    }

    // To support hot module replacement, this method unregisters the router and KO components.
    // In production scenarios where hot module replacement is disabled, this would not be invoked.
    public dispose() {
        this.router.dispose();

        // TODO: Need a better API for this
        Object.getOwnPropertyNames((<any>ko).components._allRegisteredComponents).forEach(componentName => {
            ko.components.unregister(componentName);
        });
    }
}

export default { viewModel: AppRootViewModel, template: require('./app-root.html') };
