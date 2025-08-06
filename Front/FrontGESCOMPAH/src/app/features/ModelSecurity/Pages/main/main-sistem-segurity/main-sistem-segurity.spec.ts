import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MainSistemSegurity } from './main-sistem-segurity';

describe('MainSistemSegurity', () => {
  let component: MainSistemSegurity;
  let fixture: ComponentFixture<MainSistemSegurity>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MainSistemSegurity]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MainSistemSegurity);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
