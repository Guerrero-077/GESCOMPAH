import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DepartmentsCitiesComponent } from './departments-cities-component';

describe('DepartmentsCitiesComponent', () => {
  let component: DepartmentsCitiesComponent;
  let fixture: ComponentFixture<DepartmentsCitiesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DepartmentsCitiesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DepartmentsCitiesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
