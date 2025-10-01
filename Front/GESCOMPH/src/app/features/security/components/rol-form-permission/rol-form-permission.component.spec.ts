import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RolFormPermissionComponent } from './rol-form-permission.component';

describe('RolFormPermissionComponent', () => {
  let component: RolFormPermissionComponent;
  let fixture: ComponentFixture<RolFormPermissionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RolFormPermissionComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RolFormPermissionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
