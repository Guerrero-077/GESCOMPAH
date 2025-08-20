import { Injectable } from "@angular/core";
import { BehaviorSubject, catchError, Observable, tap, throwError } from "rxjs";
import { TenantFormData, TenantsCreateModel, TenantsSelectModel, TenantsUpdateModel } from "../../models/tenants.models";
import { TenantsService } from './tenants.service';

@Injectable({
  providedIn: 'root'
})
export class tenantStore {

  private readonly _tenants = new BehaviorSubject<TenantsSelectModel[]>([]);
  readonly tenant$ = this._tenants.asObservable();

  constructor(private tenantService: TenantsService) {
    this.loadAll();
  }

  private get tenants(): TenantsSelectModel[] {
    return this._tenants.getValue();
  }

  private set tenants(val: TenantsSelectModel[]) {
    this._tenants.next(val);
  }

  loadAll() {
    this.tenantService.getAll().pipe(
      tap(data => this.tenants = data),
      catchError(err => {
        console.log('Error loading tenants', err);
        return throwError(() => err);
      })
    ).subscribe();
  }

  create(tenant: TenantsCreateModel): Observable<TenantsSelectModel> {
    return this.tenantService.create(tenant).pipe(
      tap(() => {
        this.loadAll();
      })
    )
  }

  update(id: number, updateDto: TenantsUpdateModel): Observable<TenantsSelectModel> {
    return this.tenantService.update(id, updateDto).pipe(
      tap(() => {
        this.loadAll();
      })
    )
  }

  delete(id: number): Observable<void> {
    return this.tenantService.delete(id).pipe(
      tap(() => {
        this.tenants = this.tenants.filter(c => c.id !== id);
      })
    )
  }

  deleteLogic(id: number): Observable<void> {
    return this.tenantService.deleteLogic(id).pipe(
      tap(() => {
        this.loadAll();
      })
    );
  }
  changeActiveStatus(id: number, active: boolean): Observable<TenantsSelectModel> {
    return this.tenantService.changeActiveStatus(id, active).pipe(
      tap(() => {
        this.loadAll();
      })
    );
  }
}
