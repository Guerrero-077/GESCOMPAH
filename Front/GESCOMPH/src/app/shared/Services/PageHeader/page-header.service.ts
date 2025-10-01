import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class PageHeaderService {
  title = signal('');
  description = signal('');

  setPageHeader(title: string, description: string) {
    this.title.set(title);
    this.description.set(description);
  }
}
