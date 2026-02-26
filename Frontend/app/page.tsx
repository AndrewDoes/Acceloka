'use client'
import Image from "next/image";
import { useEffect, useState } from "react";
import { Ticket, TicketFilters } from "./types/Ticket";
import { bookingService } from "./services/api";

export default function Home() {

  const [tickets, setTickets] = useState<Ticket[]>([]);
  const [ticketCount, setTicketCount] = useState(0);
  const [loading, setLoading] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);

  const [filters, setFilters] = useState<TicketFilters>({
    categoryName: "",
    ticketCode: "",
    ticketName: "",
    maxPrice: "",
    minEventDate: "",
    maxEventDate: "",
    orderBy: "ticketCode",
    orderState: "asc",
  });

  useEffect(() => {
    setLoading(true);
    const fetchTickets = async () => {
      try {
        const response = await bookingService.getAvailableTickets({
          ...filters,
          page: currentPage,
        });
        setTickets(response.tickets);
        setTicketCount(response.totalTickets);
      } catch (err: any) {
        console.error("Fetch error: ", err);
      } finally {
        setLoading(false);
      }
    }
  }, [currentPage])



  return (
    <div className="">

    </div>
  );
}
