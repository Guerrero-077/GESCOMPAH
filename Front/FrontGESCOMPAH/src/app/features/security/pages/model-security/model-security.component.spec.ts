import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModelSecurityComponent } from './model-security.component';

describe('ModelSecurityComponent', () => {
  let component: ModelSecurityComponent;
  let fixture: ComponentFixture<ModelSecurityComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ModelSecurityComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ModelSecurityComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
