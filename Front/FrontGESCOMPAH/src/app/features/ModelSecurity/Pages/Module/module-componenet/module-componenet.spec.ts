import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModuleComponenet } from './module-componenet';

describe('ModuleComponenet', () => {
  let component: ModuleComponenet;
  let fixture: ComponentFixture<ModuleComponenet>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ModuleComponenet]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ModuleComponenet);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
