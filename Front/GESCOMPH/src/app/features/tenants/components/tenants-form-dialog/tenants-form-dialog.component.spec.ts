import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TenantsFormDialogComponent } from './tenants-form-dialog.component';

describe('TenantsFormDialogComponent', () => {
  let component: TenantsFormDialogComponent;
  let fixture: ComponentFixture<TenantsFormDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TenantsFormDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TenantsFormDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
