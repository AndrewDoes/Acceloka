'use client';

import React, { useEffect, useState, useCallback } from 'react';
import {
    Typography,
    Table,
    Tag,
    Space,
    Spin,
    Empty,
    Button,
    InputNumber,
    Card,
    Badge,
    ConfigProvider,
    theme,
    App,
    notification
} from "antd";
import {
    HistoryOutlined,
    ExclamationCircleOutlined,
    CalendarOutlined,
    DeleteOutlined,
    EditOutlined,
    InfoCircleOutlined,
    ShoppingOutlined,
    DownOutlined
} from '@ant-design/icons';
import { useAuth } from '../context/AuthContext';

const { Title, Text } = Typography;

// --- API Service Logic ---
const getBaseUrl = () => {
    if (typeof window === 'undefined') return "http://localhost:5225";
    if ((window as any).NEXT_PUBLIC_API_URL) return (window as any).NEXT_PUBLIC_API_URL;
    const hostname = window.location.hostname;
    return (hostname !== 'localhost' && hostname !== '127.0.0.1')
        ? `http://${hostname}:5225`
        : "http://localhost:5225";
};

const API_BASE_URL = getBaseUrl();

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
        const errorBody = await response.json().catch(() => ({ detail: "An unexpected error occurred." }));
        throw errorBody;
    }
    return response.status === 204 ? {} as T : response.json();
}

const bookingService = {
    getAllBookings: () => apiRequest<BookingSummary[]>('api/v1/get-all-bookings'),
    getBookedTicketDetail: (id: string | number) => apiRequest<TicketsPerCategory[]>(`api/v1/get-booked-ticket/${id}`),
    revokeTicket: (id: string | number, code: string, qty: number) =>
        apiRequest<void>(`api/v1/revoke-ticket/${id}/${code}/${qty}`, { method: 'DELETE' }),
    editBookedTicket: (id: string | number, data: any) =>
        apiRequest<any>(`api/v1/edit-booked-ticket/${id}`, { method: 'PUT', body: JSON.stringify(data) })
};

// --- Interfaces ---
interface BookingSummary {
    bookingId: number;
    bookingDate: string;
    totalPrice: number;
    totalTickets: number;
}

interface BookedTicket {
    ticketCode: string;
    ticketName: string;
    quantity: number;
    eventDate: string;
}

interface TicketsPerCategory {
    qtyPerCategory: number;
    categoryName: string;
    tickets: BookedTicket[];
}

/**
 * CONTENT COMPONENT
 */
function MyBookingsContent() {
    const { modal, message, notification } = App.useApp();

    const { isLoggedIn } = useAuth();
    const [history, setHistory] = useState<BookingSummary[]>([]);
    const [isLoadingHistory, setIsLoadingHistory] = useState(false);
    const [detailsCache, setDetailsCache] = useState<Record<number, TicketsPerCategory[]>>({});
    const [loadingDetails, setLoadingDetails] = useState<Record<number, boolean>>({});
    const [expandedRowKeys, setExpandedRowKeys] = useState<number[]>([]);

    const fetchHistory = useCallback(async () => {
        if (!isLoggedIn) return;
        setIsLoadingHistory(true);
        try {
            const data = await bookingService.getAllBookings();
            setHistory(data || []);
        } catch (err: any) {
            message.error(err.detail || "History unavailable");
        } finally {
            setIsLoadingHistory(false);
        }
    }, [isLoggedIn, message]);

    useEffect(() => { fetchHistory(); }, [fetchHistory]);

    const fetchDetails = async (id: number, force = false) => {
        if (loadingDetails[id] || (!force && detailsCache[id])) return;
        setLoadingDetails(prev => ({ ...prev, [id]: true }));
        try {
            const data = await bookingService.getBookedTicketDetail(id);
            setDetailsCache(prev => ({ ...prev, [id]: data }));
        } catch (err: any) {
            message.error(`Details for #${id} failed`);
        } finally {
            setLoadingDetails(prev => ({ ...prev, [id]: false }));
        }
    };

    const handleRevoke = (bookingId: number, ticket: BookedTicket) => {
        let revokeQty = 1;
        modal.confirm({
            title: <Text strong className="text-lg text-acceloka-text!">Revoke Ticket</Text>,
            centered: true,
            icon: <ExclamationCircleOutlined className="text-acceloka-danger!" />,
            okText: 'Confirm Revoke',
            okButtonProps: { danger: true, className: "rounded-lg h-10 font-bold bg-acceloka-danger! button-text" },
            cancelButtonProps: { className: "rounded-lg h-10 font-bold bg-acceloka-surface! text-acceloka-text!" },
            content: (
                <div className="pt-4 flex flex-col gap-4">
                    <Text className="text-acceloka-muted!">How many units of <strong>{ticket.ticketName}</strong> to revoke?<br /> max: {ticket.quantity}</Text>
                    <InputNumber
                        min={1} max={ticket.quantity} defaultValue={1}
                        className="w-full h-11 flex items-center rounded-lg bg-acceloka-surface! text-acceloka-text!"
                        onChange={(val: any) => {
                            if (val > ticket.quantity) {
                                val = ticket.quantity;
                            }
                            if (val) revokeQty = val;
                        }}
                    />
                </div>
            ),
            async onOk() {
                try {
                    await bookingService.revokeTicket(bookingId, ticket.ticketCode, revokeQty);
                    message.success("Revoked successfully");
                    setDetailsCache(prev => { const next = { ...prev }; delete next[bookingId]; return next; });
                    if (ticket.quantity === revokeQty) { setExpandedRowKeys([]); fetchHistory(); }
                    else await Promise.all([fetchDetails(bookingId, true), fetchHistory()]);
                } catch (err: any) { message.error("Action failed"); }
            }
        });
    };

    const handleEdit = (bookingId: number, ticket: BookedTicket) => {
        let newQty = ticket.quantity;
        modal.confirm({
            title: <Text strong className="text-lg text-acceloka-text!">Adjust Quantity</Text>,
            centered: true,
            icon: <EditOutlined className="text-acceloka-blue!" />,
            okText: 'Update',
            okButtonProps: { className: "rounded-lg h-10 font-bold bg-acceloka-blue!" },
            cancelButtonProps: { className: "rounded-lg h-10 font-bold bg-acceloka-surface! text-acceloka-text!" },
            content: (
                <div className="pt-4 flex flex-col gap-4">
                    <Text className="text-acceloka-muted!">Set new total quantity:</Text>
                    <InputNumber
                        min={1} defaultValue={ticket.quantity}
                        className="w-full h-11 flex items-center rounded-lg bg-acceloka-surface! text-acceloka-text!"
                        onChange={(val: any) => { if (val) newQty = val; }}
                    />
                </div>
            ),
            async onOk() {
                try {
                    await bookingService.editBookedTicket(bookingId, { tickets: [{ ticketCode: ticket.ticketCode, quantity: newQty }] });
                    message.success("Updated successfully");
                    setDetailsCache(prev => { const next = { ...prev }; delete next[bookingId]; return next; });
                    await Promise.all([fetchDetails(bookingId, true), fetchHistory()]);
                } catch (err: any) { message.error("Action failed"); }
            }
        });
    };

    const expandedRowRender = (record: BookingSummary) => {
        const details = detailsCache[record.bookingId];
        if (loadingDetails[record.bookingId]) return <div className="p-12 text-center bg-acceloka-bg!"><Spin /></div>;
        if (!details) return null;

        return (
            <div className="p-4 md:p-6 flex flex-col gap-8 bg-acceloka-bg! border-l-4 border-acceloka-blue! rounded-r-2xl max-h-125 overflow-y-auto">
                {details.map((cat) => (
                    <div key={cat.categoryName} className="flex flex-col gap-4">
                        <div className="flex items-center justify-between">
                            <Space><Badge color="var(--acceloka-blue)" /><Text strong className="uppercase text-[10px] tracking-widest text-acceloka-blue!">{cat.categoryName}</Text></Space>
                            <Tag className="rounded-full border-none font-bold text-[10px] px-3 py-0.5 bg-acceloka-success-bg! text-acceloka-success!">{cat.qtyPerCategory} Items</Tag>
                        </div>

                        {/* Mobile Cards */}
                        <div className="flex flex-col gap-3 md:hidden">
                            {cat.tickets.map((t) => (
                                <Card key={t.ticketCode} size="small" className="rounded-xl border-acceloka-border! bg-acceloka-surface!">
                                    <div className="flex flex-col gap-4">
                                        <div className="flex justify-between items-start">
                                            <div><Text strong className="block text-acceloka-text! text-sm">{t.ticketName}</Text><Text className="text-[10px] opacity-50 font-mono text-acceloka-muted!">{t.ticketCode}</Text></div>
                                            <Tag className="m-0 font-bold border-none bg-acceloka-bg! text-acceloka-blue!">x{t.quantity}</Tag>
                                        </div>
                                        <div className="grid grid-cols-1 gap-2">
                                            <Button block size="middle" className="bg-acceloka-surface! text-acceloka-text! border-acceloka-border!" icon={<EditOutlined />} onClick={() => handleEdit(record.bookingId, t)}>Edit</Button>
                                            <Button block danger size="middle" className="bg-acceloka-danger! text-white! border-none" icon={<DeleteOutlined />} onClick={() => handleRevoke(record.bookingId, t)}>Revoke</Button>
                                        </div>
                                    </div>
                                </Card>
                            ))}
                        </div>

                        {/* Desktop Table */}
                        <div className="hidden md:block">
                            <Table
                                size="small"
                                pagination={false}
                                dataSource={cat.tickets}
                                rowKey="ticketCode"
                                className="bg-acceloka-surface! rounded-2xl overflow-hidden border border-acceloka-border!"
                                columns={[
                                    {
                                        title: 'Ticket', key: 'info', render: (_: any, r: BookedTicket) => (
                                            <div><Text strong className="block text-acceloka-text!">{r.ticketName}</Text><Text className="text-[10px] opacity-50 font-mono text-acceloka-muted!">{r.ticketCode}</Text></div>
                                        )
                                    },
                                    { title: 'Date', dataIndex: 'eventDate', render: (d: string) => <Text className="text-xs text-acceloka-muted!"><CalendarOutlined className="mr-1 text-acceloka-blue!" />{d}</Text> },
                                    { title: 'Qty', dataIndex: 'quantity', align: 'center', render: (q: number) => <Tag className="font-bold border-none bg-acceloka-bg! text-acceloka-blue! rounded-lg px-3">x{q}</Tag> },
                                    {
                                        title: 'Actions', align: 'right', render: (_: any, t: BookedTicket) => (
                                            <Space>
                                                <Button size="small" type="text" className="font-bold text-acceloka-muted! hover:text-acceloka-text!" icon={<EditOutlined />} onClick={() => handleEdit(record.bookingId, t)}>Edit</Button>
                                                <Button size="small" type="text" danger className="font-bold text-acceloka-danger! hover:bg-acceloka-danger! hover:text-white!" icon={<DeleteOutlined />} onClick={() => handleRevoke(record.bookingId, t)}>Revoke</Button>
                                            </Space>
                                        )
                                    }
                                ]}
                            />
                        </div>
                    </div>
                ))}
            </div>
        );
    };

    const historyColumns = [
        {
            title: 'Booking ID',
            render: (_: any, record: BookingSummary) => <Text strong className="text-acceloka-blue! font-mono">#{record.bookingId}</Text>
        },
        {
            title: 'Date',
            responsive: ['md'] as any,
            render: (_: any, record: BookingSummary) => <Text className="text-acceloka-text!">{new Date(record.bookingDate).toLocaleDateString('en-GB', { day: '2-digit', month: 'long', year: 'numeric' })}</Text>
        },
        {
            title: 'Total',
            align: 'right' as const,
            render: (_: any, record: BookingSummary) => (
                <div className="flex flex-col items-end">
                    <Text strong className="text-acceloka-text!">IDR {record.totalPrice.toLocaleString()}</Text>
                    <div className="flex items-center gap-1 opacity-50"><ShoppingOutlined className="text-[10px] text-acceloka-muted!" /><Text className="text-[10px] font-bold text-acceloka-muted!">{record.totalTickets} ITEMS</Text></div>
                </div>
            )
        }
    ];

    return (
        <div className="w-full max-w-5xl mx-auto py-8 md:py-16 px-4 md:px-8 min-h-screen bg-acceloka-bg!">
            <header className="mb-10 flex items-center gap-5">
                <div className="p-4 bg-acceloka-surface! rounded-2xl border border-acceloka-border! shadow-sm flex items-center justify-center">
                    <HistoryOutlined className="text-acceloka-blue! text-2xl" />
                </div>
                <div>
                    <Title level={2} className="m-0 text-acceloka-text! font-black">My Bookings</Title>
                    <Text className="text-acceloka-muted! text-sm">Review your history and manage active reservations.</Text>
                </div>
            </header>

            <Table
                className="acceloka-history-table shadow-xl border border-acceloka-border! rounded-3xl overflow-hidden bg-acceloka-surface!"
                loading={isLoadingHistory}
                dataSource={history}
                columns={historyColumns}

                rowKey="bookingId"

                pagination={{
                    pageSize: 5,
                    placement: ['bottomCenter'],
                    className: "py-6"
                }}
                expandable={{
                    expandedRowRender,
                    expandedRowKeys,
                    onExpand: (expanded, record) => {
                        setExpandedRowKeys(expanded ? [record.bookingId] : []);
                        if (expanded) fetchDetails(record.bookingId);
                    },
                    expandRowByClick: true,
                    expandIcon: ({ expanded, onExpand, record }) => (
                        <Button
                            type="text"
                            size="small"
                            className="text-acceloka-muted! hover:text-acceloka-blue!"
                            icon={<DownOutlined rotate={expanded ? 180 : 0} />}
                            onClick={e => onExpand(record, e)}
                        />
                    ),
                }}
            />

            <div className="mt-12 p-6 bg-acceloka-surface! border border-acceloka-border! rounded-3xl flex gap-5 items-start shadow-md">
                <InfoCircleOutlined className="text-acceloka-blue! text-xl mt-1" />
                <div>
                    <Text strong className="text-acceloka-text! block">Data Management</Text>
                    <Text className="text-acceloka-muted! text-xs leading-relaxed">
                        Expand a reference to see detailed categories. Modifications to quantities are saved in real-time.
                    </Text>
                </div>
            </div>
        </div>
    );
}

/**
 * ROOT EXPORT
 */
export default function MyBookingsPage() {
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
                    colorBorderSecondary: 'var(--acceloka-border)',
                    borderRadius: 12,
                    fontFamily: 'Inter, sans-serif'
                },
                components: {
                    Table: {
                        headerBg: 'var(--acceloka-surface)',
                        headerColor: 'var(--acceloka-muted)',
                        headerSplitColor: 'transparent',
                        rowHoverBg: 'var(--acceloka-bg)',
                    },

                    Pagination: {
                        itemActiveBg: 'var(--acceloka-blue)',
                        itemBg: 'var(--acceloka-surface)',
                        colorText: 'var(--acceloka-text)',
                    },
                    Modal: {
                        contentBg: 'var(--acceloka-surface)',
                        headerBg: 'var(--acceloka-surface)',
                        colorBgElevated: 'var(--acceloka-surface)',
                        colorText: 'var(--acceloka-text)',
                    },
                    InputNumber: {
                        colorBgContainer: 'var(--acceloka-surface)',
                        colorText: 'var(--acceloka-text)',
                        handleBg: 'var(--acceloka-bg)',
                        handleActiveBg: 'var(--acceloka-text)',
                        colorIcon: 'var(--acceloka-text)',
                        colorIconHover: 'var(--acceloka-text)',
                        handleHoverColor: 'var(--acceloka-text)',
                    },
                    Notification: {
                        colorBgElevated: 'var(--acceloka-surface-hover)',
                        colorText: 'var(--acceloka-text)',
                        colorTextHeading: 'var(--acceloka-text)',
                    },
                    Message: {
                        colorBgElevated: 'var(--acceloka-surface-hover)',
                        colorText: 'var(--acceloka-text)',
                    },
                }
            }}
        >
            <App className="bg-acceloka-bg!">
                <MyBookingsContent />
            </App>
        </ConfigProvider>
    );
}