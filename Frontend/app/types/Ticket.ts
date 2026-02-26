// types/ticket.ts

export interface Ticket {
    eventDate: string;
    quota: number;
    ticketCode: string;
    ticketName: string;
    categoryName: string;
    price: number;
}

export interface GetAvailableTicketResponse {
    tickets: Ticket[];
    totalTickets: number;
}

export interface TicketFilters {
    categoryName: string;
    ticketCode: string;
    ticketName: string;
    maxPrice: string | number;
    minEventDate: string;
    maxEventDate: string;
    orderBy: string;
    orderState: string;
}