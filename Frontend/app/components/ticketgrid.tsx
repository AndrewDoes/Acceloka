import { useEffect, useMemo, useRef, useState } from "react";
import TicketCard, { TicketData } from "./ticketcard";
import { Button, Empty, Pagination, Spin } from "antd";
import Text from "antd/es/typography/Text";
import { ArrowRightOutlined } from "@ant-design/icons";

interface TicketGridProps {
    tickets: TicketData[];
    totalTickets?: number;
    currentPage?: number;
    pageSize?: number;
    isLoading: boolean;
    onSelectTicket: (ticket: TicketData) => void;
    onAddTicket: (ticket: TicketData) => void;
    onPageChange: (page: number) => void;
}

export default function TicketGrid({
    tickets,
    totalTickets,
    currentPage = 1,
    pageSize = 10,
    isLoading,
    onSelectTicket,
    onAddTicket,
    onPageChange
}: TicketGridProps) {
    const [isMounted, setIsMounted] = useState(false);
    const stableTotal = useRef<number>(0);

    useEffect(() => {
        setIsMounted(true);
    }, []);

    const { displayTickets, finalTotalCount } = useMemo(() => {
        const allItems = Array.isArray(tickets) ? tickets : (tickets as any)?.tickets || [];
        const embeddedTotal = allItems[0]?.totalTickets;
        const currentCount = totalTickets || embeddedTotal || 0;

        if (currentCount > 0) {
            stableTotal.current = currentCount;
        }

        return {
            displayTickets: allItems,
            finalTotalCount: stableTotal.current || currentCount
        };
    }, [tickets, totalTickets]);

    if (!isMounted || isLoading) {
        return (
            <div className="flex flex-col justify-center items-center py-32 gap-4">
                <Spin size="large" />
                <Text type="secondary" strong>Loading events...</Text>
            </div>
        );
    }

    // Empty State
    if (displayTickets.length === 0) {
        return (
            <div className="py-20 bg-white text-center rounded-xl border border-dashed border-slate-200">
                <Empty description={`No items found on Page ${currentPage}`} />
                {currentPage > 1 && (
                    <Button onClick={() => onPageChange(1)} className="mt-4">
                        Back to Page 1
                    </Button>
                )}
            </div>
        );
    }

    return (
        <div className="w-full space-y-12">
            {/* Results Header */}
            <div className="flex justify-between items-center px-2">
                <Text type="secondary">
                    Total found: <Text strong>{finalTotalCount}</Text> tickets
                </Text>
            </div>

            {/* Grid Layout - 5 columns for desktop as per your previous code */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 xl:grid-cols-5 gap-6">
                {displayTickets.map((ticket: TicketData, idx: number) => {
                    const key = ticket.ticketCode || ticket.TicketCode || `t-${currentPage}-${idx}`;
                    return (
                        <div key={key} className="h-full">
                            <TicketCard
                                ticket={ticket}
                                onSelect={onSelectTicket}
                                onAdd={onAddTicket}
                            />
                        </div>
                    );
                })}
            </div>

            {/* Pagination Controls */}
            {finalTotalCount > pageSize && (
                <div className="flex flex-col items-center gap-4 py-8 border-t border-slate-100 mt-8">
                    <Pagination
                        onChange={onPageChange}
                        current={currentPage}
                        total={finalTotalCount}
                        pageSize={pageSize}
                        showSizeChanger={false}
                        className="bg-white p-4 rounded-xl shadow-sm border border-slate-100"
                    />
                    <Text type="secondary" className="text-xs">
                        Page {currentPage} of {Math.ceil(finalTotalCount / pageSize)}
                    </Text>
                </div>
            )}
        </div>
    );
}