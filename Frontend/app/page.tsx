'use client';

import { useEffect, useMemo, useState } from "react";
import { Ticket, TicketFilters } from "./types/Ticket";
import { bookingService } from "./services/api";
import HeroSection from "./components/hero";
import FilterBar from "./components/filterbar";
import TicketGrid from "./components/ticketgrid";
import { TicketData } from "./components/ticketcard";
import BookingDrawer from "./components/bookingdrawer";
import { Button, Typography, App, ConfigProvider, theme } from "antd";
import { CheckCircleOutlined, ShoppingCartOutlined } from "@ant-design/icons";
import { useAuth } from "./context/AuthContext";

const { Text } = Typography;

function HomeContent() {
  const { modal, message, notification } = App.useApp();
  const { isLoggedIn } = useAuth();

  const [tickets, setTickets] = useState<Ticket[]>([]);
  const [ticketCount, setTicketCount] = useState(0);
  const [isLoading, setIsLoading] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const [searchTrigger, setSearchTrigger] = useState(0);
  const [bookings, setBookings] = useState<TicketData[]>([]);
  const [isDrawerOpen, setIsDrawerOpen] = useState(false);

  //filters
  const [filters, setFilters] = useState<TicketFilters>({
    categoryName: "",
    ticketCode: "",
    ticketName: "",
    maxPrice: "",
    startEventDate: "",
    endEventDate: "",
    orderBy: "ticketCode",
    orderState: "asc",
  });

  // pricing
  const totalPrice = useMemo(() =>
    bookings.reduce((sum, t) => {
      const price = t.Price ?? t.price ?? 0;
      const qty = t.quantity ?? 1;
      return sum + (price * qty);
    }, 0),
    [bookings]);


  // add ticket
  const handleAddTicket = (ticket: TicketData) => {
    if (!isLoggedIn) {
      notification.info({
        title: 'Login Required',
        description: 'You need to be logged in to add tickets.',
        actions: <Button
          type="text"
          size="medium"
          href="/login"
          className="bg-acceloka-blue! border border-acceloka-border!"
          onMouseEnter={(e) => {
            e.currentTarget.style.scale = '1.05';
            e.currentTarget.style.border = '1px solid var(--acceloka-text)';
          }}
          onMouseLeave={(e) => {
            e.currentTarget.style.scale = '1';
            e.currentTarget.style.border = '1px solid var(--acceloka-border)';
          }}
        >Login Now</Button>
      });
      return;
    }
    const ticketId = ticket.ticketCode || ticket.TicketCode;
    const existingItem = bookings.find(item => (item.ticketCode || item.TicketCode) === ticketId);
    const currentQty = existingItem ? (existingItem.quantity || 1) : 0;
    const maxQuota = ticket.quota ?? 999;

    if (currentQty + 1 > maxQuota) {
      message.warning({
        content: `Maximum available for ${ticket.ticketName || ticket.TicketName} is ${maxQuota} units.`,
        className: "text-acceloka-text!"
      });
      return;
    }

    setBookings(prev => {
      if (existingItem) {
        return prev.map(item =>
          (item.ticketCode || item.TicketCode) === ticketId
            ? { ...item, quantity: currentQty + 1 }
            : item
        );
      }
      return [...prev, { ...ticket, quantity: 1 }];
    });

    message.success(`${ticket.ticketName || ticket.TicketName} added to selection.`);
  };

  const handleCheckout = () => {
    modal.confirm({
      title: <Text strong className="text-acceloka-text">Complete Your Booking</Text>,
      icon: <CheckCircleOutlined className="text-acceloka-blue" />,
      centered: true,
      content: (
        <div className="mt-4">
          <Text className="text-acceloka-muted">Finalizing booking for <b>{bookings.length}</b> ticket types.</Text>
          <div className="mt-4 p-4 bg-acceloka-bg border border-acceloka-border rounded-xl">
            <div className="flex justify-between items-center">
              <Text className="text-acceloka-muted font-medium">Total Amount:</Text>
              <Text strong className="text-lg text-acceloka-blue">IDR {totalPrice.toLocaleString()}</Text>
            </div>
          </div>
        </div>
      ),
      okText: 'Confirm & Checkout',
      okButtonProps: { className: "bg-acceloka-blue h-10 rounded-lg border-none font-bold" },
      cancelButtonProps: { className: "h-10 rounded-lg border-acceloka-border text-acceloka-muted font-bold" },
      async onOk() {
        const hide = message.loading('Processing booking...', 0);
        try {
          const payload = {
            tickets: bookings.map(b => ({
              ticketCode: (b.ticketCode || b.TicketCode) as string,
              quantity: b.quantity || 1
            }))
          };
          const result = await bookingService.bookTickets(payload);
          setBookings([]);
          setIsDrawerOpen(false);
          notification.success({ title: 'Booking Successful', description: result.message });
        } catch (err: any) {
          notification.error({ title: 'Booking Failed', description: err.message });
        } finally {
          hide();
          setSearchTrigger(prev => prev + 1);
        }
      },
    });
  };

  // fetching all tickets
  useEffect(() => {
    const fetchTickets = async () => {
      setIsLoading(true);
      try {
        const response = await bookingService.getAvailableTickets({ ...filters, page: currentPage });
        const data = Array.isArray(response) ? response : response.tickets;
        const count = response.length || response.totalTickets || 0;
        setTickets(data);
        setTicketCount(count);
      } catch (err: any) {
        console.error("Fetch error:", err);
      } finally {
        setIsLoading(false);
      }
    };
    fetchTickets();
  }, [currentPage, searchTrigger]);


  return (
    <div className="min-h-screen bg-acceloka-bg">
      <HeroSection />

      <FilterBar
        filters={filters}
        updateFilter={(key, value) => setFilters({ ...filters, [key]: value })}
        onSearch={() => { setCurrentPage(1); setSearchTrigger(prev => prev + 1); }}
      />

      <main className="px-6 xl:px-20 mx-auto py-8 w-full">
        <TicketGrid
          tickets={tickets}
          isLoading={isLoading}
          onSelectTicket={handleAddTicket}
          onAddTicket={handleAddTicket}
          totalTickets={ticketCount}
          currentPage={currentPage}
          pageSize={10}
          onPageChange={setCurrentPage}
        />
      </main>

      <BookingDrawer
        isOpen={isDrawerOpen}
        onClose={() => setIsDrawerOpen(false)}
        bookings={bookings}
        totalPrice={totalPrice}
        onUpdateQuantity={(code, delta) => {
          const item = bookings.find(i => (i.ticketCode || i.TicketCode) === code);
          if (!item) return;

          const newQty = (item.quantity || 1) + delta;
          const maxQuota = item.quota ?? 999;
          if (delta > 0 && newQty > maxQuota) {
            message.warning({
              content: `Maximum available for this ticket is ${maxQuota}.`,
              className: "text-acceloka-text!"
            });
            return;
          }

          setBookings(prev => prev.map(item => {
            const currentCode = item.ticketCode || item.TicketCode;
            if (currentCode === code) {
              const newQty = (item.quantity || 1) + delta;
              return newQty > 0 ? { ...item, quantity: newQty } : item;
            }
            return item;
          }));
        }}
        onRemoveTicket={(code) => {
          setBookings(prev => prev.filter(i => (i.ticketCode || i.TicketCode) !== code));
          message.info("Ticket removed.");
        }}
        onClearAll={() => setBookings([])}
        onCheckout={handleCheckout}
      />

      {bookings.length > 0 && !isDrawerOpen && (
        <button
          onClick={() => setIsDrawerOpen(true)}
          className="fixed bottom-10 right-6 md:right-10 z-100 flex items-center gap-4 bg-acceloka-blue hover:scale-105 active:scale-95 text-white font-bold py-4 px-6 md:px-8 rounded-2xl shadow-2xl transition-all border-none cursor-pointer"
          style={{ backgroundColor: 'var(--acceloka-blue)' }}
        >
          <div className="bg-white text-acceloka-blue rounded-full w-7 h-7 flex items-center justify-center text-sm font-black">
            {bookings.reduce((sum, b) => sum + (b.quantity || 1), 0)}
          </div>
          <span className="text-white tracking-wide flex items-center gap-2">
            <ShoppingCartOutlined className="text-xl" /> Summary
          </span>
        </button>
      )}
    </div>
  );
}

/**
 * MAIN EXPORT (The Wrapper)
 */
export default function Home() {
  return (
    <ConfigProvider
      theme={{
        algorithm: theme.defaultAlgorithm,
        token: {
          colorPrimary: 'var(--acceloka-blue)',
          colorBgContainer: 'var(--acceloka-surface)',
          colorBgLayout: 'var(--acceloka-bg)',
          colorText: 'var(--acceloka-text)',
          colorTextDescription: 'var(--acceloka-muted)',
          borderRadius: 12,
          fontFamily: 'Inter, sans-serif'
        },
        components: {
          Notification: {
            colorBgElevated: 'var(--acceloka-surface-hover)',
            colorText: 'var(--acceloka-text)',
            colorTextHeading: 'var(--acceloka-text)',
          },
          Message: {
            colorBgElevated: 'var(--acceloka-surface-hover)',
            colorText: 'var(--acceloka-text)',
          },
          Modal: {
            colorBgElevated: 'var(--acceloka-surface-hover)',
            colorBgMask: 'rgba(0, 0, 0, 0.45)',
          }
        }
      }}
    >
      <App>
        <HomeContent />
      </App>
    </ConfigProvider>
  );
}