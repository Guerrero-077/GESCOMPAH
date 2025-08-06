import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MainConfig } from './main-config';

describe('MainConfig', () => {
  let component: MainConfig;
  let fixture: ComponentFixture<MainConfig>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MainConfig]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MainConfig);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
