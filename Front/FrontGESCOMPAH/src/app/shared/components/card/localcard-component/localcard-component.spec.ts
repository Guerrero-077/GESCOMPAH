import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LocalcardComponent } from './localcard-component';

describe('LocalcardComponent', () => {
  let component: LocalcardComponent;
  let fixture: ComponentFixture<LocalcardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LocalcardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LocalcardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
