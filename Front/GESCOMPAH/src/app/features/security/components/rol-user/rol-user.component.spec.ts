import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RolUserComponent } from './rol-user.component';

describe('RolUserComponent', () => {
  let component: RolUserComponent;
  let fixture: ComponentFixture<RolUserComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RolUserComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RolUserComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
