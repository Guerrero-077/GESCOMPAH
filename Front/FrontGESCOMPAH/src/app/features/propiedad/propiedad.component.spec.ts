import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PropiedadComponent } from './propiedad.component';

describe('PropiedadComponent', () => {
  let component: PropiedadComponent;
  let fixture: ComponentFixture<PropiedadComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PropiedadComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PropiedadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
