import { FormControl } from "@angular/forms";

/* ===== Formularios *tipados* (Typed Forms) =====
   Definimos las "shape" de los form groups para aprovechar intellisense y type-safety */
export type GeneralForm = {
  name: FormControl<string>;
  description: FormControl<string>;
  rentValueBase: FormControl<number>;
  uvtQty: FormControl<number>;
  areaM2: FormControl<number>;
};
export type UbicacionForm = {
  plazaId: FormControl<number>;
  address: FormControl<string>;
};
