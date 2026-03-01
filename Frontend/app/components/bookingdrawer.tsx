'use client';

import { Drawer, Button, Empty, Typography, Tag, Space } from 'antd';
import { CloseOutlined, DeleteOutlined, MinusOutlined, PlusOutlined, ShoppingCartOutlined } from '@ant-design/icons';
import { TicketData } from './ticketcard';

const { Text } = Typography;

interface BookingDrawerProps {
    isOpen: boolean;
    onClose: () => void;
    bookings: TicketData[];
    totalPrice: number;
    onUpdateQuantity: (ticketCode: string, delta: number) => void;
    onRemoveTicket: (ticketCode: string) => void;
    onClearAll: () => void;
    onCheckout: () => void;
}

export default function BookingDrawer({
    isOpen,
    onClose,
    bookings,
    totalPrice,
    onUpdateQuantity,
    onRemoveTicket,
    onClearAll,
    onCheckout,
}: BookingDrawerProps) {

    const totalItems = bookings.reduce((sum, b) => sum + (b.quantity || 1), 0);

    return (
        <Drawer
            title={<Text strong className="text-xl text-acceloka-text!">My Selection</Text>}
            onClose={onClose}
            open={isOpen}
            closeIcon={<CloseOutlined style={{ color: 'var(--acceloka-text)' }} />}
            size={typeof window !== 'undefined' && window.innerWidth < 768 ? '100%' : 450}
            styles={{
                body: { backgroundColor: 'var(--acceloka-bg)', padding: '24px' },
                header: { backgroundColor: 'var(--acceloka-surface)', borderBottom: '1px solid var(--acceloka-border)' },
                footer: { backgroundColor: 'var(--acceloka-surface)', borderTop: '1px solid var(--acceloka-border)', padding: 0 },
            }}
            extra={
                <Button type="text" danger onClick={onClearAll} className="font-bold text-acceloka-danger">
                    Clear All
                </Button>
            }
            footer={
                <div className="p-6">
                    <div className="flex justify-between items-center mb-6">
                        <div>
                            <Text className="block text-[10px] uppercase tracking-widest font-bold text-acceloka-muted!">Total to Pay</Text>
                            <Text strong className="text-2xl text-acceloka-blue!">
                                IDR {totalPrice.toLocaleString()}
                            </Text>
                        </div>
                        <Tag style={{ backgroundColor: 'var(--acceloka-bg)', color: 'var(--acceloka-blue)' }} className="mr-0 border-acceloka-border font-bold px-3 py-1 rounded-lg text-acceloka-blue">
                            {totalItems} Tickets
                        </Tag>
                    </div>
                    <Button
                        type="primary"
                        size="large"
                        block
                        disabled={bookings.length === 0}
                        className="h-14 rounded-2xl text-lg font-bold border-none shadow-lg"
                        style={{ backgroundColor: 'var(--acceloka-blue)' }}
                        onClick={onCheckout}
                    >
                        Confirm Booking
                    </Button>
                </div>
            }
        >
            {bookings.length === 0 ? (
                <div className="h-full flex items-center justify-center">
                    <Empty
                        image={Empty.PRESENTED_IMAGE_SIMPLE}
                        description={<Text className="text-acceloka-muted!">Your cart is empty</Text>}
                    />
                </div>
            ) : (
                <div className="flex flex-col gap-4">
                    {bookings.map((item, index) => {
                        const itemCode = item.ticketCode || item.TicketCode || "";
                        const itemPrice = item.Price ?? item.price ?? 0;
                        const itemQty = item.quantity || 1;

                        return (
                            <div
                                key={itemCode || index}
                                className="p-5 rounded-2xl border border-acceloka-border shadow-sm transition-all bg-acceloka-surface"
                            >
                                <div className="flex justify-between items-start mb-4">
                                    <div className="flex-1 pr-4">
                                        <Text strong className="block text-acceloka-text! leading-tight">{item.ticketName || item.TicketName}</Text>
                                        <Text className="text-[10px] font-mono text-acceloka-muted! uppercase tracking-tighter">{itemCode}</Text>
                                    </div>
                                    <Button
                                        type="text"
                                        size="small"
                                        danger
                                        icon={<DeleteOutlined />}
                                        onClick={() => onRemoveTicket(itemCode)}
                                        className="hover:bg-red-50 rounded-lg"
                                    />
                                </div>

                                <div className="flex justify-between items-center">
                                    {/* Quantity Controls */}
                                    <div className="flex items-center gap-3 bg-acceloka-bg border border-acceloka-border rounded-xl p-1.5">
                                        <Button
                                            size="small"
                                            type="text"
                                            className="text-acceloka-text! hover:bg-acceloka-surface rounded-lg"
                                            icon={<MinusOutlined className="text-[10px]" />}
                                            onClick={() => onUpdateQuantity(itemCode, -1)}
                                            disabled={itemQty <= 1}
                                        />
                                        <Text strong className="min-w-5 text-center text-acceloka-text!">{itemQty}</Text>
                                        <Button
                                            size="small"
                                            type="text"
                                            className="text-acceloka-text! hover:bg-acceloka-surface rounded-lg"
                                            icon={<PlusOutlined className="text-[10px]" />}
                                            onClick={() => onUpdateQuantity(itemCode, 1)}
                                        />
                                    </div>

                                    <div className="text-right">
                                        <Text className="block text-[9px] font-bold text-acceloka-muted! uppercase tracking-widest">Subtotal</Text>
                                        <Text strong className="text-acceloka-blue! text-base">
                                            IDR {(itemPrice * itemQty).toLocaleString()}
                                        </Text>
                                    </div>
                                </div>
                            </div>
                        );
                    })}
                </div>
            )}
        </Drawer>
    );
}