import {
  Directive, inject, Input,
  TemplateRef, ViewContainerRef
} from '@angular/core';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { BehaviorSubject, combineLatest, map } from 'rxjs';
import { PermissionService } from '../services/permission/permission.service';
import { UserStore } from '../services/permission/User.Store';

type Mode = 'any' | 'all';

@Directive({
  selector: '[appHasRoleAndPermission]',
  standalone: true,
})
export class HasRoleAndPermissionDirective {
  private tpl = inject(TemplateRef<any>);
  private vcr = inject(ViewContainerRef);
  private permissionsSvc = inject(PermissionService);
  private userStore = inject(UserStore);

  private roles$ = new BehaviorSubject<string[] | null>(null);
  private perms$ = new BehaviorSubject<string[] | null>(null);
  private mode$ = new BehaviorSubject<Mode>('any');
  private route$ = new BehaviorSubject<string | null>(null);
  private shown = false;

  @Input('appHasRoleAndPermission') set roles(v: string | string[]) {
    this.roles$.next(Array.isArray(v) ? v : [v]);
  }
  @Input('appHasRoleAndPermissionPerms') set perms(v: string | string[]) {
    this.perms$.next(Array.isArray(v) ? v : [v]);
  }
  @Input('appHasRoleAndPermissionMode') set mode(v: Mode) {
    this.mode$.next(v ?? 'any');
  }
  @Input('appHasRoleAndPermissionForRoute') set forRoute(r: string) {
    this.route$.next(r ?? null);
  }

  constructor() {
    combineLatest([
      toObservable(this.userStore.user),
      this.roles$, this.perms$, this.mode$, this.route$
    ])
      .pipe(
        map(([user, roles, perms, mode, route]) => {
          if (!user) return false;
          return this.checkRoles(roles) && this.checkPerms(perms, mode, route);
        }),
        takeUntilDestroyed()
      )
      .subscribe(ok => {
        if (ok && !this.shown) {
          this.vcr.clear();
          this.vcr.createEmbeddedView(this.tpl);
          this.shown = true;
        } else if (!ok && this.shown) {
          this.vcr.clear();
          this.shown = false;
        }
      });
  }

  private checkRoles(roles: string[] | null): boolean {
    if (!roles || roles.length === 0) return true;
    return roles.some(r => this.permissionsSvc.hasRole(r));
  }

  private checkPerms(perms: string[] | null, mode: Mode, route: string | null): boolean {
    if (!perms || perms.length === 0) return true;
    const check = (p: string) =>
      route ? this.permissionsSvc.hasPermissionForRoute(p, route)
        : this.permissionsSvc.hasPermission(p);
    return mode === 'all' ? perms.every(check) : perms.some(check);
  }
}
