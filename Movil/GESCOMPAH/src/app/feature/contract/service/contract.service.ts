import { inject, Injectable } from "@angular/core";
import { ContractStore } from "./contract.store";
import { Observable, of, tap } from "rxjs";
import { ContractCard } from "../models/contract.models";

const mockData: ContractCard[] = [
    {
        id: 1,
        personId: 101,
        personFullName: 'Juan Pérez',
        personDocument: '12345678',
        personPhone: '3001112222',
        personEmail: 'juan@example.com',
        startDate: '2025-01-01',
        endDate: '2025-12-31',
        totalBase: 1000,
        totalUvt: 35,
        active: true
    },
    {
        id: 2,
        personId: 102,
        personFullName: 'Ana Gómez',
        personDocument: '87654321',
        personPhone: '3003334444',
        personEmail: 'ana@example.com',
        startDate: '2025-03-01',
        endDate: '2025-11-30',
        totalBase: 1500,
        totalUvt: 50,
        active: false
    },
    {
        id: 3,
        personId: 103,
        personFullName: 'Carlos López',
        personDocument: '11223344',
        personPhone: '3005556666',
        personEmail: null,
        startDate: '2025-02-15',
        endDate: '2025-08-15',
        totalBase: 1200,
        totalUvt: 42,
        active: true
    }
];

@Injectable({
    providedIn: 'root'
})
export class ContractService {
    private readonly store = inject(ContractStore);

    readonly rows = this.store.rows;

    // Mock de datos



    getList(options: { force?: boolean } = {}): Observable<ContractCard[]> {
        const force = !!options.force;
        if (this.store.rows().length > 0 && !force) return of(this.store.rows());
        // return this.http.get<ContractCard[]>(${ this.baseUrl } / mine, { 
        //     withCredentials: true 
        // }).pipe(tap(list => this.store.setRows(list))); 
        return of(mockData).pipe(
            tap(list => this.store.setRows(list))
        );
    }
}