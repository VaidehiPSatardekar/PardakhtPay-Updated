// import { Injectable } from '@angular/core';
// import { CanActivate, Router } from '@angular/router';

// import { catchError } from 'rxjs/operators/catchError';
// import { map } from 'rxjs/operators/map';
// import { tap } from 'rxjs/operators/tap';

// import { Observable } from 'rxjs/Observable';
// import { of } from 'rxjs/observable/of';
// import { IEnvironment } from './environment.model';
// import { EnvironmentService } from './environment.service';

// @Injectable()
// export class EnvironmentGuard implements CanActivate {

//   constructor(private router: Router, private environmentService: EnvironmentService) { }

//   canActivate(): Observable<boolean> {
//     return this.environmentService.loadEnvironment().pipe(
//       tap((es: IEnvironment) => this.environmentService.setEnvSpecific(es)),
//       map(() => true),
//       catchError((err: any) => {
//         console.log(err);

//         return of(false);
//       })
//     );
//   }
// }
