import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LocalCard } from './local-card';

describe('LocalCard', () => {
  let component: LocalCard;
  let fixture: ComponentFixture<LocalCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LocalCard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LocalCard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
