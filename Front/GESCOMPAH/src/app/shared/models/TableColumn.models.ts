import { TemplateRef } from "@angular/core";

// export interface TableColumn<T> {
//     key: keyof T | string;
//     header: string;
//     sortable?: boolean;
//     type?: 'text' | 'number' | 'boolean' | 'badge' | 'actions' | 'custom' | 'index';
//     template?: TemplateRef<any>;
// }


export interface TableColumn<T> {
  /** Nombre del campo en el objeto T */
  key: keyof T | string;

  /** Encabezado visible en la tabla */
  header: string;

  /** ¿La columna se puede ordenar? */
  sortable?: boolean;

  /**
   * Tipo de contenido:
   * - `text`: valor plano
   * - `number`: se puede alinear a la derecha, por ejemplo
   * - `boolean`: puede usarse para íconos o chips
   * - `badge`: para mostrar estado en colores
   * - `actions`: para incluir botones por fila
   * - `index`: para numeración incremental
   * - `custom`: se usa un TemplateRef
   */
  type?: 'text' | 'number' | 'boolean' | 'badge' | 'actions' | 'custom' | 'index';

  /**
   * Permite personalizar el rendering con un TemplateRef.
   * Solo válido si type === 'custom'.
   */
  template?: TemplateRef<any>;

  /**
   * Formateador opcional para transformar el valor
   */
  format?: (value: any, row?: T) => string;

  /**
   * Alineación opcional por columna (left, center, right)
   */
  align?: 'left' | 'center' | 'right';
  render?: (row: T) => unknown;
}
