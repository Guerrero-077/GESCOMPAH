import { ComponentFixture, TestBed } from '@angular/core/testing';
import { GenericTableComponents } from './generic-table-components';

interface DummyEntity {
  id: number;
  nombre: string;
  descripcion: string;
}

describe('GenericTableComponents', () => {
  let component: GenericTableComponents<DummyEntity>;
  let fixture: ComponentFixture<GenericTableComponents<DummyEntity>>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GenericTableComponents]
    }).compileComponents();

    fixture = TestBed.createComponent(GenericTableComponents<DummyEntity>);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
