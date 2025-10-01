GESCOMPAH – Estándar de Formularios (Bootstrap + Angular Material)

Objetivo

- Usar una estructura y estilos consistentes en todos los formularios
- Combinar Bootstrap para el layout y Angular Material para campos y validaciones
- Centralizar mensajes de error y espaciados

Layout base

- Contenedor: usar `form-container` o `form-card` según sea página o modal
- Grid: usar `row g-3` y columnas Bootstrap (`col-12 col-md-6`, etc.) o las utilidades `.form-grid`
- Campo: siempre con `mat-form-field` `appearance="outline"` y ancho completo
- Acciones: usar `form-actions` con variantes `space-between`, `center`, `start`

Snippet recomendado

```
<form [formGroup]="form" class="form-container" (ngSubmit)="onSubmit()">
  <div class="row g-3">
    <div class="col-12 col-md-6">
      <mat-form-field appearance="outline" class="w-100">
        <mat-label>Nombre</mat-label>
        <input matInput formControlName="name" (blur)="form.get('name')?.markAsTouched()" />
        <app-form-error [control]="form.get('name')" label="Nombre"></app-form-error>
      </mat-form-field>
    </div>

    <div class="col-12 col-md-6">
      <mat-form-field appearance="outline" class="w-100">
        <mat-label>Correo</mat-label>
        <input matInput type="email" formControlName="email" />
        <mat-hint class="hint-email"></mat-hint>
        <app-form-error [control]="form.get('email')" label="Correo"></app-form-error>
      </mat-form-field>
    </div>
  </div>

  <div class="form-actions space-between">
    <app-standard-button text="Cancelar" variant="stroked" (clicked)="onCancel()"></app-standard-button>
    <app-standard-button text="Guardar" variant="raised" color="primary" icon="save" [disabled]="form.invalid"></app-standard-button>
  </div>
</form>
```

Mensajes de error unificados

- Usa el componente `app-form-error` bajo cada `mat-form-field`
- Opcional: sobreescribe mensajes específicos con `messages` si un campo necesita textos distintos

```
<app-form-error
  [control]="form.get('password')"
  label="Contraseña"
  [messages]="{ minlength: () => 'Mínimo 8 caracteres' }"
></app-form-error>
```

Buenas prácticas

- Activar validación en `blur` cuando el campo sea sensible a escritura (p. ej. números/monedas)
- Normalizar strings al enviar (trim + espacios simples)
- Evitar duplicar mensajes en templates; preferir `app-form-error` o el `DymanicFormsComponent`
- Para formularios largos, dividir en secciones con `.section-header`

Notas

- Los estilos globales están en `src/styles/standardized-forms.css`
- Bootstrap ya está incluido vía `angular.json` (CSS y bundle JS)
- Para formularios generados, usa `app-dymanic-forms` que ya aplica reglas consistentes

