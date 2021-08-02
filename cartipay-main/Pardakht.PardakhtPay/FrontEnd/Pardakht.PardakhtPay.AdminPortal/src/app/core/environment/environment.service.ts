// tslint:disable:no-backbone-get-set-outside-model
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { IEnvironment } from './environment.model';

@Injectable()
export class EnvironmentService {

  envSpecific: IEnvironment;
  envSpecificNull: IEnvironment = null;

  private envSpecificSubject: BehaviorSubject<IEnvironment> = new BehaviorSubject<IEnvironment>(null);

  constructor(private http: HttpClient) { }

  setEnvSpecific(es: IEnvironment): void {
    // console.log('setting environment:');
    // console.log(es.serviceUrl);
    // This has already been set so bail out.
    if (es === null || es === undefined) {
      return;
    }
    this.envSpecific = es;

    if (this.envSpecificSubject) {
      this.envSpecificSubject.next(this.envSpecific);
    }
  }

  // we need to load environment settings first to get the service url
  // this is done as a promise to the app.module knows when to start loading the rest of the app
  // permissions are reqd when constructing the ui navigation links, therefore they need to be loaded here
  loadAndSetEnvironment(): Promise<any> {
    return new Promise(resolve => {
      const promise: any = this.http.get<IEnvironment>('../../../../assets/environment.json')
        .subscribe((config: IEnvironment) => {
          this.setEnvSpecific(config);
          resolve();
        });
    });
  }

  /*
    Call this if you want to know when EnvSpecific is set.
  */
  subscribe(caller: any, callback: (caller: any, es: IEnvironment) => void): void {
    this.envSpecificSubject
      .subscribe((es: IEnvironment) => {
        if (es === null) {
          return;
        }
        callback(caller, es);
      });
  }
}
