export interface IAlert {
    title: string;
    description: string;
    type: 'warning' | 'info';
    text: string;
}