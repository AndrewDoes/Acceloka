'use client'
import { App, Button, ConfigProvider, Spin, theme } from "antd";
import { AuthProvider, useAuth } from "./context/AuthContext";
import { useEffect, useMemo, useState } from "react";
import { TicketData } from "./components/ticketcard";
import { Ticket, TicketFilters } from "./types/Ticket";
import { CheckCircleOutlined, PlusCircleOutlined, ShoppingCartOutlined } from "@ant-design/icons";
import Text from "antd/es/typography/Text";
import { bookingService } from "./services/api";
import HeroSection from "./components/hero";
import FilterBar from "./components/filterbar";
import Title from "antd/es/typography/Title";
import TicketGrid from "./components/ticketgrid";
import BookingDrawer from "./components/bookingdrawer";
import AddTicketModal from "./components/addticketmodal";


function HomeContent() {
  const { modal, message, notification } = App.useApp();
  const { isLoggedIn, role } = useAuth();
  const isAdmin = role === 'Admin';

  const [hasMounted, setHasMounted] = useState(false);
  const [tickets, setTickets] = useState<Ticket[]>([]);
  const [ticketCount, setTicketCount] = useState(0);
  const [isLoading, setIsLoading] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const [searchTrigger, setSearchTrigger] = useState(0);
  const [bookings, setBookings] = useState<TicketData[]>([]);
  const [isDrawerOpen, setIsDrawerOpen] = useState(false);
  const [isAdminModalOpen, setIsAdminModalOpen] = useState(false);

  useEffect(() => { setHasMounted(true); }, []);

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

  const totalPrice = useMemo(() =>
    bookings.reduce((sum, t) => {
      const price = t.Price ?? t.price ?? 0;
      const qty = t.quantity ?? 1;
      return sum + (price * qty);
    }, 0),
    [bookings]);


  const handleAddTicket = (ticket: TicketData) => {
    if (!isLoggedIn) {
      notification.info({
        message: <span className="text-acceloka-text font-bold">Login Required</span>,
        description: <span className="text-acceloka-muted">You need to be logged in to add tickets.</span>,
        btn: <Button type="primary" size="small" href="/login" className="bg-acceloka-blue border-none">Login Now</Button>
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
        const hide = message.loading({ content: 'Processing booking...' }, 0);
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

  useEffect(() => {
    if (!hasMounted) return;
    const fetchTickets = async () => {
      setIsLoading(true);
      try {
        const response = await bookingService.getAvailableTickets({ ...filters, page: currentPage });
        const data = Array.isArray(response) ? response : (response.tickets || []);
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
  }, [currentPage, searchTrigger, hasMounted]);

  if (!hasMounted) return <div className="min-h-screen bg-acceloka-bg flex items-center justify-center"><Spin size="large" /></div>;

  return (
    <div className="min-h-screen bg-acceloka-bg!">
      <HeroSection />

      <FilterBar
        filters={filters}
        updateFilter={(key, value) => setFilters({ ...filters, [key as keyof TicketFilters]: value })}
        onSearch={() => { setCurrentPage(1); setSearchTrigger(prev => prev + 1); }}
      />

      <main className="xl:px-20 mx-auto py-8 w-full px-6 md:px-60">
        <div className="flex justify-between items-center mb-10 px-2">
          <div>
            <Title level={2} className="m-0 text-acceloka-text! font-black">Upcoming Events</Title>
            <Text className="text-acceloka-muted!">Discover and book tickets for the best events in town.</Text>
          </div>

          {isAdmin && (
            <Button
              type="primary"
              icon={<PlusCircleOutlined />}
              size="large"
              className="h-12 rounded-xl! bg-acceloka-blue! border-none! font-bold! shadow-lg shadow-blue-500/20"
              onClick={() => setIsAdminModalOpen(true)}
            >
              Add Ticket
            </Button>
          )}
        </div>

        <TicketGrid
          tickets={tickets}
          isLoading={isLoading}
          onAddTicket={handleAddTicket}
          onSelectTicket={handleAddTicket}
          totalTickets={ticketCount}
          currentPage={currentPage}
          onPageChange={setCurrentPage}
        />
      </main>

      <AddTicketModal
        isOpen={isAdminModalOpen}
        onClose={() => setIsAdminModalOpen(false)}
        onRefresh={() => setSearchTrigger(prev => prev + 1)}
      />

      <BookingDrawer
        isOpen={isDrawerOpen}
        onClose={() => setIsDrawerOpen(false)}
        onClearAll={() => setBookings([])}
        bookings={bookings}
        totalPrice={totalPrice}
        onUpdateQuantity={(code: string, delta: number) => {
          setBookings(prev => prev.map(item => {
            if ((item.ticketCode || item.TicketCode) === code) {
              const newQty = (item.quantity || 1) + delta;
              return newQty > 0 ? { ...item, quantity: newQty } : item;
            }
            return item;
          }));
        }}
        onRemoveTicket={(code: string) => {
          setBookings(prev => prev.filter(i => (i.ticketCode || i.TicketCode) !== code));
          message.info("Ticket removed.");
        }}
        onCheckout={handleCheckout}
      />

      {bookings.length > 0 && !isDrawerOpen && (
        <button
          onClick={() => setIsDrawerOpen(true)}
          className="fixed bottom-10 right-6 md:right-10 z-[100] flex items-center gap-4 bg-acceloka-blue hover:scale-105 active:scale-95 text-white font-bold py-4 px-6 md:px-8 rounded-2xl shadow-2xl transition-all border-none cursor-pointer"
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

// --- 7. THEME PROVIDER & DYNAMIC LOGIC ---
export default function Home() {
  const [isDarkMode, setIsDarkMode] = useState(false);

  useEffect(() => {
    const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
    setIsDarkMode(mediaQuery.matches);
    const handler = (e: MediaQueryListEvent) => setIsDarkMode(e.matches);
    mediaQuery.addEventListener('change', handler);
    return () => mediaQuery.removeEventListener('change', handler);
  }, []);

  return (
    <ConfigProvider
      theme={{
        algorithm: isDarkMode ? theme.darkAlgorithm : theme.defaultAlgorithm,
        token: {
          colorPrimary: 'var(--acceloka-blue)',
          colorBgContainer: 'var(--acceloka-surface)',
          colorBgLayout: 'var(--acceloka-bg)',
          colorText: 'var(--acceloka-text)',
          colorTextDescription: 'var(--acceloka-muted)',
          colorTextPlaceholder: 'rgba(148, 163, 184, 0.75)',
          borderRadius: 12,
          fontFamily: 'Inter, sans-serif'
        },
        components: {
          Input: {
            colorBgContainer: 'var(--acceloka-bg)',
            colorBorder: 'var(--acceloka-border)',
            colorTextPlaceholder: 'rgba(148, 163, 184, 0.75)',
          },
          Select: {
            colorBgContainer: 'var(--acceloka-bg)',
            colorBorder: 'var(--acceloka-border)',
            colorTextPlaceholder: 'rgba(148, 163, 184, 0.75)',
          },
          DatePicker: {
            colorBgContainer: 'var(--acceloka-bg)',
            colorBorder: 'var(--acceloka-border)',
            colorTextPlaceholder: 'rgba(148, 163, 184, 0.75)',
          },
          Modal: {
            colorBgElevated: 'var(--acceloka-surface-hover)',
            colorBgMask: 'rgba(0, 0, 0, 0.65)',
          }
        }
      }}
    >
      <App className="bg-acceloka-bg!">
        <AuthProvider>
          <HomeContent />
        </AuthProvider>
      </App>
    </ConfigProvider>
  );
}