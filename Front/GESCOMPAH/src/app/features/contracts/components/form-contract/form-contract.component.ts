import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ReactiveFormsModule, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { EstablishmentSelect } from '../../../establishments/models/establishment.models';
import { EstablishmentService } from '../../../establishments/services/establishment/establishment.service';
import { PersonService } from '../../../security/services/person/person.service';
import { CitySelectModel } from '../../../setting/models/city.models';
import { CityService } from '../../../setting/services/city/city.service';
import { ContractCreateModel } from '../../models/contract.models';
import { ContractService } from '../../services/contract/contract.service';

@Component({
  selector: 'app-form-contract',
  imports: [

    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatSlideToggleModule,
    MatDatepickerModule,
    MatNativeDateModule
  ],
  templateUrl: './form-contract.component.html',
  styleUrl: './form-contract.component.css'
})
export class FormContractComponent implements OnInit {
  form!: FormGroup;

  ciudades: CitySelectModel[] = [];
  establecimientos: EstablishmentSelect[] = [];

  personaEncontrada = false;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private ciudadService: CityService,
    private establishmentService: EstablishmentService,
    private contractService: ContractService,
    private personService: PersonService
  ) { }

  ngOnInit(): void {
    this.initForm();
    this.loadCiudades();
    this.loadEstablecimientos();
  }

  initForm(): void {
    this.form = this.fb.group({
      startDate: [null, Validators.required],
      endDate: [null, Validators.required],
      address: ['', Validators.required],        // ✔ Nombre correcto
      cityId: [null, Validators.required],       // ✔ Nombre correcto
      document: ['', Validators.required],
      firstName: [''],                           // Solo requerido si persona no existe
      lastName: [''],
      phone: [''],
      email: [''],
      establishmentIds: [[], Validators.required],
      calculationKey: ['', Validators.required],
    });
  }

  loadCiudades(): void {
    this.ciudadService.getAll().subscribe({
      next: (res) => (this.ciudades = res),
      error: (err) => console.error('Error al cargar ciudades', err)
    });
  }

  loadEstablecimientos(): void {
    this.establishmentService.getAll().subscribe({
      next: (res) => (this.establecimientos = res),
      error: (err) => console.error('Error al cargar establecimientos', err)
    });
  }

  buscarPersona(): void {
    const document = this.form.get('document')?.value;
    if (!document) return;

    this.loading = true;
    this.personService.getByDocument(document).subscribe({
      next: (persona) => {
        if (persona) {
          this.personaEncontrada = true;
          this.form.patchValue({
            firstName: persona.firstName,
            lastName: persona.lastName,
            phone: persona.phone,
            email: persona.email ?? ''
          });
        } else {
          this.personaEncontrada = false;
          this.form.patchValue({
            firstName: '',
            lastName: '',
            phone: '',
            email: ''
          });
        }
      },
      error: (err) => {
        console.error('Error al buscar persona', err);
        this.personaEncontrada = false;
      },
      complete: () => (this.loading = false)
    });
  }

  submit(): void {
    if (this.form.invalid) return;

    const payload: ContractCreateModel = this.form.value;

    this.contractService.createContract(payload).subscribe({
      next: (res) => {
        console.log('✅ Contrato creado con ID:', res);
        this.form.reset();
        this.personaEncontrada = false;
        // TODO: Agrega notificación (toast) si lo deseas
      },
      error: (err) => {
        console.error('❌ Error al crear contrato', err);
      }
    });
  }
}
