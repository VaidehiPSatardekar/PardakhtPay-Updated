//// tslint:disable:no-backbone-get-set-outside-model

//import { Injectable } from '@angular/core';

//import { BehaviorSubject } from 'rxjs/BehaviorSubject';
//import { Observable } from 'rxjs/Observable';
//import { of } from 'rxjs/observable/of';
//import { catchError } from 'rxjs/operators/catchError';
//import { map } from 'rxjs/operators/map';
//import { tap } from 'rxjs/operators/tap';


//@Injectable()
//export class MockEnvironmentService {

//    envSpecific: IEnvironment = {
//        serviceUrl: 'test/', serviceUrlHelpDesk: 'test/', CustomerWebsiteName: 'test/', imageUrlPrefix: 'test/',
//        testUsername: 'test', testPassword: 'test', testEmail: 'test@test.com', testTenantBrandName: 'test', testTenantBusinessName: 'test',
//        testTenantDescription: 'test', testTenantName: 'test', flagImageUrl: 'test', userServiceUrl: 'test/', tenantServiceUrl: 'test/', platformGuid: "0", allowAllTenants: false, roleGuid: []
//    };
//    envSpecificNull: IEnvironment = null;
//    private envSpecificSubject: BehaviorSubject<IEnvironment> = new BehaviorSubject<IEnvironment>(null);

//    loadEnvironment(): Observable<IEnvironment> {
//        return of(<IEnvironment>{
//            serviceUrl: 'test/', serviceUrlHelpDesk: 'test/', CustomerWebsiteName: 'test/', imageUrlPrefix: 'test/',
//            testUsername: 'test', testPassword: 'test', testEmail: 'test@test.com', testTenantBrandName: 'test', testTenantBusinessName: 'test',
//            testTenantDescription: 'test', testTenantName: 'test', flagImageUrl: 'test', userServiceUrl: 'test/', tenantServiceUrl: 'test/', platformGuid: "0", allowAllTenants: false, roleGuid: []
//        });
//    }

//    setEnvSpecific(es: IEnvironment): void {
//        // This has already been set so bail out.
//        if (es === null || es === undefined) {
//            return;
//        }
//        this.envSpecific = es;

//        if (this.envSpecificSubject) {
//            this.envSpecificSubject.next(this.envSpecific);
//        }
//    }

//    loadAndSetEnvironment(): Observable<boolean> {
//        return this.loadEnvironment().pipe(
//            tap((es: IEnvironment) => this.setEnvSpecific(es)),
//            map((es: IEnvironment) => true),
//            catchError((err: any) => {
//                console.log(err);

//                return of(false);
//            })
//        );
//    }

//    /*
//      Call this if you want to know when EnvSpecific is set.
//    */
//    subscribe(caller: any, callback: (caller: any, es: IEnvironment) => void): void {
//        callback(caller, <IEnvironment>{
//            serviceUrl: 'test/', serviceUrlHelpDesk: 'test/', CustomerWebsiteName: 'test/', imageUrlPrefix: 'test/',
//            testUsername: 'test', testPassword: 'test', testEmail: 'test@test.com', testTenantBrandName: 'test', testTenantBusinessName: 'test',
//            testTenantDescription: 'test', testTenantName: 'test', flagImageUrl: 'test', userServiceUrl: 'test/', tenantServiceUrl: 'test/', platformGuid: "0", allowAllTenants: false, roleGuid: []
//        });
//    }
//}
