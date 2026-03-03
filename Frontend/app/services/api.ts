const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL;

async function apiRequest<T>(path: string, options: RequestInit = {}): Promise<T> {
    const url = `${API_BASE_URL}/${path.replace(/^\//, '')}`;

    const response = await fetch(url, {
        ...options,
        credentials: "include",
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/problem+json',
            ...options.headers,
        },
    });

    if (!response.ok) {
        const errorBody = await response.json();
        throw errorBody;
    }

    // Handle empty responses (like 204 No Content for Delete)
    if (response.status === 204) return {} as T;

    return response.json();
}

export const bookingService = {
    // POST Add Ticket
    addTicket: (data: {
        ticketName: string;
        ticketCode: string;
        categoryName: string;
        eventDate: string;
        price: number;
        quota: number;
    }) => {
        return apiRequest<any>('api/v1/add-ticket', {
            method: 'POST',
            body: JSON.stringify(data),
        });
    },

    // GET Available Tickets with searching and pagination
    getAvailableTickets: (params: Record<string, any>) => {
        const query = new URLSearchParams(params).toString();
        return apiRequest<any>(`api/v1/get-available-tickets?${query}`);
    },

    //GET All Booked Tickets for User
    getAllBookings: () => {
        return apiRequest<any>('api/v1/get-all-bookings');
    },

    // POST Book Tickets
    bookTickets: (data: { tickets: { ticketCode: string; quantity: number }[] }) => {
        return apiRequest<any>('api/v1/book-ticket', {
            method: 'POST',
            body: JSON.stringify(data),
        });
    },

    // GET Booked Ticket Details (Grouped by Category)
    getBookedTicketDetail: (bookedTicketId: string | number) => {
        return apiRequest<any>(`api/v1/get-booked-ticket/${bookedTicketId}`);
    },

    // DELETE Revoke Ticket
    revokeTicket: (bookedTicketId: string | number, ticketCode: string, qty: number) => {
        return apiRequest<any>(`api/v1/revoke-ticket/${bookedTicketId}/${ticketCode}/${qty}`, {
            method: 'DELETE',
        });
    },

    // PUT Edit Booked Ticket
    editBookedTicket: (bookedTicketId: string | number, data: { tickets: { ticketCode: string; quantity: number }[] }) => {
        return apiRequest<any>(`api/v1/edit-booked-ticket/${bookedTicketId}`, {
            method: 'PUT',
            body: JSON.stringify(data),
        });
    }
}

export const authService = {
    // GET Check Authentication Status
    checkAuthStatus: () => {
        return apiRequest<any>('api/v1/auth/status', {
            method: 'GET',
            credentials: 'include'
        });
    },

    // POST Logout
    logout: () => {
        return apiRequest<any>('api/v1/auth/logout', {
            method: 'POST',
            credentials: 'include'
        });
    }
}